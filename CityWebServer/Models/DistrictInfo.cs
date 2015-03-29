using System;

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
    }
}