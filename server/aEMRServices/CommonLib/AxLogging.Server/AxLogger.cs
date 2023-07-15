using System.Collections.Generic;
using System.Threading;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace AxLogging
{
    public class AxLogger : AxThreadWithMessageQueue<AxLoggingEvent>
    {
        public AxLogger() : base()
        {
        }
        public static AxLogger Instance
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

            internal static readonly AxLogger instance = new AxLogger();
        }

        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ILog SqlLoger = LogManager.GetLogger("SqlExecutionTimeLogger");

        readonly object _locker = new object();
        public override void ProcessEvent(AxLoggingEvent evt)
        {
            try
            {
                lock (_locker)
                {
                    switch(evt.LogLevel)
                    {
                        case AxLogLevel.DEBUG:
                            Debug(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        case AxLogLevel.WARN:
                            Warn(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        case AxLogLevel.INFO:
                            Info(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        case AxLogLevel.ERROR:
                            Error(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        case AxLogLevel.FATAL:
                            Fatal(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        case AxLogLevel.SQLINFO:
                            SqlInfo(evt.LoggedMessage, evt.NDC_Extra);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch 
            {
            }
        }

        private void Debug(string s, object ndcExtra = null)
        {
            if (log.IsDebugEnabled)
            {
                if (ndcExtra != null)
                {
                    using (log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        log.Debug(s);
                    }
                }
                else
                {
                    log.Debug(s);
                }
            }
        }
        private void Info(string s, object ndcExtra = null)
        {
            if (log.IsInfoEnabled)
            {
                if(ndcExtra != null)
                {
                    using(log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        log.Info(s);
                    }
                }
                else
                {
                    log.Info(s);
                }
            }
        }
        private void SqlInfo(string s, object ndcExtra = null)
        {
            if (SqlLoger.IsInfoEnabled)
            {
                if (ndcExtra != null)
                {
                    using (log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        SqlLoger.Info(s);
                    }
                }
                else
                {
                    SqlLoger.Info(s);
                }
            }
        }
        private void Warn(string s, object ndcExtra = null)
        {
            if (log.IsWarnEnabled)
            {
                if (ndcExtra != null)
                {
                    using (log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        log.Warn(s);
                    }
                }
                else
                {
                    log.Warn(s);
                }
            }
        }
        private void Fatal(string s, object ndcExtra = null)
        {
            if (log.IsFatalEnabled)
            {
                if (ndcExtra != null)
                {
                    using (log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        log.Fatal(s);
                    }
                }
                else
                {
                    log.Fatal(s);
                }
            }
        }
        private void Error(string s, object ndcExtra = null)
        {
            if (log.IsErrorEnabled)
            {
                if (ndcExtra != null)
                {
                    using (log4net.NDC.Push(ndcExtra.ToString()))
                    {
                        log.Error(s);
                    }
                }
                else
                {
                    log.Error(s);
                }
            }
        }

        public void LogDebug(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.DEBUG,ndcExtra);
            this.EnqueueEvent(evt);
        }
        public void LogInfo(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.INFO,ndcExtra);
            this.EnqueueEvent(evt);
        }
        public void SqlLogInfo(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.SQLINFO, ndcExtra);
            this.EnqueueEvent(evt);
        }
        public void LogWarn(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.WARN,ndcExtra);
            this.EnqueueEvent(evt);
        }
        public void LogFatal(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.FATAL,ndcExtra);
            this.EnqueueEvent(evt);
        }
        public void LogError(string s, object ndcExtra = null)
        {
            AxLoggingEvent evt = new AxLoggingEvent(s, AxLogLevel.ERROR,ndcExtra);
            this.EnqueueEvent(evt);
        }

        public void LogDebug(object ex, object ndcExtra = null)
        {
            if (ex != null)
            {
                AxLoggingEvent evt = new AxLoggingEvent(ex.ToString(), AxLogLevel.DEBUG,ndcExtra);
                this.EnqueueEvent(evt);
            }
        }
        public void LogInfo(object ex, object ndcExtra = null)
        {
            if (ex != null)
            {
                AxLoggingEvent evt = new AxLoggingEvent(ex.ToString(), AxLogLevel.INFO,ndcExtra);
                this.EnqueueEvent(evt);
            }
        }
        public void LogWarn(object ex, object ndcExtra = null)
        {
            if (ex != null)
            {
                AxLoggingEvent evt = new AxLoggingEvent(ex.ToString(), AxLogLevel.WARN,ndcExtra);
                this.EnqueueEvent(evt);
            }
        }
        public void LogFatal(object ex, object ndcExtra = null)
        {
            if (ex != null)
            {
                AxLoggingEvent evt = new AxLoggingEvent(ex.ToString(), AxLogLevel.FATAL,ndcExtra);
                this.EnqueueEvent(evt);
            }
        }
        public void LogError(object ex, object ndcExtra = null)
        {
            if (ex != null)
            {
                AxLoggingEvent evt = new AxLoggingEvent(ex.ToString(), AxLogLevel.ERROR,ndcExtra);
                this.EnqueueEvent(evt);
            }
        }
    }

    public class AxLoggingEvent
    {
        public AxLoggingEvent():this("",AxLogLevel.DEBUG)
        {

        }
        public AxLoggingEvent(string message):this()
        {
            this.LoggedMessage = message;
        }
        public AxLoggingEvent(string message, AxLogLevel level)
        {
            this.LoggedMessage = message;
            this.LogLevel = level;
        }
        public AxLoggingEvent(string message, AxLogLevel level, object ndcExtra)
        {
            this.LoggedMessage = message;
            this.LogLevel = level;
            this.NDC_Extra = ndcExtra;
        }

        //Ghi them thong tin vao nested diagnostic context.
        public object NDC_Extra { get; set; }

        public string LoggedMessage { get; set; }

        public AxLogLevel LogLevel { get; set; }
    }
    public enum AxLogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL,
        SQLINFO
    }

    public class AxThreadWithMessageQueue<T>
    {
        bool _uniqueQueue = false;
        readonly object _locker = new object();
        Thread _workerThread;
        Queue<T> _eventQueue = new Queue<T>();

        public AxThreadWithMessageQueue(bool isUniqueQueue = false)
        {
            _uniqueQueue = isUniqueQueue;
            ThreadStart ts = new ThreadStart(Run);
            _workerThread = new Thread(ts);
            _workerThread.Start();
        }
        void Run()
        {
            while (true)
            {
                T evt;
                lock (_locker)
                {
                    while (_eventQueue.Count == 0)
                    {
                        Monitor.Wait(_locker);
                    }
                    evt = _eventQueue.Dequeue();
                }
                ProcessEvent(evt);
            }
        }

        public void EnqueueEvent(T evt)
        {
            lock (_locker)
            {
                if (_uniqueQueue)
                {
                    if (!_eventQueue.Contains(evt))
                    {
                        _eventQueue.Enqueue(evt);
                    }
                }
                else
                {
                    _eventQueue.Enqueue(evt);
                }

                Monitor.Pulse(_locker);
            }
        }
        public virtual void ProcessEvent(T evt)
        {
        }
    }
}
