using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class BillItem
    {
        public int Item_ID { get; set; }
        public object Size_ID { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal Per_item_Discount { get; set; }
    }
}
