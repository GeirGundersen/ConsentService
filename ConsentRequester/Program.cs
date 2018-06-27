using Common;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsentRequester
{
    public class Program
    {
        static List<Customer> customers;

        // There is no natural end of execution because the replies are async.
        // If it is quiet for 10 seconds, we assume for this test that we are finished.

        static DateTime deadline = DateTime.MaxValue;

        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Start this program with a filename argument. Typically testdata.csv.");
            }

            ReadCustomerFile(args);

            ConsentReceiver receiver = new ConsentReceiver();
            receiver.ConsentResponseReceived += Receiver_ConsentResponseReceived;

            RequestConsents(receiver);

            WaitForResponses();

            WriteResults();
        }

        private static void ReadCustomerFile(string[] args)
        {
            var filename = CSVFile.FindFile(args[0]);

            Console.WriteLine("Fetching bank consent for customers in {0}", filename);

            customers = new List<Customer>(CSVFile.ReadFile(filename));
        }

        private static void WriteResults()
        {
            Console.WriteLine("Writing results");

            StreamWriter sw = new StreamWriter("results.json");

            sw.Write(JsonConvert.SerializeObject(customers, Formatting.Indented));

            sw.Close();
        }

        private static void WaitForResponses()
        {
            deadline = DateTime.Now.AddSeconds(10);

            Console.WriteLine("Waiting for replies");

            while (DateTime.Now < deadline)
            {
                Thread.Sleep(1000);
            }
        }

        private static void RequestConsents(ConsentReceiver receiver)
        {
            var banks = new Dictionary<string, BankClient>();

            foreach (var customer in customers
                .Where(c => c.ValidFrom.Year > 2010
                    && !string.IsNullOrEmpty(c.ClerkId)
                    && c.ConsentInsurance))
            {
                if (!banks.ContainsKey(customer.BankId))
                {
                    banks.Add(customer.BankId,
                        new BankClient(customer.BankId, URLS.MakeURL(customer.BankId)));
                    receiver.ListenFor(customer.BankId);
                }
                var bank = banks[customer.BankId];

                customer.ConsentBank = false;

                Console.WriteLine("Asking {0} for customer {1} bank consent", bank.Url, customer.CustomerId);

                banks[customer.BankId].RequestConsent(customer.CustomerId);
            }
        }

        private static void Receiver_ConsentResponseReceived(object sender, ConsentResponse response)
        {
            var customer = customers.FirstOrDefault(c => c.CustomerId == response.Customer);
            if (customer != null)
                customer.ConsentBank = response.Consent;
            Console.WriteLine("Customer {0} bank consent : {1}", customer.CustomerId, customer.ConsentBank);
            deadline = DateTime.Now.AddSeconds(10);
        }
    }
}
