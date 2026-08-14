using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;
using ApplicationId = JobHunting.Domain.Primatives.ApplicationId;

namespace JobHunting.Domain.Repositories
{
    public interface IJobApplicationRepository : IRepository<JobApplication, ApplicationId>
    {
        Task<IReadOnlyList<JobApplication>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    }
}
