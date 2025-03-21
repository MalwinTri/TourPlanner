using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for TourListView.xaml
    /// </summary>
    public partial class TourListView : UserControl
    {
        public TourListView()
        {
            InitializeComponent();
            DataContext = new TourListViewModel();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddTourWindow addTourWindow = new AddTourWindow();
            addTourWindow.Owner = Window.GetWindow(this);
            addTourWindow.ShowDialog();
        }
    }
}
