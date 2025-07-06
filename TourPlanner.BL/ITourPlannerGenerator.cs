using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface ITourPlannerGenerator
    {
        Task<Tour?> GenerateTourFromTourAsync(Tour tour);
        Task<bool> LoadImage(Tour tour, string? sessionId = null);
    }
}
