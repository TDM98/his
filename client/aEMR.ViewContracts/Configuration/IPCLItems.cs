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
    public interface IPCLItems
    {
        void FormLoad();

        PCLExamTypeSearchCriteria SearchCriteria { get; set; }

        object leftContent { get; set; }

        PCLForm ObjPCLForm { get; set; }

        PCLExamType ObjPCLExamType_SelectForAdd { get; set; }
        
    }
}
