using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Threading;
using System.Windows.Threading;


/*********************************
*
* Created by: Trinh Thai Tuyen
* Date Created: Wednesday, September 01, 2010
* Last Modified Date: 
*
*********************************/
      
namespace aEMR.Common.SynchronousDispatcher
{
    public class SynchronousUIContext:ISynchronizationContext
    {
        protected DispatcherSynchronizationContext _context;
        protected Dispatcher _dispatcher;

        readonly object lockObj = new object();

        static readonly SynchronousUIContext _instance = new SynchronousUIContext();
        public static ISynchronizationContext Instance
        {
            get
            {
                return _instance;
            }
        }

        void EnsureInitialized()
        {
            if (_dispatcher != null && _context != null)
                return;
            lock (lockObj)
            {
                try
                {
                    _dispatcher = Application.Current.Dispatcher; //Deployment.Current.Dispatcher;
                    _context = new DispatcherSynchronizationContext(_dispatcher);
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    throw;
                }
            }
        }
        #region ISynchronizationContext Members

        public bool InvokeRequired
        {
            get
            {
                EnsureInitialized();
                return !_dispatcher.CheckAccess();
            }
        }

        public void Initialize()
        {
            EnsureInitialized();
        }

        public void Initialize(System.Windows.Threading.Dispatcher dispatcher)
        {
            lock (lockObj)
            {
                try
                {
                    _dispatcher = dispatcher;
                    _context = new DispatcherSynchronizationContext(_dispatcher);
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    throw;
                }
            }
        }

        public void InvokeAndBlockUntilCompletion(Action action)
        {
            EnsureInitialized();
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                _context.Send(delegate { action(); }, null);
            }
        }

        public void InvokeAndBlockUntilCompletion(System.Threading.SendOrPostCallback callback, object state)
        {
            EnsureInitialized();
            _context.Send(callback, state);
        }

        public void InvokeWithoutBlocking(Action action)
        {
            EnsureInitialized();
            _context.Post(state => action(), null);
        }

        public void InvokeWithoutBlocking(System.Threading.SendOrPostCallback callback, object state)
        {
            EnsureInitialized();
            _context.Post(callback, state);
        }

        #endregion

        
    }
}
