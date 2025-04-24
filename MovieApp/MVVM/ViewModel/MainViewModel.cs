using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.MVVM.View;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace MovieApp.MVVM.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private HomeViewModel _homeVM;

        [ObservableProperty]
        private TopMoviesViewModel _topMoviesVM;

        [ObservableProperty]
        private SelectedMoviePageViewModel _selectedMovieVM;

        [ObservableProperty]
        private WatchlistViewModel _watchlistVM;

        [ObservableProperty]
        private RatingsViewModel _ratingsVM;

        [ObservableProperty]
        private ListsViewModel _listsVM;

        [ObservableProperty]
        private ReviewsViewModel _reviewsVM;

        [ObservableProperty]
        private ProfileViewModel _profileVM;

        [ObservableProperty]
        private SettingsViewModel _settingsVM;

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies;

        [ObservableProperty]
        private MovieModel _selectedMovie;

        [ObservableProperty]
        private User? _currentUser;

        public MainViewModel()
        {
            // Normal initialization
            _ = InitializeMovies();

            _dbService = new DatabaseService();

            HomeVM = new HomeViewModel();
            TopMoviesVM = new TopMoviesViewModel();
            SelectedMovieVM = new SelectedMoviePageViewModel();
            WatchlistVM = new WatchlistViewModel();
            RatingsVM = new RatingsViewModel();
            ListsVM = new ListsViewModel();
            ReviewsVM = new ReviewsViewModel();
            ProfileVM = new ProfileViewModel();
            SettingsVM = new SettingsViewModel();
            CurrentView = HomeVM;
            
        }

        private async Task InitializeMovies()
        {
            if (AllMovies == null || AllMovies.Count == 0)
            {
                var movies = await MovieApi.GetMoviesFromApi();

                // Initialize watchlist status for each movie
                if (CurrentUser != null)
                {
                    foreach (var movie in movies)
                    {
                        movie.IsInWatchlist = _dbService.IsInWatchlist(CurrentUser.user_id, movie.Id);
                    }
                }
                AllMovies = new ObservableCollection<MovieModel>(movies);

                HomeVM.SetMovies(AllMovies);
                _dbService.SeedMovies(AllMovies);
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
        private void NavigateToTopMovies()
        {
            if (CurrentView != TopMoviesVM)
            {
                TopMoviesVM.SetMovies(AllMovies);
                CurrentView = TopMoviesVM;
            }
        }

        [RelayCommand]
        private void NavigateToWatchlist()
        {
            if (CurrentView != WatchlistVM)
            {
                CurrentView = WatchlistVM;
            }
        }

        [RelayCommand]
        private void NavigateToRatings()
        {
            if (CurrentView != RatingsVM)
            {
                CurrentView = RatingsVM;
            }
        }

        [RelayCommand]
        private void NavigateToLists()
        {
            if (CurrentView != ListsVM)
            {
                CurrentView = ListsVM;
            }
        }

        [RelayCommand]
        private void NavigateToReviews()
        {
            if (CurrentView != ReviewsVM)
            {
                CurrentView = ReviewsVM;
            }
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            if (CurrentView != ProfileVM)
            {
                CurrentView = ProfileVM;
            }
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            if (CurrentView != SettingsVM)
            {
                CurrentView = SettingsVM;
            }
        }

        [RelayCommand]
        private void ShowMovieDetails(MovieModel movie)
        {
            if (movie != null)
            {
                SelectedMovie = movie;
                SelectedMovieVM.SetMovie(movie);
                CurrentView = SelectedMovieVM;
            }
        }

        [RelayCommand]
        private void NavigateBack()
        {
            CurrentView = HomeVM;
        }

        [RelayCommand]
        private void ToggleWatchlist(MovieModel movie)
        {
            if (CurrentUser == null || movie == null) return;

            try
            {
                // Toggle the watchlist status
                movie.IsInWatchlist = !movie.IsInWatchlist;

                if (movie.IsInWatchlist)
                {
                    _dbService.AddToWatchlist(CurrentUser.user_id, movie.Id);
                    //MessageBox.Show($"{movie.PrimaryTitle} added to watchlist!");
                }
                else
                {
                    _dbService.RemoveFromWatchlist(CurrentUser.user_id, movie.Id);
                    //MessageBox.Show($"{movie.PrimaryTitle} removed from watchlist!");
                }
            }
            catch (Exception ex)
            {
                // Revert on error
                movie.IsInWatchlist = !movie.IsInWatchlist;
                MessageBox.Show($"Error: {ex.Message}");
            }
        }



    }

}