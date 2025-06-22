using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class Item
    {
        public int Item_ID { get; set; }
        public int quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal unitCost { get; set; }
        public decimal maxDiscount { get; set; }
        public string description { get; set; }
        public string barcode { get; set; }
        public int MinStockLevel { get; set; }
        public bool IsDeleted { get; set; }
        public int Brand_ID { get; set; }
        public int Category_ID { get; set; }
        public int? Size_ID { get; set; }  // Nullable for items without size
        public byte[] ItemImage { get; set; }
        public int Gender_ID { get; set; }

        // Navigation properties
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string SizeLabel { get; set; }  // For display purposes
    }
}



