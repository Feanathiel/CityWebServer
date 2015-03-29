using System;
using System.Collections.Generic;
using CityWebServer.Extensibility;
using CityWebServer.Services;

namespace CityWebServer.RequestHandlers
{
    internal class ServerRequestHandler : RequestHandlerBase
    {
        private readonly List<Func<IRequestParameters, IResponseFormatter>> _pathHandlers;

        private readonly GameService _gameService;

        /// <summary>
        /// Gets a unique identifier for this handler.
        /// </summary>
        public override Guid HandlerID
        {
            get { return new Guid("4E3ADFBB-FB73-4C78-8223-E19F7DD1123B"); }
        }

        /// <summary>
        /// Gets the priority of this request handler.
        /// </summary>
        public override int Priority
        {
            get { return 1000; }
        }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        public override String Name
        {
            get { return "Base"; }
        }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        public override String Author
        {
            get { return "Feanathiel"; }
        }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.
        /// </summary>
        public override String MainPath
        {
            get { return "/Api/Server/"; }
        }

        public ServerRequestHandler()
        {
            _pathHandlers = new List<Func<IRequestParameters, IResponseFormatter>>
            {
                HandleCityName
            };

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
        /// Handles the specified request.  The method should not close the stream.
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
