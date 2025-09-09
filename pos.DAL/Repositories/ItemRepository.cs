using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Data;
using System.Linq;

namespace pos_system.pos.DAL.Repositories
{
    internal class ItemRepository
    {
        //public bool AddItem(Item item)
        //{
        //    const string productQuery = @"
        //        INSERT INTO Product (
        //            description, barcode, MinStockLevel, maxDiscount, 
        //            Brand_ID, Category_ID, Gender_ID, ItemImage
        //        ) VALUES (
        //            @Description, @Barcode, @MinStockLevel, @MaxDiscount, 
        //            @Brand_ID, @Category_ID, @Gender_ID, @ItemImage
        //        ); SELECT SCOPE_IDENTITY();";

        //    const string sizeQuery = @"
        //        INSERT INTO ProductSize (
        //            Product_ID, Size_ID, quantity, RetailPrice, unitCost
        //        ) VALUES (
        //            @Product_ID, @Size_ID, @Quantity, @RetailPrice, @UnitCost)";

        //    try
        //    {
        //        using var conn = DbHelper.GetConnection();
        //        conn.Open();

        //        using var transaction = conn.BeginTransaction();

        //        // Insert product
        //        using var cmdProduct = new SqlCommand(productQuery, conn, transaction);
        //        cmdProduct.Parameters.AddWithValue("@Description", item.Description);
        //        cmdProduct.Parameters.AddWithValue("@Barcode", item.Barcode);
        //        cmdProduct.Parameters.AddWithValue("@MinStockLevel", item.MinStockLevel);
        //        cmdProduct.Parameters.AddWithValue("@MaxDiscount", item.MaxDiscount);
        //        cmdProduct.Parameters.AddWithValue("@Brand_ID", item.Brand_ID);
        //        cmdProduct.Parameters.AddWithValue("@Category_ID", item.Category_ID);
        //        cmdProduct.Parameters.AddWithValue("@Gender_ID", item.Gender_ID);
        //        cmdProduct.Parameters.AddWithValue("@ItemImage", (object)item.ItemImage ?? DBNull.Value);

        //        int productId = Convert.ToInt32(cmdProduct.ExecuteScalar());

        //        // Insert sizes
        //        foreach (var size in item.Sizes)
        //        {
        //            using var cmdSize = new SqlCommand(sizeQuery, conn, transaction);
        //            cmdSize.Parameters.AddWithValue("@Product_ID", productId);
        //            cmdSize.Parameters.AddWithValue("@Size_ID", size.Size_ID);
        //            cmdSize.Parameters.AddWithValue("@Quantity", size.Quantity);
        //            cmdSize.Parameters.AddWithValue("@RetailPrice", size.RetailPrice);
        //            cmdSize.Parameters.AddWithValue("@UnitCost", size.UnitCost);
        //            cmdSize.ExecuteNonQuery();
        //        }

        //        transaction.Commit();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log error
        //        File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
        //        return false;
        //    }
        //}

