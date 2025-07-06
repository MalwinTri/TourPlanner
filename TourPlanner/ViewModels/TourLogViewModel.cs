using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.Logging;
using TourPlanner.Models;
using TourPlanner.ViewModels.Commands;

namespace TourPlanner.ViewModels
{
    public class TourLogViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private DateTime _date;
        private string? _comment;
        private string? _totalTime;
        public Tour? CorrespondingTour { get; set; }
        public TourLog? TourLogToEdit { get; set; } = null;

        public string Usecase { get; set; } = "Add Tour Log";

        public event EventHandler<TourLog>? TourLogAdded = null;
        public event EventHandler<TourLog>? TourLogEdited = null;
        public event EventHandler? ValidationsFailed = null;

        public ICommand TourLogCommand { get; }

        public TourLogViewModel(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<TourLogViewModel>();

            Date = DateTime.Now;

            TourLogCommand = new RelayCommand((_) =>
            {

                if (!InputLogValidation(Comment, TotalTime, Rating, Difficulty))
                {
                    ValidationsFailed?.Invoke(this, EventArgs.Empty);
                    return;
                }
                switch (Usecase)
                {
                    case "Add Tour Log":
                        {
                            var realtime = TimeSpan.Parse(TotalTime!).TotalSeconds;

                            {

                                var dif = Difficulty!.Content.ToString() switch
                                {
                                    "Easy" => 1,
                                    "Medium" => 2,
                                    "Hard" => 3,
                                    _ => 0.0
                                };
                                var rat = Rating!.Content.ToString() switch
                                {
                                    "Poor" => 1,
                                    "Fair" => 2,
                                    "Good" => 3,
                                    "VeryGood" => 4,
                                    "Excellent" => 5,
                                    _ => 0.0
                                };

                                var tourLog = new TourLog(Guid.NewGuid(), CorrespondingTour!.Id, Date.ToUniversalTime(),
                                    Comment, dif, realtime, rat);
                                OnTourLogAdded(tourLog);
                                NavigationService?.Close();
                                break;
                            }
                        }
                    case "Edit Tour Log":
                        {
                            var realtime = TimeSpan.Parse(TotalTime!).TotalSeconds;

                            var dif = Difficulty!.Content.ToString() switch
                            {
                                "Easy" => 1,
                                "Medium" => 2,
                                "Hard" => 3,
                                _ => 0.0
                            };
                            var rat = Rating!.Content.ToString() switch
                            {
                                "Poor" => 1,
                                "Fair" => 2,
                                "Good" => 3,
                                "VeryGood" => 4,
                                "Excellent" => 5,
                                _ => 0.0
                            };

                            if (TourLogToEdit == null)
                            {
                                _logger.Error("TourLogToEdit is null");
                                NavigationService?.ShowMessageBox("TourLogToEdit is null", "Error");
                                return;
                            }

                            TourLogToEdit.Date = Date.ToUniversalTime();
                            TourLogToEdit.Comment = Comment;
                            TourLogToEdit.Difficulty = dif;
                            TourLogToEdit.TotalTime = realtime;
                            TourLogToEdit.Rating = rat;

                            var tourLog = TourLogToEdit;
                            OnTourLogEdited(tourLog);
                            NavigationService?.Close();
                            break;
                        }
                }
            }, (_) => true);
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string? Comment
        {
            get => _comment;
            set
            {
                _comment = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxItem? Difficulty { get; set; }

        public string? TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
                OnPropertyChanged();
            }
        }
        public ComboBoxItem? Rating { get; set; }

        private void OnTourLogAdded(TourLog tourLog)
        {
            TourLogAdded?.Invoke(this, tourLog);
        }
        private void OnTourLogEdited(TourLog tourLog)
        {
            TourLogEdited?.Invoke(this, tourLog);
        }

        private bool InputLogValidation(string? comment, string? totalTime, ComboBoxItem? rating, ComboBoxItem? difficulty)
        {
            if (rating == null || difficulty == null)
            {
                _logger.Warning("Rating or Difficulty is null");
                return false;
            }
            if (string.IsNullOrWhiteSpace(comment) || string.IsNullOrWhiteSpace(totalTime) || string.IsNullOrWhiteSpace(rating.Content.ToString()) || string.IsNullOrWhiteSpace(difficulty.Content.ToString()))
            {
                _logger.Warning("Comment, TotalTime, Rating or Difficulty is null or empty");
                return false;
            }
            var pattern = @"^([0-1][0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$";
            return Regex.IsMatch(totalTime, pattern);
        }
    }
}
