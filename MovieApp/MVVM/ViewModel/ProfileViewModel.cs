using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MovieApp.MVVM.ViewModel
{
    public partial class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<string> Genres { get; set; }


        private string _selectedGenre;
        public string SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                if (_selectedGenre != value)
                {
                    _selectedGenre = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProfileViewModel()
        {
            Genres = new ObservableCollection<string>
            {
                "Action", "Comedy", "Drama", "Fantasy", "Horror", "Sci-Fi", "Romance", "Thriller"
            };
            SelectedGenre = "Sci-Fi";
        }
    }
}
