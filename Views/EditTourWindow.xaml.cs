using System.Windows;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class EditTourWindow : Window
    {
        public EditTourViewModel ViewModel { get; }

        public Tour UpdatedTour => ViewModel.EditedTour;

        public EditTourWindow(Tour tourToEdit)
        {
            InitializeComponent();
            ViewModel = new EditTourViewModel(tourToEdit);
            DataContext = ViewModel;
        }
    }
}