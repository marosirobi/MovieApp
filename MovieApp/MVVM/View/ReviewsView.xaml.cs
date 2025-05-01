using System;
using System.Windows;
using System.Windows.Controls;

namespace MovieApp.MVVM.View
{
    public partial class ReviewsView : UserControl
    {
        public ReviewsView()
        {
            InitializeComponent();
        }
        private void WriteNewReview_Click(object sender, RoutedEventArgs e)
        {
            // Show the NewReviewPanel when the button is clicked
            NewReviewPanel.Visibility = Visibility.Visible;
        }

        private void SaveReview_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string review = ReviewTextBox.Text;

            // Ellenőrizzük, hogy a ComboBox-ban kiválasztott érték érvényes-e
            if (RatingComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int rating = Convert.ToInt32(selectedItem.Content); // A kiválasztott értéket int-té konvertáljuk

                // Kiírhatjuk az adatokat
                Console.WriteLine("Title: " + title);
                Console.WriteLine("Review: " + review);
                Console.WriteLine("Rating: " + rating);

                // További feldolgozás, pl. adatbázisba mentés vagy lista frissítése...
                MessageBox.Show("Review saved successfully!");
            }
            else
            {
                // Ha nincs kiválasztott rating, hibaüzenetet küldhetünk
                MessageBox.Show("Please select a valid rating.");
            }
        }
    }
}