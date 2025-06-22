using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace pos_system.pos.DAL.Repositories
{
    internal class CategoryRepository
    {
        public List<Category> GetAllCategorie()
        {
            var categories = new List<Category>();
            string query = "SELECT Category_ID, categoryName FROM Category ORDER BY categoryName";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Category_ID = reader.GetInt32(reader.GetOrdinal("Category_ID")),
                            categoryName = reader.GetString(reader.GetOrdinal("categoryName"))
                        });
                    }
                }
            }
            return categories;
        }

        public DataTable GetAll()
        {
            string query = "SELECT Category_ID, categoryName FROM Category";
            return DbHelper.GetDataTable(query, CommandType.Text);
        }

        public bool Add(string name)
        {
            string query = "INSERT INTO Category (categoryName) VALUES (@name)";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@name", name)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Update(int id, string name)
        {
            string query = "UPDATE Category SET categoryName = @name WHERE Category_ID = @id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@name", name),
                new SqlParameter("@id", id)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Delete(int id)
        {
            string query = "DELETE FROM Category WHERE Category_ID = @id";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@id", id)
            };
            return DbHelper.ExecuteNonQuery(query, CommandType.Text, parameters) > 0;
        }

        public bool Exists(string name, int? id = null)
        {
            string query = "SELECT COUNT(*) FROM Category WHERE categoryName = @name";
            if (id.HasValue)
                query += " AND Category_ID != @id";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@name", name)
            };

            if (id.HasValue)
                parameters.Add(new SqlParameter("@id", id.Value));

            var result = (int)DbHelper.ExecuteScalar(query, CommandType.Text, parameters.ToArray());
            return result > 0;
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

        public bool UpdateCategorySizes(int categoryId, List<int> sizeIds)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Delete existing mappings
                        string deleteQuery = "DELETE FROM CategorySize WHERE Category_ID = @categoryId";
                        using (var cmd = new SqlCommand(deleteQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@categoryId", categoryId);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert new mappings
                        if (sizeIds.Count > 0)
                        {
                            var insertQuery = new StringBuilder("INSERT INTO CategorySize (Category_ID, Size_ID) VALUES ");
                            var parameters = new List<SqlParameter>();

                            for (int i = 0; i < sizeIds.Count; i++)
                            {
                                insertQuery.Append($"(@categoryId, @sizeId{i}),");
                                parameters.Add(new SqlParameter($"@sizeId{i}", sizeIds[i]));
                            }

                            insertQuery.Length--; // Remove last comma

                            using (var cmd = new SqlCommand(insertQuery.ToString(), conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                                cmd.Parameters.AddRange(parameters.ToArray());
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public bool CategoryExists(int categoryId)
        {
            string query = "SELECT COUNT(*) FROM Category WHERE Category_ID = @categoryId";
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
