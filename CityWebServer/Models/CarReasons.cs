using System.Collections.Generic;

namespace CityWebServer.Models
{
    internal class CarReasons
    {
        public IEnumerable<CarReasonCategory> Categories { get;set; }
    }
}