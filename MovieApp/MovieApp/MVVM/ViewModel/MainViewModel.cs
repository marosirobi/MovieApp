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

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies;

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();
            CurrentView = HomeVM;

            // Start loading movies when MainViewModel is created
            _ = InitializeMovies();
        }

        private async Task InitializeMovies()
        {
            // Only load if we don't have movies already
            if (AllMovies == null || AllMovies.Count == 0)
            {
                AllMovies = await MovieApi.GetMoviesFromApi();
                HomeVM.SetMovies(AllMovies);
            }
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            if (CurrentView != HomeVM)
            {
                CurrentView = HomeVM;
                HomeVM.SetMovies(AllMovies);
            }
        }

        [RelayCommand]
        private void NavigateToDiscovery()
        {
            if (CurrentView != DiscoveryVM)
            {
                CurrentView = DiscoveryVM;
            }
        }
    }
}