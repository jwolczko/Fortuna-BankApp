using Fortuna.ReadModel.Dashboard.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace Fortuna.ReadModel.Persistence;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    public DbSet<ProductTileReadModel> ProductTiles => Set<ProductTileReadModel>();
    public DbSet<TimelineEventReadModel> TimelineEvents => Set<TimelineEventReadModel>();
    public DbSet<ProcessedOutboxMessageReadModel> ProcessedOutboxMessages => Set<ProcessedOutboxMessageReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);
    }
}
