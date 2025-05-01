using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.MVVM.View;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
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
        private MovieRatingViewModel _rateMovieVM;

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
        private bool _isRatingDialogOpen;

        [ObservableProperty]
        private User? _currentUser;

        private readonly Stack<object> _navigationStack = new Stack<object>();

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
            RateMovieVM = new MovieRatingViewModel();
            CurrentView = HomeVM;
            _navigationStack.Push(HomeVM);
        }

        private async Task InitializeMovies()
        {
            if (AllMovies == null || AllMovies.Count == 0)
            {
                var movies = await MovieApi.GetMoviesFromApi();
                AllMovies = new ObservableCollection<MovieModel>(movies);

                // Initialize watchlist status
                if (CurrentUser != null)
                {
                    var watchlistApiIds = _dbService.GetWatchlistApiIds(CurrentUser.user_id);
                    foreach (var movie in AllMovies)
                    {
                        movie.IsInWatchlist = watchlistApiIds.Contains(movie.Id);

                        var existingReview = _dbService.GetReview(CurrentUser.user_id, movie.Id);
                        movie.UpdateUserRating(existingReview?.stars);
                    }
                }

                HomeVM.SetMovies(AllMovies);
                _dbService.SeedMovies(AllMovies);

            }
        }
        private void NavigateToView(object viewModel)
        {
            if (CurrentView != viewModel)
            {
                _navigationStack.Push(CurrentView); // Save current view before changing
                CurrentView = viewModel;
            }
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            if (CurrentView != HomeVM)
            {
                NavigateToView(HomeVM);
                HomeVM.SetMovies(AllMovies);
            }
        }

        [RelayCommand]
        private void NavigateToTopMovies()
        {
            if (CurrentView != TopMoviesVM)
            {
                TopMoviesVM.SetMovies(AllMovies);
                NavigateToView(TopMoviesVM);
            }
        }

        [RelayCommand]
        private void NavigateToWatchlist()
        {
            if (CurrentView != WatchlistVM)
            {
                WatchlistVM.SetCurrentUser(CurrentUser);
                WatchlistVM.LoadWatchlistMovies(AllMovies);
                NavigateToView(WatchlistVM);
            }
        }

        [RelayCommand]
        private void NavigateToRatings()
        {
            if (CurrentView != RatingsVM)
            {
                NavigateToView(RatingsVM);

            }
        }

        [RelayCommand]
        private void NavigateToLists()
        {
            if (CurrentView != ListsVM)
            {
                NavigateToView(ListsVM);

            }
        }

        [RelayCommand]
        private void NavigateToReviews()
        {
            if (CurrentView != ReviewsVM)
            {
                NavigateToView(ReviewsVM);

            }
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            if (CurrentView != ProfileVM)
            {
                NavigateToView(ProfileVM);

            }
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            if (CurrentView != SettingsVM)
            {
                NavigateToView(SettingsVM);

            }
        }

        [RelayCommand]
        private void ShowMovieDetails(MovieModel movie)
        {
            if (movie != null)
            {
                SelectedMovie = movie;
                SelectedMovieVM.SetMovie(movie);
                NavigateToView(SelectedMovieVM);

            }
        }

        [RelayCommand]
        private void RateMovie(MovieModel movie)
        {
            if (movie != null && CurrentUser != null)
            {
                // Force fresh initialization
                RateMovieVM.SetMovie(movie);
                RateMovieVM.CurrentUser = CurrentUser;

                Debug.WriteLine($"Opening rating dialog for {movie.PrimaryTitle}");
                IsRatingDialogOpen = true;
            }
        }

        [RelayCommand]
        private void OnRatingSubmitted()
        {
            if (SelectedMovie != null && CurrentUser != null)
            {
                // Update all instances of this movie
                RefreshMovieRatings(SelectedMovie.Id, RateMovieVM.Rating);
                IsRatingDialogOpen = false;
            }
        }
        private void RefreshMovieRatings(string movieId, int newRating)
        {
            var movieInCollection = AllMovies.FirstOrDefault(m => m.Id == movieId);
            if (movieInCollection != null)
            {
                movieInCollection.UpdateUserRating(newRating);
            }
            if (SelectedMovie?.Id == movieId)
            {
                SelectedMovie.UpdateUserRating(newRating);
            }
        }

        [RelayCommand]
        private void CloseRatingDialog()
        {
            IsRatingDialogOpen = false; // Hide the popup
        }

        [RelayCommand]
        private void NavigateBack()
        {
            if (_navigationStack.Count > 1) // Don't pop the last item (Home)
            {
                var previousView = _navigationStack.Pop();
                CurrentView = previousView;
            }
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
                }
                else
                {
                    _dbService.RemoveFromWatchlist(CurrentUser.user_id, movie.Id);

                    // If we're currently viewing the watchlist, remove the movie from the displayed collection
                    if (CurrentView is WatchlistViewModel watchlistVM)
                    {
                        watchlistVM.WatchlistMovies.Remove(movie);
                    }
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