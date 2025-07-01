using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

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

                if (result == null)
                {
                    Debug.WriteLine("[Database Error] Failed to retrieve data");
                    return new DataTable();
                }

                Debug.WriteLine($"[Data Retrieved] {result.Rows.Count} rows found");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Database Error] {ex.Message}");
                return new DataTable();
            }
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
                    picture = @Picture,
                    email = @Email
                  WHERE Employee_ID = @ID" :
                @"INSERT INTO Employee 
                (firstName, lastName, nic, userName, password, 
                 address, contactNo, status, Role_ID, picture, email)
                VALUES
                (@FirstName, @LastName, @NIC, @UserName, @Password, 
                 @Address, @ContactNo, @Status, @RoleID, @Picture, @Email)";

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
                new("@Picture", employee.picture ?? (object)DBNull.Value),
                new("@Email", employee.email ?? (object)DBNull.Value)  // New email parameter
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

        public bool CheckExisting(string nic, string username, string contactNo, string email)
        {
            string query = @"SELECT COUNT(*) FROM Employee 
                            WHERE nic = @NIC OR userName = @User 
                            OR contactNo = @Contact OR email = @Email";
            var parameters = new SqlParameter[] {
                new("@NIC", nic),
                new("@User", username),
                new("@Contact", contactNo),
                new("@Email", email)  // Added email check
            };

            using var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);
            return Convert.ToInt32(dt.Rows[0][0]) > 0;
        }
    }
}