
using aEMR.Infrastructure.ServiceCore;
using CommonUtilService;
namespace aEMR.ServiceClient
{
    public class CommonUtilsServiceClient : ServiceClientFactory<ICommonUtils>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_ICommonUtils"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
