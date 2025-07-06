using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class GeocodeFeature
    {
        [JsonPropertyName("geometry")]
        public GeocodeGeometry Geometry { get; set; } = new();
    }

}