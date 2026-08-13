using JobHunting.Domain.Repositories;
using JobHunting.Infrastructure.Persistence;
using JobHunting.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;


namespace JobHunting.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(  
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });

            // 2. Repositories
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();

            return services;
        }
    }
}
