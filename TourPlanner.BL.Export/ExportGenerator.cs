using Newtonsoft.Json;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL.Export
{
    public class ExportGenerator : IExportConfiguration
    {
        private readonly ILogger _logger;
        private readonly string _exportPath;

        public ExportGenerator(IExportConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExportGenerator>();
            _exportPath = configuration.ExportPath;
            _logger.Debug("Export generator created");
        }

        public string ExportPath => throw new NotImplementedException();

        public async Task<bool> ExportTour(Tour tour, IEnumerable<TourLog> tourLogs)
        {
            try
            {
                if (!Directory.Exists(_exportPath))
                {
                    Directory.CreateDirectory(_exportPath);
                    _logger.Debug($"Created export directory at {_exportPath}");
                }

                var tourPath = Path.Combine(_exportPath, $"{tour.Name}.json");
                var jsonObject = new { tour, tourLogs };
                var json = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
                await File.WriteAllTextAsync(tourPath, json);

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