        public int AddItem(Item item)
        {
            const string productQuery = @"
        INSERT INTO Product (
            description, barcode, MinStockLevel, maxDiscount, 
            Brand_ID, Category_ID, Gender_ID
        ) VALUES (
            @Description, @Barcode, @MinStockLevel, @MaxDiscount, 
            @Brand_ID, @Category_ID, @Gender_ID
        ); SELECT SCOPE_IDENTITY();";

            const string sizeQuery = @"
        INSERT INTO ProductSize (
            Product_ID, Size_ID, quantity, RetailPrice, unitCost
        ) VALUES (
            @Product_ID, @Size_ID, @Quantity, @RetailPrice, @UnitCost)";

            try
            {
                using var conn = DbHelper.GetConnection();
                conn.Open();

                using var transaction = conn.BeginTransaction();

                // Insert product (without image first)
                using var cmdProduct = new SqlCommand(productQuery, conn, transaction);
                cmdProduct.Parameters.AddWithValue("@Description", item.Description);
                cmdProduct.Parameters.AddWithValue("@Barcode", item.Barcode);
                cmdProduct.Parameters.AddWithValue("@MinStockLevel", item.MinStockLevel);
                cmdProduct.Parameters.AddWithValue("@MaxDiscount", item.MaxDiscount);
                cmdProduct.Parameters.AddWithValue("@Brand_ID", item.Brand_ID);
                cmdProduct.Parameters.AddWithValue("@Category_ID", item.Category_ID);
                cmdProduct.Parameters.AddWithValue("@Gender_ID", item.Gender_ID);

                // Get generated Product_ID
                int productId = Convert.ToInt32(cmdProduct.ExecuteScalar());

                if (productId <= 0)
                {
                    transaction.Rollback();
                    return 0;
                }

                // Insert sizes
                foreach (var size in item.Sizes)
                {
                    using var cmdSize = new SqlCommand(sizeQuery, conn, transaction);
                    cmdSize.Parameters.AddWithValue("@Product_ID", productId);
                    cmdSize.Parameters.AddWithValue("@Size_ID", size.Size_ID);
                    cmdSize.Parameters.AddWithValue("@Quantity", size.Quantity);
                    cmdSize.Parameters.AddWithValue("@RetailPrice", size.RetailPrice);
                    cmdSize.Parameters.AddWithValue("@UnitCost", size.UnitCost);
                    cmdSize.ExecuteNonQuery();
                }

                transaction.Commit();
                return productId;
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: AddItem failed - {ex}\n\n");
                return 0;
            }
        }

