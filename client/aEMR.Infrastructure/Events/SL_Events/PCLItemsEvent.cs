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
using DataEntities;

namespace aEMR.Infrastructure.Events
{
    public class PCLItemsEvent
    {

    }

    // Modify từ InPatientSelectServiceContent - khi thêm dịch vụ

    public class ModifyPriceToInsert_Completed
    {
        public MedRegItemBase ModifyItem { get; set; }
    }


    // modify từ BillingInvoice
    public class ModifyPriceToUpdate_Completed
    {

    }


    public class PCLItemsEvent_Save
    {
        public object Result { get; set; }
    }

    public class PCLDeptImagingResultLoadEvent
    {
        public PatientPCLRequest PatientPCLRequest_Imaging { get; set; }
    }

    public class ReloadEchoCardiographyResult
    {

    }
    public class PCLResultReloadEvent
    {
        public PatientPCLRequest PatientPCLRequest_Imaging { get; set; }
    }
}
