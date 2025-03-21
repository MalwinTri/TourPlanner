using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public class AddLogViewModel : INotifyPropertyChanged
    {
        private string _dateTime;
        private double _distance;
        private int _rating;
        private string _comment;

        public string DateTime
        {
            get => _dateTime;
            set { _dateTime = value; OnPropertyChanged(nameof(DateTime)); }
        }

        public double Distance
        {
            get => _distance;
            set { _distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        public int Rating
        {
            get => _rating;
            set { _rating = value; OnPropertyChanged(nameof(Rating)); }
        }

        public string Comment
        {
            get => _comment;
            set { _comment = value; OnPropertyChanged(nameof(Comment)); }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddLogViewModel()
        {
            SaveCommand = new RelayCommand(SaveLog);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveLog(object obj)
        {

        }

        private void Cancel(object obj)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
