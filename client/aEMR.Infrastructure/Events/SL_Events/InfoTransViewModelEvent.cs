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
    public class AcceptChangeDeptViewModelEvent
    {

    }

    public class InfoTransViewModelEvent
    {

    }

    public class TranPaymentUpdateEvent
    {
        public long TransactionID { get; set; }
    }

    public class InfoTransViewModelEvent_ChooseRscrForMaintenance
    {
        public object ObjResourcePropLocations { get; set; }
    }
}
