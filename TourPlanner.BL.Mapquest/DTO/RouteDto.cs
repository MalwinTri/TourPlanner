namespace TourPlanner.BL.Mapquest.DTO
{
    internal class RouteDto
    {
        public double Distance { get; set; }
        public string? SessionId { get; set; }
        public double Time { get; set; }
        public List<LocationDto>? Locations { get; set; }
    }
}
