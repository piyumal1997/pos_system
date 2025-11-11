using Microsoft.Data.SqlClient;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static pos_system.pos.UI.Forms.Sales.BillingForm;

namespace pos_system.pos.DAL.Repositories
{
    public class SalesRepository
    {
        public ReturnToken GetTokenDetails(int tokenId)
        {
            ReturnToken token = null;

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT Return_ID, TotalRefund, IsUsed, ReturnDate, Employee_ID, OriginalBill_ID 
                    FROM [Return] 
                    WHERE Return_ID = @TokenId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TokenId", tokenId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            token = new ReturnToken
                            {
                                ReturnId = reader.GetInt32("Return_ID"),
                                TotalRefund = reader.GetDecimal("TotalRefund"),
                                IsUsed = reader.GetBoolean("IsUsed"),
                                ReturnDate = reader.GetDateTime("ReturnDate"),
                                EmployeeId = reader.GetInt32("Employee_ID"),
                                OriginalBillId = reader.GetInt32("OriginalBill_ID")
                            };
                        }
                    }
                }
            }

            return token;
        }

        public List<QueuedBill> GetQueuedBills(int employeeId)
        {
            var queuedBills = new List<QueuedBill>();

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                    SELECT 
                        q.Queue_ID,
                        q.Bill_ID,
                        q.Employee_ID,
                        q.QueuePosition,
                        q.PausedAt,
                        q.CartData,
                        q.IsActive,
                        CASE 
                            WHEN ISNUMERIC(JSON_VALUE(q.CartData, '$.ItemCount')) = 1 
                            THEN CAST(JSON_VALUE(q.CartData, '$.ItemCount') AS INT)
                            ELSE 0 
                        END AS ItemCount,
                        CASE 
                            WHEN ISNUMERIC(JSON_VALUE(q.CartData, '$.Subtotal')) = 1 
                            THEN CAST(JSON_VALUE(q.CartData, '$.Subtotal') AS DECIMAL(10,2))
                            ELSE 0.00 
                        END AS SubTotal
                    FROM BillQueue q
                    WHERE q.Employee_ID = @EmployeeID AND q.IsActive = 1
                    ORDER BY q.QueuePosition";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var queuedBill = new QueuedBill
                            {
                                Queue_ID = reader.GetInt32("Queue_ID"),
                                Bill_ID = reader.GetInt32("Bill_ID"),
                                Employee_ID = reader.GetInt32("Employee_ID"),
                                QueuePosition = reader.GetInt32("QueuePosition"),
                                PausedAt = reader.GetDateTime("PausedAt"),
                                CartData = reader.GetString("CartData"),
                                IsActive = reader.GetBoolean("IsActive"),
                                ItemCount = reader.GetInt32("ItemCount"),
                                SubTotal = reader.GetDecimal("SubTotal")
                            };
                            queuedBills.Add(queuedBill);
                        }
                    }
                }
            }

            return queuedBills;
        }

        public bool DeleteQueuedBill(int queueId)
        {
            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // First get the Bill_ID to update the Bill table
                    string getBillIdQuery = "SELECT Bill_ID FROM BillQueue WHERE Queue_ID = @QueueId";
                    int billId;

                    using (var cmd = new SqlCommand(getBillIdQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueueId", queueId);
                        var result = cmd.ExecuteScalar();
                        if (result == null) return false;
                        billId = (int)result;
                    }

                    // Update Bill status to Abandoned
                    string updateBillQuery = "UPDATE Bill SET BillStatus = 'Abandoned' WHERE Bill_ID = @BillId";
                    using (var cmd = new SqlCommand(updateBillQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillId", billId);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from BillQueue
                    string deleteQuery = "DELETE FROM BillQueue WHERE Queue_ID = @QueueId";
                    using (var cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueueId", queueId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Error deleting queued bill: {ex.Message}");
                return false;
            }
        }

        public bool RestorePausedBill(int queueId, int employeeId, out int billId, out string cartData)
        {
            billId = 0;
            cartData = null;

            try
            {
                using (var conn = DbHelper.GetConnection())
                {
                    conn.Open();

                    // Get the BillID and CartData from the queue
                    string getQuery = @"
                        SELECT Bill_ID, CartData 
                        FROM BillQueue 
                        WHERE Queue_ID = @QueueID AND Employee_ID = @EmployeeID AND IsActive = 1";

                    using (var cmd = new SqlCommand(getQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueueID", queueId);
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                billId = reader.GetInt32("Bill_ID");
                                cartData = reader.GetString("CartData");
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }

                    // Update the Bill status to 'Active'
                    string updateBillQuery = "UPDATE Bill SET BillStatus = 'Active' WHERE Bill_ID = @BillID";
                    using (var cmd = new SqlCommand(updateBillQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@BillID", billId);
                        cmd.ExecuteNonQuery();
                    }

                    // Mark the queue record as inactive
                    string updateQueueQuery = "UPDATE BillQueue SET IsActive = 0 WHERE Queue_ID = @QueueID";
                    using (var cmd = new SqlCommand(updateQueueQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueueID", queueId);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring paused bill: {ex.Message}");
                return false;
            }
        }
    }
}
