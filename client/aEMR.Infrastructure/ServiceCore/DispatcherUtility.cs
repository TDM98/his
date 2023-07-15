using System;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Reflection;

namespace aEMR.Infrastructure.ServiceCore
{
    public static class DispatcherUtility
    {
        [DebuggerStepThrough]
        public static void Invoke(this Dispatcher dispatcher, Action action)
        {
            // Constraint: The "dispatcher" parameter must not be null.
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            // Constraint: The "action" parameter must not be null.
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            dispatcher.Invoke((Delegate)action);
        }

        [DebuggerStepThrough]
        public static void Invoke(this Dispatcher dispatcher, Delegate callback, params object[] args)
        {
            // Constraint: The "dispatcher" parameter must not be null.
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            // Constraint: The "callback" parameter must not be null.
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            // Using this little (intellisense invisible) method check if we are on the UI thread.
            if (!dispatcher.CheckAccess())
            {
                // If we are not on the UI thread, call the dispatcher in a synchronized fashion.
                // In our case the ManualResetEvent is used for synching.
                ManualResetEvent manualResetEvent = new ManualResetEvent(false);
                Action actDelegate = delegate ()
                {
                    try
                    {
                        // Invoke the delegate, passing all the parameters.
                        callback.DynamicInvoke(args);
                    }
                    catch (TargetInvocationException exc)
                    {
                        // Unwrap TargetInvocationExceptions. Upstream error handlers 
                        // are not interested wether the exception ocurred in callback or not.
                        // This neutralized the effects of the DispatcherUtility.
                        throw exc.InnerException;
                    }
                    finally
                    {
                        // Make sure that the other thread is unblocked even if an error has occoured.
                        manualResetEvent.Set();
                    }
                };
                dispatcher.BeginInvoke(actDelegate);

                // Wait for the call of the "Set" method and unblock the thread afterwards.
                manualResetEvent.WaitOne();

                // Release all the resources that might be used by the wait handle.
                manualResetEvent.Close();
            }
            else
            {
                // This code is running on the UI thread. Therefore just invoke the callback.
                callback.DynamicInvoke(args);
            }
        }
    }
}
