using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    //[Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        private const string SUPPLIER_SEARCH_INPUT = "SupplierSearchInput";
        public IActionResult Index()
        {
            //kiểm tra trong session có điều kiện tìm kiếm được lưu lại hay không
            //nếu có thì sử dụng, nếu không thì tạo mới điều kiện tìm kiếm
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH_INPUT);
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
        /// Tìm kiếm, hiển thị nhà cung cấp dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchInput input)
        {
            PaginationSearchOutput<Supplier> model = new PaginationSearchOutput<Supplier>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = CommonDataService.SupplierDB.Count(input.SearchValue ?? ""),
                Data = CommonDataService.SupplierDB.List(input.Page, input.PageSize, input.SearchValue ?? "")
            };
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH_INPUT, input);
            return View(model);
        }
        /// <summary>
        /// Hiển thị giao diện để nhập nhà cung cấp mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            //Tạo một đối tượng để lưu thông tin của nhà cung cấp mới
            Supplier model = new Supplier()
            {
                SupplierID = 0,
            };
            return View("Edit", model);
        }
        /// <summary>
        /// Hiển thị giao diện để cập nhật thông tin của nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            //Lấy thông tin của nhà cung cấp cần cập nhật
            Supplier? model = CommonDataService.SupplierDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        /// <summary>
        /// Lưu dữ liệu nhà cung cấp vào CSDL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveData(Supplier model)
        {
            try
            {
                //TODO: Kiểm tra và xử lý dữ liệu đầu vào để đảm bảo tính hợp lệ
                ViewBag.Title = model.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhập thông tin nhà cung cấp";

                if (string.IsNullOrWhiteSpace(model.SupplierName))
                    ModelState.AddModelError(nameof(model.SupplierName), "Vui lòng nhập tên nhà cung cấp");
                if (string.IsNullOrWhiteSpace(model.ContactName))
                    ModelState.AddModelError(nameof(model.ContactName), "Vui lòng nhập tên giao dịch của nhà cung cấp");
                if (string.IsNullOrWhiteSpace(model.Phone))
                    ModelState.AddModelError(nameof(model.Phone), "Vui lòng nhập số điện thoại của nhà cung cấp");


                if (string.IsNullOrWhiteSpace(model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Vui lòng nhập email của nhà cung cấp");
                else
                    if (CommonDataService.SupplierDB.ExistsEmail(model.SupplierID, model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");


                if (string.IsNullOrWhiteSpace(model.Address))
                    ModelState.AddModelError(nameof(model.Address), "Vui lòng nhập địa chỉ của nhà cung cấp");
                if (string.IsNullOrWhiteSpace(model.Provice))
                    ModelState.AddModelError(nameof(model.Provice), "Vui lòng nhập tỉnh thành của nhà cung cấp");


                //Kiểm tra dữ liệu đầu vào, nếu dữ liệu không hợp lệ thì đưa các thông báo lỗi
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }

                if (model.SupplierID == 0)
                {
                    CommonDataService.SupplierDB.Add(model);
                }
                else
                {
                    CommonDataService.SupplierDB.Update(model);
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
        /// Hiển thị và xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //Xóa nhà cung cấp
            if (Request.Method == "POST")
            {
                CommonDataService.SupplierDB.Delete(id);
                return RedirectToAction("Index");
            }

            //Hiển thị thông tin nhà cung cấp cần xóa
            Supplier? model = CommonDataService.SupplierDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}