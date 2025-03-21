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
        }

        private void AddLog_Click(object sender, RoutedEventArgs e)
        {
            AddLogWindow addLogWindow = new AddLogWindow();
            addLogWindow.Owner = Window.GetWindow(this);
            addLogWindow.ShowDialog();
        }

        private void DelLog_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDeleteWindow confirmWin = new ConfirmDeleteWindow();
            confirmWin.Owner = Window.GetWindow(this);

            bool? result = confirmWin.ShowDialog();
            if (result == true)
            {
                MessageBox.Show("Log deleted successfully!");
            }
        }
        private void ModLog_Click(object sender, RoutedEventArgs e)
        {
            if (LogsDataGrid.SelectedItem is TourLog selectedLog)
            {
                EditLogWindow editWindow = new EditLogWindow(selectedLog);
                editWindow.Owner = Window.GetWindow(this);

                bool? result = editWindow.ShowDialog();
                if (result == true)
                {
                    TourLog updatedLog = editWindow.GetUpdatedLog(); 

                    selectedLog.DateTime = updatedLog.DateTime;
                    selectedLog.Difficulty = updatedLog.Difficulty;
                    selectedLog.TotalDistance = updatedLog.TotalDistance;
                    selectedLog.TotalTime = updatedLog.TotalTime;
                    selectedLog.Rating = updatedLog.Rating;
                    selectedLog.Comment = updatedLog.Comment;

                    MessageBox.Show("Log successfully updated!");
                }
            }
            else
            {
                MessageBox.Show("No log selected!");
            }
        }
    }
}
