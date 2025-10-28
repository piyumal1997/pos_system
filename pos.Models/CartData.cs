using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static pos_system.pos.UI.Forms.Sales.BillingForm;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace pos_system.pos.Models
{
    public class CartData
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal BillDiscountPercentage { get; set; }
        public bool IsBillDiscountApplied { get; set; }
        public ReturnToken AppliedToken { get; set; }
        public bool TokenApplied { get; set; }
        public int ItemCount { get; set; }
    }
}
