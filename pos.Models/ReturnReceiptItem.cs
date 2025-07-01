using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class ReturnReceiptItem
    {
        public string ItemName { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Refund { get; set; }
    }
}