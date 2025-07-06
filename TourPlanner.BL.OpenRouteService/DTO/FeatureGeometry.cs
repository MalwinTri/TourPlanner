using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class FeatureGeometry
    {
        [JsonPropertyName("coordinates")]
        public List<List<double>> Coordinates { get; set; } = new();
    }
}