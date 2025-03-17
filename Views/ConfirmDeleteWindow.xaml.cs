using System.Windows;
using TourPlanner.ViewModels; // ✅ Sicherstellen, dass das richtige ViewModel genutzt wird

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
