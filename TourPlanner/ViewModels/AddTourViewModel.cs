using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TourPlanner.Models;
using TourPlanner.Models.TourPlanner.Models;
using TourPlanner.Services;
using TourPlanner.ViewModels;

public class AddTourViewModel : INotifyPropertyChanged
{
    private readonly IFileDialogService _fileDialogService;

    public ICommand UploadImageCommand { get; }

    private string _name;
    private string _description;
    private string _from;
    private string _to;
    private string _routeInformation;
    private string _transportType;
    private double _distance;
    private string _estimatedTime;
    private string _imagePath;

    private readonly Action<Tour> _addTourAction;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(nameof(Name)); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(nameof(Description)); }
    }

    public string From
    {
        get => _from;
        set { _from = value; OnPropertyChanged(nameof(From)); }
    }

    public string To
    {
        get => _to;
        set { _to = value; OnPropertyChanged(nameof(To)); }
    }

    public string RouteInformation
    {
        get => _routeInformation;
        set { _routeInformation = value; OnPropertyChanged(nameof(RouteInformation)); }
    }

    public string TransportType
    {
        get => _transportType;
        set { _transportType = value; OnPropertyChanged(nameof(TransportType)); }
    }

    public double Distance
    {
        get => _distance;
        set { _distance = value; OnPropertyChanged(nameof(Distance)); }
    }

    public string EstimatedTime
    {
        get => _estimatedTime;
        set { _estimatedTime = value; OnPropertyChanged(nameof(EstimatedTime)); }
    }

    public string ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); }
    }

    private BitmapImage _imagePreview;
    public BitmapImage ImagePreview
    {
        get => _imagePreview;
        set { _imagePreview = value; OnPropertyChanged(nameof(ImagePreview)); }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public Action CloseAction { get; set; }

    public AddTourViewModel(IFileDialogService fileDialogService, Action<Tour> addTourAction)
    {
        _fileDialogService = fileDialogService;
        UploadImageCommand = new RelayCommand(_ => UploadImage());
        _addTourAction = addTourAction;
        SaveCommand = new RelayCommand(SaveTour);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void UploadImage()
    {
        string path = _fileDialogService.OpenImageFileDialog();
        if (!string.IsNullOrEmpty(path))
        {
            ImagePath = path;

            var bitmap = new BitmapImage(new Uri(path));
            ImagePreview = bitmap;
        }
    }

    private void SaveTour(object obj)
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(From) || string.IsNullOrEmpty(To) || string.IsNullOrEmpty(RouteInformation) || string.IsNullOrEmpty(TransportType) || string.IsNullOrEmpty(EstimatedTime) || string.IsNullOrEmpty(ImagePath))
        {
            MessageBox.Show("Please fill out all fields.");
            return;
        }

        if (Distance <= 0)
        {
            MessageBox.Show("Distance must be greater than 0.");
            return;
        }


        if (!double.TryParse(Distance.ToString(), out var parsedDistance))
        {
            MessageBox.Show("Distance must be a number.");
            return;
        }

        if (!TimeSpan.TryParse(EstimatedTime, out var parsedTime))
        {
            MessageBox.Show("Estimated time must be a valid timespan (e.g. 01:30:00).");
            return;
        }

        var newTour = new Tour
        {
            Name = Name,
            Description = Description,
            From = From,
            To = To,
            RouteInformation = RouteInformation,
            TransportType = TransportType,
            Distance = parsedDistance,
            EstimatedTime = parsedTime,
            ImagePath = ImagePath
        };
        _addTourAction(newTour);
        CloseAction?.Invoke();
    }

    private void Cancel(object obj)
    {
        CloseAction?.Invoke();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
