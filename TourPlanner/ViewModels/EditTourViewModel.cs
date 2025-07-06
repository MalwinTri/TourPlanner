using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

namespace TourPlanner.ViewModels
{
    public class EditTourViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

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
            }
        }

        public EditTourViewModel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EditTourViewModel>();

            EditTourCommand = new RelayCommand((_) =>
            {
                if (TourToEdit == null)
                {
                    _logger.Error("Tour to edit is null");
                    NavigationService!.ShowMessageBox("Tour to edit is null", "Edit dialog error");
                    return;
                }
                if (Transport == null)
                {
                    _logger.Error("Transport combobox item is null");
                    NavigationService!.ShowMessageBox("Transport method not selected", "Edit dialog error");
                    return;
                }

                if (!InputEditValidation(Name, Description, From, To, Transport!.Content.ToString()))
                {
                    ValidationsFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                TourToEdit.Name = Name;
                TourToEdit.Description = Description;
                TourToEdit.From = From;
                TourToEdit.To = To;
                TourToEdit.Transport = Transport.Content.ToString()!;
                OnTourEdited(TourToEdit);

                NavigationService!.Close();
            }, (_) => true);
        }

        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string? From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged();
            }
        }

        public string? To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxItem? Transport { get; set; }


        private void OnTourEdited(Tour tour)
        {
            TourEdited?.Invoke(this, tour);
        }

        private static bool InputEditValidation(string? name, string? description, string? from, string? to, string? transport)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(transport))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
