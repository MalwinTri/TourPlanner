using TourPlanner.DAL;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL
{
    public class TourPlannerLogManager : ITourPlannerLogManager
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerRepository _repository;
        public TourPlannerLogManager(ITourPlannerRepository repository, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TourPlannerLogManager>();
            _repository = repository;
        }
        public async Task<TourLog?> AddAsync(TourLog tourLog)
        {
            try
            {
                var result = await _repository.AddAsync(tourLog);
                _logger.Debug($"Added tourLog {tourLog.Id}");
                return result;
            }
            catch (Exception e)
            {
                _logger.Error($"[AddAsync] {e.Message}");
                throw;
            }
        }

        public TourLog? Edit(TourLog tourLog)
        {
            try
            {
                _repository.Edit(tourLog);
                _logger.Debug($"Edited tourLog {tourLog.Id}");
                return tourLog;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public void Delete(TourLog tourLog)
        {
            try
            {
                _repository.Remove(tourLog);
                _logger.Debug($"Deleted tourLog {tourLog.Id}");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }

        public IEnumerable<TourLog> FindMatchingTourLogs(Tour tour)
        {
            try
            {
                _logger.Debug($"Returning tourLogs for tour {tour.Id}");
                return _repository.GetAllTourLogsById(tour.Id);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                throw;
            }
        }
    }
}
