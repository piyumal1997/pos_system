using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace pos_system.pos.Models
{
    public class Employee
    {
        public int Employee_ID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string nic { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string contactNo { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public int Role_ID { get; set; }
        public string RoleName { get; set; } // For display
        public byte[] picture { get; set; }
    }
}
