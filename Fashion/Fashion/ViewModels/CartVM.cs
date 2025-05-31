using Fashion.Models;

namespace Fashion.ViewModels
{
    public class CartVM
    {
        public IEnumerable<Product> Products { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
