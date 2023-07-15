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
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts.Configuration
{
    public interface IDeptTreeCommon
    {
        RefDepartment DefaultDepartment { get; set; }
        void GetDeptLocTreeAll();

        void GetDeptLocTreeDeptID();

        //ObservableCollection<long> LstRefDepartment { get; set; }

    }
}
