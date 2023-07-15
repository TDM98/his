using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IConsultationOld_V2
    {
        DiagnosisTreatment DiagTrmtItem { get; set; }
        void InitPatientInfo();
        bool CheckValidDiagnosis();
        ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        long Compare2Object();
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        bool IsShowSummaryContent { get; set; }
        bool IsDailyDiagnosis { get; set; }
        bool IsDiagnosisOutHospital { get; set; }
        bool btUpdateIsEnabled { get; set; }
        bool btSaveCreateNewIsEnabled { get; set; }
    }
}