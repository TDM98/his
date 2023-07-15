using System.Collections.Generic;
using System.Threading;
using DataEntities;
using System.Threading.Tasks;
using System;
using eHCMS.Services.Core;
namespace EventManagementService
{
    public class AxDuplexEventManager : AxThreadWithMessageQueue<AxEvent>
    {
        readonly object _locker = new object();
        public AxDuplexEventManager()
            : base()
        {
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////
        /// </summary>
        /// 
        private List<string> _disconnectedClients = new List<string>();
        private Dictionary<string, IEventManagerCallback> _registeredClients = new Dictionary<string, IEventManagerCallback>();
        public Dictionary<string, IEventManagerCallback> RegisteredClients
        {
            get
            {
                return _registeredClients;
            }
            set
            {
                _registeredClients = value;
            }
        }
        public void Subscribe(string sessionID, IEventManagerCallback evtMgr)
        {
            lock (_locker)
            {
                if (!_registeredClients.ContainsKey(sessionID))
                {
                    _registeredClients.Add(sessionID, evtMgr);
                }
            }
        }
        public void UnSubscribe(string sessionID)
        {
            lock (_locker)
            {
                if (_registeredClients.ContainsKey(sessionID))
                {
                    _registeredClients.Remove(sessionID);
                }
            }
        }
        public void SubscribeEvents(string sessionID, List<AxEvent> evtList)
        {
            lock (_locker)
            {
                if (!_registeredEvents.ContainsKey(sessionID))
                {
                    _registeredEvents.Add(sessionID, evtList);
                }
                else
                {
                    foreach (AxEvent evt in evtList)
                    {
                        if (!_registeredEvents[sessionID].Exists((e) => e.EventType == evt.EventType))
                        {
                            _registeredEvents[sessionID].Add(evt);
                        }
                    }

                }
            }
        }
        public override void ProcessEvent(AxEvent evt)
        {
            try
            {
                lock (_locker)
                {
                    try
                    {
                        if (_disconnectedClients != null)
                        {
                            foreach (string clientID in _disconnectedClients)
                            {
                                _registeredClients.Remove(clientID);
                            }
                            _disconnectedClients.Clear();
                        }
                    }
                    catch
                    {

                    }
                }

                //IEventManagerCallback callback;
                foreach (string key in _registeredClients.Keys)
                //foreach (IEventManagerCallback callback in _registeredClients.Values)
                {
                    //Kiem tra neu event nay dc user dang ky chua?
              
                    var callback = _registeredClients[key];
                    if (callback != null)
                    {
                        if (CheckIfEventRegistered(key,evt))
                        {
                            var curTask = Task.Factory.StartNew((sessionID) =>
                                            {
                                                try
                                                {
                                                    callback.Receive(evt);
                                                }
                                                catch 
                                                {
                                                    System.Diagnostics.Debug.WriteLine("Cannot make a call to client");
                                                    lock (_locker)
                                                    {
                                                        //_disconnectedClients.Add(key);
                                                        _disconnectedClients.Add((string)sessionID);
                                                    }
                                                }
                                                //callback.Receive(evt);
                                            }, key); 
                        }
                    }
                }
            }
            catch
            {

            }
        }
        /// <summary>
        /// Test.
        /// 09.12.2010
        /// Danh sach cac events 1 user dang ky
        /// </summary>


        private Dictionary<string, List<AxEvent>> _registeredEvents = new Dictionary<string, List<AxEvent>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        private bool CheckIfEventRegistered(string sessionID, AxEvent evt)
        {
            if (!_registeredEvents.ContainsKey(sessionID))
                return false;
            return _registeredEvents[sessionID].Exists((e) => e.EventType == evt.EventType);
        }
    }
}
