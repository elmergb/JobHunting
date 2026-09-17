using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using UserId = JobHunting.Domain.Primatives.UserId;

namespace JobHunting.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("tblUser");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.MiddleName)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(x => x.RefreshToken)
                .IsRequired(false)
                .HasMaxLength(512);

            builder.Property(x => x.RefreshTokenExpiry)
                .IsRequired(false);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // 1-to-1 navigation to profile
            builder.HasOne(x => x.Profile)
                .WithOne()
                .HasForeignKey<UserProfile>("UserId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Email).IsUnique();
        }
    }
}
