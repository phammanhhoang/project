using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using SV21T1020873.Shop.Models;

namespace SV21T1020873.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart1";
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale1";
        private const int PRODUCT_PAGE_SIZE = 5;

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

        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }

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

        public IActionResult Init(string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống, Vui lòng chọn mặt hàng cần bán");
            if (string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            int customerID = Convert.ToInt32(User.GetUserData()?.UserId);
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
            int orderID = SaleDataService.Init(1, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);


        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult HistoryOrder()
        {
            int customerID = Convert.ToInt32(User.GetUserData()?.UserId);
            List<OrderDetailModel>? response = new List<OrderDetailModel>();
            var data = SaleDataService.OrderHistory(customerID);
            if (data == null)
            {
                return RedirectToAction("Index", "Home");
            }
            foreach (var item in data)
            {
                var order = SaleDataService.GetOrder(item.OrderID);
                var orderDetails = SaleDataService.ListOrderDetails(item.OrderID);
                var orderDetailsModel = new OrderDetailModel()
                {
                    Details = orderDetails,
                    Order = order,
                };
                response.Add(orderDetailsModel);
            }
            return View(response);
        }
        public IActionResult OrderDetails(int id)
        {
            var order = SaleDataService.GetOrder(id);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var details = SaleDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }
    }
}
