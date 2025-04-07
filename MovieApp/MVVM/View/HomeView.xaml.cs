using MovieApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MovieApp.MVVM.View
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMoviesPerPage();
        }

        private void MoviesGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMoviesPerPage();
        }

        private void UpdateMoviesPerPage()
        {
            if (DataContext is HomeViewModel vm)
            {
                const double movieCardWidth = 220; // 200 width + 20 margin
                vm.CalculateMoviesPerPage(MoviesGrid.ActualWidth, movieCardWidth);
            }
        }

        private void Swipe_Left(object sender, RoutedEventArgs e)
        {
            if (DataContext is HomeViewModel vm)
            {
                vm.PreviousPage();
            }
        }

        private void Swipe_Right(object sender, RoutedEventArgs e)
        {
            if (DataContext is HomeViewModel vm)
            {
                vm.NextPage();
            }
        }
    }
}