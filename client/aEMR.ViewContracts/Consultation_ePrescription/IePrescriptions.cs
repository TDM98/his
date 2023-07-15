using DataEntities;
using System.Collections.ObjectModel;

/*
 * 20180920 #001 TBL: Added IsPrescriptionChanged
 */
namespace aEMR.ViewContracts
{
    public interface IePrescriptions
    {
        //long ServiceRecID { get; set; }
        long PtRegistrationID { get; set; }
        //string DiagnosisForDrug { get; set; }
        //bool IsChildWindow { get; set; }
        bool IsShowSummaryContent { get; set; }
        void InitPatientInfo();
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        Prescription ObjTaoThanhToaMoi { get; set; }
        ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory { get; set; }
        void AddNewBlankDrugIntoPrescriptObjectNew();
        Prescription PrecriptionsBeforeUpdate { get; set; }
        void ChangeStatesBeforeUpdate();
        bool AllowUpdateThoughReturnDrugNotEnough { get; set; }
        bool CheckValidPrescription();
        bool btnSaveAddNewIsEnabled { get; }
        bool btnUpdateIsEnabled { get; }
        /*▼====: #001*/
        bool IsPrescriptionChanged { get;}
        /*▲====: #001*/
        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        void AllowModifyPrescription();
        void GetPhacDo();
        PhysicalExamination curPhysicalExamination { get; set; }
        void ResetPrescriptionInfoChanged();
        bool IsUpdateWithoutChangeDoctorIDAndDatetime { get; set; }
        bool IsChildControl { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        Staff Secretary { get; set; }
        long ServiceRecIDDiagTrmt { get; set; }
        void NotifyViewDataChanged();
    }
}