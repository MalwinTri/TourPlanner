using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class GeocodeResponseWrapperDto
    {
        [JsonPropertyName("features")]
        public List<GeocodeFeature> Features { get; set; } = new();
    }
}
