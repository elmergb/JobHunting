using JobHunting.Domain.Entities;
using JobHunting.Domain.Primatives;
using JobHunting.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserId = JobHunting.Domain.Primatives.UserId;

namespace JobHunting.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }
    }
}
