using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EventManagementService
{
    public class OutstandingTaskSocketServer
    {
        private Socket _socketListener;
        private ManualResetEvent _clientConnected = new ManualResetEvent(false);

        public OutstandingTaskSocketServer()
        {
            _socketListener = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 4530);
            _socketListener.Bind(endpoint);
            
            Start();
        }
        public void Start()
        {
            _socketListener.Listen(5);
            //Our server started
            while (true)
            {
                _clientConnected.Reset();
                _socketListener.BeginAccept(new AsyncCallback(OnClientConnect), null);
                _clientConnected.WaitOne();
            }
        }

        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                _clientConnected.Set();

            }
            catch (ObjectDisposedException)
            {
                
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

        }
    }
}
