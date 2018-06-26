using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BankSimulator
{
    public class Service : IConsentService
    {
        private WebServiceHost host;

        public Service(string bankId)
        {



            Uri baseAddress = new Uri(URL);

            // Create the ServiceHost.
            host = new WebServiceHost(this, baseAddress);

        }


    }
}
