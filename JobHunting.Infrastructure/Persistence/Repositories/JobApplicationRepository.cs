using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for JobApplication aggregate root.
    /// Handles data access operations specific to job applications.
    /// </summary>
    public class JobApplicationRepository : BaseRepository<JobApplication, ApplicationId>, IJobApplicationRepository
    {
        public JobApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<JobApplication?> GetByIdAsync(ApplicationId id, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(a => a.Interviews)   
                .Include(a => a.History)   
                .Include(a => a.Offer)       
                .FirstOrDefaultAsync(a => a.Id.Equals(id), ct);
        }

        public async Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()              
                .Include(a => a.Interviews)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync(ct);
        }
    }
}
