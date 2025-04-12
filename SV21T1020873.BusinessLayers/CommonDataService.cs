using SV21T1020873.DataLayers;
using SV21T1020873.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020873.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng để giao tiếp với các đối tượng dữ liệu
    /// liên quan đến: Khách hàng, Nhà cung cấp, Người giao hàng, Nhân viên và Loại hàng
    /// </summary>
    public static class CommonDataService
    {
        /// <summary>
        /// 
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;

            ProvinceDB = new ProvinceDAL(connectionString);
            CustomerDB = new CustomerDAL(connectionString);
            SupplierDB = new SupplierDAL(connectionString);
            ShipperDB = new ShipperDAL(connectionString);
            EmployeeDB = new EmployeeDAL(connectionString);
            CategoryDB = new CategoryDAL(connectionString);
            ProductDB = new ProductDAL(connectionString);
        }
        /// <summary>
        /// Tỉnh thành
        /// </summary>
        public static ProvinceDAL ProvinceDB { get; private set; }
        /// <summary>
        /// Khách hàng
        /// </summary>
        public static CustomerDAL CustomerDB { get; private set; }
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public static SupplierDAL SupplierDB { get; private set; }
        /// <summary>
        /// Người giao hàng
        /// </summary>
        public static ShipperDAL ShipperDB { get; private set; }
        /// <summary>
        /// Nhân viên
        /// </summary>
        public static EmployeeDAL EmployeeDB { get; private set; }
        /// <summary>
        /// Loại hàng
        /// </summary>
        public static CategoryDAL CategoryDB { get; private set; }
        /// <summary>
        /// Mặt hàng
        /// </summary>
        public static ProductDAL ProductDB { get; private set; }
        public static List<Province> ListOfProvince()
        {
            return ProvinceDB.List();
        }
        public static List<Province> ListOfProvinces()
        {
            return ProvinceDB.List().ToList();
        }
        public static List<Supplier> ListOfSupplier(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = SupplierDB.Count(searchValue);
            return SupplierDB.List(page, pageSize, searchValue);
        }
        public static List<Supplier> ListOfSupplier(string searchValue = "")
        {
            return SupplierDB.List(1, 0, searchValue);
        }
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = CategoryDB.Count(searchValue);
            return CategoryDB.List(page, pageSize, searchValue);
        }
        public static List<Category> ListOfCategories(string searchValue = "")
        {
            return CategoryDB.List(1, 0, searchValue);
        }
        public static List<Product> ListProducts(string searchValue = "")
        {
            return ProductDB.List(1, 0, searchValue, 0, 0, 0, 0);
        }
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = ProductDB.Count(searchValue, categoryId, supplierId, minPrice, maxPrice);
            return ProductDB.List(page, pageSize, searchValue, categoryId, supplierId, minPrice, maxPrice);
        }
        public static bool DeleteProduct(int productID)
        {
            return ProductDB.Delete(productID);
        }
        public static Product? GetProduct(int productId)
        {
            return ProductDB.Get(productId);
        }
        public static Customer? GetCustomer(int id)
        {
            return CustomerDB.Get(id);
        }
        public static int AddCustomer(Customer data)
        {
            return CustomerDB.Add(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return CustomerDB.Update(data);
        }
        public static bool DeleteCustomer(int id)
        {
            return CustomerDB.Delete(id);
        }
        public static bool IsUsedCustomer(int id)
        {
            return CustomerDB.InUsed(id);
        }
    }
}
