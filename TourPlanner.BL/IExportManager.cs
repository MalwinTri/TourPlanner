using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface IExportManager
    {
        Task<bool> ExportTour(Tour tour, IEnumerable<TourLog> tourLogs);
    }
}
