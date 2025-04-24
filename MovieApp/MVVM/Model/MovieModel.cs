using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Globalization;

namespace MovieApp.MVVM.Model
{
    public class MovieModel : ObservableObject
    {
        private bool _isInWatchlist;
        public bool IsInWatchlist
        {
            get => _isInWatchlist;
            set => SetProperty(ref _isInWatchlist, value); // This triggers UI updates
        }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("primaryTitle")]
        public string? PrimaryTitle { get; set; }

        [JsonProperty("primaryImage")]
        public string? PrimaryImage { get; set; }

        [JsonProperty("averageRating")]
        public double AverageRating { get; set; }

        [JsonProperty("genres")]
        public string[]? Genres { get; set; }

        [JsonProperty("startYear")]
        public int StartYear { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("contentRating")]
        public string? ContentRating { get; set; }

        // Raw runtime
        [JsonProperty("runtimeMinutes")]
        public int? RuntimeMinutes { get; set; }

        // Raw Budget
        [JsonProperty("budget")]
        public string? Budget { get; set; }

        // Formatted runtime
        [JsonIgnore]
        public string FormattedRuntime => GetFormattedRuntime();

        // Formatted budget
        [JsonIgnore]
        public string? FormattedBudget => GetFormattedBudget();

        private string GetFormattedRuntime()
        {
            if (!RuntimeMinutes.HasValue) return "N/A";

            var minutes = RuntimeMinutes.Value;
            var hours = minutes / 60;
            var remainingMinutes = minutes % 60;

            return hours > 0
                ? remainingMinutes > 0
                    ? $"{hours}h {remainingMinutes}m"
                    : $"{hours}h"
                : $"{remainingMinutes}m";
        }
        private string? GetFormattedBudget()
        {
            if (string.IsNullOrEmpty(Budget) || Budget == "0")
                return "N/A";

            // Try to parse the budget as a number
            if (long.TryParse(Budget, out long budgetValue))
            {
                // Format with commas and dollar sign
                return "$" + budgetValue.ToString("N0", CultureInfo.InvariantCulture);
            }

            // If not a number, return as-is
            return Budget;
        }
    }


    public class MovieBuilder
    {
        private readonly MovieModel movie = new MovieModel();

        public MovieBuilder SetId(string id)
        {
            movie.Id = id;
            return this;
        }
        public MovieBuilder SetPrimaryTitle(string title)
        {
            movie.PrimaryTitle = title;
            return this;
        }
        public MovieBuilder SetPrimaryImage(string imageUrl)
        {
            movie.PrimaryImage = imageUrl;
            return this;
        }
        public MovieBuilder SetAverageRating(double rating)
        {
            movie.AverageRating = rating;
            return this;
        }
        public MovieBuilder SetGenre(string[] genres)
        {
            movie.Genres = genres;
            return this;
        }
        public MovieBuilder SetStartYear(int startYear)
        {
            movie.StartYear = startYear;
            return this;
        }
        public MovieBuilder SetDesc(string desc)
        {
            movie.Description = desc;
            return this;
        }
        public MovieBuilder SetContentRating(string conrat)
        {
            movie.ContentRating = conrat;
            return this;
        }
        public MovieBuilder SetRuntime(int minutes)
        {
            movie.RuntimeMinutes = minutes;
            return this;
        }
        public MovieBuilder SetBudget(string budget)
        {
            movie.Budget = budget;
            return this;
        }

        public MovieModel Build()
        {
            return movie;
        }
    }
}