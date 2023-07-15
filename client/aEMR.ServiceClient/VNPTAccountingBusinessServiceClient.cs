using aEMR.Infrastructure.ServiceCore;
using VNPTAccountingServiceProxy;

namespace aEMR.ServiceClient
{
    public class VNPTAccountingBusinessServiceClient : ServiceClientFactory<IVNPTAccountingBusinessService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_BusinessService"; }
        }
        public override string EndPointAddress
        {
            get { return "http://tempuri.org"; }
        }
    }
}