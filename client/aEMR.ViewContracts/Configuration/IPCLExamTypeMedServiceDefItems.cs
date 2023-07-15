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
    public interface IPCLExamTypeMedServiceDefItems
    {
        void FormLoad();

        object leftContent { get; set; }

        RefMedicalServiceItem ObjRefMedicalServiceItem_isPCL { get; set; }

        PCLExamType ObjPCLExamType_SelectForAdd { get; set; }
    }
}
