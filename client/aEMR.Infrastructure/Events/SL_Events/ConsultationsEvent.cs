using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
/*
 * 20191121 #001 TTM:   BM 0019594: Fix lỗi không thêm được dị ứng, tình trạng thể chất
 * 20210427 #002 TNHX:   Chọn ICD sao kèm theo nếu ICD chính có dấu găm
 * 20210611 #003 TNHX:   Chọn ICD kèm theo đối với ICD sao
 * 20220909 #004 DatTB: Thêm event load "Lời dặn" qua tab Toa thuốc xuất viện
 * 20230201 #005 DatTB: Thêm trường dữ liệu về KSNK trong phần Thông tin chung NB nội trú
 * 20230424 #006 DatTB: Thêm event truyền loại thiết bị đã chọn màn hình quản lý vật tư
 */
namespace aEMR.Infrastructure.Events
{
    public class ConsultationDoubleClickEvent
    {
        public DiagnosisTreatment DiagTrmtItem { get; set; }
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
    }

    public class ConsultationDoubleClickEvent_InPt_1
    {
    }

    public class ConsultationDoubleClickEvent_InPt_2
    {
        public DiagnosisTreatment DiagTrmtItem { get; set; }
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List { get; set; }
        public ObservableCollection<DiagnosisICD9Items> refICD9List { get; set; }
    }

    public class ePrescriptionDoubleClickEvent
    {
        public Prescription SelectedPrescription { get; set; }
        public bool isTemplate { get; set; }
    }


    public class ePrescriptionDoubleClickEvent_InPt_1
    {
    }

    public class ePrescriptionDoubleClickEvent_InPt_2
    {
        public Prescription SelectedPrescription { get; set; }
        public bool isTemplate { get; set; }
    }

    public class SelectListDrugDoubleClickEvent
    {
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList { get; set; }
    }
    public class ePrescriptionSelectedEvent
    {
        public Prescription SelectedPrescription { get; set; }
    }

    public class DuocSi_EditingToaThuocEvent
    {
        public Prescription SelectedPrescription { get; set; }
    }

    public class ReloadDataePrescriptionEvent
    {

    }

    public class ReloadePrescriptionTemplateEvent
    {

    }

    public class CommonClosedPhysicalForSummaryEvent
    {
    }

    public class CommonClosedPhysicalForDiagnosisEvent
    {
    }

    public class CommonClosedPhysicalForDiagnosis_InPtEvent
    {
    }
    //KMx: Sau khi kiểm tra, thấy event này không còn sử dụng nữa (26/05/2014 09:16).
    //public class SelectPatientChangeEvent
    //{
    //    public long PatientID { get; set; }
    //}

    public class ReloadDataConsultationEvent
    {

    }

    public class ClearAllDiagnosisListAfterAddNewEvent
    {

    }

    public class ClearAllDiagnosisListAfterUpdateEvent
    {

    }


    public class ClearAllDiagnosisListAfterAddNewEvent_InPt
    {

    }

    public class ClearAllDiagnosisListAfterUpdateEvent_InPt
    {

    }

    public class RiskFactorSaveCompleteEvent
    {

    }

    //▼==== #005
    public class SaveInfectionControlCompleteEvent
    {

    }
    //▲==== #005


    //public class RaiseEPrescriptoldEvent
    //{
    //    public DiagnosisTreatment DiagTrmtItem { get; set; }
    //}


    public class EventKhamChoVIP<TPtReg>
    {
        public TPtReg PtReg { get; set; }
    }


    public class ShowPatientInfo<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    public class ShowPatientInfo<TPt>
    {
        public TPt Pt { get; set; }        
    }

    public class InitDataForPtPCLLaboratoryResult
    {

    }

    public class InitDataForPtPCLImagingResult
    {

    }

    public class ShowPatientInfoForConsultation
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowInPatientInfoForConsultation
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPatientInfoFromPCLOutStandingTask
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPatientInfoFromPopUpSearchPCLRequest
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPatientInfoFromTextBoxSearchPCLRequest
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPtRegDetailForDiagnosis
    {
        public PatientRegistration PtRegistration { get; set; }
        public PatientRegistrationDetail PtRegDetail { get; set; }
    }

    public class ShowPtRegDetailForPrescription
    {
        public PatientRegistration PtRegistration { get; set; }
        public PatientRegistrationDetail PtRegDetail { get; set; }
    }

    public class ShowPMRForConsultation
    {

    }


