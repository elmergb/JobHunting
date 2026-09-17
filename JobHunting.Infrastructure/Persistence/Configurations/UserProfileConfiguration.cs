using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ProfileId = JobHunting.Domain.Primatives.ProfileId;
using UserId = JobHunting.Domain.Primatives.UserId;

namespace JobHunting.Infrastructure.Persistence.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("tblProfile");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new ProfileId(value));

            builder.Property(x => x.UserId)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .IsRequired();

            builder.Property(x => x.PhoneNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(x => x.AvatarUrl)
                .IsRequired(false)
                .HasMaxLength(2000);

            builder.Property(x => x.Bio)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.HasIndex(x => x.UserId).IsUnique();
        }
    }
}
