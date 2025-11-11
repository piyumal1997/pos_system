using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_system.pos.Models
{
    public class ReturnToken
    {
        public int ReturnId { get; set; }
        public decimal TotalRefund { get; set; }
        public bool IsUsed { get; set; }
        public DateTime ReturnDate { get; set; }
        public int EmployeeId { get; set; }
        public int OriginalBillId { get; set; }
    }
}
