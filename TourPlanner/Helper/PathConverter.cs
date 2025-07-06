using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TourPlanner.Helper
{
    public class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path)
            {
                Debug.WriteLine($"[PathConverter] Received path: {path}");

                // Wenn es eine URL ist (z. B. fürs Wetter)
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    var uri = new Uri(path, UriKind.Absolute);

                    if (uri.Scheme == Uri.UriSchemeFile)
                    {
                        // Für file://-URIs → lokalen Pfad extrahieren
                        var localPath = uri.LocalPath;
                        Debug.WriteLine($"[PathConverter] Local file path: {localPath}");

                        if (File.Exists(localPath))
                        {
                            Debug.WriteLine("[PathConverter] File exists.");
                            return uri;
                        }
                        else
                        {
                            Debug.WriteLine("[PathConverter] File does NOT exist.");
                        }
                    }
                    else
                    {
                        // Online-Bild wie Wetter-Icon
                        return uri;
                    }
                }
            }

            Debug.WriteLine("[PathConverter] Returning placeholder.");
            return new Uri("https://via.placeholder.com/300x200.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}