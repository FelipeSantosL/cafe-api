using Cafe.Application.Interfaces;
using Cafe.Domain.Entities;
using Cafe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Infrastructure.Repositories
{
    public class UserRepository(CafeDbContext db) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
            => await db.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync(ct);
        }
    }
}