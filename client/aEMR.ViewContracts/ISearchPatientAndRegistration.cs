using DataEntities;
using aEMR.Common;

namespace aEMR.ViewContracts
{
    public interface ISearchPatientAndRegistration
    {
        void SearchPatientCmd();
        void SearchRegistrationCmd();
        void CreateNewPatientCmd();
        void SetDefaultButton(SearchRegistrationButtons defaultButton);
        void SetDefaultValue();
        void InitButtonVisibility(SearchRegButtonsVisibility values);
        //tìm để Khám bệnh -> tìm trong ngày -> không có thì tìm bệnh nhân
        bool IsSearchGoToKhamBenh { get; set; }
        void GetCurrentDate();
        bool CloseRegistrationFormWhenCompleteSelection { get; set; }
        bool PatientFindByVisibility { get; set; }
        AllLookupValues.PatientFindBy PatientFindBy { get; set; }
        //bool IsSearchingRegistration { get; }
        //bool IsLoading { get; }
        //bool? IsAdmission { get; set; }
        //bool ConsultationFormModeRUD { get; set; }
        bool mTimBN { get; set; }
        bool mThemBN { get; set; }
        bool mTimDangKy { get; set; }
        bool CanSearhRegAllDept { get; set; }
        LeftModuleActive LeftModule { get; set; }
        bool? SearchAdmittedInPtRegOnly { get; set; }
        long SearchByVregForPtOfType { get; set; }
        bool EnableSerchConsultingDiagnosy { get; set; }
        bool IsConsultingHistoryView { get; set; }
        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }

        // 20181018 TNHX: [BM0002186] Add the function to show to button "Tim ten BN"
        bool IsAllowSearchingPtByName_Visible { get; set; }
        bool IsSearchPtByNameChecked { get; set; }
        bool IsSearchByRegistrationDetails { get; set; }
        bool IsSearchGoToConfirmHI { get; set; }
        bool IsProcedureEdit { get; set; }
        bool IsSearchOutPtRegistrationOnly { get; set; }
        bool IsSearchPhysicalExaminationOnly { get; set; }
        bool IsSearchRegisAndGetPrescript { get; set; }
        long SearchRegisAndGetPrescriptPtRegID { get; set; }
        bool bEnableForConsultation { get; set; }
        bool IsSearchOnlyProcedure { get; set; }
        bool IsShowCallQMS { get; set; }
        bool IsShowCallButton { get; set; }
        bool IsShowGetTicketButton { get; set; }
        bool IsSearchOutPtTreatmentPre { get; set; }
        bool IsShowBtnChooseUserOfficial { get; set; }
        bool IsSearchForCashAdvance { get; set; }
    }
    public interface ISearchPatientAndRegistrationV2: ISearchPatientAndRegistration
    {

    }
}
