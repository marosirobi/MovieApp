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

        [ObservableProperty]
        private MovieModel _currentMovie;

        [ObservableProperty]
        private int _rating;

        [ObservableProperty]
        private int _hoveredStar;

        public ObservableCollection<StarViewModel> Stars { get; } = new();

        private readonly DatabaseService _dbService;

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
            if (CurrentUser != null && CurrentMovie != null)
            {
                _dbService.AddReview(CurrentUser.user_id, CurrentMovie.Id, Rating);

                // Use the public method to update rating
                CurrentMovie.UpdateUserRating(Rating);

                CloseRatingDialogCommand?.Execute(null);
            }
        }

        [RelayCommand]
        public void SetMovie(MovieModel movie)
        {
            CurrentMovie = movie;
            if (CurrentUser != null && CurrentMovie != null)
            {
                var existingReview = _dbService.GetReview(CurrentUser.user_id, CurrentMovie.Id);
                Rating = existingReview?.stars ?? 0;
            }
        }
        public User CurrentUser { get; set; }
                public ICommand CloseRatingDialogCommand { get; set; }
            }
        }