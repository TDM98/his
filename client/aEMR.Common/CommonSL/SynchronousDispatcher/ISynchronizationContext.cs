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
using System.Windows.Threading;
using System.Threading;


/*********************************
*
* Created by: Trinh Thai Tuyen
* Date Created: Wednesday, September 01, 2010
* Last Modified Date: 
*
*********************************/

namespace aEMR.Common.SynchronousDispatcher
{
    public interface ISynchronizationContext
    {
        bool InvokeRequired { get; }

        void Initialize();
        void Initialize(Dispatcher dispatcher);
        void InvokeAndBlockUntilCompletion(Action action);
        void InvokeAndBlockUntilCompletion(SendOrPostCallback callback, object state);
        void InvokeWithoutBlocking(Action action);
        void InvokeWithoutBlocking(SendOrPostCallback callback, object state);
    }
}
