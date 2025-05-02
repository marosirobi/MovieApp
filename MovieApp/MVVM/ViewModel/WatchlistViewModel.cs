using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MovieApp.MVVM.ViewModel
{
    public partial class WatchlistViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        public ObservableCollection<MovieModel> _watchlistMovies;

        [ObservableProperty]
        private User? _currentUser;

        public WatchlistViewModel()
        {
            _dbService = new DatabaseService();
            WatchlistMovies = new ObservableCollection<MovieModel>();
        }

        public void SetCurrentUser(User? user)
        {
            CurrentUser = user;
        }

        public void LoadWatchlistMovies(ObservableCollection<MovieModel> allMovies)
        {
            if (CurrentUser == null || allMovies == null) return;

            try
            {
                var watchlistApiIds = _dbService.GetWatchlistApiIds(CurrentUser.user_id);
                WatchlistMovies.Clear();

                foreach (var apiId in watchlistApiIds)
                {
                    var movie = allMovies.FirstOrDefault(m => m.Id == apiId);
                    if (movie != null)
                    {
                        movie.IsInWatchlist = true;
                        WatchlistMovies.Add(movie);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading watchlist: {ex.Message}");
            }
        }
    }
}