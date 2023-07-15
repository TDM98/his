using System.Windows;
using System.Windows.Data;
using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IPatientSummaryInfoCommon
    {
        Patient CurrentPatient { get; set; }

        PatientHI_SummaryInfo PtHISumInfo { get; set; }
        void SetPatientHISumInfo(PatientHI_SummaryInfo PtHISumInfo); 

        /// <summary>
        /// Sửa thông tin hành chánh bệnh nhân.
        /// </summary>
        void EditGeneralInfoCmd();

        /// <summary>
        /// Xác nhận thẻ bảo hiểm.
        /// </summary>
        void ConfirmHiCmd();
        /// <summary>
        /// Có được phép Confirm thẻ bảo hiểm hay không
        /// </summary>
        bool CanConfirmHi { get; set; }

        //bool CanConfirmPaperReferal { get; set; }
        //double? HiBenefit { get; set; }
        //double? HiBenefit_2 { get; set; }
        //double? HiBenefit_3 { get; set; }

        string HiComment { get; set; }

        Visibility GeneralInfoVisibility { get; }
        Visibility ConfirmHiVisibility { get; }
        Visibility ConfirmPaperReferalVisibility { get; }

        bool DisplayButtons { get; set; }
        void ExpandCmd();
        void CollapseCmd();

        PatientClassification CurrentPatientClassification { get; set; }
        

        bool mInfo_CapNhatThongTinBN { get; set; }
        bool mInfo_XacNhan { get; set; }
        bool mInfo_XoaThe { get; set; }
        bool mInfo_XemPhongKham { get; set; }

        // TxD 09/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        bool Enable_ReConfirmHI_InPatientOnly { get; set; }
        bool ThongTuyen { get; set; }

    }

    public interface IPatientSummaryInfoV2 : IPatientSummaryInfoCommon
    {
        PatientRegistration CurrentPatientRegistration { get; set; }
    }
    public interface IPatientSummaryInfoV3 : IPatientSummaryInfoCommon
    {
        string BasicDiagTreatment { get; set; }
        PatientRegistration curRegistration { get; set; }
        Staff gSelectedDoctorStaff { get; set; }
        void ApplySpecialPatientClass(PatientClassification aPatientClass);
        RegistrationViewCase ViewCase { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        Patient RefByPatient { get; set; }
        Staff RefByStaff { get; set; }
    }
}