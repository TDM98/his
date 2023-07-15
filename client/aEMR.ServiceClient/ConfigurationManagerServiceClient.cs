using aEMR.Infrastructure.ServiceCore;
using ConfigurationManagerServiceProxy;


namespace aEMR.ServiceClient
{

    public class ConfigurationManagerServiceClient : ServiceClientFactory<IConfigurationManagerService>
    {
        public override string EndPointName
        {
            get { return "BasicHttpBinding_IConfigurationManagerService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
