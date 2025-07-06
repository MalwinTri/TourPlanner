using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text.Json;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.OpenRouteService.DTO;
using TourPlanner.Logging;
using TourPlanner.Models;

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
                if (string.IsNullOrWhiteSpace(tour.From) || string.IsNullOrWhiteSpace(tour.To))
                {
                    _logger.Error("Tour.From or Tour.To is null or empty.");
                    throw new OpenRouteServicemanagerException("Start or destination location is missing.");
                }

                _logger.Warning($"[Start] Generating tour: {tour.Name} ({tour.From} → {tour.To})");

                tour.StartCoordinates = await GetCoordinatesAsync(tour.From!);
                _logger.Debug($"[Coordinates] From '{tour.From}' → {string.Join(", ", tour.StartCoordinates)}");

                tour.EndCoordinates = await GetCoordinatesAsync(tour.To!);
                _logger.Debug($"[Coordinates] To   '{tour.To}' → {string.Join(", ", tour.EndCoordinates)}");

                var requestBody = new
                {
                    coordinates = new[] { tour.StartCoordinates, tour.EndCoordinates }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);
                _logger.Debug($"[Request JSON] {requestJson}");

                var request = new HttpRequestMessage(HttpMethod.Post, "v2/directions/driving-car")
                {
                    Content = new StringContent(requestJson)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _logger.Warning("[HTTP] Sending directions request...");

                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessOrThrow(response, "Directions API");

                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.Debug($"[HTTP Response JSON] {responseJson}");

                var routeResponse = JsonSerializer.Deserialize<ORSRouteResponse>(responseJson);

                if (routeResponse == null || routeResponse.Routes == null)
                {
                    _logger.Error("[Deserialization] ORSRouteResponse is null.");
                    throw new OpenRouteServicemanagerException("Failed to parse directions response.");
                }

                var summary = routeResponse.Routes.FirstOrDefault()?.Summary;

                if (summary == null)
                {
                    _logger.Error("[ORS] No summary in response.");
                    throw new OpenRouteServicemanagerException("ORS response contains no route summary.");
                }

                tour.Distance = summary.Distance / 1000.0;
                tour.Time = summary.Duration / 60.0;

                _logger.Warning($"[Result] Distance: {tour.Distance} km, Time: {tour.Time} min");

                var imageSuccess = await LoadImage(tour);
                if (!imageSuccess)
                {
                    _logger.Warning("[Image] Map image could not be downloaded.");
                }

                _logger.Warning("[Success] Tour generated successfully.");
                _logger.Warning($"[Tour Result] ImagePath: {tour.ImagePath}");
                return tour;
            }
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
                if (File.Exists(_imagePath + $"{tour.Name}.Jpeg"))
                {
                    File.Delete(_imagePath + $"{tour.Name}.Jpeg");
                }
                image.Save(_imagePath + $"{tour.Name}.Jpeg", ImageFormat.Jpeg);
                tour.ImagePath = _imagePath + $"{tour.Name}.Jpeg";

                _logger.Debug($"Mapquest image saved to {_imagePath + $"{tour.Name}.Jpeg"}");

                return true;
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
                throw;
            }
        }
    }
}
