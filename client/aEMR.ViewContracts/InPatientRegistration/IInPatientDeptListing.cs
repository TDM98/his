using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientDeptListing
    {
        ObservableCollection<InPatientDeptDetail> AllItems { get; set;  }

        PatientRegistration CurrentRegistration { get; set; }

        InPatientAdmDisDetails AdmissionInfo { get; set; }

        //ObservableCollection<long> LstRefDepartment { get; set; }

        void LoadData();
        bool IsLoading { get; set; }

        bool ShowBookingBedColumn { get; set; }
        bool ShowOutDeptColumn { get; set; }

        bool ShowDeleteEditColumn { get; set; }

    }
}
