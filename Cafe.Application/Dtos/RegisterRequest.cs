using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos
{
    public class RegisterRequest
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
