using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CompanyId = JobHunting.Domain.Primatives.CompanyId;

namespace JobHunting.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : BaseRepository<Company, CompanyId>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context)
        {
        }
    }
}
