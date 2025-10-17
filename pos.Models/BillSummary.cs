using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class BillSummary
    {
        public BillHeader Header { get; set; }
        public List<BillItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalPerItemDiscount { get; set; }
        public decimal TotalBillDiscount { get; set; }
        public decimal Total { get; set; }
        public int Bill_ID { get; set; }
        public string PaymentMethod { get; set; }
        public int Employee_ID { get; set; }
        public string Discount_Method { get; set; }
        public string CustomerContact { get; set; }
        public decimal? Token_Value { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal CashPayment { get; set; }
    }
}
