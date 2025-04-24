using Microsoft.EntityFrameworkCore;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace MovieApp.Utils
{
    public class DatabaseService
    {
        private readonly MovieDB _context;

        public DatabaseService()
        {
            _context = new MovieDB();
            InitializeDatabase();
        }

        public User? GetUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.username == username);
        }
        public bool ValidateUser(string username, string password)
        {
            var user = GetUser(username);
            if (user == null) return false;

            return BCrypt.Net.BCrypt.Verify(password, user.passwd);
        }

        public bool RegisterUser(string username, string password)
        {
            try
            {
                Debug.WriteLine($"Attempting to register user: {username}");

                if (_context.Users.Any(u => u.username == username))
                {
                    Debug.WriteLine("Username already exists in database");
                    return false;
                }

                var user = new User
                {
                    username = username,
                    passwd = BCrypt.Net.BCrypt.HashPassword(password)
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                Debug.WriteLine("User successfully registered");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database registration error: {ex.Message}");
                return false;
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                _context.Database.EnsureCreated();
                Debug.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization failed: {ex.Message}");
                throw; // Critical failure
            }
        }

        public void SeedMovies(ObservableCollection<MovieModel> apiMovies)
        {
            try
            {
                foreach (var movieModel in apiMovies)
                {
                    var existingMovie = _context.Movies
                        .FirstOrDefault(m => m.title == movieModel.PrimaryTitle);

                    if (existingMovie == null)
                    {
                        var movie = new Movie
                        {
                            api_id = movieModel.Id,
                            title = movieModel.PrimaryTitle,
                            genre = movieModel.Genres,
                            releaseYear = movieModel.StartYear,
                            runTime = movieModel.RuntimeMinutes
                        };
                        _context.Movies.Add(movie);
                    }
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error seeding movies: {ex.Message}");
            }
        }

        public void SeedUser(string username, string password)
        {
            try
            {
                var existingUser = _context.Users
                    .FirstOrDefault(u => u.username == username);

                if (existingUser == null)
                {
                    var user = new User
                    {
                        username = username,
                        passwd = password
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error seeding user: {ex.Message}");
            }
        }

        public void AddToWatchlist(int userId, string api_id, string listName = null)
        {
            try
            {
                Debug.WriteLine($"Attempting to add movie {api_id} to watchlist for user {userId}");

                var user = _context.Users
                    .Include(u => u.Watchlists)
                    .ThenInclude(w => w.Movie_Watchlists1)
                    .FirstOrDefault(u => u.user_id == userId);

                if (user == null)
                {
                    Debug.WriteLine("User not found");
                    return;
                }

                var watchlist = user.Watchlists.FirstOrDefault(w =>
                    (listName == null && w.isDefault) ||
                    (listName != null && w.list_name == listName));

                if (watchlist == null)
                {
                    Debug.WriteLine("Creating new watchlist");
                    watchlist = new Watchlist
                    {
                        User = user,
                isDefault = listName == null,
                list_name = listName == null ? "Watchlist" : listName
                    };
                    user.Watchlists.Add(watchlist);
                }

                var movie = _context.Movies.FirstOrDefault(m => m.api_id == api_id);
                if (movie == null)
                {
                    Debug.WriteLine("Movie not found in database");
                    return;
                }

                if (!watchlist.Movie_Watchlists1.Any(mw => mw.Movie.api_id == api_id))
                {
                    Debug.WriteLine("Adding movie to watchlist");
                    watchlist.Movie_Watchlists1.Add(new Movie_Watchlist { Movie = movie });
                    _context.SaveChanges();
                    Debug.WriteLine("Successfully added to watchlist");
                }
                else
                {
                    Debug.WriteLine("Movie already in watchlist");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in AddToWatchlist: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public bool IsInWatchlist(int userId, string movieId)
        {
            return _context.Movie_Watchlists
                .Include(mw => mw.Watchlist)
                .Include(mw => mw.Movie)
                .Any(mw => mw.Watchlist.User.user_id == userId &&
                          mw.Movie.api_id == movieId);
        }

        // MovieApp/Utils/DatabaseService.cs
        public void RemoveFromWatchlist(int userId, string movieApiId)
        {
            try
            {
                var itemToRemove = _context.Movie_Watchlists
                    .Include(mw => mw.Watchlist)
                    .ThenInclude(w => w.User)
                    .Include(mw => mw.Movie)
                    .FirstOrDefault(mw => mw.Watchlist.User.user_id == userId &&
                                        mw.Movie.api_id == movieApiId);

                if (itemToRemove != null)
                {
                    _context.Movie_Watchlists.Remove(itemToRemove);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error removing from watchlist: {ex.Message}");
                throw;
            }
        }

        public ObservableCollection<MovieModel> GetWatchlistMovies(int userId)
        {
            var watchlistMovies = new ObservableCollection<MovieModel>();

            var movies = _context.Movie_Watchlists
                .Include(mw => mw.Movie)
                .Include(mw => mw.Watchlist)
                .Where(mw => mw.Watchlist.User.user_id == userId)
                .Select(mw => new MovieModel
                {
                    Id = mw.Movie.api_id,
                    PrimaryTitle = mw.Movie.title,
                    Genres = mw.Movie.genre,
                    StartYear = mw.Movie.releaseYear,
                    RuntimeMinutes = mw.Movie.runTime,
                    IsInWatchlist = true // Since these are from watchlist
                })
                .ToList();

            foreach (var movie in movies)
            {
                watchlistMovies.Add(movie);
            }

            return watchlistMovies;
        }

        public List<string> GetWatchlistApiIds(int userId)
        {
            return _context.Movie_Watchlists
                .Include(mw => mw.Movie)
                .Include(mw => mw.Watchlist)
                .Where(mw => mw.Watchlist.User.user_id == userId)
                .Select(mw => mw.Movie.api_id)
                .ToList();
        }

        public void AddReview(int userId, int movieId, string content, int stars)
        {
            try
            {
                var existingReview = _context.Reviews
                    .FirstOrDefault(r => r.User.user_id == userId && r.Movie.movie_id == movieId);

                if (existingReview != null)
                {
                    existingReview.content = content;
                    existingReview.stars = stars;
                    existingReview.publish_date = DateTime.Now;
                }
                else
                {
                    var review = new Review
                    {
                        user_id = userId,
                        movie_id = movieId,
                        content = content,
                        stars = stars,
                        publish_date = DateTime.Now
                    };
                    _context.Reviews.Add(review);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding review: {ex.Message}");
            }
        }
    }
}
