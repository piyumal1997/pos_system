using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_system.pos.Models
{
    public class BillPayment
    {
        public int Payment_ID { get; set; }
        public int Bill_ID { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PaymentAmount { get; set; }
        public string CardLast4 { get; set; }
        public string BankAccountLast4 { get; set; }
        public int PaymentOrder { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
