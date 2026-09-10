using Cafe.Application.Dtos;
using Cafe.Application.Interfaces;
using Cafe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Services
{
    public class AuthService(IUserRepository userRepository,IPasswordHasher passwordHasher,ITokenService tokenService) : IAuthService
    {
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (await userRepository.EmailExistsAsync(request.Email, ct))
                throw new ConflictException($"O email '{request.Email}' já está em uso.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHasher.Hash(request.Password), // nunca a senha pura!
                Role = UserRoles.Customer, // todo mundo nasce Customer; Admin é criado manualmente
                CreatedAt = DateTime.UtcNow
            };

            await userRepository.AddAsync(user, ct);

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = tokenService.GenerateToken(user)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            // Verificação genérica de propósito: não revela se o email existe ou não
            if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Email ou senha inválidos.");

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = tokenService.GenerateToken(user)
            };
        }
    }
}
