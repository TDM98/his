

using aEMR.Infrastructure.ServiceCore;
using ePrescriptionService;

namespace aEMR.ServiceClient
{
    public class ePrescriptionsServiceClient : ServiceClientFactory<IePrescriptions>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IePrescriptions"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
