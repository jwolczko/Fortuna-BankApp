using Fortuna.Domain.Customers;
using Fortuna.Domain.Customers.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.Infrastructure.Persistence.Write.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly WriteDbContext _dbContext;

    public CustomerRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken)
        => _dbContext.Customers.AddAsync(customer, cancellationToken).AsTask();

    public Task<Customer?> GetByIdAsync(CustomerId customerId, CancellationToken cancellationToken)
        => _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);

    public Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        => _dbContext.Customers.FirstOrDefaultAsync(x => x.Email.Value == email.Value, cancellationToken);
}
