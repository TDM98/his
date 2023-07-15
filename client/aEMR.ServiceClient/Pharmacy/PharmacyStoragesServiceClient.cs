
using aEMR.Infrastructure.ServiceCore;
using StorageProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyStoragesServiceClient : ServiceClientFactory<IStorages>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IStorages"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
