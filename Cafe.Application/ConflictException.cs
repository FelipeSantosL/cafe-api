using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application
{
    public class ConflictException: Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}
