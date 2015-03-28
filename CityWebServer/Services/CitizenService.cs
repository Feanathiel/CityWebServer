using System;
using CityWebServer.Models;
using ColossalFramework;

namespace CityWebServer.Services
{
    /// <summary>
    /// Gets citizen specific data from the game.
    /// </summary>
    internal class CitizenService
    {
        private readonly DistrictManager _districtManager;

        /// <summary>
        /// Creates a new instance of the <see cref="CitizenService"/>.
        /// </summary>
        public CitizenService()
        {
            _districtManager = Singleton<DistrictManager>.instance;
        }

        /// <summary>
        /// Gets the age distribution of the city.
        /// </summary>
        public AgeDistribution GetAgeGroupPopulation()
        {
            const int globalDistrictId = 0;

            District district = _districtManager.m_districts.m_buffer[globalDistrictId];

            AgeDistribution ageDistribution = new AgeDistribution
            {
                Children = (int) district.m_childData.m_finalCount,
                Teens = (int) district.m_teenData.m_finalCount,
                YoungAdults = (int) district.m_youngData.m_finalCount,
                Adults = (int) district.m_adultData.m_finalCount,
                Seniors = (int) district.m_seniorData.m_finalCount
            };

            return ageDistribution;
        }

        public BirthAndDeathRate GetBirthAndDeathRate()
        {
            const int globalDistrictId = 0;

            District district = _districtManager.m_districts.m_buffer[globalDistrictId];

            BirthAndDeathRate rate = new BirthAndDeathRate
            {
                Birth = (int) district.m_birthData.m_finalCount,
                Death = (int) district.m_deathData.m_finalCount
            };

            return rate;
        }
    }
}
