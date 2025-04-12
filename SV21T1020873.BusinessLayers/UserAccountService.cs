using SV21T1020873.DataLayers;
using SV21T1020873.DomainModels;

namespace SV21T1020873.BusinessLayers
{
    /// <summary>
    /// Các dịch vụ liên quan đến tài khoản
    /// </summary>
    public static class UserAccountService
    {
        public static EmployeeUserAccountDAL EmployeeAccountDB;
        public static CustomerUserAccountDAL CustomerAccountDB;
        /// <summary>
        /// 
        /// </summary>
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;

            EmployeeAccountDB = new EmployeeUserAccountDAL(connectionString);
            CustomerAccountDB = new CustomerUserAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            var userAccount = EmployeeAccountDB.Authentiate(username, password);
            Console.WriteLine($"Authorize - Photo: {userAccount?.Photo}");

            if (userTypes == UserTypes.Employee)
                return EmployeeAccountDB.Authentiate(username, password);
            else if (userTypes == UserTypes.Customer)
                return CustomerAccountDB.Authorize(username, password);
            return null;
        }
        /// <summary>
        /// Tài khoản của nhân viên
        /// </summary>
        public static bool ChangePassword(UserTypes userType, string username, string oldpassword, string newpassword)
        {
            if (userType == UserTypes.Employee)
                return EmployeeAccountDB.ChangePassword(username,  newpassword);
            else
                return CustomerAccountDB.ChangePassword(username,  newpassword);
        }
        public static UserAccount? GetUserProfile(string username)
        {

            return CustomerAccountDB.GetUserProfile(username);
        }
        public static bool RegisterCustomer(string email, string password, string displayName, string phone, string province, string contactName)
        {
            return CustomerAccountDB.RegisterCustomer(email, password, displayName, phone, province, contactName);
        }
        public static UserAccount? AuthorizeCustomer(string userName, string password)
        {
            return CustomerAccountDB.Authorize(userName, password);
        }
    }

    public enum UserTypes
    {
        Employee,
        Customer
    }
}