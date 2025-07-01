using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.WeatherAPI.DTO;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL.WeatherAPI
{
    public class WeatherApiGenerator : IWeatherGenerator
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherApiGenerator(IWeatherApiConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WeatherApiGenerator>();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration.WeatherApiUrl)
            };

            _apiKey = configuration.WeatherApiKey;

        }

        public async Task<Weather?> GetWeather(Tour tour)
        {
            try
            {
                if (tour.EndCoordinates.Count < 2)
                {
                    _logger.Error("No coordinates for weather");
                    throw new WeatherApiReturnedNullException("No coordinates for weather");
                }

                var requestString =
                    $"current.json?key={_apiKey}&q={tour.EndCoordinates[0].ToString(CultureInfo.InvariantCulture)},{tour.EndCoordinates[1].ToString(CultureInfo.InvariantCulture)}&aqi=no&units=metric";

                var t = await _httpClient.GetFromJsonAsync<WeatherApiDto>(requestString);
                if (t == null)
                {
                    _logger.Error("Weather returned null");
                    throw new WeatherApiReturnedNullException("Json deserialize error");
                }
                _logger.Debug($"Weather returned");

                return t.ToWeather();
            }
            catch (HttpRequestException e) // Non success
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new WeatherApiReturnedNullException("Http request error", e);
            }
            catch (NotSupportedException e) // When content type is not valid
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new WeatherApiReturnedNullException("Content type error", e);
            }
            catch (JsonException e) // Invalid JSON
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new WeatherApiReturnedNullException("Json error", e);
            }
        }
    }
}
