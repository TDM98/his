using DataEntities;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Xử lý tạm ứng tiền cho bệnh nhân.
    /// </summary>
    public interface IInPatientPayment
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }

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
