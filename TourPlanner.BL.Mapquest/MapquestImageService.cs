using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Versioning;
using System.Text.Json;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.BL.Mapquest
{
    [SupportedOSPlatform("windows")]
    public class MapquestImageService : IMapImageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _imagePath;
        private readonly string _apiKey;

        public MapquestImageService(IMapquestConfiguration config, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MapquestImageService>();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(config.MapquestApiUrl ?? "https://www.mapquestapi.com/")
            };

            _imagePath = string.IsNullOrWhiteSpace(config.ImagePath) ? "Images" : config.ImagePath;
            _apiKey = string.IsNullOrWhiteSpace(config.MapquestApiKey)
                ? throw new ArgumentNullException(nameof(config.MapquestApiKey), "MapQuest API key is missing.")
                : config.MapquestApiKey;
        }

        public async Task<bool> LoadImage(Tour tour, string? sessionId = null)
        {
            try
            {
                if (tour.StartCoordinates == null || tour.EndCoordinates == null ||
                    tour.StartCoordinates.Count < 2 || tour.EndCoordinates.Count < 2)
                {
                    _logger.Error("[MapQuest] Tour coordinates are invalid or missing.");
                    return false;
                }

                var from = $"{tour.StartCoordinates[1].ToString(CultureInfo.InvariantCulture)},{tour.StartCoordinates[0].ToString(CultureInfo.InvariantCulture)}"; // lat, lon
                var to = $"{tour.EndCoordinates[1].ToString(CultureInfo.InvariantCulture)},{tour.EndCoordinates[0].ToString(CultureInfo.InvariantCulture)}";     // lat, lon

                var routeSession = await GetMapQuestRouteSessionAsync(from, to);
                if (string.IsNullOrEmpty(routeSession))
                {
                    _logger.Error("[MapQuest] Route session could not be retrieved.");
                    return false;
                }

                var requestUrl = $"staticmap/v5/map?key={_apiKey}" +
                                 $"&session={routeSession}" +
                                 $"&size=1000,700@2x" +
                                 $"&routeColor=ff0000" +
                                 $"&routeWidth=5";

                _logger.Debug($"[MapQuest] Static map URL: {requestUrl}");

                using var responseStream = await _httpClient.GetStreamAsync(requestUrl);
                using var image = Image.FromStream(responseStream);

                var safeName = string.Concat(tour.Name.Split(Path.GetInvalidFileNameChars()));
                var directoryPath = Path.GetFullPath(_imagePath);
                var filePath = Path.Combine(directoryPath, $"{safeName}_{Guid.NewGuid()}.jpeg"); 

                Directory.CreateDirectory(directoryPath);
                if (File.Exists(filePath)) File.Delete(filePath);

                image.Save(filePath, ImageFormat.Jpeg);
                tour.ImagePath = new Uri(Path.GetFullPath(filePath)).AbsoluteUri;

                _logger.Debug($"[MapQuest] Image saved: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"[MapQuest] Failed to load/save image: {ex.Message}");
                return false;
            }
        }

        private async Task<string?> GetMapQuestRouteSessionAsync(string from, string to)
        {
            try
            {
                var url = $"directions/v2/route?key={_apiKey}&from={from}&to={to}";
                _logger.Debug($"[MapQuest] Getting directions: {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Error($"[MapQuest] Directions failed: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var session = doc.RootElement
                                 .GetProperty("route")
                                 .GetProperty("sessionId")
                                 .GetString();

                _logger.Debug($"[MapQuest] SessionId retrieved: {session}");
                return session;
            }
            catch (Exception ex)
            {
                _logger.Error($"[MapQuest] Failed to get session ID: {ex.Message}");
                return null;
            }
        }
    }
}