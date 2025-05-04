using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MovieApp.MVVM.View
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private string selectedLanguage;
        private bool notificationsEnabled;
        private AppTheme selectedTheme;

        // Removed static saved settings to make this instance-specific
        public List<AppTheme> AvailableThemes { get; } = Enum.GetValues(typeof(AppTheme)).Cast<AppTheme>().ToList();

        // Property for the language selection
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

        // Property for notifications enabled/disabled
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

        // Property for the theme selection
        public AppTheme SelectedTheme
        {
            get => selectedTheme;
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    OnPropertyChanged(nameof(SelectedTheme));
                    UpdateTheme();
                }
            }
        }

        // Labels for UI elements
        public string SettingsTitle { get; set; }
        public string LanguageLabel { get; set; }
        public string NotificationsLabel { get; set; }
        public string ThemeLabel { get; set; }
        public string SaveButtonText { get; set; }

        // Command for saving settings
        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            // Default values for settings
            SelectedLanguage = "English";
            NotificationsEnabled = false;
            SelectedTheme = AppTheme.Dark;

            // Initialize the save command
            SaveSettingsCommand = new RelayCommand(SaveSettings);

            // Update language initially
            UpdateLanguage();
        }

        // Method to update language based on selection
        private void UpdateLanguage()
        {
            if (SelectedLanguage == "Magyar")
            {
                SettingsTitle = "Beállítások";
                LanguageLabel = "Nyelv";
                NotificationsLabel = "Értesítések";
                ThemeLabel = "Téma";
                SaveButtonText = "Mentés";
            }
            else
            {
                SettingsTitle = "Settings";
                LanguageLabel = "Language";
                NotificationsLabel = "Notifications";
                ThemeLabel = "Theme";
                SaveButtonText = "Save Settings";
            }

            // Notify UI about changes
            OnPropertyChanged(nameof(SettingsTitle));
            OnPropertyChanged(nameof(LanguageLabel));
            OnPropertyChanged(nameof(NotificationsLabel));
            OnPropertyChanged(nameof(ThemeLabel));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        // Method to update the theme dynamically
        private void UpdateTheme()
        {
            string themeKey = SelectedTheme.ToString();
            ResourceDictionary themeDictionary = Application.LoadComponent(new Uri($"Themes/{themeKey}.xaml", UriKind.Relative)) as ResourceDictionary;

            // Clear previous theme and add the new theme
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDictionary);
        }

        // Method to save the settings
        private void SaveSettings()
        {
            // For this example, we're just showing a message (replace with actual persistence)
            MessageBox.Show($"{SaveButtonText}:\n" +
                            $"{LanguageLabel}: {SelectedLanguage}\n" +
                            $"{NotificationsLabel}: {NotificationsEnabled}\n" +
                            $"{ThemeLabel}: {SelectedTheme}");
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Enum for app themes
    public enum AppTheme
    {
        LightGray,
        Dark,
        Pink
    }

    // RelayCommand implementation
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
