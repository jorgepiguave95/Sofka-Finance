using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

namespace Account.Infrastructure.Persistence;

public class MigrationDbContext : IDesignTimeDbContextFactory<AccountDbContext>
{
    public AccountDbContext CreateDbContext(string[] args)
    {
        Env.Load(".env");

        var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();

        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "1433";
        var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "SofkaAccount";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "sa";
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? throw new InvalidOperationException("DB_PASSWORD is required");

        var connectionString = $"Server={dbHost},{dbPort};Database={dbName};User Id={dbUser};Password={dbPassword};TrustServerCertificate=true;";

        optionsBuilder.UseSqlServer(connectionString,
            b => b.MigrationsAssembly("Account.Infrastructure"));

        return new AccountDbContext(optionsBuilder.Options);
    }
}
