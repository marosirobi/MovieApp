using MovieApp.MVVM.View;
using MovieApp.MVVM.ViewModel;
using System.Windows;

namespace MovieApp;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Only show login window - nothing else
        new LoginView().ShowDialog();
    }
}

