using aEMR.Infrastructure.ServiceCore;
using CommonRecordsService;

namespace aEMR.ServiceClient.Consultation_PCLs
{
    public class CommonRecordsServiceClient : ServiceClientFactory<ICommonRecordsService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_ICommonRecords"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
