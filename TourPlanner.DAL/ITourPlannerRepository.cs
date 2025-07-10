using TourPlanner.Models;

namespace TourPlanner.DAL
{
    public interface ITourPlannerRepository
    {
        Task<Tour> AddAsync(Tour tour);
        Tour? Edit(Tour tour);
        bool Remove(Tour tour);
        Task<TourLog?> AddAsync(TourLog tourLog);
        Task<Tour> AddTourWithLogsAsync(Tour tour, List<TourLog> tourLogs);
        TourLog? Edit(TourLog tourLog);
        bool Remove(TourLog tourLog);

        IEnumerable<Tour> GetAllTours();
        IEnumerable<Tour> GetAllToursByText(string searchPattern);
        IEnumerable<TourLog> GetAllTourLogsById(Guid tourId);
        IEnumerable<TourLog> GetAllTourLogsByText(string searchPattern);
    }
}
