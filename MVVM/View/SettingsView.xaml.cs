using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MovieApp.MVVM.View
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel(); // ViewModel hozzárendelése
        }
    }

    public class SettingsViewModel : INotifyPropertyChanged
    {
        // „Állandó” tároló a szimulált mentéshez (itt memóriában marad)
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

        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel()
        {
            // Betöltjük az elmentett értékeket induláskor
            SelectedLanguage = savedLanguage;
            NotificationsEnabled = savedNotificationsEnabled;
            IsDarkMode = savedIsDarkMode;

            SaveSettingsCommand = new RelayCommand(SaveSettings);
        }

        private void SaveSettings()
        {
            // Mentés: eltároljuk statikus változókba
            savedLanguage = SelectedLanguage;
            savedNotificationsEnabled = NotificationsEnabled;
            savedIsDarkMode = IsDarkMode;

            MessageBox.Show("Settings saved:\n" +
                            $"Language: {SelectedLanguage}\n" +
                            $"Notifications: {NotificationsEnabled}\n" +
                            $"Dark Mode: {IsDarkMode}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
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