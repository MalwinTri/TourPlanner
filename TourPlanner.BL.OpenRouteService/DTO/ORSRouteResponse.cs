using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class ORSRouteResponse
    {
        [JsonPropertyName("routes")]
        public List<Route> Routes { get; set; } = new();
    }
}
