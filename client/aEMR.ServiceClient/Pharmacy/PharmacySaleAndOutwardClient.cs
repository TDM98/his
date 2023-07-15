
using aEMR.Infrastructure.ServiceCore;
using PharmacySaleAndOutwardProxy;

namespace aEMR.ServiceClient
{
    public class PharmacySaleAndOutwardClient : ServiceClientFactory<IPharmacySaleAndOutward>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IPharmacySaleAndOutward"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }

}
