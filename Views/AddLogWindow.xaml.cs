using System.Windows;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class AddLogWindow : Window
    {
        public AddLogWindow(Action<TourLog> addLogAction)
        {
            InitializeComponent();

            var viewModel = new AddLogViewModel(addLogAction);
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }
    }
}
