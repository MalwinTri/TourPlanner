using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    [Table("tours")]
    public class Tour
    {

        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("From")]
        public string? From { get; set; }

        [Column("To")]
        public string? To { get; set; }

        [Column("transport")]
        public string Transport { get; set; }

        [Column("distance")]
        public double? Distance { get; set; }

        [Column("time")]
        public double? Time { get; set; }

        [Column("imagepath")]
        public string? ImagePath { get; set; }

        [Column("popularity")]
        public double? Popularity { get; set; }

        [Column("childfriendliness")]
        public double? ChildFriendliness { get; set; }

        [Column("startcoordinates")]
        public List<double> StartCoordinates { get; set; } = new();

        [Column("endcoordinates")]
        public List<double> EndCoordinates { get; set; } = new();

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
