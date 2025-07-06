using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface IImportManager
    {
        Task<Tour> ImportTour(string path);
    }
}
