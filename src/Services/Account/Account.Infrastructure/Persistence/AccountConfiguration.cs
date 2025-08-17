using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Account.Domain.Entities;

namespace Account.Infrastructure.Persistence.Configuration;

public class AccountConfiguration : IEntityTypeConfiguration<Domain.Entities.Account>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(a => a.Id)
            .HasName("PK_Accounts");

        builder.Property(a => a.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(a => a.CustomerId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.OwnsOne(a => a.Number, number =>
        {
            number.Property(n => n.Value)
                .HasColumnName("AccountNumber")
                .HasMaxLength(30)
                .IsRequired();

            number.HasIndex(n => n.Value)
                .IsUnique()
                .HasDatabaseName("IX_Accounts_AccountNumber");
        });

        builder.OwnsOne(a => a.Type, type =>
        {
            type.Property(t => t.Value)
                .HasColumnName("AccountType")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(a => a.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(a => a.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired(false);
    }
}