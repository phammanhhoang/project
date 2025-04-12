using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class EmployeeController : Controller
    {
        private const string EMPLOYEE_SEARCH_INPUT = "EmployeeSearchInput";
        public IActionResult Index()
        {
            //kiểm tra trong session có điều kiện tìm kiếm được lưu lại hay không
            //nếu có thì sử dụng, nếu không thì tạo mới điều kiện tìm kiếm
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_INPUT);
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
        /// Tìm kiếm, hiển thị nhân viên dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchInput input)
        {
            PaginationSearchOutput<Employee> model = new PaginationSearchOutput<Employee>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = CommonDataService.EmployeeDB.Count(input.SearchValue ?? ""),
                Data = CommonDataService.EmployeeDB.List(input.Page, input.PageSize, input.SearchValue ?? "")
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_INPUT, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            Employee model = new Employee()
            {
                EmployeeID = 0,
                Photo = "nophoto.png",
                IsWorking = true
            };
            return View("Edit", model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            Employee? model = CommonDataService.EmployeeDB.Get(id);
            if (model == null)
                return Redirect("Index");
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveData(Employee model, string birthday, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = model.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhập thông tin nhân viên";

                if (string.IsNullOrWhiteSpace(model.FullName))
                    ModelState.AddModelError(nameof(model.FullName), "Vui lòng nhập tên của nhân viên");
                if (string.IsNullOrWhiteSpace(model.Address))
                    ModelState.AddModelError(nameof(model.Address), "Vui lòng nhập địa chỉ của nhân viên");
                if (string.IsNullOrWhiteSpace(model.Phone))
                    ModelState.AddModelError(nameof(model.Phone), "Vui lòng nhập điện thoại của nhân viên");
                if (string.IsNullOrWhiteSpace(model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Vui lòng nhập email của nhân viên");
                else if (CommonDataService.EmployeeDB.ExistsEmail(model.EmployeeID, model.Email))
                    ModelState.AddModelError(nameof(model.Email), "Email bị trùng");



                //Xử lý cho ngày sinh
                DateTime? d = birthday.ToDateTime();
                if (d == null)
                    ModelState.AddModelError(nameof(model.BirthDate), "Ngày sinh không hợp lệ");
                else
                    model.BirthDate = d.Value;

                //Xử lý cho ảnh
                if (uploadPhoto != null)
                {
                    string fileName = $"{DateTime.Now.Ticks}-{uploadPhoto.FileName}";
                    string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\employees", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadPhoto.CopyTo(stream);
                    }
                    model.Photo = fileName;
                }

                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }

                if (model.EmployeeID == 0)
                {
                    CommonDataService.EmployeeDB.Add(model);
                }
                else
                {
                    CommonDataService.EmployeeDB.Update(model);
                }
                return Redirect("Index");
            }
                catch //(Exception ex)
                {
                    //Ghi log của lỗi trong exception
                    ModelState.AddModelError("Error", "Hệ thống tạm thời quá tải hoặc gián đoạn. Vui lòng thử lại sau");
                    return View("Edit", model);
                }
            }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.EmployeeDB.Delete(id);
                return RedirectToAction("Index");
            }

            Employee? model = CommonDataService.EmployeeDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }

    }
}
