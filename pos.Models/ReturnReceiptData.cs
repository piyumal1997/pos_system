using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class ReturnReceiptData
    {
        public int ReturnId { get; set; }
        public int OriginalBillId { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime BillDate { get; set; }
        public decimal TotalRefund { get; set; }
        public string CashierName { get; set; }
        public List<ReturnReceiptItem> Items { get; set; }
    }
}