using System.Windows;
using System.Windows.Input;

namespace MovieApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Az ablak mozgatása
    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }


    // Ablak bezárása
    private void CloseApp(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    // Ablak minimalizálása
    private void MinimizeApp(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    // Ablak maximalizálása
    private void MaximizeApp(object sender, RoutedEventArgs e)
    {
        if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
        else
        {
            this.WindowState = WindowState.Maximized;
        }
    }

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {

    }
}

}
