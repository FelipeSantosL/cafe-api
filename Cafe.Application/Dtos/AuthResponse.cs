using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
