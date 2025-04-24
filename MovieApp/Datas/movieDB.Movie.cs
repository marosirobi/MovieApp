namespace MovieApp
{
    public partial class Movie {

        public Movie()
        {
            //a new movie has 0 reviews and 0.0 rating (on default)
            this.review_count = 0;
            this.avg_rating = 0;

            //init an empty "Reviews" list and a empty "Movie_watchlists" list 
            this.Reviews = new List<Review>();
            this.Movie_Watchlists = new List<Movie_Watchlist>();
            OnCreated();
        }

        public int movie_id { get; set; }

        public string? api_id { get; set; }
        public int review_count { get; set; }

        public decimal avg_rating { get; set; }

        public string? title { get; set; }

        public string[]? genre { get; set; }

        public int releaseYear { get; set; }

        public int? runTime { get; set; }

        public virtual IList<Review> Reviews { get; set; }

        public virtual IList<Movie_Watchlist> Movie_Watchlists { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
