using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Logging.Log4Net
{
    internal class Log4NetLogger : ILogger
    {
        private readonly ILog _logger;

        public Log4NetLogger(ILog logger)
        {
            _logger = logger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }
    }
}
