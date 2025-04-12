using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    //[Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        private const string PRODUCT_SEARCH = "ProductSearch";
        public IActionResult Index()
        {
            //Kiểm tra xem trong session có lưu điều kiện tìm kiếm không/
            //Nếu có thì sử dụng lại điều kiện tìm kiếm, ngược lại thì tìm kiếm theo điều kiện mặc định
            PaginationSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = 12,
                    SearchValue = "",
                    CategoryID = 0,
                    SupplierID = 0,
                };
            }
            return View(input);

        }
        public IActionResult Search(ProductSearchInput input)
        {
            ProductSearchOutput model = new ProductSearchOutput()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue,
                CategoryID = input.CategoryID,
                SupplierID = input.SupplierID,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice,
                RowCount = CommonDataService.ProductDB.Count(input.SearchValue ?? "", input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice),
                Data = CommonDataService.ProductDB.List(input.Page, input.PageSize, input.SearchValue ?? "", input.CategoryID, input.SupplierID, input.MinPrice, input.MaxPrice)
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            ViewBag.IsEdit = false;
            Product model = new Product()
            {
                ProductID = 0,
                Photo = "nophoto.png",
                IsSelling = true,
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            ViewBag.IsEdit = true;
            Product? model = CommonDataService.ProductDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            if (string.IsNullOrWhiteSpace(model.Photo))
                model.Photo = "nophoto.png";
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.ProductDB.Delete(id);
                return RedirectToAction("Index");
            }

            Product? model = CommonDataService.ProductDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

        public IActionResult Photo(int id, string method, long photoId = 0)
        {
            ProductPhoto model = null;

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh";
                    model = new ProductPhoto
                    {
                        ProductID = id,
                        PhotoID = 0,
                        Photo = "nophoto.png",
                        IsHidden = false,
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Cập nhật ảnh";
                    model = CommonDataService.ProductDB.GetPhoto(photoId);
                    if (model == null)
                        return RedirectToAction("Edit");

                    if (string.IsNullOrWhiteSpace(model.Photo))
                        model.Photo = "nophoto.png";
                    return View(model);
                case "delete":
                    //TODO: Xóa ảnh có mã photoID (xóa trực tiếp, ko cần xác nhận)
                    CommonDataService.ProductDB.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }

        public IActionResult Attribute(int id, string method, long attributeId = 0)
        {
            ProductAttribute model = null;

            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính";
                    model = new ProductAttribute
                    {
                        ProductID = id,
                        AttributeID = 0
                    };
                    return View(model);
                case "edit":
                    ViewBag.Title = "Chỉnh sửa thuộc tính";
                    model = CommonDataService.ProductDB.GetAttribute(attributeId);
                    if (model == null)
                        return RedirectToAction("Edit");

                    return View(model);
                case "delete":
                    //TODO: Xóa thuộc tính có mã attributeId (xóa trực tiếp, ko cần xác nhận)
                    CommonDataService.ProductDB.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SaveData(Product model, IFormFile? uploadPhoto)
        {
            if (string.IsNullOrWhiteSpace(model.ProductName))
                ModelState.AddModelError(nameof(model.ProductName), "Vui lòng nhập tên của mặt hàng");
            if (!model.CategoryID.HasValue || model.CategoryID == 0)
                ModelState.AddModelError(nameof(model.CategoryID), "Vui lòng chọn loại hàng cho mặt hàng");
            if (!model.SupplierID.HasValue || model.SupplierID == 0)
                ModelState.AddModelError(nameof(model.SupplierID), "Vui lòng chọn nhà cung cấp cho mặt hàng");
            if (string.IsNullOrWhiteSpace(model.Unit))
                ModelState.AddModelError(nameof(model.Unit), "Vui lòng nhập đơn vị tính cho mặt hàng");
            if (model.Price <= 0)
                ModelState.AddModelError(nameof(model.Price), "Vui lòng nhập giá cho mặt hàng");

            //Xử lý cho ảnh
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
                ViewBag.isEdit = model.ProductID == 0 ? false : true;
                return View("Edit", model);
            }

            if (model.ProductID == 0)
            {
                CommonDataService.ProductDB.Add(model);
            }
            else
            {
                CommonDataService.ProductDB.Update(model);
            }
            return Redirect("Index");
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto)
        {
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, @"images\products", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }

            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError(nameof(model.Description), "Vui lòng nhập mô tả ảnh");
            if (!model.DisplayOrder.HasValue || model.DisplayOrder <= 0)        
                ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị không được bé hơn hoặc bằng 0");
           

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.PhotoID == 0 ? "Bổ sung ảnh" : "Chỉnh sửa ảnh";
                return View("Photo", model);
            }


            if (model.PhotoID == 0)
            {
                CommonDataService.ProductDB.AddPhoto(model);
            }
            else
            {
                CommonDataService.ProductDB.UpdatePhoto(model);
            }
            return RedirectToAction("Edit", new { id = model.ProductID });

        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute model)
        {
            if (string.IsNullOrWhiteSpace(model.AttributeName))
                ModelState.AddModelError(nameof(model.AttributeName), "Tên không được để trống");
            if (string.IsNullOrWhiteSpace(model.AttributeValue))
                ModelState.AddModelError(nameof(model.AttributeValue), "Giá trị thuộc tính không được để trống");
            if (!model.DisplayOrder.HasValue || model.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(model.DisplayOrder), "Thứ tự hiển thị không được bé hơn hoặc bằng 0");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.AttributeID == 0 ? "Bổ sung thuộc tính" : "Chỉnh sửa thông tin thuộc tính";
                return View("Attribute", model);
            }

            if (model.AttributeID == 0)
            {
                CommonDataService.ProductDB.AddAttribute(model);
            }
            else
            {
                CommonDataService.ProductDB.UpdateAttribute(model);
            }
            return RedirectToAction("Edit", new { id = model.ProductID });
        }
    }
}