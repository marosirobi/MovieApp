using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.ViewModel;
using MovieApp.Utils;
using System.Diagnostics;
using System.Windows;

namespace MovieApp.MVVM.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly DatabaseService _dbService;

        [ObservableProperty]
        private string _username = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        

        // Use an action to handle successful login
        public Action<User> OnLoginSuccess { get; set; }

        public LoginViewModel()
        {
            _dbService = new DatabaseService();
        }

        [RelayCommand]
        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please fill in both fields!";
                return;
            }

            var user = _dbService.GetUser(Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(Password, user.passwd))
            {
                ErrorMessage = "Invalid username or password!";
                return;
            }

            ErrorMessage = string.Empty;
            OnLoginSuccess?.Invoke(user);
            CloseWindow();
        }

        [RelayCommand]
        private void Register()
        {
            Debug.WriteLine($"Register command triggered. Username: {Username}, Password: {Password}");

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required!";
                Debug.WriteLine("Validation failed - empty fields");
                return;
            }

            try
            {
                Debug.WriteLine("Attempting to register user...");

                if (_dbService.GetUser(Username) != null)
                {
                    ErrorMessage = "Username already exists!";
                    Debug.WriteLine("Registration failed - username exists");
                    return;
                }

                var success = _dbService.RegisterUser(Username, Password);

                if (success)
                {
                    ErrorMessage = "Registration successful! You can now login.";
                    Debug.WriteLine("Registration successful");

                    // Clear fields
                    Username = string.Empty;
                    Password = string.Empty;

                    (Application.Current.Windows.OfType<LoginView>().FirstOrDefault())?.ClearPassword();

                }
                else
                {
                    ErrorMessage = "Registration failed!";
                    Debug.WriteLine("Registration failed - unknown reason");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during registration.";
                Debug.WriteLine($"Registration error: {ex.Message}");
            }
        }

        // Add this event to notify the view to clear the password box

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginView)
                {
                    window.Close();
                    break;
                }
            }
        }
        [RelayCommand]
        private void CloseApp()
        {
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive)?
                .Close();
        }
    }
}