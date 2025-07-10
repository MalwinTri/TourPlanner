using System.Net.Http.Headers;
using System.Runtime.Versioning;
using System.Text.Json;
using TourPlanner.BL.Exceptions;
using TourPlanner.BL.Mapquest;
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
        private readonly IMapImageService _mapImageService;
        private readonly string _apiKey;

        public OpenRouteServiceTourGenerator(
            IOpenRouteServiceConfiguration orsConfig,
            IMapImageService mapImageService,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OpenRouteServiceTourGenerator>();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(orsConfig.OpenRouteServiceApiUrl)
            };

            _apiKey = orsConfig.ApiKey;
            _mapImageService = mapImageService;
        }

        public async Task<Tour?> GenerateTourFromTourAsync(Tour tour)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tour.From) || string.IsNullOrWhiteSpace(tour.To))
                    throw new OpenRouteServicemanagerException("Start or destination location is missing.");

                _logger.Warning($"[GenerateTour] Request for '{tour.Name}' from '{tour.From}' to '{tour.To}'");

                // 1. Geocode Start/End
                tour.StartCoordinates = await GetCoordinatesAsync(tour.From!);
                tour.EndCoordinates = await GetCoordinatesAsync(tour.To!);
                _logger.Debug($"StartCoords: {string.Join(", ", tour.StartCoordinates)}");
                _logger.Debug($"EndCoords: {string.Join(", ", tour.EndCoordinates)}");

                // 2. Select Profile (driving, walking etc.)
                var profile = tour.Transport?.ToLower() switch
                {
                    "car" => "driving-car",
                    "bike" or "bicycle" => "cycling-regular",
                    "walk" or "walking" => "foot-walking",
                    _ => "driving-car"
                };

                // 3. Build ORS Request
                var requestBody = new
                {
                    coordinates = new[]
                    {
                        new[] { tour.StartCoordinates[0], tour.StartCoordinates[1] },
                        new[] { tour.EndCoordinates[0], tour.EndCoordinates[1] }
                    }
                };

                var requestJson = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, $"v2/directions/{profile}")
                {
                    Content = new StringContent(requestJson)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _logger.Debug($"[ORS] Profile: {profile}");
                _logger.Debug($"[ORS] JSON: {requestJson}");

                // 4. Call ORS
                var response = await _httpClient.SendAsync(request);
                await EnsureSuccessOrThrow(response, "Directions API");

                var responseJson = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var routeResponse = JsonSerializer.Deserialize<ORSRouteResponse>(responseJson, options);

                _logger.Warning($"[ORS RAW RESPONSE] {responseJson}");

                var summary = routeResponse?.Routes?.FirstOrDefault()?.Summary;

                if (summary == null)
                    throw new OpenRouteServicemanagerException("No summary in ORS response");

                // 5. Parse & set
                tour.Distance = Math.Round(summary.Distance / 1000.0, 2);     // in km
                tour.Time = Math.Round(summary.Duration, 2);  //  Minuten                                                                    // in Minuten

                _logger.Warning($"[Routing Done] Distance: {tour.Distance} km | Time: {tour.Time} min");

                // 6. Load static map image
                await _mapImageService.LoadImage(tour);

                return tour;
            }
            catch (Exception ex)
            {
                _logger.Error($"[GenerateTour] ERROR: {ex.Message}\n{ex.StackTrace}");
                throw new OpenRouteServicemanagerException("Failed to generate tour", ex);
            }
        }

        private async Task<List<double>> GetCoordinatesAsync(string location)
        {
            var encoded = Uri.EscapeDataString(location);
            var request = new HttpRequestMessage(HttpMethod.Get, $"geocode/search?text={encoded}&size=1");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.SendAsync(request);
            await EnsureSuccessOrThrow(response, "Geocoding API");

            var json = await response.Content.ReadAsStringAsync();

            var doc = JsonDocument.Parse(json);
            var featuresJson = doc.RootElement.GetProperty("features").GetRawText();
            var result = JsonSerializer.Deserialize<List<GeocodeFeature>>(featuresJson);

            var coordinates = result?.FirstOrDefault()?.Geometry?.Coordinates;

            if (coordinates == null || coordinates.Count < 2)
                throw new Exception("Coordinates not found");

            return new List<double> { coordinates[0], coordinates[1] }; // [lon, lat] 
        }

        private async Task EnsureSuccessOrThrow(HttpResponseMessage response, string context)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new OpenRouteServicemanagerException($"{context} API error: {response.StatusCode} - {error}");
            }
        }
    }
}
