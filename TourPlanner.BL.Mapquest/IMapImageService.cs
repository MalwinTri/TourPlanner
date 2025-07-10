using TourPlanner.Models;

namespace TourPlanner.BL.Mapquest
{
    public interface IMapImageService
    {
        Task<bool> LoadImage(Tour tour, string? sessionId = null);
    }
}