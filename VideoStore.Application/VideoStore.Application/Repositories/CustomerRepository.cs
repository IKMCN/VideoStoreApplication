using Dapper;
using Npgsql;
using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void AddCustomer(Customer customer)
    {
        
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            INSERT INTO customers (id, name, email)
            VALUES (@Id, @Name, @Email)";

        connection.Execute(sql, customer);
    }

    public List<Customer> GetAllCustomers()
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, name, email FROM customers";

        return connection.Query<Customer>(sql).ToList();
    }

    public Customer GetCustomer(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "SELECT id, name, email FROM customers WHERE id = @Id";

        return connection.QuerySingleOrDefault<Customer>(sql, new { Id = id });
    }

    public bool DeleteCustomer(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = "DELETE FROM customers WHERE id = @Id";

        var rowsAffected = connection.Execute(sql, new { Id = id });

        return rowsAffected > 0;
    }

    public bool UpdateCustomer(Guid id, Customer updatedCustomer)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        var sql = @"
            UPDATE customers
            SET name = @Name, email = @Email
            WHERE id = @Id";

        var rowsAffected = connection.Execute(sql, new
        {
            Id = id,
            updatedCustomer.Name,
            updatedCustomer.Email
        });

        return rowsAffected > 0;
    }
}