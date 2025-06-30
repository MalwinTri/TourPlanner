using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private readonly AddTourViewModel? _addTourViewModel;
        private readonly EditTourViewModel? _editTourViewModel;
        private readonly TourLogViewModel _tourLogViewModel;
        private TourDetailsViewModel? _tourDetailsViewModel;


        public TourListViewModel TourListViewModel { get; }
        public TourPreviewViewModel TourPreviewViewModel { get; }
        public SearchViewModel SearchViewModel { get; }


        public ICommand DeleteTour { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ShowAddTourDialog { get; }
        public ICommand ShowEditTourDialog { get; }
        public ICommand ReportCommand { get; }
        public ICommand SummaryCommand { get; }

        private Tour? _mySelectedTour;

        public Tour? MySelectedTour
        {
            get => _mySelectedTour;
            set
            {
                _mySelectedTour = value;
                OnPropertyChanged();

                _logger.Debug("SelectedTour changed");

                if (MySelectedTour == null) return;
                (DeleteTour as RelayCommand)?.RaiseCanExecuteChanged();

            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                // otherwise CanExecute of command will not be re queried until there is some user interaction
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string? _isBusyText;
        //Busy Text Content
        public string? IsBusyText
        {
            get => _isBusyText;
            set
            {
                _isBusyText = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(AddTourViewModel? addTourViewModel, EditTourViewModel? editTourViewModel,
            TourListViewModel tourListViewModel, TourPreviewViewModel tourPreviewViewModel,
            TourLogViewModel tourLogViewModel, SearchViewModel searchViewModel, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MainViewModel>();

            _addTourViewModel = addTourViewModel;
            _editTourViewModel = editTourViewModel;
            _tourLogViewModel = tourLogViewModel;

            TourListViewModel = tourListViewModel;
            TourPreviewViewModel = tourPreviewViewModel;
            SearchViewModel = searchViewModel;

        }
    }
}