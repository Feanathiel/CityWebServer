using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using ApacheMimeTypes;
using CityWebServer.Extensibility;
using CityWebServer.Extensibility.ResponseFormatters;
using CityWebServer.Helpers;
using CityWebServer.Services;
using ColossalFramework.Plugins;
using ICities;
using JetBrains.Annotations;

namespace CityWebServer
{
    [UsedImplicitly]
    public class IntegratedWebServer : ThreadingExtensionBase
    {
        private const String WebServerPortKey = "webServerPort";
        private static readonly Type RequestHandlerType = typeof(IRequestHandler);

        private readonly LogService _logService;
        private static String _endpoint;

        private WebServer _server;
        private readonly SortedList<IRequestHandler, IRequestHandler> _requestHandlers;

        // Not required, but prevents a number of spurious entries from making it to the log file.
        private static readonly List<String> IgnoredAssemblies = new List<String>
        {
            "Anonymously Hosted DynamicMethods Assembly",
            "Assembly-CSharp",
            "Assembly-CSharp-firstpass",
            "Assembly-UnityScript-firstpass",
            "Boo.Lang",
            "ColossalManaged",
            "ICSharpCode.SharpZipLib",
            "ICities",
            "Mono.Security",
            "mscorlib",
            "System",
            "System.Configuration",
            "System.Core",
            "System.Xml",
            "UnityEngine",
            "UnityEngine.UI",
        };

        /// <summary>
        /// Gets the root endpoint for which the server is configured to service HTTP requests.
        /// </summary>
        public static String Endpoint
        {
            get { return _endpoint; }
        }

        /// <summary>
        /// Gets the full path to the directory where static pages are served from.
        /// </summary>
        public static String GetWebRoot()
        {
            var modPaths = PluginManager.instance.GetPluginsInfo().Select(obj => obj.modPath);
            foreach (var path in modPaths)
            {
                var testPath = Path.Combine(path, "wwwroot");
                
                if (Directory.Exists(testPath))
                {
                    return testPath;
                }
            }
            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegratedWebServer"/> class.
        /// </summary>
        public IntegratedWebServer()
        {
            _logService = LogService.Instance;

            // We need a place to store all the request handlers that have been registered.
            var requestHandlerComparer = new RequestHandlerPriorityComparer();
            _requestHandlers = new SortedList<IRequestHandler, IRequestHandler>(requestHandlerComparer);
        }

        #region Create

        /// <summary>
        /// Called by the game after this instance is created.
        /// </summary>
        /// <param name="threading">The threading.</param>
        public override void OnCreated(IThreading threading)
        {
            this.InitializeServer();
            RegisterAssemblyLoader();
            RegisterHandlers();

            base.OnCreated(threading);
        }

        private void RegisterAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
            AppDomain.CurrentDomain.DomainUnload += OnAppDomainUnload;
        }

        private void OnAppDomainUnload(object sender, EventArgs e)
        {
            // TODO: Unload handlers
            _requestHandlers.Clear();
        }

        private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;

            List<Type> handlers = new List<Type>();
            AppendPotentialHandlers(assembly, handlers);
            RegisterHandlers(handlers);

        }

        private void InitializeServer()
        {
            if (_server != null)
            {
                _server.Stop();
                _server = null;
            }

            LogMessage("Initializing Server...");
            
            // I'm not sure how I feel about making the port registration configurable.
            // Honestly, it sort of defeats the purpose, since other mods could potentially expect it to exist on a specific port.

            int port = 8080;
            if (Configuration.HasSetting(WebServerPortKey))
            {
                port = Configuration.GetInt(WebServerPortKey);
            }
            else
            {
                Configuration.SetInt(WebServerPortKey, port);
                Configuration.SaveSettings();
            }

            String endpoint = String.Format("http://localhost:{0}/", port);
            _endpoint = endpoint;

            WebServer ws = new WebServer(HandleRequest, endpoint);
            _server = ws;
            _server.Run();
            LogMessage("Server Initialized.");
        }

