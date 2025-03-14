using System.Collections.ObjectModel;
using System.ComponentModel;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ToursListViewModel _toursListViewModel;
        private ObservableCollection<TourLog> _selectedTourLogs = new ObservableCollection<TourLog>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ToursListViewModel ToursListViewModel
        {
            get => _toursListViewModel;
            set
            {
                if (_toursListViewModel != value)
                {
                    _toursListViewModel = value;
                    OnPropertyChanged(nameof(ToursListViewModel));
                }
            }
        }

        public ObservableCollection<TourLog> SelectedTourLogs
        {
            get => _selectedTourLogs;
            set
            {
                if (_selectedTourLogs != value)
                {
                    _selectedTourLogs = value;
                    OnPropertyChanged(nameof(SelectedTourLogs));
                }
            }
        }

        public MainViewModel()
        {
            ToursListViewModel = new ToursListViewModel();

            // Beispiel für das Hinzufügen von TourLogs:
            SelectedTourLogs.Add(new TourLog
            {
                DateTime = DateTime.Now,
                TotalTime = "1h 30min",
                Ranking = 5,
                Difficulty = "Medium",
                Comment = "Schöne Tour!"
            });
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
