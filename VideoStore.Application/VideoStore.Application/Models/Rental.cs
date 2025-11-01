namespace VideoStore.Application.Models;

public class Rental
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CustomerId { get; set; }
    public Guid VideoId { get; set; }
    public DateTime RentedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }  // Nullable - null means not returned
    public decimal LateFee { get; set; }
}
