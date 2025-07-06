using System.Text.Json.Serialization;

namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class GeocodeGeometry
    {
        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; } = new(); // [lon, lat] ✔️
    }
}
