namespace Fortuna.Domain.Customers.Repositories;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken);
    Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
}
