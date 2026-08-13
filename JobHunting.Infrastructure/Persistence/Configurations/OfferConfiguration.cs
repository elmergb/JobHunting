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
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new OfferId(value));

            builder.Property(x => x.ApplicationId)
                .HasConversion(
                    id => id.Value,
                    value => new ApplicationId(value));

            builder.OwnsOne(x => x.BaseSalary, money =>
            {
                money.Property(m => m.Amount).HasColumnName("BaseSalaryAmount");
                money.Property(m => m.Currency).HasColumnName("BaseSalaryCurrency");
            });

            builder.OwnsOne(x => x.SigningBonus, money =>
            {
                money.Property(m => m.Amount).HasColumnName("SigningBonusAmount");
                money.Property(m => m.Currency).HasColumnName("SigningBonusCurrency");
            });

            builder.OwnsOne(x => x.AnnualBonus, money =>
            {
                money.Property(m => m.Amount).HasColumnName("AnnualBonusAmount");
                money.Property(m => m.Currency).HasColumnName("AnnualBonusCurrency");
            });

            builder.OwnsOne(x => x.Equity, eq =>
            {
                eq.Property(e => e.Shares).HasColumnName("EquityShares");
                eq.Property(e => e.StrikePrice).HasColumnName("EquityStrikePrice");
                eq.Property(e => e.VestingYears).HasColumnName("EquityVestingYears");
                eq.Property(e => e.CliffMonths).HasColumnName("EquityCliffMonths");
            });
        }
    }
}
