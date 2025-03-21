using System.Windows;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow()
        {
            InitializeComponent();
            var viewModel = new ConfirmDeleteViewModel();
            viewModel.CloseAction = () => this.Close(); 
            DataContext = viewModel;
        }
    }
}
