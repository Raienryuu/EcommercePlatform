namespace ProductService.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public required string CategoryName { get; set;}
        public ProductCategory? ParentCategory { get; set;}
    }
}