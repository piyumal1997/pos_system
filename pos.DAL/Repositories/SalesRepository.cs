using Microsoft.Data.SqlClient;
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
            using (var cmd = new SqlCommand("sp_GetSalesReports", conn))
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
                    // 1. Accounting Summary
                    if (reader.Read())
                    {
                        report.AccountingSummary = new AccountingSummary
                        {
                            GrossSales = GetSafeDecimal(reader, "GrossSales"),
                            Discounts = GetSafeDecimal(reader, "Discounts"),
                            Returns = GetSafeDecimal(reader, "Returns"),
                            NetSales = GetSafeDecimal(reader, "NetSales"),
                            GrossCOGS = GetSafeDecimal(reader, "GrossCOGS"),
                            ReturnsCOGS = GetSafeDecimal(reader, "ReturnsCOGS"),
                            NetCOGS = GetSafeDecimal(reader, "NetCOGS"),
                            GrossProfit = GetSafeDecimal(reader, "GrossProfit"),
                            NetProfit = GetSafeDecimal(reader, "NetProfit"),
                            GrossItemsSold = GetSafeInt(reader, "GrossItemsSold"),
                            ReturnedItems = GetSafeInt(reader, "ReturnedItems"),
                            NetItemsSold = GetSafeInt(reader, "NetItemsSold"),
                            BillCount = GetSafeInt(reader, "BillCount")
                        };
                    }

                    // 2. Cash Flow Summary
                    if (reader.NextResult() && reader.Read())
                    {
                        report.CashFlowSummary = new CashFlowSummary
                        {
                            CashInflow = GetSafeDecimal(reader, "CashInflow"),
                            CashOutflow = GetSafeDecimal(reader, "CashOutflow"),
                            NetCashFlow = GetSafeDecimal(reader, "NetCashFlow"),
                            CashSales = GetSafeDecimal(reader, "CashSales"),
                            CardSales = GetSafeDecimal(reader, "CardSales"),
                            BankSales = GetSafeDecimal(reader, "BankSales")
                        };
                    }

                    // 3. Token Activity
                    if (reader.NextResult() && reader.Read())
                    {
                        report.TokenActivity = new TokenActivity
                        {
                            TokensIssued = GetSafeInt(reader, "TokensIssued"),
                            TokenValueIssued = GetSafeDecimal(reader, "TokenValueIssued"),
                            TokensUsed = GetSafeInt(reader, "TokensUsed"),
                            TokenValueUsed = GetSafeDecimal(reader, "TokenValueUsed"),
                            TokensOutstanding = GetSafeInt(reader, "TokensOutstanding"),
                            TokenValueOutstanding = GetSafeDecimal(reader, "TokenValueOutstanding")
                        };
                    }

                    // 4. Sales Items
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
                                MaxAllowedDiscount = GetSafeDecimal(reader, "MaxAllowedDiscount"),
                                AppliedDiscount = GetSafeDecimal(reader, "AppliedDiscount"),
                                GrossAmount = GetSafeDecimal(reader, "GrossAmount"),
                                DiscountAmount = GetSafeDecimal(reader, "DiscountAmount"),
                                NetAmount = GetSafeDecimal(reader, "NetAmount")
                            });
                        }
                    }

                    // 5. Return Items
                    if (reader.NextResult())
                    {
                        report.ReturnItems = new List<ReturnItemDetail>();
                        while (reader.Read())
                        {
                            report.ReturnItems.Add(new ReturnItemDetail
                            {
                                Return_ID = GetSafeInt(reader, "Return_ID"),
                                ReturnDate = GetSafeDateTime(reader, "ReturnDate"),
                                OriginalBill_ID = GetSafeInt(reader, "OriginalBill_ID"),
                                ItemDescription = GetSafeString(reader, "ItemDescription"),
                                Size = GetSafeString(reader, "Size"),
                                Quantity = GetSafeInt(reader, "Quantity"),
                                RefundValue = GetSafeDecimal(reader, "RefundValue"),
                                Reason = GetSafeString(reader, "Reason"),
                                RefundMethod = GetSafeString(reader, "RefundMethod"),
                                TokenUsedInBill = GetSafeNullableInt(reader, "TokenUsedInBill")
                            });
                        }
                    }

                    // 6. Bill Summaries
                    if (reader.NextResult())
                    {
                        report.BillSummaries = new List<BillSummary>();
                        while (reader.Read())
                        {
                            report.BillSummaries.Add(new BillSummary
                            {
                                Bill_ID = GetSafeInt(reader, "Bill_ID"),
                                PaymentMethod = GetSafeString(reader, "PaymentMethod"),
                                Employee_ID = GetSafeInt(reader, "Employee_ID"),
                                Discount_Method = GetSafeString(reader, "Discount_Method"),
                                CustomerContact = GetSafeString(reader, "CustomerContact"),
                                Token_Value = GetSafeNullableDecimal(reader, "Token_Value"),
                                SaleDate = GetSafeDateTime(reader, "SaleDate"),
                                GrossAmount = GetSafeDecimal(reader, "GrossAmount"),
                                NetAmount = GetSafeDecimal(reader, "NetAmount"),
                                CashPayment = GetSafeDecimal(reader, "CashPayment")
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
            cmd.Parameters.AddWithValue(paramName, value ?? DBNull.Value);
        }

        private decimal GetSafeDecimal(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }

        private int GetSafeInt(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }

        private string GetSafeString(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }

        private DateTime GetSafeDateTime(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? DateTime.MinValue : reader.GetDateTime(ordinal);
        }

        private int? GetSafeNullableInt(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
        }

        private decimal? GetSafeNullableDecimal(SqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? (decimal?)null : reader.GetDecimal(ordinal);
        }
        #endregion
    }
}