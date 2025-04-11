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
        private WatchlistViewModel _watchlistVM;                       //2.Watchlist Menü

        [ObservableProperty]
        private RatingsViewModel _ratingsVM;                       //3.Ratings Menü


        [ObservableProperty]
        private ListsViewModel _listsVM;                       //4.Lists Menü

        [ObservableProperty]
        private ReviewsViewModel _reviewsVM;                   //5.Reviews Menü

        [ObservableProperty]
        private ProfileViewModel _profileVM;                   //6.Profile Menü

        [ObservableProperty]
        private SettingsViewModel _settingsVM;                 //7.Settings Menü

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies;

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();

            WatchlistVM = new WatchlistViewModel();          //2.Watchlist Menü
            RatingsVM = new RatingsViewModel();              //3.Ratings Menü
            ListsVM = new ListsViewModel();                  //4.Lists Menü
            ReviewsVM = new ReviewsViewModel();              //5.Reviews Menü
            ProfileVM = new ProfileViewModel();              //6.Profile Menü
            SettingsVM = new SettingsViewModel();            //7.Settings Menü

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

        [RelayCommand]                                                  //2.Watchlist Menü
        private void NavigateToWatchlist()
        {
            if (CurrentView != WatchlistVM)
            {
                CurrentView = WatchlistVM;
            }
        }

        [RelayCommand]                                                  //3.Ratings Menü
        private void NavigateToRatings()
        {
            if (CurrentView != RatingsVM)
            {
                CurrentView = RatingsVM;
            }
        }

        [RelayCommand]                                                  //4.Lists Menü
        private void NavigateToLists()
        {
            if (CurrentView != ListsVM)
            {
                CurrentView = ListsVM;
            }
        }

        [RelayCommand]                                                  //5.Reviews Menü
        private void NavigateToReviews()
        {
            if (CurrentView != ReviewsVM)
            {
                CurrentView = ReviewsVM;
            }
        }

        [RelayCommand]                                                  //6.Profile Menü
        private void NavigateToProfile()
        {
            if (CurrentView != ProfileVM)
            {
                CurrentView = ProfileVM;
            }
        }

        [RelayCommand]                                                  //7.Settings Menü
        private void NavigateToSettings()
        {
            if (CurrentView != SettingsVM)
            {
                CurrentView = SettingsVM;
            }
        }

    }
}