using MovieApp.MVVM.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MovieApp.MVVM.ViewModel
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();

            var vm = new SettingsViewModel();
            vm.PropertyChanged += ViewModel_PropertyChanged;
            this.DataContext = vm;

            if (vm.SelectedTheme != null) // Null ellenőrzés, hogy ne legyen hiba
            {
                UpdateBackground(vm.SelectedTheme);
            }
        }

        private void InitializeComponent()
        {
          
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.SelectedTheme))
            {
                var vm = DataContext as SettingsViewModel;
                if (vm != null)
                {
                    UpdateBackground(vm.SelectedTheme);
                }
            }
        }

        private void UpdateBackground(AppTheme selectedTheme)
        {
            Color background, menuSelected, menuHover, border;

            switch (selectedTheme)
            {
                case AppTheme.LightGray:
                    background = Color.FromRgb(230, 230, 230);
                    menuSelected = Color.FromRgb(200, 200, 200);
                    menuHover = Color.FromRgb(220, 220, 220);
                    border = Color.FromRgb(180, 180, 180);
                    break;
                case AppTheme.Pink:
                    background = Color.FromRgb(255, 192, 203);
                    menuSelected = Color.FromRgb(255, 182, 193);
                    menuHover = Color.FromRgb(255, 174, 185);
                    border = Color.FromRgb(255, 105, 180);
                    break;
                case AppTheme.Dark:
                default:
                    background = Color.FromRgb(39, 37, 55);
                    menuSelected = Color.FromRgb(34, 32, 47);
                    menuHover = Color.FromRgb(45, 43, 60);
                    border = Color.FromRgb(60, 60, 90);
                    break;
            }

            Application.Current.Resources["AppBackgroundBrush"] = new SolidColorBrush(background);
            Application.Current.Resources["MenuButtonSelectedBackground"] = new SolidColorBrush(menuSelected);
            Application.Current.Resources["MenuButtonHoverBackground"] = new SolidColorBrush(menuHover);
            Application.Current.Resources["AppBorderBrush"] = new SolidColorBrush(border);

            Background = new SolidColorBrush(background);
        }
    }
}
