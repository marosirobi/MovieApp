using MovieApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace LoginViewModel.Views;

public partial class LoginView : Window
{
    private readonly AuthViewModel viewModel;

    public LoginView()
    {
        InitializeComponent();
        viewModel = new AuthViewModel();
        DataContext = viewModel;

        PasswordBox.PasswordChanged += (s, e) =>
        {
            viewModel.Password = PasswordBox.Password;
        };
    }
}
