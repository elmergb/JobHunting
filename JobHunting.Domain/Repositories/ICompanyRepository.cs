using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobHunting.Domain.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(CompanyId id, CancellationToken ct = default);
        Task AddAsync(Company company, CancellationToken ct = default);
    }
}
