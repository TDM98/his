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
    public interface IResourceMaintenanceLog_Confirm
    {
        DataEntities.ResourceMaintenanceLog ObjResourceMaintenanceLog_Current { get; set; }
        object Tab2 { get; set; }
        void LoadForm(DataEntities.ResourceMaintenanceLog Obj);
        bool btConfirmSave_IsEnabled { get; set; }
    }
}