    public class SendPrescriptionDetailSchedulesEvent<TData, THasSchedule, TTongThuoc, TSoNgayDung, TGhiChu, TModeForm>
    {
        public TData Data { get; set; }
        public THasSchedule HasSchedule { get; set; }
        public TTongThuoc TongThuoc { get; set; }
        public TSoNgayDung SoNgayDung { get; set; }
        public TGhiChu GhiChu { get; set; }
        public TModeForm ModeForm { get; set; }
    }

    public class SendPrescriptionDetailSchedulesFormEvent<TData, THasSchedule, TTongThuoc, TSoNgayDung, TGhiChu, TModeForm>
    {
        public TData Data { get; set; }
        public THasSchedule HasSchedule { get; set; }
        public TTongThuoc TongThuoc { get; set; }
        public TSoNgayDung SoNgayDung { get; set; }
        public TGhiChu GhiChu { get; set; }
        public TModeForm ModeForm { get; set; }
    }

    public class PrescriptionDrugNotInCatSelectedEvent<TPrescriptionDrugNotInCat, TIndex>
    {
        public TPrescriptionDrugNotInCat PrescriptionDrugNotInCat { get; set; }
        public TIndex Index { get; set; }
    }


    public class DiagnosisTreatmentSelectedEvent<TDiagnosisTreatment>
    {
        public TDiagnosisTreatment DiagnosisTreatment { get; set; }
    }
    public class DiagnosisTreatmentSelectedAndCloseEvent<TDiagnosisTreatment>
    {
        public TDiagnosisTreatment DiagnosisTreatment { get; set; }
    }

    //Sự kiện đọc lại cho Dị Ứng/Cảnh Báo
    public class Re_ReadAllergiesEvent
    {
    }
    public class Re_ReadWarningEvent
    {
    }
    //Sự kiện đọc lại cho Dị Ứng/Cảnh Báo

    public class ChooseDiagnosisTreatmentOpenPopupPrescriptionEvent<TDiagTrmtItem>
    {
        public TDiagTrmtItem DiagTrmtItem { get; set; }
    }

    public class ReLoadToaOfDuocSiEditEvent<TPrescription>
    {
        public TPrescription Prescription { get; set; }
    }

    public class PatientTreeChange
    {
        public PatientServicesTree curPatientServicesTree { get; set; }
    }

    public class PatientInfoChange
    {
    }

    public class PatientSummaryChange
    {
        public AllLookupValues.PatientSummary curPatientSummary { get; set; }
    }

    public class isConsultationStateEditEvent
    {
        public bool isConsultationStateEdit { get; set; }
    }

    public class GlobalCurPatientServiceRecordLoadComplete_Consult
    {
    }

    public class GlobalCurPatientServiceRecordLoadComplete_Consult_InPt
    {
    }

    public class GlobalCurPatientServiceRecordLoadComplete_EPrescript
    {
        public bool bJustCallAllowModifyPrescription = false;
    }

    public class LoadPrescriptionAfterSaved
    {
    }

    public class LoadPrescriptionInPtAfterSaved
    {
    }

    //Dinh them cai nay cho tung left module

