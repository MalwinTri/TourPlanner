using TourPlanner.BL.Enum;
using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface ITourPlannerManager
    {
        Task<Tour?> Add(Tour tour);
        Task<Tour?> Edit(Tour tour, edit mode = edit.Generate);
        void DeleteTour(Tour tour);
        IEnumerable<Tour> FindMatchingTours(string? searchText = null);

        int CalculatePopularity(Tour tour);
        int CalculateChildFriendliness(Tour tour);
    }
}
