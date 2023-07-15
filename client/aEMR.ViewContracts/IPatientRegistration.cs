using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientRegistration
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        //IPatientSummaryInfoV2 PatientSummaryInfoContent { get; set; }
        IPatientSummaryInfoV3 PatientSummaryInfoContent { get; set; }
        IRegistrationSummaryV2 RegistrationDetailsContent { get; set; }
        IInPatientSelectPcl SelectPCLContent { get; set; }
        //IDefaultExamType DefaultPclExamTypeContent {get;set;}
        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }
        DeptLocation DeptLocation { get; set; }

        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        HealthInsurance ConfirmedHiItem { get; set; }

        /// <summary>
        /// Thông tin giấy chuyển viện
        /// </summary>
        PaperReferal ConfirmedPaperReferal { get; set; }

        //ObservableCollection<RefDepartment> Departments { get; set; }
        //ObservableCollection<PatientClassification> PatientClassifications { get; set; }
        
        //RefDepartment Department { get; set; }
        ObservableCollection<RefMedicalServiceType> ServiceTypes { get; set; }
        RefMedicalServiceType MedServiceType { get; set; }
        ObservableCollection<RefMedicalServiceItem> MedicalServiceItems { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }

        /// <summary>
        /// Thêm một dịch vụ
        /// </summary>
        void AddRegItemCmd();

        /// <summary>
        /// Tạo mới đăng ký cho chính bệnh nhân đang được chọn.
        /// </summary>
        void CreateNewRegistrationForThisPatientCmd();
     
        /// <summary>
        /// Có thể đăng ký bệnh nhân.
        /// </summary>
        bool CanAddService { get; set; }

        //bool IsRegistering { get; set; }
        //bool IsLoadingAppointment { get; set; }
        bool IsLoadingRegistration { get; set; }

        void RefreshRegistration(PatientRegistration newRegInfo);


        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

        void ApplyViewCase(RegistrationViewCase aViewCase, DiagnosisTreatment CurrentDiagnosisTreatment);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void HandlePatientFromConsultation(Patient patient);
        bool RegistrationView { get; set; }
        bool IsProcess { get; set; }

        bool ShowConfirmedEmergencyPatient { get; set; }
    }
    public interface IPatientRegistration_V2
    {
        IPatientSummaryInfoV3 PatientSummaryInfoContent { get; set; }
        IRegistrationSummaryV2 RegistrationDetailsContent { get; set; }
        IInPatientSelectPcl SelectPCLContent { get; set; }
        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }
        DeptLocation DeptLocation { get; set; }
        HealthInsurance ConfirmedHiItem { get; set; }
        PaperReferal ConfirmedPaperReferal { get; set; }
        ObservableCollection<RefMedicalServiceType> ServiceTypes { get; set; }
        RefMedicalServiceType MedServiceType { get; set; }
        ObservableCollection<RefMedicalServiceItem> MedicalServiceItems { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }
        void AddRegItemCmd();
        void CreateNewRegistrationForThisPatientCmd();
        bool CanAddService { get; set; }
        bool IsLoadingRegistration { get; set; }
        void RefreshRegistration(PatientRegistration newRegInfo);
        void ApplyViewCase(RegistrationViewCase aViewCase, DiagnosisTreatment CurrentDiagnosisTreatment);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void HandlePatientFromConsultation(Patient patient);
        bool RegistrationView { get; set; }
        string Diagnosis { get; set; }
        bool IsProcess { get; set; }
        void GetRefTreatmentRegimensAndDetail(PatientRegistrationDetail aRegistrationDetail);
        bool IsFromPCLExamAccording { get; set; }
        ObservableCollection<PCLExamType> pCLExamTypes { get; set; }
        bool IsFromRequestDoctor { get; set; }
        string ICD10List { get; set; }
    }

    public interface IPatientRegistrationNew
    {
        ISearchPatientAndRegistration SearchRegistrationContent { get; set; }
        IPatientSummaryInfoV3 PatientSummaryInfoContent { get; set; }
        IRegistrationSummaryV3 RegistrationDetailsContent { get; set; }
        IInPatientSelectPcl SelectPCLContent { get; set; }
        //IDefaultExamType DefaultPclExamTypeContent {get;set;}
        Patient CurrentPatient { get; set; }
        PatientRegistration CurRegistration { get; set; }
        DeptLocation DeptLocation { get; set; }

        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        HealthInsurance ConfirmedHiItem { get; set; }

        /// <summary>
        /// Thông tin giấy chuyển viện
        /// </summary>
        PaperReferal ConfirmedPaperReferal { get; set; }

        //ObservableCollection<RefDepartment> Departments { get; set; }
        //ObservableCollection<PatientClassification> PatientClassifications { get; set; }

        //RefDepartment Department { get; set; }
        ObservableCollection<RefMedicalServiceType> ServiceTypes { get; set; }
        RefMedicalServiceType MedServiceType { get; set; }
        ObservableCollection<RefMedicalServiceItem> MedicalServiceItems { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }

        /// <summary>
        /// Thêm một dịch vụ
        /// </summary>
        void AddRegItemCmd();

        /// <summary>
        /// Tạo mới đăng ký cho chính bệnh nhân đang được chọn.
        /// </summary>
        void CreateNewRegistrationForThisPatientCmd();

        /// <summary>
        /// Có thể đăng ký bệnh nhân.
        /// </summary>
        bool CanAddService { get; set; }

        //bool IsRegistering { get; set; }
        //bool IsLoadingAppointment { get; set; }
        bool IsLoadingRegistration { get; set; }

        void RefreshRegistration(PatientRegistration newRegInfo);

        // Hpt 27/11/2015: Thêm biến này vào Interface để truyền vào VM, làm trung gian để gán giá trị cho PatientFindBy trong SearchPatientRegistrationViewModel, không dùng bắn sự kiện nữa
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }

    }

    public interface IPatientMedicalServiceRequest
    {
        string Diagnosis { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void CallDoOpenRegistration(long regID);
    }
    public interface ISetEkipForMedicalService
    {
        PatientRegistration CurrentRegistration { get; set; }
        Lookup Selected_Ekip { get; set; }
        bool SaveOK { get; set; }
    }
    public enum RegistrationViewCase
    {
        RegistrationView = 1,
        MedicalServiceGroupView = 2,
        RegistrationRequestView = 3
    }
}
