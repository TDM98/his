using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
using System.IO;
using System.Collections.ObjectModel;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-22
 * Contents: Consultation Services Iterfaces - Patient Medical Records
/*******************************************************************/
#endregion

/*
 * 20180923 #001 TBL: BM 0000066. Added out long DTItemID
 * 20210428 #002 TNHX:  Lấy danh sách ICD sao kèm theo
 * 20210611 #003 TNHX:  346 Lấy danh sách RuleDiseasesReferences
 * 20220602 #004 DatTB: IssueID: 1619 | [CSKH] Thêm màn hình nhập kết quả cho KSK
 * 20230102 #005 BLQ: Thêm thông tin tờ tự khai
 * 20230307 #006 BLQ: Thêm định mức bàn khám 
 * 20230330 #007 QTD: Thêm ICD9 cho ngoại trú
 * 20230403 #008 QTD: Thêm mã máy
 * 20230503 #009 QTD: Thêm sơ kết điều trị
* 20230517 #010 DatTB: Chỉnh sửa service xóa giấy chuyển tuyến
* 20230518 #011 DatTB: Chỉnh service lưu thêm người thêm,sửa giấy chuyển tuyến
* 20230527 #012 DatTB: Thêm service xóa kết quả KSK
*/
namespace ConsultationsService.ePMRs
{
    [ServiceContract]
    public interface IePMRs
    {

        #region Common
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupBehaving();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupProcessingType();
        #endregion

        #region 1.MedicalRecordTemplate

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<MedicalRecordTemplate> GetAllMedRecTemplates();
        #endregion

