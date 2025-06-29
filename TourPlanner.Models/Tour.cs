using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models
{
    namespace TourPlanner.Models
    {
        public class Tour
        {
            public Guid Id { get; set; } = Guid.NewGuid(); // automatisch neue ID

            public string Name { get; set; }
            public string Description { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string TransportType { get; set; }
            public TimeSpan EstimatedTime { get; set; }     // ⬅ richtige Typen!
            public string RouteInformation { get; set; }

            public double Distance { get; set; }            // ⬅ hinzufügen
            public string ImagePath { get; set; }           // ⬅ hinzufügen

            public Tour() { }

            public Tour(Guid id, string name, string description, string from, string to, string transportType, TimeSpan estimatedTime, string routeInformation, double distance, string imagePath)
            {
                Id = id;
                Name = name;
                Description = description;
                From = from;
                To = to;
                TransportType = transportType;
                EstimatedTime = estimatedTime;
                RouteInformation = routeInformation;
                Distance = distance;
                ImagePath = imagePath;
            }
        }
    }
}