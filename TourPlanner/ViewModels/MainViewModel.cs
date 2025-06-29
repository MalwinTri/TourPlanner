using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.Models.TourPlanner.Models;

namespace TourPlanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Beispiel-Properties für Sub-ViewModels
        public AddTourViewModel AddTourVM { get; }
        public EditTourViewModel EditTourVM { get; }
        public TourListViewModel TourListVM { get; }
        //public TourLogsViewModel TourLogsVM { get; }
        //public TourDetailViewModel TourDetailVM { get; }

        // Beispiel-Command für das Öffnen eines Views
        public ICommand OpenAddTourCommand { get; }

        public MainViewModel(
            AddTourViewModel addTourVM,
            EditTourViewModel editTourVM,
            TourListViewModel tourListVM)
            //TourLogsViewModel tourLogsVM,
            //TourDetailViewModel tourDetailVM)
        {
            AddTourVM = addTourVM;
            EditTourVM = editTourVM;
            TourListVM = tourListVM;
            //TourLogsVM = tourLogsVM;
            //TourDetailVM = tourDetailVM;

            // Beispielcommand initialisieren
            OpenAddTourCommand = new RelayCommand(OpenAddTourDialog);
        }

        private void OpenAddTourDialog(object? parameter)
        {
            // Hier z. B. Fenster öffnen oder View wechseln
            NavigationService?.NavigateTo(AddTourVM);
        }
    }
}