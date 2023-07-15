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
    public interface IcwdBedPatientCommon
    {
        BedPatientAllocs selectedBedPatientAllocs { get; set; }
        BedPatientAllocs selectedTempBedPatientAllocs { get; set; }
        ObservableCollection<BedPatientAllocs> allBedPatientAllocs { get; set; }
        bool isDeleteAll { get; set; }

        bool isDelete { get; set; }
        bool BookBedAllocOnly { get; set; }

        RefDepartment ResponsibleDepartment { get; set; }
        IDepartmentListing DepartmentContent { get; set; }

        eFireBookingBedEvent eFireBookingBedEventTo { get; set; }

        InPatientDeptDetail InPtDeptDetail { get; set; }

        bool IsReSelectBed { get; set; }
        void GetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria SearchCriteria);
        bool IsEdit { get; set; }

        IMinHourDateControl CheckInDateTime { get; set; }
    }
}
