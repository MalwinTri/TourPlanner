using Microsoft.Extensions.Configuration;
using TourPlanner.BL.Export;
using TourPlanner.BL.iText;
using TourPlanner.BL.OpenRouteService;
using TourPlanner.BL.WeatherAPI;
using TourPlanner.DAL.Postgres;

namespace TourPlanner.Configuration
{
    internal class AppConfiguration : IOpenRouteServiceConfiguration, ITourPlannerPostgresRepositoryConfiguration,
        IItextConfiguration, IWeatherApiConfiguration, IExportConfiguration
    {
        private readonly IConfiguration _configuration;

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // OpenRouteService
        public string OpenRouteServiceApiUrl => _configuration["openrouteservice:baseurl"]
            ?? throw new InvalidOperationException("Missing ORS baseurl in configuration.");
        public string ApiKey => _configuration["openrouteservice:apikey"]!;
        string IOpenRouteServiceConfiguration.ImagePath => _configuration["openrouteservice:imagepath"]!;

        // Postgres
        public string ConnectionString => _configuration["postgres:connectionstring"]!;
        public string Username => _configuration["postgres:username"]!;
        public string Password => _configuration["postgres:password"]!;

        // iText
        public string OutputPath => _configuration["itext:outputpath"]!;

        // Weather API
        public string WeatherApiUrl => _configuration["weatherapi:baseurl"]!;
        public string WeatherApiKey => _configuration["weatherapi:apikey"]!;

        // Export
        public string ExportPath => _configuration["export:path"]!;
    }
}