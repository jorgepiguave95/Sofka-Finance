using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Account.Domain.Entities;

namespace Account.Infrastructure.Persistence.Configuration;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.ToTable("Movements");

        builder.HasKey(m => m.Id)
            .HasName("PK_Movements");

        builder.Property(m => m.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.Property(m => m.AccountId)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        builder.OwnsOne(m => m.Type, type =>
        {
            type.Property(t => t.Value)
                .HasColumnName("MovementType")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(m => m.Value)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(m => m.AvailableBalance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(m => m.Status)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(m => m.Date)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(m => m.Concept)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.HasOne<Domain.Entities.Account>()
            .WithMany()
            .HasForeignKey(m => m.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}