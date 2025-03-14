using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TourPlanner.ViewModels
{
    public class ToursListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _tours = new();
        private string _selectedTour;

        public string SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour != value)
                {
                    _selectedTour = value;
                    OnPropertyChanged(nameof(SelectedTour));
                }
            }
        }

        public ObservableCollection<string> Tours
        {
            get => _tours;
            set
            {
                if (_tours != value)
                {
                    _tours = value;
                    OnPropertyChanged(nameof(Tours));
                }
            }
        }

        public ToursListViewModel()
        {
            // Beispiel-Touren-Daten
            Tours.Add("Wienerwald");
            Tours.Add("Dopperlhütte");
            Tours.Add("Figlwarte");
            Tours.Add("Dorfrunde");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
