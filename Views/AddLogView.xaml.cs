using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddLogView.xaml
    /// </summary>
    public partial class AddLogView : UserControl
    {
        public AddLogView()
        {
            InitializeComponent();
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            // Erlaubt nur Ziffern 0-9
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]*$");
        }
    }
}
