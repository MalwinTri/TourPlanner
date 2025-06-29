using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using TourPlanner.ViewModels;

namespace TourPlanner.Navigation
{
    public interface INavigationService
    {
        bool? NavigateTo<TViewModel>(TViewModel viewModel, NavigationMode navigationMode = NavigationMode.Modal) where TViewModel : BaseViewModel;

        bool? NavigateTo<TViewModel>(NavigationMode navigationMode = NavigationMode.Modal) where TViewModel : BaseViewModel;

        void Close();

        string? OpenFileDialog(string? filter = null);

        string? SaveFileDialog(string? filter = null);

        void ShowMessageBox(string message, string title, MessageBoxImage icon = MessageBoxImage.None);

    }
}
