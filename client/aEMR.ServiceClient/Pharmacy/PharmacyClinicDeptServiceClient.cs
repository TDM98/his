
using aEMR.Infrastructure.ServiceCore;
using ClinicDeptProxy;

namespace aEMR.ServiceClient
{

    public class PharmacyClinicDeptServiceClient : ServiceClientFactory<IInClinicDept>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IInClinicDept"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
