using Microsoft.Extensions.Configuration;
using TourPlanner.BL.Export;
using TourPlanner.BL.iText;
using TourPlanner.BL.Mapquest;
using TourPlanner.BL.WeatherAPI;
using TourPlanner.DAL.Postgres;

namespace TourPlanner.Configuration
{
    internal class AppConfiguration : IMapquestConfiguration, ITourPlannerPostgresRepositoryConfiguration,
        IItextConfiguration, IWeatherApiConfiguration, IExportConfiguration
    {
        private readonly IConfiguration _configuration;

        public AppConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string MapquestApiUrl => _configuration["mapquest:baseurl"]!;
        public string MapquestApiKey => _configuration["mapquest:apikey"]!;
        public string ImagePath => _configuration["mapquest:imagepath"]!;
        public string ConnectionString => _configuration["postgres:connectionstring"]!;
        public string Username => _configuration["postgres:username"]!;
        public string Password => _configuration["postgres:password"]!;
        public string OutputPath => _configuration["itext:outputpath"]!;
        public string WeatherApiUrl => _configuration["weatherapi:baseurl"]!;
        public string WeatherApiKey => _configuration["weatherapi:apikey"]!;
        public string ExportPath => _configuration["export:path"]!;
    }
}
