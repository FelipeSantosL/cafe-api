using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = UserRoles.Customer;
        public DateTime CreatedAt { get; set; }
    }
}
