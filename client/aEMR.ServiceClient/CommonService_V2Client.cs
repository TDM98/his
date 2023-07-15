using aEMR.Infrastructure.ServiceCore;
using CommonService_V2Proxy;
namespace aEMR.ServiceClient
{
    public class CommonService_V2Client : ServiceClientFactory<ICommonService_V2>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_ICommonService_V2"; }
        }
        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
