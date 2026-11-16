
using System.Net.Http.Json;
using MauiAndroidTest.Models;

namespace MauiAndroidTest.Services;

public class ReqResApiService
{
    private const string PublicKey = "pub_0fbc6629172b42ad51dfcc7f8b1f27fc13d938007ec3780a4d6d6cf8dbc34e6b";
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("https://reqres.in/")
    };

    private const string MoviesEndpoint =
        "api/collections/movies/records";

    // GET
    public async Task<List<MovieRecord>> GetMoviesAsync()
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, MoviesEndpoint);

        request.Headers.Add("x-api-key", PublicKey);
        request.Headers.Add("X-Reqres-Env", "prod");

        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<MovieListResponse>();

        return result?.Data ?? new List<MovieRecord>();
    }

    // POST
    public async Task<MovieRecord> AddMovieAsync(
        Movie movie, string manageKey)
    {
        var body = new
        {
            data = movie
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post, MoviesEndpoint);

        request.Headers.Add("x-api-key", manageKey);
        request.Headers.Add("X-Reqres-Env", "prod");
        request.Content = JsonContent.Create(body);

        using var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content
            .ReadFromJsonAsync<MovieRecordResponse>();

        return result?.Data
            ?? throw new Exception("Empty server response");
    }
}
