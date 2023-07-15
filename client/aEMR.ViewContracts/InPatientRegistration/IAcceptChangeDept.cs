using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IAcceptChangeDept
    {
        PatientRegistration CurrentRegistration { get; set; }
        void LoadLocations(long? deptId);
        InPatientTransferDeptReq CurInPatientTransferDeptReq { get; set; }
        //ObservableCollection<long> LstRefDepartment { get; set; }
        bool IsFromRequestPaper { get; set; }
        void LoadData();
        int DlgUsageMode { get; set; }
        void SetModInPtDeptDetails(InPatientDeptDetail modInPtDeptDetails);
        bool IsOpenToDischarge { get; set; }
        bool IsTemp { get; set; }
        void SetDesDepartment(DeptLocation aDeptLocation);
    }
}