        #endregion Create

        #region Release

        /// <summary>
        /// Called by the game before this instance is about to be destroyed.
        /// </summary>
        public override void OnReleased()
        {
            ReleaseServer();

            // TODO: Unregister from events (i.e. ILogAppender.LogMessage)
            _requestHandlers.Clear();

            Configuration.SaveSettings();

            base.OnReleased();
        }

        private void ReleaseServer()
        {
            LogMessage("Checking for existing server...");
            if (_server != null)
            {
                LogMessage("Server found; disposing...");
                _server.Stop();
                _server = null;
                LogMessage("Server Disposed.");
            }
        }

        #endregion Release

        /// <summary>;
        /// Handles the specified request.
        /// </summary>
        /// <remarks>
        /// Defers execution to an appropriate request handler, except for requests to the reserved endpoints: <c>~/</c> and <c>~/Log</c>.<br />
        /// Returns a default error message if an appropriate request handler can not be found.
        /// </remarks>
        private void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            LogMessage(String.Format("{0} {1}", request.HttpMethod, request.RawUrl));

            // There are two reserved endpoints: "/" and "/Log".
            // These take precedence over all other request handlers.
            if (ServiceRoot(request, response))
            {
                return;
            }

            IRequestParameters requestParameters = new RequestParameters(request);

            // Get the request handler associated with the current request.

            var handler = _requestHandlers.Values.FirstOrDefault(obj => obj.ShouldHandle(requestParameters));
            if (handler != null)
            {
                try
                {
                    IResponseFormatter responseFormatterWriter = handler.Handle(requestParameters);

                    if (responseFormatterWriter == null)
                    {
                        responseFormatterWriter = new PlainTextResponseFormatter(String.Empty, HttpStatusCode.NotFound);
                    }
                    
                    responseFormatterWriter.WriteContent(response);

                    return;
                }
                catch (Exception ex)
                {
                    _logService.LogMessage(ex.ToString());

                    IResponseFormatter errorResponseFormatter = new PlainTextResponseFormatter(
                        String.Empty, 
                        HttpStatusCode.InternalServerError);

                    errorResponseFormatter.WriteContent(response);

                    return;
                }
            }

            var wwwroot = GetWebRoot();
            
