using System.Collections.Generic;
using System.Threading;

namespace eHCMS.Services.Core
{
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
