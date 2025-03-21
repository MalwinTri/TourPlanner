using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace TourPlanner.ViewModels
{
    public class TourListViewModel : INotifyPropertyChanged
    {
        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

        public ObservableCollection<Tour> Tours { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>
            {
                new Tour { Name = "Tour 1", Description = "A beautiful bike tour.", From = "City A", To = "City B", TransportType = "Bike", Distance = 15.5, EstimatedTime = "1h 30m" },
                new Tour { Name = "Tour 2", Description = "A scenic hike through the mountains.", From = "Mountain Base", To = "Mountain Peak", TransportType = "Hike", Distance = 8.2, EstimatedTime = "3h" },
                new Tour { Name = "Tour 3", Description = "A relaxing vacation tour.", From = "Beach A", To = "Beach B", TransportType = "Car", Distance = 120.0, EstimatedTime = "2h" },
                new Tour { Name = "Tour 4", Description = "A challenging running route.", From = "Park Entrance", To = "Park Exit", TransportType = "Run", Distance = 10.0, EstimatedTime = "1h" }
            };

            SelectedTour = Tours[0];
        }

    }
}