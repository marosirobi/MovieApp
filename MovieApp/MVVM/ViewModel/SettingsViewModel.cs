using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MovieApp.MVVM.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private const string LightThemeImagePath = "Theme/light.jpg";
        private const string DarkThemeImagePath = "Theme/dark.jpg";
        private const string PinkThemeImagePath = "Theme/pink.jpg";

        private const string BackgroundBrushKey = "PrimaryBackgroundBrush";
        private const string PrimaryTextBrushKey = "PrimaryTextBrush";
        private const string HeaderTextBrushKey = "HeaderTextBrush";

        private string _settingsTitle = "Beállítások";
        private string _languageLabel = "Nyelv";
        private string _selectedLanguage = "English";
        private string _notificationsLabel = "Értesítések";
        private bool _notificationsEnabled = true;
        private string _saveButtonText = "Mentés";
        private string _selectedTheme = "Dark";
        private ImageBrush _selectedThemeImage;

        // Új: Elérhető témák listája a ComboBox-hoz
        public List<string> AvailableThemes { get; } = new List<string> { "Dark", "Light", "Pink" };

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

        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetProperty(ref _selectedTheme, value))
                {
                    UpdateThemeProperties();
                }
            }
        }

        public ImageBrush SelectedThemeImage
        {
            get => _selectedThemeImage;
            set => SetProperty(ref _selectedThemeImage, value);
        }

        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            UpdateThemeProperties();
        }

        private void SaveSettings()
        {
            System.Diagnostics.Debug.WriteLine("Beállítások mentve.");
        }
        private Brush _borderBrush;
        public Brush BorderBrush
        {
            get => _borderBrush;
            set => SetProperty(ref _borderBrush, value);
        }

        private void UpdateThemeProperties()
        {
            string imagePath;
            Color textColor;


            switch (SelectedTheme)
            {
                case "Pink":
                    imagePath = PinkThemeImagePath;
                    textColor = (Color)ColorConverter.ConvertFromString("#FF1493");
                    BorderBrush = new SolidColorBrush(Colors.Purple);
                    break;
                case "Light":
                    imagePath = LightThemeImagePath;
                    textColor = Colors.DarkSlateGray;
                    BorderBrush = new SolidColorBrush(Colors.Black);
                    break;
                case "Dark":
                default:
                    imagePath = DarkThemeImagePath;
                    textColor = Colors.White;
                    BorderBrush = new SolidColorBrush(Colors.Black);
                    break;
            }

            var imgBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new System.Uri(imagePath, System.UriKind.Relative)),
                Stretch = Stretch.UniformToFill
            };

            SelectedThemeImage = imgBrush;

            Application.Current.Resources[BackgroundBrushKey] = imgBrush;
            var brush = new SolidColorBrush(textColor);
            Application.Current.Resources[PrimaryTextBrushKey] = brush;
            Application.Current.Resources[HeaderTextBrushKey] = brush;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
