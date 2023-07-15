
using aEMR.Infrastructure.ServiceCore;
using SupplierProxy;

namespace aEMR.ServiceClient
{
    public class PharmacySuppliersServiceClient : ServiceClientFactory<ISupplier>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_ISupplier"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
