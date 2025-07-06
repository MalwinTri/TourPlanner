using System.Text.Json.Serialization;
using TourPlanner.Models;

namespace TourPlanner.BL.Mapquest.DTO
{
    internal class MapquestTourDto
    {
        [JsonPropertyName("route")]
        public RouteDto? Route { get; set; }
        [JsonPropertyName("options")]
        public OptionsDto? Options { get; set; }


        public Tour AddToTour(Tour tour)
        {
            tour.Distance = Route!.Distance;
            tour.Time = Route.Time;
            tour.EndCoordinates.Clear();
            tour.EndCoordinates.Add(Route.Locations![1].LatLng!.Lat);
            tour.EndCoordinates.Add(Route.Locations[1].LatLng!.Lng);
            tour.StartCoordinates.Clear();
            tour.StartCoordinates.Add(Route.Locations[0].LatLng!.Lat);
            tour.StartCoordinates.Add(Route.Locations[0].LatLng!.Lng);

            return tour;
        }
    }
}
