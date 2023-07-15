
using aEMR.Infrastructure.ServiceCore;
using EstimateDrugDeptProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyEstimattionServiceClient : ServiceClientFactory<IEstimateDrugDeptService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IEstimateDrugDeptService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
