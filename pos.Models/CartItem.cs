using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_system.pos.Models
{
    public class CartItem
    {
        public int ProductSize_ID { get; set; }
        public int Product_ID { get; set; }
        public string Barcode { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountAmountPerItem { get; set; }
        public decimal MaxDiscount { get; set; }
        public int AvailableStock { get; set; }
    }
}
