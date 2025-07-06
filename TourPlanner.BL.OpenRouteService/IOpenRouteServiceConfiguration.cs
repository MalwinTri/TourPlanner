namespace TourPlanner.BL.OpenRouteService
{
    public interface IOpenRouteServiceConfiguration
    {
        string ApiKey { get; }
        string ImagePath { get; }
        string OpenRouteServiceApiUrl { get; }
    }
}
