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

namespace TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for TourLogsView.xaml
    /// </summary>
    public partial class TourLogsView : UserControl
    {
        public TourLogsView()
        {
            InitializeComponent();
        }

        private void AddLog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button clicked!");

            AddLogWindow addLogWindow = new AddLogWindow();
            addLogWindow.Owner = Window.GetWindow(this); // Setzt das Hauptfenster als Owner
            addLogWindow.ShowDialog(); // Modales Fenster öffnen
        }
    }
}
