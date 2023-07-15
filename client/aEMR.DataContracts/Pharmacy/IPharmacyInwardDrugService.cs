/*
 * 201470803 #001 CMN: Add HI Store Service
 * 20181124 #002 TTM: BM 0005309: Load chi tiết Phiếu xuất kho BHYT - Nhà thuốc cho màn hình nhập trả cho khoa dược
 * 20200903 #003 TNHX [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using DataEntities;
namespace InwardDrugProxy
{
    [ServiceContract]
    public interface IInwardDrug
    {
        #region 0. Supplier member

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetAllSupplierCbx(AsyncCallback callback, object state);
        //IList<Supplier> EndGetAllSupplierCbx(IAsyncResult asyncResult);

        #endregion

        #region 1. RefShelfDrugLocation member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefShelfDrugLocationAutoComplete(string Name, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<RefShelfDrugLocation> EndGetRefShelfDrugLocationAutoComplete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertRefShelfDrugLocation(RefShelfDrugLocation Location, AsyncCallback callback, object state);
        bool EndInsertRefShelfDrugLocation(out long ID, IAsyncResult asyncResult);

        #endregion

        #region 2. inwarddrug member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardInvoiceDrugByID(long invoiceID, AsyncCallback callback, object state);
        InwardDrugInvoice EndGetInwardInvoiceDrugByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetspInwardDrugDetailsByID(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrug> EndGetspInwardDrugDetailsByID(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugDetails_ByID(long inviID, AsyncCallback callback, object state);
        IList<InwardDrug> EndInwardDrugDetails_ByID(out decimal TongTienSPChuaVAT
                                                                                  , out decimal CKTrenSP
                                                                                  , out decimal TongTienTrenSPDaTruCK
                                                                                  , out decimal TongCKTrenHoaDon
                                                                                  , out decimal TongTienHoaDonCoThueNK
                                                                                  , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardInvoiceDrug_Save(InwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        bool EndInwardInvoiceDrug_Save(out long inwardid, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug, bool IsNotCheckInvalid, AsyncCallback callback, object state);
        int EndAddInwardInvoiceDrug(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardInvoiceDrug(InwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardInvoiceDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardInvoiceDrug(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardInvoiceDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardDrug(InwardDrug invoicedrug, AsyncCallback callback, object state);
        bool EndAddInwardDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrug_Insert(PharmacyPurchaseOrderDetail invoicedrug, long inviID, AsyncCallback callback, object state);
        bool EndInwardDrug_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrug_InsertList(List<PharmacyPurchaseOrderDetail> OrderDetails, long inviID, AsyncCallback callback, object state);
        bool EndInwardDrug_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrug_Update(InwardDrug invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndInwardDrug_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrug(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllInwardInvoiceDrug(int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugInvoice> EndGetAllInwardInvoiceDrug(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardInvoiceDrug(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugInvoice> EndSearchInwardInvoiceDrug(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenericDrugDetail(long drugID, long? drugVersionID, AsyncCallback callback, object state);
        RefGenericDrugDetail EndGetRefGenericDrugDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugInvoice_SaveXML(InwardDrugInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, AsyncCallback callback, object state);
        int EndInwardDrugInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        #endregion

        #region 3. Outwarddrug

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddOutWardDrugInvoiceReturnVisitor(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        bool EndAddOutWardDrugInvoiceReturnVisitor(out long outwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoiceVisitor_Cancel(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndOutWardDrugInvoiceVisitor_Cancel(out long TransItemID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoicePrescriptChuaThuTien_Cancel(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndOutWardDrugInvoicePrescriptChuaThuTien_Cancel(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoice_Delete(long id, AsyncCallback callback, object state);
        int EndOutWardDrugInvoice_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoice_UpdateStatus(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndOutWardDrugInvoice_UpdateStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInvoiceInfo(OutwardDrugInvoice Outward, AsyncCallback callback, object state);
        bool EndUpdateInvoiceInfo(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceVisitorByID(long outwardid, AsyncCallback callback, object state);
        OutwardDrugInvoice EndGetOutWardDrugInvoiceVisitorByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceReturnByID(long? OutiID, AsyncCallback callback, object state);
        OutwardDrugInvoice EndGetOutWardDrugInvoiceReturnByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, bool? bFlagStoreHI, bool bFlagPaidTime, AsyncCallback callback, object state);
        IList<OutwardDrugInvoice> EndGetOutWardDrugInvoiceSearchAllByStatus(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceSearchReturn(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<OutwardDrugInvoice> EndGetOutWardDrugInvoiceSearchReturn(out int totalCount, IAsyncResult asyncResult);

        //▼====== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_New(string BrandName, long StoreID, bool? IsCode, bool? IsSearchByGenericName, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorAutoComplete_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGennericDrugDetails_GetRemaining_Paging(string BrandName, long StoreID, bool? IsCode, int PageSize, int PageIndex
            , bool? IsSearchByGenericName, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndRefGennericDrugDetails_GetRemaining_Paging(out int Total, IAsyncResult asyncResult);
        //▲====== #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorBatchNumber(long DrugID, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorBatchNumber(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorBatchNumber_V2(long DrugID, long StoreID, bool? IsHIPatient, bool? InsuranceCover, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorBatchNumber_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? PrescriptID, bool? IsCode, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorAutoComplete_ForPrescription(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long? PrescriptID, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugDetailsByOutwardInvoice(long OutiID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetOutwardDrugDetailsByOutwardInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugDetailsByOutwardInvoice_V2(long OutiID, bool HI, long[] OutiIDArray, long? PtRegistrationID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetOutwardDrugDetailsByOutwardInvoice_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_List(long StoreID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndspGetInBatchNumberAndPrice_List(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByPresciption(long PrescriptID, long StoreID, Int16 IsObject, AsyncCallback callback, object state);
        IList<OutwardDrug> EndspGetInBatchNumberAndPrice_ByPresciption(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInBatchNumberAndPrice_ByPresciption_V3(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetInBatchNumberAndPrice_ByPresciption_V3(IAsyncResult asyncResult);

        //==== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByPresciptionHI(long PrescriptID, long StoreID, Int16 IsObject, AsyncCallback callback, object state);
        IList<OutwardDrug> EndspGetInBatchNumberAndPrice_ByPresciptionHI(IAsyncResult asyncResult);
        //==== #001

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByPresciption_V2(long PrescriptID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndspGetInBatchNumberAndPrice_ByPresciption_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByPresciption_NotSave(Prescription ObjPrescription, long StoreID, Int16 RegistrationType, AsyncCallback callback, object state);
        IList<OutwardDrug> EndspGetInBatchNumberAndPrice_ByPresciption_NotSave(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugInvoice_CollectMultiDrug(int top, DateTime FromDate, DateTime ToDate, long StoreID, AsyncCallback callback, object state);
        List<OutwardDrugInvoice> EndOutwardDrugInvoice_CollectMultiDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugInvoice_UpdateCollectMulti(IEnumerable<OutwardDrugInvoice> lst, AsyncCallback callback, object state);
        bool EndOutwardDrugInvoice_UpdateCollectMulti(IAsyncResult asyncResult);

        #endregion

        #region 5. Add Full OutWardDrug and Transaction member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCountMoneyForVisitorPharmacy(long outiID, AsyncCallback callback, object state);
        bool EndCountMoneyForVisitorPharmacy(out decimal AmountPaid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, long? StaffID, long? CollectorDeptLocID, AsyncCallback callback, object state);
        bool EndAddTransactionVisitor(out long PaymentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, long? StaffID, AsyncCallback callback, object state);
        bool EndAddTransactionHoanTien(out long PaymentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInvoicePayed(OutwardDrugInvoice Outward, AsyncCallback callback, object state);
        bool EndUpdateInvoicePayed(out long outiID, out long PaymentID, out string StrError, IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListDrugExpiryDate(long? StoreID, int Type, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetListDrugExpiryDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginListDrugExpiryDate_Save(OutwardDrugInvoice Invoice, AsyncCallback callback, object state);
        bool EndListDrugExpiryDate_Save(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoice_SearchByType(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<OutwardDrugInvoice> EndOutWardDrugInvoice_SearchByType(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceByID(long? OutiID, AsyncCallback callback, object state);
        OutwardDrugInvoice EndGetOutWardDrugInvoiceByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardInternalExportPharmacyInvoice_Cbx(long? StoreID, AsyncCallback callback, object state);
        List<OutwardDrugInvoice> EndOutwardInternalExportPharmacyInvoice_Cbx(IAsyncResult asyncResult);

        #region 6. Report member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugs_TonKho(string BrandName, long StoreID, bool IsDetail, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<InwardDrug> EndInwardDrugs_TonKho(out int TotalCount, out decimal TotalMoney, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugs_SellOnDate(string BrandName, long StoreID, DateTime FromDate, DateTime ToDate, bool IsDetail, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<OutwardDrug> EndOutwardDrugs_SellOnDate(out int TotalCount, out decimal TotalMoney, IAsyncResult asyncResult);

        #endregion

        #region 30. StockTakes member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyStockTakes_Search(PharmacyStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<PharmacyStockTakes> EndPharmacyStockTakes_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyStockTakeDetails_Get(long StoreID, DateTime StockTakeDate, AsyncCallback callback, object state);
        IList<PharmacyStockTakeDetails> EndPharmacyStockTakeDetails_Get(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyStockTakeDetails_Load(long PharmacyStockTakeID, AsyncCallback callback, object state);
        IList<PharmacyStockTakeDetails> EndPharmacyStockTakeDetails_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForAutoCompleteStockTake(string BrandName, long StoreID, bool IsCode, AsyncCallback callback, object state);
        IList<PharmacyStockTakeDetails> EndGetDrugForAutoCompleteStockTake(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyStockTake_Save(PharmacyStockTakes StockTake, bool IsConfirmFinished, AsyncCallback callback, object state);
        bool EndPharmacyStockTake_Save(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginKetChuyenTonKho(long StoreID, long StaffID, string CheckPointName, AsyncCallback callback, object state);
        bool EndKetChuyenTonKho(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyLockAndUnlockStore(long StoreID, bool IsLock, AsyncCallback callback, object state);
        bool EndPharmacyLockAndUnlockStore(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLastPharmacyStockTakes(long storeId, AsyncCallback callback, object state);
        PharmacyStockTakes EndGetLastPharmacyStockTakes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginReGetPharmacyStockTakeDetails(long storeID, DateTime stockTakeDate, AsyncCallback callback, object state);
        List<PharmacyStockTakeDetails> EndReGetPharmacyStockTakeDetails(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyStockTake_Resave(PharmacyStockTakes StockTake, AsyncCallback callback, object state);
        bool EndPharmacyStockTake_Resave(out long ID, out string StrError, IAsyncResult asyncResult);
        #endregion

        #region 31. Request Form Drug For Pharmacy

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFullOperatorRequestDrugInward(RequestDrugInward Request, AsyncCallback callback, object state);
        bool EndFullOperatorRequestDrugInward(out RequestDrugInward OutRequest, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardByID(long ReqDrugInID, AsyncCallback callback, object state);
        RequestDrugInward EndGetRequestDrugInwardByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardDetailByID(long ReqDrugInID, AsyncCallback callback, object state);
        List<RequestDrugInwardDetail> EndGetRequestDrugInwardDetailByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardDetailByRequestID(long ReqDrugInID, AsyncCallback callback, object state);
        List<RequestDrugInwardDetail> EndGetRequestDrugInwardDetailByRequestID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardDetailByRequestIDNew(long ReqDrugInID, AsyncCallback callback, object state);
        List<OutwardDrug> EndGetRequestDrugInwardDetailByRequestIDNew(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRequestDrugInward(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<RequestDrugInward> EndSearchRequestDrugInward(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRequestDrugInward(long ReqDrugInID, AsyncCallback callback, object state);
        bool EndDeleteRequestDrugInward(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByRequestPharmacy(bool? IsCost, long RequestID, long StoreID, AsyncCallback callback, object state);
        List<OutwardDrug> EndspGetInBatchNumberAndPrice_ByRequestPharmacy(IAsyncResult asyncResult);

        //▼====== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(bool? IsCost, string BrandName, long StoreID, long? RequestID, bool? IsCode
            , bool? IsSearchByGenericName, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(IAsyncResult asyncResult);
        //▲====== #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrug_GetMaxDayBuyInsurance(long PatientID, long outiID, AsyncCallback callback, object state);
        DateTime? EndOutwardDrug_GetMaxDayBuyInsurance(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugInvoice_SaveByType(OutwardDrugInvoice Invoice, AsyncCallback callback, object state);
        bool EndOutwardDrugInvoice_SaveByType(out long ID, out string StrError, IAsyncResult asyncResult);


        #endregion

        #region 32. SupplierPharmacyPaymentReqs Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_Details(InwardInvoiceSearchCriteria criteria, AsyncCallback callback, object state);
        List<InwardDrugInvoice> EndSupplierPharmacyPaymentReqs_Details(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_Save(SupplierPharmacyPaymentReqs PaymentReqs, AsyncCallback callback, object state);
        bool EndSupplierPharmacyPaymentReqs_Save(out SupplierPharmacyPaymentReqs OutPaymentReqs, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_DetailsByReqID(long PharmacySupplierPaymentReqID, AsyncCallback callback, object state);
        List<InwardDrugInvoice> EndSupplierPharmacyPaymentReqs_DetailsByReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_ID(long PharmacySupplierPaymentReqID, AsyncCallback callback, object state);
        SupplierPharmacyPaymentReqs EndSupplierPharmacyPaymentReqs_ID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_Search(RequestSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierPharmacyPaymentReqs> EndSupplierPharmacyPaymentReqs_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID, AsyncCallback callback, object state);
        bool EndSupplierPharmacyPaymentReqs_UpdateStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierPharmacyPaymentReqs_Delete(long ID, AsyncCallback callback, object state);
        bool EndSupplierPharmacyPaymentReqs_Delete(IAsyncResult asyncResult);
        #endregion

        #region 33. Hopital member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginHopital_IsFriends(AsyncCallback callback, object state);
        List<Hospital> EndHopital_IsFriends(IAsyncResult asyncResult);

        #endregion

        #region 43. PharmacyOutwardDrugReport Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyOutwardDrugReportDetail_GetReport(PharmacyOutwardDrugReport para, long loggedStaffID, AsyncCallback callback, object state);
        IList<PharmacyOutwardDrugReportDetail> EndPharmacyOutwardDrugReportDetail_GetReport(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyOutwardDrugReport_Save(PharmacyOutwardDrugReport para, AsyncCallback callback, object state);
        bool EndPharmacyOutwardDrugReport_Save(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyOutwardDrugReport_Search(SearchOutwardReport para, long loggedStaffID, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<PharmacyOutwardDrugReport> EndPharmacyOutwardDrugReport_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyOutwardDrugReportDetail_GetID(long ID, AsyncCallback callback, object state);
        IList<PharmacyOutwardDrugReportDetail> EndPharmacyOutwardDrugReportDetail_GetID(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrderDetails_CheckOrder(long DrugID, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<PharmacyPurchaseCheckOrder> EndPharmacyPurchaseOrderDetails_CheckOrder(out List<PharmacyPurchaseCheckOrderInward> InwardList, IAsyncResult asyncResult);

        //▼====== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugDetailsByOutwardInvoiceForDrugDept(long OutiID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetOutwardDrugDetailsByOutwardInvoiceForDrugDept(IAsyncResult asyncResult);
        //▲====== #002

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugAndPrice_ByListPresciption(ObservableCollection<Prescription> ListPrescription, long aStoreID, short aIsObject, bool IsHIOutPt, long PtRegistrationID, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetOutwardDrugAndPrice_ByListPresciption(out DateTime LastDaySoldHI, out ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptIDForConfirm, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugForOutPtTreatment(long IssueID, long StoreID, Int16 IsObject, bool HI, bool IsIssueID, long? PtRegistrationID, bool SecondTimesSell, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetOutwardDrugForOutPtTreatment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(long PtRegistrationID, Int16 RegistrationType, AsyncCallback callback, object state);
        IList<OutwardDrug> EndGetInBatchNumberAndPrice_AllPrescription_ByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoCompleteFromCategory
            (bool? IsCost, string BrandName, long StoreID, long? RequestID, bool? IsCode
            , bool? IsSearchByGenericName, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForSellVisitorAutoCompleteFromCategory(IAsyncResult asyncResult);
    }
}
