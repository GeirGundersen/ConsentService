using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ConsentResponse
    {
        public string Bank { get; set; }

        public string Customer { get; set; }

        public bool Consent { get; set; }
    }
}
