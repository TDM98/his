using DataEntities;
using System.Collections.ObjectModel;

/*
 * 20180920 #001 TBL: Added IsDiagTrmentChanged
 */
namespace aEMR.ViewContracts
{
    public interface IConsultationOld
    {
        DiagnosisTreatment DiagTrmtItem { get; set; }
        //bool IsChildWindow { get; set; }
        void InitPatientInfo();
        bool CheckValidDiagnosis();
        ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        long Compare2Object();
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        bool IsShowSummaryContent { get; set; }
        bool btUpdateIsEnabled { get; set; }
        bool btSaveCreateNewIsEnabled { get; set; }
        /*▼====: #001*/
        bool IsDiagTrmentChanged { get; set; }
        /*▲====: #001*/

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        bool IsShowEditTinhTrangTheChat { get; set; }
        bool IsUpdateFromPresciption { get; set; }
        ICD10Changed gICD10Changed { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void ICD10Changed(ObservableCollection<DiagnosisIcd10Items> ICD10List);
    }
}