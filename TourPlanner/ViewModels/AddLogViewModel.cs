using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class AddLogViewModel : INotifyPropertyChanged
    {
        private string _dateTime;
        private string _distance;
        private string _totalTime;
        private string _rating;
        private string _comment;

        private readonly Action<TourLog> _addLogAction;

        public string DateTime
        {
            get => _dateTime;
            set { _dateTime = value; OnPropertyChanged(nameof(DateTime)); }
        }

        public string Distance
        {
            get => _distance;
            set { _distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        public string TotalTime
        {
            get => _totalTime;
            set { _totalTime = value; OnPropertyChanged(nameof(TotalTime)); }
        }

        public string Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(nameof(Rating)); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(nameof(Comment)); }
        }

        private string _difficulty;
        public string Difficulty
        {
            get => _difficulty;
            set
            {
                _difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public AddLogViewModel(Action<TourLog> addLogAction)
        {
            _addLogAction = addLogAction;
            DateTime = System.DateTime.Now.ToString("g");
            SaveCommand = new RelayCommand(SaveLog);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveLog(object obj)
        {
            if (!System.DateTime.TryParse(DateTime, out var parsedDateTime))
            {
                MessageBox.Show("Invalid date/time format.");
                return;
            }

            if (!double.TryParse(Distance, out var parsedDistance))
            {
                MessageBox.Show("Distance must be a number.");
                return;
            }

            if (!TimeSpan.TryParse(TotalTime, out var parsedTime))
            {
                MessageBox.Show("Total time must be a valid timespan (e.g. 01:30:00).");
                return;
            }

            if (!int.TryParse(Rating, out var parsedRating) || parsedRating < 1 || parsedRating > 5)
            {
                MessageBox.Show("Rating must be a number between 1 and 5.");
                return;
            }

            var newLog = new TourLog
            {
                Date = parsedDateTime, // statt DateTime
                TotalTime = parsedTime,
                Rating = parsedRating,
                Comment = Comment,
                Difficulty = string.IsNullOrWhiteSpace(Difficulty) ? null : double.Parse(Difficulty)
            };

            _addLogAction?.Invoke(newLog);
            CloseAction?.Invoke();
        }

        private void Cancel(object obj)
        {
            CloseAction?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
