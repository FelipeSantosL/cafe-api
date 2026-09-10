using Cafe.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Infrastructure.Auth
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
