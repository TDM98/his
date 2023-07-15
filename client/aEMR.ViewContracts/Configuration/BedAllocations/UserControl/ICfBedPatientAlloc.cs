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

namespace aEMR.ViewContracts.Configuration
{
    public interface ICfBedPatientAlloc
    {
        //ObservableCollection<BedAllocation> allBedAllocation { get; set; }
        Patient PatientInfo { get; set; }
        PatientRegistration curPatientRegistration { get; set; }
        
        //RefDepartment DefaultDepartment { get; set; }
        //RefDepartment ResponsibleDepartment { get; set; }
    }
}
