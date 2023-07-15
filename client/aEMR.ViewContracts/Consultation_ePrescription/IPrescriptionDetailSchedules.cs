using System;
using System.Collections.ObjectModel;
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

namespace aEMR.ViewContracts
{
    public interface IPrescriptionDetailSchedules
    {
        PrescriptionDetail ObjPrescriptionDetail { get; set; }
        ObservableCollection<PrescriptionDetailSchedules> ObjPrescriptionDetailSchedules_ByPrescriptDetailID { get; set; }
        bool HasSchedule { get; set; }
        int ModeForm { get; set; }/*0:AddNew; 1:Update*/
        int NDay { get; set; }
        void Initialize();
        bool IsMaxDay { get; set; }
    }

    
}
