namespace TourPlanner.BL.Mapquest
{
    public interface IMapquestConfiguration
    {
        string MapquestApiUrl { get; }
        string ImagePath { get; }
        string MapquestApiKey { get; }
    }
}
