using Microsoft.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Linq;

namespace pos_system.pos.DAL
{
    internal class DbHelper
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["RetailPOSConnection"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // For queries returning DataTable
        public static DataTable GetDataTable(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }

        // For INSERT/UPDATE/DELETE
        public static int ExecuteNonQuery(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
