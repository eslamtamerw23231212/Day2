using System.ComponentModel.DataAnnotations;

namespace Day2.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Soft Delete (المطلوب 4)
        public bool IsDeleted { get; set; }

        // Concurrency Token (المطلوب 6) - خاصية SQL Server rowversion
        [Timestamp]
        public byte[] RowVersion { get; set; } = [];
    }
}
