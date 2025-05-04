using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace MovieApp.MVVM.ViewModel;
public partial class TopMoviesViewModel : ObservableObject
{
    private readonly ObservableCollection<MovieModel> _movies = new();
    private List<MovieModel> _allMovies = new();

    public ICollectionView Movies { get; }

    public TopMoviesViewModel()
    {
        Movies = CollectionViewSource.GetDefaultView(_movies);
        Movies.Filter = FilterMovies; // Optional filtering
    }

    public async Task SetMoviesAsync(IEnumerable<MovieModel> movies)
    {
        _allMovies = movies.ToList();
        _movies.Clear();

        await Task.Run(async () =>
        {
            foreach (var movie in _allMovies)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _movies.Add(movie);
                }, System.Windows.Threading.DispatcherPriority.Background);

                await Task.Delay(300); // Yield to UI thread
            }
        });
    }

    private bool FilterMovies(object item)
    {
        if (item is MovieModel movie)
        {
            // Add your filter logic here
            return true;
        }
        return false;
    }
}