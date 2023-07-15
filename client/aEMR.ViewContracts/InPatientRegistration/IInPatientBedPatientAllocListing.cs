using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientBedPatientAllocListing
    {
        ObservableCollection<BedPatientAllocs> AllItems { get;}
        PatientRegistration Registration { get; set; }
        void LoadData();
        bool ShowDeleteColumn { get; set; }
        bool ShowReturnBedColumn { get; set; }
        bool isEdit { get; set; }
        InPatientDeptDetail InPtDeptDetail { get; set; }
    }
}
