using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;

namespace MovieApp.MVVM.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private HomeViewModel _homeVM;
        [ObservableProperty]
        private DiscoveryViewModel _discoveryVM;


        public MainViewModel()
        {
            
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();

            CurrentView = HomeVM;
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            CurrentView = HomeVM;
        }

        [RelayCommand]
        private void NavigateToDiscovery()
        {
            CurrentView = DiscoveryVM;
        }
    }
}