            // At this point, we can guarantee that we don't need any game data, so we can safely start a new thread to perform the remaining tasks.
            ServiceFileRequest(wwwroot, request, response);
        }

        private static void ServiceFileRequest(String wwwroot, HttpListenerRequest request, HttpListenerResponse response)
        {
            var relativePath = request.Url.AbsolutePath.Substring(1);
            relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            var absolutePath = Path.Combine(wwwroot, relativePath);

            if (File.Exists(absolutePath))
            {
                var extension = Path.GetExtension(absolutePath);
                response.ContentType = Apache.GetMime(extension);
                response.StatusCode = 200; // HTTP 200 - SUCCESS

                // Open file, read bytes into buffer and write them to the output stream.
                using (FileStream fileReader = File.OpenRead(absolutePath))
                {
                    byte[] buffer = new byte[4096];
                    int read;
                    while ((read = fileReader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.OutputStream.Write(buffer, 0, read);
                    }
                }
            }
            else
            {
                String body = String.Format("No resource is available at the specified filepath: {0}", absolutePath);

                IResponseFormatter notFoundResponseFormatter = new PlainTextResponseFormatter(body, HttpStatusCode.NotFound);
                notFoundResponseFormatter.WriteContent(response);
            }
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain for class definitions that implement the <see cref="IRequestHandler"/> interface.  Those classes are instantiated and registered as request handlers.
        /// </summary>
        private void RegisterHandlers()
        {
            IEnumerable<Type> handlers = FindHandlersInLoadedAssemblies();
            RegisterHandlers(handlers);
        }

        private void RegisterHandlers(IEnumerable<Type> handlers)
        {
            foreach (var handler in handlers)
            {
                // Only register handlers that we don't already have an instance of.
                if (_requestHandlers.Any(h => h.GetType() == handler))
                {
                    continue;
                }

                IRequestHandler handlerInstance = null;
                Boolean exists = false;

                try
                {
                    handlerInstance = (IRequestHandler) Activator.CreateInstance(handler);

                    // Duplicates handlers seem to pass the check above, so now we filter them based on their identifier values, which should work.
                    exists = _requestHandlers.Values.Any(obj => obj.HandlerID == handlerInstance.HandlerID);
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                }

                if (exists)
                {
                    // TODO: Allow duplicate registrations to occur; previous registration is removed and replaced with a new one?
                    LogMessage(String.Format("Supressing duplicate handler registration for '{0}'", handler.Name));
                }
                else
                {
                    _requestHandlers.Add(handlerInstance, handlerInstance);
                    if (handlerInstance is ILogAppender)
                    {
                        var logAppender = (handlerInstance as ILogAppender);
                        logAppender.LogMessage += RequestHandlerLogAppender_OnLogMessage;
                    }

                    LogMessage(String.Format("Added Request Handler: {0}", handler.FullName));
                }
            }
        }

        private void RequestHandlerLogAppender_OnLogMessage(object sender, LogAppenderEventArgs logAppenderEventArgs)
        {
            var senderTypeName = sender.GetType().Name;
            LogMessage(logAppenderEventArgs.LogLine, senderTypeName, false);
        }

        /// <summary>
        /// Searches all the assemblies in the current AppDomain, and returns a collection of those that implement the <see cref="IRequestHandler"/> interface.
        /// </summary>
        private IEnumerable<Type> FindHandlersInLoadedAssemblies()
        {
            List<Type> handlers = new List<Type>();
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                AppendPotentialHandlers(assembly, handlers);
            }

            return handlers;
        }

        private void AppendPotentialHandlers(Assembly assembly, ICollection<Type> handlers)
        {
            var assemblyName = assembly.GetName().Name;

            // Skip any assemblies that we don't anticipate finding anything in.
            if (IgnoredAssemblies.Contains(assemblyName))
            {
                return;
            }

            int typeCount = 0;
            try
            {
                var types = assembly.GetTypes().ToList();
                typeCount = types.Count;
                foreach (var type in types)
                {
                    if (RequestHandlerType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                    {
                        handlers.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }

            var message = String.Format(
                "Found {0} types in {1}, of which {2} were potential request handlers.", 
                typeCount,
                assembly.GetName().Name,
                handlers.Count);

            LogMessage(message);
        }

        #region Reserved Endpoint Handlers

        /// <summary>
        /// Services requests to <c>~/</c>
        /// </summary>
        private static Boolean ServiceRoot(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.Url.AbsolutePath.ToLower() == "/")
            {
                const String newUrl = "/index.html";
                IResponseFormatter redirectFormatter = new RedirectResponseFormatter(newUrl);
                redirectFormatter.WriteContent(response);
                
                return true;
            }

            return false;
        }
        
        #endregion Reserved Endpoint Handlers

        #region Logging

        /// <summary>
        /// Adds a timestamp to the specified message, and appends it to the internal log.
        /// </summary>
        public void LogMessage(String message, String label = null, Boolean showInDebugPanel = false)
        {
            var dt = DateTime.Now;
            String time = String.Format("{0} {1}", dt.ToShortDateString(), dt.ToShortTimeString());
            String messageWithLabel = String.IsNullOrEmpty(label) ? message : String.Format("{0}: {1}", label, message);
            String line = String.Format("[{0}] {1}{2}", time, messageWithLabel, Environment.NewLine);

            _logService.LogMessage(line);

            if (showInDebugPanel)
            {
                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, line);
            }
        }

        #endregion Logging
    }
}