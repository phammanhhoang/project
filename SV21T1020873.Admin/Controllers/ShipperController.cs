using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.Admin.Models;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020873.Admin.Controllers
{
    //[Authorize(Roles = $"{WebUserRoles.Administrator},{WebUserRoles.Employee}")]
    public class ShipperController : Controller
    {
        private const string SHIPPER_SEARCH_INPUT = "ShipperSearchInput";
        public IActionResult Index()
        {
            //kiểm tra trong session có điều kiện tìm kiếm được lưu lại hay không
            //nếu có thì sử dụng, nếu không thì tạo mới điều kiện tìm kiếm
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_INPUT);
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
        /// Tìm kiếm, hiển thị người giao hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        public IActionResult Search(PaginationSearchInput input)
        {
            PaginationSearchOutput<Shipper> model = new PaginationSearchOutput<Shipper>()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = CommonDataService.ShipperDB.Count(input.SearchValue ?? ""),
                Data = CommonDataService.ShipperDB.List(input.Page, input.PageSize, input.SearchValue ?? "")
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_INPUT, input);
            return View(model);
        }
        /// <summary>
        /// Hiển thị giao diện để nhập người giao hàng mới
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung người giao hàng";
            //Tạo một đối tượng để lưu thông tin của người giao hàng mới
            Shipper model = new Shipper()
            {
                ShipperID = 0,
            };
            return View("Edit", model);
        }
        /// <summary>
        /// Hiển thị giao diện để cập nhật thông tin của người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin người giao hàng";
            //Lấy thông tin của người giao hàng cần cập nhật
            Shipper? model = CommonDataService.ShipperDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
        /// <summary>
        /// Lưu dữ liệu người giao hàng vào CSDL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveData(Shipper model)
        {
            try
            {
                //TODO: Kiểm tra và xử lý dữ liệu đầu vào để đảm bảo tính hợp lệ
                ViewBag.Title = model.ShipperID == 0 ? "Bổ sung người giao hàng" : "Cập nhập thông tin người giao hàng";

            if (string.IsNullOrWhiteSpace(model.ShipperName))
                ModelState.AddModelError(nameof(model.ShipperName), "Vui lòng nhập tên người giao hàng");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError(nameof(model.Phone), "Vui lòng nhập số điện thoại của người giao hàng");
            else
                    if (CommonDataService.ShipperDB.ExistsEmail(model.ShipperID, model.Phone))
                ModelState.AddModelError(nameof(model.Phone), "Số điện thoại bị trùng");


                //Dựa vào thuộc tính IsValid của ModelState để biết có tồn tại trường hợp "lỗi" nào không?
                if (!ModelState.IsValid)
                {
                    return View("Edit", model);
                }

                if (model.ShipperID == 0)
                {
                    CommonDataService.ShipperDB.Add(model);
                }
                else
                {
                    CommonDataService.ShipperDB.Update(model);
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
        /// Hiển thị và xóa người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Delete(int id = 0)
        {
            //Xóa người giao hàng
            if (Request.Method == "POST")
            {
                CommonDataService.ShipperDB.Delete(id);
                return RedirectToAction("Index");
            }

            //Hiển thị thông tin người giao hàng cần xóa
            Shipper? model = CommonDataService.ShipperDB.Get(id);
            if (model == null)
                return RedirectToAction("Index");

            return View(model);
        }
    }
}