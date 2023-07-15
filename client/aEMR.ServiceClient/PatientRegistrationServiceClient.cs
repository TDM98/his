using aEMR.Infrastructure.ServiceCore;
using PatientServiceProxy;

namespace aEMR.ServiceClient
{

    public class PatientRegistrationServiceClient : ServiceClientFactory<IPatientRegistrationService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IPatientRegistrationService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
