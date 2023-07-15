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
    public class BedCategory_Event
    {
    }

    public class dgListDblClickSelectBedCategory_Event
    {
        public object BedCategory_Current { get; set; }
    }

    public class dgBedCategoryListClickSelectionChanged_Event
    {
        public object BedCategory_Current { get; set; }
    }

    public class BedCategory_Event_Save
    {
        public object Result { get; set; }
    }
}
