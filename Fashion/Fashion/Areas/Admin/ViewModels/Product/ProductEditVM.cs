using Fashion.Models;
using System.ComponentModel.DataAnnotations;

namespace Fashion.Areas.Admin.ViewModels.Product
{
    public class ProductEditVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public IEnumerable<IFormFile> UploadImages { get; set; }

        public IEnumerable<ProductImage> ProductImages { get; set; }
    }
}
