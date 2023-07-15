using ReportServiceProxy;
using aEMR.Infrastructure.ServiceCore;

namespace aEMR.ServiceClient
{
    public class ReportServiceClient : ServiceClientFactory<IAxReportService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IAxReportService"; }
        }
       
        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
