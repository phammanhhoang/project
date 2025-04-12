using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        private const string CUSTOMER_SEARCH_INPUT = "CustomerSearchInput";
        public IActionResult Index()
        {
            //kiểm tra trong session có điều kiện tìm kiếm được lưu lại hay không
            //nếu có thì sử dụng, nếu không thì tạo mới điều kiện tìm kiếm
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH_INPUT);
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
        /// Tìm kiếm, hiển thị khách hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchInput input)
        {
            PaginationSearchOutput<Customer> model = new PaginationSearchOutput<Customer>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = CommonDataService.CustomerDB.Count(input.SearchValue ?? ""),
                Data = CommonDataService.CustomerDB.List(input.Page, input.PageSize, input.SearchValue ?? "")
            };
            //lưu lại điều kiện tìm kiếm vào session
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH_INPUT, input);
            return View(model);
        }
        /// <summary>
        /// Hiển thị giao diện để nhập khách hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung khách hàng";
            //Tạo một đối tượng để lưu thông tin của khách hàng mới
            Customer model = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit", model);
        }
        /// <summary>
        /// Hiển thị giao diện để cập nhật thông tin của khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            //Lấy thông tin của khách hàng cần cập nhật
            Customer? model = CommonDataService.CustomerDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        /// <summary>
        /// Lưu dữ liệu khách hàng vào CSDL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveData(Customer model)
        {
            try
            {
                //TODO: Kiểm tra và xử lý dữ liệu đầu vào để đảm bảo tính hợp lệ
                ViewBag.Title = model.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhập thông tin khách hàng";

                if (string.IsNullOrWhiteSpace(model.CustomerName))
                    ModelState.AddModelError(nameof(model.CustomerName), "Vui lòng nhập tên khách hàng");
                if (string.IsNullOrWhiteSpace(model.ContactName))
                    ModelState.AddModelError(nameof(model.ContactName), "Vui lòng nhập tên giao dịch của khách hàng");
                if (string.IsNullOrWhiteSpace(model.Phone))
                    ModelState.AddModelError(nameof(model.Phone), "Vui lòng nhập số điện thoại của khách hàng");


                if (string.IsNullOrWhiteSpace(model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Vui lòng nhập email của khách hàng");
                else
                    if (CommonDataService.CustomerDB.ExistsEmail(model.CustomerID, model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");


                if (string.IsNullOrWhiteSpace(model.Address))
                    ModelState.AddModelError(nameof(model.Address), "Vui lòng nhập địa chỉ của khách hàng");
                if (string.IsNullOrWhiteSpace(model.Province))
                    ModelState.AddModelError(nameof(model.Province), "Vui lòng nhập tỉnh thành của khách hàng");


                //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại trường hợp "lỗi" nào không?
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }

                if (model.CustomerID == 0)
                {
                    CommonDataService.CustomerDB.Add(model);
                }
                else
                {
                    CommonDataService.CustomerDB.Update(model);
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
        /// Hiển thị và xóa khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //Xóa khách hàng
            if (Request.Method == "POST")
            {
                CommonDataService.CustomerDB.Delete(id);
                return RedirectToAction("Index");
            }

            //Hiển thị thông tin khách hàng cần xóa
            Customer? model = CommonDataService.CustomerDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}