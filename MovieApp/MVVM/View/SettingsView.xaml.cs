using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MovieApp.MVVM.View
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            var vm = new SettingsViewModel();
            vm.PropertyChanged += ViewModel_PropertyChanged;
            this.DataContext = vm;

            UpdateBackground(vm.IsDarkMode); // Kezdeti háttérbeállítás
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.IsDarkMode))
            {
                var vm = DataContext as SettingsViewModel;
                UpdateBackground(vm.IsDarkMode);
            }
        }

        private void UpdateBackground(bool isDarkMode)
        {
            if (isDarkMode)
                MainGrid.Background = new SolidColorBrush(Color.FromRgb(39, 37, 55)); // Sötét háttér
            else   
                MainGrid.Background = new SolidColorBrush(Colors.Green); // Világos háttér*/
        }
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        private static string savedLanguage = "English";
        private static bool savedNotificationsEnabled = false;
        private static bool savedIsDarkMode = false;

        private string selectedLanguage;
        private bool notificationsEnabled;
        private bool isDarkMode;

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

        public bool IsDarkMode
        {
            get => isDarkMode;
            set
            {
                if (isDarkMode != value)
                {
                    isDarkMode = value;
                    OnPropertyChanged(nameof(IsDarkMode));
                }
            }
        }

        public string SettingsTitle { get; set; }
        public string LanguageLabel { get; set; }
        public string NotificationsLabel { get; set; }
        public string DarkModeLabel { get; set; }
        public string SaveButtonText { get; set; }

        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            SelectedLanguage = savedLanguage;
            NotificationsEnabled = savedNotificationsEnabled;
            IsDarkMode = savedIsDarkMode;

            SaveSettingsCommand = new RelayCommand(SaveSettings);
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            if (SelectedLanguage == "Magyar")
            {
                SettingsTitle = "Beállítások";
                LanguageLabel = "Nyelv";
                NotificationsLabel = "Értesítések";
                DarkModeLabel = "Sötét mód";
                SaveButtonText = "Mentés";
            }
            else
            {
                SettingsTitle = "Settings";
                LanguageLabel = "Language";
                NotificationsLabel = "Notifications";
                DarkModeLabel = "Dark Mode";
                SaveButtonText = "Save Settings";
            }

            OnPropertyChanged(nameof(SettingsTitle));
            OnPropertyChanged(nameof(LanguageLabel));
            OnPropertyChanged(nameof(NotificationsLabel));
            OnPropertyChanged(nameof(DarkModeLabel));
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private void SaveSettings()
        {
            savedLanguage = SelectedLanguage;
            savedNotificationsEnabled = NotificationsEnabled;
            savedIsDarkMode = IsDarkMode;

            MessageBox.Show($"{SaveButtonText}:\n" +
                            $"{LanguageLabel}: {SelectedLanguage}\n" +
                            $"{NotificationsLabel}: {NotificationsEnabled}\n" +
                            $"{DarkModeLabel}: {IsDarkMode}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

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
