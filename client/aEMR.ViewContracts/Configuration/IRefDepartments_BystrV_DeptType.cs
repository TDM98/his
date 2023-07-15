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
    public interface IRefDepartments_BystrV_DeptType
    {
        string strV_DeptType { get; set; }
        bool ShowDeptLocation { get; set; }
        void RefDepartments_Tree();
        object Parent { get; set; }
    }
}
