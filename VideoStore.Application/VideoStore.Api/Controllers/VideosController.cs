using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoStore.Application.Models;
using VideoStore.Application.Repositories;

namespace VideoStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {

        private readonly IVideoRepository _videoRepository;

        public VideosController(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        [HttpPost]
        public IActionResult CreateVideo(Video video)
        {
            _videoRepository.AddVideo(video);
            return Ok(video);
        }

        [HttpGet]
        public IActionResult GetAllVideos()
        {
            var output = _videoRepository.GetAllVideos();
            return Ok(output);

        }

        [HttpGet("{id}")]  // GET /api/videos/{id}
        public IActionResult GetVideo(Guid id)
        {
            var video = _videoRepository.GetVideo(id);

            if (video == null)
            {
                return NotFound();  // 404 if customer doesn't exist
            }

            return Ok(video);  // 200 with the customer
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVideo(Guid id)
        {
            var deleted = _videoRepository.DeleteVideo(id);

            if (!deleted)
            {
                return NotFound();  // 404 if customer didn't exist
            }

            return NoContent();  // 204 No Content (success, no body)
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVideo(Guid id, [FromBody] Video video)
        {
            var updated = _videoRepository.UpdateVideo(id, video);

            if (!updated)
            {
                return NotFound();  // 404 if video doesn't exist
            }

            return Ok(video);  // 200 OK with updated video
                                  // Or: return NoContent();  // 204 No Content (also valid)
        }
    }
}
