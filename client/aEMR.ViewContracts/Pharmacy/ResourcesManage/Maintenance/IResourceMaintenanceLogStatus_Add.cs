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

namespace aEMR.ViewContracts.ResourcesManage.Maintenance
{
    public interface IResourceMaintenanceLogStatus_Add
    {
        DataEntities.ResourceMaintenanceLog ObjResourceMaintenanceLog_Current { get; set; }
        void LoadListHistoryStatus();
        Int64 V_CurrentStatus_Seleted { get; set; }

        bool btUpdateStatus_IsEnabled { get; set; }
        bool ex1DoneAndWaiting_IsExpanded { get; set; }
        bool btSaveWaitingVerified_IsEnabled { get; set; }
        bool btCancel_IsEnabled { get; set; }
        bool IsSupplierFix { get; set; }
        bool chkIsSupplier_IsEnabled { get; set; }
        Visibility cboStaff_Visibility { get; set; }
        Visibility cboSupplier_Visibility { get; set; }
        void InitializeNewItem(Int64 pV_CurrentStatus);
    }
}
