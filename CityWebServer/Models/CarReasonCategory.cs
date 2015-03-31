using System.Collections.Generic;

namespace CityWebServer.Models
{
    internal class CarReasonCategory
    {
        public string Category { get; set; }

        public IEnumerable<CarReasonReason> Reasons { get; set; }

        public float Percentage { get; set; }
    }
}