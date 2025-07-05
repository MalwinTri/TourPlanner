using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.Versioning;
using System.Text.Json;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Mapquest.DTO;
using TourPlanner.Logging;
using TourPlanner.Models;
using System.Drawing.Imaging;
using System.Drawing;

namespace TourPlanner.BL.Mapquest
{
    [SupportedOSPlatform("windows")]
    public class MapquestTourGenerator : ITourPlannerGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        private readonly string _imagePath;
        private readonly string _apiKey;


        public MapquestTourGenerator(IMapquestConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MapquestTourGenerator>();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration.MapquestApiUrl)
            };

            _imagePath = configuration.ImagePath;
            _apiKey = configuration.MapquestApiKey;

        }

        public async Task<Tour?> GenerateTourFromTourAsync(Tour tour)
        {
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
                _logger.Debug($"Created directory: {_imagePath}");
            }

            try
            {
                // simulate slow connection by adding delay

                var transport = tour.Transport switch
                {
                    "Bike" => "bicycle",
                    "Walking" => "pedestrian",
                    _ => "fastest"
                };
                var mapquestResponse = await _httpClient.GetFromJsonAsync<MapquestTourDto>(
                    $"directions/v2/route?key={_apiKey}&from={tour.From!.ToLower()}&to={tour.To!.ToLower()}&outFormat=json&ambiguities=ignore&routeType={transport}&unit=k");
                if (mapquestResponse == null)
                {
                    _logger.Error("Mapquest returned null");
                    throw new MapquestReturnedNullException();
                }

                var imageTask = LoadImage(tour, mapquestResponse.Route!.SessionId);
                _logger.Debug($"Mapquest returned session id: {mapquestResponse.Route.SessionId}");

                mapquestResponse.AddToTour(tour);
                _logger.Debug("Data added to tour");

                await imageTask;
                _logger.Debug("Image loaded");

                return tour;
            }
            catch (HttpRequestException e) // Non success
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new MapquestReturnedNullException("Http request error", e);
            }
            catch (NotSupportedException e) // When content type is not valid
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new MapquestReturnedNullException("Content type error", e);
            }
            catch (JsonException e) // Invalid JSON
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new MapquestReturnedNullException("Json error", e);
            }
            catch (Exception e) // For any other error
            {
                _logger.Error($"An error occurred: {e.Message}");
                throw new MapquestReturnedNullException("Unknown error", e);
            }
        }

        public async Task<bool> LoadImage(Tour tour, string? sessionId = null)
        {
            try
            {
                Stream imageStream;
                if (sessionId == null)
                {
                    var start =
                        $"{tour.StartCoordinates[0].ToString(CultureInfo.InvariantCulture)},{tour.StartCoordinates[1].ToString(CultureInfo.InvariantCulture)}";
                    var end = $"{tour.EndCoordinates[0].ToString(CultureInfo.InvariantCulture)},{tour.EndCoordinates[1].ToString(CultureInfo.InvariantCulture)}";

                    imageStream = await _httpClient.GetStreamAsync($"staticmap/v5/map?key={_apiKey}&start={start}&end={end}&size=600,400@2x");
                }
                else
                {
                    imageStream = await _httpClient.GetStreamAsync($"staticmap/v5/map?key={_apiKey}&session={sessionId}&size=600,400@2x");
                }

                _logger.Debug($"Mapquest returned image stream");

                var image = Image.FromStream(imageStream);
                var safeName = string.Concat(tour.Name.Split(Path.GetInvalidFileNameChars()));
                var filePath = Path.Combine(Path.GetFullPath(_imagePath), $"{safeName}.Jpeg");
                image.Save(filePath, ImageFormat.Jpeg);
                tour.ImagePath = "file:///" + filePath.Replace("\\", "/");

                _logger.Debug($"Mapquest image saved to {filePath}");

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Image.Save failed: " + e.Message);
                throw;
            }
        }
    }
}
