using System;
using System.ComponentModel;

namespace TourPlanner.Models
{
    public class TourLog : INotifyPropertyChanged
    {
        private DateTime _dateTime;
        private string _comment;
        private string _difficulty;
        private double _totalDistance;
        private TimeSpan _totalTime;
        private int _rating;

        public DateTime DateTime
        {
            get => _dateTime;
            set { _dateTime = value; OnPropertyChanged(nameof(DateTime)); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(nameof(Comment)); }
        }

        public string Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(nameof(Difficulty)); }
        }

        public double TotalDistance
        {
            get => _totalDistance;
            set { _totalDistance = value; OnPropertyChanged(nameof(TotalDistance)); }
        }

        public TimeSpan TotalTime
        {
            get => _totalTime;
            set { _totalTime = value; OnPropertyChanged(nameof(TotalTime)); }
        }

        public int Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(nameof(Rating)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
