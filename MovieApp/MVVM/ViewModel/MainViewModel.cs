using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.MVVM.View;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
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
        private CustomListsViewModel _customListsVM;

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies;

        [ObservableProperty]
        private MovieModel _selectedMovie;

        [ObservableProperty]
        private bool _isRatingDialogOpen;

        [ObservableProperty]
        private bool _isListDialogOpen;

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<MovieModel> _searchResults = new();

        [ObservableProperty]
        private bool _showSearchResults;

        private CancellationTokenSource _searchCancellationTokenSource;

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
            CustomListsVM = new CustomListsViewModel();
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
                    var watchlistApiIds = _dbService.GetListApiIds(CurrentUser.user_id,"Watchlist");
                    foreach (var movie in AllMovies)
                    {
                        movie.IsInWatchlist = watchlistApiIds.Contains(movie.Id);
                        movie.AverageRating = _dbService.GetMovieRating(movie.Id);
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
                TopMoviesVM.SetMoviesAsync(AllMovies.OrderByDescending(m => m.AverageRating));
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
                RatingsVM.SetCurrentUser(CurrentUser);
                RatingsVM.LoadRatedMovies(AllMovies);
                NavigateToView(RatingsVM);

            }
        }

        [RelayCommand]
        private void NavigateToLists()
        {
            if (CurrentView != ListsVM)
            {
                ListsVM.SetCurrentUser(CurrentUser);
                ListsVM.Initialize(AllMovies);
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
        private void CloseApp()
        {
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive)?
                .Close();
        }
        [RelayCommand]
        private void MinimizeApp()
        {
            var window = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive);

            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        [RelayCommand]
        private void MaximizeApp()
        {
            var window = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive);
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.WindowState = WindowState.Normal;
                }
                else
                {
                    window.WindowState = WindowState.Maximized;
                }
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
        private void CloseDialog()
        {
            
            IsRatingDialogOpen = false;
            IsListDialogOpen = false;
        }

        [RelayCommand]
        private void NavigateBack()
        {
            if (_navigationStack.Count > 1) // Don't pop the last item (Home)
            {
                var previousView = _navigationStack.Pop();
                Debug.WriteLine(previousView);
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

        [RelayCommand]
        private void ShowCustomLists(MovieModel movie)
        {
            if (movie != null && CurrentUser != null)
            {
                CustomListsVM.CurrentMovie = movie;
                CustomListsVM.CurrentUser = CurrentUser;
                CustomListsVM.LoadCustomLists();
                IsListDialogOpen = true;
            }
                
        }

        partial void OnSearchTextChanged(string value)
        {
            // Cancel previous search if it was still running
            _searchCancellationTokenSource?.Cancel();
            _searchCancellationTokenSource = new CancellationTokenSource();

            if (string.IsNullOrWhiteSpace(value))
            {
                ClearSearch();
                return;
            }

            // Debounce the search to avoid too many requests
            Task.Run(async () =>
            {
                await Task.Delay(200, _searchCancellationTokenSource.Token);

                if (!_searchCancellationTokenSource.IsCancellationRequested)
                {
                    Application.Current.Dispatcher.Invoke(() => SearchMoviesCommand.Execute(null));
                }
            }, _searchCancellationTokenSource.Token);
        }

        [RelayCommand]
        private void SearchMovies()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                ClearSearch();
                return;
            }

            try
            {
                var results = AllMovies
                    .Where(m => m.PrimaryTitle?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true)
                    .OrderBy(m => m.PrimaryTitle)
                    .Take(5)
                    .ToList();

                SearchResults.Clear();
                foreach (var movie in results)
                {
                    SearchResults.Add(movie);
                }
                ShowSearchResults = SearchResults.Count > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Search error: {ex.Message}");
                ClearSearch();
            }
        }

        [RelayCommand]
        private void SelectMovieFromSearch(MovieModel movie)
        {
            if (movie == null) return;

            SelectedMovie = movie;
            SelectedMovieVM.SetMovie(movie);
            NavigateToView(SelectedMovieVM);
            ClearSearch();
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = string.Empty;
            SearchResults.Clear();
            ShowSearchResults = false;
        }

        [RelayCommand]
        private void Closed()
        {

            IsRatingDialogOpen = false;
            IsListDialogOpen = false;
            ListsVM.LoadListedMovies();
        }
    }

}