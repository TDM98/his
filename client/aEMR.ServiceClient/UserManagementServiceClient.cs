
using aEMR.Infrastructure.ServiceCore;
using UserServiceProxy;

namespace aEMR.ServiceClient
{

    public class UserManagementServiceClient : ServiceClientFactory<IUserManagementService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IUserManagementService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }

}
