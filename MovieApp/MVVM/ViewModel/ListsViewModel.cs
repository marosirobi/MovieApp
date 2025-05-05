using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MovieApp.MVVM.ViewModel
{
    public partial class ListsViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;
        private ObservableCollection<MovieModel> _allMovies;

        [ObservableProperty]
        private ObservableCollection<MovieModel> _listedMovies = new();

        [ObservableProperty]
        private ObservableCollection<string> _availableLists = new();

        [ObservableProperty]
        private User? _currentUser;

        [ObservableProperty]
        private string? _selectedList;

        public ListsViewModel()
        {
            _dbService = new DatabaseService();
            
        }

        public void Initialize(ObservableCollection<MovieModel> allMovies)
        {
            _allMovies = allMovies;
            LoadListedMovies();
        }

        public void SetCurrentUser(User? user)
        {
            CurrentUser = user;
            LoadAvailableLists();
        }

        partial void OnSelectedListChanged(string? value)
        {
            LoadListedMovies();
        }

        private void LoadAvailableLists()
        {
            if (CurrentUser == null) return;

            AvailableLists.Clear();
            var lists = _dbService.GetUserLists(CurrentUser.user_id);
            foreach (var list in lists)
            {
                if(list != "Watchlist")
                AvailableLists.Add(list);
            }

            // Default to watchlist if available
            SelectedList = AvailableLists.FirstOrDefault();
        }

        public void LoadListedMovies()
        {
            if (CurrentUser == null || _allMovies == null || string.IsNullOrEmpty(SelectedList)) return;

            try
            {
                ListedMovies.Clear();
                ListedMovies = _dbService.GetListMovies(CurrentUser.user_id, _allMovies, SelectedList);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading list: {ex.Message}");
            }
        }
    }
}