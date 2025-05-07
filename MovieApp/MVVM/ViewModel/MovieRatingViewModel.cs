using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using MovieApp.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.MVVM.ViewModel
{
    public partial class MovieRatingViewModel : ObservableObject
    {
        public class StarViewModel : ObservableObject
        {
            private Brush _color = Brushes.Gray;
            public int Number { get; set; }
            public Brush Color
            {
                get => _color;
                set => SetProperty(ref _color, value);
            }
        }

        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private MovieModel _currentMovie;

        [ObservableProperty]
        private int _rating;

        [ObservableProperty]
        private int _hoveredStar;

        [ObservableProperty]
        private bool _hasRating;

        [ObservableProperty]
        private Decimal _averageRating;

        private bool _isInitialLoad = false;
        public ObservableCollection<StarViewModel> Stars { get; } = new();
        public User CurrentUser { get; set; }
        public ICommand CloseRatingDialogCommand { get; set; }

        public MovieRatingViewModel()
        {
            _dbService = new DatabaseService();
            InitializeStars();
        }

        private void InitializeStars()
        {
            for (int i = 1; i <= 10; i++)
            {
                Stars.Add(new StarViewModel { Number = i });
            }
        }

        partial void OnRatingChanged(int value)
        {
            UpdateStarColors();
        }

        partial void OnHoveredStarChanged(int value)
        {
            UpdateStarColors();
        }

        private void UpdateStarColors()
        {
            int highlightUntil = HoveredStar > 0 ? HoveredStar : Rating;

            foreach (var star in Stars)
            {
                star.Color = star.Number <= highlightUntil ? Brushes.Gold : Brushes.Gray;
            }
        }


        [RelayCommand]
        private void SetRating(int stars)
        {
            Rating = stars;
            HasRating = stars > 0;
            UpdateStarColors();
        }

        [RelayCommand]
        private void StarHover(int starNumber)
        {
            HoveredStar = starNumber;
        }

        [RelayCommand]
        private void StarLeave()
        {
            HoveredStar = 0;
        }

        [RelayCommand]
        private void SubmitRating()
        {
            if (CurrentUser != null && CurrentMovie != null && Rating > 0)
            {
                _dbService.AddReview(CurrentUser.user_id, CurrentMovie.Id, Rating);
                CurrentMovie.UpdateUserRating(Rating);
                CurrentMovie.UpdateAverageRating(_dbService.GetMovieRating(CurrentMovie.Id));
                HasRating = true;
                Debug.WriteLine($"Rating submitted: {Rating}");
            }
        }

        

        public void SetMovie(MovieModel movie)
        {
            if (CurrentMovie?.Id == movie?.Id && _isInitialLoad)
                return;

            CurrentMovie = movie;
            _isInitialLoad = true;

            if (CurrentUser != null && CurrentMovie != null)
            {
                var existingReview = _dbService.GetReview(CurrentUser.user_id, CurrentMovie.Id);
                Rating = existingReview?.stars ?? 0;
                HasRating = existingReview != null;
                
                UpdateStarColors();
                OnPropertyChanged(nameof(Rating));
                OnPropertyChanged(nameof(HasRating));
            }
        }

        [RelayCommand]
        private void DeleteRating()
        {
            if (CurrentUser != null && CurrentMovie != null)
            {
                _dbService.DeleteReview(CurrentUser.user_id, CurrentMovie.Id);
                Rating = 0;
                CurrentMovie.UpdateUserRating(null);
                HasRating = false;
                UpdateStarColors();
                Debug.WriteLine($"Rating deleted for {CurrentMovie.PrimaryTitle}");
            }
        }

        partial void OnCurrentMovieChanged(MovieModel value)
        {
            if (value != null && CurrentUser != null)
            {
                var existingReview = _dbService.GetReview(CurrentUser.user_id, value.Id);
                HasRating = existingReview != null;
            }
        }
    }
        }

