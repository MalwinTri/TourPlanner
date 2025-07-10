using TourPlanner.Models;

namespace TourPlanner.BL.Mapquest
{
    public interface IMapquestConfiguration
    {
        string MapquestApiKey { get; }
        string MapquestApiUrl { get; }
        string ImagePath { get; }
    }
}
