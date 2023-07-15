
using aEMR.Infrastructure.ServiceCore;
using RefGenMedProductDetailsServiceProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyGenMedDeptDetailsServiceClient : ServiceClientFactory<IRefGenMedProductDetails>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IRefGenMedProductDetails"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
