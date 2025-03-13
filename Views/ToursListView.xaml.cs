using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace TourPlanner.ViewModels
{
    public class ToursListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<string> _tours;

        public ObservableCollection<string> Tours
        {
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }

        public ToursListViewModel()
        {
            // Beispiel-Daten, könnten aus einer Datenbank geladen werden
            Tours = new ObservableCollection<string>
            {
                "Wienerwald",
                "Dopperlhütte",
                "Figlwarte",
                "Dorfrunde"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
