using System;
using System.Linq;

namespace pos_system.pos.Models
{
    public class ReturnItem
    {
        public int BillId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int ReasonId { get; set; }
        public bool IsRestocked { get; set; }
    }
}
