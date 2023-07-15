
using aEMR.Infrastructure.ServiceCore;
using InwardDrugProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyInwardDrugServiceClient : ServiceClientFactory<IInwardDrug>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IInwardDrug"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
