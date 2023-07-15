using System.Collections.Generic;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using DataEntities;

using aEMR.Common.Utilities;
using System;

namespace aEMR.ViewContracts
{
    public delegate void CallCloseDialog();
    public interface IRegistrationSummaryV2
    {
        void Reset();
        //bool IsSavingRegistration { get; }
        //bool IsEnableRoleUser { get; set; }
        ///// <summary>
        ///// Content chứa các dịch vụ Thuốc chưa được tính tiền
        ///// </summary>
        //IOutPatientDrugManage NewDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ Thuốc đã được tính tiền rồi
        /// </summary>
        IOutPatientDrugManage OldDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS chưa được tính tiền
        /// </summary>
        IOutPatientPclRequestManage NewPclContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS đã được tính tiền rồi
        /// </summary>
        IOutPatientPclRequestManage OldPclContent { get; set; }
        /// <summary>
        /// Content chứa các dịch vụ đã được tính tiền rồi
        /// </summary>
        IOutPatientServiceManage OldServiceContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ chưa được tính tiền
        /// </summary>
        IOutPatientServiceManage NewServiceContent { get; set; }

        /// <summary>
        /// Chứa thông tin các lần trả tiền.
        /// </summary>
        IPatientPayment OldPaymentContent { get; set; }

        PatientRegistration CurrentRegistration { get; set; }
        PatientRegistration tempRegistration { get; set; }

        void AddNewService(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, long? ConsultationRoomStaffAllocID = null,
                            DateTime? ApptStartDate = null, DateTime? ApptEndDate = null, Staff staff = null, DateTime? datetime = null
            , string diagnosis = null, bool isPriority = false
            , decimal Qty = 1);
        void AddNewPclRequestDetailFromPclExamType(PCLExamType examType, DeptLocation deptLoc, Staff staff = null, DateTime? dt = null, string diagnosis = null, bool isPriority = false);
        void AddNewAllPclRequestDetailFromPclExamType(ObservableCollection<PCLExamType> AllExamType, Staff staff = null, DateTime? dt = null, string diagnosis = null);
        /// <summary>
        /// Đang lấy thông tin đăng ký từ server
        /// </summary>
        bool RegistrationLoading { get; set; }

        void RefreshServicesView();
        void RefreshPCLRequestDetailsView();
        void SetRegistration(PatientRegistration registrationInfo);

        //bool ShowCheckBoxColumn { get; set; }
        //bool ShowDeleteColumn { get; set; }

        void StartAddingNewServicesAndPclCmd(bool fromAppointment = false);
        //PatientClassification CurClassification { get; set; }

        /// <summary>
        /// Có đang sử dụng bảo hiểm hay không.
        /// </summary>
        bool HiServiceBeingUsed { get; set; }

        //RefDepartment Department { get; set; }
        //HealthInsurance ConfirmedHiItem { get; set; }
        //PaperReferal ConfirmedPaperReferal { get; set; }

        /// <summary>
        /// Delegate kiểm tra phần bảo hiểm, chuyển viện
        /// </summary>
        ValidateRegistrationInfo ValidateRegistration { get; set; }
        long DoctorStaffID { get; set; }

        bool CanAddService { get; }
        bool ShowButtonList { get; set; }
        bool ShowCheckBoxColumn { get; set; }
        bool CanStartAddingNewServicesAndPclEx { get; set; }
        bool CanStartEditRegistrationEx { get; set; }


        //authorization
        bool mDichVuDaTT_ChinhSua { get; set; }
        bool mDichVuDaTT_Luu { get; set; }
        bool mDichVuDaTT_TraTien { get; set; }
        bool mDichVuDaTT_In { get; set; }
        bool mDichVuDaTT_LuuTraTien { get; set; }

        bool mDichVuMoi_ChinhSua { get; set; }
        bool mDichVuMoi_Luu { get; set; }
        bool mDichVuMoi_TraTien { get; set; }
        bool mDichVuMoi_In { get; set; }
        bool mDichVuMoi_LuuTraTien { get; set; }
        bool IsShowCount15HIPercentCmd { get; set; }

        bool ShowClickButton { get; set; }

