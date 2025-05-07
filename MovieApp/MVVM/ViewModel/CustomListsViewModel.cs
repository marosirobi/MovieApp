using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MovieApp.MVVM.ViewModel
{
    public partial class CustomListsViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private MovieModel _currentMovie;

        [ObservableProperty]
        private ObservableCollection<Watchlist> _customLists = new();

        [ObservableProperty]
        private string _newListName;

        public User CurrentUser { get; set; }

        public CustomListsViewModel()
        {
            _dbService = new DatabaseService();
            _customLists = new ObservableCollection<Watchlist>();
        }

        public void LoadCustomLists()
        {
            if (CurrentUser == null || CurrentMovie == null) return;

            CustomLists.Clear();

            var lists = _dbService.GetUserCustomLists(CurrentUser.user_id);

            foreach (var list in lists)
            {
                list.IsInCurrentList = _dbService.IsInCustomList(CurrentUser.user_id, CurrentMovie.Id, list.list_name);
                CustomLists.Add(list);
            }
        }

        [RelayCommand]
        private void CreateNewList()
        {
            if (CurrentUser == null || string.IsNullOrWhiteSpace(NewListName)) return;

            try
            {
                if (NewListName.Length > 255)
                {
                    Debug.WriteLine("List name cannot exceed 255 characters");
                    // Show error to user (implement your preferred notification method)
                    return;
                }
                _dbService.CreateCustomList(CurrentUser.user_id, NewListName);
                LoadCustomLists(); // Refresh the list
                NewListName = string.Empty; // Clear the input field
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating list: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleToCustomList(Watchlist list)
        {
            if (CurrentUser == null || CurrentMovie == null || list == null) return;

            try
            {
                bool isInList = list.IsInCurrentList;

                if (isInList)
                {
                    _dbService.RemoveFromCustomList(CurrentUser.user_id, CurrentMovie.Id, list.list_name);
                    Debug.WriteLine($"Removed {CurrentMovie.PrimaryTitle} from list {list.list_name}");
                }
                else
                {
                    _dbService.AddToWatchlist(CurrentUser.user_id, CurrentMovie.Id, list.list_name);
                    Debug.WriteLine($"Added {CurrentMovie.PrimaryTitle} to list {list.list_name}");
                }
                LoadCustomLists();
                
                list.IsInCurrentList = !isInList;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error toggling list: {ex.Message}");
            }
        }

    }
}