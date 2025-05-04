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

            // Initial background update
            UpdateBackground(vm.SelectedTheme);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.SelectedTheme))
            {
                var vm = DataContext as SettingsViewModel;
                UpdateBackground(vm.SelectedTheme);  // Update the background when the theme changes
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

            // Update colors in the application resources
            Application.Current.Resources["AppBackgroundBrush"] = new SolidColorBrush(background);
            Application.Current.Resources["MenuButtonSelectedBackground"] = new SolidColorBrush(menuSelected);
            Application.Current.Resources["MenuButtonHoverBackground"] = new SolidColorBrush(menuHover);
            Application.Current.Resources["AppBorderBrush"] = new SolidColorBrush(border);

            // Local background update for the main grid
            MainGrid.Background = new SolidColorBrush(background);
        }
    }
}
