using System;
using System.Collections.ObjectModel;
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

namespace aEMR.ViewContracts.Configuration
{
    public interface IDeptMedServiceItems_EditInfo
    {
        ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll { get; set; }
        DataEntities.DeptMedServiceItems ObjDeptMedServiceItems_Save { get; set; }

        RefMedicalServiceType ObjRefMedicalServiceTypeSelected { get; set; }
        RefMedicalServiceItem ObjRefMedicalServiceItem { get; set; }
        string TitleForm { get; set; }
        bool isUpdate { get; set; }
        RefMedicalServiceItem tmpRefMedicalServiceItem { get; set; }
    }
}
