using System;
using System.Collections.Generic;
using System.Linq;
using CityWebServer.Helpers;
using CityWebServer.Models;
using ColossalFramework;
using UnityEngine;

namespace CityWebServer.Services
{
    internal class CityInfoService
    {
        private readonly DistrictManager _districtManager;
        private readonly VehicleManager _vehicleManager;
        private readonly BuildingManager _buildingManager;
        private readonly ImmaterialResourceManager _immaterialResourceManager;

        public CityInfoService()
        {
            _districtManager = Singleton<DistrictManager>.instance;
            _vehicleManager = Singleton<VehicleManager>.instance;
            _buildingManager = Singleton<BuildingManager>.instance;
            _immaterialResourceManager = Singleton<ImmaterialResourceManager>.instance;
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
			
			var pollution = Math.Round((district.m_groundData.m_finalPollution / (Double) byte.MaxValue), 2);

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
				Pollution = pollution,
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

        public IEnumerable<CarReasonCategory> GetCarReasons()
        {
            IEnumerable<Vehicle> vehicles = GetValidCars();
            IEnumerable<CarItem> items = GetFlatCarReasons(vehicles);

            var total = items.Count();

            var model = items
                .GroupBy(x => x.Category)
                .Select(
                    x =>
                        new CarReasonCategory
                        {
                            Category = x.Key,
                            Reasons = x
                                .GroupBy(y => y.Reason)
                                .Select(y => new CarReasonReason {Reason = y.Key, Percentage = (float) y.Count()/total*100})
                                .OrderBy(y => y.Reason)
                                .ToList(),
                            Percentage = (float) x.Count()/total*100
                        })
                .OrderBy(x => x.Category)
                .ToList();

            return model;
        }

        private IEnumerable<CarItem> GetFlatCarReasons(IEnumerable<Vehicle> vehicles)
        {
            IList<CarItem> items = new List<CarItem>();

            foreach (var vehicle in vehicles)
            {
                var reason = (TransferManager.TransferReason) vehicle.m_transferType;

                if ((vehicle.m_flags & Vehicle.Flags.Importing) == Vehicle.Flags.Importing)
                {
                    items.Add(new CarItem
                    {
                        Category = "Import",
                        Reason = reason.ToString()
                    });
                }
                else if ((vehicle.m_flags & Vehicle.Flags.Exporting) == Vehicle.Flags.Exporting)
                {
                    items.Add(new CarItem
                    {
                        Category = "Export",
                        Reason = reason.ToString()
                    });
                }
                else if (reason == TransferManager.TransferReason.None)
                {
                    VehicleAI vehicleAi = ((VehicleAI) vehicle.Info.GetAI());
                    Vehicle dummy = vehicle;
                    InstanceID target = vehicleAi.GetTargetID(vehicle.Info.m_instanceID.Vehicle, ref dummy);

                    var service = _buildingManager.m_buildings.m_buffer[target.Building].Info.GetService();

                    items.Add(new CarItem
                    {
                        Category = "Citizen",
                        Reason = service.ToString()
                    });
                }
                else
                {
                    items.Add(new CarItem
                    {
                        Category = "Intra",
                        Reason = reason.ToString()
                    });
                }
            }

            return items;
        }

        private IEnumerable<Vehicle> GetValidCars()
        {
            Vehicle[] vehicles = _vehicleManager.m_vehicles.m_buffer;

            foreach (var vehicle in vehicles)
            {
                VehicleInfo.VehicleType vehicleType = vehicle.Info.m_vehicleType;

                if (!((vehicleType & VehicleInfo.VehicleType.Car) == VehicleInfo.VehicleType.Car))
                {
                    continue;
                }

                if (!((vehicle.m_flags & Vehicle.Flags.Created) == Vehicle.Flags.Created))
                {
                    continue;
                }

                if (!((vehicle.m_flags & Vehicle.Flags.Spawned) == Vehicle.Flags.Spawned))
                {
                    continue;
                }

                if (vehicle.m_leadingVehicle != 0)
                {
                    continue;
                }

                yield return vehicle;
            }
        }

        private class CarItem
        {
            public String Category;
            public String Reason;
        }

        public VitalsValue GetElectricityValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetElectricityCapacity() / 1000;
            int need = district.GetElectricityConsumption() / 1000;
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Electricity",
                CapacityName = "Production",
                Capacity = capacity,
                NeedName = "Consumption",
                Need = need,
                Percentage = percentage,
                Unit = "MW"
            };
        }

