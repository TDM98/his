using aEMR.Infrastructure.ServiceCore;
using CommonServiceProxy;
using PatientServiceProxy;

namespace aEMR.ServiceClient
{

    public class CommonServiceClient : ServiceClientFactory<ICommonService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_ICommonService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
