using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IRegistration_DataStorage
    {
        PatientRegistration CurrentPatientRegistration { get; set; }
        PatientRegistrationDetail CurrentPatientRegistrationDetail { get; set; }
        Patient CurrentPatient { get; }
        ObservableCollection<PrescriptionIssueHistory> PrescriptionIssueHistories { get; set; }
        ObservableCollection<PatientServiceRecord> PatientServiceRecordCollection { get; set; }
        DiagnosisTreatment CurrentDiagnosisTreatment { get; set; }
        List<FilterPrescriptionsHasHIPay> ListFilterPrescriptionsHasHIPay { get; set; }
        List<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }
        List<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        // 20210610 TNHX: 331 Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
        ObservableCollection<PhysicalExamination> PtPhyExamList { get; set; }
    }
    public interface IConsultationsSummary_InPt
    {
        void CallSetInPatientInfoForConsultation(PatientRegistration aRegistration, PatientRegistrationDetail aRegistrationDetail);
        void CallPatientServiceRecordsGetForKhamBenh_Ext();
        bool IsUpdateDiagConfirmInPT { get; set; }
        string Title { get; set; }
    }
}