using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface ITourPlannerLogManager
    {
        TourLog? Add(TourLog tourLog);
        TourLog? Edit(TourLog tourLog);
        void Delete(TourLog tour);
        IEnumerable<TourLog> FindMatchingTourLogs(Tour tour);
    }
}
