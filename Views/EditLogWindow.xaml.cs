using System.Windows;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class EditLogWindow : Window
    {
        private readonly EditLogViewModel _viewModel;

        public EditLogWindow(TourLog log)
        {
            InitializeComponent();
            _viewModel = new EditLogViewModel(log);
            _viewModel.CloseAction = () => this.Close();
            DataContext = _viewModel;
        }

        public TourLog GetUpdatedLog() => _viewModel.EditedLog; 
    }
}
