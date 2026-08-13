using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CompanyId(value));

            builder.OwnsOne(x => x.Location, loc =>
            {
                loc.Property(l => l.City).HasColumnName("LocationCity");
                loc.Property(l => l.State).HasColumnName("LocationState");
                loc.Property(l => l.Country).HasColumnName("LocationCountry");
                loc.Property(l => l.IsRemote).HasColumnName("IsRemote");
            });

            builder.HasIndex(x => x.Name).IsUnique();  
        }
    }
}
