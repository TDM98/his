using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMS.Services.Core;
using aEMR.Infrastructure.Events;
using System;
/*
* 20230530 #001 DatTB:
* + Thêm service tìm kiếm bệnh nhân bằng QRCode CCCD
* + Thêm bệnh nhân bằng thông tin QRCode CCCD
*/
namespace aEMR.ViewContracts
{
    /// <summary>
    /// Hiển thị cửa sổ PatientDetails để Thêm BN, Quản lý BH của BN.
    /// </summary>
    public interface IPatientDetails
    {
        Patient CurrentPatient { get; set; }

        void InitPatient(Patient _currentPatient);

        IPatientHiManagement HealthInsuranceContent { get; set; }
        /*TMA*/
        int PatientFindBy { get; set; }
        //void CloseGeneralInfoCmd();
        void SavePatientCmd();

        /// <summary>
        /// Tạo mới một bệnh nhân (dùng trên form, chưa đưa vào database)
        /// </summary>
        void CreateNewPatient(bool bCreateNewPatientDetailsOnly = false);

        bool ConfirmHIBeforeRegister();

        ObservableCollection<Gender> Genders { get; set; }
        ObservableCollection<Lookup> MaritalStatusList { get; set; }
        ObservableCollection<Lookup> EthnicsList { get; set; }
        ObservableCollection<Lookup> FamilyRelationshipList { get; set; }
        ObservableCollection<RefCountry> Countries { get; set; }
        ObservableCollection<CitiesProvince> Provinces { get; set; }

        /// <summary>
        /// Đang trong quá trình lưu thông tin bệnh nhân.
        /// </summary>
        bool IsSaving { get; set; }

        bool IsChildWindow { get; set; }

        /// <summary>
        /// Kiểm tra thông tin hành chính của bệnh nhân có thay đổi chưa.
        /// </summary>
        bool GeneralInfoChanged { get; set; }

        /// <summary>
        /// Trạng thái của form Thông tin bệnh nhân (Thêm mới, Cập nhật, ...)
        /// </summary>
        FormState FormState { get; set; }

        /// <summary>
        /// Tiêu đề của form Thông tin bệnh nhân. 
        /// </summary>
        string CurrentAction { get; set; }

        /// <summary>
        /// Danh sách thẻ bảo hiểm của bệnh nhân hiện tại.
        /// </summary>
        //BlankRowCollection<HealthInsurance> HealthInsurances { get; set; }

        /// <summary>
        /// Đóng form lại khi thao tác xong.
        /// </summary>
        bool CloseWhenFinish { get; set; }

        // TxD 14/07/2014 : The following method is replaced with LoadPatientDetailsAndHI_GenAction that is to be called
        //                  inside a CoRoutine to prevent RACING condition.
        //void StartEditingPatientLazyLoad(object patient);

        void LoadPatientDetailsAndHI_GenAction(object genCoRoutineTask, object patient, object ToRegisOutPt);

        void LoadPatientDetailsAndHI_V2(object patient, object ToRegisOutPt);

        bool ShowExtraConfirmHI_Fields { get; set; }

        ActivationMode ActivationMode { get; set; }

        /// <summary>
        /// Đang load thông tin hành chánh của bệnh nhân.
        /// </summary>
        bool IsLoading { get; set; }

        PatientInfoTabs ActiveTab { get; set; }
        HiCardConfirmedEvent hiCardConfirmedEvent { get; set; }

        void Close();

        bool ShowCloseFormButton { get; set; }

        bool mNhanBenh_ThongTin_Sua { get; set; }
        bool mNhanBenh_ThongTin_XacNhan { get; set; }
        bool mNhanBenh_TheBH_ThemMoi { get; set; }
        bool mNhanBenh_TheBH_XacNhan { get; set; }
        bool mNhanBenh_DangKy { get; set; }
        bool mNhanBenh_TheBH_Sua { get; set; }

        AllLookupValues.RegistrationType RegistrationType { get; set; }

        // TxD 10/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        bool Enable_ReConfirmHI_InPatientOnly { get; set; }

        // TxD 15/07/2014 : Added the following to replace the Binding of ActivationMode (complicated and mixed up)  to the Visibility of 
        //                  the Health Insurance Tab
        bool HITabVisible { get; set; }

        bool IsConfirmedEmergencyPatient { get; set; }

        bool ShowConfirmedEmergencyPatient { get; set; }

        bool IsConfirmedForeignerPatient { get; set; }

        bool ShowConfirmedForeignerPatient { get; set; }

        bool EmergInPtReExamination { get; set; }

        bool ShowEmergInPtReExamination { get; set; }

        bool IsChildUnder6YearsOld { get; set; }

        bool IsHICard_FiveYearsCont { get; set; }

        bool IsAllowCrossRegion { get; set; }

        IReceivePatient Parent { get; set; }

        HIQRCode QRCode { get; set; }

