using pos_system.pos.BLL.Utilities;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace pos_system.pos.BLL.Services
{
    internal class UserService
    {
        private readonly UserRepository _repository = new UserRepository();

        public Employee AuthenticateUser(string username, string password)
        {
            var hashedPassword = HashHelper.ComputeSqlCompatibleHash(password);
            Debug.WriteLine("User Service " + username + " / " + hashedPassword);
            return _repository.ValidateLogin(username, hashedPassword);
        }

        public List<Employee> GetFilteredEmployees(int roleId, string status)
        {
            return _repository.GetFilteredEmployees(roleId, status);
        }

        //public void SaveEmployee(Employee employee)
        //{
        //    //ValidateEmployee(employee);
        //    _repository.SaveEmployee(employee);
        //}

        //private void ValidateEmployee(Employee employee)
        //{
        //    if (string.IsNullOrWhiteSpace(employee.nic) || !Regex.IsMatch(employee.nic, @"^\d{12}[Vv]?$"))
        //        throw new ArgumentException("Invalid NIC format");

        //    if (!string.IsNullOrEmpty(employee.contactNo) && !Regex.IsMatch(employee.contactNo, @"^\d{10}$"))
        //        throw new ArgumentException("Invalid contact number");
        //}
    }
}
