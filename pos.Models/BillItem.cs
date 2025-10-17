using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class BillItem
    {
        public int ProductSize_ID { get; set; }
        public int Quantity { get; set; }
        public decimal ItemSellingPrice { get; set; }
        public decimal Per_item_Discount { get; set; }
        public string Description { get; set; }
        public decimal PerItemDiscount { get; set; }
        public decimal NetPrice { get; set; }
    }
}
