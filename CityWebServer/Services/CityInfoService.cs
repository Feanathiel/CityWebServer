using System;
using System.Collections.Generic;
using CityWebServer.Helpers;
using CityWebServer.Models;
using ColossalFramework;

namespace CityWebServer.Services
{
    internal class CityInfoService
    {
        private readonly DistrictManager _districtManager;
        private readonly VehicleManager _vehicleManager;
        private readonly BuildingManager _buildingManager;

        public CityInfoService()
        {
            _districtManager = Singleton<DistrictManager>.instance;
            _vehicleManager = Singleton<VehicleManager>.instance;
            _buildingManager = Singleton<BuildingManager>.instance;
        }

        public IEnumerable<int> GetNonCityDistricts()
        {
            const int count = DistrictManager.MAX_DISTRICT_COUNT;

            var districts = _districtManager.m_districts.m_buffer;

            for (int i = 1; i < count; i++)
            {
                if (!districts[i].IsAlive())
                {
                    continue;
                }

                yield return i;
            }
        }

        public DistrictInfo GetDistrictInfo(int districtID)
        {
            var district = _districtManager.m_districts.m_buffer[districtID];

            if (!district.IsValid())
            {
                return null;
            }

            String districtName = String.Empty;

            if (districtID == 0)
            {
                // The district with ID 0 is always the global district.
                // It receives an auto-generated name by default, but the game always displays the city name instead.
                districtName = "City";
            }
            else
            {
                districtName = _districtManager.GetDistrictName(districtID);
            }

            var model = new DistrictInfo
            {
                DistrictID = districtID,
                DistrictName = districtName,
                TotalPopulationCount = (int)district.m_populationData.m_finalCount,
                CurrentHouseholds = (int)district.m_residentialData.m_finalAliveCount,
                AvailableHouseholds = (int)district.m_residentialData.m_finalHomeOrWorkCount,
                CurrentJobs = (int)district.m_commercialData.m_finalAliveCount + (int)district.m_industrialData.m_finalAliveCount + (int)district.m_officeData.m_finalAliveCount + (int)district.m_playerData.m_finalAliveCount,
                AvailableJobs = (int)district.m_commercialData.m_finalHomeOrWorkCount + (int)district.m_industrialData.m_finalHomeOrWorkCount + (int)district.m_officeData.m_finalHomeOrWorkCount + (int)district.m_playerData.m_finalHomeOrWorkCount,
                AverageLandValue = district.GetLandValue(),
                WeeklyTouristVisits = (int)district.m_tourist1Data.m_averageCount + (int)district.m_tourist2Data.m_averageCount + (int)district.m_tourist3Data.m_averageCount,
            };

            return model;
        }

        public IDictionary<int, int> GetBuildingBreakdownByDistrict()
        {
            Dictionary<int, int> districtBuildings = new Dictionary<int, int>();

            foreach (Building building in _buildingManager.m_buildings.m_buffer)
            {
                if (building.m_flags == Building.Flags.None)
                {
                    continue;
                }

                var districtID = (int)_districtManager.GetDistrict(building.m_position);

                if (districtBuildings.ContainsKey(districtID))
                {
                    districtBuildings[districtID]++;
                }
                else
                {
                    districtBuildings.Add(districtID, 1);
                }
            }

            return districtBuildings;
        }

        public IDictionary<int, int> GetVehicleBreakdownByDistrict()
        {
            Dictionary<int, int> districtVehicles = new Dictionary<int, int>();

            foreach (Vehicle vehicle in _vehicleManager.m_vehicles.m_buffer)
            {
                if (vehicle.m_flags == Vehicle.Flags.None)
                {
                    continue;
                }

                var districtID = (int)_districtManager.GetDistrict(vehicle.GetLastFramePosition());

                if (districtVehicles.ContainsKey(districtID))
                {
                    districtVehicles[districtID]++;
                }
                else
                {
                    districtVehicles.Add(districtID, 1);
                }
            }

            return districtVehicles;
        }
    }
}
