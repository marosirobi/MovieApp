using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MovieApp.MVVM.View
{
    public partial class ProfileView : UserControl
    {
        private readonly string dataFile = "profile.txt";
        private readonly string imageFile = "profile_image.png";

        public ProfileView()
        {
            InitializeComponent();

            GenreComboBox.ItemsSource = new List<string>
            {
                "Action", "Comedy", "Drama", "Horror", "Sci-Fi"
            };

            LoadProfileData(); // Betöltés fájlból
            LoadProfileImage(); // Profilkép betöltése
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            string name = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string genre = GenreComboBox.SelectedItem?.ToString() ?? "";

            File.WriteAllLines(dataFile, new[] { name, email, genre });

            MessageBox.Show("Profil elmentve!", "Mentés", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadProfileData()
        {
            if (File.Exists(dataFile))
            {
                var lines = File.ReadAllLines(dataFile);
                if (lines.Length >= 3)
                {
                    UsernameTextBox.Text = lines[0];
                    EmailTextBox.Text = lines[1];
                    GenreComboBox.SelectedItem = lines[2];
                }
            }
            else
            {
                // Alapértékek első betöltéskor
                UsernameTextBox.Text = "John Doe";
                EmailTextBox.Text = "john.doe@email.com";
                GenreComboBox.SelectedIndex = 0;
            }
        }

        private void ChangeProfileImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Képfájlok (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Válassz profilképet"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Másoljuk a kiválasztott képet az alkalmazás mappájába
                    File.Copy(openFileDialog.FileName, imageFile, overwrite: true);
                    LoadProfileImage();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt a kép betöltésekor: {ex.Message}",
                                  "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LoadProfileImage()
        {
            if (File.Exists(imageFile))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(Path.GetFullPath(imageFile));
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProfileImage.Source = bitmap;
                }
                catch
                {
                    SetDefaultProfileImage();
                }
            }
            else
            {
                SetDefaultProfileImage();
            }
        }

        private void SetDefaultProfileImage()
        {
            // Létrehozunk egy egyszerű, alapértelmezett profilkép grafikát
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                context.DrawEllipse(Brushes.LightGray, null, new Point(48, 48), 48, 48);
                context.DrawText(
                    new FormattedText("JD",
                                    System.Globalization.CultureInfo.CurrentCulture,
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial"),
                                    32,
                                    Brushes.White,
                                    1.0),
                    new Point(24, 24));
            }

            var image = new RenderTargetBitmap(96, 96, 96, 96, PixelFormats.Pbgra32);
            image.Render(visual);
            ProfileImage.Source = image;
        }
    }
}