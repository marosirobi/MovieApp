using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace MovieApp.MVVM.View
{
    public partial class ReviewsView : UserControl
    {
        public ObservableCollection<FilmReview> Reviews { get; set; } = new ObservableCollection<FilmReview>();

        public ReviewsView()
        {
            InitializeComponent();
            ReviewsListBox.ItemsSource = Reviews;
            RatingSlider.ValueChanged += RatingSlider_ValueChanged;
            Loaded += ReviewsView_Loaded;
        }

        private async void ReviewsView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-rapidapi-key", "9ef2b7c24bmsh3e07f666b690bfep1f80eajsne4294a75043c");
                client.DefaultRequestHeaders.Add("x-rapidapi-host", "imdb236.p.rapidapi.com");

                var response = await client.GetAsync("https://imdb236.p.rapidapi.com/imdb/top250-movies");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);

                var movies = doc.RootElement.GetProperty("data").EnumerateArray();
                foreach (var movie in movies)
                {
                    var title = movie.GetProperty("title").GetString();
                    FilmComboBox.Items.Add(title);
                }

                if (FilmComboBox.Items.Count > 0)
                    FilmComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba a filmek betöltésekor: " + ex.Message);
            }
        }

        private void RatingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RatingValueText.Text = RatingSlider.Value.ToString("0");
        }

        private void SaveReview_Click(object sender, RoutedEventArgs e)
        {
            if (FilmComboBox.SelectedItem is string title)
            {
                int rating = (int)RatingSlider.Value;
                Reviews.Add(new FilmReview { Title = title, Rating = rating });
            }
            else
            {
                MessageBox.Show("Válassz egy filmet!");
            }
        }
    }

    public class FilmReview
    {
        public string Title { get; set; }
        public int Rating { get; set; }
    }
}
