using System;
using System.Collections.Generic;
using CityWebServer.Extensibility;
using CityWebServer.Services;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    /// <summary>
    /// Handles core server specific requests.
    /// </summary>
    internal class ServerRequestHandler : RequestHandlerBase
    {
        private readonly List<Func<IRequestParameters, IResponseFormatter>> _pathHandlers;

        private readonly LogService _logService;
        private readonly GameService _gameService;

        /// <summary>
        /// Creates a new instance of the <see cref="ServerRequestHandler"/>.
        /// </summary>
        [UsedImplicitly]
        public ServerRequestHandler(IWebServer server)
            : base(server, new Guid("4E3ADFBB-FB73-4C78-8223-E19F7DD1123B"), "Base", "Feanathiel", int.MaxValue, "/Api/Server/")
        {
            _pathHandlers = new List<Func<IRequestParameters, IResponseFormatter>>
            {
                HandleLogLines,
                HandleCityName
            };

            _logService = LogService.Instance;
            _gameService = new GameService();
        }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public override bool ShouldHandle(IRequestParameters request)
        {
            return request.Url.AbsolutePath.StartsWith(MainPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        public override IResponseFormatter Handle(IRequestParameters request)
        {
            foreach (var pathHandler in _pathHandlers)
            {
                IResponseFormatter result = pathHandler(request);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Handles the log lines request.
        /// </summary>
        private IResponseFormatter HandleLogLines(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "LogLines.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            IEnumerable<String> logLines = _logService.GetLogLines();

            return JsonResponse(logLines);
        }

        /// <summary>
        /// Handles the city name request.
        /// </summary>
        private IResponseFormatter HandleCityName(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "CityName.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            String cityName = _gameService.GetCityName();

            var data = new
            {
                CityName = cityName
            };

            return JsonResponse(data);
        }
    }
}
