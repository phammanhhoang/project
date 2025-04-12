using SV21T1020873.DomainModels;

namespace SV21T1020873.Shop.Models
{
    /// <summary>
    /// Chi tiết đơn hàng
    /// </summary>
    public class OrderDetailModel
    {
        public required Order Order { get; set; }
        public required List<OrderDetail> Details { get; set; }
    }
}
