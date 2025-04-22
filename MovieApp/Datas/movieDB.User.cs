namespace MovieApp
{
    public partial class User {

        public User()
        {            
            //init an empty "Reviews" list and a empty "Watchlists" list (default watchlist)
            this.Watchlists = new List<Watchlist>();
            this.Reviews = new List<Review>();
            OnCreated();
        }

        public int user_id { get; set; }
        public string username { get; set; }
        public string passwd { get; set; }

        public ICollection<Watchlist> Watchlists { get; set; }
        public ICollection<Review> Reviews { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
