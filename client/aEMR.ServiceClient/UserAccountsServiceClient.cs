
using aEMR.Infrastructure.ServiceCore;
using UserAccountsProxy;

namespace aEMR.ServiceClient
{
    public class UserAccountsServiceClient : ServiceClientFactory<IUserAccounts>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IUserAccounts"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
