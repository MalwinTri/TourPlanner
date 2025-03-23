using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Views;

namespace TourPlanner.ViewModels
{
    public class TourLogsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TourLog> Logs { get; set; }

        private TourLog _selectedLog;
        public TourLog SelectedLog
        {
            get => _selectedLog;
            set
            {
                _selectedLog = value;
                OnPropertyChanged(nameof(SelectedLog));
                (EditLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteLogCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddLogCommand { get; }
        public ICommand EditLogCommand { get; }
        public ICommand DeleteLogCommand { get; }

        public TourLogsViewModel()
        {
            Logs = new ObservableCollection<TourLog>
            {
                new TourLog
                {
                    DateTime = DateTime.Now.AddDays(-1),
                    Comment = "Relaxed morning hike",
                    Difficulty = "Easy",
                    TotalDistance = 5.2,
                    TotalTime = TimeSpan.FromMinutes(60),
                    Rating = 4
                },
                new TourLog
                {
                    DateTime = DateTime.Now.AddDays(-2),
                    Comment = "Rainy but fun",
                    Difficulty = "Medium",
                    TotalDistance = 12.0,
                    TotalTime = TimeSpan.FromMinutes(110),
                    Rating = 3
                },
                new TourLog
                {
                    DateTime = DateTime.Now.AddDays(-3),
                    Comment = "Challenging terrain",
                    Difficulty = "Hard",
                    TotalDistance = 20.5,
                    TotalTime = TimeSpan.FromMinutes(180),
                    Rating = 5
                },
                new TourLog
                {
                    DateTime = DateTime.Now.AddDays(-4),
                    Comment = "Beautiful sunset tour",
                    Difficulty = "Easy",
                    TotalDistance = 7.7,
                    TotalTime = TimeSpan.FromMinutes(70),
                    Rating = 4
                }
            };

            AddLogCommand = new RelayCommand(_ => AddLog());
            EditLogCommand = new RelayCommand(_ => EditLog(), _ => CanEditOrDeleteLog());
            DeleteLogCommand = new RelayCommand(_ => DeleteLog(), _ => CanEditOrDeleteLog());
        }

        private void AddLog()
        {
            var win = new AddLogWindow(newLog =>
            {
                Logs.Add(newLog);
                MessageBox.Show("Log added!");
            });

            win.ShowDialog();
        }

        private void EditLog()
        {
            if (SelectedLog == null) return;

            var win = new EditLogWindow(SelectedLog);
            if (win.ShowDialog() == true)
            {
                var updated = win.GetUpdatedLog();

                SelectedLog.DateTime = updated.DateTime;
                SelectedLog.Difficulty = updated.Difficulty;
                SelectedLog.TotalDistance = updated.TotalDistance;
                SelectedLog.TotalTime = updated.TotalTime;
                SelectedLog.Rating = updated.Rating;
                SelectedLog.Comment = updated.Comment;

                MessageBox.Show("Log successfully updated!");
            }
        }

        private void DeleteLog()
        {
            var win = new DeleteTourLogWindow();
            if (win.ShowDialog() == true)
            {
                Logs.Remove(SelectedLog);
                MessageBox.Show("Log deleted successfully!");
            }
        }

        private bool CanEditOrDeleteLog() => SelectedLog != null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
