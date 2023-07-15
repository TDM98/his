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
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-11-22
 * Contents: Consultation Services Iterfaces - ePrescription
/*******************************************************************/
#endregion

/*
* 20180922 #001 TBL: BM 0000073. Them parameter List<string> listICD10Codes cho GetDrugsInTreatmentRegimen
* 20181004 #002 TTM: BM 0000138: Thêm hàm chỉ lấy chi tiết toa thuốc (không đầy đủ, trả về là string).
* 20181012 #003 TTM:   Chuyển GetPrescriptionDetailsByPrescriptID => GetPrescriptionDetailsByPrescriptID_V2
* 20220823 #004 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
* 20220929 #005 DatTB:
* + Thêm textbox tìm bệnh nhân theo tên/mã/stt
* + Thêm đối tượng ưu tiên
* 20230329 #006 DatTB: Thay đổi list soạn thuốc trước thành list phân trang
 */
namespace ConsultationsService.ePrescriptions
{
    [ServiceContract]
    public interface IePrescriptions
    {
        #region Common
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupPrescriptionType();
        #endregion

        #region 1.Prescription

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetAllPrescriptions();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetPrescriptionByPtID(long patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetPrescriptionByPtID_Paging(long patientID, long? V_PrescriptionType, bool isInPatient, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        IList<Prescription> GetPrescriptionByID(long PrescriptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> SearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetChoNhanThuocList(DateTime fromDate,DateTime toDate,int IsWaiting);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateChoNhanThuoc(long outiID, bool IsWaiting, int CountPrint, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> Prescription_Seach_WithIsSoldIssueID(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription GetLatestPrescriptionByPtID(long patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription GetLatestPrescriptionByPtID_InPt(long patientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription GetNewPrescriptionByPtID(long patientID, long doctorID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription GetPrescriptionID(long PrescriptID);

        //▼====== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetPrescriptDetailsStr_FromPrescriptID(long PrescriptID);
        //▲====== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription GetPrescriptionByIssueID(long IssueID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePrescription(Prescription entity);
        

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddPrescription(Prescription entity, long patientID, long? PtRegistrationID);

        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Prescriptions_UpdateDoctorAdvice(Prescription entity, out string Result);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddFullPrescription(Prescription entity, long? PtRegistrationID, out long newPrescriptID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddFullPrescriptionByServiceRecID(Prescription entity, out long newPrescriptID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Prescriptions_Add(Int16 NumberTypePrescriptions_Rule, Prescription entity, out long newPrescriptIDout, out long IssueID, out string OutError
            , out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Prescriptions_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID
            , out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Prescriptions_InPt_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, out string Result, out long NewPrescriptID, out long IssueID, out string ServerError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Prescriptions_InPt_Add(Prescription entity, out long newPrescriptIDout, out long IssueID, out string OutError);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Prescriptions_DuocSiEdit(Prescription entity, Prescription entity_BacSi, out long newPrescriptID, out long IssueID, out string OutError);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Prescriptions_DuocSiEditDuocSi(Prescription entity, Prescription entity_DuocSi, out long newPrescriptID, out long IssueID, out string OutError);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Prescription> Prescriptions_TrongNgay_ByPatientID(Int64 PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Prescription Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiagnosisIcd10Items> GetAllDiagnosisIcd10Items_InDay(long PatientID, long ServiceRecID, long PtRegDetailID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Prescription> Prescriptions_ListRootByPatientID_Paging(
            PrescriptionSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total);

        //==== 20161004 CMN Begin: Interface Drug Interaction
        [OperationContract]
        [FaultContract(typeof(AxException))]
        string CheckDrugInteraction(string[] aGenericName, string[] aBrandName);
        //==== 20161004 CMN End.
        #endregion

        #region 2.GetDrugForPrescription

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex,int IsMedDept,long? StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> SearchDrugForPrescription_Paging(String BrandName, bool IsSearchByGenericName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        /*▼====: #001*/
        //IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID);
        IList<GetDrugForSellVisitor> GetDrugsInTreatmentRegimen(long PtRegDetailID, List<string> listICD10Codes);
        /*▲====: #001*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetail(long? PtRegDetailID = null, List<string> listICD10Codes = null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefTreatmentRegimen> GetRefTreatmentRegimensAndDetailByPtRegistrationID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditRefTreatmentRegimen(RefTreatmentRegimen aRefTreatmentRegimen, out RefTreatmentRegimen aOutRefTreatmentRegimen);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> SearchGenMedProductForPrescription_Paging(String BrandName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, bool IsSearchByGenericName, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForPrescription_Remaining(long? StoreID, string xml);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetListDrugPatientUsed(long PatientID, int PageIndex, int PageSize, out int Total);
        #endregion

        #region 3.Prescription Details
        //▼====== #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_V2(long PrescriptID, long? IssueID, long? AppointmentID, out bool CanEdit, out string ReasonCanEdit, out bool IsEdit, bool GetRemaining);
        //▲====== #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID(long PrescriptID, bool GetRemaining);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_WithNDay(long PrescriptID, out int NDay, bool GetRemaining);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt(long PrescriptID, bool GetRemaining, bool OutPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionDetail> GetPrescriptionDetailsByPrescriptID_InPt_V2(long PrescriptID, bool GetRemaining, bool OutPatient, long[] V_CatDrugType = null);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PrescriptionDetail> GetPrescriptDetailsByID(long prescriptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateDrugNotDisplayInList(long PatientID, long DrugID, bool? NotDisplayInList);
        #endregion

        #region 4.PrescriptionIssueHistory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPrescriptIssueHistory(Int16 NumberTypePrescriptions_Rule, Prescription entity, out string OutError);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdatePrescriptIssueHistory(Prescription entity);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionIssueHistory> GetPrescriptionIssueHistory(long PrescriptID);

        
        #endregion

        #region 5.patientPaymentOld

        [OperationContract]
        [FaultContract(typeof(AxException))]
        FeeDrug GetValuePatientPaymentOld(long PtRegistrationID);

        #endregion

        #region choose dose member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ChooseDose> InitChooseDoses();
        #endregion

        #region PrescriptionDetailSchedules
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PrescriptionDetailSchedules> PrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID, bool IsNotIncat);
        #endregion

        #region PrescriptionDetailSchedulesLieuDung
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PrescriptionDetailSchedulesLieuDung> InitChoosePrescriptionDetailSchedulesLieuDung();
        #endregion


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Prescriptions_CheckCanEdit(Int64 PrescriptID, Int64 IssueID, out bool CanEdit, out string ReasonCanEdit);


        #region "PrescriptionNoteTemplates"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAll();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PrescriptionNoteTemplates> PrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTemplates Obj);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PrescriptionNoteTemplates_Save(PrescriptionNoteTemplates Obj, out string Result);


        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PrescriptionsTemplateInsert(PrescriptionTemplate Obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PrescriptionsTemplateDelete(PrescriptionTemplate Obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PrescriptionTemplate> PrescriptionsTemplateGetAll(PrescriptionTemplate Obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetAppointmentID(long issueID, bool isInPatient, out long? appointmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        decimal GetTotalHIPaymentForRegistration(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> SearchOutPtTreatmentPrescription(PrescriptionSearchCriteria Criteria, DateTime SearchTime, int pageIndex, int pageSize, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckStatusPCLRequestBeforeCreateNew(long PtRegistrationID, bool IsGCT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Prescription> Prescriptions_BaoHiemConThuoc_ByPatientID(Int64 PatientID, long PtRegDetailID, bool CungChuyenKhoa);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int? GetRemainingMedicineDays(long OutPtTreatmentProgramID, long PtRegDetailID, out int MinNumOfDayMedicine, out int MaxNumOfDayMedicine);

        //▼==== #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> GetChoSoanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting, string sPatientCode, string sPatientName, int sStoreServiceSeqNum, int pageIndex, int pageSize, out int totalCount); //==== #005, #006

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateChoSoanThuoc(long PtRegistrationID, bool IsWaiting, int CountPrint, out string Result);
        //▲==== #004
    }
}
