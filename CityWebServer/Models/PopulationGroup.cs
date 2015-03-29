using System;

namespace CityWebServer.Models
{
    internal class PopulationGroup
    {
        public String Name { get; set; }

        public int Amount { get; set; }

        public PopulationGroup(String name, int amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
