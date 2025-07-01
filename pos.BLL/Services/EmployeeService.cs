using Microsoft.Data.SqlClient;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace pos_system.pos.BLL.Services
{
    internal class EmployeeService
    {
        private readonly EmployeeRepository _repo = new();

        public DataTable GetAllEmployees() => _repo.GetAllEmployees();

        public (bool success, string message) SaveEmployee(Employee employee, bool isUpdate)
        {
            if (!isUpdate && _repo.CheckExisting(
                employee.nic,
                employee.userName,
                employee.contactNo,
                employee.email))
            {
                return (false, "NIC, Username, Contact or Email already exists");
            }

            try
            {
                int result = _repo.AddUpdateEmployee(employee, isUpdate);
                return (result > 0, result > 0 ? "Saved successfully" : "Save failed");
            }
            catch (Exception ex)
            {
                return (false, $"Database error: {ex.Message}");
            }
        }

        public bool ToggleStatus(int employeeId) =>
            _repo.ToggleEmployeeStatus(employeeId) > 0;
    }
}
