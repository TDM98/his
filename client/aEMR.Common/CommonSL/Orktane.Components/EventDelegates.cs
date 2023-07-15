using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orktane.Components
{

    public delegate void EventDelegate<E>(Object sender, E args)
        where E : EventArgs;

    public delegate void EventDelegate<T, E>(T @this, Object sender, E args)
        where E : EventArgs;

}
