using DataEntities;
using System.Collections.ObjectModel;
/*
* 20180923 #001 TTM: Tạo hàm để trường hợp view này là màn hình con lấy đc thông tin Patient từ view cha (ConsultationSummary)
*/
namespace aEMR.ViewContracts
{
    public interface IPatientPCLLaboratoryResult
    {
        bool IsShowSummaryContent { get; set; }
        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        //▼====== #001
        void InitPatientInfo(Patient patientInfo);
        //▲====== #001
        bool IsDialogView { get; set; }
        void LoadPCLRequestResult(PatientPCLRequest aPCLRequest);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        ObservableCollection<PatientPCLLaboratoryResultDetail> PCLLaboratoryResultCollection { get; }
        PatientPCLRequest SelectedPCLLaboratory { get; }
        bool IsShowCheckTestItem { get; set; }
    }
}