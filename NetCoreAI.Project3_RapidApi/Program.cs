using NetCoreAI.Project3_RapidApi.ViewModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;


var client = new HttpClient();
List<ApiSeriesViewModel> apiSeriesViewModels = new List<ApiSeriesViewModel>();
var request = new HttpRequestMessage // HttpRequestMessage ismindeki sınıfa istekte bulunur
{
    Method = HttpMethod.Get,
    RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/"),
    Headers =
    {
        { "x-rapidapi-key", "d3711deaacmsh0c41d39277ef1e2p100512jsn73ac568f3574" },
        { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
    },
};
using (var response = await client.SendAsync(request))
{
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();
    apiSeriesViewModels = JsonConvert.DeserializeObject<List<ApiSeriesViewModel>>(body);
    foreach(var series in apiSeriesViewModels)
    {
        Console.WriteLine(series.rank + "- " + series.title + " Film Puanı: " + series.rating + " Yapım Yılı: " + series.year);
    }
}

Console.ReadLine();