using System.Data;
using TourPlanner.Logging;
using TourPlanner.Models;
using Newtonsoft.Json;
using TourPlanner.BL.Exceptions;
using Microsoft.EntityFrameworkCore;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL;

namespace TourPlanner.BL.Import
{
    public class ImportGenerator : IImportManager
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerManager _tourManager;
        private readonly ITourPlannerGenerator _tourGenerator;
        private readonly ITourPlannerRepository _repository; // Neu

        public ImportGenerator(
            ITourPlannerManager tourManager,
            ITourPlannerGenerator tourGenerator,
            ITourPlannerRepository repository, // Neu
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ImportGenerator>();

            _tourManager = tourManager;
            _tourLogManager = tourLogManager;
            _tourGenerator = tourGenerator;
            _repository = repository; // Neu
        }

        public async Task<Tour> ImportTour(string tourPath)
        {
            try
            {
                _logger.Debug($"[ImportTour] Starting import from: {tourPath}");

                var json = await File.ReadAllTextAsync(tourPath);
                if (string.IsNullOrWhiteSpace(json))
                    throw new ImportReturnedNullException("Import file is empty.");

                var importData = JsonConvert.DeserializeObject<ImportContainer>(json);
                if (importData?.Tour == null)
                    throw new ImportReturnedNullException("Tour is missing in import file.");

                var tour = importData.Tour;

                if (tour.Id == Guid.Empty)
                    tour.Id = Guid.NewGuid();

                var existingTours = _tourManager.FindMatchingTours();
                var baseName = tour.Name;
                int counter = 1;
                while (existingTours.Any(t => t.Name == tour.Name))
                    tour.Name = $"{baseName} ({counter++})";

                await _tourGenerator.GenerateTourFromTourAsync(tour);

                // Logs vorbereiten
                var logs = new List<TourLog>();
                if (importData.TourLogs?.Any() == true)
                {
                    foreach (var log in importData.TourLogs)
                    {
                        logs.Add(new TourLog(
                            id: Guid.NewGuid(),
                            tourId: tour.Id,
                            date: log.Date.ToUniversalTime(),
                            comment: log.Comment,
                            difficulty: log.Difficulty,
                            totalTime: log.TotalTime,
                            rating: log.Rating
                        ));
                }
                }

                var savedTour = await _repository.AddTourWithLogsAsync(tour, logs);
                if (savedTour == null || savedTour.Id == Guid.Empty)
                    throw new ImportReturnedNullException("Tour was not properly saved to DB.");

                _logger.Debug($"[ImportTour] Tour with logs saved. ID: {savedTour.Id}");
                return savedTour;
            }
            catch (OpenRouteServicemanagerException ex)
                {
                _logger.Error($"[ImportTour] Routing failed: {ex.Message}");
                throw new ImportReturnedNullException("Routing failed", ex.InnerException ?? ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.Error($"[ImportTour] DB update failed: {ex.InnerException?.Message ?? ex.Message}");
                throw new PostgresDataBaseException("Database update failed", ex);
            }
            catch (ImportReturnedNullException ex)
                {
                _logger.Error($"[ImportTour] Import error: {ex.Message}");
                throw;
                }
            catch (Exception e)
                {
                var innerMessage = e.InnerException?.Message ?? "(no inner exception)";
                _logger.Error($"Failed to add tour with logs: {e.Message} | INNER: {innerMessage}");
                throw new PostgresDataBaseException($"Failed to add tour with logs: {innerMessage}", e);
                }
            }

        private class ImportContainer
            {
            [JsonProperty("tour")]
            public Tour? Tour { get; set; }

            [JsonProperty("tourLogs")]
            public List<TourLog>? TourLogs { get; set; }
        }
    }
}
