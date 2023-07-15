using DataEntities;
/*
 * 20220526 #001 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
 * 20220622 #002 DatTB:
 * + Ẩn nút “Xác nhận hoãn/ miễn tạm ứng” khi đã tạm ứng
 * + Ẩn nút sát nhập khi đã sát nhập rồi.
 */
namespace aEMR.ViewContracts
{
    /// <summary>
    /// Xử lý tạm ứng tiền cho bệnh nhân.
    /// </summary>
    public interface IInPatientCashAdvance
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IPatientPayment OldPaymentContent { get; set; }

        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }

        decimal? PayAmount { get; set; }
        void PayCmd();
        bool CanPayCmd { get;}

        //▼==== #001
        void PrintPostponementCmd();
        bool CanPrintPostponementCmd { get; }
        void ConfirmPostponementCmd();
        bool CanConfirmPostponementCmd { get; }
        //▲==== #001

        //▼==== #002
        void MergerCmd();
        bool CanMergerCmd { get; }
        //▲==== #002

        /// <summary>
        /// Lấy đầy đủ thông tin đăng ký để tính tiền
        /// </summary>
        /// <param name="registrationID"></param>
        //void LoadRegistrationByID(long registrationID);

        string DeptLocTitle { get; set; }

        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

    }
}
