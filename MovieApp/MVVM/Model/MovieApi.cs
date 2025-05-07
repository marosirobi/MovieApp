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
                { "x-rapidapi-key", "5515f0fb43mshab66141f6ee656ap1d4862jsn30c2b230ff4f" },
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
                string content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
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
