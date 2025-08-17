using Microsoft.EntityFrameworkCore;
using Customers.Domain.Entities;
using Customers.Infrastructure.Persistence.Configuration;

namespace Customers.Infrastructure.Persistence;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ClientConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Client && e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var client = (Client)entityEntry.Entity;

            entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
