using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_system.pos.Models
{
    public class QueuedBill
    {
        public int Queue_ID { get; set; }
        public int Bill_ID { get; set; }
        public int QueuePosition { get; set; }
        public DateTime PausedAt { get; set; }
        public string CartData { get; set; }
        public int ItemCount { get; set; }
        public decimal SubTotal { get; set; }
    }
}
