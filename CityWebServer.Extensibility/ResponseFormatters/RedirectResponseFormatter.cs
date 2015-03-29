using System;
using System.Net;

namespace CityWebServer.Extensibility.ResponseFormatters
{
    internal class RedirectResponseFormatter : IResponseFormatter
    {
        private readonly String _url;

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
