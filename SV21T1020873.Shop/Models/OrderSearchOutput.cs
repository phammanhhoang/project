using Microsoft.AspNetCore.Mvc.Rendering;
using SV21T1020873.Shop.Models;
using SV21T1020873.DomainModels;

namespace SV21T1020873.Shop.Models
{
    /// <summary>
    /// Đầu ra tìm kiếm đơn hàng
    /// </summary>
    public class OrderSearchOutput : PaginationSearchOutput<Order>
    {
        public int Status { get; set; } = 0;
        public string DateRange { get; set; } = "";
    }
}