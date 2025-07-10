using System;
using System.Globalization;
using System.Windows.Data;

namespace TourPlanner.Helper
{
    public class SecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double seconds)
            {
                var time = TimeSpan.FromSeconds(seconds);
                if (time.TotalHours >= 1)
                    return $"{(int)time.TotalHours} h {time.Minutes} min";
                if (time.TotalMinutes >= 1)
                    return $"{(int)time.TotalMinutes} min";
                return $"{time.Seconds} sec";
            }

            return "0 min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !double.TryParse(value.ToString(), out double seconds) || double.IsInfinity(seconds) || seconds <= 0)
                return "Dauer unbekannt";

            var roundedSeconds = Math.Floor(seconds); 
            var time = TimeSpan.FromSeconds(roundedSeconds);
            return time.ToString(@"hh\:mm\:ss"); 
        }
    }
}
