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
        Movies.SortDescriptions.Add(new SortDescription(nameof(MovieModel.AverageRating), ListSortDirection.Descending));
    }

    public async Task SetMoviesAsync(IEnumerable<MovieModel> movies)
    {
        _allMovies = movies
            .OrderByDescending(m => m.AverageRating)
            .ToList();

        _movies.Clear();

        await Task.Run(async () =>
        {
            foreach (var movie in _allMovies)
            {
                if (Application.Current != null)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        _movies.Add(movie);
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                await Task.Delay(300); // Reduced delay for better performance
            }
        });

        // Refresh the view after all movies are added
        Movies.Refresh();
    }

    [RelayCommand]
    private void RefreshMovies()
    {
        Movies.Refresh();
    }
}