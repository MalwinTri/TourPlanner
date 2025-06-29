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
using TourPlanner.Models;

namespace TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for AddLogWindow.xaml
    /// </summary>
    public partial class AddLogWindow : Window
    {
        private readonly Action<TourLog> _onLogAdded;

        public AddLogWindow(Action<TourLog> onLogAdded)
        {
            InitializeComponent();
            _onLogAdded = onLogAdded;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Beispiel-Erstellung eines neuen Logs
            var newLog = new TourLog
            {
                Date = DateTime.Now,
                Comment = "New Log",
                Difficulty = 1,
                TotalDistance = 1.0,
                TotalTime = TimeSpan.FromMinutes(30),
                Rating = 3
            };

            _onLogAdded?.Invoke(newLog);
            DialogResult = true;
            Close();
        }
    }

}
