using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public void AddCustomer(Customer customer)
    {
        _customers.Add(customer);
    }
    public List<Customer> GetAllCustomers() 
    {
        return _customers;
    }

    public Customer GetCustomer(Guid id)
    {
        return _customers.SingleOrDefault(c => c.Id == id);
    }

    public bool DeleteCustomer(Guid id)
    {
        var removedCount = _customers.RemoveAll(x => x.Id == id);
        return removedCount > 0;  // true if deleted, false if not found
    }

    public bool UpdateCustomer(Guid id, Customer updatedCustomer)
    {
        var existingCustomer = _customers.FirstOrDefault(c => c.Id == id);

        if (existingCustomer == null)
        {
            return false;  // Customer not found
        }

        // Update the properties
        existingCustomer.Name = updatedCustomer.Name;
        existingCustomer.Email = updatedCustomer.Email;

        return true;
    }
}
