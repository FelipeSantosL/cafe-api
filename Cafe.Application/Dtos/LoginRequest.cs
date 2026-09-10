using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
