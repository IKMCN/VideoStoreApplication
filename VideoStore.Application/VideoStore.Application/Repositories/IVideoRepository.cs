using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories
{
    public interface IVideoRepository
    {
        void AddVideo(Video video);
        bool DeleteVideo(Guid id);
        List<Video> GetAllVideos();
        Video GetVideo(Guid id);
        bool UpdateVideo(Guid id, Video updatedVideo);
    }
}