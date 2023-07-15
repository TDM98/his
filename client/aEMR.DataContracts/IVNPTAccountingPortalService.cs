using System;
using System.ServiceModel;

namespace VNPTAccountingServiceProxy
{
    [ServiceContract]
    public interface IVNPTAccountingPortalService
    {
        [OperationContractAttribute(Action = "http://tempuri.org/getCus", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BegingetCus(string cusCode, string userName, string userPass, AsyncCallback callback, object state);
        string EndgetCus(IAsyncResult asyncResult);

        [OperationContractAttribute(Action = "http://tempuri.org/downloadInvNoPay", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BegindownloadInvNoPay(string invToken, string userName, string userPass, AsyncCallback callback, object state);
        string EnddownloadInvNoPay(IAsyncResult asyncResult);

        [OperationContractAttribute(Action = "http://tempuri.org/downloadInvFkey", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BegindownloadInvFkey(string fkey, string userName, string userPass, AsyncCallback callback, object state);
        string EnddownloadInvFkey(IAsyncResult asyncResult);

        [OperationContractAttribute(Action = "http://tempuri.org/downloadInvPDFFkeyNoPay", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BegindownloadInvPDFFkeyNoPay(string fkey, string userName, string userPass, AsyncCallback callback, object state);
        string EnddownloadInvPDFFkeyNoPay(IAsyncResult asyncResult);
    }
}
