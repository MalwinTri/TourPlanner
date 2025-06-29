using System.Windows;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public class DeleteTourViewModel
    {
        public bool Confirmed { get; private set; }

        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }

        public DeleteTourViewModel()
        {
            CancelCommand = new RelayCommand(_ => Cancel());
            DeleteCommand = new RelayCommand(_ => Delete());
        }

        private void Cancel()
        {
            Confirmed = false;
            CloseWindow?.Invoke(false);
        }

        private void Delete()
        {
            Confirmed = true;
            CloseWindow?.Invoke(true);
        }

        public Action<bool> CloseWindow { get; set; }
    }
}
