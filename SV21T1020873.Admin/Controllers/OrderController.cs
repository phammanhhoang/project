using Microsoft.AspNetCore.Mvc;
using static SV21T1020873.Admin.Models.OrderViewModels;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using System.Globalization;
using SV21T1020873.DomainModels;

namespace SV21T1020873.Admin.Controllers
{
    public class OrderController : Controller
    {
        private const string ORDER_SEARCH_INPUT = "OrderSearchInput";

        /// <summary>
        /// Giao diện tìm kiếm, hiển thị danh sách đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_INPUT);
            if (input == null)
            {
                var cultureInfo = new CultureInfo("en-GB");
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = 20,
                    SearchValue = "",
                    Status = 0,
                    DateRange = $"{DateTime.Today.AddYears(-3).ToString("dd/MM/yyyy", cultureInfo)} - {DateTime.Today.ToString("dd/MM/yyyy", cultureInfo)}"
                };
            }
            return View(input);
        }
        /// <summary>
        /// Tìm kiếm đơn hàng
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = SaleDataService.ListOrders(out rowCount, condition.Page, condition.PageSize,
                            condition.Status, condition.FromDate, condition.ToDate, condition.SearchValue ?? "");
            var model = new OrderSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                DateRange = condition.DateRange,
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_INPUT, condition);
            return View(model);
        }
        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id = 0)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }

            var order = SaleDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");

            var model = new OrderDetailModel()
            {
                Order = order,
                Details = SaleDataService.ListOrderDetails(id)
            };
            return View(model);
        }
        /// <summary>
        /// Duyệt đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Accept(int id = 0)
        {
            bool result = SaleDataService.AcceptOrder(id, Convert.ToInt32(User.GetUserData()?.UserId));
            if (!result)
                TempData["Message"] = "Không thể duyệt đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Hiển thị giao diện để chọn người giao hàng cho đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            var order = SaleDataService.GetOrder(id);
            return View(order);
        }
        /// <summary>
        /// Ghi nhận chuyển đơn hàng cho người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="shipperID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọn người giao hàng");

            bool result = SaleDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng");

            return Json("");
        }
        /// <summary>
        /// Ghi nhận hoàn tất đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Finish(int id = 0)
        {
            bool result = SaleDataService.FinishOrder(id);

            if (!result)
                TempData["Message"] = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";

            return RedirectToAction("Details", new { id });
        }

        /// <summary>
        /// Hủy bỏ đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Cancel(int id = 0)
        {
            bool result = SaleDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Không thể hủy đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Reject(int id = 0)
        {
            bool result = SaleDataService.RejectOrder(id, Convert.ToInt32(User.GetUserData()?.UserId));
            if (!result)
                TempData["Message"] = "Không thể từ chối đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Xóa đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            bool result = SaleDataService.DeleteOrder(id);
            if (!result)
                TempData["Message"] = "Không được phép xóa đơn hàng này";

            return RedirectToAction("Details", new { id });
        }
        /// <summary>
        /// Giao diện để cập nhật số lượng và giá của mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = SaleDataService.GetOrderDetail(id, productId);
            return View(model);
        }

        /// <summary>
        /// Cập nhật số lượng và giá của mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json("Số lượng bán không hợp lệ");
            if (salePrice < 0)
                return Json("Giá bán không hợp lệ");

            bool result = SaleDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            if (!result)
                return Json("Không cập nhật được dữ liệu");

            return Json("");
        }
        /// <summary>
        /// Xóa mặt hàng được bán trong đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public IActionResult DeleteDetail(int id, int productID)
        {
            bool result = SaleDataService.DeleteOrderDetail(id, productID);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";
            return RedirectToAction("Details", new { id });
        }


        ///////////////////////////////////////////////////////
        // CÁC CHỨC NĂNG LIÊN QUAN ĐẾN TẠO ĐƠN HÀNG MỚI
        ///////////////////////////////////////////////////////

        //Số mặt hàng được hiển thị trên một trang khi tìm kiếm mặt hàng để đưa vào đơn hàng
        private const int PRODUCT_PAGE_SIZE = 5;
        //Tên biến session lưu điều kiện tìm kiếm mặt hàng khi lập đơn hàng
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";
        //Tên biến session lưu giỏ hàng
        private const string SHOPPING_CART = "ShoppingCart";

        /// <summary>
        /// Giao diện trang lập đơn hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        /// <summary>
        /// Tìm kiếm mặt hàng cần bán
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            var model = new ProductSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = CommonDataService.ProductDB.Count(condition.SearchValue ?? ""),
                Data = CommonDataService.ProductDB.List(condition.Page, condition.PageSize, condition.SearchValue ?? "")
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        /// <summary>
        /// Lấy giỏ hàng hiện đang có (lưu trong session)
        /// </summary>
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        /// <summary>
        /// Trang hiển thị giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        /// <summary>
        /// Bổ sung thêm hàng vào giỏ
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == item.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice = item.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        /// <summary>
        /// Xóa một mặt hàng ra khỏi giỏ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        /// <summary>
        /// Xóa tất cả các mặt hàng trong giỏ
        /// </summary>
        /// <returns></returns>
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        /// <summary>
        /// Khởi tạo đơn hàng
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="deliveryProvince"></param>
        /// <param name="deliveryAddress"></param>
        /// <returns></returns>
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");

            if (customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice
                });
            }
            Order data = new Order()
            {
                OrderID = 0,
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince,
                DeliveryAddress = deliveryAddress,
                EmployeeID = null
            };
            int orderID = SaleDataService.AddOrder(data);
            if (orderID > 0)
            {
                foreach (var item in shoppingCart)
                {
                    SaleDataService.SaveOrderDetail(orderID, item.ProductID, item.Quantity, item.SalePrice);
                }
                ClearCart();
                return Json(orderID);
            }
            else
            {
                return Json("Không lập được đơn hàng");
            }
        }
    }
}
