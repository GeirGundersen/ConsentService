using Common.Models;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BankSimulator
{
    [ServiceContract]
    public interface IConsentService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "/Consent/")]
        void Consent(ConsentRequest request);
    }
}