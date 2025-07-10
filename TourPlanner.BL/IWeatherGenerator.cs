using TourPlanner.Models;

namespace TourPlanner.BL
{
    public interface IWeatherGenerator
    {
        Task<Weather?> GetWeather(Tour tour);
    }
}
