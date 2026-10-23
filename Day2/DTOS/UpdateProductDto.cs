using System.ComponentModel.DataAnnotations;

namespace Day2.DTOS
{
    
        public class UpdateProductDto
        {
            [Required, MaxLength(200)]
            public string Name { get; set; } = string.Empty;

            [Range(0.01, double.MaxValue)]
            public decimal Price { get; set; }

            public int CategoryId { get; set; }

            [Required]
            public byte[] RowVersion { get; set; } = [];
        }
    }

