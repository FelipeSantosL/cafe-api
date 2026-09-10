using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application
{
    public class UnauthorizedException: Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
