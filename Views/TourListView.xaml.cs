using System.Windows.Controls;
using TourPlanner.ViewModels;
using TourPlanner.Models;

namespace TourPlanner.Views
{
    public partial class TourListView : UserControl
    {
        public TourListView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is TourListViewModel vm && tourlist.SelectedItem is Tour selectedTour)
            {
                vm.SelectedTour = selectedTour;
            }
        }
    }
}
