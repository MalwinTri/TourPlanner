using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class Route
    {
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; } = new();
    }
}
