using aEMR.Infrastructure.ServiceCore;

namespace QMSService
{
    public class QMSServiceClient : ServiceClientFactory<QMSService.IQMSService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IQMSService"; }
        }
        public override string EndPointAddress
        {
            get { return "http://localhost:29090"; }
        }
    }
}