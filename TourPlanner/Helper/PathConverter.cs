using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TourPlanner.Helper
{
    public class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                return new Uri("file:///" + path.Replace("\\", "/"));
            }

            // Fallback-Bild anzeigen, falls kein gültiger Pfad
            return new Uri("https://via.placeholder.com/300x200.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}