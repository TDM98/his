using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IManagePatientDetails
    {
        string PageTitle { get; set; }
        string DeptLocTitle { get; set; }
        object SearchRegistrationContent { get; set; }

        IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        //IPatientInfo PatientSummaryInfoContent { get; set; }
        
        IPatientDetails PatientDetailsContent { get; set; }
        Patient CurrentPatient { get; set; }


        /// <summary>
        /// Loại đăng ký (Nhận bệnh cho đăng ký nội trú hay ngoại trú)
        /// </summary>
        AllLookupValues.RegistrationType RegistrationType { get; set; }

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
    }
}
