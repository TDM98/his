using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using eHCMS.Services.Core;

namespace EventManagementService
{
    public class OutstandingTaskTcpServer : AxThreadWithMessageQueue<AxEvent>
    {
        Thread _workerThread;
        private TcpListener _listener;
        private readonly ManualResetEvent _clientConnected = new ManualResetEvent(false);
        List<StreamWriter> _clientStreams = new List<StreamWriter>();
        public AsyncCallback _workerCallBack;
        #region SOCKET
        public void Start()
        {
            ThreadStart ts = new ThreadStart(Run);
            _workerThread = new Thread(ts);
            _workerThread.Start();
        }
        void Run()
        {
            try
            {
                TcpListener _listener = new TcpListener(IPAddress.Any, 4530);
                _listener.Start();
                AxLogging.AxLogger.Instance.LogDebug("Outstanding server started in Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                //Our server started
                while (true)
                {
                    _clientConnected.Reset();
                    //Waiting for client connection
                    _listener.BeginAcceptTcpClient(OnBeginAccept, _listener);
                    _clientConnected.WaitOne(); //Block until client connects
                }
            }
            catch(Exception ex)
            {
                AxLogging.AxLogger.Instance.LogDebug("Outstanding server failed. Exception thrown: " + ex.ToString());
                //TODO: Log error message.
            }
        }
        private void OnBeginAccept(IAsyncResult asyncResult)
        {
            try
            {
                // Signal the main thread to continue.
                _clientConnected.Set();
                TcpListener listener = (TcpListener) asyncResult.AsyncState;
                var client = listener.EndAcceptTcpClient(asyncResult);
                
                byte[] bytesFrom = new byte[10025];

                if (client.Connected)
                {
                    var sw = new StreamWriter(client.GetStream());
                    sw.AutoFlush = true;
                    sw.Write("CONNECTED");
                    //_clientStreams.Add(sw);
                }
                WaitForClientRequest(client);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }

        public void Stop()
        {
            AxLogging.AxLogger.Instance.LogDebug("Outstanding server has stopped");
            _listener.Stop();
            _listener = null;
        }
        public void SendData(string data)
        {
            //if (_clientStreams != null)
            //{
            //    foreach (var writer in _clientStreams)
            //    {
            //        if (writer != null)
            //        {
            //            writer.Write(data);
            //        }
            //    }
            //}
        } 

        public void WaitForClientRequest(TcpClient client)
        {
            byte[] data = new byte[1];
            if (_workerCallBack == null)
            {
                _workerCallBack = new AsyncCallback(OnDataReceived);
            }
            client.Client.BeginReceive(data, 0, 0, SocketFlags.None, _workerCallBack, client);
            
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                string content = string.Empty;

                TcpClient client = (TcpClient)asyn.AsyncState;

                int bytesRead = client.Client.EndReceive(asyn);

                //if (bytesRead > 0)
                {
                    byte[] data = new byte[1];
                    //Doc thong tin tu client gui len.
                    var sr = new StreamReader(client.GetStream());
                   
                    XmlSerializer xs = new XmlSerializer(typeof(OutstandingTaskEventData));
                    OutstandingTaskEventData clientData = (OutstandingTaskEventData)xs.Deserialize(sr);

                    client.Client.BeginReceive(data, 0, data.Length, SocketFlags.None, _workerCallBack, client);
                }

            }
            catch (System.Net.Sockets.SocketException es)
            {
                if (es.ErrorCode != 64)
                {
                    Console.WriteLine(string.Format("ReadCallback Socket Exception: {0}, {1}.", es.ErrorCode, es.ToString()));
                }
            }
            catch (Exception e)
            {
                if (e.GetType().FullName != "System.ObjectDisposedException")
                {
                    Console.WriteLine(string.Format("ReadCallback Exception: {0}.", e.ToString()));
                }
            }
        }
        #endregion

        /// <summary>
        /// //////////////////////////////
        /// </summary>
        readonly object _locker = new object();
        //private Dictionary<AxEvent, List<string>> _registeredEvents = new Dictionary<AxEvent, List<string>>();
        //private bool CheckIfEventRegistered(string sessionID, AxEvent evt)
        //{
        //    if (!_registeredEvents.ContainsKey(evt))
        //        return false;
        //    return _registeredEvents[evt].Exists((e) => e == sessionID);
        //}

        //public void SubscribeEvents(string sessionID, List<AxEvent> evtList)
        //{
        //    lock (_locker)
        //    {
        //        foreach (var axEvent in evtList)
        //        {
        //            if(_registeredEvents.ContainsKey(axEvent))
        //            {
        //                if(!_registeredEvents[axEvent].Exists((e) => e == sessionID))
        //                {
        //                    _registeredEvents[axEvent].Add(sessionID);
        //                }
        //            }
        //            else
        //            {
        //                _registeredEvents.Add(axEvent,new List<string>(){sessionID});
        //            }
        //        }
        //    }
        //}

        //public void UnsubscribeEvents(string sessionID, List<AxEvent> evtList)
        //{
        //    lock (_locker)
        //    {
        //        foreach (var axEvent in evtList)
        //        {
        //            if (_registeredEvents.ContainsKey(axEvent))
        //            {
        //                if (_registeredEvents[axEvent].Exists((e) => e == sessionID))
        //                {
        //                    _registeredEvents[axEvent].Add(sessionID);
        //                }
        //            }
        //        }
        //    }
        //}

        private Dictionary<AxEvent, List<TcpClient>> _registeredClients = new Dictionary<AxEvent, List<TcpClient>>();
        /// <summary>
        /// Kiểm tra xem client đã đăng ký cho sự kiện này chưa.
        /// </summary>
        /// <param name="client">TCPClient đăng ký sự kiện</param>
        /// <param name="evt">Loại sự kiện đăng ký</param>
        /// <returns></returns>
        private bool CheckIfEventRegistered(TcpClient client, AxEvent evt)
        {
            if (!_registeredClients.ContainsKey(evt))
                return false;
            return _registeredClients[evt].Exists((e) => e == client);
        }

        private void SubscribeEvents(TcpClient client, List<AxEvent> evtList)
        {
            lock (_locker)
            {
                foreach (var axEvent in evtList)
                {
                    if (_registeredClients.ContainsKey(axEvent))
                    {
                        if (!_registeredClients[axEvent].Exists((e) => e == client))
                        {
                            _registeredClients[axEvent].Add(client);
                        }
                    }
                    else
                    {
                        _registeredClients.Add(axEvent, new List<TcpClient>() { client });
                    }
                }
            }
        }

        private void UnsubscribeEvents(TcpClient client, List<AxEvent> evtList)
        {
            lock (_locker)
            {
                foreach (var axEvent in evtList)
                {
                    if (_registeredClients.ContainsKey(axEvent))
                    {
                        if (_registeredClients[axEvent].Exists((e) => e == client))
                        {
                            _registeredClients[axEvent].Add(client);
                        }
                    }
                }
            }
        }
    }

    public enum OutstandingTaskActionsEnum
    {
        Register = 0,
        Unregister = 1
    }
    public class OutstandingTaskEventData
    {
        public OutstandingTaskActionsEnum Action { get; set; }
        public AxEvent Event { get; set; }

        public string ToXml()
        {
            var sw = new StringWriter();
            var xm = new XmlSerializer(typeof(OutstandingTaskEventData));
            xm.Serialize(sw, this);
            return sw.ToString();
        }
    }
}
