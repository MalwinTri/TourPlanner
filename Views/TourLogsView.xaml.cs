using System.Windows;
using System.Windows.Controls;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{

    public partial class TourLogsView : UserControl
    {
        public TourLogsView()
        {
            InitializeComponent();
            DataContext = new TourLogsViewModel();
        }
    }
}