using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application
{
    public class NotFoundException: Exception
    {
        public NotFoundException(string entity, object key): base($"{entity} com id '{key}' não foi encontrado.") { }
    }
}
