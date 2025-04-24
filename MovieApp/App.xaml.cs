using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MovieApp.MVVM.View;
using MovieApp.MVVM.ViewModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace MovieApp;
public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();

        #if DEBUG
                PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;
        #endif
    }
    public static IConfiguration? Configuration { get; private set; }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var basePath = AppContext.BaseDirectory;

        Configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Only show login window - nothing else
        new LoginView().ShowDialog();
    }
}

