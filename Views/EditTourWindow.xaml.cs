using System.Windows;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class EditTourWindow : Window
    {
        private readonly EditTourViewModel _viewModel;

        public EditTourWindow(Tour tourToEdit)
        {
            InitializeComponent();

            _viewModel = new EditTourViewModel(tourToEdit);
            DataContext = _viewModel;
        }

        public Tour UpdatedTour => _viewModel.EditedTour;
    }
}
