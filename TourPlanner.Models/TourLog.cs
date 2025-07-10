using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    public class TourLog
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tourid")]
        public Guid TourId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }
        public string? Comment { get; set; }
        public double? Difficulty { get; set; }
        public TimeSpan? TotalTime { get; set; }
        public double? Rating { get; set; }

        public double? TotalDistance { get; set; }

        public TourLog() { }

        public TourLog(Guid id, Guid tourId, DateTime date, string? comment, double? difficulty, double? totalTime, double? rating)
        {
            Id = id;
            TourId = tourId;
            Date = date;
            Comment = comment;
            Difficulty = difficulty;
            TotalTime = totalTime;
            Rating = rating;
            TotalDistance = totalDistance;
        }
    }


}
