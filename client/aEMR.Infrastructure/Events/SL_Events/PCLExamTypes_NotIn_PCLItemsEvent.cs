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
    public class PCLExamTypes_NotIn_PCLItemsEvent
    {

    }

    public class PCLExamTypes_NotIn_PCLItemsDblClickSelectLocation_Event
    {
        public object PCLExamType_Current { get; set; }
    }

    public class PCLExamTypes_NotIn_PCLItemsClickSelectionChanged_Event
    {
        public object PCLExamType_Current { get; set; }
    }
}
