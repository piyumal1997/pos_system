using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;

namespace pos_system.pos.BLL.Services
{
    internal class EmployeeService
    {
        private readonly EmployeeRepository _repo = new();

        public DataTable GetAllEmployees() => _repo.GetAllEmployees();

        public (bool success, string message) SaveEmployee(Employee employee, bool isUpdate)
        {
            if (!isUpdate && _repo.CheckExisting(employee.nic, employee.userName, employee.contactNo))
                return (false, "NIC, Username or Contact already exists");

            try
            {
                int result = _repo.AddUpdateEmployee(employee, isUpdate);
                return (result > 0, result > 0 ? "Saved successfully" : "Save failed");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"Database error: {ex.Message}");
                return (false, $"Database error: {ex.Message}");
            }
        }

        public bool ToggleStatus(int employeeId) =>
            _repo.ToggleEmployeeStatus(employeeId) > 0;
    }
}
