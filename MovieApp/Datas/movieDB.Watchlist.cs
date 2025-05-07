using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp
{
    public partial class Watchlist : ObservableObject 
    {
        [NotMapped]
        public bool IsInCurrentList { get; set; }

        public Watchlist()
        {
            create_date = DateTime.Now;
            //isDefault always true if its not a custom list
            this.isDefault = true;
            this.movie_count = 0;
            this.Movie_Watchlists1 = new List<Movie_Watchlist>();
            OnCreated();
        }

        public int watchlist_id { get; set; }

        
        [MaxLength(255)]
        public string list_name { get; set; }
        public DateTime create_date { get; set; }
        public int movie_count { get; set; }
        public bool isDefault { get; set; }
        public virtual User User { get; set; }
        public virtual IList<Movie_Watchlist> Movie_Watchlists1 { get; set; }

        #region Extensibility Method Definitions

        partial void OnCreated();

        #endregion
    }

}
