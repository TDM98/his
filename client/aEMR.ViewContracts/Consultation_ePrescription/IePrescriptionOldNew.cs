using System.Collections.ObjectModel;
using DataEntities;
/*
 * 20180920 #001 TBL: Added IsPrescriptionChanged
 */
namespace aEMR.ViewContracts
{
    public interface IePrescriptionOldNew
    {
        bool DuocSi_IsEditingToaThuoc { get; set; }
        //void Init();

        ////Dược Sĩ thì không thấy mấy nút này
        bool btChonChanDoanVisibility { get; set; }
        ////Dược Sĩ thì không thấy mấy nút này

        //Nút Này chỉ dành cho Dược Sĩ
        bool btDuocSiEditVisibility { get; set; }
        //Nút Này chỉ dành cho Dược Sĩ


        bool hasTitle { get; set; }

        bool mToaThuocDaPhatHanh_ThongTin { get; set; }
        bool mToaThuocDaPhatHanh_ChinhSua { get; set; }
        bool mToaThuocDaPhatHanh_TaoToaMoi { get; set; }
        bool mToaThuocDaPhatHanh_PhatHanhLai { get; set; }
        bool mToaThuocDaPhatHanh_In { get; set; }
        bool mToaThuocDaPhatHanh_ChonChanDoan { get; set; }

        bool NumberOfTimesPrintVisibility { get; set; }

        void HandleDrugSchedule(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note);
        void HandleDrugScheduleForm(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note);
        void InitPatientInfo();
        void LoadPrescriptionInfo(long PrescriptID = 0);
        bool IsShowSummaryContent { get; set; }
        void ChangeStatesAfterUpdated(bool IsUpdate = false);
        Prescription ObjTaoThanhToaMoi { get; set; }
        ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory { get; set; }
        void AddNewBlankDrugIntoPrescriptObjectNew();
        Prescription PrecriptionsBeforeUpdate { get; set; }
        void ChangeStatesBeforeUpdate();
        bool AllowUpdateThoughReturnDrugNotEnough { get; set; }
        bool CheckValidPrescription();
        bool CheckValidPrescriptionWithDiagnosis();
        bool btnSaveAddNewIsEnabled { get; set; }
        bool btnUpdateIsEnabled { get; set; }
        /*▼====: #001*/
        bool IsPrescriptionInfoChanged { get; }
        /*▲====: #001*/

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        void AllowModifyPrescription();
        void GetPhacDo();
        PhysicalExamination curPhysicalExamination { get; set; }
        void ResetPrescriptionInfoChanged();
        bool IsViewOnly { get; set; }

        bool IsUpdateWithoutChangeDoctorIDAndDatetime { get; set; }
        bool IsChildControl { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        Staff Secretary { get; set; }
        long ServiceRecIDDiagTrmt { get; set; }
        long PtRegistrationID { get; set; }
        Prescription LatestePrecriptions { get; set; }
        void NotifyViewDataChanged();
    }
    public interface IePrescriptionSimple
    {
        void LoadPrescriptionInfo(long PrescriptID = 0);
        Prescription LatestePrecriptions { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}