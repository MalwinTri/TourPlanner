using System.ComponentModel.DataAnnotations.Schema;

namespace TourPlanner.Models
{
    [Table("tourlogs")]
    public class TourLog
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("tourid")]
        public Guid TourId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("difficulty")]
        public double? Difficulty { get; set; }

        [Column("totaltime")]
        public double? TotalTime { get; set; }

        [Column("rating")]
        public double? Rating { get; set; }

        public TourLog(Guid id, Guid tourId, DateTime date, string? comment, double? difficulty, double? totalTime, double? rating)
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
