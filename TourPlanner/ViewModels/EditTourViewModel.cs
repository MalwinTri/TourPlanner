using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TourPlanner.Models;
using TourPlanner.Models.TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class EditTourViewModel : INotifyPropertyChanged
    {
        private Tour _tour;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand UploadImageCommand { get; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private BitmapImage _imagePreview;
        public BitmapImage ImagePreview
        {
            get => _imagePreview;
            set
            {
                _imagePreview = value;
                OnPropertyChanged(nameof(ImagePreview));
            }
        }

        public string Name
        {
            get => _tour.Name;
            set { _tour.Name = value; OnPropertyChanged(nameof(Name)); }
        }

        public string Description
        {
            get => _tour.Description;
            set { _tour.Description = value; OnPropertyChanged(nameof(Description)); }
        }

        public string From
        {
            get => _tour.From;
            set { _tour.From = value; OnPropertyChanged(nameof(From)); }
        }

        public string To
        {
            get => _tour.To;
            set { _tour.To = value; OnPropertyChanged(nameof(To)); }
        }

        public double Distance
        {
            get => _tour.Distance;
            set { _tour.Distance = value; OnPropertyChanged(nameof(Distance)); }
        }

        public TimeSpan EstimatedTime
        {
            get => _tour.EstimatedTime;
            set { _tour.EstimatedTime = value; OnPropertyChanged(nameof(EstimatedTime)); }
        }

        public string RouteInformation
        {
            get => _tour.RouteInformation;
            set { _tour.RouteInformation = value; OnPropertyChanged(nameof(RouteInformation)); }
        }

        public string TransportType
        {
            get => _tour.TransportType;
            set { _tour.TransportType = value; OnPropertyChanged(nameof(TransportType)); }
        }

        public Tour EditedTour => _tour;

        public EditTourViewModel(Tour tourToEdit)
        {
            _tour = new Tour
            {
                Name = tourToEdit.Name,
                Description = tourToEdit.Description,
                From = tourToEdit.From,
                To = tourToEdit.To,
                Distance = tourToEdit.Distance,
                EstimatedTime = tourToEdit.EstimatedTime,
                RouteInformation = tourToEdit.RouteInformation,
                TransportType = tourToEdit.TransportType,
                ImagePath = tourToEdit.ImagePath
            };

            if (!string.IsNullOrEmpty(_tour.ImagePath))
            {
                ImagePath = _tour.ImagePath;
                ImagePreview = new BitmapImage(new Uri(ImagePath));
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            UploadImageCommand = new RelayCommand(UploadImage);
        }

        private void UploadImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                ImagePreview = new BitmapImage(new Uri(ImagePath));
                _tour.ImagePath = ImagePath;
            }
        }


        private void Save(object window)
        {
            if (window is Window w)
            {
                w.DialogResult = true;
                w.Close();
            }
        }

        private void Cancel(object window)
        {
            if (window is Window w)
            {
                w.DialogResult = false;
                w.Close();
            }
        }

        public Tour UpdatedTour => EditedTour;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
