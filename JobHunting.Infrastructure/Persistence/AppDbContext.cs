using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;


namespace JobHunting.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        // DbSet = table sa database
        public DbSet<JobApplication> JobApplications { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DomainEvent as keyless (not a table)
            modelBuilder.Ignore<DomainEvent>();

            // Auto-scan lahat ng IEntityTypeConfiguration sa assembly na ito
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
