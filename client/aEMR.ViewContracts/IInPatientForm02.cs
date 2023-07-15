using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientForm02
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }

        PatientRegistration CurRegistration { get; set; }
        string DeptLocTitle { get; set; }


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

    }
}
