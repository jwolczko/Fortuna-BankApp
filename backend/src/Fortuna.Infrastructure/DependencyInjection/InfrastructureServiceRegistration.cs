using Fortuna.Domain.Accounts.Repositories;
using Fortuna.Domain.Customers.Repositories;
using Fortuna.Domain.Products.Repositories;
using Fortuna.Domain.Transfers.Repositories;
using Fortuna.Infrastructure.Auth;
using Fortuna.Infrastructure.Options;
using Fortuna.Infrastructure.Persistence.Outbox;
using Fortuna.Infrastructure.Persistence.Write;
using Fortuna.Infrastructure.Persistence.Write.Repositories;
using Fortuna.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fortuna.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(databaseOptions.WriteConnectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBankAccountRepository, BankAccountRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ITransferRepository, TransferRepository>();

        services.AddScoped<Fortuna.Application.Abstractions.Persistence.IUnitOfWork>(sp => sp.GetRequiredService<WriteDbContext>());
        services.AddSingleton<Fortuna.Application.Abstractions.Security.IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<Fortuna.Application.Abstractions.Security.ITokenProvider, JwtTokenProvider>();
        services.AddSingleton<Fortuna.Application.Abstractions.Clock.IDateTimeProvider, DateTimeProvider>();

        services.AddHostedService<OutboxMessageProcessor>();

        return services;
    }
}
