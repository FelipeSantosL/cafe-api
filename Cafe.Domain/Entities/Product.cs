using System;
using System.Collections.Generic;
using System.Text;

namespace Cafe.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; } 
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
