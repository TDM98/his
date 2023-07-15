
using aEMR.Infrastructure.ServiceCore;
using SummaryService;


namespace aEMR.ServiceClient.Consultation_PCLs
{
    public class SummaryServiceClient : ServiceClientFactory<ISummary>
    {
            public override string EndPointName
            {
                get { return "BasicHttpBinding_ISummary"; }
            }

            public override string EndPointAddress
            {
                get { return "http://192.168.1.98:9000/"; }
            }
        
    }
}
