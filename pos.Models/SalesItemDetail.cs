using System.Data;

namespace pos_system.pos.Models
{
    public class SalesItemDetail
    {
        public int Bill_ID { get; set; }
        public DateTime SaleDate { get; set; }
        public string ItemDescription { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
    }
}