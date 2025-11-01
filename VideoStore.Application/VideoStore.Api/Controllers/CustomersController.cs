using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VideoStore.Application.Models;
using VideoStore.Application.Repositories;

namespace VideoStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            _customerRepository.AddCustomer(customer);
            return Ok(customer);
        }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var output = _customerRepository.GetAllCustomers();
            return Ok(output);

        }

        [HttpGet("{id}")]  // GET /api/customers/{id}
        public IActionResult GetCustomer(Guid id)
        {
            var customer = _customerRepository.GetCustomer(id);

            if (customer == null)
            {
                return NotFound();  // 404 if customer doesn't exist
            }

            return Ok(customer);  // 200 with the customer
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(Guid id)
        {
            var deleted = _customerRepository.DeleteCustomer(id);

            if (!deleted)
            {
                return NotFound();  // 404 if customer didn't exist
            }

            return NoContent();  // 204 No Content (success, no body)
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCustomer(Guid id, [FromBody] Customer customer)
        {
            var updated = _customerRepository.UpdateCustomer(id, customer);

            if (!updated)
            {
                return NotFound();  // 404 if customer doesn't exist
            }

            return Ok(customer);  // 200 OK with updated customer
                                  // Or: return NoContent();  // 204 No Content (also valid)
        }
    }
}
