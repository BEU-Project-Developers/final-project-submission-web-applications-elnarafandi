using System.ComponentModel.DataAnnotations;

namespace Fashion.Areas.Admin.ViewModels.Blog
{
    public class BlogCreateVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]  
        public IFormFile UploadImage { get; set; }
    }
}
