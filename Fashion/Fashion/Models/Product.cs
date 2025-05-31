namespace Fashion.Models
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public DateTime CreatedTime { get; set; }= DateTime.Now;
        public List<AppUserProduct> AppUserProducts { get; set; }
    }
}
