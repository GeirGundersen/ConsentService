using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using Common;
using Common.Models;
using PusherServer;
using System.ComponentModel;
using System.Threading;

namespace BankSimulator
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IConsentService
    {
        private string bankId;

        private Queue<ConsentRequest> queuedRequests = new Queue<ConsentRequest>();

        private WebServiceHost host;

        private List<BankCustomer> customers;

        private BackgroundWorker worker;

        public Service(string bankId, string URL = null)
        {
            this.bankId = bankId;

            var data = from c in CSVFile.ReadFile(CSVFile.FindFile("testdata.csv"))
                       where c.BankId == bankId
                       select new BankCustomer
                       {
                           Id = c.CustomerId,
                           Consent = c.ConsentBank,
                           PostCode = c.PostCode,
                           City = c.City
                       };

            customers = new List<BankCustomer>(data);

            if (URL != null)
            {
                Uri baseAddress = new Uri(URL);
                host = new WebServiceHost(this, baseAddress);
                host.Open();
            }
        }

        public void Start()
        {
            worker = new BackgroundWorker();
            worker.DoWork += ProcessQueuedRequests;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        private void ProcessQueuedRequests(object sender, DoWorkEventArgs e)
        {
            while(!e.Cancel)
            {
                while(queuedRequests.Count() > 0)
                {
                    var request = queuedRequests.Dequeue();
                    try
                    {
                        SendResponse(request.Customer, Consent(request.Customer));
                    }
                    catch(KeyNotFoundException notfound)
                    {
                        Console.WriteLine(notfound.Message);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        queuedRequests.Enqueue(request);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            worker.CancelAsync();
        }

        public bool Consent(string customerId)
        {
            var customer = customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
                throw new KeyNotFoundException(string.Format("Customer {0} was not found", customerId));
            return customer.Consent;
        }

        void IConsentService.Consent(ConsentRequest request)
        {
            queuedRequests.Enqueue(request);

            Console.WriteLine("Request received for customer {0}. {1} in queue.", request.Customer, QueuedRequests);
        }

        public int QueuedRequests
        {
            get
            {
                return queuedRequests.Count;
            }
        }

        public void SendResponse(string customer, bool consent)
        {
            var options = new PusherOptions
            {
                Cluster = "eu",
                Encrypted = true
            };

            var pusher = new Pusher(
              "550686",
              "3d986af38ba72547d258",
              "67f609126ac55f7698f7",
              options);

            bool finished = false;
            int attempts = 5;

            while (!finished && attempts-- > 0)
            {
                try
                {
                    var result = pusher.TriggerAsync(
                      bankId,
                      "Consent",
                      new ConsentResponse
                      {
                          Customer = customer,
                          Bank = bankId,
                          Consent = consent
                      });

                    result.Wait();

                    if (result.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Sent response {0} for customer {1}", consent, customer);
                        finished = true;
                    }
                    else
                    {
                        Console.Write("Error sending response: {0}", result.Result.StatusCode);
                        // Retry very soon
                        Thread.Sleep(100);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    if (attempts == 0)
                        finished = true;
                }
            }
        }
    }
}
