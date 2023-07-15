

using aEMR.Infrastructure.ServiceCore;
using ePrescriptionService;
namespace aEMR.ServiceClient
{
    public class PatientInstructionServiceClient : ServiceClientFactory<IPatientInstructionService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IPatientInstructions"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
