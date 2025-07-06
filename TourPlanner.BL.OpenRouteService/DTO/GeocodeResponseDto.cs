using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class GeocodeResponseDto
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("features")]
        public List<GeocodeFeature> Features { get; set; } = new();
    }
}