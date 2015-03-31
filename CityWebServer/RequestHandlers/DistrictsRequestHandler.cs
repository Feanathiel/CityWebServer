using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using CityWebServer.Services;
using JetBrains.Annotations;

namespace CityWebServer.RequestHandlers
{
    /// <summary>
    /// Handles district based information.
    /// </summary>
    public class DistrictsRequestHandler : RequestHandlerBase
    {
        private readonly List<Func<IRequestParameters, IResponseFormatter>> _pathHandlers;

        private readonly GameService _gameService;
        private readonly CityInfoService _cityInfoService;

        /// <summary>
        /// Creates a new instance of the <see cref="DistrictsRequestHandler"/>.
        /// </summary>
        [UsedImplicitly]
        public DistrictsRequestHandler(IWebServer server)
			: base(server, new Guid("eeada0d0-f1d2-43b0-9595-2a6a4d917631"), "Districts", "Rychard", 0, "/Api/Districts/")
        {
            _pathHandlers = new List<Func<IRequestParameters, IResponseFormatter>>
            {
                HandleDistricts,
                HandleCarReasons
            };

            _gameService = new GameService();
            _cityInfoService = new CityInfoService();
        }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public override Boolean ShouldHandle(IRequestParameters request)
        {
            return (request.Url.AbsolutePath.StartsWith(MainPath, StringComparison.OrdinalIgnoreCase));
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

            return NotFoundResponse();
        }

        private IResponseFormatter HandleDistricts(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "Districts.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var districtIDs = _cityInfoService.GetNonCityDistricts();

            List<DistrictInfo> districtInfoList = new List<DistrictInfo>();

            var buildings = _cityInfoService.GetBuildingBreakdownByDistrict();
            var vehicles = _cityInfoService.GetVehicleBreakdownByDistrict();

            foreach (var districtId in districtIDs)
            {
                DistrictInfo districtInfo = _cityInfoService.GetDistrictInfo(districtId);
                districtInfo.TotalBuildingCount = buildings.Where(obj => obj.Key == districtId).Sum(obj => obj.Value);
                districtInfo.TotalVehicleCount = vehicles.Where(obj => obj.Key == districtId).Sum(obj => obj.Value);
                
                districtInfoList.Add(districtInfo);
            }

            const int globalDistrictId = 0;

            DistrictInfo globalDistrictInfo = _cityInfoService.GetDistrictInfo(globalDistrictId);
            globalDistrictInfo.TotalBuildingCount = buildings.Where(obj => obj.Key == globalDistrictId).Sum(obj => obj.Value);
            globalDistrictInfo.TotalVehicleCount = vehicles.Where(obj => obj.Key == globalDistrictId).Sum(obj => obj.Value);

            var cityInfo = new CityInfo
            {
                Name = _gameService.GetCityName(),
                Time = _gameService.GetGameTime(),
                GlobalDistrict = globalDistrictInfo,
                Districts = districtInfoList.ToArray(),
            };

            return JsonResponse(cityInfo);
        }

        private IResponseFormatter HandleCarReasons(IRequestParameters request)
        {
            if (!request.Url.AbsolutePath.Equals(MainPath + "CarReasons.json", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            IEnumerable<CarReasonCategory> grouped = _cityInfoService.GetCarReasons();

            var data = new CarReasons
            {
                Categories = grouped.ToList()
            };

            return JsonResponse(data);
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