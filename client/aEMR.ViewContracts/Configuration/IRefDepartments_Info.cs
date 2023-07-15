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

namespace aEMR.ViewContracts.Configuration
{
    public interface IRefDepartments_Info
    {   
        string TitleForm { get; set; }
        object ObjRefDepartments_Current { get; set; }
        object ObjRefDepartments_ParDeptID_Current { get; set; }
        void RefDepartment_SubtractAllChild_ByDeptID(Int64 DeptID);
        void InitializeRefDepartments_New(object NodeTreeParent);
        void SetInfo_ObjRefDepartments_Current(object objNode);
    }
}
