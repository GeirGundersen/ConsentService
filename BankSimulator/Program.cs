using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Start this simulator with a bankid as argument, typically 0000, 0010 or 0020.");
            }
            else
            {
                var bankid = args[0];
                var url = URLS.MakeURL(bankid);

                Console.WriteLine("Simulating bank {0} at {1}.", bankid, url);
                Console.WriteLine("Press enter to terminate.");

                var service = new Service(bankid, url);

                service.Start();

                Console.ReadLine();
            }
        }
    }
}
