using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Data.Services;
using System.Windows.Forms;
using System.IO;
using eHCMS;
using System.ServiceModel.Description;
using UserManagementService;

namespace aEMRServices
{
    class ServiceHostManager
    {
        public ServiceHostManager()
        {
            Hosts = new List<ServiceHost>();
        }
        public List<ServiceHost> Hosts { get; private set; }

        private bool Exists(ServiceHost host)
        {
            foreach (ServiceHost h in Hosts)
            {
                if (h == host)
                    return true;
            }
            return false;
        }
        public bool AddServiceHost(Type type)
        {
            ServiceHost host = new ServiceHost(type);
            return AddServiceHost(host);
        }
        public bool AddServiceHost(ServiceHost newHost)
        {
            if (Exists(newHost))
                return false;

            Hosts.Add(newHost);
            return true;
        }

        /// <summary>
        /// Open all the services
        /// </summary>
        public void Open(bool trace = false)
        {
            foreach (ServiceHost host in Hosts)
            {
                host.Open();
                if(trace)
                {
                    Trace.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff --- ") + "Service " + host.Description.ConfigurationName + " stated.");
                    Trace.Flush();
                }
            }
        }
        /// <summary>
        /// Close all the services
        /// </summary>
        public void CLose()
        {
            foreach (ServiceHost host in Hosts)
            {
                if (host != null && host.State == CommunicationState.Opened)
                    host.Close();
            }
        }
    }
}
