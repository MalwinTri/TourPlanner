using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.Logging;
using TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class TourPreviewViewModel : BaseViewModel
    {
        private Tour? _selectedTour;

        public EventHandler<Tour>? TourDetailsOpened;


        public ICommand DetailsCommand { get; }

        public Tour? SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged();
            }
        }


        public TourPreviewViewModel(ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<TourPreviewViewModel>();

            DetailsCommand = new RelayCommand((_) =>
            {
                if (SelectedTour == null)
                {
                    logger.Error("Selected tour is null");
                    NavigationService?.ShowMessageBox("Something went wrong.", "Error");
                    return;
                }
                OnTourDetailsOpened(SelectedTour);
                logger.Debug("Tour details opened");
            }, (_) => SelectedTour != null);
        }


        private void OnTourDetailsOpened(Tour tour)
        {
            TourDetailsOpened?.Invoke(this, tour);
        }

    }
}