        #region 2.DiagnosisTreatment

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatmentsByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatment_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetAllDiagnosisTreatments();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatmentByPtID_V2(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? ServiceRecID, long? PtRegDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt,long? V_Behaving, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagnosisTreatmentListByPtID_InPt(long patientID, long? V_Behaving, int PageIndex, int PageSize, out int TotalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment DiagnosisTreatment_GetLast(long PatientID, long PtRegistrationID, long ServiceRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetDiagnosisTreatment_InPt(long ServiceRecID);


        [OperationContract]
        [FaultContract(typeof (AxException))]
        DiagnosisTreatment GetDiagnosisTreatmentByDTItemID(long DTItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt(long patientID, long? V_DiagnosisType);

        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID);
        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetLatestDiagnosisTreatment_InPtByInstructionID_V2(long patientID, long? V_DiagnosisType, long IntPtDiagDrInstructionID, out List<DiagnosisIcd10Items> ICD10List);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long? spGetPtRegistrationIDInDiagnosisTreatment_Latest(long patientID, long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckDiagnosisTreatmentExists_PtRegDetailID(long patientID, long PtRegDetailID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckStatusInPtRegistration_InPtRegistrationID(long InPtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateInPtRegistrationID_PtRegistration(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetBlankDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDiagnosisTreatment(DiagnosisTreatment entity);
        //CRUDOperationResponse

        [OperationContract]
        [FaultContract(typeof(AxException))]
        /*▼====: #001*/
        //bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServicerecID);
        bool AddDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out long ServicerecID, out long DTItemID);
        /*▲====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddDiagnosisTreatmentAndPrescription(bool IsUpdateWithoutChangeDoctorIDAndDatetime, DiagnosisTreatment aDiagnosisTreatment, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> aDiagnosisIcd10Items, out long ServiceRecID
            //▼====: #007
            , long DiagnosisIcd9ListID, IList<DiagnosisICD9Items> aDiagnosisIcd9Items
            //▲====: #007
            , Int16 NumberTypePrescriptions_Rule, Prescription aPrescription, out long OutPrescriptID, out long OutIssueID, out string OutError
            , Prescription aPrescription_Old, bool AllowUpdateThoughReturnDrugNotEnough
            /*▼====: #001*/
            //, out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory);
            , out IList<PrescriptionIssueHistory> AllPrescriptionIssueHistory, out long DTItemID
            , SmallProcedure UpdatedSmallProcedure, long StaffID, IList<Resources> ResourceList, out long SmallProcedureID, out int VersionNumberOut);
            /*▲====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, out List<InPatientDeptDetail> ReloadInPatientDeptDetails);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #008
            , IList<Resources> ResourceList
            //▲====: #008
            , out List<InPatientDeptDetail> ReloadInPatientDeptDetails, SmallProcedure aSmallProcedure, out long SmallProcedureID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDiagnosisTreatment(DiagnosisTreatment entity, long DiagnosisIcd10ListID, IList<DiagnosisIcd10Items> XML, out int VersionNumberOut, bool IsUpdateWithoutChangeDoctorIDAndDatetime);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDiagnosisTreatment_InPt(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDiagnosisTreatment_InPt_V2(DiagnosisTreatment entity, long DiagnosisIcd10ListID, out List<InPatientDeptDetail> ReloadInPatientDeptDetails, IList<DiagnosisIcd10Items> XML, long DiagnosisICD9ListID, IList<DiagnosisICD9Items> ICD9XML
            //▼====: #008
            , IList<Resources> ResourceList
            //▲====: #008
            , SmallProcedure aSmallProcedure, bool IsUpdateDiagConfirmInPT, out int VersionNumberOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckValidProcedure(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisIcd10Items> GetDiagnosisIcd10Items_Load_InPt(long DTItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load_InPt(long DTItemID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDiseaseProgression(DiagnosisTreatment entity, bool IsUpdate);
        #endregion

        #region 3.PMRs
        [OperationContract]
        bool DeletePMR(PatientMedicalRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPMR(long ptID, string ptRecBarCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePMR(PatientMedicalRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientMedicalRecord> GetAllPMRsByPtID(long? patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientMedicalRecord> GetOpeningPMRsByPtID(long? patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientMedicalRecord> GetExpiredPMRsByPtID(long? patientID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientMedicalRecord> GetPMRsByPtID(long? patientID, int? inclExpiredPMR);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientMedicalRecord GetPMRByPtID(long? patientID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientMedicalRecord PatientMedicalRecords_ByPatientID(long patientID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientMedicalRecords_Save(long PatientRecID,long PatientID,string NationalMedicalCode,out string Error);

        #endregion

        #region thao tac tren file
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientMedicalFile PatientMedicalFiles_ByID(long ServiceRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientMedicalFile> PatientMedicalFiles_ByPatientRecID(long PatientRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckExists_PatientMedicalFiles(long ServiceRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Insert_PatientMedicalFiles(PatientMedicalFile entity, out long PatientRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientMedicalFiles_Update(PatientMedicalFile entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientMedicalFiles_Delete(PatientMedicalFile entity, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientMedicalFiles_Active(PatientMedicalFile entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool WriteFileXML(string path, string FileName, List<StringXml> Contents);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<StringXml> ReadFileXML(string FullPath);
        #endregion

        //-----Dinh them
        #region patient service

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<PatientServicesTree> GetPatientServicesTreeView(long patientID, bool IsCriterion_PCLResult, DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<PatientServicesTree> GetPatientServicesTreeViewEnum(long patientID, int PatientSummaryEnum, bool IsCriterion_PCLResult, DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertInPatientAdmDisDetails(InPatientAdmDisDetails entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InPatientAdmDisDetails GetInPatientAdmDisDetails(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistrationDetailEx> GetPatientRegistrationDetailsByRoom(out int totalCount,SeachPtRegistrationCriteria SeachRegCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToExcelBangKeChiTietKhamBenh(SeachPtRegistrationCriteria SeachRegCriteria, string StoreName, string ShowTitle);

        #endregion
        #region TreatmentProcess
        [OperationContract]
        [FaultContract(typeof(AxException))]
        TreatmentProcess SaveTreatmentProcess(TreatmentProcess entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        TreatmentProcess GetTreatmentProcessByPtRegistrationID(long PtRegistrationID);/*TMA*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteTreatmentProcess(long TreatmentProcessID, long StaffID);
        #endregion
        //HPT: Giấy chuyển tuyến
        [OperationContract]
        [FaultContract(typeof(AxException))]
        TransferForm SaveTransferForm(TransferForm entity, long StaffID); //==== #011

        //HPT: Giấy chuyển tuyến

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TransferForm> GetTransferForm(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID);/*TMA*/
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        TransferForm GetTransferFormByID(long TransferFormID,int V_PatientFindBy);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TransferForm> GetTransferForm_Date(string TextCriterial, long FindBy, int V_TransferFormType, int V_PatientFindBy, long? TransferFormID, DateTime FromDate, DateTime ToDate);/*TMA*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteTransferForm(long TransferFormID, long StaffID, string DeletedReason);  //==== #010

        //BaoLQ Thêm hàm check giấy chuyển tuyến khám ngoại trú
        [OperationContract]
        [FaultContract(typeof(AxException))]
        TransferForm GetTransferFormByPtRegistrationID( long PtRegistrationID);/*TMA*/
        #region Other Func

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosisTreatment GetLatestDiagnosisTreatmentByPtID_InPt_ForDiag(long? InPtRegistrationID, long? V_DiagnosisType);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        MedicalExaminationResult GetMedicalExaminationResultByPtRegistrationID(long PtRegistrationID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        MedicalExaminationResult GetMedicalExaminationResultByPtRegDetailID(long PtRegDetailID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult);

        //▼==== #012
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeleteMedicalExaminationResult(MedicalExaminationResult aMedicalExaminationResult);
        //▲==== #012

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TreatmentHistory> GetTreatmentHistoriesByPatientID(long PatientID, bool IsCriterion_PCLResult, DateTime? ToDate, DateTime? FromDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TreatmentHistory> GetTreatmentHistoriesByPtRegistrationID(long PtRegistrationID);

        //▼====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RequiredSubDiseasesReferences> GetListRequiredSubDiseasesReferences(string MainICD10);
        //▲====: #002
        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RuleDiseasesReferences> GetListRuleDiseasesReferences(string MainICD10);
        //▲====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamAccordingICD> GetListPCLExamAccordingICD(long PatientID, long V_SpecialistType, long PtRegistrationID);
        #region
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddUpdateDiagnosysConsultation(DiagnosysConsultationSummary gDiagConsultation, List<Staff> SurgeryDoctorCollection, List<DiagnosisIcd10Items> refICD10List, bool isUpdate, out long DiagConsultationSummaryID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DiagnosysConsultationSummary LoadDiagnosysConsultationSummary(long DiagConsultationSummaryID, out List<Staff> StaffList, out List<DiagnosisIcd10Items> ICD10List);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AdmissionExamination GetAdmissionExamination(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveAdmissionExamination(AdmissionExamination admissionExamination, out long AdmissionExaminationID_New);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisTreatment> GetDiagAndDoctorInstruction_InPt_ByPtRegID(long PtRegistrationID, long? DTItemID, long? DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(long IntPtDiagDrInstructionID, out string Disease_Progression, out string Diet, out long V_LevelCare,
             out string PCLExamTypeList, out IList<RefMedicalServiceGroupItem> MedServiceList, out string DrugList);

        //▼==== #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateConclusionMedicalExaminationResult(MedicalExaminationResult Result);
        //▲==== #004

        //▼==== #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        SelfDeclaration GetSelfDeclarationByPatientID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SelfDeclaration GetSelfDeclarationByPtRegistrationID(long PtRegistrationID, long V_RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveSelfDeclaration(SelfDeclaration Obj, long StaffID);
        //▲==== #005
        //▼==== #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckBeforeUpdateDiagnosisTreatment(long PtRegDetailID, long DoctorStaffID, out string errorMessages, out string confirmMessages);
        //▲==== #006

        //▼====: #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiagnosisICD9Items> GetDiagnosisICD9Items_Load(long DTItemID);
        //▲====: #007

        //▼====: #009
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TreatmentProcess> GetAllTreatmentProcessByPtRegistrationID(long PtRegistrationID);
        //▲====: #009

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDiagDetal_KhoaSan(long PtRegistrationID, int? SoConChet, DateTime? NgayConChet);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetSoConChet_KhoaSan(long PtRegistrationID, out int? SoConChet, out DateTime? NgayConChet);
    }
}