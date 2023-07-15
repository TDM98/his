using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;
using aEMR.DataContracts;

/*
 * 20180922 #001 TBL: BM 0000073: Them parameter List<string> listICD10Codes vao BeginGetDrugsInTreatmentRegimen.
 * 20181004 #002 TTM: BM 0000138: Thêm Begin/End lấy chi tiết thuốc khi người dùng click vào toa thuốc.
 * 20181012 #003 TTM: Thay đổi cách check edit toa thuốc.
 * 20220823 #004 DatTB: Chỉnh sửa màn hình chờ nhận thuốc
 * 20220929 #005 DatTB:
 * + Thêm textbox tìm bệnh nhân theo tên/mã/stt
 * + Thêm đối tượng ưu tiên
 */
namespace ePrescriptionService
{
    [ServiceContract]
    public interface IePrescriptions
    {
        #region Common
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupPrescriptionType(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupPrescriptionType(IAsyncResult asyncResult);
        #endregion

        #region 1.Prescription
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByServiceRecID(long patientID, long? ServiecRecID, long? PtRegistrationID, DateTime? ExamDate, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByServiceRecID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPrescriptions(AsyncCallback callback, object state);
        IList<Prescription> EndGetAllPrescriptions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByPtID(long patientID, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByPtID_Paging(long patientID, long? V_PrescriptionType, bool isInPatient, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByPtID_Paging(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByID(long PrescriptID, AsyncCallback callback, object state);
        IList<Prescription> EndGetPrescriptionByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchPrescription(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<Prescription> EndSearchPrescription(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestPrescriptionByPtID(long patientID, AsyncCallback callback, object state);
        Prescription EndGetLatestPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLatestPrescriptionByPtID_InPt(long patientID, AsyncCallback callback, object state);
        Prescription EndGetLatestPrescriptionByPtID_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetChoNhanThuocList(DateTime fromDate, DateTime toDate,int IsWaiting, AsyncCallback callback, object state);
        ObservableCollection<Prescription> EndGetChoNhanThuocList(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateChoNhanThuoc(long outiID, bool IsWaiting, int CountPrint, AsyncCallback callback, object state);
        void EndUpdateChoNhanThuoc(out string Result,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetNewPrescriptionByPtID(long patientID, long doctorID, AsyncCallback callback, object state);
        Prescription EndGetNewPrescriptionByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionID(long PrescriptID, AsyncCallback callback, object state);
        Prescription EndGetPrescriptionID(IAsyncResult asyncResult);

        //▼====== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptDetailsStr_FromPrescriptID(long PrescriptID, AsyncCallback callback, object state);
        string EndGetPrescriptDetailsStr_FromPrescriptID(IAsyncResult asyncResult);
        //▲====== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionByIssueID(long IssueID, AsyncCallback callback, object state);
        Prescription EndGetPrescriptionByIssueID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePrescription(Prescription entity, AsyncCallback callback, object state);
        bool EndDeletePrescription(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddPrescription(Prescription entity, long patientID, long? PtRegistrationID, AsyncCallback callback, object state);
        //bool EndAddPrescription(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, AsyncCallback callback, object state);
        void EndPrescriptions_Update(out string Result, out long newPrescriptID, out long IssueID
            ,out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory,IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_InPt_Update(Prescription entity, Prescription entity_OLD, bool AllowUpdateThoughReturnDrugNotEnough, AsyncCallback callback, object state);
        void EndPrescriptions_InPt_Update(out string Result, out long newPrescriptID, out long IssueID, out string ServerError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_UpdateDoctorAdvice(Prescription entity, AsyncCallback callback, object state);
        void EndPrescriptions_UpdateDoctorAdvice(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_Add(Int16 NumberTypePrescriptions_Rule, Prescription entity, AsyncCallback callback, object state);
        bool EndPrescriptions_Add(out long newPrescriptID, out long IssueID, out string OutError
            , out IList<PrescriptionIssueHistory> allPrescriptionIssueHistory, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_InPt_Add(Prescription entity, AsyncCallback callback, object state);
        bool EndPrescriptions_InPt_Add(out long newPrescriptID, out long IssueID, out string OutError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_DuocSiEdit(Prescription entity, Prescription entity_BacSi, AsyncCallback callback, object state);
        bool EndPrescriptions_DuocSiEdit(out long newPrescriptID, out long IssueID, out string OutError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_DuocSiEditDuocSi(Prescription entity, Prescription entity_DuocSi, AsyncCallback callback, object state);
        bool EndPrescriptions_DuocSiEditDuocSi(out long newPrescriptID, out long IssueID, out string OutError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_TrongNgay_ByPatientID(Int64 PatientID, AsyncCallback callback, object state);
        List<Prescription> EndPrescriptions_TrongNgay_ByPatientID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_BaoHiemConThuoc_ByPatientID(Int64 PatientID, long PtRegDetailID, bool CungChuyenKhoa, AsyncCallback callback, object state);
        List<Prescription> EndPrescriptions_BaoHiemConThuoc_ByPatientID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID, AsyncCallback callback, object state);
        Prescription EndPrescription_ByPrescriptIDIssueID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDiagnosisIcd10Items_InDay(long PatientID, long ServiceRecID, long PtRegDetailID, AsyncCallback callback, object state);
        List<DiagnosisIcd10Items> EndGetAllDiagnosisIcd10Items_InDay(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptions_ListRootByPatientID_Paging(PrescriptionSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal, AsyncCallback callback, object state);
        List<Prescription> EndPrescriptions_ListRootByPatientID_Paging(out int Total, IAsyncResult asyncResult);

        //==== 20161004 CMN Begin: Create new interface to check Drug Interactions
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckDrugInteraction(string[] aGenericName, string[] aBrandName, AsyncCallback callback, object state);
        string EndCheckDrugInteraction(string[] aGenericName, string[] aBrandName, IAsyncResult asyncResult);
        //==== 20161004 CongNM End
        #endregion

        #region 2.GetDrugForPrescription
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, long? StoreID, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForPrescription_Auto(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchDrugForPrescription_Paging(String BrandName, bool IsSearchByGenericName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndSearchDrugForPrescription_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        /*▼====: #001*/
        //IAsyncResult BeginGetDrugsInTreatmentRegimen(long PtRegDetailID, AsyncCallback callback, object state);
        IAsyncResult BeginGetDrugsInTreatmentRegimen(long PtRegDetailID, List<string> listICD10Codes, AsyncCallback callback, object state);
        /*▲====: #001*/
        IList<GetDrugForSellVisitor> EndGetDrugsInTreatmentRegimen(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRefTreatmentRegimensAndDetail(long? PtRegDetailID, List<string> listICD10Codes, AsyncCallback callback, object state);
        IList<RefTreatmentRegimen> EndGetRefTreatmentRegimensAndDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetRefTreatmentRegimensAndDetailByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<RefTreatmentRegimen> EndGetRefTreatmentRegimensAndDetailByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditRefTreatmentRegimen(RefTreatmentRegimen aRefTreatmentRegimen, AsyncCallback callback, object state);
        bool EndEditRefTreatmentRegimen(out RefTreatmentRegimen aOutRefTreatmentRegimen, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchGenMedProductForPrescription_Paging(String BrandName, int IsInsurance, long? StoreID, int PageIndex, int PageSize, bool CountTotal, bool IsSearchByGenericName, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndSearchGenMedProductForPrescription_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForPrescription_Remaining(long? StoreID, string xml, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForPrescription_Remaining(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListDrugPatientUsed(long PatientID, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetListDrugPatientUsed(out int Total, IAsyncResult asyncResult);

        #endregion

        #region 3.Prescription Details
        //▼====== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID_V2(long PrescriptID, long? IssueID, long? AppointmentID, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID_V2(out bool CanEdit, out string ReasonCanEdit, out bool IsEdit, IAsyncResult asyncResult);
        //▲====== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID(long PrescriptID, bool GetRemaining, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID_WithNDay(long PrescriptID, bool GetRemaining, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID_WithNDay(out int NDay, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID_InPt(long PrescriptID, bool GetRemaining, bool OutPatient, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID_InPt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionDetailsByPrescriptID_InPt_V2(long PrescriptID, long[] V_CatDrugType, AsyncCallback callback, object state);
        IList<PrescriptionDetail> EndGetPrescriptionDetailsByPrescriptID_InPt_V2(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPrescriptDetailsByID(long prescriptID, AsyncCallback callback, object state);
        //IList<PrescriptionDetail> EndGetPrescriptDetailsByID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrugNotDisplayInList(long PatientID, long DrugID, bool? NotDisplayInList, AsyncCallback callback, object state);
        void EndUpdateDrugNotDisplayInList(IAsyncResult asyncResult);
        #endregion

        #region 4.PrescriptionIssueHistory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPrescriptIssueHistory(Int16 NumberTypePrescriptions_Rule, Prescription entity, AsyncCallback callback, object state);
        bool EndAddPrescriptIssueHistory(out string OutError, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginUpdatePrescriptIssueHistory(Prescription entity, AsyncCallback callback, object state);
        //bool EndUpdatePrescriptIssueHistory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPrescriptionIssueHistory(long PrescriptID, AsyncCallback callback, object state);
        IList<PrescriptionIssueHistory> EndGetPrescriptionIssueHistory(IAsyncResult asyncResult);


        #endregion

        #region 5.patientPaymentOld
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetValuePatientPaymentOld(long PtRegistrationID, AsyncCallback callback, object state);
        FeeDrug EndGetValuePatientPaymentOld(IAsyncResult asyncResult);

        #endregion

        #region choose dose member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInitChooseDoses(AsyncCallback callback, object state);
        IList<ChooseDose> EndInitChooseDoses(IAsyncResult asyncResult);
        #endregion

        #region PrescriptionDetailSchedules
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionDetailSchedules_ByPrescriptDetailID(Int64 PrescriptDetailID, bool IsNotIncat, AsyncCallback callback, object state);
        List<PrescriptionDetailSchedules> EndPrescriptionDetailSchedules_ByPrescriptDetailID(IAsyncResult asyncResult);
        #endregion

        #region PrescriptionDetailSchedulesLieuDung
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInitChoosePrescriptionDetailSchedulesLieuDung(AsyncCallback callback, object state);
        List<PrescriptionDetailSchedulesLieuDung> EndInitChoosePrescriptionDetailSchedulesLieuDung(IAsyncResult asyncResult);
        #endregion

        #region "PrescriptionNoteTemplates"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionNoteTemplates_GetAll(AsyncCallback callback, object state);
        IList<PrescriptionNoteTemplates> EndPrescriptionNoteTemplates_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionNoteTemplates_GetAllIsActive(PrescriptionNoteTemplates Obj,AsyncCallback callback, object state);
        IList<PrescriptionNoteTemplates> EndPrescriptionNoteTemplates_GetAllIsActive(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionNoteTemplates_Save(PrescriptionNoteTemplates Obj, AsyncCallback callback, object state);
        void EndPrescriptionNoteTemplates_Save(out string Result, IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionsTemplateInsert(PrescriptionTemplate Obj, AsyncCallback callback, object state);
        bool EndPrescriptionsTemplateInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionsTemplateDelete(PrescriptionTemplate Obj, AsyncCallback callback, object state);
        bool EndPrescriptionsTemplateDelete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPrescriptionsTemplateGetAll(PrescriptionTemplate Obj, AsyncCallback callback, object state);
        List<PrescriptionTemplate> EndPrescriptionsTemplateGetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointmentID(long issueID, bool isInPatient, AsyncCallback callback, object state);
        void EndGetAppointmentID(out long? appointmentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchOutPtTreatmentPrescription(PrescriptionSearchCriteria Criteria, DateTime SearchTime, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<Prescription> EndSearchOutPtTreatmentPrescription(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTotalHIPaymentForRegistration(long PtRegistrationID, AsyncCallback callback, object state);
        decimal EndGetTotalHIPaymentForRegistration(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckStatusPCLRequestBeforeCreateNew(long PtRegistrationID, bool IsGCT, AsyncCallback callback, object state);
        bool EndCheckStatusPCLRequestBeforeCreateNew(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRemainingMedicineDays(long OutPtTreatmentProgramID,long PtRegDetailID, AsyncCallback callback, object state);
        int? EndGetRemainingMedicineDays(out int MinNumOfDayMedicine, out int MaxNumOfDayMedicine, IAsyncResult asyncResult);

        //▼==== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetChoSoanThuocList(DateTime fromDate, DateTime toDate, int IsWaiting, string sPatientCode, string sPatientName, int sStoreServiceSeqNum, int pageIndex, int pageSize, AsyncCallback callback, object state); //==== #005, #008
        IList<Prescription> EndGetChoSoanThuocList(out int totalCount, IAsyncResult asyncResult); //==== #008
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateChoSoanThuoc(long PtRegistrationID, bool IsWaiting, int CountPrint, AsyncCallback callback, object state);
        void EndUpdateChoSoanThuoc(out string Result, IAsyncResult asyncResult);
        //▲==== #004
    }
}
