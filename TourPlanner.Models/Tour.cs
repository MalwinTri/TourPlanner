using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models
{
    public class Tour
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? From { get; set; }
        public string? To { get; set; }
        public string Transport { get; set; }

        public double? Distance { get; set; }
        public double? Time { get; set; }
        public string? ImagePath { get; set; }
        public double? Popularity { get; set; }
        public double? ChildFriendliness { get; set; }

        public List<double> StartCoordinates { get; set; } = new List<double>();
        public List<double> EndCoordinates { get; set; } = new List<double>();

        public Tour(Guid id, string? name, string? description, string? from, string? to, string transport)
        {
            Id = id;
            Name = name;
            Description = description;
            From = from;
            To = to;
            Transport = transport;
        }
    }
}