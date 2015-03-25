using System;
using CityWebServer.Extensibility;

namespace CityWebServer.Helpers
{
    /// <summary>
    /// Extensions for the <see cref="IRequestParameters"/> interface.
    /// </summary>
    internal static class RequestParametersExtensions
    {
        /// <summary>
        /// Checks if the query string within the request's parameters contains a specific key.
        /// </summary>
        /// <param name="parameters">The request's parameters.</param>
        /// <param name="key">The key to lookup.</param>
        /// <returns>True if the key was found, false otherwise.</returns>
        public static Boolean HasQueryStringKey(this IRequestParameters parameters, String key)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            String value; // Ignored
            return parameters.TryGetQueryStringValue(key, out value);
        }
    }
}
