using Microsoft.AspNetCore.Mvc;
using VideoStore.Application.Models;
using VideoStore.Application.Repositories;

namespace VideoStore.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly IRentalRepository _rentalRepository;

    public RentalsController(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
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