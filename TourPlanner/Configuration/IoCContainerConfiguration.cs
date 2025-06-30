using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner;
using TourPlanner.Configuration;
using TourPlanner.DAL;
using TourPlanner.DAL.Postgres;
using TourPlanner.Logging.Log4Net;
using TourPlanner.Navigation;
using TourPlanner.ViewModels;
using TourPlanner.Views;
using ILoggerFactory = TourPlanner.Logging.ILoggerFactory;

namespace SWEN2_TourPlanner.Configuration
{
    internal class IoCContainerConfiguration
    {
        private readonly ServiceProvider _serviceProvider;

        public INavigationService NavigationService => _serviceProvider.GetRequiredService<INavigationService>();
        public ILoggerFactory LoggerFactory => _serviceProvider.GetRequiredService<ILoggerFactory>();

        public IoCContainerConfiguration()
        {
            var services = new ServiceCollection();

            // Load appsettings.json
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<AppConfiguration>();

            // Get connection string parts
            var postgresConfig = configuration.GetSection("postgres");
            var connectionString = postgresConfig["connectionstring"];
            var username = postgresConfig["username"];
            var password = postgresConfig["password"];
            var fullConnectionString = $"{connectionString};Username={username};Password={password}";

            // Register custom DbContext
            services.AddSingleton<TourPlannerDbContext>(sp =>
                new TourPlannerDbContext(fullConnectionString));

            services.AddSingleton<ITourPlannerPostgresRepositoryConfiguration>(s => s.GetRequiredService<AppConfiguration>());

            // Logging
            services.AddSingleton<ILoggerFactory>(new Log4NetFactory("log4net.conf"));

            // DAL
            services.AddSingleton<ITourPlannerRepository, TourPlannerPostgresRepository>();

            // Navigation
            services.AddSingleton<INavigationService, NavigationService>(s =>
            {
                var navigationService = new NavigationService(s);
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

            // ViewModels
            services.AddSingleton<SearchViewModel>();
            services.AddTransient<AddTourViewModel>();
            services.AddTransient<TourLogViewModel>();
            services.AddTransient<EditTourViewModel>();
            services.AddTransient<TourDetailsViewModel>();
            services.AddSingleton<TourListViewModel>();
            services.AddTransient<TourPreviewViewModel>();
            services.AddSingleton<MainViewModel>();

            _serviceProvider = services.BuildServiceProvider();
        }
    }
}
