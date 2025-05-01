using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MovieApp.MVVM.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string _settingsTitle = "Beállítások";
        private string _languageLabel = "Nyelv";
        private string _selectedLanguage = "English";
        private string _notificationsLabel = "Értesítések";
        private bool _notificationsEnabled = true;
        private string _saveButtonText = "Mentés";

        public string SettingsTitle
        {
            get => _settingsTitle;
            set => SetProperty(ref _settingsTitle, value);
        }

        public string LanguageLabel
        {
            get => _languageLabel;
            set => SetProperty(ref _languageLabel, value);
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string NotificationsLabel
        {
            get => _notificationsLabel;
            set => SetProperty(ref _notificationsLabel, value);
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => SetProperty(ref _notificationsEnabled, value);
        }

        public string SaveButtonText
        {
            get => _saveButtonText;
            set => SetProperty(ref _saveButtonText, value);
        }

        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        // Három téma kezelése
        private bool _isDarkMode = true;
        private bool _isPinkMode = false;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set => SetProperty(ref _isDarkMode, value);
        }

        public bool IsPinkMode
        {
            get => _isPinkMode;
            set => SetProperty(ref _isPinkMode, value);
        }

        private void SaveSettings()
        {
            // Frissíti a témát a kiválasztott beállítás alapján
            UpdateThemeProperties();

            // Ide jöhet mentési logika, pl. fájlba vagy adatbázisba mentés
            System.Diagnostics.Debug.WriteLine("Beállítások mentve.");
        }

        private void UpdateThemeProperties()
        {
            string imagePath = "Theme/light.jpg"; // Alapértelmezett világos téma.

            if (IsDarkMode)
            {
                imagePath = "Theme/dark.jpg"; // Ha Dark Mode van, akkor dark.jpg
            }
            else if (IsPinkMode) // Ha Pink Mode van, akkor pink.jpg
            {
                imagePath = "Theme/pink.jpg";
            }

            // Convert the URI to an ImageSource using BitmapImage
            var imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new System.Uri(imagePath, System.UriKind.Relative))
            };

            // Alapértelmezett háttérbeállítások a három téma alapján
            Application.Current.Resources["PrimaryBackgroundBrush"] = imageBrush;

            // Betűszínek beállítása a téma alapján
            if (IsDarkMode)
            {
                Application.Current.Resources["PrimaryTextBrush"] = new SolidColorBrush(Colors.White); // Fehér betűk sötét módban
                Application.Current.Resources["HeaderTextBrush"] = new SolidColorBrush(Colors.White);
            }
            else if (IsPinkMode)
            {
                Application.Current.Resources["PrimaryTextBrush"] = new SolidColorBrush(Colors.Black); // Fekete betűk pink módban
                Application.Current.Resources["HeaderTextBrush"] = new SolidColorBrush(Colors.Black);
            }
            else
            {
                Application.Current.Resources["PrimaryTextBrush"] = new SolidColorBrush(Colors.DarkSlateGray); // Sötét betűk világos módban
                Application.Current.Resources["HeaderTextBrush"] = new SolidColorBrush(Colors.DarkSlateGray);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
