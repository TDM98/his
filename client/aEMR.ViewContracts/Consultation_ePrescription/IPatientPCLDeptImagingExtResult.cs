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

namespace aEMR.ViewContracts
{
    public interface IPatientPCLDeptImagingExtResult
    {
        //object UCHeaderInfoPMR { get; set; }
        bool IsEnableButton { get; set; }
        void LoadData(PatientPCLRequest_Ext message);
        bool IsEdit { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
