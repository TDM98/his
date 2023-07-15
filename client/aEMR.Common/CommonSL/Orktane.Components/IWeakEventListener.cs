using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orktane.Components
{

#if SILVERLIGHT

    public interface IWeakEventListener
    {
        /// <remarks> The managerType returns the event (handler) type to identify the event.
        /// and returing false would remove the listener.
        bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e);
    }

#endif

    

}
