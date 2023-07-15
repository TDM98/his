
using aEMR.Infrastructure.ServiceCore;
using BedAllocationsProxy;

namespace aEMR.ServiceClient
{
    public class BedAllocationsServiceClient : ServiceClientFactory<IBedAllocations>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IBedAllocations"; }
        }
        
        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
