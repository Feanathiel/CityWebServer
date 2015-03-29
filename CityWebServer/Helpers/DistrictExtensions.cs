using System;

namespace CityWebServer.Helpers
{
    internal static class DistrictExtensions
    {
        public static Boolean IsValid(this District district)
        {
            return (district.m_flags != District.Flags.None);
        }

        public static Boolean IsAlive(this District district)
        {
            // Get the flags on the district, to ensure we don't access garbage memory if it doesn't have a flag for District.Flags.Created
            Boolean alive = ((district.m_flags & District.Flags.Created) == District.Flags.Created);
            return alive;
        }
    }
}