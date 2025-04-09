using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;

namespace MovieApp.MVVM.Model
{
    class MovieApi
    {
        public static async Task<ObservableCollection<MovieModel>> GetMoviesFromApi()
        {
            var headers = new Dictionary<string, string>
            {
                { "x-rapidapi-key", "05663317ebmsh594a98da95da942p134351jsn27869b438227" },
                { "x-rapidapi-host", "imdb236.p.rapidapi.com" },
            };

            var api = new ApiHandler()
            {
                Url = "https://imdb236.p.rapidapi.com/imdb/top250-movies",
                Method = HttpMethod.Get,
                Headers = headers
            };

            var response = await api.SendRequest();

            if (response.IsSuccessStatusCode)
            {
                // For System.Text.Json:
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ObservableCollection<MovieModel>>(content);
            }
            else
            {
                // Handle API errors
                Debug.WriteLine($"API Error: {response.StatusCode}");
                return new ObservableCollection<MovieModel>();
            }
        }
    }
}
