using System.Data;

namespace pos_system.pos.Models
{
    public class ReturnItemDetail
    {
        public int Return_ID { get; set; }
        public DateTime ReturnDate { get; set; }
        public int OriginalBill_ID { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal RefundValue { get; set; }
        public string Reason { get; set; }
        public string RefundMethod { get; set; }
        public int? TokenUsedInBill { get; set; }
        public bool IsUsed { get; set; }
        public DateTime? UsedDate { get; set; }
        public string ProcessedBy { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public bool IsRestocked { get; set; }
    }

}