﻿using System.Windows;
using System.Windows.Input;

namespace MovieApp;

public partial class MainWindow : Window
{   
    public MainWindow()
    {
        InitializeComponent();      
    }
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

}