// Models/Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OssecAssignment.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; }
    }
}
