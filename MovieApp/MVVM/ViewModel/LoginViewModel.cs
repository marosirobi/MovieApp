using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MovieApp.MVVM.ViewModel
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public AuthViewModel()
        {
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);

            using var db = new MovieAppDBContext();
            db.Database.EnsureCreated(); // létrehozza az adatbázist, ha nem létezik
        }

        private void Login()
        {
            using var db = new MovieAppDBContext();
            var user = db.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);
            MessageBox.Show(user != null ? "Sikeres bejelentkezés!" : "Hibás felhasználónév vagy jelszó.");
        }

        private void Register()
        {
            using var db = new MovieAppDBContext();
            if (db.Users.Any(u => u.Username == Username))
            {
                MessageBox.Show("Ez a felhasználónév már foglalt.");
                return;
            }

            db.Users.Add(new User { Username = Username, Password = Password });
            db.SaveChanges();
            MessageBox.Show("Sikeres regisztráció!");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}