using Cafe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
