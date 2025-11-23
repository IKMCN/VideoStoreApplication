using Microsoft.AspNetCore.Mvc;
using VideoStore.Application.Models;
using VideoStore.Application.Repositories;
using VideoStore.Application.Services;

namespace VideoStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBankingApiService _bankingApiService;

    public RentalsController(IRentalRepository rentalRepository, IBankingApiService bankingApiService)
    {
        _rentalRepository = rentalRepository;
        _bankingApiService = bankingApiService;
    }

    // POST /api/rentals - Rent a video
    [HttpPost]
    public IActionResult RentVideo([FromBody] Rental rental)
    {
        // Check if video is already rented
        var existingRental = _rentalRepository.GetActiveRentalForVideo(rental.VideoId);
        if (existingRental != null)
        {
            return BadRequest(new { message = "Video is already rented out" });
        }

        _rentalRepository.RentVideo(rental);
        return Ok(rental);
    }

    // GET /api/rentals - Get all rentals
    [HttpGet]
    public IActionResult GetAllRentals()
    {
        var rentals = _rentalRepository.GetAllRentals();
        return Ok(rentals);
    }

    // GET /api/rentals/{id} - Get specific rental
    [HttpGet("{id}")]
    public IActionResult GetRental(Guid id)
    {
        var rental = _rentalRepository.GetRental(id);
        if (rental == null)
        {
            return NotFound();
        }
        return Ok(rental);
    }

    // GET /api/rentals/customer/{customerId} - Get customer's active rentals
    [HttpGet("customer/{customerId}")]
    public IActionResult GetCustomerRentals(Guid customerId)
    {
        var rentals = _rentalRepository.GetActiveRentalsForCustomer(customerId);
        return Ok(rentals);
    }

    // POST /api/rentals/{id}/return - Return a video
    [HttpPost("{id}/return")]
    public IActionResult ReturnVideo(Guid id, [FromBody] ReturnVideoRequest request)
    {
        var success = _rentalRepository.ReturnVideo(id, request.LateFee);
        if (!success)
        {
            return NotFound(new { message = "Rental not found or already returned" });
        }
        return Ok(new { message = "Video returned successfully" });
    }

    // POST /api/rentals/{id}/confirm-payment - Confirm payment for a rental
    [HttpPost("{id}/confirm-payment")]
    public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] PaymentConfirmationRequest request)
    {
        // Get the rental
        var rental = _rentalRepository.GetRental(id);
        if (rental == null)
        {
            return NotFound(new { message = "Rental not found" });
        }

        // Check if already paid
        if (rental.PaymentStatus == PaymentStatus.Paid)
        {
            return BadRequest(new { message = "Rental has already been paid" });
        }

        // Verify transaction with Banking API
        var transaction = await _bankingApiService.VerifyTransactionAsync(request.TransactionId);
        if (transaction == null)
        {
            return BadRequest(new { message = "Transaction not found in Banking system" });
        }

        // Optional: Verify transaction amount matches rental amount
        if (Math.Abs(transaction.Amount - rental.RentalAmount) > 0.01m)
        {
            return BadRequest(new 
            { 
                message = "Transaction amount does not match rental amount",
                expected = rental.RentalAmount,
                actual = transaction.Amount
            });
        }

        // Update rental with payment information
        var success = _rentalRepository.ConfirmPayment(id, request.TransactionId);
        if (!success)
        {
            return BadRequest(new { message = "Failed to confirm payment" });
        }

        return Ok(new 
        { 
            message = "Payment confirmed successfully",
            rentalId = id,
            transactionId = request.TransactionId
        });
    }

    // DELETE /api/rentals/{id} - Delete a rental
    [HttpDelete("{id}")]
    public IActionResult DeleteRental(Guid id)
    {
        var deleted = _rentalRepository.DeleteRental(id);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}

// Helper class for return endpoint
public class ReturnVideoRequest
{
    public decimal LateFee { get; set; }
}
