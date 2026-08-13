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
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(
                    id => id.Value,
                    value => new InterviewId(value));

                       builder.Property(x => x.ApplicationId)
                           .HasConversion(
                               id => id.Value,
                               value => new ApplicationId(value));

                       builder.OwnsOne(x => x.Interviewer, contact =>
                       {
                           contact.Property(c => c.Name).HasColumnName("InterviewerName");
                           contact.Property(c => c.Role).HasColumnName("InterviewerRole");
                           contact.Property(c => c.Email).HasColumnName("InterviewerEmail");
                       });
                   }

               }
           }
