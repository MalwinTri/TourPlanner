namespace TourPlanner.BL.WeatherAPI
{
    public interface IWeatherApiConfiguration
    {
        public string WeatherApiKey { get; }
        public string WeatherApiUrl { get; }
    }
}
