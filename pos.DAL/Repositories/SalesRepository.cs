using Microsoft.Data.SqlClient;
using pos_system.pos.DAL.Repositories;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace pos_system.pos.DAL.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        public SalesReport GetSalesReport(SalesFilter filter)
        {
            var report = new SalesReport();

            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("sp_GetSalesReport", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                AddParameterWithNull(cmd, "@StartDate", filter.StartDate);
                AddParameterWithNull(cmd, "@EndDate", filter.EndDate);
                AddParameterWithNull(cmd, "@BrandId", filter.BrandId);
                AddParameterWithNull(cmd, "@CategoryId", filter.CategoryId);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    // Sales Summary
                    if (reader.Read())
                    {
                        report.TotalSales = GetSafeDecimal(reader, "TotalSales");
                        report.TotalCost = GetSafeDecimal(reader, "TotalCost");
                        report.TotalItemsSold = GetSafeInt(reader, "TotalItemsSold");
                        report.BillCount = GetSafeInt(reader, "BillCount");
                    }

                    // Payment Methods
                    if (reader.NextResult() && reader.Read())
                    {
                        report.CashSales = GetSafeDecimal(reader, "CashSales");
                        report.CardSales = GetSafeDecimal(reader, "CardSales");
                        report.BankTransferSales = GetSafeDecimal(reader, "BankTransferSales");
                    }

                    // Return Summary
                    if (reader.NextResult() && reader.Read())
                    {
                        report.ReturnCount = GetSafeInt(reader, "ReturnCount");
                        report.ReturnItemCount = GetSafeInt(reader, "ReturnItemCount");
                        report.ReturnTotalValue = GetSafeDecimal(reader, "ReturnTotalValue");
                    }

                    // Sales Items
                    if (reader.NextResult())
                    {
                        report.SalesItems = new List<SalesItemDetail>();
                        while (reader.Read())
                        {
                            report.SalesItems.Add(new SalesItemDetail
                            {
                                Bill_ID = GetSafeInt(reader, "Bill_ID"),
                                SaleDate = GetSafeDateTime(reader, "SaleDate"),
                                ItemDescription = GetSafeString(reader, "ItemDescription"),
                                Size = GetSafeString(reader, "Size"),
                                Quantity = GetSafeInt(reader, "Quantity"),
                                UnitPrice = GetSafeDecimal(reader, "UnitPrice"),
                                Discount = GetSafeDecimal(reader, "Discount"),
                                NetAmount = GetSafeDecimal(reader, "NetAmount")
                            });
                        }
                    }

                    // Return Items
                    if (reader.NextResult())
                    {
                        report.ReturnItems = new List<ReturnItemDetail>();
                        while (reader.Read())
                        {
                            report.ReturnItems.Add(new ReturnItemDetail
                            {
                                Return_ID = GetSafeInt(reader, "Return_ID"),
                                ReturnDate = GetSafeDateTime(reader, "ReturnDate"),
                                Bill_ID = GetSafeInt(reader, "Bill_ID"),
                                ItemDescription = GetSafeString(reader, "ItemDescription"),
                                Size = GetSafeString(reader, "Size"),
                                Quantity = GetSafeInt(reader, "Quantity"),
                                RefundValue = GetSafeDecimal(reader, "RefundValue"),
                                Reason = GetSafeString(reader, "Reason")
                            });
                        }
                    }
                }
            }
            return report;
        }

        public List<Brand> GetBrands()
        {
            var brands = new List<Brand>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT Brand_ID, brandName FROM Brand", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        brands.Add(new Brand
                        {
                            Brand_ID = reader.GetInt32(0),
                            brandName = reader.GetString(1)
                        });
                    }
                }
            }
            return brands;
        }

        public List<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (var conn = DbHelper.GetConnection())
            using (var cmd = new SqlCommand("SELECT Category_ID, categoryName FROM Category", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Category_ID = reader.GetInt32(0),
                            categoryName = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }

        #region Helper Methods
        private void AddParameterWithNull(SqlCommand cmd, string paramName, object value)
        {
            if (value != null)
            {
                cmd.Parameters.AddWithValue(paramName, value);
            }
            else
            {
                cmd.Parameters.AddWithValue(paramName, DBNull.Value);
            }
        }

        private decimal GetSafeDecimal(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }
        }

        private int GetSafeInt(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                return 0;
            }
        }

        private string GetSafeString(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }

        private DateTime GetSafeDateTime(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
            }
            catch (IndexOutOfRangeException)
            {
                return DateTime.MinValue;
            }
        }
        #endregion
    }
}