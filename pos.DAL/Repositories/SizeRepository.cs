using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Linq;

namespace pos_system.pos.DAL.Repositories
{
    internal class SizeRepository
    {
        public List<CategorySize> GetAll()
        {
            var sizes = new List<CategorySize>();
            string query = "SELECT Size_ID, SizeLabel, SizeType FROM Size ORDER BY SizeType, SizeLabel";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sizes.Add(new CategorySize
                        {
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            SizeType = reader.GetString(reader.GetOrdinal("SizeType"))
                        });
                    }
                }
            }
            return sizes;
        }

        public List<CategorySize> GetSizesByCategory(int categoryId)
        {
            var sizes = new List<CategorySize>();
            string query = @"
                SELECT s.Size_ID, s.SizeLabel, s.SizeType 
                FROM CategorySize cs
                JOIN Size s ON cs.Size_ID = s.Size_ID
                WHERE cs.Category_ID = @categoryId
                ORDER BY s.SizeType, s.SizeLabel";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sizes.Add(new CategorySize
                        {
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            SizeType = reader.GetString(reader.GetOrdinal("SizeType"))
                        });
                    }
                }
            }
            return sizes;
        }
    }
}
