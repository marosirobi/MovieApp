using System.Windows;
using System.Windows.Controls;
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
                // Create and show the main window
                var mainWindow = new MainWindow();
                mainWindow.DataContext = new MainViewModel { CurrentUser = user };
                mainWindow.Show();

                // Close the login window
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
    }
}