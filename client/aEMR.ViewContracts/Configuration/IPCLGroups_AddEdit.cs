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
    public interface IPCLGroups_AddEdit
    {
        string TitleForm { get; set; }
        ObservableCollection<Lookup> ObjV_PCLCategory { get; set; }

        DataEntities.PCLGroup ObjPCLGroups_Current { get; set; }
        void InitializeNewItem(Int64 V_PCLCategory_Selected);
    }
}
