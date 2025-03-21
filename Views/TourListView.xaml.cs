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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTourWindow addTourWindow = new AddTourWindow();
            addTourWindow.Owner = Window.GetWindow(this);
            addTourWindow.ShowDialog();
        }
    }
}
