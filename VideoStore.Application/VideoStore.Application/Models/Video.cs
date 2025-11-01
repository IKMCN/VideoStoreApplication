
namespace VideoStore.Application.Models;

public class Video
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; set; }
}
