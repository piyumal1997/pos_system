using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Repositories;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace pos_system.pos.BLL.Services
{
    public class ReturnService
    {
        private readonly ReturnRepository _repository = new ReturnRepository();

        public ReturnResult ProcessReturn(int employeeId, int originalBillId, List<ReturnItem> returnItems)
        {
            return _repository.ProcessReturn(employeeId, originalBillId, returnItems);
        }

        public ReturnReceiptData GetReturnReceiptData(int returnId)
        {
            return _repository.GetReturnReceiptData(returnId);
        }

        public DataTable GetReturns(int? returnId = null, int? billId = null, bool? isUsed = null, DateTime? returnDate = null)
        {
            return _repository.GetReturns(returnId, billId, isUsed, returnDate);
        }

        public DataTable GetReturnItems(int returnId)
        {
            return _repository.GetReturnItems(returnId);
        }

        public DataTable GetTokenUsageReport()
        {
            return _repository.GetTokenUsageReport();
        }

        public decimal GetTotalRefundsByDate(DateTime date)
        {
            try
            {
                string query = @"
                    SELECT ISNULL(SUM(TotalRefund), 0) 
                    FROM [Return] 
                    WHERE CAST(ReturnDate AS DATE) = @Date 
                    AND IsUsed = 1";

                SqlParameter[] parameters = { new SqlParameter("@Date", date.Date) };
                var result = DbHelper.ExecuteScalar(query, CommandType.Text, parameters);
                return Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting total refunds: {ex.Message}", ex);
            }
        }
    }

}