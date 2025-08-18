using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customers.Domain.Entities;
using Customers.Domain.ValueObjects;
using Customers.Infrastructure.Persistence.Converters;

namespace Customers.Infrastructure.Persistence.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id)
            .HasName("PK_Clients");

        builder.Property(c => c.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Identification)
            .HasColumnType("nvarchar(20)")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(c => c.Identification)
            .IsUnique()
            .HasDatabaseName("IX_Clients_Identification");

        builder.Property(c => c.IsActive)
            .HasColumnType("bit")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnType("datetime2")
            .IsRequired();

        // Configuración de Value Objects con ValueConverters personalizados
        builder.Property(c => c.Gender)
            .HasColumnName("Gender")
            .HasColumnType("nvarchar(10)")
            .HasMaxLength(10)
            .HasConversion(new GenderValueConverter())
            .IsRequired();

        builder.Property(c => c.Age)
            .HasColumnName("Age")
            .HasColumnType("int")
            .HasConversion(new AgeValueConverter())
            .IsRequired();

        builder.Property(c => c.Address)
            .HasColumnName("Address")
            .HasColumnType("nvarchar(200)")
            .HasMaxLength(200)
            .HasConversion(new AddressValueConverter())
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasColumnName("Phone")
            .HasColumnType("nvarchar(15)")
            .HasMaxLength(15)
            .HasConversion(new PhoneNumberValueConverter())
            .IsRequired();

        builder.Property(c => c.Email)
            .HasColumnName("Email")
            .HasColumnType("nvarchar(100)")
            .HasMaxLength(100)
            .HasConversion(new EmailValueConverter())
            .IsRequired();

        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasDatabaseName("IX_Clients_Email");

        builder.Property(c => c.Password)
            .HasColumnName("PasswordHash")
            .HasColumnType("nvarchar(255)")
            .HasMaxLength(255)
            .HasConversion(new PasswordValueConverter())
            .IsRequired();
    }
}
