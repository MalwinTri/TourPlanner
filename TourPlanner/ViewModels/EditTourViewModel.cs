using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BL;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

namespace TourPlanner.ViewModels
{
    public class EditTourViewModel : BaseViewModel
    {
        private readonly ILogger _logger;
        private readonly ITourPlannerManager _tourManager;

        private string? _name;
        private string? _description;
        private string? _from;
        private string? _to;
        private Tour? _tourToEdit;

        public event EventHandler<Tour>? TourEdited = null;
        public event EventHandler? ValidationsFailed = null;

        public ICommand EditTourCommand { get; }

        public Tour? TourToEdit
        {
            get => _tourToEdit;
            set
            {
                _tourToEdit = value;
                if (_tourToEdit == null) return;
                Name = _tourToEdit.Name;
                Description = _tourToEdit.Description;
                From = _tourToEdit.From;
                To = _tourToEdit.To;
                SelectedTransport = _tourToEdit.Transport;
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        public EditTourViewModel(ITourPlannerManager tourManager, ILoggerFactory loggerFactory)
        {
            _tourManager = tourManager;
            _logger = loggerFactory.CreateLogger<EditTourViewModel>();

            EditTourCommand = new RelayCommand(async (_) =>
            {
                _logger.Debug("EditTourCommand triggered");

                if (TourToEdit == null)
                {
                    _logger.Error("Tour to edit is null");
                    NavigationService!.ShowMessageBox("Tour to edit is null", "Edit dialog error");
                    return;
                }

                if (!InputEditValidation(Name, Description, From, To, SelectedTransport))
                {
                    _logger.Warning("Validation failed during tour edit");
                    ValidationsFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                try
                {
                    IsEditing = true;
                    _logger.Debug("Starting tour edit...");

                    TourToEdit.Name = Name;
                    TourToEdit.Description = Description;
                    TourToEdit.From = From;
                    TourToEdit.To = To;
                    TourToEdit.Transport = SelectedTransport;

                    _logger.Debug($"Sending tour to manager: {TourToEdit.Name}, From={TourToEdit.From}, To={TourToEdit.To}");

                    var updatedTour = await _tourManager.Edit(TourToEdit, BL.Enum.edit.Generate);

                    if (updatedTour != null)
                    {
                        _logger.Debug($"Tour edited successfully. New image path: {updatedTour.ImagePath}");
                        OnTourEdited(updatedTour);
                    }

                    NavigationService!.Close();
                }
                catch (Exception ex)
                {
                    _logger.Error("Fehler beim Bearbeiten: " + ex.Message);
                    NavigationService!.ShowMessageBox("Fehler beim Bearbeiten", ex.Message);
                }
                finally
                {
                    IsEditing = false;
                    _logger.Debug("Edit command finished");
                }
            }, (_) => true);
        }

        public string? Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string? Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public string? From
        {
            get => _from;
            set { _from = value; OnPropertyChanged(); }
        }

        public string? To
        {
            get => _to;
            set { _to = value; OnPropertyChanged(); }
        }

        public string? SelectedTransport { get; set; }

        private void OnTourEdited(Tour tour)
        {
            TourEdited?.Invoke(this, tour);
        }

        private static bool InputEditValidation(string? name, string? description, string? from, string? to, string? transport)
        {
            return !(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) ||
                     string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) ||
                     string.IsNullOrWhiteSpace(transport));
        }
    }
}