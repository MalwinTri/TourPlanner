using System.Windows;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class AddLogWindow : Window
    {
        public AddLogWindow()
        {
            InitializeComponent();
            DataContext = new AddLogViewModel(); 
        }
    }
}
