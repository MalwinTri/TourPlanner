using System.Windows.Input;
using TourPlanner.BL;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

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


        private readonly ITourPlannerManager _tourManager;
        private readonly ITourPlannerLogManager _tourLogManager;
        private readonly IReportGenerator _reportGenerator;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;

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
            TourLogViewModel tourLogViewModel, SearchViewModel searchViewModel, ITourPlannerManager tourManager, ITourPlannerLogManager tourLogManager,
            IReportGenerator reportGenerator, IExportManager exportManager, IImportManager importManager, ILoggerFactory loggerFactory,
            IWeatherGenerator weatherGenerator)
        {
            _logger = loggerFactory.CreateLogger<MainViewModel>();

            _addTourViewModel = addTourViewModel;
            _editTourViewModel = editTourViewModel;
            _tourLogViewModel = tourLogViewModel;
            _tourManager = tourManager;
            _tourLogManager = tourLogManager;
            _reportGenerator = reportGenerator;
            _exportManager = exportManager;
            _importManager = importManager;

            TourListViewModel = tourListViewModel;
            TourPreviewViewModel = tourPreviewViewModel;
            SearchViewModel = searchViewModel;

            SearchTours(null);


            _addTourViewModel!.TourAdded += async (_, tour) =>
            {
                try
                {
                    IsBusy = true;
                    IsBusyText = "Adding Tour...";
                    var t = await _tourManager.Add(tour);
                    IsBusy = false;

                    if (t == null)
                    {
                        NavigationService?.ShowMessageBox("Tour could not be added", "Error");
                        return;
                    }

                    TourListViewModel.AddTour(t);
                }
                catch (PostgresDataBaseException e)
                {
                    _logger.Error("Tour could not be added");
                    _logger.Error(e.Message);
                    NavigationService?.ShowMessageBox("Tour could not be added", "Error");
                    IsBusy = false;
                }
                catch (MapquestReturnedNullException e)
                {
                    _logger.Error("Tour could not be retrieved from mapquest");
                    _logger.Error(e.Message);
                    NavigationService?.ShowMessageBox("Tour could not be retrieved from mapquest", "Error");
                    IsBusy = false;
                }
            };

            _addTourViewModel.ValidationsFailed += (_, errors) =>
            {
                const string errorString = "Validation failed:\n";
                NavigationService?.ShowMessageBox(errorString, "Validation Error");
            };

            _editTourViewModel!.TourEdited += async (_, tour) =>
            {
                try
                {
                    IsBusy = true;
                    IsBusyText = "Editing Tour...";
                    var t = await _tourManager.Edit(tour);

                    TourPreviewViewModel.SelectedTour = null;
                    TourPreviewViewModel.SelectedTour = t;
                    IsBusy = false;

                    if (t == null)
                    {
                        NavigationService?.ShowMessageBox("Tour could not be edited", "Error");
                        return;
                    }

                    TourListViewModel.EditTour(t);
                }
                catch (PostgresDataBaseException e)
                {
                    _logger.Error("Tour could not be added");
                    _logger.Error(e.Message);
                    NavigationService?.ShowMessageBox("Tour could not be added", "Error");
                    IsBusy = false;
                }
                catch (MapquestReturnedNullException e)
                {
                    _logger.Error("Tour could not be retrieved from mapquest");
                    _logger.Error(e.Message);
                    NavigationService?.ShowMessageBox("Tour could not be retrieved from mapquest", "Error");
                    IsBusy = false;
                }

            };

            _editTourViewModel.ValidationsFailed += (_, errors) =>
            {
                const string errorString = "Validation failed:\n";
                NavigationService?.ShowMessageBox(errorString, "Validation Error");
            };

            TourListViewModel.SelectedTourChanged += (_, tour) =>
            {
                TourPreviewViewModel.SelectedTour = tour;
                MySelectedTour = tour;
            };

            _tourLogViewModel.TourLogAdded += (_, tourLog) =>
            {
                try
                {
                    IsBusy = true;
                    IsBusyText = "Adding Tour Log...";
                    _tourLogManager.Add(tourLog);
                    IsBusy = false;
                }
                catch (PostgresDataBaseException e)
                {
                    _logger.Error("Tour Log could not be added");
                    _logger.Error(e.Message);
                    NavigationService?.ShowMessageBox("Tour Log could not be added", "Error");
                    IsBusy = false;
                }
            };

            _tourLogViewModel.ValidationsFailed += (_, errors) =>
            {
                const string errorString = "Validation failed:\n";
                NavigationService?.ShowMessageBox(errorString, "Validation Error");
            };

            searchViewModel.SearchTextChanged += (_, searchText) =>
            {
                IsBusy = true;
                IsBusyText = "Searching Tours...";
                SearchTours(searchText);
                IsBusy = false;
            };

            TourPreviewViewModel.TourDetailsOpened += (_, tour) =>
            {
                _tourDetailsViewModel = new TourDetailsViewModel(loggerFactory, tourLogManager, weatherGenerator, tour, tourManager);

                NavigationService?.NavigateTo(_tourDetailsViewModel);
            };

            ShowAddTourDialog = new RelayCommand((_) =>
            {
                NavigationService?.NavigateTo(_addTourViewModel);
            }, (_) => IsBusy == false);

            ShowEditTourDialog = new RelayCommand((_) =>
            {
                _editTourViewModel!.TourToEdit = MySelectedTour;
                NavigationService?.NavigateTo(_editTourViewModel);
            }, (_) => MySelectedTour != null && IsBusy == false);

            DeleteTour = new RelayCommand((_) =>
            {
                try
                {
                    if (MySelectedTour == null)
                    {
                        return;
                    }

                    tourManager.DeleteTour(MySelectedTour);
                    tourListViewModel.RemoveTour(MySelectedTour);
                }
                catch (PostgresDataBaseException)
                {
                    NavigationService?.ShowMessageBox("Tour could not be deleted", "Error");
                }
            }, (_) => MySelectedTour != null && IsBusy == false);


            ExportCommand = new RelayCommand(ExecuteExport, (_) => MySelectedTour != null && IsBusy == false);

            ImportCommand = new RelayCommand(ExecuteImport, (_) => IsBusy == false);

            ReportCommand = new RelayCommand(ExecuteReport, (_) => MySelectedTour != null && IsBusy == false);

            SummaryCommand = new RelayCommand(ExecuteSummary, (_) => IsBusy == false);
        }

        private async void ExecuteExport(object? _)
        {
            try
            {
                await ExportCommandHandler();
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error("Tour could not be exported");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Tour could not be exported", "Error");
                IsBusy = false;
            }
            catch (Exception e)
            {
                _logger.Error("Something went wrong");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Something went wrong", "Error");
                IsBusy = false;
            }
        }
        private async void ExecuteSummary(object? _)
        {
            try
            {
                await SummaryCommandHandler();
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error("Summary could not be saved");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Summary could not be saved", "Error");
                IsBusy = false;
            }
            catch (Exception e)
            {
                _logger.Error("Something went wrong");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Something went wrong", "Error");
                IsBusy = false;
            }
        }

        private async void ExecuteReport(object? _)
        {
            try
            {
                if (MySelectedTour == null)
                {
                    _logger.Error("SelectedTour is null");
                    NavigationService?.ShowMessageBox("SelectedTour is null", "Error");
                    return;
                }

                await ReportCommandHandler(MySelectedTour!);
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error("Report could not be saved");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Report could not be saved", "Error");
                IsBusy = false;
            }
            catch (Exception e)
            {
                _logger.Error("Something went wrong");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Something went wrong", "Error");
                IsBusy = false;
            }

        }

        private async void ExecuteImport(object? _)
        {
            try
            {
                IsBusy = true;
                IsBusyText = "Importing Tour...";

                var path = NavigationService?.OpenFileDialog("JSON Files (*.json)|*.json");
                if (path == null)
                {
                    IsBusy = false;
                    NavigationService?.ShowMessageBox("Import failed", "Import");
                    return;
                }

                var t = await _importManager.ImportTour(path);
                TourListViewModel.AddTour(t);
                IsBusy = false;

                NavigationService?.ShowMessageBox("Import successful", "Import");
            }
            catch (ImportReturnedNullException e)
            {
                _logger.Error("Import returned null");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Tour could not be imported", "Error");
                IsBusy = false;
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error("Tour could not be imported");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Tour could not be imported", "Error");
                IsBusy = false;
            }
            catch (Exception exception)
            {
                _logger.Error("Something went wrong");
                _logger.Error(exception.Message);
                NavigationService?.ShowMessageBox("Something went wrong", "Error");
                IsBusy = false;
            }

        }

        private async Task ExportCommandHandler()
        {
            IsBusy = true;
            IsBusyText = "Exporting Tour...";
            var tourLogs = _tourLogManager.FindMatchingTourLogs(MySelectedTour!).ToList();
            var result = await _exportManager.ExportTour(MySelectedTour!, tourLogs);
            IsBusy = false;

            NavigationService?.ShowMessageBox(result ? "Export successful" : "Export failed", "Export");
        }

        private async Task ReportCommandHandler(Tour tour)
        {
            IsBusy = true;
            IsBusyText = "Generating Report...";
            var result = await Task.Run(() => _reportGenerator.GenerateReport(tour));
            IsBusy = false;

            NavigationService?.ShowMessageBox(result ? "Saving report successful" : "Saving report failed", "Report");
        }
        private async Task SummaryCommandHandler()
        {
            IsBusy = true;
            IsBusyText = "Generating Summary...";
            var result = await Task.Run(() => _reportGenerator.GenerateSummary());
            IsBusy = false;

            NavigationService?.ShowMessageBox(result ? "Saving summary successful" : "Saving summary failed", "Summary");
        }


        private void SearchTours(string? searchText)
        {
            try
            {
                var tours = _tourManager.FindMatchingTours(searchText).ToList();
                CalculateComputedValues(tours);

                TourListViewModel.SetTours(tours);
            }
            catch (TourPlannerManagerException e)
            {
                _logger.Error("Computed attributes error");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Computed attributes error", "Error");
                IsBusy = false;
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error("Tours could not be retrieved");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Tours could not be retrieved", "Error");
                IsBusy = false;
            }
        }

        private void CalculateComputedValues(List<Tour> tours)
        {
            foreach (var t in tours)
            {
                t.Popularity = _tourManager.CalculatePopularity(t);
                t.ChildFriendliness = _tourManager.CalculateChildFriendliness(t);
                _tourManager.Edit(t);
            }
        }
    }
}