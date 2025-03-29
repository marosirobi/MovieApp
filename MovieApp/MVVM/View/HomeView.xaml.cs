using MovieApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MovieApp.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private readonly HomeViewModel _viewModel = new HomeViewModel();
        public HomeView()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadRandom20Movies();
        }
    }
}
