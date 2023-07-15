using System;
using System.Diagnostics;

namespace aEMR.Infrastructure.ServiceCore
{
    public class AsyncUtility : IAsyncUtility
    {
       public AsyncCallback DispatchCallback(AsyncCallback callback)
        {
            return (new DispatchedCallback(callback).Execute);
        }

        /// <summary>
        /// Encapsulates a callback so that we can execute it as a different method.
        /// </summary>
        private class DispatchedCallback
        {
            private AsyncCallback _callback;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchedCallback"/> class.
            /// </summary>
            /// <param name="callback">The callback.</param>
            public DispatchedCallback(AsyncCallback callback)
            {
                _callback = callback;
            }

            /// <summary>
            /// Executes the callback delegate on the Silverlight application UI thread.
            /// </summary>
            /// <param name="result">The result of an asynchronous operation.</param>
            [DebuggerStepThrough]
            [DebuggerHidden]
            public void Execute(IAsyncResult result)
            {
                // TxD 31/05/2018 The following line was applicable to Silverlight and REPLACED with:
                // SILVERLIGHT : System.Windows.Deployment.Current.Dispatcher ===> WPF : System.Windows.Application.Current.Dispatcher
                //System.Windows.Threading.Dispatcher dispatcher = System.Windows.Deployment.Current.Dispatcher;
                System.Windows.Threading.Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
                dispatcher.Invoke(() => _callback(result));
            }
        }
    }
    
}
