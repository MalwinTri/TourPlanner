using System.Windows;
using TourPlanner.Models;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    public partial class EditLogWindow : Window
    {
        public EditLogWindow(TourLog log)
        {
            InitializeComponent();
            DataContext = new EditLogViewModel(log)
            {
                CloseAction = () => this.DialogResult = true
            };
        }
    }
}
