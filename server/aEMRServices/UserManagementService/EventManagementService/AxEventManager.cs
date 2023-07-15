using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using DataEntities;
using System.Runtime.Serialization;

namespace EventManagementService
{
    public class AxEventManager
    {
        public static AxEventManager Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly AxEventManager instance = new AxEventManager();
        }

        private AxDuplexEventManager _duplexEventManager;
        /// <summary>
        /// This is the duplex manager (run in a separate thread)
        /// </summary>
        public AxDuplexEventManager DuplexEventManager
        {
            get
            {
                return _duplexEventManager;
            }
        }
        
        readonly object _locker=new object();
        public AxEventManager():base()
        {
            _duplexEventManager = new AxDuplexEventManager();
            _outstdTaskServer = new OutstandingTaskTcpServer();
        }











        /////////////////
        private OutstandingTaskTcpServer _outstdTaskServer;
        /// <summary>
        /// This is the OutstandingTaskSocketServer (run in a separate thread)
        /// </summary>
        public OutstandingTaskTcpServer OutstdTaskServer
        {
            get
            {
                return _outstdTaskServer;
            }
        }
    }

    [DataContract]
    public enum AxEventType:int
    {
        [EnumMember]
        GET_CONSULTATION_LIST = 1,
        
        [EnumMember]
        GET_PCL_LIST = 2,

        [EnumMember]
        GET_PRESCRIPTON_LIST = 3

        //[EnumMember]
        //GET_REGISTERED_PATIENTS = 5,
        //[EnumMember]
        //GET_PAID_PATIENTS = 6,
        //[EnumMember]
        //RECEIVE_WAITING_FOR_EXAMINATION_PATIENT = 7
    }
    [DataContract]
    [KnownType(typeof(PatientQueue))]
    public class AxEvent
    {
        [DataMember]
        public decimal? LocationID
        {
            get;
            set;
        }
        [DataMember]
        public AxEventType EventType { get; set; }

        [DataMember]
        public object UserState { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            AxEvent evt = obj as AxEvent;
            if (evt == null)
            {
                return false;
            }

            return this.EventType == evt.EventType;
        }
        public string ToXml()
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xm = new XmlSerializer(typeof(AxEvent));
            xm.Serialize(sw, this);
            return sw.ToString();
        }
    }
}
