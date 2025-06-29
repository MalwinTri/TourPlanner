using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Repository;


namespace TourPlanner.Logging.Log4Net
{
    public class Log4NetFactory : ILoggerFactory
    {
        private readonly string _configPath;

        public Log4NetFactory(string configPath)
        {
            _configPath = configPath;
        }

        public ILogger CreateLogger<TContext>()
        {
            if (!File.Exists(_configPath))
            {
                throw new ArgumentException("Does not exist", nameof(_configPath));
            }

            log4net.Config.XmlConfigurator.Configure(new FileInfo(_configPath));

            var logger = log4net.LogManager.GetLogger(typeof(TContext));

            return new Log4NetLogger(logger);
        }
    }
}
