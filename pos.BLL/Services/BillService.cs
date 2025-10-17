using pos_system.pos.Models;
using RetailPOS.DAL.Repositories;
using System.Data; // Add this for DataTable
using System.Linq; // Add this for LINQ operations

namespace pos_system.pos.BLL.Services
{
    public class BillService
    {
        private readonly BillRepository _billRepository;

        public BillService()
        {
            _billRepository = new BillRepository();
        }

        public BillSummary GetBillSummary(int billId)
        {
            try
            {
                return _billRepository.GetBillSummary(billId);
            }
            catch (Exception ex)
            {
                // Log the exception here if you have a logging framework
                throw new Exception($"Error retrieving bill summary: {ex.Message}", ex);
            }
        }

        public DataTable GetBillItemsForDisplay(int billId)
        {
            try
            {
                return _billRepository.GetBillItemsDataTable(billId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving bill items: {ex.Message}", ex);
            }
        }

        public bool ValidateBillForReprint(int billId)
        {
            try
            {
                var summary = _billRepository.GetBillSummary(billId);
                return summary?.Header != null && summary.Items?.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public DataTable SearchBills(int? billId, DateTime startDate, DateTime endDate, string customerContact)
        {
            try
            {
                // Validate date range
                TimeSpan dateDiff = endDate.Date - startDate.Date;
                if (dateDiff.TotalDays > 7)
                {
                    throw new ArgumentException("Date range cannot exceed 7 days");
                }

                return _billRepository.SearchBills(billId, startDate, endDate, customerContact);
            }
            catch (ArgumentException ex)
            {
                throw ex; // Re-throw validation exceptions
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching bills: {ex.Message}", ex);
            }
        }

        //public static DataTable SearchBills(int? billId, string contact)
        //{
        //    return BillRepository.SearchBills(billId, contact);
        //}

    }
}