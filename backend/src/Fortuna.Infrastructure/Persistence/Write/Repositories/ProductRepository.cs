using Fortuna.Domain.Products;
using Fortuna.Domain.Products.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.Infrastructure.Persistence.Write.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly WriteDbContext _dbContext;

    public ProductRepository(WriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken)
        => _dbContext.Products.AddAsync(product, cancellationToken).AsTask();

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
        => _dbContext.Products.FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

    public async Task<long> GetNextNumberSequenceAsync(CancellationToken cancellationToken)
    {
        var currentMaxSequence = await _dbContext.Products
            .MaxAsync(x => (long?)x.NumberSequence, cancellationToken);

        return (currentMaxSequence ?? 0) + 1;
    }
}
