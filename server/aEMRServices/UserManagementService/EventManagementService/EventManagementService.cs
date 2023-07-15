using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using DataEntities;

namespace EventManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Single)]
    public class EventManagementService : eHCMS.WCFServiceCustomHeader, IEventManagement
    {
        //Thread t;

        //public EventManagementService()
        //{

        //}
        //public static event SubscribeDelegate SubscribeEvt;
        //public static event NotifyDelegate NotifyEvt;

        //AxEventManager mgr;

        //public IEventManagerCallback CurrentCallback
        //{
        //    get
        //    {
        //        return OperationContext.Current.GetCallbackChannel<IEventManagerCallback>();

        //    }
        //}


        //#region IEventManagement Members

        //public bool Subscribe(UserAccount account, AxEvent evt)
        //{
        //    bool addOK = SessionManager.Instance.AddNewSession(new AxSession(account));
        //    AxEventManager.Instance.DuplexEventManager.Subscribe(account, evt, CurrentCallback);
        //    OperationContext.Current.Channel.Closing += new EventHandler(Channel_Closing);
        //    OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);
        //    return addOK;
        //}

        //void Channel_Closing(object sender, EventArgs e)
        //{
        //    IContextChannel channel = (IContextChannel)sender;

        //}

        //void Channel_Faulted(object sender, EventArgs e)
        //{
        //    IContextChannel channel = (IContextChannel)sender;
        //    string id = channel.SessionId;
        //}

        //public void Unsubscribe(UserAccount account, AxEvent evt)
        //{
        //    AxEventManager.Instance.DuplexEventManager.UnSubscribe(account, evt);
        //}

        //public void Notify(AxEvent evt)
        //{
        //    AxEventManager.Instance.DuplexEventManager.EnqueueEvent(evt);
        //}

        //#endregion
       // Thread t;

        public EventManagementService()
        {

        }

        public IEventManagerCallback CurrentCallback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IEventManagerCallback>();

            }
        }
        
        #region IEventManagement Members

        public bool Subscribe()
        {
            string sessionId = OperationContext.Current.Channel.SessionId;
            AxEventManager.Instance.DuplexEventManager.Subscribe(sessionId, CurrentCallback);
            OperationContext.Current.Channel.Closing += new EventHandler(Channel_Closing);
            OperationContext.Current.Channel.Faulted += new EventHandler(Channel_Faulted);
            return true;
        }

        public bool SubscribeEvents(List<AxEvent> evtList)
        {
            Subscribe();
            AxEventManager.Instance.DuplexEventManager.SubscribeEvents(OperationContext.Current.Channel.SessionId,evtList);
            return true;
        }

        public void Unsubscribe()
        {
            AxEventManager.Instance.DuplexEventManager.UnSubscribe(OperationContext.Current.Channel.SessionId);
        }

        /// <summary>
        /// Keep the connection alive between the client and server.
        /// The clients need to call this.
        /// </summary>
        public void KeepConnection()
        {
            string sessionId = OperationContext.Current.Channel.SessionId;
        }

        void Channel_Closing(object sender, EventArgs e)
        {
            IContextChannel channel = (IContextChannel)sender;
            AxEventManager.Instance.DuplexEventManager.UnSubscribe(channel.SessionId);
        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            IContextChannel channel = (IContextChannel)sender;
            AxEventManager.Instance.DuplexEventManager.UnSubscribe(channel.SessionId);
        }

        public void Notify(AxEvent evt)
        {
            AxEventManager.Instance.DuplexEventManager.EnqueueEvent(evt);
        }
        #endregion
        

        public static void StartOutstandingTaskServer()
        {
            PolicySocketServer server = new PolicySocketServer();
            server.StartSocketServer();
            AxEventManager.Instance.OutstdTaskServer.Start();
            //ScoreSocketServer testserver = new ScoreSocketServer();
            //testserver.StartSocketServer();
        }

        public static void StopOutstandingTaskServer()
        {
            AxEventManager.Instance.OutstdTaskServer.Stop();
        }
    }


}
