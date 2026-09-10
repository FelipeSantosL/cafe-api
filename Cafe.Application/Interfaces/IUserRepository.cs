using Cafe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
    }
}
