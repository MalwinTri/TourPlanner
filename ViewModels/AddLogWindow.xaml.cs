using System;
using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.Views
{
    public partial class AddLogWindow : Window
    {
        public AddLogWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DateTime.TryParse(DateTimeTextBox.Text, out var dateResult))
            {
                MessageBox.Show("Invalid Date/Time format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var difficultyItem = DifficultyComboBox.SelectedItem as ComboBoxItem;
            string difficulty = difficultyItem?.Content.ToString();

            if (!double.TryParse(DistanceTextBox.Text, out var distance))
            {
                MessageBox.Show("Invalid distance value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string totalTime = TotalTimeTextBox.Text;

            if (!int.TryParse(RatingTextBox.Text, out var rating))
            {
                MessageBox.Show("Invalid rating value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string comment = CommentTextBox.Text;

            this.DialogResult = true;
        }

  

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
