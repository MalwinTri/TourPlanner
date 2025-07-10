using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner.BL;
using TourPlanner.BL.Export;
using TourPlanner.BL.Import;
using TourPlanner.BL.iText;
using TourPlanner.BL.Mapquest;
using TourPlanner.BL.OpenRouteService;
using TourPlanner.BL.WeatherAPI;
using TourPlanner.DAL;
using TourPlanner.DAL.Postgres;
using TourPlanner.Logging.Log4Net;
using TourPlanner.Navigation;
using TourPlanner.ViewModels;
using TourPlanner.Views;
using ILoggerFactory = TourPlanner.Logging.ILoggerFactory;

namespace TourPlanner.Configuration
{
    internal class IoCContainerConfiguration
    {
        private readonly ServiceProvider _serviceProvider;

        public INavigationService NavigationService => _serviceProvider.GetRequiredService<INavigationService>();
        public ILoggerFactory LoggerFactory => _serviceProvider.GetRequiredService<ILoggerFactory>();

        public IoCContainerConfiguration()
        {
            var services = new ServiceCollection();


            services.AddSingleton<IConfiguration>((_) => new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("apikeys.json", optional: false, reloadOnChange: true)
                .Build());
            services.AddSingleton<AppConfiguration>();


            services.AddSingleton<ITourPlannerPostgresRepositoryConfiguration>(s => s.GetRequiredService<AppConfiguration>());
            services.AddSingleton<IMapquestConfiguration>(s => s.GetRequiredService<AppConfiguration>());
            services.AddSingleton<IOpenRouteServiceConfiguration>(s => s.GetRequiredService<AppConfiguration>());
            services.AddSingleton<IItextConfiguration>(s => s.GetRequiredService<AppConfiguration>());
            services.AddSingleton<IWeatherApiConfiguration>(s => s.GetRequiredService<AppConfiguration>());
            services.AddSingleton<IExportConfiguration>(s => s.GetRequiredService<AppConfiguration>());

            services.AddSingleton<ILoggerFactory>(new Log4NetFactory("log4net.conf"));

            services.AddSingleton<ITourPlannerRepository, TourPlannerPostgresRepository>();

            services.AddSingleton<ITourPlannerGenerator, OpenRouteServiceTourGenerator>();
            services.AddSingleton<IMapImageService, MapquestImageService>();
            services.AddSingleton<ITourPlannerManager, TourPlannerManager>();
            services.AddSingleton<ITourPlannerLogManager, TourPlannerLogManager>();
            services.AddSingleton<IReportGenerator, iTextReportGenerator>();
            services.AddSingleton<IExportManager, ExportGenerator>();
            services.AddSingleton<IImportManager, ImportGenerator>();
            services.AddSingleton<IWeatherGenerator, WeatherApiGenerator>();

            services.AddSingleton<INavigationService, NavigationService>(s =>
            {
                var navigationService = new NavigationService(s);
                //register viewmodel with window
                navigationService.RegisterNavigation<TourDetailsViewModel, TourDetailsDialog>();
                navigationService.RegisterNavigation<TourLogViewModel, TourLogDialog>();
                navigationService.RegisterNavigation<AddTourViewModel, AddTourDialog>();
                navigationService.RegisterNavigation<EditTourViewModel, EditTourDialog>();

                navigationService.RegisterNavigation<MainViewModel, MainWindow>((viewModel, window) =>
                {
                    window.TourList.DataContext = viewModel.TourListViewModel;
                    window.TourPreview.DataContext = viewModel.TourPreviewViewModel;
                    window.SearchView.DataContext = viewModel.SearchViewModel;
                });
                return navigationService;
            });


            services.AddSingleton<SearchViewModel>();
            services.AddTransient<AddTourViewModel>();
            services.AddTransient<TourLogViewModel>();
            services.AddTransient<EditTourViewModel>();
            services.AddTransient<TourDetailsViewModel>();
            services.AddSingleton<TourListViewModel>();
            services.AddTransient<TourPreviewViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<IMapImageService, MapquestImageService>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
