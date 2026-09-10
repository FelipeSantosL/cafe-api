using Cafe.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    }
}
