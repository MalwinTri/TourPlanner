using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.Helper
{
    public class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrWhiteSpace(path))
            {
                return new Uri(path, UriKind.RelativeOrAbsolute);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
