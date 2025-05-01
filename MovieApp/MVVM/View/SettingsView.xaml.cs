using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _selectedTheme;
    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme != value)
            {
                _selectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));
                UpdateThemeImage();
            }
        }
    }

    private ImageBrush _selectedThemeImage;
    public ImageBrush SelectedThemeImage
    {
        get => _selectedThemeImage;
        set
        {
            _selectedThemeImage = value;
            OnPropertyChanged(nameof(SelectedThemeImage));
        }
    }


    public SettingsViewModel()
    {
        // Alapértelmezett érték
        SelectedTheme = "Theme/dark.jpg";
    }

    private void UpdateThemeImage()
    {
        string imagePath = SelectedTheme switch
        {
            "Pink" => "Theme/pink.jpg",
            "Dark" => "Theme/dark.jpg",
            "Light" => "Theme/light.jpg",
            _ => "Theme/dark.jpg"
        };

        try
        {
            var image = new BitmapImage(new Uri(imagePath, UriKind.Relative));
            SelectedThemeImage = new ImageBrush(image) { Stretch = Stretch.UniformToFill };
        }
        catch
        {
            // Hiba esetén fallback háttér
            SelectedThemeImage = new ImageBrush(new BitmapImage(new Uri("Theme/dark.jpg", UriKind.Relative)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
