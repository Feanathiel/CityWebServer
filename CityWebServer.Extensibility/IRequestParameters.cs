using System;

namespace CityWebServer.Extensibility
{
    /// <summary>
    /// Information about the request the client has made.
    /// </summary>
    public interface IRequestParameters
    {
        /// <summary>
        /// Gets the HTTP method specified by the client.
        /// </summary>
        String HttpMethod { get; }

        /// <summary>
        /// Gets the <see cref="T:System.Uri"/> object requested by the client.
        /// </summary>summary>
        Uri Url { get; }

        /// <summary>
        /// Tries to get the value based on the given key in the query string.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The returned value, only usable if the method returned true.</param>
        /// <returns>True if the key was found, false otherwise.</returns>
        bool TryGetQueryStringValue(string key, out string value);
    }
}
