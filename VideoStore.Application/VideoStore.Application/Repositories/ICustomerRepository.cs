using VideoStore.Application.Models;

namespace VideoStore.Application.Repositories
{
    public interface ICustomerRepository
    {
        void AddCustomer(Customer customer);
        List<Customer> GetAllCustomers();
        Customer GetCustomer(Guid id);

        bool DeleteCustomer(Guid id);

        bool UpdateCustomer(Guid id, Customer updatedCustomer);
    }
}