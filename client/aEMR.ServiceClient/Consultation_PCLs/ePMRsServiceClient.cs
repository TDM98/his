
using aEMR.Infrastructure.ServiceCore;
using ePMRsService;

namespace aEMR.ServiceClient
{
    public class ePMRsServiceClient : ServiceClientFactory<IePMRs>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IePMRs"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
