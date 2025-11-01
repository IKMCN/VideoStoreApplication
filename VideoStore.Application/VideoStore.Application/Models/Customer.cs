namespace VideoStore.Application.Models;


public class Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string Email { get; set; }
}
