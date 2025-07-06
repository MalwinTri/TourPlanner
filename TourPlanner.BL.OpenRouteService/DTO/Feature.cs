namespace TourPlanner.BL.OpenRouteService.DTO
{
    public class Feature
    {
        public FeatureProperties Properties { get; set; } = new();
        public FeatureGeometry Geometry { get; set; } = new();
    }
}