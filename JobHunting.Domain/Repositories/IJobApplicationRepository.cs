using JobHunting.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Repositories
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(ApplicationId id, CancellationToken ct = default);
        Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId, CancellationToken ct = default);
        Task AddAsync(JobApplication application, CancellationToken ct = default);
        Task UpdateAsync(JobApplication application, CancellationToken ct = default);
    }
}
