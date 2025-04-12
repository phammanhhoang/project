using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;
using System.Security.Claims;

namespace SV21T1020873.Shop.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string email, string password, string confirmPassword, string displayName, string phone, string province, string contactName)
        {
            // Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu và xác nhận mật khẩu không khớp.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(province) || string.IsNullOrWhiteSpace(contactName))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin khách hàng.");
                return View();
            }

            // Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = UserAccountService.GetUserProfile(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
                return View();
            }

            // Đăng ký người dùng mới
            bool isRegistered = UserAccountService.RegisterCustomer(email, password, displayName, phone, province, contactName);
            if (isRegistered)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đầy đủ tên và mật khẩu");
                return View();
            }

            //Kiểm tra xem username và password (của Employee) có đúng hay không?
            var userAccount = UserAccountService.Authorize(UserTypes.Customer, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Đăng nhập thành công

            //1. Tạo ra thông tin "định danh" người dùng
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var username = User.FindFirstValue(nameof(WebUserData.UserName));

            // Kiểm tra mật khẩu mới và xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            if (string.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("", "Không tìm thấy tên người dùng.");
                return View();
            }

            // Xác thực người dùng
            var userType = UserTypes.Customer;
            var userAccount = UserAccountService.Authorize(userType, username, oldPassword);
            if (userAccount == null)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng.");
                return View();
            }

            // Thay đổi mật khẩu
            bool isPasswordChanged = UserAccountService.ChangePassword(userType, username, oldPassword, newPassword);
            if (isPasswordChanged)
            {
                ViewBag.Message = "Đổi mật khẩu thành công!";
                return View();
                /*return View("Login");*/
            }
            else
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đổi mật khẩu. Vui lòng thử lại.");
                return View();
            }
        }

        public IActionResult ChangeProfile(int id)
        {
            var data = CommonDataService.GetCustomer(id);
            if (data == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(data);

        }
        [HttpPost]
        public async Task<IActionResult> ChangeProfile(Customer data)
        {
            if (string.IsNullOrWhiteSpace(nameof(data.CustomerName)))
            {
                ModelState.AddModelError(nameof(data.CustomerName), "Tên không được để rỗng");
            }
            if (string.IsNullOrWhiteSpace(nameof(data.Email)))
            {
                ModelState.AddModelError(nameof(data.Email), "Email không được để rỗng");
            }
            if (string.IsNullOrWhiteSpace(nameof(data.Phone)))
            {
                ModelState.AddModelError(nameof(data.Phone), "Phone không được để rỗng");
            }
            if (string.IsNullOrWhiteSpace(nameof(data.Province)))
            {
                ModelState.AddModelError(nameof(data.Province), "Tỉnh thành phải chọn");
            }
            if (string.IsNullOrWhiteSpace(nameof(data.Address)))
            {
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được để rỗng");
            }
            if (ModelState.IsValid == false)
            {
                return View("ChangeProfile", data);
            }

            bool result = CommonDataService.UpdateCustomer(data);
            if (result == false)
            {
                ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                return View("ChangeProfile", data);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenined()
        {
            return View();
        }
    }
}