        public bool UpdateItemImage(int productId, string fileName)
        {
            const string query = "UPDATE Product SET ItemImage = @FileName WHERE Product_ID = @ProductId";

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool UpdateItem(Item item)
        {
            const string productQuery = @"
        UPDATE Product SET
            description = @Description,
            barcode = @Barcode,
            MinStockLevel = @MinStockLevel,
            maxDiscount = @MaxDiscount,
            Brand_ID = @Brand_ID,
            Category_ID = @Category_ID,
            Gender_ID = @Gender_ID,
            ItemImage = @ItemImage
        WHERE Product_ID = @Product_ID";

            const string sizeUpsertQuery = @"
        MERGE ProductSize AS target
        USING (VALUES (@Size_ID, @Quantity, @RetailPrice, @UnitCost)) 
            AS source (Size_ID, Quantity, RetailPrice, UnitCost)
        ON target.Product_ID = @Product_ID AND target.Size_ID = source.Size_ID
        WHEN MATCHED THEN
            UPDATE SET 
                quantity = source.Quantity,
                RetailPrice = source.RetailPrice,
                unitCost = source.UnitCost
        WHEN NOT MATCHED BY TARGET THEN
            INSERT (Product_ID, Size_ID, quantity, RetailPrice, unitCost)
            VALUES (@Product_ID, source.Size_ID, source.Quantity, source.RetailPrice, source.UnitCost);";

            const string deleteObsoleteSizesQuery = @"
        DELETE FROM ProductSize 
        WHERE Product_ID = @Product_ID
        AND Size_ID NOT IN (SELECT value FROM STRING_SPLIT(@SizeIdList, ','))
        AND ProductSize_ID NOT IN (
            SELECT ProductSize_ID FROM Bill_Item
        )";

            using var conn = DbHelper.GetConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. Update main product details
                using (var cmdProduct = new SqlCommand(productQuery, conn, transaction))
                {
                    // Handle image safely
                    object imageParam = (item.ItemImage != null && item.ItemImage.Length > 0)
                        ? (object)item.ItemImage
                        : DBNull.Value;

                    cmdProduct.Parameters.AddWithValue("@Product_ID", item.Product_ID);
                    cmdProduct.Parameters.AddWithValue("@Description", item.Description);
                    cmdProduct.Parameters.AddWithValue("@Barcode", item.Barcode);
                    cmdProduct.Parameters.AddWithValue("@MinStockLevel", item.MinStockLevel);
                    cmdProduct.Parameters.AddWithValue("@MaxDiscount", item.MaxDiscount);
                    cmdProduct.Parameters.AddWithValue("@Brand_ID", item.Brand_ID);
                    cmdProduct.Parameters.AddWithValue("@Category_ID", item.Category_ID);
                    cmdProduct.Parameters.AddWithValue("@Gender_ID", item.Gender_ID);
                    cmdProduct.Parameters.AddWithValue("@ItemImage", imageParam);
                    cmdProduct.ExecuteNonQuery();
                }

                // 2. Upsert size variants
                using (var cmdSize = new SqlCommand(sizeUpsertQuery, conn, transaction))
                {
                    cmdSize.Parameters.Add("@Product_ID", SqlDbType.Int).Value = item.Product_ID;
                    cmdSize.Parameters.Add("@Size_ID", SqlDbType.Int);
                    cmdSize.Parameters.Add("@Quantity", SqlDbType.Int);
                    cmdSize.Parameters.Add("@RetailPrice", SqlDbType.Decimal);
                    cmdSize.Parameters.Add("@UnitCost", SqlDbType.Decimal);

                    foreach (var size in item.Sizes)
                    {
                        cmdSize.Parameters["@Size_ID"].Value = size.Size_ID;
                        cmdSize.Parameters["@Quantity"].Value = size.Quantity;
                        cmdSize.Parameters["@RetailPrice"].Value = size.RetailPrice;
                        cmdSize.Parameters["@UnitCost"].Value = size.UnitCost;
                        cmdSize.ExecuteNonQuery();
                    }
                }

                // 3. Safely remove obsolete sizes (only if not used in sales)
                if (item.Sizes.Count > 0)
                {
                    var sizeIds = string.Join(",", item.Sizes.Select(s => s.Size_ID));

                    using (var cmdDelete = new SqlCommand(deleteObsoleteSizesQuery, conn, transaction))
                    {
                        cmdDelete.Parameters.AddWithValue("@Product_ID", item.Product_ID);
                        cmdDelete.Parameters.AddWithValue("@SizeIdList", sizeIds);
                        cmdDelete.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                // Attempt to roll back on error
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    File.WriteAllText("error.log",
                        $"[{DateTime.Now}] Rollback failed: {rollbackEx}\n\n");
                }

                // Wrap and rethrow with meaningful message
                throw new Exception($"Database update failed: {ex.Message}", ex);
            }
        }

        public List<Item> SearchItems(string searchTerm)
        {
            var items = new List<Item>();
            const string query = @"
            SELECT p.*, b.brandName, c.categoryName, g.GenderName,
                   ps.ProductSize_ID, ps.Size_ID, s.SizeLabel, 
                   ps.quantity, ps.RetailPrice
            FROM Product p
            INNER JOIN Brand b ON p.Brand_ID = b.Brand_ID
            INNER JOIN Category c ON p.Category_ID = c.Category_ID
            INNER JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
            LEFT JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
            LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
            WHERE p.IsDeleted = 0
            AND (p.description LIKE @searchTerm OR p.barcode LIKE @searchTerm)";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                conn.Open();
                using var reader = cmd.ExecuteReader();

                var itemsDict = new Dictionary<int, Item>();

                while (reader.Read())
                {
                    int productId = reader.GetInt32(reader.GetOrdinal("Product_ID"));

                    if (!itemsDict.TryGetValue(productId, out Item item))
                    {
                        item = new Item
                        {
                            Product_ID = productId,
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Barcode = reader.GetString(reader.GetOrdinal("barcode")),
                            BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                            CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                            GenderName = reader.GetString(reader.GetOrdinal("GenderName")),
                            Sizes = new List<ProductSize>()
                        };
                        itemsDict.Add(productId, item);
                        items.Add(item);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("ProductSize_ID")))
                    {
                        item.Sizes.Add(new ProductSize
                        {
                            ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: Search failed - {ex}\n\n");
            }

            return items;
        }
        public bool DeleteItem(int productId)
        {
            const string query = "UPDATE Product SET IsDeleted = 1 WHERE Product_ID = @Product_ID";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Product_ID", productId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
                return false;
            }
        }

        public Item GetItem(int productId)
        {
            const string query = @"
                SELECT p.*, b.brandName, c.categoryName, g.GenderName
                FROM Product p
                INNER JOIN Brand b ON p.Brand_ID = b.Brand_ID
                INNER JOIN Category c ON p.Category_ID = c.Category_ID
                INNER JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
                WHERE p.Product_ID = @ProductId;
        
                SELECT ps.*, s.SizeLabel 
                FROM ProductSize ps
                INNER JOIN Size s ON ps.Size_ID = s.Size_ID
                WHERE ps.Product_ID = @ProductId;";

            var item = new Item();

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                conn.Open();

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    item.Product_ID = reader.GetInt32(reader.GetOrdinal("Product_ID"));
                    item.Description = reader.GetString(reader.GetOrdinal("description"));
                    item.Barcode = reader.GetString(reader.GetOrdinal("barcode"));
                    item.MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel"));
                    item.MaxDiscount = reader.GetDecimal(reader.GetOrdinal("maxDiscount"));
                    item.Brand_ID = reader.GetInt32(reader.GetOrdinal("Brand_ID"));
                    item.Category_ID = reader.GetInt32(reader.GetOrdinal("Category_ID"));
                    item.Gender_ID = reader.GetInt32(reader.GetOrdinal("Gender_ID"));
                    item.BrandName = reader.GetString(reader.GetOrdinal("brandName"));
                    item.CategoryName = reader.GetString(reader.GetOrdinal("categoryName"));
                    item.GenderName = reader.GetString(reader.GetOrdinal("GenderName"));

                    // Get filename string
                    item.ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                        null : reader.GetString(reader.GetOrdinal("ItemImage"));
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        item.Sizes.Add(new ProductSize
                        {
                            ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                            UnitCost = reader.GetDecimal(reader.GetOrdinal("unitCost"))
                        });
                    }
                }
            }
            return item;
        }

