using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using TourPlanner.Services;

namespace TourPlanner.Views
{
    public partial class AddTourWindow : Window
    {
        public AddTourWindow()
        {
            InitializeComponent();
            var fileDialogService = new FileDialogService();
            DataContext = new AddTourViewModel(fileDialogService);
        }
    }
}
