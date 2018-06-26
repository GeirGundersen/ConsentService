using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Customer
    {
        public string CustomerId { get; set; }

        public DateTime ValidFrom { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        public string ClerkId { get; set; }

        public string BankId { get; set; }

        public bool ConsentInsurance { get; set; }

        public bool ConsentBank { get; set; }
    }
}
