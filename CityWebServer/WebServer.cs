using System;
using System.Net;
using System.Threading;

namespace CityWebServer
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Action<HttpListenerRequest, HttpListenerResponse> _responderMethod;

        public WebServer(Action<HttpListenerRequest, HttpListenerResponse> method, String prefix)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("This wouldn't happen if you upgraded your operating system more than once a decade.");
            }

            // URI prefix is required, for example:
            // "http://localhost:8080/index/".
            if (String.IsNullOrEmpty(prefix))
            {
                throw new ArgumentNullException("prefix");
            }

            // A responder method is required
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            _listener.Prefixes.Add(prefix);

            _responderMethod = method;
            _listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(RequestHandlerCallback, _listener.GetContext());
                    }
                }
                catch
                {
                    // Suppress exceptions.
                }
            });
        }

        private void RequestHandlerCallback(Object context)
        {
            var ctx = context as HttpListenerContext;

            if (ctx == null)
            {
                return;
            }

            try
            {
                var request = ctx.Request;
                var response = ctx.Response;

                // Allow accessing pages from pages hosted from another local web-server, such as IIS, for instance.
                response.AddHeader("Access-Control-Allow-Origin", "http://localhost");

                this._responderMethod(request, response);
            }
            catch
            {
                // Suppress any exceptions.
            }
            finally
            {
                // Ensure that the stream is never left open.
                ctx.Response.OutputStream.Close();
            }
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}