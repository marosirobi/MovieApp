using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovieApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

       /* public class MovieAppContext : DbContext -> ide kell majd az adatb neve
        {
           // public DbSet<User> Users { get; set; }
        }*/

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text.Trim();
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Kérlek töltsd ki mindkét mezőt!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new MovieAppContext())
            {
                bool userExists = context.Users.Any(u => u.Username == username);

                if (userExists)
                {
                    MessageBox.Show("Ez a felhasználónév már létezik!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    var newUser = new User { Username = username, Password = password };
                    context.Users.Add(newUser);
                    context.SaveChanges();
                    MessageBox.Show("Sikeres regisztráció!", "Register", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }



        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = UserTextBox.Text.Trim();
            string password = PasswordTextBox.Password;

            using (var context = new MovieAppContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    MessageBox.Show("Nincs ilyen felhasználó!", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (user.Password != password)
                {
                    MessageBox.Show("Hibás jelszó!", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Sikeres bejelentkezés!", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                    // főablak megnyitása ide
                }
            }
        }


    }
    }
