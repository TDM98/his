using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientRecalcHi
    {
        bool CanPayCmd { get; }
        HealthInsurance ConfirmedHiItem { get; }
        PaperReferal ConfirmedPaperReferal { get; }
        PatientRegistration CurRegistration { get; set; }
        Patient CurrentPatient { get; }
        bool IsPaying { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IRegistrationSummaryV2 RegistrationDetailsContent { get; set; }
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }

        void PayCmd();

        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

       
    }
}
