using System.Runtime.Versioning;
using System.Text.Json;
using TourPlanner.BL.Exceptions;
using TourPlanner.Logging;
using TourPlanner.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net.Http.Headers;
using TourPlanner.BL.OpenRouteService.DTO;
using System.Globalization;

namespace TourPlanner.BL.OpenRouteService
{
    [SupportedOSPlatform("windows")]
    public class OpenRouteServiceTourGenerator : ITourPlannerGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _imagePath;
        private readonly string _apiKey;

        public OpenRouteServiceTourGenerator(IOpenRouteServiceConfiguration config, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OpenRouteServiceTourGenerator>();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config.OpenRouteServiceApiUrl)
            };

            _imagePath = config.ImagePath;
            _apiKey = config.ApiKey;
        }

        public async Task<Tour?> GenerateTourFromTourAsync(Tour tour)
        {
            EnsureImageDirectoryExists();

            try
            {
                tour.StartCoordinates = await GetCoordinatesAsync(tour.From!);
                tour.EndCoordinates = await GetCoordinatesAsync(tour.To!);

                var requestBody = new
                {
                    coordinates = new[] { tour.StartCoordinates, tour.EndCoordinates }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, "v2/directions/driving-car")
                {
                    Content = new StringContent(requestJson)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _logger.Debug($"Sending directions request: {requestJson}");

                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessOrThrow(response, "Directions API");

                var responseJson = await response.Content.ReadAsStringAsync();

                var routeResponse = JsonSerializer.Deserialize<ORSRouteResponse>(responseJson);

                _logger.Debug("Deserialized routeResponse: " + JsonSerializer.Serialize(routeResponse));

                var summary = routeResponse?.Routes?.FirstOrDefault()?.Summary;
                if (summary == null)
                    throw new OpenRouteServicemanagerException("ORS response contains no route summary.");

                tour.Distance = summary.Distance / 1000.0;
                tour.Time = summary.Duration / 60.0;

                await LoadImage(tour);
                _logger.Debug("Tour created successfully.");

                return tour;
            }
            //catch (Exception ex) when (ex is not OpenRouteServicemanagerException)
            //{
            //    _logger.Error($"Unexpected error: {ex.Message}");
            //    throw new OpenRouteServicemanagerException("Unexpected error during tour generation.", ex);
            //}
            catch (Exception ex)
            {
                _logger.Error("=== ERROR in GenerateTourFromTourAsync ===");
                _logger.Error($"Message: {ex.Message}");
                _logger.Error($"StackTrace: {ex.StackTrace}");
                _logger.Error($"InnerException: {ex.InnerException?.Message}");

                throw new OpenRouteServicemanagerException("Tour could not be retrieved from OpenRouteService", ex);
            }

        }

        private async Task<List<double>> GetCoordinatesAsync(string location)
        {
            var encodedLocation = Uri.EscapeDataString(location);
            var url = $"geocode/search?text={encodedLocation}&size=1";

            _logger.Debug($"Calling ORS Geocoding API: {url}");

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.Error($"Geocoding API error: {response.StatusCode} - {error}");
                throw new OpenRouteServicemanagerException($"Geocoding failed for: {location}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(json);
            var featuresJson = jsonDoc.RootElement.GetProperty("features").GetRawText();
            var result = JsonSerializer.Deserialize<List<GeocodeFeature>>(featuresJson);


            var coordinates = result?.FirstOrDefault()?.Geometry?.Coordinates;

            if (coordinates == null || coordinates.Count < 2)
            {
                throw new OpenRouteServicemanagerException($"No coordinates found for location: {location}");
            }

            return coordinates;
        }


        private async Task EnsureSuccessOrThrow(HttpResponseMessage response, string context)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.Error($"{context} API error: {response.StatusCode} - {error}");
                throw new OpenRouteServicemanagerException($"{context} API returned an error.");
            }
        }

        private void EnsureImageDirectoryExists()
        {
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
                _logger.Debug($"Created image directory: {_imagePath}");
            }
        }

        public async Task<bool> LoadImage(Tour tour, string? sessionId = null)
        {
            try
            {
                var startLat = tour.StartCoordinates[1].ToString(CultureInfo.InvariantCulture);
                var startLon = tour.StartCoordinates[0].ToString(CultureInfo.InvariantCulture);
                var endLat = tour.EndCoordinates[1].ToString(CultureInfo.InvariantCulture);
                var endLon = tour.EndCoordinates[0].ToString(CultureInfo.InvariantCulture);

                var imageUrl =
                    $"https://staticmap.openstreetmap.de/staticmap.php?" +
                    $"center={startLat},{startLon}" +
                    $"&zoom=8&size=600x400" +
                    $"&markers={startLat},{startLon},red-pushpin" +
                    $"|{endLat},{endLon},blue-pushpin";

                var safeName = string.Concat(tour.Name.Split(Path.GetInvalidFileNameChars()));
                var filePath = Path.Combine(Path.GetFullPath(_imagePath), $"{safeName}.jpeg");

                using var imageStream = await _httpClient.GetStreamAsync(imageUrl);
                using var fileStream = File.Create(filePath);
                await imageStream.CopyToAsync(fileStream);

                tour.ImagePath = filePath;

                _logger.Debug($"Image saved to: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Image download failed: " + ex.Message);
                return false;
            }
        }
    }
}
