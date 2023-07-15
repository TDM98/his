using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Xử lý tạm ứng tiền cho bệnh nhân.
    /// </summary>
    public interface ISuggestCashAdvance
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }

        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }

        bool IsPaying { get; set; }
        void PayCmd();
        bool CanPayCmd { get;}

        void SetRegistration(object registrationInfo);

        /// <summary>
        /// Lấy đầy đủ thông tin đăng ký để tính tiền
        /// </summary>
        /// <param name="registrationID"></param>
       // void LoadRegistrationByID(long registrationID);

        bool IsLoadingPatient { get; set; }
        bool IsLoadingRegistration { get; set; }

        string DeptLocTitle { get; set; }

        bool UsedByTaiVuOffice { get; set; }


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }


    }
}
