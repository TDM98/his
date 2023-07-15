using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Xử lý tạm ứng tiền cho bệnh nhân.
    /// </summary>
    public interface ISuggestAdvanceMoney
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
    }
}
