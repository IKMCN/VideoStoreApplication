using Dapper;
using Npgsql;
using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly string _connectionString;

    public VideoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AddVideo(Video video)
    {

        using var connection = new NpgsqlConnection(_connectionString);

        // Convert List<string> to comma-separated string
        var genresString = string.Join(",", video.Genres);

        var sql = @"
            INSERT INTO videos (id, title, year_of_release, genres)
            VALUES (@Id, @Title, @YearOfRelease, @Genres)";

        connection.Execute(sql, new
        {
            video.Id,
            video.Title,
            video.YearOfRelease,
            Genres = genresString
        });
    }

    public List<Video> GetAllVideos()
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, title, year_of_release, genres FROM videos";

        var videos = connection.Query<VideoDb>(sql).ToList();

        // Convert back to domain model with List<string> genres
        return videos.Select(v => new Video
        {
            Id = v.Id,
            Title = v.Title,
            YearOfRelease = v.YearOfRelease,
            Genres = v.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>()
        }).ToList();
    }

    public Video GetVideo(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, title, year_of_release, genres FROM videos WHERE id = @Id";

        var videoDb = connection.QuerySingleOrDefault<VideoDb>(sql, new { Id = id });

        if (videoDb == null) return null;

        return new Video
        {
            Id = videoDb.Id,
            Title = videoDb.Title,
            YearOfRelease = videoDb.YearOfRelease,
            Genres = videoDb.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>()
        };
    }

    public bool DeleteVideo(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "DELETE FROM videos WHERE id = @Id";

        var rowsAffected = connection.Execute(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public bool UpdateVideo(Guid id, Video updatedVideo)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var genresString = string.Join(",", updatedVideo.Genres);

        var sql = @"
            UPDATE videos
            SET title = @Title, year_of_release = @YearOfRelease, genres = @Genres
            WHERE id = @Id";

        var rowsAffected = connection.Execute(sql, new
        {
            Id = id,
            updatedVideo.Title,
            updatedVideo.YearOfRelease,
            Genres = genresString
        });

        return rowsAffected > 0;
    }

    // Helper class for database mapping
    private class VideoDb
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int YearOfRelease { get; set; }
        public string Genres { get; set; }  // Comma-separated string
    }
}