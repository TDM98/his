using AppointmentServiceProxy;
using aEMR.Infrastructure.ServiceCore;

namespace aEMR.ServiceClient
{

    public class AppointmentServiceClient : ServiceClientFactory<IAppointmentService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IAppointmentService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
