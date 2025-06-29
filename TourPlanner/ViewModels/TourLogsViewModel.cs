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
                    Date = DateTime.Now.AddDays(-1),
                    Comment = "Relaxed morning hike",
                    Difficulty = 1.0,
                    TotalDistance = 5.2,
                    TotalTime = TimeSpan.FromMinutes(60),
                    Rating = 4
                },
                new TourLog
                {
                    Date = DateTime.Now.AddDays(-2),
                    Comment = "Rainy but fun",
                    Difficulty = 2.0,
                    TotalDistance = 12.0,
                    TotalTime = TimeSpan.FromMinutes(110),
                    Rating = 3
                },
                new TourLog
                {
                    Date = DateTime.Now.AddDays(-3),
                    Comment = "Challenging terrain",
                    Difficulty = 3.0,
                    TotalDistance = 20.5,
                    TotalTime = TimeSpan.FromMinutes(180),
                    Rating = 5
                },
                new TourLog
                {
                    Date = DateTime.Now.AddDays(-4),
                    Comment = "Beautiful sunset tour",
                    Difficulty = 1.0,
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

            var window = new EditLogWindow(SelectedLog)
            {
                Owner = Application.Current.MainWindow
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
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
