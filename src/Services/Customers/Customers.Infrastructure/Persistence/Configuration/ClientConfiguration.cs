using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Customers.Domain.Entities;

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

        // Configuración de Value Objects
        builder.OwnsOne(c => c.Gender, gender =>
        {
            gender.Property(g => g.Value)
                .HasColumnName("Gender")
                .HasColumnType("nvarchar(10)")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.OwnsOne(c => c.Age, age =>
        {
            age.Property(a => a.Value)
                .HasColumnName("Age")
                .HasColumnType("int")
                .IsRequired();
        });

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Value)
                .HasColumnName("Address")
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(c => c.Phone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("Phone")
                .HasColumnType("nvarchar(15)")
                .HasMaxLength(15)
                .IsRequired();
        });

        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100)
                .IsRequired();

            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Clients_Email");
        });

        builder.OwnsOne(c => c.Password, password =>
        {
            password.Property(p => p.Value)
                .HasColumnName("PasswordHash")
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255)
                .IsRequired();
        });
    }
}
