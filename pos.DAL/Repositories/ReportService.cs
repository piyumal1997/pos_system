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

        public DataTable GetCategorySales(DateTime startDate, DateTime endDate)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("sp_GetCategorySales", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);

                Debug.WriteLine($"Executing sp_GetCategorySales with StartDate: {startDate}, EndDate: {endDate}");

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                Debug.WriteLine($"Retrieved {dt.Rows.Count} rows");
                return dt;
            }
        }
    }
}