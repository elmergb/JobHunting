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
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.HasKey(x => x.Id);

            
            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,           
                    value => new ApplicationId(value));  

          
            builder.Property(x => x.CompanyId)
                .HasConversion(
                    id => id.Value,
                    value => new CompanyId(value));

            builder.OwnsOne(x => x.SalaryExpectation, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("SalaryExpectationAmount");
                money.Property(m => m.Currency)
                    .HasColumnName("SalaryExpectationCurrency");
            });

            builder.OwnsOne(x => x.PostedSalaryRange, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("PostedSalaryMin");
                money.Property(m => m.Currency)
                    .HasColumnName("PostedSalaryCurrency");
            });

            builder.OwnsOne(x => x.Source, source =>
            {
                source.Property(s => s.Type).HasColumnName("SourceType");
                source.Property(s => s.Url).HasColumnName("SourceUrl");
                source.Property(s => s.ReferralContactName).HasColumnName("SourceReferralName");
            });

            builder.HasMany(typeof(Interview), "_interviews") 
                .WithOne()
                .HasForeignKey("ApplicationId")  
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(typeof(StatusChange), "_history")
                .WithOne()
                .HasForeignKey("ApplicationId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}