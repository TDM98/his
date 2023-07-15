using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IConsultations_V2
    {
        bool IsPopUp { get; set; }
        bool IsShowSummaryContent { get; set; }
        void InitPatientInfo();
        bool CheckValidDiagnosis();
        ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        long Compare2Object();
        DiagnosisTreatment DiagTrmtItem { get; set; }
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        bool IsDailyDiagnosis { get; set; }
        bool IsDiagnosisOutHospital { get; set; }
        void activeControl();
        bool btUpdateIsEnabled { get; }
        bool btSaveCreateNewIsEnabled { get; }
    }
}