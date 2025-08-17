using Microsoft.EntityFrameworkCore;
using Account.Domain.Entities;
using Account.Infrastructure.Persistence.Configuration;
using AccountEntity = Account.Domain.Entities.Account;

namespace Account.Infrastructure.Persistence;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {
    }

    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<Movement> Movements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new MovementConfiguration());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AccountEntity && e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            var account = (AccountEntity)entityEntry.Entity;
            entityEntry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}