﻿using MovieApp.MVVM.View;
using System.Diagnostics;
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
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Only show login window - nothing else
        new LoginView().ShowDialog();
    }
}

