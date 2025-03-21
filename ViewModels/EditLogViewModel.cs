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

        public string DateTime { get; set; }
        public string Difficulty { get; set; }
        public string TotalDistance { get; set; }
        public string TotalTime { get; set; }
        public string Rating { get; set; }
        public string Comment { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; } 

        public EditLogViewModel(TourLog log)
        {
            EditedLog = log;
            DateTime = log.DateTime.ToString("g");
            Difficulty = log.Difficulty;
            TotalDistance = log.TotalDistance.ToString();
            TotalTime = log.TotalTime;
            Rating = log.Rating.ToString();
            Comment = log.Comment;

            SaveCommand = new RelayCommand(SaveLog);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveLog(object parameter)
        {
            try
            {
                if (!System.DateTime.TryParse(DateTime, out System.DateTime parsedDateTime))
                {
                    MessageBox.Show("Invalid date/time format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!double.TryParse(TotalDistance, out double parsedTotalDistance))
                {
                    MessageBox.Show("Total distance must be a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(Rating, out int parsedRating) || parsedRating < 1 || parsedRating > 5)
                {
                    MessageBox.Show("Rating must be a number between 1 and 5.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                EditedLog.DateTime = parsedDateTime;
                EditedLog.Difficulty = Difficulty;
                EditedLog.TotalDistance = parsedTotalDistance.ToString();
                EditedLog.TotalTime = TotalTime;
                EditedLog.Rating = parsedRating.ToString();
                EditedLog.Comment = Comment;

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid input: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            CloseAction?.Invoke(); 
        }
    }
}
