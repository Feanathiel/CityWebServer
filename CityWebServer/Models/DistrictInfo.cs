using System;
using System.Collections.Generic;
using CityWebServer.Helpers;
using ColossalFramework;

namespace CityWebServer.Models
{
    public class DistrictInfo
    {
        public int DistrictID { get; set; }

        public String DistrictName { get; set; }

        public int TotalPopulationCount { get; set; }

        public int TotalBuildingCount { get; set; }

        public int TotalVehicleCount { get; set; }

        public int CurrentHouseholds { get; set; }

        public int AvailableHouseholds { get; set; }

        public int CurrentJobs { get; set; }

        public int AvailableJobs { get; set; }

        public int WeeklyTouristVisits { get; set; }

        public int AverageLandValue { get; set; }

        public static IEnumerable<int> GetDistricts()
        {
            var districtManager = Singleton<DistrictManager>.instance;

            // This is the value used in Assembly-CSharp, so I presume that's the maximum number of districts allowed.
            const int count = 128;

            var districts = districtManager.m_districts.m_buffer;

            for (int i = 0; i < count; i++)
            {
                if (!districts[i].IsAlive()) { continue; }
                yield return i;
            }
        }

        public static DistrictInfo GetDistrictInfo(int districtID)
        {
            var districtManager = Singleton<DistrictManager>.instance;
            var district = GetDistrict(districtID);

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
                districtName = districtManager.GetDistrictName(districtID);
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

        private static District GetDistrict(int districtID)
        {
            var districtManager = Singleton<DistrictManager>.instance;
            var district = districtManager.m_districts.m_buffer[districtID];
            return district;
        }
    }
}