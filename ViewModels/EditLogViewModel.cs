using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class EditLogViewModel : BaseViewModel
    {
        private DateTime _dateTime;
        private string _difficulty;
        private double _totalDistance;
        private TimeSpan _totalTime;
        private int _rating;
        private string _comment;

        public TourLog EditedLog { get; }

        public DateTime DateTime
        {
            get => _dateTime;
            set { _dateTime = value; OnPropertyChanged(); }
        }

        public string Difficulty
        {
            get => _difficulty;
            set { _difficulty = value; OnPropertyChanged(); }
        }

        public double TotalDistance
        {
            get => _totalDistance;
            set { _totalDistance = value; OnPropertyChanged(); }
        }

        public TimeSpan TotalTime
        {
            get => _totalTime;
            set { _totalTime = value; OnPropertyChanged(); }
        }

        public int Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public EditLogViewModel(TourLog log)
        {
            EditedLog = log;

            DateTime = log.DateTime;
            Difficulty = log.Difficulty;
            TotalDistance = log.TotalDistance;
            TotalTime = log.TotalTime;
            Rating = log.Rating;
            Comment = log.Comment;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object obj)
        {
            if (Rating < 1 || Rating > 5)
            {
                MessageBox.Show("Rating must be between 1 and 5.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            EditedLog.DateTime = DateTime;
            EditedLog.Difficulty = Difficulty;
            EditedLog.TotalDistance = TotalDistance;
            EditedLog.TotalTime = TotalTime;
            EditedLog.Rating = Rating;
            EditedLog.Comment = Comment;

            CloseAction?.Invoke();
        }

        private void Cancel(object obj)
        {
            CloseAction?.Invoke();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
