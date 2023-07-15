using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.DataContracts;
/*
 * 20180923 #001 TBL:   BM 0000066. Added out long DTItemID
 * 20191010 #002 TTM:   BM 0017443: [Kiểm soát nhiễm khuẩn]: Bổ sung màn hình hội chẩn.
 * 20210611 #003 TNHX:  346 Lấy danh sách RuleDiseasesReferences
 * 20220602 #004 DatTB: IssueID: 1619 | [CSKH] Thêm màn hình nhập kết quả cho KSK
 * 20230102 #005 BLQ: Thêm phiếu tự khai
 * 20230307 #006 BLQ: Thêm định mức bàn khám 
 * 20230330 #007 QTD: Thêm ICD9 cho ngoại trú
 * 20230403 #008 QTD: Thêm mã máy
 * 20230503 #009 QTD: Thêm dữ liệu sơ kết
* 20230517 #010 DatTB: Chỉnh sửa service xóa giấy chuyển tuyến
* 20230518 #011 DatTB: Chỉnh service lưu thêm người thêm,sửa giấy chuyển tuyến
* 20230527 #012 DatTB: Thêm service xóa kết quả KSK
*/
namespace ePMRsService
{
    [ServiceContract]
    public interface IePMRs
    {

        #region Common
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupBehaving(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupBehaving(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupProcessingType(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupProcessingType(IAsyncResult asyncResult);
        #endregion

        #region 1.MedicalRecordTemplate
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllMedRecTemplates(AsyncCallback callback, object state);
        IList<MedicalRecordTemplate> EndGetAllMedRecTemplates(IAsyncResult asyncResult);
        #endregion

        #region 2.DiagnosisTreatment
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentsByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate,AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatmentsByServiceRecID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetDiagnosisTreatment_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID, AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatment_InPt_ByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDiagnosisTreatments(AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetAllDiagnosisTreatments(IAsyncResult asyncResult);
                
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID, AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatmentByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentByPtID_V2(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID, long? PtRegDetailID,AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatmentByPtID_V2(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID, AsyncCallback callback, object state);
        DiagnosisTreatment EndDiagnosisTreatment_GetLast(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatment_InPt(long ServiceRecID, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetDiagnosisTreatment_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetLatestDiagnosisTreatmentByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetLatestDiagnosisTreatmentByPtID_InPt(IAsyncResult asyncResult);
        //==== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetLatestDiagnosisTreatment_InPtByInstructionID(IAsyncResult asyncResult);
        //==== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestDiagnosisTreatment_InPtByInstructionID_V2(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetLatestDiagnosisTreatment_InPtByInstructionID_V2(out ObservableCollection<DiagnosisIcd10Items> ICD10List, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID, AsyncCallback callback, object state);
        long? EndspGetPtRegistrationIDInDiagnosisTreatment_Latest(IAsyncResult asyncResult);

         
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID, AsyncCallback callback, object state);
        bool EndCheckDiagnosisTreatmentExists_PtRegDetailID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckStatusInPtRegistration_InPtRegistrationID(long InPtRegistrationID, AsyncCallback callback, object state);
        bool EndCheckStatusInPtRegistration_InPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInPtRegistrationID_PtRegistration(long PtRegistrationID, AsyncCallback callback, object state);
        void EndUpdateInPtRegistrationID_PtRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, long? V_Behaving, int PageIndex, int PageSize,  AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatmentListByPtID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagnosisTreatmentListByPtID_InPt(out int TotalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisTreatmentByDTItemID(long DTItemID, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetDiagnosisTreatmentByDTItemID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBlankDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetBlankDiagnosisTreatmentByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDiagnosisTreatment(DiagnosisTreatment entity, AsyncCallback callback, object state);
        bool EndDeleteDiagnosisTreatment(IAsyncResult asyncResult);
        //CRUDOperationResponse

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, AsyncCallback callback, object state);
        bool EndAddDiagnosisTreatment(out long ServicerecID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddDiagnosisTreatmentAndPrescription(bool IsUpdateWithoutChangeDoctorIDAndDatetime, DiagnosisTreatment aDiagnosisTreatment, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> aDiagnosisIcd10Items
            , long DiagnosisIcd9ListID, IList<DiagnosisICD9Items> aDiagnosisIcd9Items
            , Int16 NumberTypePrescriptions_Rule, Prescription aPrescription
            , Prescription aPrescription_Old, bool AllowUpdateThoughReturnDrugNotEnough
            , SmallProcedure UpdatedSmallProcedure, long StaffID
            , IList<Resources> ResourceList
            , AsyncCallback callback, object state);
        /*▼====: #001*/
        //bool EndAddDiagnosisTreatmentAndPrescription(out long ServicerecID, out long OutPrescriptID, out long OutIssueID, out string OutError, out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory, IAsyncResult asyncResult);
        int EndAddDiagnosisTreatmentAndPrescription(out long ServiceRecID, out long OutPrescriptID, out long OutIssueID, out string OutError, out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory, out long DTItemID, out long SmallProcedureID, out int VersionNumberOut, IAsyncResult asyncResult);
        /*▲====: #001*/

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, AsyncCallback callback, object state);
        bool EndAddDiagnosisTreatment_InPt(out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #008
            , IList<Resources> ResourceList
            //▲====: #008
            , SmallProcedure aSmallProcedure, AsyncCallback callback, object state);
        bool EndAddDiagnosisTreatment_InPt_V2(out List<InPatientDeptDetail> ReloadInPatientDeptDetails, out long SmallProcedureID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, AsyncCallback callback, object state);
        bool EndUpdateDiagnosisTreatment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, bool IsUpdateDiagConfirmInPT, AsyncCallback callback, object state);
        bool EndUpdateDiagnosisTreatment_InPt(out List<InPatientDeptDetail> ReloadInPatientDeptDetails, out int VersionNumberOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #008
            , IList<Resources> ResourceList
            //▲====: #008
            , SmallProcedure aSmallProcedure, bool IsUpdateDiagConfirmInPT, AsyncCallback callback, object state);
        bool EndUpdateDiagnosisTreatment_InPt_V2(out List<InPatientDeptDetail> ReloadInPatientDeptDetails, out int VersionNumberOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last, AsyncCallback callback, object state);
        IList<DiagnosisIcd10Items> EndGetDiagnosisIcd10Items_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckValidProcedure(long PtRegistrationID, AsyncCallback callback, object state);
        bool EndCheckValidProcedure(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisIcd10Items_Load_InPt(long DTItemID, AsyncCallback callback, object state);
        IList<DiagnosisIcd10Items> EndGetDiagnosisIcd10Items_Load_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisICD9Items_Load_InPt(long DTItemID, AsyncCallback callback, object state);
        IList<DiagnosisICD9Items> EndGetDiagnosisICD9Items_Load_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDiseaseProgression(DiagnosisTreatment entity, bool IsUpdate, AsyncCallback callback, object state);
        bool EndUpdateDiseaseProgression(IAsyncResult asyncResult);
        #endregion

        #region 3.PMRs
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePMR(PatientMedicalRecord entity, AsyncCallback callback, object state);
        bool EndDeletePMR(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPMR(long ptID, string ptRecBarCode, AsyncCallback callback, object state);
        bool EndAddPMR(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePMR(PatientMedicalRecord entity,AsyncCallback callback, object state);
        bool EndUpdatePMR(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPMRsByPtID(long? patientID, AsyncCallback callback, object state);
        IList<PatientMedicalRecord> EndGetAllPMRsByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOpeningPMRsByPtID(long? patientID,AsyncCallback callback, object state);
        IList<PatientMedicalRecord> EndGetOpeningPMRsByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetExpiredPMRsByPtID(long? patientID, AsyncCallback callback, object state);
        IList<PatientMedicalRecord> EndGetExpiredPMRsByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPMRByPtID(long? patientID, AsyncCallback callback, object state);
        PatientMedicalRecord EndGetPMRByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalRecords_ByPatientID(long patientID, AsyncCallback callback, object state);
        PatientMedicalRecord EndPatientMedicalRecords_ByPatientID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPMRsByPtID(long? patientID, int? inclExpiredPMR, AsyncCallback callback, object state);
        IList<PatientMedicalRecord> EndGetPMRsByPtID(IAsyncResult asyncResult);
        

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalRecords_Save(long PatientRecID, long PatientID, string NationalMedicalCode, AsyncCallback callback, object state);
        bool EndPatientMedicalRecords_Save(out string Error, IAsyncResult asyncResult);


        #endregion

        #region thao tac tren file
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFiles_ByID(long ServiceRecID, AsyncCallback callback, object state);
        PatientMedicalFile EndPatientMedicalFiles_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFiles_ByPatientRecID(long PatientRecID, AsyncCallback callback, object state);
        List<PatientMedicalFile> EndPatientMedicalFiles_ByPatientRecID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckExists_PatientMedicalFiles(long ServiceRecID, AsyncCallback callback, object state);
        bool EndCheckExists_PatientMedicalFiles(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsert_PatientMedicalFiles(PatientMedicalFile entity, AsyncCallback callback, object state);
        bool EndInsert_PatientMedicalFiles(out long PatientRecID,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFiles_Update(PatientMedicalFile entity, AsyncCallback callback, object state);
        bool EndPatientMedicalFiles_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFiles_Delete(PatientMedicalFile entity, long StaffID, AsyncCallback callback, object state);
        bool EndPatientMedicalFiles_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFiles_Active(PatientMedicalFile entity, AsyncCallback callback, object state);
        bool EndPatientMedicalFiles_Active(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginWriteFileXML(string path, string FileName, List<StringXml> Contents, AsyncCallback callback, object state);
        bool EndWriteFileXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginReadFileXML(string FullPath, AsyncCallback callback, object state);
        List<StringXml> EndReadFileXML(IAsyncResult asyncResult);
        #endregion

        #region dinh them

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientServicesTreeView(long patientID, bool IsCriterion_PCLResult, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<PatientServicesTree> EndGetPatientServicesTreeView(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientServicesTreeViewForCriterion_PCLResult(long patientID,DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<PatientServicesTree> EndGetPatientServicesTreeViewForCriterion_PCLResult(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientServicesTreeViewEnum(long patientID,int PatientSummaryEnum, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<PatientServicesTree> EndGetPatientServicesTreeViewEnum(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertInPatientAdmDisDetails(InPatientAdmDisDetails entity, AsyncCallback callback, object state);
        bool EndInsertInPatientAdmDisDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, AsyncCallback callback, object state);
        bool EndUpdateInPatientAdmDisDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInPatientAdmDisDetails(long PtRegistrationID, AsyncCallback callback, object state);
        InPatientAdmDisDetails EndGetInPatientAdmDisDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientRegistrationDetailsByRoom(SeachPtRegistrationCriteria SeachRegCriteria, AsyncCallback callback, object state);
        List<PatientRegistrationDetailEx> EndGetPatientRegistrationDetailsByRoom(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria, string StoreName, string ShowTitle, AsyncCallback callback, object state);
        byte[] EndExportToExcelBangKeChiTietKhamBenh(IAsyncResult asyncResult);

        #endregion
        #region TreatmentProcess
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveTreatmentProcess(TreatmentProcess entity, AsyncCallback callback, object state);
        TreatmentProcess EndSaveTreatmentProcess(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreatmentProcessByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        TreatmentProcess EndGetTreatmentProcessByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteTreatmentProcess(long TreatmentProcessID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteTreatmentProcess(IAsyncResult asyncResult);
        #endregion

        //HPT 09/02/2017: TransferForm
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, AsyncCallback callback, object state);/*TMA*/
        IList<TransferForm> EndGetTransferForm(IAsyncResult asyncResult);
        //HPT 09/02/2017: TransferForm
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveTransferForm(TransferForm entity, long StaffID, AsyncCallback callback, object state); //==== #011
        TransferForm EndSaveTransferForm(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTransferFormByID(long TransferFormID,int V_PatientFindBy, AsyncCallback callback, object state);
        TransferForm EndGetTransferFormByID(IAsyncResult asyncResult);

        /*TMA 20/11/2017*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);/*TMA*/
        IList<TransferForm> EndGetTransferForm_Date(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginDeleteTransferForm(long TransferFormID, long StaffID, string DeletedReason, AsyncCallback callback, object state); //==== #010
        bool EndDeleteTransferForm(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTransferFormByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        TransferForm EndGetTransferFormByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(long? InPtRegistrationID, long? V_DiagnosisType, AsyncCallback callback, object state);
        DiagnosisTreatment EndGetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetMedicalExaminationResultByPtRegDetailID(long PtRegDetailID, AsyncCallback callback, object state);
        MedicalExaminationResult EndGetMedicalExaminationResultByPtRegDetailID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetMedicalExaminationResultByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        MedicalExaminationResult EndGetMedicalExaminationResultByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginUpdateMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult, AsyncCallback callback, object state);
        void EndUpdateMedicalExaminationResult(IAsyncResult asyncResult);

        //▼===== #012
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginDeleteMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult, AsyncCallback callback, object state);
        void EndDeleteMedicalExaminationResult(IAsyncResult asyncResult);
        //▲===== #012

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetTreatmentHistoriesByPatientID(long PatientID, DateTime? ToDate, DateTime? FromDate, AsyncCallback callback, object state);
        IList<TreatmentHistory> EndGetTreatmentHistoriesByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetTreatmentHistoriesByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<TreatmentHistory> EndGetTreatmentHistoriesByPtRegistrationID(IAsyncResult asyncResult);

        //▼===== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddUpdateDiagnosysConsultation(DiagnosysConsultationSummary gDiagConsultation, List<Staff> SurgeryDoctorCollection, List<DiagnosisIcd10Items> refICD10List, bool isUpdate, AsyncCallback callback, object state);
        bool EndAddUpdateDiagnosysConsultation(out long DiagConsultationSummaryID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLoadDiagnosysConsultationSummary(long DiagConsultationSummaryID, AsyncCallback callback, object state);
        DiagnosysConsultationSummary EndLoadDiagnosysConsultationSummary(out List<Staff> StaffList, out List<DiagnosisIcd10Items> ICD10List ,IAsyncResult asyncResult);
        //▲===== #002
        //▼===== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListRequiredSubDiseasesReferences(string MainICD10, AsyncCallback callback, object state);
        IList<RequiredSubDiseasesReferences> EndGetListRequiredSubDiseasesReferences(IAsyncResult asyncResult);
        //▲===== #003
        //▼===== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListRuleDiseasesReferences(string MainICD10, AsyncCallback callback, object state);
        IList<RuleDiseasesReferences> EndGetListRuleDiseasesReferences(IAsyncResult asyncResult);
        //▲===== #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListPCLExamAccordingICD(long PatientID, long V_SpecialistType, long PtRegistrationID, AsyncCallback callback, object state);
        IList<PCLExamAccordingICD> EndGetListPCLExamAccordingICD(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAdmissionExamination(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        AdmissionExamination EndGetAdmissionExamination(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveAdmissionExamination(AdmissionExamination admissionExamination, AsyncCallback callback, object state);
        bool EndSaveAdmissionExamination(out long AdmissionExaminationID_New, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetDiagAndDoctorInstruction_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID, AsyncCallback callback, object state);
        IList<DiagnosisTreatment> EndGetDiagAndDoctorInstruction_InPt_ByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(AxException))]
        IAsyncResult BeginGetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(long IntPtDiagDrInstructionID, AsyncCallback callback, object state);
        void EndGetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(out string Disease_Progression ,out string Diet, out long V_LevelCare, out string PCLExamTypeList
            , out IList<RefMedicalServiceGroupItem> MedServiceList, out string DrugList, IAsyncResult asyncResult);

        //▼====: #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateConclusionMedicalExaminationResult(MedicalExaminationResult Result, AsyncCallback callback, object state);
        bool EndUpdateConclusionMedicalExaminationResult(IAsyncResult asyncResult);
        //▲====: #004

        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSelfDeclarationByPatientID(long PatientID, AsyncCallback callback, object state);
        SelfDeclaration EndGetSelfDeclarationByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSelfDeclarationByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        SelfDeclaration EndGetSelfDeclarationByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveSelfDeclaration(SelfDeclaration Obj, long StaffID, AsyncCallback callback, object state);
        bool EndSaveSelfDeclaration(IAsyncResult asyncResult);
        //▲====: #005 

        //▼====: #006
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCheckBeforeUpdateDiagnosisTreatment(long PtRegDetailID, long DoctorStaffID, AsyncCallback callback, object state);
        bool EndCheckBeforeUpdateDiagnosisTreatment(out string errorMessages, out string confirmMessages, IAsyncResult asyncResult);
        //▲====: #006

        //▼====: #007      
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDiagnosisICD9Items_Load(long DTItemID, AsyncCallback callback, object state);
        IList<DiagnosisICD9Items> EndGetDiagnosisICD9Items_Load(IAsyncResult asyncResult);
        //▲====: #007

        //▼====: #009
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllTreatmentProcessByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<TreatmentProcess> EndGetAllTreatmentProcessByPtRegistrationID(IAsyncResult asyncResult);
        //▲====: #009

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDiagDetal_KhoaSan(long PtRegistrationID, int? SoConChet, DateTime? NgayConChet, AsyncCallback callback, object state);
        bool EndAddNewDiagDetal_KhoaSan(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSoConChet_KhoaSan(long PtRegistrationID, AsyncCallback callback, object state);
        bool EndGetSoConChet_KhoaSan(out int? SoConChet, out DateTime? NgayConChet, IAsyncResult asyncResult);
    }
}

