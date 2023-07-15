
using aEMR.Infrastructure.ServiceCore;
using DrugProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyDrugServiceClient : ServiceClientFactory<IDrugs>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IDrugs"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
