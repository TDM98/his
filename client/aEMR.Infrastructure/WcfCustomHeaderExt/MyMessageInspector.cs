using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Diagnostics;

namespace aEMR.Infrastructure.ServiceCore
{
    public class MyMessageInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply,
                                        object correlationState)
        {
            //Debug.WriteLine("SOAP Response: {0}", reply.ToString());
        }

        public object BeforeSendRequest(ref Message request,
                                        IClientChannel channel)
        {
            string STR_Customer_Unique_Id = "UnknownUser";
            if (Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.AccountName != null && Globals.LoggedUserAccount.AccountName.Length > 0)
            {
                STR_Customer_Unique_Id = Globals.LoggedUserAccount.AccountName;
            }
            var header = MessageHeader.CreateHeader("CurrentLoggedUser", 
                                                    "eHCMS", 
                                                    STR_Customer_Unique_Id);
            request.Headers.Add(header);
            //Debug.WriteLine("SOAP Request: {0}", request.ToString());
            return null;
        }
        #endregion
    }
}
