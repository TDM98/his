using DataEntities;
using System.Collections.ObjectModel;
using System.Windows;

namespace aEMR.ViewContracts
{
    public delegate void ICD10Changed(ObservableCollection<DiagnosisIcd10Items> IDC10Collection);
    public interface IConsultationOld_InPt
    {
        DiagnosisTreatment DiagTrmtItem { get; set; }
        //bool IsChildWindow { get; set; }
        bool IsDailyDiagnosis { get; set; }
        void InitPatientInfo();
        bool IsDiagnosisOutHospital { get; set; }
        bool IsForCollectDiagnosis { get; set; }
        void SaveCreateNew(SmallProcedure aSmallProcedure, System.Collections.Generic.List<Resources> resourceList);
        bool IsProcedureEdit { get; set; }
        long? InPtRegistrationID { get; set; }
        void UpdateDiagTrmtItemIntoLayout(DiagnosisTreatment aDiagTrmtItem
            , ObservableCollection<DiagnosisIcd10Items> aRefIDC10List
            , ObservableCollection<DiagnosisICD9Items> aRefICD9List);
        ICD10Changed gICD10Changed { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }

        bool IsAdmRequest { get; set; }
        Visibility IsVisibleAdmRequest { get; set; }
        void SetDepartment();
        void HideAllButton();
        Staff gSelectedDoctorStaff { get; set; }
        bool IsUpdateDiagConfirmInPT { get; set; }
    }
}