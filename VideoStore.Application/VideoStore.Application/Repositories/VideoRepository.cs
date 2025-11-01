
using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly List<Video> _videos = new();

    public void AddVideo(Video video)
    {
        _videos.Add(video);
    }
    public List<Video> GetAllVideos()
    {
        return _videos;
    }

    public Video GetVideo(Guid id)
    {
        return _videos.SingleOrDefault(c => c.Id == id);
    }

    public bool DeleteVideo(Guid id)
    {
        var removedCount = _videos.RemoveAll(x => x.Id == id);
        return removedCount > 0;  // true if deleted, false if not found
    }

    public bool UpdateVideo(Guid id, Video updatedVideo)
    {
        var existingVideo = _videos.FirstOrDefault(c => c.Id == id);

        if (existingVideo == null)
        {
            return false;  // Customer not found
        }

        // Update the properties
        existingVideo.Title = updatedVideo.Title;
        existingVideo.YearOfRelease = updatedVideo.YearOfRelease;
        existingVideo.Genres = updatedVideo.Genres;

        return true;
    }
}
