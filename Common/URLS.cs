using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class URLS
    {
        public static string MakeURL(string bankid)
        {
            return string.Format("http://localhost:{0}/", int.Parse(bankid) + 40000);
        }
    }
}
