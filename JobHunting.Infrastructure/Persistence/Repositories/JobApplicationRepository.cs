using JobHunting.Domain.Entities;
using JobHunting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Infrastructure.Persistence.Repositories
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly AppDbContext _context;

        public JobApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<JobApplication?> GetByIdAsync(ApplicationId id, CancellationToken ct = default)
        {
            return await _context.JobApplications
                .Include(a => a.Interviews)   
                .Include(a => a.History)   
                .Include(a => a.Offer)       
                .FirstOrDefaultAsync(a => a.Id.Value == id.Value, ct);
        }

        public async Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            return await _context.JobApplications
                .AsNoTracking()              
                .Include(a => a.Interviews)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync(ct);
        }

        public async Task AddAsync(JobApplication application, CancellationToken ct = default)
        {
            await _context.JobApplications.AddAsync(application, ct);
            await _context.SaveChangesAsync(ct);  
        }

        public async Task UpdateAsync(JobApplication application, CancellationToken ct = default)
        {
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync(ct);
        }
    }
}
