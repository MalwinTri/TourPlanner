using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface ITourPlannerGenerator
    {
        Task<Tour?> GenerateTourFromTourAsync(Tour tour);
    }
}