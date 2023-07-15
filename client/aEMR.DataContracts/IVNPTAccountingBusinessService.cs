using System;
using System.ServiceModel;

namespace VNPTAccountingServiceProxy
{
    [ServiceContract]
    public interface IVNPTAccountingBusinessService
    {
        [OperationContractAttribute(Action = "http://tempuri.org/AdjustInvoiceAction", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BeginAdjustInvoiceAction(string Account, string ACpass, string xmlInvData, string username, string pass, string fkey, string AttachFile, System.Nullable<int> convert, string pattern, string serial, AsyncCallback callback, object state);
        string EndAdjustInvoiceAction(IAsyncResult asyncResult);

        [OperationContractAttribute(Action = "http://tempuri.org/cancelInv", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BegincancelInv(string Account, string ACpass, string fkey, string userName, string userPass, AsyncCallback callback, object state);
        string EndcancelInv(IAsyncResult asyncResult);
    }
}