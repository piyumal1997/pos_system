using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
using pos_system.pos.Models;

namespace pos_system.pos.BLL.Services
{
    public class ReturnService
    {
        public ReturnResult ProcessReturn(int employeeId, int originalBillId, List<ReturnItem> returnItems)
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            DataTable returnItemsTable = new DataTable();
                            returnItemsTable.Columns.Add("Bill_ID", typeof(int));
                            returnItemsTable.Columns.Add("Item_ID", typeof(int));
                            returnItemsTable.Columns.Add("Quantity", typeof(int));
                            returnItemsTable.Columns.Add("Reason_ID", typeof(int));
                            returnItemsTable.Columns.Add("IsRestocked", typeof(bool));

                            foreach (var item in returnItems)
                            {
                                returnItemsTable.Rows.Add(
                                    item.BillId,
                                    item.ItemId,
                                    item.Quantity,
                                    item.ReasonId,
                                    item.IsRestocked
                                );
                            }

                            int returnId = 0;
                            decimal totalRefund = 0;

                            using (SqlCommand cmd = new SqlCommand("sp_ProcessReturn", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Transaction = transaction;

                                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                                cmd.Parameters.AddWithValue("@OriginalBillID", originalBillId);

                                SqlParameter tvpParam = cmd.Parameters.AddWithValue(
                                    "@ReturnItems",
                                    returnItemsTable
                                );
                                tvpParam.SqlDbType = SqlDbType.Structured;
                                tvpParam.TypeName = "dbo.ReturnItemType";

                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        returnId = reader.GetInt32(0);
                                        totalRefund = reader.GetDecimal(1);
                                    }
                                }
                            }

                            transaction.Commit();

                            return new ReturnResult
                            {
                                ReturnId = returnId,
                                TotalRefund = totalRefund
                            };
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception($"Error processing return transaction: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in return service: {ex.Message}", ex);
            }
        }
    }

    public class ReturnResult
    {
        public int ReturnId { get; set; }
        public decimal TotalRefund { get; set; }
    }
}