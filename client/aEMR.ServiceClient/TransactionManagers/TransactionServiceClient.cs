using aEMR.Infrastructure.ServiceCore;
using TransactionServiceProxy;

namespace aEMR.ServiceClient
{
    public class TransactionServiceClient : ServiceClientFactory<ITransactionService>
    {

        public override string EndPointName
        {
            get { return "BasicHttpBinding_ITransactionService"; }
        }

        public override string EndPointAddress
        {
            get { return "http://192.168.1.98:9000/"; }
        }
    }
}
