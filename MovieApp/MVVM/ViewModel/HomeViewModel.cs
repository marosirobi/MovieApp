using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace MovieApp.MVVM.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        private const int TotalMoviesToKeep = 20;
        private readonly Random _random = new Random();
        private int _currentPage = 0;
        private int _moviesPerPage = 6; // Default value

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies = new ObservableCollection<MovieModel>();

        [ObservableProperty]
        private ObservableCollection<MovieModel> _visibleMovies = new ObservableCollection<MovieModel>();

        public int MoviesPerPage
        {
            get => _moviesPerPage;
            set
            {
                if (SetProperty(ref _moviesPerPage, value))
                {
                    UpdateVisibleMovies();
                    OnPropertyChanged(nameof(CanGoNext));
                    OnPropertyChanged(nameof(CanGoPrevious));
                }
            }
        }

        public bool CanGoNext => (_currentPage + 1) * MoviesPerPage < AllMovies.Count;
        public bool CanGoPrevious => _currentPage > 0;

        public void CalculateMoviesPerPage(double gridWidth, double movieCardWidth)
        {
            MoviesPerPage = Math.Max(1, (int)(gridWidth / movieCardWidth));
        }

        public void SetMovies(ObservableCollection<MovieModel> allMovies)
        {
            try
            {
                Debug.WriteLine("Setting 20 random movies from pre-loaded data...");
                AllMovies.Clear();
                VisibleMovies.Clear();

                if (allMovies?.Count > 0)
                {
                    var random20 = allMovies
                        .OrderBy(x => _random.Next())
                        .Take(TotalMoviesToKeep)
                        .ToList();

                    foreach (var movie in random20)
                    {
                        AllMovies.Add(movie);
                    }

                    _currentPage = 0;
                    UpdateVisibleMovies();
                    Debug.WriteLine($"Set {random20.Count} random movies");
                }
                else
                {
                    Debug.WriteLine("No movies received, loading fallback data");
                    LoadFallbackMovies();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Critical error: {ex}");
                LoadFallbackMovies();
            }
        }

        public void NextPage()
        {
            if (CanGoNext)
            {
                _currentPage++;
                UpdateVisibleMovies();
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
            }
        }

        public void PreviousPage()
        {
            if (CanGoPrevious)
            {
                _currentPage--;
                UpdateVisibleMovies();
                OnPropertyChanged(nameof(CanGoNext));
                OnPropertyChanged(nameof(CanGoPrevious));
            }
        }

        private void UpdateVisibleMovies()
        {
            VisibleMovies.Clear();
            var startIndex = _currentPage * MoviesPerPage;
            var moviesToShow = AllMovies.Skip(startIndex).Take(MoviesPerPage).ToList();

            foreach (var movie in moviesToShow)
            {
                VisibleMovies.Add(movie);
            }
        }

        private void LoadFallbackMovies()
        {
            var fallbackTitles = new List<string>
            {
                "The Shawshank Redemption", "The Godfather", "The Dark Knight",
                "Pulp Fiction", "Fight Club", "Forrest Gump", "Inception",
                "The Matrix", "Goodfellas", "The Silence of the Lambs"
            };

            AllMovies.Clear();
            VisibleMovies.Clear();

            for (int i = 0; i < TotalMoviesToKeep; i++)
            {
                var randomTitle = fallbackTitles[_random.Next(fallbackTitles.Count)];
                AllMovies.Add(new MovieModel
                {
                    PrimaryTitle = randomTitle,
                    PrimaryImage = $"https://via.placeholder.com/200x300?text={Uri.EscapeDataString(randomTitle)}",
                    AverageRating = Math.Round(_random.NextDouble() * 3 + 7, 1)
                });
            }

            _currentPage = 0;
            UpdateVisibleMovies();
        }
    }
}