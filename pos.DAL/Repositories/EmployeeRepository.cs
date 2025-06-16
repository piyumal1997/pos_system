using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pos_system.pos.Models;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace pos_system.pos.DAL.Repositories
{
    public class EmployeeRepository
    {
        public DataTable GetAllEmployees()
        {
            string query = @"SELECT e.*, r.RoleName 
                           FROM Employee e
                           INNER JOIN Role r ON e.Role_ID = r.Role_ID";

            Debug.WriteLine($"[SQL Query]: {query}");

            try
            {
                DataTable result = DbHelper.GetDataTable(query, CommandType.Text);

                // Log data retrieval status
                if (result == null)
                {
                    Debug.WriteLine("[Database Error] Failed to retrieve data");
                    return new DataTable();
                }

                // Log row count and sample data
                Debug.WriteLine($"[Data Retrieved] {result.Rows.Count} rows found");

                if (result.Rows.Count > 0)
                {
                    Debug.WriteLine("[First Row Sample]:");
                    Debug.WriteLine($"ID: {result.Rows[0]["Employee_ID"]}");
                    Debug.WriteLine($"Name: {result.Rows[0]["firstName"]} {result.Rows[0]["lastName"]}");
                    Debug.WriteLine($"Role: {result.Rows[0]["RoleName"]}");
                    Debug.WriteLine($"Status: {result.Rows[0]["status"]}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Database Error] {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return new DataTable();
            }

            //return DbHelper.GetDataTable(query, CommandType.Text);
        }

        public int AddUpdateEmployee(Employee employee, bool isUpdate = false)
        {
            string query = isUpdate ?
                @"UPDATE Employee SET 
                    firstName = @FirstName,
                    lastName = @LastName,
                    address = @Address,
                    contactNo = @ContactNo,
                    status = @Status,
                    Role_ID = @RoleID,
                    password = @Password,
                    picture = @Picture
                  WHERE Employee_ID = @ID" :
                @"INSERT INTO Employee 
                (firstName, lastName, nic, userName, password, address, contactNo, status, Role_ID, picture)
                VALUES
                (@FirstName, @LastName, @NIC, @UserName, @Password, @Address, @ContactNo, @Status, @RoleID, @Picture)";

            var parameters = new SqlParameter[] {
                new("@FirstName", employee.firstName),
                new("@LastName", employee.lastName),
                new("@NIC", employee.nic),
                new("@UserName", employee.userName),
                new("@Password", employee.password),
                new("@Address", employee.address ?? (object)DBNull.Value),
                new("@ContactNo", employee.contactNo),
                new("@Status", employee.status),
                new("@RoleID", employee.Role_ID),
                new("@Picture", employee.picture ?? (object)DBNull.Value)
            };

            if (isUpdate)
                parameters = parameters.Append(new SqlParameter("@ID", employee.Employee_ID)).ToArray();

            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
        }

        public int ToggleEmployeeStatus(int employeeId)
        {
            string query = @"UPDATE Employee SET 
                            status = CASE WHEN status = 'Active' 
                                          THEN 'Inactive' ELSE 'Active' END
                            WHERE Employee_ID = @ID";
            return DbHelper.ExecuteNonQuery(query, CommandType.Text,
                new SqlParameter("@ID", employeeId));
        }

        public bool CheckExisting(string nic, string username, string contactNo)
        {
            string query = @"SELECT COUNT(*) FROM Employee 
                            WHERE nic = @NIC OR userName = @User 
                            OR contactNo = @Contact";
            var parameters = new SqlParameter[] {
                new("@NIC", nic),
                new("@User", username),
                new("@Contact", contactNo)
            };

            using var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }
    }
}
