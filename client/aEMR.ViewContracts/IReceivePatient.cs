using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IReceivePatient
    {
        string PageTitle { get; set; }
        string DeptLocTitle { get; set; }
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        //IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IPatientDetails PatientDetailsContent { get; set; }
        Patient CurrentPatient { get; set; }

        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        HealthInsurance ConfirmedHiItem { get; set; }

        /// <summary>
        /// Thông tin giấy chuyển viện
        /// </summary>
        PaperReferal ConfirmedPaperReferal { get; set; }

        /// <summary>
        /// Loại đăng ký (Nhận bệnh cho đăng ký nội trú hay ngoại trú)
        /// </summary>
        AllLookupValues.RegistrationType RegistrationType { get; set; }
        
        AllLookupValues.V_RegForPatientOfType V_RegForPatientOfType { get; set; }

        bool Visibility_CheckboxCasualOrPreOp { get; set; }
        bool IsRegForCasualOrPreOp { get; set; }
        bool IsEmergency { get; set; }
        bool mNhanBenh_ThongTin_Sua { get; set; }
        bool mNhanBenh_TheBH_ThemMoi { get; set; }
        bool mNhanBenh_TheBH_XacNhan { get; set; }
        bool mNhanBenh_DangKy { get; set; }
        bool mNhanBenh_TheBH_Sua { get; set; }

        bool mPatient_TimBN { get; set; }
        bool mPatient_ThemBN { get; set; }
        bool mPatient_TimDangKy { get; set; }

        bool mInfo_CapNhatThongTinBN { get; set; }
        bool mInfo_XacNhan { get; set; }
        bool mInfo_XoaThe { get; set; }
        bool mInfo_XemPhongKham { get; set; }

        bool mInPt_ConfirmHI_Only { get; set; }

        void InitViewContent();

        bool IsAllowCrossRegion { get; set; }


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        bool IsShowGetTicketButton { get; set; }
    }
}
