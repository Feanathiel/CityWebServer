using System;
using System.Net;

namespace CityWebServer.Extensibility.ResponseFormatters
{
    /// <summary>
    /// Response for redirecting the user to another page.
    /// </summary>
    internal class RedirectResponseFormatter : IResponseFormatter
    {
        private readonly String _url;

        /// <summary>
        /// Creates a new instance of the <see cref="RedirectResponseFormatter"/>.
        /// </summary>
        /// <param name="url">The url to redirect to.</param>
        public RedirectResponseFormatter(String url)
        {
            _url = url;
        }

        public override void WriteContent(HttpListenerResponse response)
        {
            response.Redirect(_url);
        }
    }
}
