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

namespace aEMR.ViewContracts.ResourcesManage.Maintenance
{
    public interface IResourceMaintenanceLog_Add
    {
        DataEntities.ResourcePropLocations ObjResource_Current { get; set; }
        void InitializeNewItem();
        void SetValueRscrPropertyID(DataEntities.ResourcePropLocations pResourcePropLocations);

        Visibility cboStaff_Visibility { get; set; }
        Visibility cboSupplier_Visibility { get; set; }

        bool IsShowAsChildWindow { get; set; }
        bool btChooseResourceIsEnabled { get;set;}
        void KhoiTaoYeuCauVaGanBien(ResourcePropLocations ObjResourcePropLocations);
    }
}
