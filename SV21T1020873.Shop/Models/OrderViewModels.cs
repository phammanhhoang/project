using Microsoft.AspNetCore.Mvc.Rendering;
using SV21T1020873.Shop.Models;
using SV21T1020873.DomainModels;

namespace SV21T1020873.Shop.Models
{
    public class OrderViewModels
    {
        /// <summary>
        /// Danh sách các SelectListItem thể hiện các trạng thái đơn hàng
        /// </summary>
        public List<SelectListItem> OrderStatus
        {
            get
            {
                List<SelectListItem> list =
                [
                    new SelectListItem() { Value = "0", Text = "-- Trạng thái đơn hàng --"},
                    new SelectListItem() { Value = Constants.ORDER_INIT.ToString(), Text = "Đơn hàng mới (đang chờ duyệt)" },
                    new SelectListItem() { Value = Constants.ORDER_ACCEPTED.ToString(), Text = "Đơn hàng đã duyệt (chờ giao hàng)" },
                    new SelectListItem() { Value = Constants.ORDER_SHIPPING.ToString(), Text = "Đơn hàng đang được giao" },
                    new SelectListItem() { Value = Constants.ORDER_FINISHED.ToString(), Text = "Đơn hàng đã hoàn tất "},
                    new SelectListItem() { Value = Constants.ORDER_CANCEL.ToString(), Text = "Đơn hàng bị hủy" },
                    new SelectListItem() { Value = Constants.ORDER_REJECTED.ToString(), Text = "Đơn hàng bị từ chối" }
                ];
                return list;
            }
        }
    }
}