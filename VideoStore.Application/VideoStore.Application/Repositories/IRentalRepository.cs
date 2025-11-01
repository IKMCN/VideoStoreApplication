using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories
{
    public interface IRentalRepository
    {
        bool DeleteRental(Guid id);
        Rental GetActiveRentalForVideo(Guid videoId);
        List<Rental> GetActiveRentalsForCustomer(Guid customerId);
        List<Rental> GetAllRentals();
        Rental GetRental(Guid id);
        void RentVideo(Rental rental);
        bool ReturnVideo(Guid rentalId, decimal lateFee);
    }
}