using System.ComponentModel.DataAnnotations;

namespace Fashion.Areas.Admin.ViewModels.Product
{
    public class ProductCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Size { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        [Required]
        public IEnumerable<IFormFile> ProductImages { get; set; }
    }
}
