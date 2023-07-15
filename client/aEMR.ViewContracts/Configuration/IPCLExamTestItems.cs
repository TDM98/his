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
    public interface IPCLExamTestItems
    {
        PCLExamType ObjPCLExamType_Current { get; set; }

        void FormLoad();
        string TitleForm { get; set; }
        PCLExamTestItems ObjPCLExamTestItems_Current { get; set; }
        void InitializeNewItem();
    }
}
