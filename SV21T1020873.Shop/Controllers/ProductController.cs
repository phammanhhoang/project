using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using SV21T1020873.Shop;
using SV21T1020873.Shop.Models;

namespace SV21T1020873.Shop.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private const string SHOPPING_CART = "ShoppingCart1";
        private const int PAGE_SIZE = 16;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            var suppliers = CommonDataService.ListOfSupplier();
            var categories = CommonDataService.ListOfCategories();
            ViewData["Suppliers"] = suppliers;
            ViewData["Categories"] = categories;
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
            {

                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    SupplierID = 0,
                    CategoryID = 0,
                    MinPrice = 0,
                    MaxPrice = 0

                };
            }
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = CommonDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchOutput model = new ProductSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Detail(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = CommonDataService.GetProduct(id);
            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }
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

        public IActionResult AddToCart(CartItem data)
        {

            if (data.SalePrice < 0 || data.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(data);
            }
            else
            {
                existsProduct.Quantity += data.Quantity;
                existsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            return Json("Thêm vào giỏ hàng thành công!");
        }
    }
}
