using Microsoft.AspNetCore.Mvc.Rendering;
using SV21T1020873.Shop.Models;

namespace SV21T1020873.Shop.Models
{
    /// <summary>
    /// Đầu vào sử dụng cho tìm kiếm đơn hàng
    /// </summary>
    public class OrderSearchInput : PaginationSearchInput
    {
        /// <summary>
        /// Trạng thái của đơn hàng cần tìm
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// Khoảng thời gian cần tìm (chuỗi 2 giá trị ngày có dạng dd/MM/yyyy - dd/MM/yyyy)
        /// </summary>
        public string DateRange { get; set; } = "";
        /// <summary>
        /// Lấy thời điểm bắt đầu dựa vào DateRange
        /// </summary>
        public DateTime? FromDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;

                string[] times = DateRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[0].Trim().ToDateTime();
                    return value;
                }

                return null;
            }
        }
        /// <summary>
        /// Lấy thời điểm kết thúc dựa vào DateRange
        /// (thời điểm kết thúc phải là cuối ngày)
        /// </summary>
        public DateTime? ToDate
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;

                string[] times = DateRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = times[1].Trim().ToDateTime();
                    if (value.HasValue)
                        value = value.Value.AddMilliseconds(86399998); //86399999
                    return value;
                }

                return null;
            }
        }
    }
}