using System;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class EditLogViewModel : BaseViewModel
    {
        private TourLog _editedLog;
        public TourLog EditedLog
        {
            get => _editedLog;
            set
            {
                _editedLog = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateTime { get; set; }
        public string Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public EditLogViewModel(TourLog log)
        {
            _editedLog = log;

            // Populate fields with existing data
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
            // Optional: Add more validation logic if needed
            if (Rating < 1 || Rating > 5)
            {
                MessageBox.Show("Rating must be between 1 and 5.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Apply changes back to the TourLog object
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
    }
}
