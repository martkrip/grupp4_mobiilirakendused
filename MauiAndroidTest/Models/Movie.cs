
using System.Text.Json.Serialization;

namespace MauiAndroidTest.Models;

public class Movie
{
    public string Title { get; set; } = "";
    public string CoverImage { get; set; } = "";
    public string ReleaseDate { get; set; } = "";
    public bool Completed { get; set; }
}

public class MovieRecord
{
    public string Id { get; set; } = "";
    public Movie Data { get; set; } = new();
}

public class MovieListResponse
{
    public List<MovieRecord> Data { get; set; } = new();
}

public class MovieRecordResponse
{
    public MovieRecord Data { get; set; } = new();
}
