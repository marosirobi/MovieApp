using System.Text.Json.Serialization;


namespace MovieApp.MVVM.Model
{
    public class MovieModel
    {
        [JsonPropertyName("primaryTitle")]
        public string? PrimaryTitle { get; set; }

        [JsonPropertyName("primaryImage")]
        public string? PrimaryImage { get; set; }

        [JsonPropertyName("averageRating")]
        public double AverageRating { get; set; }
    }       

    public class MovieBuilder
    {

        private readonly MovieModel movie = new MovieModel();
        public MovieBuilder setPrimaryTitle(string title)
        {
            movie.PrimaryTitle = title;
            return this;
        }
        public MovieBuilder setPrimaryImage(string imageUrl)
        {
            movie.PrimaryImage = imageUrl;
            return this;
        }
        public MovieBuilder setAverageRating(double rating)
        {
            movie.AverageRating = rating;
            return this;
        }

        public MovieModel Build()
        {
            return movie; 
        }

    }
} 