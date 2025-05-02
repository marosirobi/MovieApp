using MovieApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace MovieApp.MVVM.View
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            // Beállítjuk a DataContext-et a ViewModel-re
            this.DataContext = new SettingsViewModel();
        }
    }
}
