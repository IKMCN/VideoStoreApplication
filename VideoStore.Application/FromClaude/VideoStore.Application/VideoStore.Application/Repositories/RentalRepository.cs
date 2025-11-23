
using Dapper;
using Npgsql;
using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories;

public class RentalRepository : IRentalRepository
{
    private readonly string _connectionString;

    public RentalRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void RentVideo(Rental rental)
    {

        rental.RentedAt = DateTime.UtcNow;
        rental.DueDate = DateTime.UtcNow.AddDays(7); // 7 day rental period

        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            INSERT INTO rentals (id, customer_id, video_id, rented_at, due_date, returned_at, late_fee, payment_transaction_id, payment_status, rental_amount)
            VALUES (@Id, @CustomerId, @VideoId, @RentedAt, @DueDate, @ReturnedAt, @LateFee, @PaymentTransactionId, @PaymentStatus, @RentalAmount)";

        connection.Execute(sql, new
        {
            rental.Id,
            rental.CustomerId,
            rental.VideoId,
            rental.RentedAt,
            rental.DueDate,
            rental.ReturnedAt,
            rental.LateFee,
            rental.PaymentTransactionId,
            PaymentStatus = rental.PaymentStatus.ToString(),
            rental.RentalAmount
        });
    }

    public List<Rental> GetAllRentals()
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, customer_id, video_id, rented_at, due_date, returned_at, late_fee, payment_transaction_id, payment_status, rental_amount FROM rentals";

        return connection.Query<Rental>(sql).ToList();
    }

    public Rental GetRental(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, customer_id, video_id, rented_at, due_date, returned_at, late_fee, payment_transaction_id, payment_status, rental_amount FROM rentals WHERE id = @Id";

        return connection.QuerySingleOrDefault<Rental>(sql, new { Id = id });
    }

    public List<Rental> GetActiveRentalsForCustomer(Guid customerId)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            SELECT id, customer_id, video_id, rented_at, due_date, returned_at, late_fee, payment_transaction_id, payment_status, rental_amount 
            FROM rentals 
            WHERE customer_id = @CustomerId 
            AND returned_at IS NULL";

        return connection.Query<Rental>(sql, new { CustomerId = customerId }).ToList();
    }

    public Rental GetActiveRentalForVideo(Guid videoId)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            SELECT id, customer_id, video_id, rented_at, due_date, returned_at, late_fee, payment_transaction_id, payment_status, rental_amount 
            FROM rentals 
            WHERE video_id = @VideoId 
            AND returned_at IS NULL";

        return connection.QuerySingleOrDefault<Rental>(sql, new { VideoId = videoId });
    }

    public bool ReturnVideo(Guid rentalId, decimal lateFee)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            UPDATE rentals
            SET returned_at = @ReturnedAt, late_fee = @LateFee
            WHERE id = @Id
            AND returned_at IS NULL";

        var rowsAffected = connection.Execute(sql, new
        {
            Id = rentalId,
            ReturnedAt = DateTime.UtcNow,
            LateFee = lateFee
        });

        return rowsAffected > 0;
    }

    public bool DeleteRental(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "DELETE FROM rentals WHERE id = @Id";

        var rowsAffected = connection.Execute(sql, new { Id = id });

        return rowsAffected > 0;
    }
    
    // New method for confirming payment
    public bool ConfirmPayment(Guid rentalId, string transactionId)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            UPDATE rentals
            SET payment_transaction_id = @TransactionId, 
                payment_status = 'Paid'
            WHERE id = @Id
            AND payment_status = 'Pending'";

        var rowsAffected = connection.Execute(sql, new
        {
            Id = rentalId,
            TransactionId = transactionId
        });

        return rowsAffected > 0;
    }
}
