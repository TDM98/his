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
    public interface IPCLExamTypeCombo_AddEdit
    {
        string TitleForm { get; set; }
        PCLExamTypeCombo ObjPCLExamTypeCombo_Current { get; set; }
        void PCLExamTypeCombo_GetDetails(long ID);
    }
}
