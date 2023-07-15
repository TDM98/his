using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientAdmission
    {
        //InPatientAdmDisDetails CurrentAdmission { get; set; }
        IInPatientDeptListing InPatientDeptListingContent { get; set; }
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        
        IInPatientAdmissionInfo InPatientAdmissionInfoContent { get; set; }
        IInPatientBedPatientAllocListing PatientAllocListingContent { get; set; }
        Patient CurrentPatient { get; set; }

        string TitleForm { get; set; }
        bool isAdmision { get; set; }
        string DeptLocTitle { get; set; }

        void InitViewContent();


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
    }
}