        public VitalsValue GetWaterValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetWaterCapacity();
            int need = district.GetWaterConsumption();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Water",
                CapacityName = "Pump capacity",
                Capacity = capacity,
                NeedName = "Consumption",
                Need = need,
                Percentage = percentage,
                Unit = "m³/week"
            };
        }

        public VitalsValue GetSewageValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetSewageCapacity();
            int need = district.GetSewageAccumulation();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Sewage",
                CapacityName = "Draining capacity",
                Capacity = capacity,
                NeedName = "Consumption",
                Need = need,
                Percentage = percentage,
                Unit = "m³/week"
            };
        }

        public VitalsValue GetIncinerationValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetIncinerationCapacity();
            int need = district.GetGarbageAccumulation();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Incineration",
                CapacityName = "Capacity",
                Capacity = capacity,
                NeedName = "Production",
                Need = need,
                Percentage = percentage,
                Unit = "units/week"
            };
        }

        public VitalsValue GetLandfillValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetGarbageCapacity();
            int need = district.GetGarbageAmount();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Landfill",
                CapacityName = "Capacity",
                Capacity = capacity,
                NeedName = "Storage",
                Need = need,
                Percentage = percentage,
                Unit = "units/week"
            };
        }

        public VitalsValue GetHealthValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetHealCapacity();
            int need = district.GetSickCount();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Healthcare",
                CapacityName = "Heal capacity",
                Capacity = capacity,
                NeedName = "Sick citizens",
                Need = need,
                Percentage = percentage,
                Unit = "persons"
            };
        }

        public VitalsValue GetCremationValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetCremateCapacity();
            int need = district.GetDeadCount();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Crematorium",
                CapacityName = "Capacity", 
                Capacity = capacity,
                NeedName = "Deceased",
                Need = need,
                Percentage = percentage,
                Unit = "persons"
            };
        }

        public VitalsValue GetElementarySchoolValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetEducation1Capacity();
            int need = district.GetEducation1Need();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "Elementary school",
                CapacityName = "Capacity",
                Capacity = capacity,
                NeedName = "Eligible",
                Need = need,
                Percentage = percentage,
                Unit = "persons"
            };
        }

        public VitalsValue GetHighSchoolValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetEducation2Capacity();
            int need = district.GetEducation2Need();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "High school",
                CapacityName = "Capacity",
                Capacity = capacity,
                NeedName = "Eligible",
                Need = need,
                Percentage = percentage,
                Unit = "persons"
            };
        }

        public VitalsValue GetUniversityValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            int capacity = district.GetEducation3Capacity();
            int need = district.GetEducation3Need();
            int percentage = (int)GetPercentage(capacity, need);

            return new VitalsValue
            {
                Name = "University",
                CapacityName = "Capacity",
                Capacity = capacity,
                NeedName = "Eligible",
                Need = need,
                Percentage = percentage,
                Unit = "persons"
            };
        }

        public VitalsValue GetResidentialHappynessValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            return new VitalsValue
            {
                Name = "Residential happyness",
                Percentage = district.m_residentialData.m_finalHappiness
            };
        }

        public VitalsValue GetCommercialHappynessValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            return new VitalsValue
            {
                Name = "Commercial happyness",
                Percentage = district.m_commercialData.m_finalHappiness
            };
        }

        public VitalsValue GetIndustrialHappynessValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            return new VitalsValue
            {
                Name = "Industrial happyness",
                Percentage = district.m_industrialData.m_finalHappiness
            };
        }

        public VitalsValue GetOfficeHappynessValues()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            return new VitalsValue
            {
                Name = "Office happyness",
                Percentage = district.m_officeData.m_finalHappiness
            };
        }

        public VitalsValue GetHealth()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            var total = district.m_residentialData.m_finalHealth;

            return new VitalsValue
            {
                Name = "Health",
                Percentage = total
            };
        }

        public VitalsValue GetWellbeing()
        {
            int total;

            _immaterialResourceManager.CheckTotalResource(ImmaterialResourceManager.Resource.Wellbeing, out total);

            return new VitalsValue
            {
                Name = "Wellbeing",
                Percentage = total
            };
        }

        public VitalsValue GetCrimeRate()
        {
            District district = _districtManager.m_districts.m_buffer[0];

            return new VitalsValue
            {
                Name = "Crime rate",
                Percentage = district.m_finalCrimeRate,
                LowerIsBetter = true
            };
        }

        public VitalsValue GetFireHazard()
        {
            int total;
            _immaterialResourceManager.CheckTotalResource(ImmaterialResourceManager.Resource.FireHazard, out total);

            return new VitalsValue
            {
                Name = "Fire hazard",
                Percentage = total,
                LowerIsBetter = true
            };
        }

        public VitalsValue GetNoisePollution()
        {
            int total;
            _immaterialResourceManager.CheckTotalResource(ImmaterialResourceManager.Resource.NoisePollution, out total);

            return new VitalsValue
            {
                Name = "Noise pollution",
                Percentage = total,
                LowerIsBetter = true
            };
        }

        public VitalsValue GetAttractiveness()
        {
            int total;
            _immaterialResourceManager.CheckTotalResource(ImmaterialResourceManager.Resource.Attractiveness, out total);

            return new VitalsValue
            {
                Name = "Attractiveness",
                Percentage = total
            };
        }

        public VitalsValue GetLeisure()
        {
            int total;
            _immaterialResourceManager.CheckTotalResource(ImmaterialResourceManager.Resource.Entertainment, out total);

            return new VitalsValue
            {
                Name = "Leisure",
                Percentage = total
            };
        }

        public VitalsValue GetGraduatedElementarySchool()
        {
            District district = _districtManager.m_districts.m_buffer[0];
            var percentage = district.GetEducation1Rate();
            
            return new VitalsValue
            {
                Name = "Graduated elementary school citizens",
                Percentage = percentage
            };
        }

        public VitalsValue GetGraduatedHighSchool()
        {
            District district = _districtManager.m_districts.m_buffer[0];
            var percentage = district.GetEducation2Rate();

            return new VitalsValue
            {
                Name = "Graduated high school citizens",
                Percentage = percentage
            };
        }

        public VitalsValue GetGraduatedUniversity()
        {
            District district = _districtManager.m_districts.m_buffer[0];
            var percentage = district.GetEducation3Rate();

            return new VitalsValue
            {
                Name = "Graduated university citizens",
                Percentage = percentage
            };
        }

        private static double GetPercentage(int capacity, int need, int needMin = 45, int needMax = 55)
        {
            double percentage;

            if (need != 0)
            {
                double ratio = (double)capacity / need;
                double halfWay = (needMin + needMax) / 2d;

                percentage = (ratio * halfWay);
            }
            else
            {
                percentage = 100;
            }

            percentage = Math.Min(100, Math.Max(0, percentage));

            return percentage;
        }


        private static double GetPercentage(byte value, int needMin = 45, int needMax = 55)
        {
            double percentage;

            if (value != 0)
            {
                double ratio = (double)byte.MaxValue / value;
                double halfWay = (needMin + needMax) / 2d;

                percentage = (ratio * halfWay);
            }
            else
            {
                percentage = 100;
            }

            percentage = Math.Min(100, Math.Max(0, percentage));

            return percentage;
        }
    }
}
