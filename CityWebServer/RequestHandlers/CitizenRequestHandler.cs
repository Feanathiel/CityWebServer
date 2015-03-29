using System;
using System.Collections.Generic;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using CityWebServer.Services;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    /// <summary>
    /// Handles citizen specific information.
    /// </summary>
    internal class CitizenRequestHandler : RequestHandlerBase
    {
        private readonly List<Func<IRequestParameters, IResponseFormatter>> _pathHandlers;

        private readonly GameService _gameService;
        private readonly CitizenService _citizenService;

        /// <summary>
        /// Gets a unique identifier for this handler.
        /// </summary>
        public override Guid HandlerID
        {
            get { return new Guid("E0A1510B-0910-422F-B67A-DD2AE80EC950"); }
        }

        /// <summary>
        /// Gets the priority of this request handler.
        /// </summary>
        public override int Priority
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets the display name of this request handler.
        /// </summary>
        public override String Name
        {
            get { return "Citizens"; }
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
            get { return "/Api/Citizen/"; }
        }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public override bool ShouldHandle(IRequestParameters request)
        {
            return request.Url.AbsolutePath.StartsWith(MainPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CitizenRequestHandler"/>.
        /// </summary>
        [UsedImplicitly]
        public CitizenRequestHandler()
        {
            _pathHandlers = new List<Func<IRequestParameters, IResponseFormatter>>
            {
                HandleAgeGroupDistribution,
                HandleBirthAndDateRate,
                HandleEducationEmployment
            };

            _gameService = new GameService();
            _citizenService = new CitizenService();
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

            return NotFoundResponse();
        }

        /// <summary>
        /// Gets the age distribution of the currently loaded city.
        /// </summary>
        private IResponseFormatter HandleAgeGroupDistribution(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "Age.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            DateTime gameTime = _gameService.GetGameTime();
            AgeDistribution distribution = _citizenService.GetAgeGroupPopulation();

            var result = new
            {
                GameTime = gameTime,
                Distribution = distribution
            };

            return JsonResponse(result);
        }

        private IResponseFormatter HandleBirthAndDateRate(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "BirthAndDeath.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            DateTime gameTime = _gameService.GetGameTime();
            BirthAndDeathRate rate = _citizenService.GetBirthAndDeathRate();

            var result = new
            {
                GameTime = gameTime,
                Rate = rate
            };

            return JsonResponse(result);
        }

        private IResponseFormatter HandleEducationEmployment(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "EducationEmployment.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            DateTime gameTime = _gameService.GetGameTime();
            EducationEmployment educationEmployment = _citizenService.GetEducationEmploymentRate();

            var result = new
            {
                GameTime = gameTime,
                EducationEmployment = educationEmployment
            };

            return JsonResponse(result);
            
        }

        /// <summary>
        /// Creates a not-found response.
        /// </summary>
        private IResponseFormatter NotFoundResponse()
        {
            return PlainTextResponse(String.Empty, HttpStatusCode.NotFound);
        }
    }
}
