using System;
using System.Collections.Generic;

namespace CityWebServer.Helpers
{
    internal static class EnumHelper
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}