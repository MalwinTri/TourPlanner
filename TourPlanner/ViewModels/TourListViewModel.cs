using System.Collections.ObjectModel;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class TourListViewModel : BaseViewModel
    {
        public event EventHandler<Tour?>? SelectedTourChanged;
        public ObservableCollection<Tour> Tours { get; } = new();

        private Tour? _selectedTour;
        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged();
                OnSelectedTourChanged();
            }
        }
        public void SetTours(IEnumerable<Tour> tours)
        {
            Tours.Clear();
            tours.ToList().ForEach(j => Tours.Add(j));
        }

        public void RemoveTour(Tour tour)
        {
            Tours.Remove(tour);
        }

        public void AddTour(Tour tour)
        {
            Tours.Add(tour);
        }

        public void EditTour(Tour tour)
        {
            var item = Tours.FirstOrDefault(t => t.Id == tour.Id);
            if (item == null)
            {
                NavigationService?.ShowMessageBox("Tour not found", "Error");
                return;
            }
            var index = Tours.IndexOf(item);
            RemoveTour(tour);
            Tours.Insert(index, tour);
            SelectedTour = tour;
        }

        private void OnSelectedTourChanged()
        {
            SelectedTourChanged?.Invoke(this, SelectedTour);
        }
    }
}
