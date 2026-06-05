using Fortuna.Application.Dashboard.Queries.GetDashboard;
using Fortuna.Infrastructure.Options;
using Fortuna.Infrastructure.Projections.Interfaces;
using Fortuna.ReadModel.Dashboard.Queries;
using Fortuna.ReadModel.Persistence;
using Fortuna.ReadModel.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fortuna.ReadModel.DependencyInjection;

public static class ReadModelServiceRegistration
{
    public static IServiceCollection AddReadModel(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<ReadDbContext>(options =>
            options.UseSqlServer(databaseOptions.ReadConnectionString));

        services.AddScoped<IDashboardReadRepository, DashboardReadRepository>();
        services.AddScoped<IReadModelProjector, ProjectionDispatcher>();
        return services;
    }
}
