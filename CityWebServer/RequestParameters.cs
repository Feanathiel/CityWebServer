using System;
using System.Collections.Specialized;
using System.Net;
using CityWebServer.Extensibility;

namespace CityWebServer
{
    /// <summary>
    /// Wraps a <see cref="HttpListenerRequest"/> so that it matches the <see cref="IRequestParameters"/> interface.
    /// </summary>
    internal class RequestParameters : IRequestParameters
    {
        private readonly HttpListenerRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestParameters"/> class.
        /// </summary>
        public RequestParameters(HttpListenerRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            _request = request;
        }

        /// <summary>
        /// Gets the HTTP method specified by the client.
        /// </summary>
        public String HttpMethod
        {
            get
            {
                return _request.HttpMethod;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Uri"/> object requested by the client.
        /// </summary>
        public Uri Url
        {
            get
            {
                return _request.Url;
            }
        }

        /// <summary>
        /// Tries to get the value based on the given key in the query string.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The returned value, only usable if the method returned true.</param>
        /// <returns>True if the key was found, false otherwise.</returns>
        public bool TryGetQueryStringValue(String key, out String value)
        {
            NameValueCollection queryString = _request.QueryString;

            foreach (String allKey in queryString.AllKeys)
            {
                if (allKey == key)
                {
                    value = queryString[allKey];
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
