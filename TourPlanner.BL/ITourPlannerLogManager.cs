using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface ITourPlannerLogManager
    {
        Task<TourLog?> AddAsync(TourLog tourLog);
        TourLog? Edit(TourLog tourLog);
        void Delete(TourLog tour);
        IEnumerable<TourLog> FindMatchingTourLogs(Tour tour);
    }
}
