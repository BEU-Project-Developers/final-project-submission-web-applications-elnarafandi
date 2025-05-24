using Fashion.Models;

namespace Fashion.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
    }
}
