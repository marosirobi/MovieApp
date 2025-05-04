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

        public void LoadWatchlistMovies(ObservableCollection<MovieModel> allMovies, string list_name = "Watchlist")
        {
            if (CurrentUser == null || allMovies == null) return;

            try
            {
                WatchlistMovies.Clear();
                WatchlistMovies = _dbService.GetListMovies(CurrentUser.user_id, allMovies,list_name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading watchlist: {ex.Message}");
            }
        }
    }
}