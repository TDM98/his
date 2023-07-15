using System;
using System.ServiceModel;

namespace VNPTAccountingServiceProxy
{
    [ServiceContract]
    public interface IVNPTAccountingPublishService
    {
        [OperationContractAttribute(Action = "http://tempuri.org/UpdateCus", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BeginUpdateCus(string XMLCusData, string username, string pass, System.Nullable<int> convert, AsyncCallback callback, object state);
        int EndUpdateCus(IAsyncResult asyncResult);

        [OperationContractAttribute(Action = "http://tempuri.org/ImportAndPublishInv", ReplyAction = "*", AsyncPattern = true)]
        IAsyncResult BeginImportAndPublishInv(string Account, string ACpass, string xmlInvData, string username, string password, string pattern, string serial, int convert, AsyncCallback callback, object state);
        string EndImportAndPublishInv(IAsyncResult asyncResult);
    }
}