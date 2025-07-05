using System.Windows;
using TourPlanner.Configuration;
using TourPlanner.Navigation;
using TourPlanner.ViewModels;

namespace TourPlanner
{
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {

            var ioCConfig = new IoCContainerConfiguration();

            ioCConfig.NavigationService.NavigateTo<MainViewModel>(NavigationMode.Modeless);

            var logger = ioCConfig.LoggerFactory.CreateLogger<App>();
            logger.Debug("App started");
        }
    }
}