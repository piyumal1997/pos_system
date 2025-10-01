using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
using pos_system.pos.BLL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace pos_system.pos.DAL.Repositories
{
    public class DashboardRepository
    {
        public DashboardMetrics GetCashierMetrics(int employeeId, DateTime date)
        {
            var metrics = new DashboardMetrics();

            using (var conn = DbHelper.GetConnection())
            //using (var cmd = new SqlCommand("sp_GetCashierDashboardMetrics", conn))
            using (var cmd = new SqlCommand("sp_GetCashierDashboard", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                //cmd.Parameters.AddWithValue("@Today", "2025-07-18");
                cmd.Parameters.AddWithValue("@Today", date.Date);

                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    // First result set (sales metrics)
                    if (reader.Read())
                    {
                        //metrics.DailySalesAll = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                        //metrics.DailyCashIncome = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                        //metrics.DailyQuantitySold = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        //metrics.DailyItemsSold = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                        //metrics.DailyBankPayments = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);
                        //metrics.DailyCashPayments = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5);
                        //metrics.DailyCardPayments = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6);
                        metrics.DailySalesAll = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                        metrics.DailyQuantitySold = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        metrics.DailyItemsSold = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        metrics.DailyBankPayments = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3);
                        metrics.DailyCashPayments = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);
                        metrics.DailyCardPayments = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5);
                        metrics.DailyTokenPayment = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6);
                    }

                    // Second result set (return metrics)
                    if (reader.NextResult() && reader.Read())
                    {
                        metrics.DailyReturnAmount = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                        metrics.DailyReturnQuantity = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    }
                }
            }

            return metrics;
        }

        public DashboardMetrics GetDashboardMetrics()
        {
            var metrics = new DashboardMetrics();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Updated SQL queries for new schema
                string query = @"
                -- Total Items (with quantity > 0)
                SELECT COUNT(*) 
                FROM ProductSize 
                WHERE quantity > 0;

                -- Active Employees
                SELECT COUNT(*) FROM Employee WHERE status = 'Active';

                -- Total Bills
                SELECT COUNT(*) FROM Bill WHERE BillStatus = 'Completed';

                -- Total Returns
                SELECT COUNT(*) FROM [Return];

                -- Total Categories
                SELECT COUNT(*) FROM Category;

                -- Total Brands
                SELECT COUNT(*) FROM Brand;

                -- Today's Sales Income (ActualSales) - Updated with consistent calculation
                ;WITH SalesData AS (
                    SELECT 
                        b.Bill_ID,
                        b.Token_ReturnID,
                        NetAmount = 
                            CASE 
                                WHEN b.Discount_Method = 'PerItem' 
                                THEN SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity)
                                WHEN b.Discount_Method = 'TotalBill' 
                                THEN SUM(bi.ItemSellingPrice * bi.quantity) - b.Discount
                                ELSE SUM(bi.ItemSellingPrice * bi.quantity)
                            END,
                        Cost = SUM(ps.unitCost * bi.quantity),
                        AdditionalPayment = 
                            CASE 
                                WHEN b.Token_ReturnID IS NOT NULL THEN
                                    CASE 
                                        WHEN b.Discount_Method = 'PerItem' 
                                        THEN SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity)
                                        WHEN b.Discount_Method = 'TotalBill' 
                                        THEN SUM(bi.ItemSellingPrice * bi.quantity) - b.Discount
                                        ELSE SUM(bi.ItemSellingPrice * bi.quantity)
                                    END 
                                    - ISNULL(r.TotalRefund, 0)
                                ELSE 0
                            END
                    FROM Bill b
                    JOIN Bill_Item bi ON b.Bill_ID = bi.Bill_ID
                    JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                    LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
                    WHERE b.BillStatus = 'Completed'
                      AND CAST(b.[date] AS DATE) = CAST(GETDATE() AS DATE)
                    GROUP BY b.Bill_ID, b.Token_ReturnID, b.Discount_Method, b.Discount, r.TotalRefund
                ),
                BillCostAnalysis AS (
                    SELECT
                        CashInflow = SUM(
                            CASE 
                                WHEN Token_ReturnID IS NULL THEN NetAmount
                                ELSE AdditionalPayment
                            END
                        )
                    FROM SalesData
                )
                SELECT COALESCE(CashInflow, 0) AS TodaySales
                FROM BillCostAnalysis;

                -- Today's COGS (ActualCost) - Updated with consistent calculation
                ;WITH SalesData AS (
                    SELECT 
                        b.Bill_ID,
                        b.Token_ReturnID,
                        NetAmount = 
                            CASE 
                                WHEN b.Discount_Method = 'PerItem' 
                                THEN SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity)
                                WHEN b.Discount_Method = 'TotalBill' 
                                THEN SUM(bi.ItemSellingPrice * bi.quantity) - b.Discount
                                ELSE SUM(bi.ItemSellingPrice * bi.quantity)
                            END,
                        Cost = SUM(ps.unitCost * bi.quantity),
                        AdditionalPayment = 
                            CASE 
                                WHEN b.Token_ReturnID IS NOT NULL THEN
                                    CASE 
                                        WHEN b.Discount_Method = 'PerItem' 
                                        THEN SUM((bi.ItemSellingPrice - bi.Per_item_Discount) * bi.quantity)
                                        WHEN b.Discount_Method = 'TotalBill' 
                                        THEN SUM(bi.ItemSellingPrice * bi.quantity) - b.Discount
                                        ELSE SUM(bi.ItemSellingPrice * bi.quantity)
                                    END 
                                    - ISNULL(r.TotalRefund, 0)
                                ELSE 0
                            END
                    FROM Bill b
                    JOIN Bill_Item bi ON b.Bill_ID = bi.Bill_ID
                    JOIN ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                    LEFT JOIN [Return] r ON b.Token_ReturnID = r.Return_ID
                    WHERE b.BillStatus = 'Completed'
                      AND CAST(b.[date] AS DATE) = CAST(GETDATE() AS DATE)
                    GROUP BY b.Bill_ID, b.Token_ReturnID, b.Discount_Method, b.Discount, r.TotalRefund
                ),
                BillCostAnalysis AS (
                    SELECT
                        RegularBillCost = SUM(CASE WHEN Token_ReturnID IS NULL THEN Cost ELSE 0 END),
                        TokenBillAdditionalPayment = SUM(AdditionalPayment),
                        TokenBillFullCost = SUM(CASE WHEN Token_ReturnID IS NOT NULL THEN Cost ELSE 0 END),
                        TokenBillFullSales = SUM(CASE WHEN Token_ReturnID IS NOT NULL THEN NetAmount ELSE 0 END),
                        TokenBillCostRatio = 
                            CASE 
                                WHEN SUM(CASE WHEN Token_ReturnID IS NOT NULL THEN NetAmount ELSE 0 END) > 0
                                THEN SUM(CASE WHEN Token_ReturnID IS NOT NULL THEN Cost ELSE 0 END) / 
                                     SUM(CASE WHEN Token_ReturnID IS NOT NULL THEN NetAmount ELSE 0 END)
                                ELSE 0
                            END
                    FROM SalesData
                )
                SELECT COALESCE(RegularBillCost + (TokenBillAdditionalPayment * TokenBillCostRatio), 0) AS TodayCOGS
                FROM BillCostAnalysis;

                -- Today's Sales Quantity
                SELECT COALESCE(SUM(BI.quantity), 0)
                FROM Bill B
                INNER JOIN Bill_Item BI ON B.Bill_ID = BI.Bill_ID
                WHERE B.BillStatus = 'Completed' 
                    AND CAST(B.[date] AS DATE) = CAST(GETDATE() AS DATE);";

                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    metrics.TotalItems = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.ActiveEmployees = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.TotalBills = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.TotalReturns = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.TotalCategories = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.TotalBrands = ReadIntResult(reader);
                    reader.NextResult();

                    metrics.TodaysSales = ReadDecimalResult(reader);
                    reader.NextResult();

                    metrics.TodaysCOGS = ReadDecimalResult(reader);
                    reader.NextResult();

                    metrics.TodaysQuantity = ReadIntResult(reader);
                }

                // Get sales data for charts
                metrics.DailySales = GetSalesData(
                    @"SELECT CONVERT(VARCHAR(10), CAST(B.[date] AS DATE), 120) AS SaleDate, 
                         SUM(
                            CASE 
                                WHEN B.Discount_Method = 'PerItem' 
                                    THEN (BI.ItemSellingPrice - BI.Per_item_Discount) * BI.quantity
                                WHEN B.Discount_Method = 'TotalBill' 
                                    THEN (BI.ItemSellingPrice * BI.quantity) - (BI.ItemSellingPrice * BI.quantity * (B.Discount / NULLIF(bt.SubTotal,0)))
                                ELSE BI.ItemSellingPrice * BI.quantity
                            END
                         ) AS TotalSales
                  FROM Bill B
                  INNER JOIN Bill_Item BI ON B.Bill_ID = BI.Bill_ID
                  INNER JOIN (
                      SELECT Bill_ID, SUM(ItemSellingPrice * quantity) AS SubTotal
                      FROM Bill_Item
                      GROUP BY Bill_ID
                  ) bt ON B.Bill_ID = bt.Bill_ID
                  WHERE B.BillStatus = 'Completed' 
                    AND B.[date] >= DATEADD(DAY, -30, GETDATE())
                  GROUP BY CAST(B.[date] AS DATE)
                  ORDER BY CAST(B.[date] AS DATE)");

                metrics.MonthlySales = GetSalesData(
                    @"SELECT FORMAT(B.[date], 'yyyy-MM') AS SaleMonth, 
                         SUM(
                            CASE 
                                WHEN B.Discount_Method = 'PerItem' 
                                    THEN (BI.ItemSellingPrice - BI.Per_item_Discount) * BI.quantity
                                WHEN B.Discount_Method = 'TotalBill' 
                                    THEN (BI.ItemSellingPrice * BI.quantity) - (BI.ItemSellingPrice * BI.quantity * (B.Discount / NULLIF(bt.SubTotal,0)))
                                ELSE BI.ItemSellingPrice * BI.quantity
                            END
                         ) AS TotalSales
                  FROM Bill B
                  INNER JOIN Bill_Item BI ON B.Bill_ID = BI.Bill_ID
                  INNER JOIN (
                      SELECT Bill_ID, SUM(ItemSellingPrice * quantity) AS SubTotal
                      FROM Bill_Item
                      GROUP BY Bill_ID
                  ) bt ON B.Bill_ID = bt.Bill_ID
                  WHERE B.BillStatus = 'Completed' 
                    AND B.[date] >= DATEADD(MONTH, -12, GETDATE())
                  GROUP BY FORMAT(B.[date], 'yyyy-MM')
                  ORDER BY FORMAT(B.[date], 'yyyy-MM')");
            }

            return metrics;
        }

        private int ReadIntResult(SqlDataReader reader)
        {
            if (reader.Read())
            {
                return reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
            }
            return 0;
        }

        private decimal ReadDecimalResult(SqlDataReader reader)
        {
            if (reader.Read())
            {
                return reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
            }
            return 0;
        }

        private List<SalesData> GetSalesData(string query)
        {
            var salesData = new List<SalesData>();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string period;
                        if (reader[0] is DateTime dateValue)
                        {
                            period = dateValue.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            period = reader.GetString(0);
                        }

                        salesData.Add(new SalesData
                        {
                            Period = period,
                            TotalSales = reader.GetDecimal(1)
                        });
                    }
                }
            }

            return salesData;
        }
    }
}