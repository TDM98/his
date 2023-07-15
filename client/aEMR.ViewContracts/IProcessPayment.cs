using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IProcessPayment
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IRegistrationSummaryV2 RegistrationDetailsContent { get; set; }

        Patient CurrentPatient { get;}
        PatientRegistration CurRegistration { get; set; }

        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        HealthInsurance ConfirmedHiItem { get; }

        /// <summary>
        /// Thông tin giấy chuyển viện
        /// </summary>
        PaperReferal ConfirmedPaperReferal { get; }

        bool IsPaying { get; set; }
        bool CanPayCmd { get;}
        void PayCmd();


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        bool IsFinalization { get; set; }
    }
}