
using aEMR.Infrastructure.ServiceCore;
using MedDeptProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyMedDeptServiceClient : ServiceClientFactory<IInMedDept>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IInMedDept"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
