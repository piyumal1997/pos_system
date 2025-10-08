using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;

namespace pos_system.pos.BLL.Repositories
{
    public class ReportService
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["RetailPOSConnection"].ConnectionString;

        public DataTable GetSalesTimeSeries(DateTime startDate, DateTime endDate)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("sp_GetSalesTimeSeries", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }

        //public DataTable GetCategorySales(DateTime startDate, DateTime endDate)
        //{
        //    using (var conn = new SqlConnection(ConnectionString))
        //    using (var cmd = new SqlCommand("sp_GetCategorySales", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@StartDate", startDate);
        //        cmd.Parameters.AddWithValue("@EndDate", endDate);

        //        Debug.WriteLine($"Executing sp_GetCategorySales with StartDate: {startDate}, EndDate: {endDate}");

        //        conn.Open();
        //        var dt = new DataTable();
        //        dt.Load(cmd.ExecuteReader());

        //        Debug.WriteLine($"Retrieved {dt.Rows.Count} rows");
        //        return dt;
        //    }
        //}


        public DataTable GetCategorySales(DateTime startDate, DateTime endDate)
        {
            string sqlQuery = @"
                    SELECT 
                        c.categoryName AS CategoryName,
                        CASE 
                            WHEN DATEDIFF(DAY, @StartDate, @EndDate) <= 1 THEN
                                DATEADD(HOUR, DATEPART(HOUR, b.[date]), 
                                       CAST(CAST(b.[date] AS DATE) AS DATETIME))
                            ELSE
                                CAST(b.[date] AS DATE)
                        END AS Period,
                        SUM(bi.ItemSellingPrice * bi.quantity) AS TotalSales
                    FROM dbo.Bill b
                    INNER JOIN dbo.Bill_Item bi ON b.Bill_ID = bi.Bill_ID
                    INNER JOIN dbo.ProductSize ps ON bi.ProductSize_ID = ps.ProductSize_ID
                    INNER JOIN dbo.Product p ON ps.Product_ID = p.Product_ID
                    INNER JOIN dbo.Category c ON p.Category_ID = c.Category_ID
                    WHERE b.[date] BETWEEN @StartDate AND @EndDate
                        AND b.BillStatus = 'Completed'
                    GROUP BY 
                        c.categoryName,
                        CASE 
                            WHEN DATEDIFF(DAY, @StartDate, @EndDate) <= 1 THEN
                                DATEADD(HOUR, DATEPART(HOUR, b.[date]), 
                                       CAST(CAST(b.[date] AS DATE) AS DATETIME))
                            ELSE
                                CAST(b.[date] AS DATE)
                        END
                    ORDER BY Period, c.categoryName";

            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sqlQuery, conn))
            {
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                Debug.WriteLine($"Executing CategorySales query with StartDate: {startDate}, EndDate: {endDate}");

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Debug.WriteLine($"Retrieved {dt.Rows.Count} rows");
                return dt;
            }
        }
    }
}