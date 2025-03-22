using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models  
{
    public class TourLog
    {
        public DateTime DateTime { get; set; }
        public string Comment { get; set; }
        public string Difficulty { get; set; }
        public double TotalDistance { get; set; }  
        public TimeSpan TotalTime { get; set; }    
        public int Rating { get; set; }              
    }

}
