using DataEntities;
using System.Collections.ObjectModel;

/*
 * 20180920 #001 TBL: Added IsDiagTrmentChanged
 */
namespace aEMR.ViewContracts
{
    public interface IConsultations_V3
    {
        //bool IsChildWindow { get; set; }
        bool IsPopUp { get; set; }
        bool IsShowSummaryContent { get; set; }
        void InitPatientInfo(PatientRegistrationDetail PtRegDetail);
        bool CheckValidDiagnosis();
        ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        long Compare2Object();
        DiagnosisTreatment DiagTrmtItem { get; set; }
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        bool btUpdateIsEnabled { get; }
        bool btSaveCreateNewIsEnabled { get; }
        /*▼====: #001*/
        bool IsDiagTrmentChanged { get; set; }
        /*▲====: #001*/
        ICS_DataStorage CS_DS { get; set; }
        bool IsShowEditTinhTrangTheChat { get; set; }
        void ApplySmallProcedure(SmallProcedure aSmallProcedureObj);
        SmallProcedure UpdatedSmallProcedure { get; }
        SmallProcedure SmallProcedureObj { get; }
        bool IsVisibility { get; set; }
        bool IsVisibilitySkip { get; set; }
        bool FormEditorIsEnabled { get; set; }
        ConsultationOldIsEditingEnable IsEditingEnable { get; set; }
        string ProcedureDescription { get; }
        string ProcedureDescriptionContent { get; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void NotifyViewDataChanged();
        bool IsOutPtTreatmentProgram { get; set; }
        IsInTreatmentProgramChanged IsInTreatmentProgramChangedCallback { get; set; }
        void ICD10Changed(ObservableCollection<DiagnosisIcd10Items> ICD10List);
        long Compare2ICD9List();
        ObservableCollection<DiagnosisICD9Items> refICD9List { get; set; }
        ObservableCollection<Resources> SelectedResourceList { get; set; }
    }
}