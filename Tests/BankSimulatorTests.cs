using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Common.Models;
using ConsentRequester;

namespace Tests
{
    [TestClass]
    public class BankSimulatorTests
    {
        [TestMethod]
        public void ConsentTest()
        {
            var service = new BankSimulator.Service("0020");

            bool consent = service.Consent("3944510");

            Assert.AreEqual(true, consent);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UnknownCustomerConsentTest()
        {
            var service = new BankSimulator.Service("0020");

            bool consent = service.Consent("WrongId");
        }


        [TestMethod]
        public void RESTTest()
        {
            // This test will only work if you run Visual Studio with administrator privileges

            var baseUrl = "http://localhost:8020/";

            var service = new BankSimulator.Service("0020", baseUrl);

            var request = new ConsentRequest { Customer = "3944510" };

            var before = service.QueuedRequests;

            HttpClient client = new HttpClient();

            var response = client.PostAsync(baseUrl + "Consent",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            response.Wait();

            var after = service.QueuedRequests;

            Assert.AreEqual(1, after - before);
        }


        [TestMethod]
        public void QueuedTest()
        {
            // This test will only work if you run Visual Studio with administrator privileges

            var baseUrl = "http://localhost:8021/";

            var service = new BankSimulator.Service("0020", baseUrl);

            service.Start();

            var receiver = new ConsentReceiver();

            receiver.ListenFor("0020");

            ConsentResponse received = null;

            receiver.ConsentResponseReceived += (sender, response) =>
            {
                received = response;
            };

            var request = new ConsentRequest { Customer = "3944510" };

            HttpClient client = new HttpClient();

            var sending = client.PostAsync(baseUrl + "Consent",
                new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

            sending.Wait();

            int msecWait = 10000;

            while (received == null && msecWait > 0)
            {
                System.Threading.Thread.Sleep(100);
                msecWait -= 100;
            }

            Assert.AreNotEqual(null, received);

            Assert.AreEqual("3944510", received.Customer);

            Assert.AreEqual(true, received.Consent);
        }
    }
}
