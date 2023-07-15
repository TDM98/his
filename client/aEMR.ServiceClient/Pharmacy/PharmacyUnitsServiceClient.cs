
using aEMR.Infrastructure.ServiceCore;
using UnitProxy;

namespace aEMR.ServiceClient
{
    public class PharmacyUnitsServiceClient : ServiceClientFactory<IUnit>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IUnit"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
