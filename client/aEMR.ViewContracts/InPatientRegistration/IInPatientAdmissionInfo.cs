using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IInPatientAdmissionInfo
    {
        void BeginEditCmd();
        void CanceEditCmd();

        RefDepartment CurrentDepartment { get; set; }
        IDepartmentListing DepartmentContent { get; set; }
        InPatientAdmDisDetails CurrentAdmission { get; set; }
        //ILookupValueListing AdmissionReasonContent { get; set; }
        DeptLocation SelectedLocation { get; set; }

        //ObservableCollection<long> LstRefDepartment { get; set; }
        bool isAdmision { get; set; }
        bool isRead { get; set; }

        bool ValidateInfo(out ObservableCollection<ValidationResult> validationResults);

        void DepartmentChange();

        void LoadData();

        void SetSelectedLocation(long DeptLocationID);

        int RegLockFlag { get; set; }
        void SetEmergencyAmissionInfo(long aDeptID);
        bool IsNotGuestEmergencyAdmission { get; set; }

        bool IsAdmissionFromSuggestion { get; set; }
    }
}