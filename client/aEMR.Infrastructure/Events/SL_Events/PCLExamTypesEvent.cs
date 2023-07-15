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
    public class PCLExamTypesEvent
    {
    }
    public class PCLExamTypeComboEvent_AddEditSave
    {
        public object Result_AddEditSave { get; set; }
    }

    public class PCLExamTypeServiceTargetEvent_AddEditSave
    {
        public object Result_AddEditSave { get; set; }
    }
    public class RefDepartmentReqCashAdvEvent_AddEditSave
    {
        public object Result_AddEditSave { get; set; }
    }
    public class PCLExamTypesEvent_AddEditSave
    {
        public object Result_AddEditSave { get; set; }
    }

    public class dgListPCLExamTypeDblClickSelectLocation_Event
    {
        public object PCLExamType_Current { get; set; }
    }

    public class dgListPCLExamTypeClickSelectionChanged_Event
    {
        public object PCLExamType_Current { get; set; }
    }
}