        public List<Item> SearchItemsOwner(string searchTerm, int brandId, int categoryId)
        {
            var variants = new List<Item>();
            const string query = @"
                SELECT 
                    ps.ProductSize_ID,
                    p.Product_ID,
                    p.description,
                    p.barcode,
                    p.ItemImage,
                    b.brandName,
                    c.categoryName,
                    g.GenderName,
                    s.SizeLabel,
                    ps.quantity,
                    ps.RetailPrice,
                    ps.unitCost
                FROM Product p
                JOIN Brand b ON p.Brand_ID = b.Brand_ID
                JOIN Category c ON p.Category_ID = c.Category_ID
                JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
                JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
                LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                WHERE p.IsDeleted = 0
                AND (p.description LIKE @searchTerm OR p.barcode LIKE @searchTerm)
                AND (@brandId = 0 OR p.Brand_ID = @brandId)
                AND (@categoryId = 0 OR p.Category_ID = @categoryId)";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                cmd.Parameters.AddWithValue("@brandId", brandId);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);

                conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    variants.Add(new Item
                    {
                        ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                        Product_ID = reader.GetInt32(reader.GetOrdinal("Product_ID")),
                        Description = reader.GetString(reader.GetOrdinal("description")),
                        Barcode = reader.GetString(reader.GetOrdinal("barcode")),
                        BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                        GenderName = reader.GetString(reader.GetOrdinal("GenderName")),
                        SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                                   "N/A" : reader.GetString(reader.GetOrdinal("SizeLabel")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                        RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                        UnitCost = reader.GetDecimal(reader.GetOrdinal("unitCost")),
                        ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                            null : reader.GetString(reader.GetOrdinal("ItemImage"))
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
            }

            return variants;
        }

        public List<Item> SearchItemsWithVariants(string searchTerm, int brandId, int categoryId)
        {
            var variants = new List<Item>();
            const string query = @"
                SELECT 
                    p.Product_ID,
                    ps.ProductSize_ID,
                    p.description,
                    p.barcode,
                    b.brandName,
                    c.categoryName,
                    g.Gender_ID,
                    s.SizeLabel,
                    ps.quantity,
                    ps.RetailPrice,
                    ps.unitCost
                FROM Product p
                JOIN Brand b ON p.Brand_ID = b.Brand_ID
                JOIN Category c ON p.Category_ID = c.Category_ID
                JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
                JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
                LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                WHERE p.IsDeleted = 0
                AND (p.description LIKE @searchTerm OR p.barcode LIKE @searchTerm)
                AND (@brandId = 0 OR p.Brand_ID = @brandId)
                AND (@categoryId = 0 OR p.Category_ID = @categoryId)";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                cmd.Parameters.AddWithValue("@brandId", brandId);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);

                conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    variants.Add(new Item
                    {
                        Product_ID = reader.GetInt32(reader.GetOrdinal("Product_ID")),
                        ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                        Description = reader.GetString(reader.GetOrdinal("description")),
                        Barcode = reader.GetString(reader.GetOrdinal("barcode")),
                        BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                        SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                                   "N/A" : reader.GetString(reader.GetOrdinal("SizeLabel")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                        RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                        UnitCost = reader.GetDecimal(reader.GetOrdinal("unitCost")),
                        Gender_ID = reader.GetInt32(reader.GetOrdinal("Gender_ID"))
                    });
                }
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
            }

            return variants;
        }

        public List<Item> GetAllItems()
        {
            var items = new List<Item>();
            const string query = @"
                SELECT p.Product_ID, p.description, p.barcode,
                       b.brandName, c.categoryName, g.GenderName,
                       ps.ProductSize_ID, ps.Size_ID, s.SizeLabel, ps.quantity, ps.RetailPrice
                FROM Product p
                INNER JOIN Brand b ON p.Brand_ID = b.Brand_ID
                INNER JOIN Category c ON p.Category_ID = c.Category_ID
                INNER JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
                LEFT JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
                LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                WHERE p.IsDeleted = 0";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                conn.Open();

                var itemsDict = new Dictionary<int, Item>();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int productId = reader.GetInt32(reader.GetOrdinal("Product_ID"));

                    if (!itemsDict.TryGetValue(productId, out Item item))
                    {
                        item = new Item
                        {
                            Product_ID = productId,
                            Description = reader.GetString(reader.GetOrdinal("description")),
                            Barcode = reader.GetString(reader.GetOrdinal("barcode")),
                            BrandName = reader.GetString(reader.GetOrdinal("brandName")),
                            CategoryName = reader.GetString(reader.GetOrdinal("categoryName")),
                            GenderName = reader.GetString(reader.GetOrdinal("GenderName")),
                            Sizes = new List<ProductSize>()
                        };
                        itemsDict.Add(productId, item);
                        items.Add(item);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("ProductSize_ID")))
                    {
                        item.Sizes.Add(new ProductSize
                        {
                            ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                            Size_ID = reader.GetInt32(reader.GetOrdinal("Size_ID")),
                            SizeLabel = reader.GetString(reader.GetOrdinal("SizeLabel")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                            RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice"))
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
            }

            return items;
        }

        public string GenerateUniqueBarcode()
        {
            const int maxAttempts = 10;
            int attempts = 0;
            string barcode;
            Random random = new Random();

            do
            {
                barcode = random.Next(10000000, 100000000).ToString();
                attempts++;
            } while (BarcodeExists(barcode) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
                throw new Exception("Unable to generate unique barcode after 10 attempts");

            return barcode;
        }

        private bool BarcodeExists(string barcode)
        {
            const string query = "SELECT COUNT(1) FROM Product WHERE barcode = @Barcode";

            using var conn = DbHelper.GetConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Barcode", barcode);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public List<Item> SearchItems(string searchTerm, int brandId, int categoryId)
        {
            // Implementation similar to GetAllItems but with filters
            // Omitted for brevity - would include WHERE clauses for search
            return new List<Item>();
        }
        public List<Item> SearchItemsWithFilters(
            string searchTerm,
            int brandId,
            int categoryId,
            int sizeId,
            int genderId,
            decimal minPrice,
            decimal maxPrice)
                {
                    var items = new List<Item>();
                    const string query = @"
            SELECT 
                ps.ProductSize_ID,
                p.Product_ID,
                p.description AS Description,  -- Alias to match C# property
                p.barcode AS Barcode,          -- Alias to match C# property
                p.MinStockLevel,
                p.maxDiscount AS MaxDiscount,
                p.Brand_ID,
                p.Category_ID,
                p.Gender_ID,
                p.ItemImage,
                b.brandName AS BrandName,
                c.categoryName AS CategoryName,
                g.GenderName,
                s.SizeLabel,
                ps.quantity AS Quantity,       -- Alias to match C# property
                ps.RetailPrice,
                ps.unitCost AS UnitCost
            FROM Product p
            JOIN Brand b ON p.Brand_ID = b.Brand_ID
            JOIN Category c ON p.Category_ID = c.Category_ID
            JOIN GenderBaseGroup g ON p.Gender_ID = g.Gender_ID
            JOIN ProductSize ps ON p.Product_ID = ps.Product_ID
            LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
            WHERE p.IsDeleted = 0
            AND (p.description LIKE @searchTerm OR p.barcode LIKE @searchTerm OR @searchTerm = '')
            AND (@brandId = 0 OR p.Brand_ID = @brandId)
            AND (@categoryId = 0 OR p.Category_ID = @categoryId)
            AND (@sizeId = 0 OR ps.Size_ID = @sizeId)
            AND (@genderId = 0 OR p.Gender_ID = @genderId)
            AND ps.RetailPrice >= @minPrice
            AND ps.RetailPrice <= CASE WHEN @maxPrice = 0 THEN 1000000 ELSE @maxPrice END";

            try
            {
                using var conn = DbHelper.GetConnection();
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                cmd.Parameters.AddWithValue("@brandId", brandId);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                cmd.Parameters.AddWithValue("@sizeId", sizeId);
                cmd.Parameters.AddWithValue("@genderId", genderId);
                cmd.Parameters.AddWithValue("@minPrice", minPrice);
                cmd.Parameters.AddWithValue("@maxPrice", maxPrice);

                conn.Open();
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        ProductSize_ID = reader.GetInt32(reader.GetOrdinal("ProductSize_ID")),
                        Product_ID = reader.GetInt32(reader.GetOrdinal("Product_ID")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),      // PascalCase
                        Barcode = reader.GetString(reader.GetOrdinal("Barcode")),              // PascalCase
                        MinStockLevel = reader.GetInt32(reader.GetOrdinal("MinStockLevel")),
                        MaxDiscount = reader.GetDecimal(reader.GetOrdinal("MaxDiscount")),
                        Brand_ID = reader.GetInt32(reader.GetOrdinal("Brand_ID")),
                        Category_ID = reader.GetInt32(reader.GetOrdinal("Category_ID")),
                        Gender_ID = reader.GetInt32(reader.GetOrdinal("Gender_ID")),
                        BrandName = reader.GetString(reader.GetOrdinal("BrandName")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                        GenderName = reader.GetString(reader.GetOrdinal("GenderName")),
                        SizeLabel = reader.IsDBNull(reader.GetOrdinal("SizeLabel")) ?
                                   "N/A" : reader.GetString(reader.GetOrdinal("SizeLabel")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),             // PascalCase
                        RetailPrice = reader.GetDecimal(reader.GetOrdinal("RetailPrice")),
                        UnitCost = reader.GetDecimal(reader.GetOrdinal("UnitCost")),
                        ItemImage = reader.IsDBNull(reader.GetOrdinal("ItemImage")) ?
                           null : reader.GetString(reader.GetOrdinal("ItemImage"))
                    });
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", $"{DateTime.Now}: {ex}\n\n");
            }

            return items;
        }
    }

}

