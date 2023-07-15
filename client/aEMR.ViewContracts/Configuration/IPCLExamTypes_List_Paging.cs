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

namespace aEMR.ViewContracts.Configuration
{
    /*DS chọn PCLExamType: có và không tồn tại trong PCLItems*/
    public interface IPCLExamTypes_List_Paging
    {
        PCLExamTypeSearchCriteria SearchCriteria { get;set;}
        Visibility IsNotInPCLItemsVisibility { get; set; }
        void FormLoad();
        bool IsEnableV_PCLMainCategory { get; set; }

        bool IsChildWindow { get; set; }

        Lookup ObjV_PCLMainCategory_Selected { get; set; }
        void LoadForConfigPCLExamTypesIntoPCLForm();

    }
}
