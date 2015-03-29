using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly GameService _gameService;
        private readonly CityInfoService _cityInfoService;

        /// <summary>
        /// Gets a unique identifier for this handler.
        /// </summary>
        public override Guid HandlerID
        {
            get { return new Guid("eeada0d0-f1d2-43b0-9595-2a6a4d917631"); }
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
            get { return "Districts"; }
        }

        /// <summary>
        /// Gets the author of this request handler.
        /// </summary>
        public override String Author
        {
            get { return "Rychard"; }
        }

        /// <summary>
        /// Gets the absolute path to the main page for this request handler.
        /// </summary>
        public override String MainPath
        {
            get { return "/Api/Districts/"; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DistrictsRequestHandler"/>.
        /// </summary>
        [UsedImplicitly]
        public DistrictsRequestHandler()
        {
            _gameService = new GameService();
            _cityInfoService = new CityInfoService();
        }

        /// <summary>
        /// Returns a value that indicates whether this handler is capable of servicing the given request.
        /// </summary>
        public override Boolean ShouldHandle(IRequestParameters request)
        {
            return (request.Url.AbsolutePath.Equals(MainPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        public override IResponseFormatter Handle(IRequestParameters request)
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
    }
}