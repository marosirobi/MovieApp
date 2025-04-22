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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
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

        public void AddToWatchlist(int userId, int movieId, string listName = null)
        {
            try
            {
                var user = _context.Users
                    .Include(u => u.Watchlists)
                    .ThenInclude(w => w.Movie_Watchlists1)
                    .FirstOrDefault(u => u.user_id == userId);

                if (user == null) return;

                var watchlist = user.Watchlists.FirstOrDefault(w =>
                    (listName == null && w.isDefault) ||
                    (listName != null && w.list_name == listName));

                if (watchlist == null)
                {
                    watchlist = new Watchlist
                    {
                        User = user,
                        isDefault = listName == null,
                        list_name = listName
                    };
                    user.Watchlists.Add(watchlist);
                }

                var movie = _context.Movies.Find(movieId);
                if (movie != null && !watchlist.Movie_Watchlists1.Any(mw => mw.Movie.movie_id == movieId))
                {
                    watchlist.Movie_Watchlists1.Add(new Movie_Watchlist { Movie = movie });
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding to watchlist: {ex.Message}");
            }
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
