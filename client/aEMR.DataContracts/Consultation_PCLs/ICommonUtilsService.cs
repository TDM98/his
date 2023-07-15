using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.DataContracts;
using DataEntities.MedicalInstruction;
/*
 * 20220625 #001 DatTB: Thêm function lấy loại điều trị ngoại trú
* 20230111 #002 DatTB: Thêm ComboBox đối tượng khám bệnh
* 20230411 #003 BLQ: Thêm lấy TT HSBA cho điều trị ngoại trú
* 20230415 #004 QTD:  Lưu giấy ra viện
* 20230503 #005 DatTB: 
* + Viết service get/insert/update tuổi động mạch
* + Viết stored get/insert/update tuổi động mạch
*/
namespace CommonUtilService
{
    [ServiceContract]
    public interface ICommonUtils
    {
        #region 1.Drugs
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugNames(string brandName, long pageIndex, long pageSize, byte type, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugNames(IAsyncResult asyncResult);
        #endregion

        #region 2.DiseaseReferences
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDiseases(string searchKey, long pageIndex, long pageSize, byte type, long PatientID, DateTime curDatetime, AsyncCallback callback, object state);
        IList<DiseasesReference> EndSearchRefDiseases(out int Total,IAsyncResult asyncResult);
        #endregion

        #region RefICD9

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefICD9(string searchKey, long pageIndex, long pageSize, byte ICD9SearchType, AsyncCallback callback, object state);
        IList<RefICD9> EndSearchRefICD9(out int Total, IAsyncResult asyncResult);
        #endregion

        #region 3.Staff
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchStaffFullName(string searchKey, long pageIndex, long pageSize, AsyncCallback callback, object state);
        IList<Staff> EndSearchStaffFullName(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchStaffCat(Staff Staff, long pageIndex, long pageSize, AsyncCallback callback, object state);
        IList<Staff> EndSearchStaffCat(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaffs(AsyncCallback callback, object state);
        IList<Staff> EndGetAllStaffs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaffs_FromStaffID(long nFromStaffID, AsyncCallback callback, object state);
        IList<Staff> EndGetAllStaffs_FromStaffID(IAsyncResult asyncResult);

        #endregion

        #region 4.Department

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartmentByLID(long locationID, AsyncCallback callback, object state);
        RefDepartment EndGetRefDepartmentByLID(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity
            , AsyncCallback callback, object state);        
        List<PatientServiceRecord> EndPatientServiceRecordsGetForKhamBenh(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity, AsyncCallback callback, object state);
        List<PatientServiceRecord> EndPatientServiceRecordsGetForKhamBenh_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionIssueHistoriesInPtBySerRecID(long ServiceRecID, AsyncCallback callback, object state);
        IList<PrescriptionIssueHistory> EndGetPrescriptionIssueHistoriesInPtBySerRecID(IAsyncResult asyncResult);

        #region ConsultingDiagnosys
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, AsyncCallback callback, object state);
        bool EndUpdateConsultingDiagnosys(out long ConsultingDiagnosysID, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, AsyncCallback callback, object state);
        ConsultingDiagnosys EndGetConsultingDiagnosys(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetReportConsultingDiagnosys(ConsultingDiagnosysSearchCriteria aSearchCriteria, AsyncCallback callback, object state);
        ObservableCollection<ConsultingDiagnosys> EndGetReportConsultingDiagnosys(out int TotalItemCount, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSurgerySchedules(AsyncCallback callback, object state);
        List<SurgerySchedule> EndGetSurgerySchedules(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSurgeryScheduleDetails(long ConsultingDiagnosysID, AsyncCallback callback, object state);
        List<SurgeryScheduleDetail> EndGetSurgeryScheduleDetails(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSurgeryScheduleDetail_TeamMembers(long ConsultingDiagnosysID, AsyncCallback callback, object state);
        List<SurgeryScheduleDetail_TeamMember> EndGetSurgeryScheduleDetail_TeamMembers(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditSurgerySchedule(SurgerySchedule aEditSurgerySchedule, AsyncCallback callback, object state);
        bool EndEditSurgerySchedule(out long SurgeryScheduleID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllSugeriesByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndGetAllSugeriesByPtRegistrationID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveCatastropheByPtRegDetailID(long PtRegDetailID, long V_CatastropheType, AsyncCallback callback, object state);
        bool EndSaveCatastropheByPtRegDetailID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetFirstExamDate(long PatientID, AsyncCallback callback, object state);
        DateTime? EndGetFirstExamDate(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetNextAppointment(long PatientID, long MedServiceID, DateTime CurentDate, AsyncCallback callback, object state);
        DateTime? EndGetNextAppointment(IAsyncResult asyncResult);
        #endregion
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditSmallProcedure(SmallProcedure aSmallProcedure, long StaffID, AsyncCallback callback, object state);
        bool EndEditSmallProcedure(out long SmallProcedureID, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSmallProcedure(long PtRegDetailID, long? SmallProcedureID, long? V_RegistrationType, AsyncCallback callback, object state);
        SmallProcedure EndGetSmallProcedure(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus, AsyncCallback callback, object state);
        bool EndChangeStatus(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetLatesSmallProcedure(long PatientID, long MedServiceID, AsyncCallback callback, object state);
        SmallProcedure EndGetLatesSmallProcedure(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSmallProcedureTime(long MedServiceID, AsyncCallback callback, object state);
        int EndGetSmallProcedureTime(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHistoryAndPhysicalExaminationInfoByPtRegDetailID(long PtRegDetailID, AsyncCallback callback, object state);
        HistoryAndPhysicalExaminationInfo EndGetHistoryAndPhysicalExaminationInfoByPtRegDetailID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, AsyncCallback callback, object state);
        HistoryAndPhysicalExaminationInfo EndGetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditHistoryAndPhysicalExaminationInfo(HistoryAndPhysicalExaminationInfo aItem, long StaffID, bool MaxillofacialTab, bool IsSaveSummary, AsyncCallback callback, object state);
        void EndEditHistoryAndPhysicalExaminationInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveOutPtTreatmentProgram(OutPtTreatmentProgram Item, AsyncCallback callback, object state);
        void EndSaveOutPtTreatmentProgram(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetOutPtTreatmentProgramCollectionByPatientID(long PatientID, AsyncCallback callback, object state);
        List<OutPtTreatmentProgram> EndGetOutPtTreatmentProgramCollectionByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateTreatmentProgramIntoRegistration(long PtRegistrationID, long PtRegDetailID, long? OutPtTreatmentProgramID, AsyncCallback callback, object state);
        void EndUpdateTreatmentProgramIntoRegistration(out int OutPrescriptionsAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetTicketCareByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, AsyncCallback callback, object state);
        List<TicketCare> EndGetTicketCareByOutPtTreatmentProgramID(IAsyncResult asyncResult);

        //▼==== #001
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllOutpatientTreatmentType(AsyncCallback callback, object state);
        List<OutpatientTreatmentType> EndGetAllOutpatientTreatmentType(IAsyncResult asyncResult);
        //▲==== #001

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRegistrationByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, bool IsDischargePapers, AsyncCallback callback, object state);
        List<PatientRegistration> EndGetRegistrationByOutPtTreatmentProgramID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveOutPtTreatmentProgramItem(PatientRegistration Registration, AsyncCallback callback, object state);
        bool EndSaveOutPtTreatmentProgramItem(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginOutPtTreatmentProgramMarkDeleted(OutPtTreatmentProgram Item, AsyncCallback callback, object state);
        bool EndOutPtTreatmentProgramMarkDeleted(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetNarcoticDoctorOfficial(string SearchName, long NarcoticDoctorStaffID, DateTime ProcedureDateTime, bool IsInPt, AsyncCallback callback, object state);
        List<Staff> EndGetNarcoticDoctorOfficial(IAsyncResult asyncResult);
        //▼==== #002
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllPatientClassification(AsyncCallback callback, object state);
        List<PatientClassification> EndGetAllPatientClassification(IAsyncResult asyncResult);
        //▲==== #002
        //▼==== #003
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetSummaryMedicalRecordByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, AsyncCallback callback, object state);
        List<SummaryMedicalRecords> EndGetSummaryMedicalRecordByOutPtTreatmentProgramID(IAsyncResult asyncResult);
        //▲==== #003


        //▼==== #004
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveDisChargePapersInfo(DischargePapersInfo DischargePapersInfo, AsyncCallback callback, object state);
        bool EndSaveDisChargePapersInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetDischargePapersInfo(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        DischargePapersInfo EndGetDischargePapersInfo(out string DoctorAdvice, IAsyncResult asyncResult);
        //▲==== #004

        //▼==== #005
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSaveAgeOfTheArtery(AgeOfTheArtery obj, AsyncCallback callback, object state);
        bool EndSaveAgeOfTheArtery(out long AgeOfTheArteryID, IAsyncResult asyncResult);
        
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginUpdateAgeOfTheArtery(AgeOfTheArtery obj, AsyncCallback callback, object state);
        bool EndUpdateAgeOfTheArtery(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAgeOfTheArtery_ByPatient(long PtRegistrationID, long V_RegistrationType, long PatientClassID, AsyncCallback callback, object state);
        AgeOfTheArtery EndGetAgeOfTheArtery_ByPatient(IAsyncResult asyncResult);
        //▲==== #005
    }
}