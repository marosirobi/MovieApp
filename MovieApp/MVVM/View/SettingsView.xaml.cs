using System;
using System.Windows;
using System.Windows.Controls;
using MovieApp.MVVM.ViewModel;

namespace MovieApp.MVVM.View
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            var vm = new SettingsViewModel();
            this.DataContext = vm;
        }
    }
}
