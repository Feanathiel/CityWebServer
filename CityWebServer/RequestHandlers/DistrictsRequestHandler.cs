using System;
using System.Collections.Generic;
using System.Linq;
using CityWebServer.Extensibility;
using CityWebServer.Models;
using CityWebServer.Services;

namespace CityWebServer.RequestHandlers
{
    public class DistrictsRequestHandler : RequestHandlerBase
    {
        private readonly GameService _gameService;
        private readonly CityInfoService _cityInfoService;

        public override Guid HandlerID
        {
            get { return new Guid("eeada0d0-f1d2-43b0-9595-2a6a4d917631"); }
        }

        public override int Priority
        {
            get { return 0; }
        }

        public override String Name
        {
            get { return "Districts"; }
        }

        public override String Author
        {
            get { return "Rychard"; }
        }

        public override String MainPath
        {
            get { return "/Api/Districts/"; }
        }

        public DistrictsRequestHandler()
        {
            _gameService = new GameService();
            _cityInfoService = new CityInfoService();
        }

        public override Boolean ShouldHandle(IRequestParameters request)
        {
            return (request.Url.AbsolutePath.Equals("/Api/Districts/Districts.json", StringComparison.OrdinalIgnoreCase));
        }

        public override IResponseFormatter Handle(IRequestParameters request)
        {
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