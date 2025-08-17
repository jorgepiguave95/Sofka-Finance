using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace Customers.Infrastructure.Persistence;

public class MigrationDbContext : IDesignTimeDbContextFactory<CustomersDbContext>
{
    public CustomersDbContext CreateDbContext(string[] args)
    {
        Env.Load(".env");

        var optionsBuilder = new DbContextOptionsBuilder<CustomersDbContext>();

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME_CUSTOMERS") ?? "SofkaCustomers";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD is required");

        var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";

        optionsBuilder.UseSqlServer(connectionString,
            b => b.MigrationsAssembly("Customers.Infrastructure"));

        return new CustomersDbContext(optionsBuilder.Options);
    }
}
