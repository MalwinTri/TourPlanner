using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.ViewModels  // Namespace, sicherstellen, dass er zu deinem Projekt passt
{
    public class TourLog
    {
        public DateTime DateTime { get; set; }
        public string TotalTime { get; set; }
        public int Ranking { get; set; }
        public string Difficulty { get; set; }
        public string Comment { get; set; }
    }
}
