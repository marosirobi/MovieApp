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
            if (listName != null && listName.Length > 255)
                throw new ArgumentException("List name cannot exceed 255 characters");
            
            using var freshContext = new MovieDB();
            using var transaction = freshContext.Database.BeginTransaction();

            try
            {
                Debug.WriteLine($"Attempting to toggle movie {api_id} in {(listName == null ? "default watchlist" : $"list '{listName}'")} for user {userId}");

                // 1. Verify movie exists first - don't track the entity
                var movie = freshContext.Movies
                    .AsNoTracking()
                    .FirstOrDefault(m => m.api_id == api_id);

                if (movie == null)
                {
                    Debug.WriteLine("Movie not found in database");
                    return;
                }

                // 2. Get user with watchlists
                var user = freshContext.Users
                    .Include(u => u.Watchlists)
                    .ThenInclude(w => w.Movie_Watchlists1)
                    .FirstOrDefault(u => u.user_id == userId);

                if (user == null)
                {
                    Debug.WriteLine("User not found");
                    return;
                }

                // 3. Find or create appropriate watchlist
                Watchlist watchlist;

                if (listName == null)
                {
                    // Default watchlist case
                    watchlist = user.Watchlists.FirstOrDefault(w => w.isDefault);
                    if (watchlist == null)
                    {
                        watchlist = new Watchlist
                        {
                            User = user,  // Use navigation property instead of UserId
                            isDefault = true,
                            list_name = "Watchlist",
                            create_date = DateTime.Now
                        };
                        freshContext.Watchlists.Add(watchlist);
                        freshContext.SaveChanges(); // Save to get ID
                    }
                }
                else
                {
                    // Custom list case
                    watchlist = user.Watchlists.FirstOrDefault(w => w.list_name == listName);
                    if (watchlist == null)
                    {
                        watchlist = new Watchlist
                        {
                            User = user,  // Use navigation property instead of UserId
                            isDefault = false,
                            list_name = listName,
                            create_date = DateTime.Now
                        };
                        freshContext.Watchlists.Add(watchlist);
                        freshContext.SaveChanges(); // Save to get ID
                    }
                }

                // 4. Check for existing entry
                var existingEntry = freshContext.Movie_Watchlists
                    .FirstOrDefault(mw => mw.Watchlist.watchlist_id == watchlist.watchlist_id
                                       && mw.Movie.api_id == api_id);

                if (existingEntry == null)
                {
                    // Add to watchlist - let EF handle the relationships
                    var movieToAdd = freshContext.Movies.First(m => m.api_id == api_id);
                    var newEntry = new Movie_Watchlist
                    {
                        Movie = movieToAdd,
                        Watchlist = watchlist,
                        added_date = DateTime.Now
                    };

                    freshContext.Movie_Watchlists.Add(newEntry);
                    freshContext.SaveChanges();
                    transaction.Commit();
                    Debug.WriteLine($"Successfully added movie to {(listName == null ? "default watchlist" : $"list '{listName}'")}");
                }
                else
                {
                    // Remove from watchlist
                    freshContext.Movie_Watchlists.Remove(existingEntry);
                    freshContext.SaveChanges();
                    transaction.Commit();
                    Debug.WriteLine($"Successfully removed movie from {(listName == null ? "default watchlist" : $"list '{listName}'")}");
                }
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { /* Ignore rollback errors */ }
                Debug.WriteLine($"Error in AddOrRemoveFromWatchlist: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw; // Re-throw to handle in UI
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
        public ObservableCollection<MovieModel> GetListMovies(int userId, ObservableCollection<MovieModel> allMovies, string list_name)
        {
            var ListApiIds = GetListApiIds(userId, list_name); 
            var ListedMovies = new ObservableCollection<MovieModel>();

            foreach (var apiId in ListApiIds)
            {
                var movie = allMovies.FirstOrDefault(m => m.Id == apiId);
                if (movie != null)
                {
                    ListedMovies.Add(movie);
                }
            }
            return ListedMovies;
        }
        public List<string> GetUserLists(int userId)
        {
            return _context.Watchlists
                .Where(w => w.User.user_id == userId)
                .Select(w => w.list_name)
                .OrderBy(name => name)
                .ToList();
        }
        public List<string?> GetListApiIds(int userId, string list_name)
        {
            if (string.IsNullOrEmpty(list_name)) return new List<string>();

            return _context.Movie_Watchlists
                .Include(mw => mw.Movie)
                .Include(mw => mw.Watchlist)
                .Where(mw => mw.Watchlist.User.user_id == userId && mw.Watchlist.list_name == list_name)
                .Select(mw => mw.Movie.api_id)
                .ToList();
        }
        public List<string?> GetRatedApiIds(int userId)
        {
            return _context.Reviews
            .Include(r => r.Movie)
            .Where(r => r.user_id == userId)
            .Select(r => r.Movie.api_id)
            .ToList();
        }

        public void AddReview(int userId, string movieApiId, int stars, string content = null)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var movie = _context.Movies
                    .Include(m => m.Reviews)
                    .FirstOrDefault(m => m.api_id == movieApiId);

                if (movie == null)
                {
                    return;
                }

                // Handle existing review
                var existingReview = movie.Reviews.FirstOrDefault(r => r.user_id == userId);
                if (existingReview != null)
                {
                    existingReview.stars = stars;
                    existingReview.publish_date = DateTime.Now;
                }
                else
                {
                    movie.Reviews.Add(new Review
                    {
                        user_id = userId,
                        stars = stars,
                        publish_date = DateTime.Now
                    });
                    movie.review_count++;
                }

                // Calculate new average - explicitly using decimal
                movie.avg_rating = (decimal)movie.Reviews.Average(r => r.stars);

                _context.SaveChanges();
                transaction.Commit();


            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw; // Re-throw to handle in UI
            }
        }
        public Review? GetReview(int userId, string movieApiId)
        {
            return _context.Reviews
                .Include(r => r.Movie)
                .FirstOrDefault(r => r.user_id == userId && r.Movie.api_id == movieApiId);
        }
        public void DeleteReview(int userId, string movieApiId)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var movie = _context.Movies
                    .Include(m => m.Reviews)
                    .FirstOrDefault(m => m.api_id == movieApiId);

                if (movie == null) return;

                var review = movie.Reviews.FirstOrDefault(r => r.user_id == userId);
                if (review != null)
                {
                    _context.Reviews.Remove(review);
                    movie.review_count--;

                    // Recalculate average if there are remaining reviews
                    movie.avg_rating = movie.Reviews.Any()
                        ? (decimal)movie.Reviews.Average(r => r.stars)
                        : 0;

                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine($"Error deleting review: {ex.Message}");
                throw;
            }
        }
        public List<Watchlist> GetUserCustomLists(int userId)
        {
            return _context.Watchlists
                    .Include(w => w.User)
                    .Include(w => w.Movie_Watchlists1)
                    .ThenInclude(mw => mw.Movie)
                    .Where(w => w.User.user_id == userId && !w.isDefault)
                    .OrderBy(w => w.list_name)
                    .AsEnumerable()
                    .ToList();
        }
        public void CreateCustomList(int userId, string listName)
        {
            if (string.IsNullOrWhiteSpace(listName))
                throw new ArgumentException("List name cannot be empty");

            if (listName.Length > 255)
                throw new ArgumentException("List name cannot exceed 255 characters");

            var user = _context.Users.Find(userId);
            if (user == null) return;

            // Check if list already exists
            if (_context.Watchlists.Any(w => w.User.user_id == userId && w.list_name == listName))
            {
                throw new InvalidOperationException("A list with this name already exists");
            }

            var newList = new Watchlist
            {
                User = user,
                list_name = listName,
                isDefault = false,
                create_date = DateTime.Now
            };

            _context.Watchlists.Add(newList);
            _context.SaveChanges();
        }
        public void RemoveFromCustomList(int userId, string movieApiId, string listName)
        {
            var itemToRemove = _context.Movie_Watchlists
                .Include(mw => mw.Watchlist)
                .ThenInclude(w => w.User)
                .Include(mw => mw.Movie)
                .FirstOrDefault(mw => mw.Watchlist.User.user_id == userId &&
                                    mw.Movie.api_id == movieApiId &&
                                    mw.Watchlist.list_name == listName);

            if (itemToRemove != null)
            {
                _context.Movie_Watchlists.Remove(itemToRemove);
                _context.SaveChanges();
            }
        }

        public bool IsInCustomList(int userId, string movieApiId, string listName)
{
    return _context.Movie_Watchlists
        .Include(mw => mw.Watchlist)
        .ThenInclude(w => w.User)
        .Include(mw => mw.Movie)
        .Any(mw => mw.Watchlist.User.user_id == userId &&
                  mw.Movie.api_id == movieApiId &&
                  mw.Watchlist.list_name == listName &&
                          !mw.Watchlist.isDefault);
}
    }
}
