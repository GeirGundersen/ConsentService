using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsentRequester
{
    public class BankClient
    {
        private string bankId;

        private string url;

        public BankClient(string bankId)
        {
            this.bankId = bankId;
        }

        public BankClient(string bankId, string url) : this(bankId)
        {
            this.url = url;
        }

        public string Url { get { return this.url; } }

        public void RequestConsent(string customer)
        {
            HttpClient client = new HttpClient();

            var request = new ConsentRequest { Customer = customer };

            var response = client.PostAsync(url + "Consent",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
        }
    }
}
