using DataEntities;
/*
 * 20180923 #001 TTM: Tạo hàm để trường hợp view này là màn hình con lấy đc thông tin Patient từ view cha (ConsultationSummary)
*/
namespace aEMR.ViewContracts
{
    public interface IViewResultPCLLaboratoryByExamTest
    {
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
        PatientPCLRequestSearchCriteria SearchCriteria { get; set; }
        void InitData();
        //▼====== #001
        void InitPatientInfo(Patient patientInfo);
        //▲====== #001
        void setDefaultWhenRenewPatient();
        bool IsDialogView { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}