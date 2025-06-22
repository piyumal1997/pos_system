using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Linq;

namespace pos_system.pos.DAL.Repositories
{
    internal class BrandRepository
    {
        public List<Brand> GetAllBrand()
        {
            var brands = new List<Brand>();
            string query = "SELECT Brand_ID, brandName FROM Brand ORDER BY brandName";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        brands.Add(new Brand
                        {
                            Brand_ID = reader.GetInt32(reader.GetOrdinal("Brand_ID")),
                            brandName = reader.GetString(reader.GetOrdinal("brandName"))
                        });
                    }
                }
            }
            return brands;
        }
        public DataTable GetAll()
        {
            string query = "SELECT Brand_ID, brandName FROM Brand";
            return DbHelper.GetDataTable(query, CommandType.Text);
        }

        public bool Add(string name)
        {
            string query = "INSERT INTO Brand (brandName) VALUES (@name)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@name", name)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Update(int id, string name)
        {
            string query = "UPDATE Brand SET brandName = @name WHERE Brand_ID = @id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@name", name),
                new SqlParameter("@id", id)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Delete(int id)
        {
            string query = "UPDATE Brand SET IsDeleted = 1 WHERE Brand_ID = @id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Exists(string name, int? id = null)
        {
            string query = "SELECT COUNT(*) FROM Brand WHERE brandName = @name";
            if (id.HasValue)
                query += " AND Brand_ID != @id";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", name)
            };

            if (id.HasValue)
                parameters.Add(new SqlParameter("@id", id.Value));

            var result = (int)DbHelper.ExecuteScalar(query, CommandType.Text, parameters.ToArray());
            return result > 0;
        }

        public bool BrandExists(int brandId)
        {
            string query = "SELECT COUNT(*) FROM Brand WHERE Brand_ID = @brandId";
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@brandId", brandId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
