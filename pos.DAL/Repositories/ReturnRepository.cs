using Microsoft.Data.SqlClient;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace pos_system.pos.BLL.Repositories
{
    public class ReturnRepository
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
                            returnItemsTable.Columns.Add("ProductSize_ID", typeof(int));
                            returnItemsTable.Columns.Add("Quantity", typeof(int));
                            returnItemsTable.Columns.Add("Reason_ID", typeof(int));
                            returnItemsTable.Columns.Add("IsRestocked", typeof(bool));

                            foreach (var item in returnItems)
                            {
                                returnItemsTable.Rows.Add(
                                    item.BillId,
                                    item.ProductSizeId,
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
                throw new Exception($"Error in return repository: {ex.Message}", ex);
            }
        }

        public ReturnReceiptData GetReturnReceiptData(int returnId)
        {
            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Get return header
                    string headerQuery = @"
                        SELECT 
                            r.Return_ID,
                            r.TotalRefund,
                            r.ReturnDate,
                            b.Bill_ID AS OriginalBillId,
                            b.[date] AS BillDate,
                            e.firstName + ' ' + e.lastName AS CashierName
                        FROM [Return] r
                        JOIN Bill b ON r.OriginalBill_ID = b.Bill_ID
                        JOIN Employee e ON r.Employee_ID = e.Employee_ID
                        WHERE r.Return_ID = @ReturnId";

                    SqlParameter[] headerParams = { new SqlParameter("@ReturnId", returnId) };
                    DataTable headerTable = DbHelper.GetDataTable(headerQuery, CommandType.Text, headerParams);

                    if (headerTable.Rows.Count == 0)
                        throw new Exception("Return not found");

                    DataRow headerRow = headerTable.Rows[0];

                    // Get return items
                    string itemsQuery = @"
                        SELECT 
                            p.description AS ItemName,
                            br.brandName AS Brand,
                            cat.categoryName AS Category,
                            s.SizeLabel AS Size,
                            ri.Quantity,
                            ri.RefundAmount AS Refund
                        FROM ReturnItem ri
                        JOIN Bill_Item bi 
                            ON ri.Bill_ID = bi.Bill_ID 
                            AND ri.ProductSize_ID = bi.ProductSize_ID
                        JOIN ProductSize ps ON ri.ProductSize_ID = ps.ProductSize_ID
                        JOIN Product p ON ps.Product_ID = p.Product_ID
                        LEFT JOIN Brand br ON p.Brand_ID = br.Brand_ID
                        LEFT JOIN Category cat ON p.Category_ID = cat.Category_ID
                        LEFT JOIN Size s ON ps.Size_ID = s.Size_ID
                        WHERE ri.Return_ID = @ReturnId";

                    SqlParameter[] itemsParams = { new SqlParameter("@ReturnId", returnId) };
                    DataTable itemsTable = DbHelper.GetDataTable(itemsQuery, CommandType.Text, itemsParams);

                    // Build receipt data
                    var receiptData = new ReturnReceiptData
                    {
                        ReturnId = returnId,
                        OriginalBillId = Convert.ToInt32(headerRow["OriginalBillId"]),
                        ReturnDate = Convert.ToDateTime(headerRow["ReturnDate"]),
                        BillDate = Convert.ToDateTime(headerRow["BillDate"]),
                        TotalRefund = Convert.ToDecimal(headerRow["TotalRefund"]),
                        CashierName = headerRow["CashierName"].ToString(),
                        Items = new List<ReturnReceiptItem>()
                    };

                    foreach (DataRow row in itemsTable.Rows)
                    {
                        receiptData.Items.Add(new ReturnReceiptItem
                        {
                            ItemName = row["ItemName"].ToString(),
                            Brand = row["Brand"].ToString(),
                            Category = row["Category"].ToString(),
                            Size = row["Size"].ToString(),
                            Quantity = Convert.ToInt32(row["Quantity"]),
                            Refund = Convert.ToDecimal(row["Refund"])
                        });
                    }

                    return receiptData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting receipt data: {ex.Message}", ex);
            }
        }
    }

    public class ReturnResult
    {
        public int ReturnId { get; set; }
        public decimal TotalRefund { get; set; }
    }
}