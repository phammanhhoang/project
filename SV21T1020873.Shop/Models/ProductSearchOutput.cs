using SV21T1020873.Shop.Models;
using SV21T1020873.DomainModels;

namespace SV21T1020873.Shop.Models
{
    public class ProductSearchOutput : PaginationSearchOutput<Product>
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
    }
}