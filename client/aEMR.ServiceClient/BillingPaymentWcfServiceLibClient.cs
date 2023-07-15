using aEMR.Infrastructure.ServiceCore;
using BillingPaymentWcfServiceLibProxy;
namespace aEMR.ServiceClient
{
    public class BillingPaymentWcfServiceLibClient : ServiceClientFactory<IBillingPaymentWcfServiceLib>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IBillingPaymentWcfServiceLib"; }
        }
        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
