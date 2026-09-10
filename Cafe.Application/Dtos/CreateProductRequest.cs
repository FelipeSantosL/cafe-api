using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Application.Dtos
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public Guid CategoryId { get; set; }
    }
}
