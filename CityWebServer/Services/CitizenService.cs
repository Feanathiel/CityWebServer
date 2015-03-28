using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public EducationEmployment GetEducationEmploymentRate()
        {
            IEnumerable<Citizen> citizens = GetValidCitizens();

            var eduRate = citizens
                .Select(x => new
                {
                    x.EducationLevel,
                    IsEmployed = IsEmployed(x)
                })
                .GroupBy(x => x.EducationLevel)
                .ToDictionary(x => x.Key, x => new Employment
                {
                    Employed = x.Count(y => y.IsEmployed),
                    Unemployed = x.Count(y => !y.IsEmployed)
                });

            var educationEmployment = new EducationEmployment
            {
                Uneducated = new Employment(),
                Elementary = new Employment(),
                HighSchool = new Employment(),
                University = new Employment()
            };

            foreach (var employmentPair in eduRate)
            {
                Citizen.Education level = employmentPair.Key;
                Employment employment = employmentPair.Value;

                switch (level)
                {

                    case Citizen.Education.Uneducated:
                        educationEmployment.Uneducated = employment;
                        break;
                    case Citizen.Education.OneSchool:
                        educationEmployment.Elementary = employment;
                        break;
                    case Citizen.Education.TwoSchools:
                        educationEmployment.HighSchool = employment;
                        break;
                    case Citizen.Education.ThreeSchools:
                        educationEmployment.University = employment;
                        break;
                }
            }

            return educationEmployment;
        }

        private static IEnumerable<Citizen> GetValidCitizens()
        {
            Citizen[] gameCitizens = Singleton<CitizenManager>.instance.m_citizens.m_buffer;

            IList<Citizen> validCitizens = new List<Citizen>();

            foreach (var citizen in gameCitizens)
            {
                if (citizen.m_instance != 0 &&
                    !citizen.Dead &&
                    Is(citizen, Citizen.Flags.Created) &&
                    !Is(citizen, Citizen.Flags.DummyTraffic) &&
                    !Is(citizen, Citizen.Flags.Tourist) &&
                    !Is(citizen, Citizen.Flags.MovingIn))
                {
                    validCitizens.Add(citizen);
                }
            }

            return validCitizens;
        }

        private static bool IsDead(Citizen citizen)
        {
            return (citizen.m_flags & Citizen.Flags.Dead) == Citizen.Flags.Dead;
        }

        private static bool IsCreated(Citizen citizen)
        {
            return (citizen.m_flags & Citizen.Flags.Created) == Citizen.Flags.Created;
        }

        private static bool IsEmployed(Citizen citizen)
        {
            return !((citizen.m_flags & Citizen.Flags.Unemployed) == Citizen.Flags.Unemployed);
        }

        private static bool Is(Citizen citizen, Citizen.Flags flags)
        {
            return (citizen.m_flags & flags) == flags;
        }
    }
}
