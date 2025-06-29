using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public event EventHandler<string?>? SearchTextChanged;

        public ICommand SearchCommand { get; }

        public ICommand ClearCommand { get; }

        private string? searchText;
        public string? SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel()
        {
            SearchCommand = new RelayCommand((_) =>
            {
                SearchTextChanged?.Invoke(this, SearchText);
            });

            ClearCommand = new RelayCommand((_) =>
            {
                SearchText = "";
                SearchTextChanged?.Invoke(this, SearchText);
            });
        }
    }
}
