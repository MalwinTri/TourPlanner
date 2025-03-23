using TourPlanner.ViewModels;

public class Tour : BaseViewModel
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private string _from;
    public string From
    {
        get => _from;
        set => SetProperty(ref _from, value);
    }

    private string _to;
    public string To
    {
        get => _to;
        set => SetProperty(ref _to, value);
    }

    private string _transportType;
    public string TransportType
    {
        get => _transportType;
        set => SetProperty(ref _transportType, value);
    }

    private double _distance;
    public double Distance
    {
        get => _distance;
        set => SetProperty(ref _distance, value);
    }

    private string _estimatedTime;
    public string EstimatedTime
    {
        get => _estimatedTime;
        set => SetProperty(ref _estimatedTime, value);
    }

    private string _routeInformation;
    public string RouteInformation
    {
        get => _routeInformation;
        set => SetProperty(ref _routeInformation, value);
    }

    private string _imagePath;
    public string ImagePath
    {
        get => _imagePath;
        set => SetProperty(ref _imagePath, value);
    }
}
