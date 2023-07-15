

using aEMR.Infrastructure.ServiceCore;
using ClinicManagementProxy;

namespace aEMR.ServiceClient
{
    public class ClinicManagementServiceClient: ServiceClientFactory<IClinicManagementService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IClinicManagementService"; }
        }
        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
