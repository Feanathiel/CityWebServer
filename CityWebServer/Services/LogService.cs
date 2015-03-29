using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityWebServer.Services
{
    internal class LogService
    {
        private readonly List<String> _logLines;

        private LogService()
        {
            // For the entire lifetime of this instance, we'll preseve log messages.
            // After a certain point, it might be worth truncating them, but we'll cross that bridge when we get to it.
            _logLines = new List<String>();
        }

        public IEnumerable<String> GetLogLines()
        {
            return _logLines.ToList();
        }

        public static readonly LogService Instance = new LogService();

        public void LogMessage(String message)
        {
            _logLines.Add(message);
        }
    }
}
