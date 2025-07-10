using Newtonsoft.Json;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL.Export
{
    public class ExportGenerator : IExportManager
    {
        private readonly ILogger _logger;
        private readonly string _exportPath;

        public ExportGenerator(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExportGenerator>();

            // Pfad zurück ins Projektverzeichnis (3 Ordner hoch)
            _exportPath = Path.GetFullPath(Path.Combine("..", "..", "..", "Export"));

            _logger.Debug("🔧 ExportGenerator initialized");
            _logger.Debug($"🔧 Export path resolved to: {_exportPath}");
        }

        public async Task<bool> ExportTour(Tour tour, IEnumerable<TourLog> tourLogs)
        {
            try
            {
                _logger.Debug($"📦 Preparing export for tour: {tour.Name}");

                if (!Directory.Exists(_exportPath))
                {
                    Directory.CreateDirectory(_exportPath);
                    _logger.Debug($"📁 Created export directory: {_exportPath}");
                }
                else
                {
                    _logger.Debug($"📁 Export directory already exists: {_exportPath}");
                }

                var fileName = $"{tour.Name}.json";
                var fullPath = Path.Combine(_exportPath, fileName);

                _logger.Debug($"📝 Will export to file: {fullPath}");

                var jsonObject = new { tour, tourLogs };
                var json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                await File.WriteAllTextAsync(fullPath, json);

                _logger.Debug($"✅ Export completed successfully. File written: {fullPath}");

                return true;
            }
            catch (Exception e)
            {
                _logger.Error($"❌ [ExportTour] Error during export: {e.Message}");
                throw;
            }
        }
    }
}