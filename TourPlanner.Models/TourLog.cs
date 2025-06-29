using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models
{
    public class TourLog
    {
        public Guid Id { get; set; }
        public Guid? TourId { get; set; }
        public DateTime Date { get; set; }
        public string? Comment { get; set; }
        public double? Difficulty { get; set; }
        public double? TotalTime { get; set; }
        public double? Rating { get; set; }

        public TourLog(Guid id, Guid? tourId, DateTime date, string? comment, double? difficulty, double? totalTime, double? rating)
        {
            Id = id;
            TourId = tourId;
            Date = date;
            Comment = comment;
            Difficulty = difficulty;
            TotalTime = totalTime;
            Rating = rating;
        }
    }
}
