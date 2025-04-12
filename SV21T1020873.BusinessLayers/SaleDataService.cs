using SV21T1020873.DataLayers;
using SV21T1020873.DomainModels;

namespace SV21T1020873.BusinessLayers
{
    public static class SaleDataService
    {
        /// <summary>
        /// Ctor
        /// </summary>
        static SaleDataService()
        {
            string connectionString = Configuration.ConnectionString;
            OrderDB = new OrderDAL(connectionString);
        }
        /// <summary>
        /// Đơn hàng
        /// </summary>
        private static OrderDAL OrderDB { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Order> ListOrders(out int rowCount, int page = 1,int pageSize = 0, int status = 0, DateTime? fromTime = null, DateTime? toTime = null, string searchValue = "")
        {
            rowCount = OrderDB.Count(status, fromTime, toTime, searchValue);
            return OrderDB.List(page, pageSize, status, fromTime, toTime, searchValue);
        }
        /// <summary>
        /// Lấy thông tin một đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public static Order? GetOrder(int orderID)
        {
            return OrderDB.Get(orderID);
        }
        /// <summary>
        /// Bổ sung một đơn hàng. Hàm trả về ID của đơn hàng được tạo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddOrder(Order data)
        {
            return OrderDB.Add(data);
        }
        /// <summary>
        /// Duyệt đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static bool AcceptOrder(int orderId, int employeeId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status != Constants.ORDER_INIT)
                return false;
            order.Status = Constants.ORDER_ACCEPTED;
            order.EmployeeID = employeeId;
            order.AcceptTime = DateTime.Now;

            return OrderDB.Update(order);
        }
        /// <summary>
        /// Giao hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shipperId"></param>
        /// <returns></returns>
        public static bool ShipOrder(int orderId, int shipperId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status == Constants.ORDER_ACCEPTED || order.Status == Constants.ORDER_SHIPPING)
            {
                order.Status = Constants.ORDER_SHIPPING;
                order.ShipperID = shipperId;
                order.ShippedTime = DateTime.Now;
                return OrderDB.Update(order);
            }
            return false;
        }
        /// <summary>
        /// Hoàn thành đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool FinishOrder(int orderId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status == Constants.ORDER_ACCEPTED || order.Status == Constants.ORDER_SHIPPING)
            {
                order.Status = Constants.ORDER_FINISHED;
                order.FinishedTime = DateTime.Now;
                return OrderDB.Update(order);
            }
            return false;
        }
        /// <summary>
        /// Hủy đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool CancelOrder(int orderId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status == Constants.ORDER_FINISHED)
                return false;
            order.Status = Constants.ORDER_CANCEL;
            return OrderDB.Update(order);
        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public static bool RejectOrder(int orderId, int employeeId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status != Constants.ORDER_INIT)
                return false;
            order.Status = Constants.ORDER_REJECTED;
            order.AcceptTime = DateTime.Now;
            return OrderDB.Update(order);
        }
        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public static bool DeleteOrder(int orderId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status == Constants.ORDER_CANCEL || order.Status == Constants.ORDER_REJECTED)
                return OrderDB.Delete(orderId);
            return false;
        }
        public static List<OrderDetail> ListOrderDetails (int orderId)
        {
            return OrderDB.ListDetails(orderId);
        }
        public static OrderDetail? GetOrderDetail(int orderId, int productId)
        {
            return OrderDB.GetDetail(orderId, productId);
        }
        public static bool SaveOrderDetail(int orderId, int productId, int quantity, decimal salePrice)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if(order.Status == Constants.ORDER_INIT || order.Status == Constants.ORDER_ACCEPTED)
            {
                return OrderDB.SaveDetail(orderId, productId, quantity, salePrice);
            }
            return false;
        }
        public static bool DeleteOrderDetail(int orderId, int productId)
        {
            Order? order = OrderDB.Get(orderId);
            if (order == null)
                return false;
            if (order.Status == Constants.ORDER_INIT || order.Status == Constants.ORDER_ACCEPTED)
            {
                return OrderDB.DeleteDetail(orderId, productId);
            }
            return false;
        }
        public static int Init(int employeeID, int customerID, string deliveryProvince, string deliveryAddress, List<OrderDetail> orderDetails)
        {
            Order order = new Order
            {
                EmployeeID = employeeID,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                Status = Constants.ORDER_INIT,
                OrderTime = DateTime.Now
            };
            int orderId = OrderDB.Add(order);
            if (orderId > 0)
            {
                foreach (var detail in orderDetails)
                {
                    OrderDB.SaveDetail(orderId, detail.ProductID, detail.Quantity, detail.SalePrice);
                }
            }
            return orderId;
        }
        public static List<Order> OrderHistory(int customerID)
        {
            return OrderDB.OrderHistory(customerID).ToList();
        }
    }
}
