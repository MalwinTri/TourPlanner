using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

namespace TourPlanner.ViewModels
{
    public class AddTourViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private string? _name;
        private string? _description;
        private string? _from;
        private string? _to;

        public event EventHandler<Tour>? TourAdded = null;
        public event EventHandler? ValidationsFailed = null;
        public ICommand AddTourCommand { get; }

        public AddTourViewModel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AddTourViewModel>();

            AddTourCommand = new RelayCommand((_) =>
            {
                MessageBox.Show("Button funktioniert!");
                if (Transport == null)
                {
                    MessageBox.Show("Button funktioniert!");
                    _logger.Warning("Transport combobox item not selected");
                    NavigationService!.ShowMessageBox("Transport method not selected", "Add dialog error");
                    return;
                }

                if (!InputAddValidation(Name, Description, From, To, Transport!.Content.ToString()))
                {
                    MessageBox.Show("Button funktioniert!");
                    ValidationsFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var tour = new Tour(Guid.NewGuid(), Name, Description, From, To, Transport.Content.ToString()!);
                OnTourAdded(tour);

                MessageBox.Show("Button funktioniert!");
                _logger.Debug($"Tour added: {tour.Name}");
                NavigationService?.Close();
                ClearInput();
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

        private void OnTourAdded(Tour tour)
        {
            TourAdded?.Invoke(this, tour);
        }

        private bool InputAddValidation(string? name, string? description, string? from, string? to, string? transport)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(transport))
            {
                _logger.Warning("Input validation failed");
                return false;
            }

            _logger.Debug("Input validation passed");
            return true;
        }

        private void ClearInput()
        {
            Name = null;
            Description = null;
            From = null;
            To = null;
        }
    }
}
