using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using eHCMS.Services.Core.Base;
using AxLogging;
namespace eHCMS
{
    //public class WCFServiceBase
    //{
    //    public UserAccount CurrentUser 
    //    {
    //        get
    //        {
    //            MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
    //            try
    //            {
    //                return headers.GetHeader<UserAccount>("AuthenticatedUser", "eHCMS");
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //    }
    //}
    public class WCFServiceBase<T> where T: NotifyChangedBase
    {
        public T CurrentUser
        {
            get
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
                try
                {
                    //if (headers.FindHeader("CurrentLoggedUser","eHCMS") > -1 )
                    //    return headers.GetHeader<string>("CurrentLoggedUser", "eHCMS");                    
                    return default(T);
                }
                catch(Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    return null;
                }
            }
        }
    }


    public class WCFServiceCustomHeader
    {
        public string CurrentUser
        {
            get
            {
                MessageHeaders headers = OperationContext.Current.IncomingMessageHeaders;
                try
                {
                    if (headers.FindHeader("CurrentLoggedUser", "eHCMS") > -1)
                        return headers.GetHeader<string>("CurrentLoggedUser", "eHCMS");
                    return "Unknown-User";
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    return null;
                }
            }
        }
    }

}
