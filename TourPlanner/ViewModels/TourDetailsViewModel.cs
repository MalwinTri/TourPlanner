using System.Collections.ObjectModel;
using System.Windows.Input;
using TourPlanner.BL;
using TourPlanner.BL.Enum;
using TourPlanner.BL.Exceptions;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

namespace TourPlanner.ViewModels
{
    public class TourDetailsViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private readonly IWeatherGenerator _weatherGenerator;
        private readonly ITourPlannerLogManager _tourLogManager;
        private readonly ITourPlannerManager _tourManager;

        private Weather? _weather;
        private string? _tourImagePath;

        public ICommand CloseCommand { get; }
        public ICommand AddLogCommand { get; }
        public ICommand EditLogCommand { get; }
        public ICommand DeleteLogCommand { get; }

        private Tour? _selectedTour;
        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged();

                if (_selectedTour != null)
                {
                    // Bildpfad aktualisieren (mit Cache-Busting)
                    TourImagePath = $"{_selectedTour.ImagePath}?{DateTime.Now.Ticks}";
            }
        }
        }

        public string? TourImagePath
        {
            get => _tourImagePath;
            set
            {
                _tourImagePath = value;
                OnPropertyChanged();
            }
        }

        public Weather? Weather
        {
            get => _weather;
            set
            {
                _weather = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TourLog> TourLogs { get; } = new();


        private TourLog? _selectedTourLog;
        public TourLog? SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                _selectedTourLog = value;
                OnPropertyChanged();
            }
        }

        public void SetTourLogs()
        {
            try
            {
                if (SelectedTour == null)
                {
                    _logger.Error("SelectedTour is null");

                    NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                    NavigationService?.Close();
                    return;
                }
                var tourLogs = _tourLogManager.FindMatchingTourLogs(SelectedTour).ToList();

                var tourLogs = _tourLogManager.FindMatchingTourLogs(SelectedTour).ToList();
                TourLogs.Clear();
                tourLogs.ForEach(j => TourLogs.Add(j));

                _logger.Debug($"TourLogs set to {TourLogs.Count} items");
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error(e.Message);

                NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                NavigationService?.Close();
            }
        }

        private async Task EditTour()
        {
            try
            {
                if (SelectedTour == null)
                {
                    _logger.Error("SelectedTour is null");

                    NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                    NavigationService?.Close();
                    return;
                }

                var t = await _tourManager.Edit(SelectedTour, edit.NonGenerate);

                if (t == null)
                {
                    NavigationService?.ShowMessageBox("Tour not found", "Error");
                    NavigationService?.Close();
                    return;
                }

                SelectedTour = null;
                SelectedTour = t;
            }
            catch (PostgresDataBaseException e)
            {
                _logger.Error(e.Message);

                NavigationService?.ShowMessageBox("Error with Tour", "Error");
                NavigationService?.Close();
            }
        }

        public TourDetailsViewModel(
            ILoggerFactory loggerFactory,
            ITourPlannerLogManager tourLogManager,
            IWeatherGenerator weatherGenerator,
            Tour selectedTour,
            ITourPlannerManager tourManager)
        {
            _logger = loggerFactory.CreateLogger<TourDetailsViewModel>();

            _weatherGenerator = weatherGenerator;
            _tourLogManager = tourLogManager;
            _tourManager = tourManager;

            SelectedTour = selectedTour;

            LoadWeather();

            SetTourLogs();

            CloseCommand = new RelayCommand((_) => NavigationService?.Close());

            AddLogCommand = new RelayCommand((_) =>
            {
                var tourLogViewModel = new TourLogViewModel(loggerFactory)
                {
                    CorrespondingTour = SelectedTour
                };

                tourLogViewModel.TourLogAdded += async (_, tourLog) =>
                    {
                    try { await tourLogManager.AddAsync(tourLog); }
                    catch (PostgresDataBaseException e)
                    {
                        _logger.Error(e.Message);

                        NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                        NavigationService?.Close();
                    }

                    SetTourLogs();
                    await EditTour();

                    _logger.Debug($"TourLog with ID ({tourLog.Id}) added");
                };


                NavigationService?.NavigateTo(tourLogViewModel);
            });

            EditLogCommand = new RelayCommand((_) =>
            {
                var tourLogViewModel = new TourLogViewModel(loggerFactory)
                {
                    CorrespondingTour = SelectedTour,
                    TourLogToEdit = SelectedTourLog,
                    Usecase = "Edit Tour Log"
                };

                tourLogViewModel.TourLogEdited += async (_, tourLog) =>
                    {
                    try { tourLogManager.Edit(tourLog); }
                    catch (PostgresDataBaseException e)
                    {
                        _logger.Error(e.Message);

                        NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                        NavigationService?.Close();
                    }

                    SetTourLogs();
                    await EditTour();

                    _logger.Debug($"TourLog with ID ({tourLog.Id}) edited");
                };

                NavigationService?.NavigateTo(tourLogViewModel);

            }, (_) => SelectedTourLog != null);

            DeleteLogCommand = new RelayCommand(async (_) =>
            {
                var id = SelectedTourLog!.Id;
                try { tourLogManager.Delete(SelectedTourLog!); }
                catch (PostgresDataBaseException e)
                {
                    _logger.Error(e.Message);

                    NavigationService?.ShowMessageBox("Error with Tour logs", "Error");
                    NavigationService?.Close();
                }

                SetTourLogs();
                await EditTour();

                _logger.Debug($"TourLog with ID ({id}) deleted");

            }, (_) => SelectedTourLog != null);

            _logger.Debug($"TourDetailsViewModel created for Tour with ID ({SelectedTour.Id})");
        }

        private async void LoadWeather()
        {
            try
            {
                if (SelectedTour == null)
                {
                    _logger.Error("SelectedTour is null");
                    NavigationService?.ShowMessageBox("Tour not found", "Error");
                    NavigationService?.Close();
                    return;
                }

                Weather = await _weatherGenerator.GetWeather(SelectedTour);

                if (Weather == null)
                {
                    _logger.Error("Weather for SelectedTour is null");
                    NavigationService?.ShowMessageBox("Weather could not be retrieved", "Error");
                    return;
                }

                Weather.Location = SelectedTour.To;
                var temp = Weather;
                Weather = null;
                Weather = temp;
            }
            catch (WeatherApiReturnedNullException e)
            {
                _logger.Error("WeatherApiReturnedNullException");
                _logger.Error(e.Message);
                NavigationService?.ShowMessageBox("Weather could not be retrieved", "Error");
            }

        }
    }
}