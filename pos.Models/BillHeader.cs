namespace pos_system.pos.Models
{
    public class BillHeader
    {
        public DateTime BillDate { get; set; }
        public string PaymentMethod { get; set; }
        public string DiscountMethod { get; set; }
        public decimal BillDiscount { get; set; }
        public string CustomerContact { get; set; }
        public string CardLast4 { get; set; }
        public string BankLast4 { get; set; }
        public int? TokenReturnID { get; set; }
        public decimal? TokenValue { get; set; }
        public string CashierName { get; set; }
    }
}