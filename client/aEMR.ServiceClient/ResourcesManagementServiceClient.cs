
using aEMR.Infrastructure.ServiceCore;
using ResourcesManagementProxy;

namespace aEMR.ServiceClient
{
    public class ResourcesManagementServiceClient : ServiceClientFactory<IResourcesManagementService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IResourcesManagementService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
