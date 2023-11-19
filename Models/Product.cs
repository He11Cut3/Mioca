using System.ComponentModel.DataAnnotations.Schema;

namespace Miocas.Models
{
    public class Product
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
