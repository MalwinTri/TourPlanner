using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TourPlanner.Models;
using TourPlanner.Services;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class AddTourWindow : Window
    {
        public AddTourWindow(Action<Tour> addTourAction)
        {
            InitializeComponent();
            var fileDialogService = new FileDialogService();
            var viewModel = new AddTourViewModel(fileDialogService, addTourAction);
            viewModel.CloseAction = Close;
            DataContext = viewModel;
        }
    }
}