        bool CurrentlyUsed_ToConfirm_HI_Benefit { get; set; }

        void InitLoadControlData_FromExt(object objGenTask);
        bool IsReceivePatient { get; set; }
        bool ShowSaveHIAndConfirmCmd { get; set; }
        DateTime? FiveYearsAppliedDate { get; set; }
        DateTime? FiveYearsARowDate { get; set; }
        long V_ReceiveMethod { get; set; }

        //▼==== #001
        IDCardQRCode IDCardQRCode { get; set; }
        //▲==== #001
        void LoginHIAPI();
    }

    public interface IPatientDetailsAndHI
    {
        Patient CurrentPatient { get; set; }

        void InitPatient(Patient _currentPatient);

        //IPatientHiManagement HealthInsuranceContent { get; set; }
        /*TMA*/
        int PatientFindBy { get; set; }
        //void CloseGeneralInfoCmd();
        void SavePatientCmd();

        /// <summary>
        /// Tạo mới một bệnh nhân (dùng trên form, chưa đưa vào database)
        /// </summary>
        void CreateNewPatientAndHI(bool bCreateNewPatientDetailsOnly = false);

        bool ConfirmHIBeforeRegister();

        ObservableCollection<Gender> Genders { get; set; }
        ObservableCollection<Lookup> MaritalStatusList { get; set; }
        ObservableCollection<Lookup> EthnicsList { get; set; }
        ObservableCollection<Lookup> FamilyRelationshipList { get; set; }
        ObservableCollection<RefCountry> Countries { get; set; }
        ObservableCollection<CitiesProvince> Provinces { get; set; }

        /// <summary>
        /// Đang trong quá trình lưu thông tin bệnh nhân.
        /// </summary>
        bool IsSaving { get; set; }

        bool IsChildWindow { get; set; }

        /// <summary>
        /// Kiểm tra thông tin hành chính của bệnh nhân có thay đổi chưa.
        /// </summary>
        bool GeneralInfoChanged { get; set; }

        /// <summary>
        /// Trạng thái của form Thông tin bệnh nhân (Thêm mới, Cập nhật, ...)
        /// </summary>
        FormState FormState { get; set; }

        /// <summary>
        /// Tiêu đề của form Thông tin bệnh nhân. 
        /// </summary>
        string CurrentAction { get; set; }

        /// <summary>
        /// Danh sách thẻ bảo hiểm của bệnh nhân hiện tại.
        /// </summary>
        //BlankRowCollection<HealthInsurance> HealthInsurances { get; set; }

        /// <summary>
        /// Đóng form lại khi thao tác xong.
        /// </summary>
        bool CloseWhenFinish { get; set; }

        // TxD 14/07/2014 : The following method is replaced with LoadPatientDetailsAndHI_GenAction that is to be called
        //                  inside a CoRoutine to prevent RACING condition.
        //void StartEditingPatientLazyLoad(object patient);

        void LoadPatientDetailsAndHI_GenAction(object genCoRoutineTask, object patient, object ToRegisOutPt);

        ActivationMode ActivationMode { get; set; }

        /// <summary>
        /// Đang load thông tin hành chánh của bệnh nhân.
        /// </summary>
        bool IsLoading { get; set; }

        HiCardConfirmedEvent hiCardConfirmedEvent { get; set; }

        void Close();

        bool ShowCloseFormButton { get; set; }

        bool mNhanBenh_ThongTin_Sua { get; set; }
        bool mNhanBenh_ThongTin_XacNhan { get; set; }
        bool mNhanBenh_TheBH_ThemMoi { get; set; }
        bool mNhanBenh_TheBH_XacNhan { get; set; }
        bool mNhanBenh_DangKy { get; set; }
        bool mNhanBenh_TheBH_Sua { get; set; }

        AllLookupValues.RegistrationType RegistrationType { get; set; }

        // TxD 10/07/2014 : Added the following Property to enable ReConfirm HI Benefit for InPatient ONLY
        bool Enable_ReConfirmHI_InPatientOnly { get; set; }

        // TxD 15/07/2014 : Added the following to replace the Binding of ActivationMode (complicated and mixed up)  to the Visibility of 
        //                  the Health Insurance Tab
        bool ExpanderVisible { get; set; }

        bool IsConfirmedEmergencyPatient { get; set; }

        bool ShowConfirmedEmergencyPatient { get; set; }

        bool IsConfirmedForeignerPatient { get; set; }

        bool ShowConfirmedForeignerPatient { get; set; }

        bool EmergInPtReExamination { get; set; }

        bool ShowEmergInPtReExamination { get; set; }

        bool IsChildUnder6YearsOld { get; set; }

        bool IsHICard_FiveYearsCont { get; set; }

        //IReceivePatient Parent { get; set; }

        HIQRCode QRCode { get; set; }

        IPatientRegistrationNew ThePtRegNewView { get; set; }
    }
}
