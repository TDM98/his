
using aEMR.Infrastructure.ServiceCore;

using PCLsProxy;

namespace aEMR.ServiceClient
{

    public class PCLsClient : ServiceClientFactory<IPCLs>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IPCLs"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }

}
