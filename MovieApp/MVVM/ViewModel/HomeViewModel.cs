using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;


namespace MovieApp.MVVM.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        public ObservableCollection<MovieModel> Movies { get; } = new ObservableCollection<MovieModel>();
        private readonly Random _random = new Random();

        public async Task LoadRandom20Movies()
        {
            try
            {
                Debug.WriteLine("Loading 20 random movies...");

                var allMovies = await MovieApi.GetMoviesFromApi();
                Movies.Clear();

                if (allMovies?.Count > 0)
                {
                    // Take 20 random movies
                    var random20 = allMovies
                        .OrderBy(x => _random.Next())
                        .Take(allMovies.Count)
                        .ToList();

                    foreach (var movie in random20)
                    {
                        Movies.Add(movie);
                    }
                    Debug.WriteLine($"Loaded {random20.Count} random movies");
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

        private void LoadFallbackMovies()
        {
            // Generate 20 random fallback movies
            var fallbackTitles = new List<string>
    {
        "The Shawshank Redemption", "The Godfather", "The Dark Knight",
        "Pulp Fiction", "Fight Club", "Forrest Gump", "Inception",
        "The Matrix", "Goodfellas", "The Silence of the Lambs"
    };

            for (int i = 0; i < 20; i++)
            {
                var randomTitle = fallbackTitles[_random.Next(fallbackTitles.Count)];
                Movies.Add(new MovieModel
                {
                    PrimaryTitle = randomTitle,
                    PrimaryImage = $"https://via.placeholder.com/200x300?text={Uri.EscapeDataString(randomTitle)}",
                    AverageRating = Math.Round(_random.NextDouble() * 3 + 7, 1) // Random rating 7-10
                });
            }
        }

    }
}
