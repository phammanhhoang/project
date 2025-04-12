using Microsoft.AspNetCore.Mvc.Rendering;
using SV21T1020873.BusinessLayers;
using SV21T1020873.DomainModels;

namespace SV21T1020873.Admin
{
    public static class SelectListHeper
    {
        /// <summary>
        /// Danh sách tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn Tỉnh/Thành --",
            });
            foreach (var p in CommonDataService.ProvinceDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = p.ProvinceName,
                    Text = p.ProvinceName,
                });
            }
            return list;
        }
        public static List<SelectListItem> ProductList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "Chọn sản phẩm"
            });
            foreach (var p in CommonDataService.ProductDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = p.ProductID.ToString(),
                    Text = p.ProductName,
                });
            }
            return list;
        }
        public static List<SelectListItem> Categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn loại hàng --",
            });
            foreach (var p in CommonDataService.CategoryDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = p.CategoryID.ToString(),
                    Text = p.CategoryName,
                });
            }
            return list;
        }
        public static List<SelectListItem> Suppliers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn nhà cung cấp --",
            });
            foreach (var p in CommonDataService.SupplierDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = p.SupplierID.ToString(),
                    Text = p.SupplierName,
                });
            }
            return list;
        }
        public static List<SelectListItem> Status()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Trạng thái --",
            });
            list.Add(new SelectListItem()
            {
                Value = "1",
                Text = "Đơn hàng mới(chờ duyệt)",
            });
            list.Add(new SelectListItem()
            {
                Value = "2",
                Text = "Đơn hàng đã duyệt (chờ chuyển hàng)",
            });
            list.Add(new SelectListItem()
            {
                Value = "3",
                Text = "Đơn hàng đang được giao",
            });
            list.Add(new SelectListItem()
            {
                Value = "4",
                Text = "Đơn hàng đã hoàn tất thành công",
            });
            list.Add(new SelectListItem()
            {
                Value = "-1",
                Text = "Đơn hàng bị hủy",
            });
            list.Add(new SelectListItem()
            {
                Value = "-2",
                Text = "Đơn hàng bị từ chối",
            });
            return list;
        }
        public static List<SelectListItem> Shippers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Chọn người giao hàng --",
            });

            foreach (var item in CommonDataService.ShipperDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ShipperID.ToString(),
                    Text = item.ShipperName,
                });
            }
            return list;
        }
        public static List<SelectListItem> Customers()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn khách hàng --",
            });
            foreach (var item in CommonDataService.CustomerDB.List())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName,
                });
            }
            return list;
        }
        public static List<SelectListItem> RoleNames()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "employee",
                Text = "Nhân viên"
            });

            list.Add(new SelectListItem()
            {
                Value = "admin",
                Text = "Quản trị hệ thống"
            });

            list.Add(new SelectListItem()
            {
                Value = "employee,admin",
                Text = "Nhân viên, Quản trị hệ thống"
            });
            return list;
        }

    }
}