    public class LoadGeneralInfoPageEvent
    {
        public Patient Patient { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_THONGTINCHUNG<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }
    //▼===== #001: Bổ sung thêm event do gộp màn hình khám bệnh nội trú
    public class ShowSummaryInPT
    {
        public Patient Patient { get; set; }
    }
    public class SetPhysicalForSummary_InPtEvent
    {
    }
    //▲===== #001
    public class ShowPatientInfo_KHAMBENH_TONGQUAT<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }
    public class ShowPatientInfo_KHAMBENH_CHANDOAN<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }
    public class ShowPatientInfo_KHAMBENH_RATOA<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    // Clear Prescription List
    public class ClearPrescriptionListAfterSelectPatientEvent
    {

    }

    public class ClearPrescriptionListAfterAddNewEvent
    {

    }

    public class ClearPrescriptionListAfterUpdateEvent
    {

    }

    // Clear Prescription Template
    public class ClearPrescriptTemplateAfterSelectPatientEvent
    {

    }

    // Clear Drug List Patient Used
    public class ClearDrugUsedAfterSelectPatientEvent
    {

    }

    public class ClearDrugUsedAfterAddNewEvent
    {

    }

    public class ClearDrugUsedAfterUpdateEvent
    {

    }

    public class ShowPatientInfo_KHAMBENH_LSBENHAN<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }
    public class ShowPatientInfo_KHAMBENH_BKCTKHAMBENH<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    //public class ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<TPt, TPtReg, TPtRegDetail>
    //{
    //    public TPt Pt { get; set; }
    //    public TPtReg PtReg { get; set; }
    //    public TPtRegDetail PtRegDetail { get; set; }
    //}

    public class ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM
    {
        public Patient Patient { get; set; }
    }

    public class ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH
    {
        public Patient Patient { get; set; }
    }

    public class ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH
    {
        public Patient Patient { get; set; }
        public PatientRegistration PtRegistration { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_HENCLS_HENCLS<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_CLS_KETQUA_HINHANH<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_CLS_KETQUA_XETNGHIEM<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }

    public class ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM<TPt, TPtReg, TPtRegDetail>
    {
        public TPt Pt { get; set; }
        public TPtReg PtReg { get; set; }
        public TPtRegDetail PtRegDetail { get; set; }
    }


    public class IPatientPCLDeptImagingExtResultDoneEvent
    {
        
    }

    public class Icd10CollectionSelected
    {
        public DiagnosisTreatment DiagnosisTreatment { get; set; }
        public IList<DiagnosisIcd10Items> Icd10Items { get; set; }
    }

    public class LoadDiagnosisTreatmentConfirmedForPrescript
    {

    }
    public class LoadDiagnosisTreatmentConfirmedForDischarge
    {
        public DiagnosisTreatment DiagnosisTreatment { get; set; }
    }
    public class SelectedDiagnosisTreatmentForConfirmSmallProceduce
    {
        public DiagnosisTreatment DiagnosisTreatment { get; set; }
        public IList<DiagnosisIcd10Items> Icd10Items { get; set; }
    }
    public class HaveTransferForm
    {
        public bool IsHaveTransferForm { get; set; }
    }
    public class CheckV_TreatmentType
    {
        public bool IsEnableAddPrescription { get; set; }
    }
    public class CheckValidProcedure
    {
        public bool IsHaveOneProcedure { get; set; }
    }
    public class SelectedRequireSubICDForDiagnosisTreatment
    {
        public DiseasesReference SubICDInfo { get; set; }
        public int MainICDIndex { get; set; }
        public bool IsException { get; set; }
    }
    //▼====: #003
    public class SelectedRuleICDForDiagnosisTreatment
    {
        public DiseasesReference SubICDInfo { get; set; }
        public int MainICDIndex { get; set; }
        public bool IsException { get; set; }
    }
    //▲====: #003
    public class NutritionalRating_Event_Save
    {
        public object Result { get; set; }
    }
    public class PCLExamAccordingICD_Event
    {
        public List<PCLExamType> ListPCLExamAccordingICD { get; set; }
    }
    public class AllPCLRequestClose
    {
        public bool AllPCLClose { get; set; }
    }
    public class AllPCLRequestImageClose
    {
        public bool AllPCLClose { get; set; }
    }
    public class BirthCertificates_Event_Save
    {
        public BirthCertificates Result { get; set; }
    }
    public class AdmissionCriterion_Symptom_Select_Event
    {
        public bool Result { get; set; }
        public string Diagnosis { get; set; }
    }
    public class ShortHandDictionary_Event_Save
    {
        public object Result { get; set; }
    }
    public class ConfirmOutPtTreatmentProgram
    {
        public bool Result { get; set; }
        public bool IsChronic { get; set; }
    }
    public class SaveOutPtTreatmentProgramItem
    {
        public int PrescriptionsAmount { get; set; }
    }
    //▼==== #004
    public class OnChangedUpdateAdmDisDetails
    {
        public string PrescriptionDoctorAdvice { get; set; }
    }
    //▲==== #004
    public class DeleteOutPtTreatmentProgramItem
    {
        public bool Result { get; set; }
    }

    public class UpdateDiagnosisTreatmentAndPrescription_Event
    {
        public bool Result { get; set; }
    }

    public class SelectedTreatmentDischarge_Event
    {
        public string SelectedTreatmentDischarge { get; set; }
    }
    public class SaveDisChargePapersInfo_Event
    {
        public bool Result { get; set; }
    }

    //▼==== #006
    public class OnChangedSelectionResourceTypes
    {
        public ObservableCollection<RefMedicalServiceType> selectedResourceTypes { get; set; }
        public string MServiceTypeListIDStr { get; set; }
    }
    //▲==== #006
    public class SaveDeathCheckRecord_Event
    {
        public bool Result { get; set; }
    }

}
