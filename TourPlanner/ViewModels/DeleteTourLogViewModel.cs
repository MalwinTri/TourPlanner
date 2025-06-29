using System;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public class DeleteTourLogViewModel
    {
        public ICommand ConfirmDeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public Action CloseAction { get; set; }

        public DeleteTourLogViewModel()
        {
            ConfirmDeleteCommand = new RelayCommand(ExecuteConfirmDelete);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteConfirmDelete(object obj)
        {
            CloseAction?.Invoke();
        }

        private void ExecuteCancel(object obj)
        {
            CloseAction?.Invoke();
        }
    }
}
