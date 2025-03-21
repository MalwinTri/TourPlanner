using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace TourPlanner.Views
{
    public partial class AddTourWindow : Window
    {
        public AddTourWindow()
        {
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Dateiauswahl-Dialog öffnen
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bilddateien (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                // Dateiname anzeigen
                ImageFileNameTextBlock.Text = Path.GetFileName(openFileDialog.FileName);

                // Bild laden und anzeigen
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                TourImage.Source = bitmap;

                // Optional: Pfad zur Bilddatei speichern oder weitere Verarbeitung durchführen
            }
        }
    }
}
