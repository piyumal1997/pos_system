using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

//using pos_system.pos.Database.StoredProcedures;

namespace pos_system.pos.DAL.Repositories
{
    internal class UserRepository
    {
        public Employee? ValidateLogin(string username, string passwordHash)
        {
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("sp_ValidateLogin", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                Debug.WriteLine("UserRepository Data - " + username + "  " + passwordHash);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    Debug.WriteLine("ValidateLogin in UserRepo - " + reader);
                    return reader.Read() ? new Employee
                    {
                        Employee_ID = (int)reader["Employee_ID"],
                        firstName = reader["firstName"].ToString() ?? string.Empty,
                        lastName = reader["lastName"].ToString() ?? string.Empty,
                        userName = reader["userName"].ToString() ?? string.Empty,
                        Role_ID = (int)reader["Role_ID"]
                    } : null;
                }
            }
        }

        public List<Employee> GetFilteredEmployees(int roleId, string status)
        {
            const string query = @"
                SELECT e.*, r.RoleName 
                FROM Employee e
                INNER JOIN Role r ON e.Role_ID = r.Role_ID
                WHERE (@roleId = 0 OR e.Role_ID = @roleId)
                AND (@status = 'All' OR e.status = @status)";

            var parameters = new SqlParameter[] {
                new("@roleId", roleId),
                new("@status", status)
            };

            var dt = DbHelper.GetDataTable(query, CommandType.Text, parameters);

            return dt.AsEnumerable().Select(row => new Employee
            {
                Employee_ID = Convert.ToInt32(row["Employee_ID"]),
                firstName = row["firstName"].ToString(),
                lastName = row["lastName"].ToString(),
                nic = row["nic"].ToString(),
                userName = row["userName"].ToString(),
                status = row["status"].ToString(),
                Role_ID = Convert.ToInt32(row["Role_ID"]),
                contactNo = row["contactNo"].ToString(),
                address = row["address"].ToString(),
            }).ToList();
        }

    }
}
