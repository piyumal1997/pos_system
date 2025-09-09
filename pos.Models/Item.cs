using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class Item
    {
        public int Product_ID { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public int MinStockLevel { get; set; }
        public decimal MaxDiscount { get; set; }
        public bool IsDeleted { get; set; }
        public int Brand_ID { get; set; }
        public int Category_ID { get; set; }
        public int Gender_ID { get; set; }
        public string ItemImage { get; set; }
        public string TempImagePath { get; set; }

        // Navigation properties
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string GenderName { get; set; }

        // Size variants
        public List<ProductSize> Sizes { get; set; } = new List<ProductSize>();

        // For barcode printing
        public int ProductSize_ID { get; set; }
        public string SizeLabel { get; set; }
        public int Quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal UnitCost { get; set; }
        public string SizesSummary { get; internal set; }
        public Image ImageObject { get; set; }
    }

    public class ProductSize
    {
        public int ProductSize_ID { get; set; }
        public int Product_ID { get; set; }
        public int? Size_ID { get; set; }
        public string SizeLabel { get; set; }
        public int Quantity { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal UnitCost { get; set; }
    }
}