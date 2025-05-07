using CommunityToolkit.Mvvm.ComponentModel;
using MovieApp.MVVM.Model;

namespace MovieApp.MVVM.ViewModel
{
    public partial class SelectedMoviePageViewModel : ObservableObject
    {

        [ObservableProperty]
        private MovieModel _currentMovie;

        [ObservableProperty]
        private User? _currentUser;

        public void SetMovie(MovieModel movie)
        {
            CurrentMovie = movie;
        }
        public void SetCurrentUser(User? user)
        {
            CurrentUser = user;
        }
    }
}
