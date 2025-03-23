using System.Windows;
using System.ComponentModel;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class DeleteTourLogWindow : Window
    {
        public DeleteTourLogWindow()
        {
            InitializeComponent();

            var viewModel = new DeleteTourViewModel();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                viewModel.CloseWindow = result =>
                {
                    DialogResult = result;
                    Close();
                };
            }

            DataContext = viewModel;
        }

    }
}
