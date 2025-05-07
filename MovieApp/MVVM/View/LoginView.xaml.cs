using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MovieApp.MVVM.ViewModel;

namespace MovieApp.MVVM.View
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            var viewModel = new LoginViewModel();
            viewModel.OnLoginSuccess = (user) =>
            {
                var mainWindow = new MainWindow();
                mainWindow.DataContext = new MainViewModel { CurrentUser = user };
                mainWindow.Show();

                this.Close();
            };


            DataContext = viewModel;
        }
        public void ClearPassword()
        {
            PasswordBox.Clear();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}