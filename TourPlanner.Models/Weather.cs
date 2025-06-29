using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models
{
    public class Weather
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public string? WindDirection { get; set; } 
        public double Precipitation { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Location { get; set; }
    }
}
