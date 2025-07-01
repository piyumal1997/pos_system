using System.Data;

namespace pos_system.pos.Models
{
    public class ReturnItemDetail
    {
        public int Return_ID { get; set; }
        public DateTime ReturnDate { get; set; }
        public int Bill_ID { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal RefundValue { get; set; }
        public string Reason { get; set; }
    }

}