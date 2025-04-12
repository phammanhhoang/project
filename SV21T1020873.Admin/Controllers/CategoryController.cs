using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    //[Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        private const string CATEGORY_SEARCH_INPUT = "CategorySearchInput";
        public IActionResult Index()
        {
            //kiểm tra trong session có điều kiện tìm kiếm được lưu lại hay không
            //nếu có thì sử dụng, nếu không thì tạo mới điều kiện tìm kiếm
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH_INPUT);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = 12,
                    SearchValue = ""
                };
            }
            return View(input);

        }
        /// <summary>
        /// Tìm kiếm, hiển thị loại hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search (PaginationSearchInput input)
        {
            PaginationSearchOutput<Category> model = new PaginationSearchOutput<Category>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = CommonDataService.CategoryDB.Count(input.SearchValue ?? ""),
                Data = CommonDataService.CategoryDB.List(input.Page, input.PageSize, input.SearchValue ?? "")
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH_INPUT, input);
            return View(model);
        }
        /// <summary>
        /// Hiển thị giao diện để nhập loại hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung loại hàng";
            //Tạo một đối tượng để lưu thông tin của loại hàng mới
            Category model = new Category()
            {
                CategoryID = 0,
            };
            return View("Edit", model);
        }
        /// <summary>
        /// Hiển thị giao diện để cập nhật thông tin của loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            //Lấy thông tin của loại hàng cần cập nhật
            Category? model = CommonDataService.CategoryDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        /// <summary>
        /// Lưu dữ liệu loại hàng vào CSDL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveData(Category model)
        {
            try
            {
                //TODO: Kiểm tra và xử lý dữ liệu đầu vào để đảm bảo tính hợp lệ
                ViewBag.Title = model.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhập thông tin loại hàng";

            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError(nameof(model.CategoryName), "Vui lòng nhập tên loại hàng");
            else
                    if (CommonDataService.CategoryDB.ExistsEmail(model.CategoryID, model.CategoryName))
                ModelState.AddModelError(nameof(model.CategoryName), "Tên loại hàng bị trùng");


            if (string.IsNullOrWhiteSpace(model.Description))
                ModelState.AddModelError(nameof(model.Description), "Vui lòng nhập mô tả của loại hàng");


                //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại trường hợp "lỗi" nào không?
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }

                if (model.CategoryID == 0)
                {
                    CommonDataService.CategoryDB.Add(model);
                }
                else
                {
                    CommonDataService.CategoryDB.Update(model);
                }
                return RedirectToAction("Index");
            }
            catch //(Exception ex)
            {
                //Ghi log của lỗi trong exception
                ModelState.AddModelError("Error", "Hệ thống tạm thời quá tải hoặc gián đoạn. Vui lòng thử lại sau");
                return View("Edit", model);
            }
        }
        /// <summary>
        /// Hiển thị và xóa loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //Xóa loại hàng
            if (Request.Method == "POST")
            {
                CommonDataService.CategoryDB.Delete(id);
                return RedirectToAction("Index");
            }

            //Hiển thị thông tin loại hàng cần xóa
            Category? model = CommonDataService.CategoryDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}