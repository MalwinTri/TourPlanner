using System.ComponentModel;

namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ToursListViewModel _toursListViewModel = new ToursListViewModel();

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

        public MainViewModel()
        {
            ToursListViewModel = new ToursListViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
