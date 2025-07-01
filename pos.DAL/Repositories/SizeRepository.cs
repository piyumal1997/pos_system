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

        public CategorySize GetSizeById(int sizeId)
        {
            string query = "SELECT Size_ID, SizeLabel, SizeType FROM Size WHERE Size_ID = @sizeId";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@sizeId", sizeId);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new CategorySize
                        {
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            SizeType = reader.GetString(reader.GetOrdinal("SizeType"))
                        };
                    }
                }
            }
            return null; // Size not found
        }


        //public List<Sizes> GetAlls()
        //{
        //    var sizes = new List<Sizes>();
        //    const string query = "SELECT Size_ID, SizeLabel, SizeType FROM Size ORDER BY SizeType, SizeLabel";

        //    using (var conn = DbHelper.GetConnection())
        //    using (var cmd = new SqlCommand(query, conn))
        //    {
        //        conn.Open();
        //        using (var reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                sizes.Add(new Sizes
        //                {
        //                    Size_ID = reader.GetInt32(0),
        //                    SizeLabel = reader.GetString(1),
        //                    SizeType = reader.GetString(2)
        //                });
        //            }
        //        }
        //    }
        //    return sizes;
        //}

        public Sizes GetById(int id)
        {
            const string query = "SELECT Size_ID, SizeLabel, SizeType FROM Size WHERE Size_ID = @id";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Sizes
                        {
                            Size_ID = reader.GetInt32(0),
                            SizeLabel = reader.GetString(1),
                            SizeType = reader.GetString(2)
                        };
                    }
                }
            }
            return null;
        }

        public bool Insert(Sizes size)
        {
            const string query = @"INSERT INTO Size (SizeLabel, SizeType) 
                              VALUES (@label, @type);
                              SELECT SCOPE_IDENTITY();";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@label", size.SizeLabel);
                cmd.Parameters.AddWithValue("@type", size.SizeType);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToInt32(result) > 0;
            }
        }

        public bool Update(Sizes size)
        {
            const string query = @"UPDATE Size SET 
                             SizeLabel = @label, 
                             SizeType = @type 
                             WHERE Size_ID = @id";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@label", size.SizeLabel);
                cmd.Parameters.AddWithValue("@type", size.SizeType);
                cmd.Parameters.AddWithValue("@id", size.Size_ID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int id)
        {
            // Check if size is used in any products or categories
            const string checkProductUsage = "SELECT COUNT(*) FROM ProductSize WHERE Size_ID = @id";
            const string checkCategoryUsage = "SELECT COUNT(*) FROM CategorySize WHERE Size_ID = @id";
            const string deleteQuery = "DELETE FROM Size WHERE Size_ID = @id";

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Check if size is in products
                using (var cmdCheckProduct = new SqlCommand(checkProductUsage, conn))
                {
                    cmdCheckProduct.Parameters.AddWithValue("@id", id);
                    int productCount = (int)cmdCheckProduct.ExecuteScalar();

                    if (productCount > 0)
                    {
                        MessageBox.Show("Cannot delete size - it's used in existing products");
                        return false;
                    }
                }

                // Check if size is in categories
                using (var cmdCheckCategory = new SqlCommand(checkCategoryUsage, conn))
                {
                    cmdCheckCategory.Parameters.AddWithValue("@id", id);
                    int categoryCount = (int)cmdCheckCategory.ExecuteScalar();

                    if (categoryCount > 0)
                    {
                        MessageBox.Show("Cannot delete size - it's assigned to categories");
                        return false;
                    }
                }

                // Delete if not in use
                using (var cmdDelete = new SqlCommand(deleteQuery, conn))
                {
                    cmdDelete.Parameters.AddWithValue("@id", id);
                    return cmdDelete.ExecuteNonQuery() > 0;
                }
            }
        }

        // SizeRepository.cs
        public List<Sizes> GetAlls()
        {
            var sizes = new List<Sizes>();
            const string query = "SELECT Size_ID, SizeLabel, SizeType FROM Size ORDER BY SizeType, SizeLabel";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sizes.Add(new Sizes
                        {
                            Size_ID = reader.GetInt32(0),
                            SizeLabel = reader.GetString(1),
                            SizeType = reader.GetString(2)
                        });
                    }
                }
            }
            return sizes;
        }
    }
}
