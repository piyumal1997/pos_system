using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Linq;

namespace pos_system.pos.DAL.Repositories
{
    internal class ItemRepository
    {
        public Item GetItem(int itemId)
        {
            const string query = @"
        SELECT i.*, b.brandName, c.categoryName, s.SizeLabel
        FROM Item i
        INNER JOIN Brand b ON i.Brand_ID = b.Brand_ID
        INNER JOIN Category c ON i.Category_ID = c.Category_ID
        LEFT JOIN Size s ON i.Size_ID = s.Size_ID
        WHERE i.Item_ID = @ItemId";

            using (var conn = DbHelper.GetConnection())
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ItemId", itemId);
                    conn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Item
                            {
                                Item_ID = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                                barcode = reader.GetString(reader.GetOrdinal("barcode")),
                                description = reader.GetString(reader.GetOrdinal("description")),
                                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                                unitCost = reader.GetDecimal(reader.GetOrdinal("unitCost")),
                                quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                                BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                                CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                                SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                                            null : reader.GetString(reader.GetOrdinal("SizeLabel")),
                                ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                                            null : (byte[])reader["ItemImage"]
                            };
                        }
                    }
                }
            }
            return null;
        }
        public IEnumerable<Item> GetAllItems()
        {
            var items = new List<Item>();
            const string query = @"
                SELECT i.Item_ID, i.quantity, i.RetailPrice, i.unitCost, i.maxDiscount, 
                       i.description, i.barcode, i.MinStockLevel, i.IsDeleted, i.Brand_ID, 
                       i.Category_ID, i.Size_ID, i.ItemImage,
                       b.brandName, c.categoryName, s.SizeLabel
                FROM Item i
                INNER JOIN Brand b ON i.Brand_ID = b.Brand_ID
                INNER JOIN Category c ON i.Category_ID = c.Category_ID
                LEFT JOIN Size s ON i.Size_ID = s.Size_ID
                WHERE i.IsDeleted = 0";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(MapItemFromReader(reader));
                }
            }
            catch (Exception ex)
            {
                throw new DataException("Error retrieving items from database", ex);
            }

            return items;
        }

        public bool AddItem(Item item)
        {
            const string query = @"
                INSERT INTO Item (quantity, RetailPrice, unitCost, maxDiscount, description, 
                                  barcode, MinStockLevel, IsDeleted, Brand_ID, Category_ID, 
                                  Size_ID, ItemImage)
                VALUES (@quantity, @RetailPrice, @unitCost, @maxDiscount, @description, 
                        @barcode, @MinStockLevel, 0, @Brand_ID, @Category_ID, 
                        @Size_ID, @ItemImage)";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                AddItemParameters(cmd, item);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new DataException("Error adding item to database", ex);
            }
        }

        public bool UpdateItem(Item item)
        {
            const string query = @"
                UPDATE Item 
                SET quantity = @quantity, 
                    RetailPrice = @RetailPrice, 
                    unitCost = @unitCost, 
                    maxDiscount = @maxDiscount, 
                    description = @description, 
                    barcode = @barcode, 
                    MinStockLevel = @MinStockLevel, 
                    Brand_ID = @Brand_ID, 
                    Category_ID = @Category_ID, 
                    Size_ID = @Size_ID, 
                    ItemImage = @ItemImage
                WHERE Item_ID = @Item_ID";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Item_ID", item.Item_ID);
                AddItemParameters(cmd, item);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new DataException("Error updating item in database", ex);
            }
        }

        public bool DeleteItem(int itemId)
        {
            const string query = "UPDATE Item SET IsDeleted = 1 WHERE Item_ID = @Item_ID";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Item_ID", itemId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new DataException("Error deleting item from database", ex);
            }
        }

        public bool CheckItemExists(string description, string barcode, int? itemId = null)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM Item 
                WHERE (description = @description OR barcode = @barcode) 
                    AND IsDeleted = 0";

            if (itemId.HasValue)
                query += " AND Item_ID != @Item_ID";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                if (itemId.HasValue)
                    cmd.Parameters.AddWithValue("@Item_ID", itemId.Value);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
            catch (Exception ex)
            {
                throw new DataException("Error checking item existence", ex);
            }
        }

        public string GenerateUniqueBarcode()
        {
            const int maxAttempts = 10;
            int attempts = 0;
            string barcode;
            Random random = new Random();

            do
            {
                // Generate 7-digit numeric barcode
                barcode = random.Next(1000000, 10000000).ToString();
                attempts++;
            } while (BarcodeExists(barcode) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
                throw new Exception("Unable to generate unique barcode after 10 attempts");

            return barcode;
        }

        private bool BarcodeExists(string barcode)
        {
            const string query = "SELECT COUNT(1) FROM Item WHERE barcode = @Barcode";

            using var conn = DbHelper.GetConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Barcode", barcode);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public List<Item> SearchItems(string searchTerm, int brandId, int categoryId)
        {
            var items = new List<Item>();
            const string query = @"
                SELECT i.Item_ID, i.barcode, i.description, i.RetailPrice, i.quantity,
                       b.brandName AS BrandName, c.categoryName AS CategoryName, 
                       s.SizeLabel AS SizeLabel, i.ItemImage, i.unitCost
                FROM Item i
                INNER JOIN Brand b ON i.Brand_ID = b.Brand_ID
                INNER JOIN Category c ON i.Category_ID = c.Category_ID
                LEFT JOIN Size s ON i.Size_ID = s.Size_ID
                WHERE i.IsDeleted = 0
                  AND (@SearchTerm = '' OR 
                       i.barcode LIKE '%' + @SearchTerm + '%' OR 
                       i.description LIKE '%' + @SearchTerm + '%')
                  AND (@BrandId = 0 OR i.Brand_ID = @BrandId)
                  AND (@CategoryId = 0 OR i.Category_ID = @CategoryId)";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SearchTerm", string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm);
                cmd.Parameters.AddWithValue("@BrandId", brandId);
                cmd.Parameters.AddWithValue("@CategoryId", categoryId);

                conn.Open();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        Item_ID = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                        barcode = reader.GetString(reader.GetOrdinal("barcode")),
                        description = reader.GetString(reader.GetOrdinal("description")),
                        RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                        unitCost = reader.GetDecimal(reader.GetOrdinal("unitCost")),
                        quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                        BrandName = reader.GetString(reader.GetOrdinal("BrandName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                        SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                                    null : reader.GetString(reader.GetOrdinal("SizeLabel")),
                        ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                                    null : (byte[])reader["ItemImage"]
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataException("Error searching items", ex);
            }

            return items;
        }

        private static Item MapItemFromReader(SqlDataReader reader)
        {
            return new Item
            {
                Item_ID = reader.GetInt32(reader.GetOrdinal("Item_ID")),
                quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                unitCost = reader.GetDecimal(reader.GetOrdinal("unitCost")),
                maxDiscount = reader.GetDecimal(reader.GetOrdinal("maxDiscount")),
                description = reader.IsDBNull(reader.GetOrdinal("description")) ?
                              null : reader.GetString(reader.GetOrdinal("description")),
                barcode = reader.GetString(reader.GetOrdinal("barcode")),
                MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted")),
                Brand_ID = reader.GetInt32(reader.GetOrdinal("Brand_ID")),
                Category_ID = reader.GetInt32(reader.GetOrdinal("Category_ID")),
                Size_ID = reader.IsDBNull(reader.GetOrdinal("Size_ID")) ?
                          (int?)null : reader.GetInt32(reader.GetOrdinal("Size_ID")),
                ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                            null : (byte[])reader["ItemImage"],
                BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                            null : reader.GetString(reader.GetOrdinal("SizeLabel"))
            };
        }

        private static void AddItemParameters(SqlCommand cmd, Item item)
        {
            cmd.Parameters.AddWithValue("@quantity", item.quantity);
            cmd.Parameters.AddWithValue("@RetailPrice", item.RetailPrice);
            cmd.Parameters.AddWithValue("@unitCost", item.unitCost);
            cmd.Parameters.AddWithValue("@maxDiscount", item.maxDiscount);
            cmd.Parameters.AddWithValue("@description", (object)item.description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@barcode", item.barcode);
            cmd.Parameters.AddWithValue("@MinStockLevel", item.MinStockLevel);
            cmd.Parameters.AddWithValue("@Brand_ID", item.Brand_ID);
            cmd.Parameters.AddWithValue("@Category_ID", item.Category_ID);
            cmd.Parameters.AddWithValue("@Size_ID", (object)item.Size_ID ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ItemImage", (object)item.ItemImage ?? DBNull.Value);
        }
    }
}
