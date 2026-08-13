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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(CompanyId id, CancellationToken ct = default)
        {
            return await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task AddAsync(Company company, CancellationToken ct = default)
        {
            await _context.Companies.AddAsync(company, ct);
            await _context.SaveChangesAsync(ct);
        }
    }

}
