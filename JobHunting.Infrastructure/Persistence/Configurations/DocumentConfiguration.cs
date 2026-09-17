using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Infrastructure.Persistence.Configurations
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new DocumentId(value));

            builder.Property(x => x.ApplicationId)
                .HasConversion(
                    id => id == null ? (Guid?)null : id.Value,
                    value => value == null ? null : new ApplicationId(value.Value))
                .IsRequired(false);

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasMaxLength(450);     // matches ASP.NET Identity key length

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(260);     // max Windows path length

            builder.Property(x => x.StoragePath)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(x => x.ParsedText)
                .IsRequired(false);

            builder.Property(x => x.Type)
                .HasConversion<string>()    // store enum as string for readability
                .HasMaxLength(50);

            builder.Property(x => x.IsMasterVersion)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.ApplicationId);
            builder.HasIndex(x => new { x.UserId, x.Type, x.IsMasterVersion });
        }
    }
}
