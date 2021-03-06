using System;
using System.Collections.Generic;
using CityWebServer.Extensibility;

namespace CityWebServer
{
    /// <summary>
    /// Compares the priority of two <see cref="IRequestHandler"/>s.
    /// </summary>
    internal class RequestHandlerPriorityComparer : IComparer<IRequestHandler>
    {
        public int Compare(IRequestHandler x, IRequestHandler y)
        {
            if (x == null && y == null)
            {
                return 0;
            }

            if (x == null)
            {
                return 1;
            }

            if (y == null)
            {
                return -1;
            }
            
            // Note: reversed.
            int value = Comparer<Guid>.Default.Compare(y.HandlerID, x.HandlerID);

            return value;
        }
    }
}