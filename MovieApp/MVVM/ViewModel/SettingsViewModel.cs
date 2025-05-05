using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.MVVM.ViewModel
{
    // Az egyes téma opciókat reprezentáló osztály
    public class ThemeOption : INotifyPropertyChanged
    {
        public string Code { get; set; }

        private string displayName;
        public string Name
        {
            get => displayName;
            set
            {
                if (displayName != value)
                {
                    displayName = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        private static string savedLanguage = "English";
        private static bool savedNotificationsEnabled = false;
        private static string savedTheme = "Dark";

        private string selectedLanguage;
        private bool notificationsEnabled;
        private string selectedTheme;

        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                    UpdateLanguage();
                }
            }
        }

        public bool NotificationsEnabled
        {
            get => notificationsEnabled;
            set
            {
                if (notificationsEnabled != value)
                {
                    notificationsEnabled = value;
                    OnPropertyChanged(nameof(NotificationsEnabled));
                }
            }
        }

        // SelectedTheme tartalmazza a téma kódját ("Dark", "Light", "Blue")
        public string SelectedTheme
        {
            get => selectedTheme;
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    OnPropertyChanged(nameof(SelectedTheme));
                    OnPropertyChanged(nameof(ThemeColor));
                    OnPropertyChanged(nameof(ForegroundColor));
                }
            }
        }

        // Inicializáljuk a ThemeOptions kollekciót inline, hogy soha ne legyen null.
        public ObservableCollection<ThemeOption> ThemeOptions { get; set; } =
            new ObservableCollection<ThemeOption>();

        // Háttérszín a témák alapján
        public Brush ThemeColor
        {
            get
            {
                return SelectedTheme switch
                {
                    "Light" => new SolidColorBrush(Colors.White),
                    "Blue" => new SolidColorBrush(Color.FromRgb(0, 122, 204)),
                    _ => new SolidColorBrush(Color.FromRgb(39, 37, 55)), // Alapértelmezett: Dark
                };
            }
        }

        // Kontrasztos szöveg szín
        public Brush ForegroundColor
        {
            get
            {
                return SelectedTheme switch
                {
                    "Light" => new SolidColorBrush(Colors.Black),
                    "Blue" => new SolidColorBrush(Colors.White),
                    _ => new SolidColorBrush(Colors.White),
                };
            }
        }

        // Csak a "Téma:" felirat, nyelvfüggő
        public string ThemeLabel { get; set; }

        // Egyéb, érintetlen property-k
        public string SettingsTitle { get; set; }
        public string LanguageLabel { get; set; }
        public string NotificationsLabel { get; set; }
        public string SaveButtonText { get; set; }
        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            // Először feltöltjük a ThemeOptions kollekciót alapértelmezett adatokal.
            ThemeOptions.Add(new ThemeOption { Code = "Dark", Name = "Dark" });
            ThemeOptions.Add(new ThemeOption { Code = "Light", Name = "Light" });
            ThemeOptions.Add(new ThemeOption { Code = "Blue", Name = "Blue" });

            // Majd beállítjuk a kezdőértékeket.
            SelectedLanguage = savedLanguage;
            NotificationsEnabled = savedNotificationsEnabled;
            SelectedTheme = savedTheme;

            SaveSettingsCommand = new RelayCommand(SaveSettings);
            UpdateLanguage();
        }

        // Frissíti a nyelv függvényében a feliratokat és a téma opciók neveit
        private void UpdateLanguage()
        {
            if (SelectedLanguage == "Magyar")
            {
                SettingsTitle = "Beállítások";
                LanguageLabel = "Nyelv";
                NotificationsLabel = "Értesítések";
                SaveButtonText = "Mentés";
                ThemeLabel = "Téma:";

                // Frissítjük a témák megjelenített neveit magyarul.
                foreach (var option in ThemeOptions)
                {
                    switch (option.Code)
                    {
                        case "Dark":
                            option.Name = "Sötét";
                            break;
                        case "Light":
                            option.Name = "Világos";
                            break;
                        case "Blue":
                            option.Name = "Kék";
                            break;
                    }
                }
            }
            else
            {
                SettingsTitle = "Settings";
                LanguageLabel = "Language";
                NotificationsLabel = "Notifications";
                SaveButtonText = "Save Settings";
                ThemeLabel = "Theme:";

                // Frissítjük a témák megjelenített neveit angolul.
                foreach (var option in ThemeOptions)
                {
                    switch (option.Code)
                    {
                        case "Dark":
                            option.Name = "Dark";
                            break;
                        case "Light":
                            option.Name = "Light";
                            break;
                        case "Blue":
                            option.Name = "Blue";
                            break;
                    }
                }
            }

            OnPropertyChanged(nameof(SettingsTitle));
            OnPropertyChanged(nameof(LanguageLabel));
            OnPropertyChanged(nameof(NotificationsLabel));
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(ThemeLabel));
        }

        private void SaveSettings()
        {
            savedLanguage = SelectedLanguage;
            savedNotificationsEnabled = NotificationsEnabled;
            savedTheme = SelectedTheme;

            MessageBox.Show($"{SaveButtonText}:\n" +
                            $"{LanguageLabel}: {SelectedLanguage}\n" +
                            $"{NotificationsLabel}: {NotificationsEnabled}\n" +
                            $"Theme: {SelectedTheme}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // A RelayCommand osztály változatlan marad
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => canExecute == null || canExecute();

        public void Execute(object parameter) => execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
