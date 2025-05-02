using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MovieApp.MVVM.ViewModel
{
    public partial class RatingsViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        public ObservableCollection<MovieModel> _ratedMovies;

        [ObservableProperty]
        private User? _currentUser;

        public RatingsViewModel()
        {
            _dbService = new DatabaseService();
            RatedMovies = new ObservableCollection<MovieModel>();
        }

        public void SetCurrentUser(User? user)
        {
            CurrentUser = user;
        }

        public void LoadRatedMovies(ObservableCollection<MovieModel> allMovies)
        {
            Boolean rated = false;

            if (CurrentUser == null || allMovies == null) return;

            try
            {
                var ratedApiIds = _dbService.GetRatedApiIds(CurrentUser.user_id);
                RatedMovies.Clear();

                foreach (var apiId in ratedApiIds)
                {
                    rated = false;
                    var movie = allMovies.FirstOrDefault(m => m.Id == apiId);
                    if (movie != null)
                    {
                        rated = movie.YourRating > 0;
                        RatedMovies.Add(movie);
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
