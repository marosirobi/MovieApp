using System.Windows;
using System.Windows.Input;

namespace MovieApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Az ablak mozgatása
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        // Ablak bezárása
        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Ablak minimalizálása
        private void MinimizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Ablak maximalizálása / visszaállítása
        private void MaximizeApp(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        // Esemény placeholder – csak akkor kell, ha valóban fogsz itt logikát írni
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Itt lehet implementálni a logikát később
        }
    }
}
