using Common.Models;
using ConsentRequester;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class PusherTests
    {
        [TestMethod]
        public void SendAndReceiveTest()
        {
            var bank = new BankSimulator.Service("0020");

            var receiver = new ConsentReceiver();

            receiver.ListenFor("0020");

            ConsentResponse received = null;

            receiver.ConsentResponseReceived += (sender, response) =>
            {
                received = response;
            };

            bank.SendResponse("123456", true);

            int msecWait = 10000;

            while(received == null && msecWait > 0)
            {
                System.Threading.Thread.Sleep(100);
                msecWait -= 100;
            }

            Assert.AreNotEqual(null, received);

            Assert.AreEqual("123456", received.Customer);

            Assert.AreEqual(true, received.Consent);
        }
    }
}
