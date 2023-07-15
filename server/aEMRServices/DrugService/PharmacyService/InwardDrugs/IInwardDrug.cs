using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using DataEntities;
using ErrorLibrary;
/*
 * 201470803 #001 CMN: Add HI Store Service
 * 20181124 #002 TTM: BM 0005309: Load dữ liệu chi tiết phiếu xuất kho BHYT màn hình Nhập trả khoa dược
 * 20200903 #003 TNHX: Thêm điều kiện tìm thuốc theo hoạt chất
*/
namespace PharmacyService
{
    [ServiceContract]
    public interface IInwardDrug
    {
        #region 0. Supplier member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Supplier> GetAllSupplierCbx(int supplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE);
        #endregion

        #region 1. RefShelfDrugLocation member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefShelfDrugLocation> GetRefShelfDrugLocationAutoComplete(string Name, int pageIndex, int pageSize);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertRefShelfDrugLocation(RefShelfDrugLocation Location, out long ID);
        #endregion

        #region 2. inwarddrug member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrugInvoice_SaveXML(InwardDrugInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugInvoice GetInwardInvoiceDrugByID(long invoiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrug> GetspInwardDrugDetailsByID(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrug> InwardDrugDetails_ByID(long inviID
                                                                                  , out decimal TongTienSPChuaVAT
                                                                                  , out decimal CKTrenSP
                                                                                  , out decimal TongTienTrenSPDaTruCK
                                                                                  , out decimal TongCKTrenHoaDon
                                                                                  , out decimal TongTienHoaDonCoThueNK
                                                                                  , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrugInvoice_Save(InwardDrugInvoice InvoiceDrug, out long inwardid, out string StrError);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug, bool IsNotCheckInvalid, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardInvoiceDrug(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddInwardDrug(InwardDrug invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrug_Insert(PharmacyPurchaseOrderDetail invoicedrug, long inviID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrug_InsertList(List<PharmacyPurchaseOrderDetail> OrderDetails, long inviID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrug_Update(InwardDrug invoicedrug, long StaffID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrug(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugInvoice> GetAllInwardInvoiceDrug(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugInvoice> SearchInwardInvoiceDrug(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefGenericDrugDetail GetRefGenericDrugDetail(long drugID, long? drugVersionID);

        #endregion

        #region 3. Outwarddrug


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddOutWardDrugInvoiceReturnVisitor(OutwardDrugInvoice InvoiceDrug, long ReturnSaffID, out long outwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoiceVisitor_Cancel(OutwardDrugInvoice InvoiceDrug, out long TransItemID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoicePrescriptChuaThuTien_Cancel(OutwardDrugInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoice_Delete(long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoice_UpdateStatus(OutwardDrugInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugInvoice GetOutWardDrugInvoiceVisitorByID(long outwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugInvoice GetOutWardDrugInvoiceReturnByID(long? OutiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugInvoice GetOutWardDrugInvoiceByID(long? OutiID);

        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugInvoice GetHIOutWardDrugInvoiceByID(long? OutiID);
        //==== #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchAllByStatus(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, bool? bFlagStoreHI, bool bFlagPaidTime, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchReturn(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_New(string BrandName, long StoreID, bool? IsCode, bool? IsSearchByGenericName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> RefGennericDrugDetails_GetRemaining_Paging(string BrandName, long StoreID, bool? IsCode, int PageSize, int PageIndex, bool? IsSearchByGenericName, out int Total);
        //▲====: #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber(long DrugID, long StoreID, bool? IsHIPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber_V2(long DrugID, long StoreID, bool? IsHIPatient, bool? InsuranceCover);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? PrescriptID, bool? IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long? PrescriptID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoice(long OutiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoice_V2(long OutiID, bool HI, long[] OutiIDArray, long? PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> spGetInBatchNumberAndPrice_List(long StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption(long PrescriptID, long StoreID, Int16 IsObject);

        //==== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciptionHI(long PrescriptID, long StoreID, Int16 IsObject);
        //==== #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption_V2(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetInBatchNumberAndPrice_ByPresciption_V3(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID = null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption_NotSave(Prescription ObjPrescription, long StoreID, Int16 RegistrationType);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInvoicePayed(OutwardDrugInvoice Outward, out long outiID, out long PaymentID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInvoiceInfo(OutwardDrugInvoice Outward);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugInvoice> OutwardDrugInvoice_CollectMultiDrug(int top, DateTime FromDate, DateTime ToDate, long StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugInvoice_UpdateCollectMulti(IEnumerable<OutwardDrugInvoice> lst);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugInvoice> OutwardInternalExportPharmacyInvoice_Cbx(long? StoreID);
        #endregion

        #region 5. Add Full OutWardDrug and Transaction member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CountMoneyForVisitorPharmacy(long outiID, out decimal AmountPaid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID, long? StaffID, long? CollectorDeptLocID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID, long? StaffID, long? CollectorDeptLocID);

        #endregion

        #region 21. Drug Expiry
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetListDrugExpiryDate(long? StoreID, int Type);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ListDrugExpiryDate_Save(OutwardDrugInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugInvoice> OutWardDrugInvoice_SearchByType(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        #region 6. Report member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrug> InwardDrugs_TonKho(string BrandName, long StoreID, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> OutwardDrugs_SellOnDate(string BrandName, long StoreID, DateTime FromDate, DateTime ToDate, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney);

        #endregion

        #region 30. StockTakes member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyStockTakes> PharmacyStockTakes_Search(PharmacyStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Get(long StoreID, DateTime StockTakeDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Load(long PharmacyStockTakeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyStockTakeDetails> GetDrugForAutoCompleteStockTake(string BrandName, long StoreID, bool IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyStockTake_Save(PharmacyStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool KetChuyenTonKho(long StoreID, long StaffID, string CheckPointName);

       
		[OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyStockTakeDetails> ReGetPharmacyStockTakeDetails(long storeID, DateTime stockTakeDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PharmacyStockTakes GetLastPharmacyStockTakes(long storeId);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyStockTake_Resave(PharmacyStockTakes StockTake, out long ID, out string StrError);
        #endregion

        #region 31. Request Form Drug For Pharmacy

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool FullOperatorRequestDrugInward(RequestDrugInward Request, out RequestDrugInward OutRequest);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RequestDrugInward GetRequestDrugInwardByID(long ReqDrugInID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByID(long ReqDrugInID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByRequestID(long ReqDrugInID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrug> GetRequestDrugInwardDetailByRequestIDNew(long ReqDrugInID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RequestDrugInward> SearchRequestDrugInward(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRequestDrugInward(long ReqDrugInID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrug> spGetInBatchNumberAndPrice_ByRequestPharmacy(bool? IsCost, long RequestID, long StoreID);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForRequestPharmacy(bool? IsCost, string BrandName, long StoreID, long? RequestID, bool? IsCode, bool? IsSearchByGenericName);
        //▲====: #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime? OutwardDrug_GetMaxDayBuyInsurance(long PatientID, long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugInvoice_SaveByType(OutwardDrugInvoice Invoice, out long ID, out string StrError);
        #endregion

        #region 32. SupplierPharmacyPaymentReqs Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_Details(InwardInvoiceSearchCriteria criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierPharmacyPaymentReqs_Save(SupplierPharmacyPaymentReqs PaymentReqs, out SupplierPharmacyPaymentReqs OutPaymentReqs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_DetailsByReqID(long PharmacySupplierPaymentReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SupplierPharmacyPaymentReqs SupplierPharmacyPaymentReqs_ID(long PharmacySupplierPaymentReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierPharmacyPaymentReqs> SupplierPharmacyPaymentReqs_Search(RequestSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierPharmacyPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierPharmacyPaymentReqs_Delete(long ID);
        #endregion

        #region 33. Hopital member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Hospital> Hopital_IsFriends();
        #endregion

        #region 43. PharmacyOutwardDrugReport Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetReport(PharmacyOutwardDrugReport para, long loggedStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyOutwardDrugReport_Save(PharmacyOutwardDrugReport para, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyOutwardDrugReport> PharmacyOutwardDrugReport_Search(SearchOutwardReport para, long loggedStaffID, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetID(long ID);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyPurchaseCheckOrder> PharmacyPurchaseOrderDetails_CheckOrder(long DrugID, DateTime FromDate, DateTime ToDate, out List<PharmacyPurchaseCheckOrderInward> InwardList);

        //▼====== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoiceForDrugDept(long OutiID);
        //▲====== #002


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetOutwardDrugAndPrice_ByListPresciption(ObservableCollection<Prescription> ListPrescription, long aStoreID, short aIsObject, bool IsHIOutPt, long PtRegistrationID, out DateTime LastDaySoldHI, out ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptIDForConfirm);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetOutwardDrugForOutPtTreatment(long IssueID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID = null, bool SecondTimesSell = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrug> GetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(long PtRegistrationID, Int16 RegistrationType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoCompleteFromCategory(bool? IsCost, string BrandName, long StoreID, long? RequestID, bool? IsCode, bool? IsSearchByGenericName);
    }
}