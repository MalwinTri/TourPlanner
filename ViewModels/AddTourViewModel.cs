using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;
using TourPlanner.Services;
using TourPlanner.ViewModels;

public class AddTourViewModel : BaseViewModel
{
    private readonly IFileDialogService _fileDialogService;

    public ICommand UploadImageCommand { get; }

    private string _imagePath;
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

    public AddTourViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
        UploadImageCommand = new RelayCommand(_ => UploadImage());
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
}
