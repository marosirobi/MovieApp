using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;

namespace MovieApp.MVVM.ViewModel
{
    public partial class TopMoviesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<MovieModel> _movies;

        public void SetMovies(ObservableCollection<MovieModel> movies)
        {
            Movies = movies;
        }
    }
}