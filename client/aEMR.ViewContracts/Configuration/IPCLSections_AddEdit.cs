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
using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts.Configuration
{
    public interface IPCLSections_AddEdit
    {
        string TitleForm { get; set; }
        
        PCLSection ObjPCLSections_Current { get; set; }

        void InitializeNewItem();
    }
}
