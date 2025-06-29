using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Models.TourPlanner.Models;
using TourPlanner.Views;

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

        public ICommand AddTourCommand { get; }
        public ICommand DeleteTourCommand { get; }
        public ICommand ModifyTourCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>
            {
                new Tour { Name = "Tour 1", Description = "A beautiful bike tour.", From = "City A", To = "City B", TransportType = "Bike", Distance = 15.5, EstimatedTime = TimeSpan.Parse("01:00:00"), RouteInformation = "dasd" },
                new Tour { Name = "Tour 2", Description = "A scenic hike through the mountains.", From = "Mountain Base", To = "Mountain Peak", TransportType = "Hike", Distance = 8.2, EstimatedTime = TimeSpan.Parse("03:00:00"), RouteInformation = "dasd" },
                new Tour { Name = "Tour 3", Description = "A relaxing vacation tour.", From = "Beach A", To = "Beach B", TransportType = "Car", Distance = 120.0, EstimatedTime = TimeSpan.Parse("02:00:00"), RouteInformation = "dasd" },
                new Tour { Name = "Tour 4", Description = "A challenging running route.", From = "Park Entrance", To = "Park Exit", TransportType = "Run", Distance = 10.0, EstimatedTime = TimeSpan.Parse("01:00:00"), RouteInformation = "dasd" }
            };

            AddTourCommand = new RelayCommand(_ => OpenAddTourWindow());
            DeleteTourCommand = new RelayCommand(_ => OpenDeleteTourWindow(), _ => SelectedTour != null);
            ModifyTourCommand = new RelayCommand(_ => OpenEditTourWindow(), _ => SelectedTour != null);
        }

        private void OpenAddTourWindow()
        {
            var win = new AddTourWindow(newTour =>
            {
                Tours.Add(newTour);
                MessageBox.Show("Tour added!");
            });

            win.ShowDialog();
        }

        private void OpenDeleteTourWindow()
        {
            var deleteWindow = new DeleteTourWindow();
            if (deleteWindow.ShowDialog() == true && deleteWindow.Confirmed)
            {
                Tours.Remove(SelectedTour);
                SelectedTour = null;
            }
        }

        private void OpenEditTourWindow()
        {
            var editWindow = new EditTourWindow(SelectedTour);
            if (editWindow.ShowDialog() == true)
            {
                var updated = editWindow.UpdatedTour;

                SelectedTour.Name = updated.Name;
                SelectedTour.Description = updated.Description;
                SelectedTour.From = updated.From;
                SelectedTour.To = updated.To;
                SelectedTour.Distance = updated.Distance;
                SelectedTour.EstimatedTime = updated.EstimatedTime;
                SelectedTour.RouteInformation = updated.RouteInformation;
                SelectedTour.TransportType = updated.TransportType;
                SelectedTour.ImagePath = updated.ImagePath;
            }
        }
    }
}
