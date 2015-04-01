using System;

namespace CityWebServer.Models
{
    internal class VitalsValue
    {
        public String Name { get; set; }
        public String NeedName { get; set; }
        public int Need { get; set; }
        public String CapacityName { get; set; }
        public int Capacity { get; set; }
        public int Percentage { get; set; }
        public String Unit { get; set; }
        public Boolean LowerIsBetter { get; set; }
    }
}