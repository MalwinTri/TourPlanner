using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public ICommand ShowTourListCommand { get; }
        public ICommand ShowTourLogsCommand { get; }

        private readonly TourListViewModel _tourListViewModel;
        private readonly TourLogsViewModel _tourLogsViewModel;

        public MainWindowViewModel()
        {
            _tourListViewModel = new TourListViewModel();
            _tourLogsViewModel = new TourLogsViewModel();

            ShowTourListCommand = new RelayCommand(_ => CurrentView = _tourListViewModel);
            ShowTourLogsCommand = new RelayCommand(_ => CurrentView = _tourLogsViewModel);

            // Default View
            CurrentView = _tourListViewModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
