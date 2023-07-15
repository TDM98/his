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
using aEMR.Common;

namespace aEMR.ViewContracts.Configuration
{
    public interface IBedPatientAlloc
    {
        //ObservableCollection<BedAllocation> allBedAllocation { get; set; }
        Patient PatientInfo { get; set; }
        PatientRegistration curPatientRegistration { get; set; }
        bool BookBedAllocOnly { get; set; }
        RefDepartment DefaultDepartment { get; set; }
        RefDepartment ResponsibleDepartment { get; set; }
        DeptLocation SelectedDeptLocation { get; set; }

        bool isLoadAllDept { get; set; }

        //KMx: Chỉ được xem mà không được đặt giường (05/09/2014 16:50)
        bool IsReadOnly { get; set; }

        //KMx: Các khoa được cấu hình trách nhiệm (05/09/2014 10:27)
        //ObservableCollection<long> LstRefDepartment { get; set; }

        bool IsShowPatientInfo { get; set; }

        eFireBookingBedEvent eFireBookingBedEventTo { get; set; }

        InPatientDeptDetail InPtDeptDetail { get; set; }

        bool IsReSelectBed { get; set; }
    }
}
