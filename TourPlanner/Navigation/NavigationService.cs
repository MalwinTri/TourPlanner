using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TourPlanner.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace TourPlanner.Navigation
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Func<BaseViewModel, Window>> _viewMapping;
        private readonly Window? _window;

        // dependency injection
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _viewMapping = new();
        }

        private NavigationService(IServiceProvider serviceProvider, Dictionary<Type, Func<BaseViewModel, Window>> viewMapping, Window? window)
        {
            // only for internal use when navigating to a new window
            _serviceProvider = serviceProvider;
            _viewMapping = viewMapping;
            _window = window;
        }

        public bool? NavigateTo<TViewModel>(TViewModel viewModel, NavigationMode navigationMode = NavigationMode.Modal)
            where TViewModel : BaseViewModel
        {
            // the optional result, if the window was presented modally as a dialog
            bool? result = null;

            // find the instantiation function for the provided ViewModel type
            var instantiator = _viewMapping[viewModel.GetType()];

            // use it to instantiate the window
            var window = instantiator(viewModel);

            switch (navigationMode)
            {
                case NavigationMode.Modal:
                    // show as a modal dialog
                    window.Owner = _window ?? Application.Current.MainWindow;
                    result = window.ShowDialog();
                    break;
                case NavigationMode.Modeless:
                    // show as a modeless window
                    window.Show();
                    break;
            }

            return result;
        }

        // navigation with dependency injection
        public bool? NavigateTo<TViewModel>(NavigationMode navigationMode = NavigationMode.Modal) where TViewModel : BaseViewModel
        {
            // let the service provider instantiate the view model of type TViewModel
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            return NavigateTo(viewModel, navigationMode);
        }

        public void Close()
        {
            // close the associated window, if available
            _window?.Close();
        }

        public string? OpenFileDialog(string? filter = null)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public string? SaveFileDialog(string? filter = null)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = filter
            };
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public void ShowMessageBox(string message, string title, MessageBoxImage icon = MessageBoxImage.None)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        public void RegisterNavigation<TViewModel, TWindow>(Action<TViewModel, TWindow>? initializer = null)
            where TViewModel : BaseViewModel
            where TWindow : Window, new()
        {
            // create a new instantiation function which creates the window, attaches the view model
            // and calls the optionally provided initializer
            var instantiator = (BaseViewModel vm) =>
            {
                // downcast from the base class to the actual type
                var viewModel = (TViewModel)vm;

                // instantiate the window with "new" and set the view model
                // as DataContext
                var window = new TWindow
                {
                    DataContext = viewModel
                };

                // create a new navigation service with the same view mapping
                // and store the associated window as additional information for closing
                viewModel.NavigationService = new NavigationService(_serviceProvider, _viewMapping, window); ;

                // invoke the optionally provided initialization function
                initializer?.Invoke(viewModel, window);

                return window;
            };

            _viewMapping[typeof(TViewModel)] = instantiator;
        }
    }
}
