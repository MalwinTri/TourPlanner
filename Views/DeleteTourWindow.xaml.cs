using System.Windows;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class DeleteTourWindow : Window
    {
        public DeleteTourViewModel ViewModel { get; }

        public bool Confirmed => ViewModel.Confirmed;

        public DeleteTourWindow()
        {
            InitializeComponent();
            ViewModel = new DeleteTourViewModel();
            DataContext = ViewModel;
        }
    }
}
