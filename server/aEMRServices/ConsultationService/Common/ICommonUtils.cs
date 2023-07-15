using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
using DataEntities.MedicalInstruction;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-25
 * Contents: Consultation Services Iterfaces - Common Utils
/*******************************************************************/
#endregion
/*
 * 20171002 #001 CMN: Added GetAllSugeriesByPtRegistrationID
 * 20210428 #002 BLQ: Added GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID
 * 20220625 #003 DatTB: Thêm function lấy loại điều trị ngoại trú
 * 20230411 #004 BLQ: Thêm lấy TT HSBA cho điều trị ngoại trú
 * 20230415 #005 QTD: Thêm giấy ra viện
 * 20230503 #006 DatTB: 
 * + Viết service get/insert/update tuổi động mạch
 * + Viết stored get/insert/update tuổi động mạch
*/
namespace ConsultationsService.Common
{
    [ServiceContract]
    public interface ICommonUtils
    {
        #region 1.Drugs
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugNames(string brandName, long pageIndex, long pageSize, byte type);


        #endregion

        #region 2.DiseaseReferences
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DiseasesReference> SearchRefDiseases(string searchKey, long pageIndex, long pageSize, byte type, long PatientID, DateTime curDatetime, out int Total);

        #endregion

        #region RefICD9
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefICD9> SearchRefICD9(string searchKey, long pageIndex, long pageSize, byte ICD9SearchType, out int Total);

        #endregion

        #region 3.Staff
        [OperationContract]
        IList<Staff> SearchStaffFullName(string searchKey, long pageIndex, long pageSize);

        [OperationContract]
        IList<Staff> SearchStaffCat(Staff Staff, long pageIndex, long pageSize);

        [OperationContract]
        IList<Staff> GetAllStaffs();

        [OperationContract]
        IList<Staff> GetAllStaffs_FromStaffID(long nFromStaffID);

        #endregion

        #region 4.Department
        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefDepartment GetRefDepartmentByLID(long locationID);
        #endregion
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionIssueHistory> GetPrescriptionIssueHistoriesInPtBySerRecID(long ServiceRecID);

        #region ConsultingDiagnosys
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, out long ConsultingDiagnosysID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        ConsultingDiagnosys GetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultingDiagnosys> GetReportConsultingDiagnosys(ConsultingDiagnosysSearchCriteria aSearchCriteria, out int TotalItemCount);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SurgerySchedule> GetSurgerySchedules();
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SurgeryScheduleDetail> GetSurgeryScheduleDetails(long ConsultingDiagnosysID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SurgeryScheduleDetail_TeamMember> GetSurgeryScheduleDetail_TeamMembers(long ConsultingDiagnosysID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditSurgerySchedule(SurgerySchedule aEditSurgerySchedule, out long SurgeryScheduleID);
        /*▼====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> GetAllSugeriesByPtRegistrationID(long PtRegistrationID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveCatastropheByPtRegDetailID(long PtRegDetailID, long V_CatastropheType);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime? GetFirstExamDate(long PatientID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime? GetNextAppointment(long PatientID, long MedServiceID, DateTime CurentDate);
        /*▲====: #001*/
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditSmallProcedure(SmallProcedure aSmallProcedure, long StaffID, out long SmallProcedureID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SmallProcedure GetSmallProcedure(long PtRegDetailID, long? SmallProcedureID, long? V_RegistrationType = null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SmallProcedure GetLatesSmallProcedure(long PatientID, long MedServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int GetSmallProcedureTime(long MedServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByPtRegDetailID(long PtRegDetailID);

        //▼====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        HistoryAndPhysicalExaminationInfo GetHistoryAndPhysicalExaminationInfoByOutPtTreatmentProgramID(long OutPtTreatmentProgramID);
        //▲====: #002

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void EditHistoryAndPhysicalExaminationInfo(HistoryAndPhysicalExaminationInfo aItem, long StaffID, bool MaxillofacialTab, bool IsSaveSummary);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveOutPtTreatmentProgram(OutPtTreatmentProgram Item);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutPtTreatmentProgram> GetOutPtTreatmentProgramCollectionByPatientID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateTreatmentProgramIntoRegistration(long PtRegistrationID, long PtRegDetailID, long? OutPtTreatmentProgramID, out int OutPrescriptionsAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TicketCare> GetTicketCareByOutPtTreatmentProgramID(long OutPtTreatmentProgramID);

        //▼==== #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentType> GetAllOutpatientTreatmentType();
        //▲==== #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientRegistration> GetRegistrationByOutPtTreatmentProgramID(long OutPtTreatmentProgramID, bool IsDischargePapers);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveOutPtTreatmentProgramItem(PatientRegistration Registration);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutPtTreatmentProgramMarkDeleted(OutPtTreatmentProgram Item);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Staff> GetNarcoticDoctorOfficial(string SearchName, long NarcoticDoctorStaffID, DateTime ProcedureDateTime, bool IsInPt);

        //▼==== #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SummaryMedicalRecords> GetSummaryMedicalRecordByOutPtTreatmentProgramID(long OutPtTreatmentProgramID);
        //▲==== #004

        //▼==== #005
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveDisChargePapersInfo(DischargePapersInfo DischargePapersInfo);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DischargePapersInfo GetDischargePapersInfo(long PtRegistrationID, long V_RegistrationType, out string DoctorAdvice);
        //▲==== #005

        //▼==== #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveAgeOfTheArtery(AgeOfTheArtery obj, out long AgeOfTheArteryID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateAgeOfTheArtery(AgeOfTheArtery obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AgeOfTheArtery GetAgeOfTheArtery_ByPatient(long PtRegistrationID, long V_RegistrationType, long PatientClassID);
        //▲==== #006
    }
}
