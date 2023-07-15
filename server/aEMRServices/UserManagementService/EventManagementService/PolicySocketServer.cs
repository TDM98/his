using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using eHCMS.Services.Core;

namespace EventManagementService
{
    class PolicySocketServer
    {
        //TcpListener _Listener = null;
        //TcpClient _Client = null;
        static ManualResetEvent _TcpClientConnected = new ManualResetEvent(false);
        const string _PolicyRequestString = "<policy-file-request/>";
        int _ReceivedLength = 0;
        byte[] _Policy = null;
        byte[] _ReceiveBuffer = null;

        Thread _workerThread;
        private void InitializeData()
        {
            string policyFile = ConfigurationManager.AppSettings["SocketPolicyFilePath"];
            using (FileStream fs = new FileStream(policyFile, FileMode.Open))
            {
                _Policy = new byte[fs.Length];
                fs.Read(_Policy, 0, _Policy.Length);
            }
            //_Policy = GetClientAccessPolicy();
            _ReceiveBuffer = new byte[_PolicyRequestString.Length];
        }
        private byte[] GetClientAccessPolicy()
        {
            string str = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <access-policy>
                              <cross-domain-access>
                                <policy>
                                  <allow-from>
                                    <domain uri=""*"" />
                                  </allow-from>
                                  <grant-to>
                                    <socket-resource port=""4530"" protocol=""tcp"" />
                                  </grant-to>
                                </policy>
                              </cross-domain-access>
                            </access-policy>";
            return new MemoryStream(Encoding.UTF8.GetBytes(str)).ToArray();
        }
        public void StartSocketServer()
        {
            ThreadStart ts = new ThreadStart(Run);
            _workerThread = new Thread(ts);
            _workerThread.Start();
        }
        TcpListener _Listener;
        void Run()
        {
            InitializeData();

            try
            {
                //Using TcpListener which is a wrapper around a Socket
                //Allowed port is 943 for Silverlight sockets policy data
                _Listener = new TcpListener(IPAddress.Any, 943);
                _Listener.Start();
                AxLogging.AxLogger.Instance.LogDebug("PolicySocket server started in Thread " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                Console.WriteLine("Policy server listening...");
                while (true)
                {
                    _TcpClientConnected.Reset();
                    Console.WriteLine("Waiting for client connection...");
                    _Listener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), _Listener);
                    _TcpClientConnected.WaitOne(); //Block until client connects
                }
            }
            catch (Exception exp)
            {
                AxLogging.AxLogger.Instance.LogDebug("PolicySocket server failed. Exception thrown: " + exp.ToString());
                LogError(exp);
            }
        }
        private void OnBeginAccept(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            client.Client.BeginReceive(_ReceiveBuffer, 0, _PolicyRequestString.Length, SocketFlags.None,
                new AsyncCallback(OnReceiveComplete), client);
        }

        private void OnReceiveComplete(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            try
            {
                _ReceivedLength += client.Client.EndReceive(ar);
                //See if there's more data that we need to grab
                if (_ReceivedLength < _PolicyRequestString.Length)
                {
                    //Need to grab more data so receive remaining data
                    client.Client.BeginReceive(_ReceiveBuffer, _ReceivedLength,
                        _PolicyRequestString.Length - _ReceivedLength,
                        SocketFlags.None, new AsyncCallback(OnReceiveComplete), client);
                    return;
                }

                //Check that <policy-file-request/> was sent from client
                string request = System.Text.Encoding.UTF8.GetString(_ReceiveBuffer, 0, _ReceivedLength);
                if (StringComparer.InvariantCultureIgnoreCase.Compare(request, _PolicyRequestString) != 0)
                {
                    //Data received isn't valid so close
                    client.Client.Close();
                    return;
                }
                //Valid request received....send policy file
                client.Client.BeginSend(_Policy, 0, _Policy.Length, SocketFlags.None,
                    new AsyncCallback(OnSendComplete), client);
            }
            catch (Exception exp)
            {
                client.Client.Close();
                LogError(exp);
            }
            _ReceivedLength = 0;
            _TcpClientConnected.Set(); //Allow waiting thread to proceed
        }

        private void OnSendComplete(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;
            try
            {
                client.Client.EndSend(ar);
            }
            catch (Exception exp)
            {
                LogError(exp);
            }
            finally
            {
                //Close client socket
                client.Client.Close();
            }
        }

        private void LogError(Exception exp)
        {
            string appFullPath = Assembly.GetCallingAssembly().Location;
            string logPath = appFullPath.Substring(0, appFullPath.LastIndexOf("\\")) + ".log";
            StreamWriter writer = new StreamWriter(logPath, true);
            try
            {
                writer.WriteLine(logPath,
                    String.Format("Error in PolicySocketServer: "
                    + "{0} \r\n StackTrace: {1}", exp.Message, exp.StackTrace));
            }
            catch { }
            finally
            {
                writer.Close();
            }
        }
    }
}
