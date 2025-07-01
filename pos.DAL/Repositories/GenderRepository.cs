using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System.Collections.Generic;
using System.Data;

namespace pos_system.pos.DAL.Repositories
{
    internal class GenderRepository
    {
        public List<Gender> GetAllGenders()
        {
            var genders = new List<Gender>();
            const string query = "SELECT Gender_ID, GenderName FROM GenderBaseGroup";

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genders.Add(new Gender
                            {
                                Gender_ID = reader.GetInt32(reader.GetOrdinal("Gender_ID")),
                                GenderName = reader.GetString(reader.GetOrdinal("GenderName"))
                            });
                        }
                    }
                }
            }
            return genders;
        }

        public bool GenderExists(int genderId)
        {
            const string query = "SELECT COUNT(1) FROM GenderBaseGroup WHERE Gender_ID = @GenderId";

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GenderId", genderId);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }
    }
}