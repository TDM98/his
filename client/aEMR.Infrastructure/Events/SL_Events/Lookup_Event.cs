using DataEntities;
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

namespace aEMR.Infrastructure.Events
{
    public class Lookup_Event
    {
    }

    public class dgListDblClickSelectLookup_Event
    {
        public object Lookup_Current { get; set; }
    }

    public class dgLookupListClickSelectionChanged_Event
    {
        public Lookup Lookup_Current { get; set; }
    }

    public class Lookup_Event_Save
    {
        public object Result { get; set; }
    }

    public class Lookup_Event_Choose
    {
        public LookupTree ParentSelected { get; set; }
        public LookupTree ChildrenSelected { get; set; }
    }
}
