using System.ComponentModel.DataAnnotations;

namespace Fashion.Areas.Admin.ViewModels.Blog
{
    public class BlogEditVM
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public IFormFile UploadImage { get; set; }
        public string Image {  get; set; }
    }
}