        RegistrationViewCase ViewCase { get; set; }
        RefMedicalServiceGroups RefMedicalServiceGroupObj { get; }
        void ApplyRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroupObj, DeptLocation aDeptLocation, bool IsAddNewOnly = false, Staff staff = null);
        List<RefMedicalServiceGroups> MedicalServiceGroupCollection { get; }
        bool IsFinalization { get; set; }
        string BasicDiagTreatment { get; set; }
        void TinhTongGiaTien();
        List<RefTreatmentRegimen> TreatmentRegimenCollection { get; set; }
        void BeginEdit();
        DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        CallCloseDialog gCallCloseDialog { get; set; }
        bool RegistrationView { get; set; }
        long ServiceRecID { get; set; }
        bool IsProcess { get; set; }
        TicketIssue TicketIssueObj { get; set; }
        long RequestDoctorStaffID { get; set; }
        bool IsPriority { get; set; }
        bool IsFromRequestDoctor { get; set; }
    }

    public delegate IEnumerator<IResult> ValidateRegistrationInfo(PatientRegistration regInfo, YieldValidationResult result);

    public interface IRegistrationSummaryV3
    {
        void Reset();
        //bool IsSavingRegistration { get; }
        //bool IsEnableRoleUser { get; set; }
        ///// <summary>
        ///// Content chứa các dịch vụ Thuốc chưa được tính tiền
        ///// </summary>
        //IOutPatientDrugManage NewDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ Thuốc đã được tính tiền rồi
        /// </summary>
        IOutPatientDrugManage OldDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS chưa được tính tiền
        /// </summary>
        IOutPatientPclRequestManage NewPclContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS đã được tính tiền rồi
        /// </summary>
        IOutPatientPclRequestManage OldPclContent { get; set; }
        /// <summary>
        /// Content chứa các dịch vụ đã được tính tiền rồi
        /// </summary>
        IOutPatientServiceManage OldServiceContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ chưa được tính tiền
        /// </summary>
        IOutPatientServiceManage NewServiceContent { get; set; }

        /// <summary>
        /// Chứa thông tin các lần trả tiền.
        /// </summary>
        IPatientPayment OldPaymentContent { get; set; }

        PatientRegistration CurrentRegistration { get; set; }

        void AddNewService(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType);
        void AddNewPclRequestDetailFromPclExamType(PCLExamType examType, DeptLocation deptLoc);
        void AddNewAllPclRequestDetailFromPclExamType(ObservableCollection<PCLExamType> AllExamType);
        /// <summary>
        /// Đang lấy thông tin đăng ký từ server
        /// </summary>
        bool RegistrationLoading { get; set; }

        void RefreshServicesView();
        void RefreshPCLRequestDetailsView();
        void SetRegistration(PatientRegistration registrationInfo);

        //bool ShowCheckBoxColumn { get; set; }
        //bool ShowDeleteColumn { get; set; }

        void StartAddingNewServicesAndPclCmd(bool fromAppointment = false);
        //PatientClassification CurClassification { get; set; }

        /// <summary>
        /// Có đang sử dụng bảo hiểm hay không.
        /// </summary>
        bool HiServiceBeingUsed { get; set; }

        //RefDepartment Department { get; set; }
        //HealthInsurance ConfirmedHiItem { get; set; }
        //PaperReferal ConfirmedPaperReferal { get; set; }

        /// <summary>
        /// Delegate kiểm tra phần bảo hiểm, chuyển viện
        /// </summary>
        ValidateRegistrationInfo ValidateRegistration { get; set; }
        long DoctorStaffID { get; set; }

        bool CanAddService { get; }
        bool ShowButtonList { get; set; }
        bool ShowCheckBoxColumn { get; set; }
        bool CanStartAddingNewServicesAndPclEx { get; set; }
        bool CanStartEditRegistrationEx { get; set; }


        //authorization
        bool mDichVuDaTT_ChinhSua { get; set; }
        bool mDichVuDaTT_Luu { get; set; }
        bool mDichVuDaTT_TraTien { get; set; }
        bool mDichVuDaTT_In { get; set; }
        bool mDichVuDaTT_LuuTraTien { get; set; }

        bool mDichVuMoi_ChinhSua { get; set; }
        bool mDichVuMoi_Luu { get; set; }
        bool mDichVuMoi_TraTien { get; set; }
        bool mDichVuMoi_In { get; set; }
        bool mDichVuMoi_LuuTraTien { get; set; }
        bool IsShowCount15HIPercentCmd { get; set; }

        bool ShowClickButton { get; set; }
    }

    public interface IMedServiceReqSummary
    {
        void Reset();
        //bool IsSavingRegistration { get; }
        //bool IsEnableRoleUser { get; set; }
        ///// <summary>
        ///// Content chứa các dịch vụ Thuốc chưa được tính tiền
        ///// </summary>
        //IOutPatientDrugManage NewDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ Thuốc đã được tính tiền rồi
        /// </summary>
        IOutPatientDrugManage OldDrugContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS chưa được tính tiền
        /// </summary>
        IOutPatientPclRequestManage NewPclContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ CLS đã được tính tiền rồi
        /// </summary>
        IOutPatientPclRequestManage OldPclContent { get; set; }
        /// <summary>
        /// Content chứa các dịch vụ đã được tính tiền rồi
        /// </summary>
        IOutPatientServiceManage OldServiceContent { get; set; }

        /// <summary>
        /// Content chứa các dịch vụ chưa được tính tiền
        /// </summary>
        IOutPatientServiceManage NewServiceContent { get; set; }

        /// <summary>
        /// Chứa thông tin các lần trả tiền.
        /// </summary>
        IPatientPayment OldPaymentContent { get; set; }

        PatientRegistration CurrentRegistration { get; set; }

        PatientRegistration tempRegistration { get; set; }

        void AddNewService(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, Staff staff, DateTime? dt, string diagnosis);
        void AddNewPclRequestDetailFromPclExamType(PCLExamType examType, DeptLocation deptLoc);
        void AddNewAllPclRequestDetailFromPclExamType(ObservableCollection<PCLExamType> AllExamType);
        /// <summary>
        /// Đang lấy thông tin đăng ký từ server
        /// </summary>
        bool RegistrationLoading { get; set; }

        void RefreshServicesView();
        void RefreshPCLRequestDetailsView();
        void SetRegistration(PatientRegistration registrationInfo);

        //bool ShowCheckBoxColumn { get; set; }
        //bool ShowDeleteColumn { get; set; }

        void StartAddingNewServicesAndPclCmd(bool fromAppointment = false);
        //PatientClassification CurClassification { get; set; }

        /// <summary>
        /// Có đang sử dụng bảo hiểm hay không.
        /// </summary>
        bool HiServiceBeingUsed { get; set; }

        //RefDepartment Department { get; set; }
        //HealthInsurance ConfirmedHiItem { get; set; }
        //PaperReferal ConfirmedPaperReferal { get; set; }

        /// <summary>
        /// Delegate kiểm tra phần bảo hiểm, chuyển viện
        /// </summary>
        ValidateRegistrationInfo ValidateRegistration { get; set; }
        long DoctorStaffID { get; set; }

        bool CanAddService { get; }
        bool ShowButtonList { get; set; }
        bool ShowCheckBoxColumn { get; set; }
        bool CanStartAddingNewServicesAndPclEx { get; set; }
        bool CanStartEditRegistrationEx { get; set; }


        //authorization
        bool mDichVuDaTT_ChinhSua { get; set; }
        bool mDichVuDaTT_Luu { get; set; }
        bool mDichVuDaTT_TraTien { get; set; }
        bool mDichVuDaTT_In { get; set; }
        bool mDichVuDaTT_LuuTraTien { get; set; }

        bool mDichVuMoi_ChinhSua { get; set; }
        bool mDichVuMoi_Luu { get; set; }
        bool mDichVuMoi_TraTien { get; set; }
        bool mDichVuMoi_In { get; set; }
        bool mDichVuMoi_LuuTraTien { get; set; }
        bool IsShowCount15HIPercentCmd { get; set; }

        bool ShowClickButton { get; set; }
        //20181205 TNHX: [BM0005300] Add function to Show button View/Print PhieuChiDinh
        bool ShowPhieuChiDinh_In { get; set; }
        long ServiceRecID { get; set; }
    }
}