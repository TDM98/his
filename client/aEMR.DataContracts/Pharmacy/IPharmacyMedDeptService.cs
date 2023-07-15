/*
 * 20170821 #001 CMN: Added AdjustClinicPrice Service
 * 20181124 #002 TTM: BM 0005309: Load Phiếu xuất cho combobox phiếu xuất kho BHYT ở màn hình nhập trả cho khoa dược
 * 20180412 #003 TTM: BM 0005324: Load Phiếu xuất cho combobox phiếu xuất kho BHYT ở màn hình nhập từ khoa dược ở Module Nhà thuốc.
 * 20200807 #004 TNHX: Thêm loại để nhập hàng cho hóa chất
 * 20220106 #005 TNHX: Lọc danh sách thuốc/ y cụ theo danh mục COVID
 * 20221129 #006 BLQ: Thêm hàm quản lý đơn thuốc điện tử
 * 20230213 #007 QTD: Thêm DTDT Nhà thuốc
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace MedDeptProxy
{
    [ServiceContract]
    public interface IInMedDept
    {
        #region 1. Inward Drug

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardDrugClinicDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic, AsyncCallback callback, object state);
        List<OutwardDrugMedDeptInvoice> EndOutwardDrugMedDeptInvoice_Cbx(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, bool IsNotCheckInvalid, AsyncCallback callback, object state);
        int EndAddInwardDrugMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        //▼====== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, long? V_MedProductType, AsyncCallback callback, object state);
        int EndUpdateInwardDrugMedDeptInvoice(IAsyncResult asyncResult);
        //▲====== #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrugMedDeptInvoice(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        int EndDeleteInwardDrugMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, long? V_MedProductType, AsyncCallback callback, object state);
        bool EndInwardDrugMedDept_InsertList(IAsyncResult asyncResult);

        //▼====== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardDrugMedDept(InwardDrugMedDept invoicedrug, long StaffID, long? V_MedProductType, AsyncCallback callback, object state);
        int EndUpdateInwardDrugMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrugMedDept(long invoicedrug, long? V_MedProductType, AsyncCallback callback, object state);
        int EndDeleteInwardDrugMedDept(IAsyncResult asyncResult);
        //▲====== #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrugMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardDrugMedDeptTemp(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardDrugMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, bool IsConsignment, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardDrugMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        //▼====== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, long? V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardDrugMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDept_ByIDDrugDeptInIDOrig(long inviID, long? V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardDrugMedDept_ByIDDrugDeptInIDOrig(IAsyncResult asyncResult);
        //▲====== #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardDrugMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(long inviID, long? V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndspGetInBatchNumberAllDrugDept_ByGenMedProductID(IAsyncResult asyncResult);

        //▼====== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDeptInvoice_ByID(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardDrugMedDeptInvoice_ByID(IAsyncResult asyncResult);
        //▲====== #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDeptInvoice_ByID_V2(long ID, long? V_MedProductType, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardDrugMedDeptInvoice_ByID_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, AsyncCallback callback, object state);
        int EndInwardDrugMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID, AsyncCallback callback, object state);
        RefGenMedProductDetails EndGetRefGenMedProductDetails(IAsyncResult asyncResult);

        #endregion

        #region 2. Outward Drug

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndspGetInBatchNumberAndPrice_ListForRequest(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugDeptList_ForDepositGoods(long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetRequestDrugDeptList_ForDepositGoods(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugMedDeptDetailByInvoice(long ID, long V_MedProductType, bool FromClinicDept, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetOutwardDrugMedDeptDetailByInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(long ID, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListDrugExpiryDate_DrugDept(long? StoreID, int Type, long V_MedProductType, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetListDrugExpiryDate_DrugDept(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardClinicDeptDetailByRequestID(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        IList<RequestDrugInwardClinicDeptDetail> EndGetRequestDrugInwardClinicDeptDetailByRequestID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRequestDrugInwardClinicDeptDetailByRequestIDNew(long ReqDrugInClinicDeptID, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetRequestDrugInwardClinicDeptDetailByRequestIDNew(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugMedDeptInvoice(long ID, long V_MedProductType, AsyncCallback callback, object state);
        OutwardDrugMedDeptInvoice EndGetOutwardDrugMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRequireUnlockOutMedDeptInvoice(long outiID, AsyncCallback callback, object state);
        bool EndRequireUnlockOutMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteOutwardDrugMedDeptInvoice(long outiID, long StaffID, long V_MedProductType, AsyncCallback callback, object state);
        bool EndDeleteOutwardDrugMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDept_Delete(long ID, long StaffID, AsyncCallback callback, object state);
        bool EndOutwardDrugMedDept_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        IList<OutwardDrugMedDeptInvoice> EndOutwardDrugMedDeptInvoice_SearchByType(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenMedProductDetailsAuto_ByRequestID(string BrandName, long V_MedProductType, long? RequestID, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetRefGenMedProductDetailsAuto_ByRequestID(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(IAsyncResult asyncResult);

        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(bool IsSearchByGenericName, bool? IsCost, string BrandName, long StoreID
            , long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2
            , long? RequestTemplateID, bool IsGetOnlyRemain, bool? IsCOVID, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IAsyncResult asyncResult);
        //▲====: #005

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetGenMedProductForSellAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long V_MedProductType, long? IssueID, long RefGenDrugCatID_1, AsyncCallback callback, object state);
        IList<GetGenMedProductForSell> EndGetGenMedProductForSellAutoComplete_ForPrescriptionByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspGetInBatchNumberAndPrice_ByPresciption_InPt(long PrescriptID, long StoreID, Int16 IsObject, long V_MedProductType, long RefGenDrugCatID_1, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndspGetInBatchNumberAndPrice_ByPresciption_InPt(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, AsyncCallback callback, object state);
        bool EndSaveOutwardInvoice(out OutwardDrugMedDeptInvoice SavedOutwardInvoice, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, AsyncCallback callback, object state);
        bool EndUpdateOutwardInvoice(out OutwardDrugMedDeptInvoice SavedOutwardInvoice, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<OutwardDrugMedDeptInvoice> EndGetOutWardDrugInvoiceSearchAllByStatus_InPt(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutwardDrugDetailsByOutwardInvoice_InPt(long OutiID, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetOutwardDrugDetailsByOutwardInvoice_InPt(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetGenMedProductForSellAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? IssueID, bool? IsCode, long V_MedProductType, long RefGenDrugCatID_1, AsyncCallback callback, object state);
        IList<GetGenMedProductForSell> EndGetGenMedProductForSellAutoComplete_ForPrescription(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteOutwardInvoice(long outiID, long staffID, AsyncCallback callback, object state);
        bool EndDeleteOutwardInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutWardDrugInvoiceByID_InPt(long outiID, AsyncCallback callback, object state);
        OutwardDrugMedDeptInvoice EndGetOutWardDrugInvoiceByID_InPt(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(long GenMedProductID, long V_MedProductType, long StoreID, bool IsHIPatient, AsyncCallback callback, object state);
        IList<GetGenMedProductForSell> EndGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCreateBillForOutwardFromPrescription(OutwardDrugMedDeptInvoice OutwardInvoice, long StaffID, AsyncCallback callback, object state);
        bool EndCreateBillForOutwardFromPrescription(out long InPatientBillingInvID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteBillForOutwardFromPrescription(long InPatientBillingInvID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteBillForOutwardFromPrescription(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, int PageSize, int PageIndex, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice Invoice, AsyncCallback callback, object state);
        bool EndOutwardDrugMedDeptInvoice_SaveByType(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_Update(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, AsyncCallback callback, object state);
        bool EndOutwardDrugMedDeptInvoice_Update(out long ID, out string StrError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, AsyncCallback callback, object state);
        bool EndOutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(out long ID, out string StrError, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoices_HangKyGoi_Delete(long outiID, AsyncCallback callback, object state);
        bool EndOutwardDrugMedDeptInvoices_HangKyGoi_Delete(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefOutputType_Get(bool? All, AsyncCallback callback, object state);
        List<RefOutputType> EndRefOutputType_Get(IAsyncResult asyncResult);


        #endregion


        #region Outward From Prescription

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchPrescription_InPt(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<Prescription> EndSearchPrescription_InPt(out int totalCount, IAsyncResult asyncResult);


        #endregion


        #region 30.1 DrugDept StockTakes member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTakes_Search(DrugDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, AsyncCallback callback, object state);
        List<DrugDeptStockTakes> EndDrugDeptStockTakes_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate, AsyncCallback callback, object state);
        List<DrugDeptStockTakeDetails> EndDrugDeptStockTakeDetails_Get(IAsyncResult asyncResult);

        //--▼--01-- 11/12/2020 DatTB
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginReGetDrugDeptStockTakeDetails(long StoreID, long V_MedProductType, DateTime StockTakeDate, AsyncCallback callback, object state);
        List<DrugDeptStockTakeDetails> EndReGetDrugDeptStockTakeDetails(IAsyncResult asyncResult);
        //--▲--01-- 11/12/2020 DatTB

        //--▼--01-- 12/01/2021 QTD
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginImportFromExcell_DrugDeptStockTakeDetails(byte[] file, AsyncCallback callback, object state);
        byte[] EndImportFromExcell_DrugDeptStockTakeDetails(out string strError, IAsyncResult asyncResult);
        //--▲--01-- 01/01/2021

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTakeDetails_Load(long DrugDeptStockTakeID, AsyncCallback callback, object state);
        List<DrugDeptStockTakeDetails> EndDrugDeptStockTakeDetails_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetProductForDrugDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugDeptStockTakeDetails> EndGetProductForDrugDeptStockTake(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTake_Save(DrugDeptStockTakes StockTake, bool IsConfirmFinished, AsyncCallback callback, object state);
        bool EndDrugDeptStockTake_Save(out long ID, out string StrError, IAsyncResult asyncResult);

        //--▼--03-- 12/12/2020 DatTB
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTake_Resave(DrugDeptStockTakes StockTake, bool IsConfirmFinished, AsyncCallback callback, object state);
        bool EndDrugDeptStockTake_Resave(out long ID, out string StrError, IAsyncResult asyncResult);
        //--▲--03-- 12/12/2020 DatTB

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginKetChuyenTonKho_DrugDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType, AsyncCallback callback, object state);
        bool EndKetChuyenTonKho_DrugDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTake_Remove(long DrugDeptStockTakeID, long StaffID, AsyncCallback callback, object state);
        bool EndDrugDeptStockTake_Remove(IAsyncResult asyncResult);
        #endregion

        #region 30.2 ClinicDept StockTakes member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptStockTakes_Search(ClinicDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, AsyncCallback callback, object state);
        List<ClinicDeptStockTakes> EndClinicDeptStockTakes_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate, AsyncCallback callback, object state);
        List<ClinicDeptStockTakeDetails> EndClinicDeptStockTakeDetails_Get(IAsyncResult asyncResult);

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="StoreID"></param>
        /// <param name="V_MedProductType"></param>
        /// <param name="StockTakeDate"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginReGetClinicDeptStockTakeDetails(long storeID, long vMedProductType, DateTime stockTakeDate, AsyncCallback callback, object state);
        List<ClinicDeptStockTakeDetails> EndReGetClinicDeptStockTakeDetails(IAsyncResult asyncResult);

        /// <summary>
        /// Get the last clinic dept stock takes
        /// VuTTM
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="vMedProductType"></param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLastClinicDeptStockTakes(long storeId, long vMedProductType, AsyncCallback callback, object state);
        ClinicDeptStockTakes EndGetLastClinicDeptStockTakes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptLockAndUnlockStore(long StoreID, long V_MedProductType, long staffID, bool IsLock, AsyncCallback callback, object state);
        bool EndClinicDeptLockAndUnlockStore(out string msg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginIsLockedClinicWarehouse(long StoreID, long vMedProductType, AsyncCallback callback, object state);
        bool EndIsLockedClinicWarehouse(IAsyncResult asyncResult);

        //--▼--01-- 12/12/2020 DatTB
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock, AsyncCallback callback, object state);
        bool EndDrugDeptLockAndUnlockStore(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLastDrugDeptStockTakes(long storeId, long vMedProductType, AsyncCallback callback, object state);
        DrugDeptStockTakes EndGetLastDrugDeptStockTakes(IAsyncResult asyncResult);
        //--▲--01-- 12/12/2020 DatTB

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptStockTakeDetails_Load(long ClinicDeptStockTakeID, AsyncCallback callback, object state);
        List<ClinicDeptStockTakeDetails> EndClinicDeptStockTakeDetails_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetProductForClinicDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType, AsyncCallback callback, object state);
        IList<ClinicDeptStockTakeDetails> EndGetProductForClinicDeptStockTake(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptStockTake_Save(ClinicDeptStockTakes StockTake, AsyncCallback callback, object state);
        bool EndClinicDeptStockTake_Save(out long ID, out string StrError, IAsyncResult asyncResult);

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="StockTake"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptStockTake_Resave(ClinicDeptStockTakes StockTake, AsyncCallback callback, object state);
        bool EndClinicDeptStockTake_Resave(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginKetChuyenTonKho_ClinicDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType, DateTime CheckPointDate, AsyncCallback callback, object state);
        bool EndKetChuyenTonKho_ClinicDept(IAsyncResult asyncResult);

        #endregion

        #region 32.0 SupplierDrugDeptPaymentReqs Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_Details(InwardInvoiceSearchCriteria criteria, long? V_MedProductType, AsyncCallback callback, object state);
        List<InwardDrugMedDeptInvoice> EndSupplierDrugDeptPaymentReqs_Details(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_Save(SupplierDrugDeptPaymentReqs PaymentReqs, AsyncCallback callback, object state);
        bool EndSupplierDrugDeptPaymentReqs_Save(out SupplierDrugDeptPaymentReqs OutPaymentReqs, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_DetailsByReqID(long DrugDeptSupplierPaymentReqID, AsyncCallback callback, object state);
        List<InwardDrugMedDeptInvoice> EndSupplierDrugDeptPaymentReqs_DetailsByReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_ID(long DrugDeptSupplierPaymentReqID, AsyncCallback callback, object state);
        SupplierDrugDeptPaymentReqs EndSupplierDrugDeptPaymentReqs_ID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_Search(RequestSearchCriteria Criteria, long? V_MedProductType, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierDrugDeptPaymentReqs> EndSupplierDrugDeptPaymentReqs_Search(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID, AsyncCallback callback, object state);
        bool EndSupplierDrugDeptPaymentReqs_UpdateStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierDrugDeptPaymentReqs_Delete(long ID, AsyncCallback callback, object state);
        bool EndSupplierDrugDeptPaymentReqs_Delete(IAsyncResult asyncResult);
        #endregion

        #region 39.CostTableForMedDeptInvoice Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCostTableMedDept_Insert(CostTableMedDept Item, AsyncCallback callback, object state);
        bool EndCostTableMedDept_Insert(out long CoID, out string StrCoNumber, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCostTableMedDept_Search(InwardInvoiceSearchCriteria criteria, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<CostTableMedDept> EndCostTableMedDept_Search(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCostTableMedDeptDetails_ByID(long CoID, AsyncCallback callback, object state);
        CostTableMedDept EndCostTableMedDeptDetails_ByID(IAsyncResult asyncResult);

        //▼====== #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugMedDeptInvoice_GetListCost(long inviID, long? V_MedProductType, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardDrugMedDeptInvoice_GetListCost(IAsyncResult asyncResult);
        #endregion
        //▲====== #004

        #region 40. Return MedDept Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugMedDeptInvoiceReturn_Insert(OutwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        bool EndOutWardDrugMedDeptInvoiceReturn_Insert(out long outwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTransactionPayMedDeptReturn(PatientTransactionPayment payment, long ReturnID, AsyncCallback callback, object state);
        bool EndTransactionPayMedDeptReturn(IAsyncResult asyncResult);

        #endregion

        #region Xáp nhập phiếu nhập tạm
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugMedDeptIsInputTemp_BySupplierID(long SupplierID, long V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndInwardDrugMedDeptIsInputTemp_BySupplierID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrugInvoices_XapNhapInputTemp_Save(long inviIDJoin, IEnumerable<InwardDrugMedDept> ObjInwardDrugMedDeptList, AsyncCallback callback, object state);
        bool EndInwardDrugInvoices_XapNhapInputTemp_Save(out string Result, IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrderDetails_CheckOrder(long GenMedProductID, long V_MedProductType, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<DrugDeptPurchaseCheckOrder> EndDrugDeptPurchaseOrderDetails_CheckOrder(out List<DrugDeptPurchaseCheckOrderInward> InwardList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddTransactionMedDept(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, long? StaffID, AsyncCallback callback, object state);
        bool EndAddTransactionMedDept(out long PaymentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddTransactionMedDeptHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, long? StaffID, AsyncCallback callback, object state);
        bool EndAddTransactionMedDeptHoanTien(out long PaymentID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedDeptInvoice_UpdateInvoicePayed(OutwardDrugMedDeptInvoice Outward, AsyncCallback callback, object state);
        bool EndMedDeptInvoice_UpdateInvoicePayed(out long outiID, out long PaymentID,
                                           out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedDeptInvoice_UpdateInvoiceInfo(OutwardDrugMedDeptInvoice Outward, AsyncCallback callback, object state);
        bool EndMedDeptInvoice_UpdateInvoiceInfo(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInStock_MedDept(string BrandName, long StoreID, bool IsDetail, long V_MedProductType, int PageIndex, int PageSize, long? BidID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndInStock_MedDept(out int TotalCount, out decimal TotalMoney, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugMedDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardDrugMedDeptForAdjustOutPrice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedDeptAdjustOutPrice(ObservableCollection<InwardDrugMedDept> InwardDrugMedDeptList, AsyncCallback callback, object state);
        bool EndMedDeptAdjustOutPrice(IAsyncResult asyncResult);

        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardDrugClinicDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType, AsyncCallback callback, object state);
        IList<InwardDrugClinicDept> EndGetInwardDrugClinicDeptForAdjustOutPrice(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginClinicDeptAdjustOutPrice(IList<InwardDrugClinicDept> InwardDrugMedDeptList, long? StaffID, AsyncCallback callback, object state);
        bool EndClinicDeptAdjustOutPrice(IAsyncResult asyncResult);
        /*▲====: #001*/


        //▼====== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugPharmacyInvoice_Cbx(long? StoreID, AsyncCallback callback, object state);
        List<OutwardDrugInvoice> EndOutwardDrugPharmacyInvoice_Cbx(IAsyncResult asyncResult);
        //▲====== #002

        //▼====== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoice_Cbx_V2(long? StoreID, AsyncCallback callback, object state);
        List<OutwardDrugMedDeptInvoice> EndOutwardDrugMedDeptInvoice_Cbx_V2(IAsyncResult asyncResult);
        //▲====== #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long RequestDrugInwardHiStoreID, long StoreID, long V_MedProductType, AsyncCallback callback, object state);
        IList<OutwardDrugMedDept> EndGetInBatchNumberAndPrice_ListForRequest(IAsyncResult asyncResult);



        #region 41. Inward VTYTTH Med Dept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardVTYTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardVTYTTHMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardVTYTTHMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardVTYTTHMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTYTTHMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardVTYTTHMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTYTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardVTYTTHMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVTYTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardVTYTTHMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTYTTHMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardVTYTTHMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTYTTHMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardVTYTTHMedDeptTemp(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardVTYTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardVTYTTHMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTYTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVTYTTHMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginspGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        //IList<RefGenMedProductDetails> EndspGetInBatchNumberAllDrugDept_ByGenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTYTTHMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardVTYTTHMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTYTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardVTYTTHMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTYTTHMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardVTYTTHMedDeptInvoice_GetListCost(IAsyncResult asyncResult);
        #endregion

        #region 42. Inward TiemNgua Med Dept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardTiemNguaMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardTiemNguaMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardTiemNguaMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardTiemNguaMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardTiemNguaMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardTiemNguaMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardTiemNguaMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardTiemNguaMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardTiemNguaMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardTiemNguaMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardTiemNguaMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardTiemNguaMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardTiemNguaMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardTiemNguaMedDeptTemp(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardTiemNguaMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardTiemNguaMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardTiemNguaMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardTiemNguaMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginspGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        //IList<RefGenMedProductDetails> EndspGetInBatchNumberAllDrugDept_ByGenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardTiemNguaMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardTiemNguaMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardTiemNguaMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardTiemNguaMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardTiemNguaMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardTiemNguaMedDeptInvoice_GetListCost(IAsyncResult asyncResult);
        #endregion

        #region 44. Inward Chemical Med Dept

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardChemicalMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardChemicalMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardChemicalMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardChemicalMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardChemicalMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardChemicalMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardChemicalMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardChemicalMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardChemicalMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardChemicalMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardChemicalMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardChemicalMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardChemicalMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardChemicalMedDeptTemp(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardChemicalMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardChemicalMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardChemicalMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardChemicalMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardChemicalMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardChemicalMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginspGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient, AsyncCallback callback, object state);
        //IList<RefGenMedProductDetails> EndspGetInBatchNumberAllDrugDept_ByGenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardChemicalMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardChemicalMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardChemicalMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardChemicalMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardChemicalMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardChemicalMedDeptInvoice_GetListCost(IAsyncResult asyncResult);
        #endregion

        #region Kho máu

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardBloodMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardBloodMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardBloodMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardBloodMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardBloodMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardBloodMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardBloodMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardBloodMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardBloodMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardBloodMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardBloodMedDeptInvoice_ByID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardBloodMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardBloodMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardBloodMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardBloodMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardBloodMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardBloodMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardBloodMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardBloodMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardBloodMedDeptInvoice_GetListCost(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardBloodMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardBloodMedDept(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardBloodMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardBloodMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardBloodMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardBloodMedDeptTemp(IAsyncResult asyncResult);
        #endregion

        #region Kho Thanh Trùng

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardThanhTrungMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardThanhTrungMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardThanhTrungMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardThanhTrungMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardThanhTrungMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardThanhTrungMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardThanhTrungMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardThanhTrungMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardThanhTrungMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardThanhTrungMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardThanhTrungMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardThanhTrungMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardThanhTrungMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardThanhTrungMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardThanhTrungMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardThanhTrungMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardThanhTrungMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardThanhTrungMedDeptInvoice_GetListCost(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardThanhTrungMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardThanhTrungMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardThanhTrungMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardThanhTrungMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardThanhTrungMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardThanhTrungMedDeptTemp(IAsyncResult asyncResult);

        #endregion

        #region Kho Văn phòng phẩm

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardVPPMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardVPPMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVPPMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardVPPMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardVPPMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVPPMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVPPMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVPPMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVPPMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVPPMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardVPPMedDeptInvoice_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardVPPMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVPPMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardVPPMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVPPMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardVPPMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardVPPMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardVPPMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVPPMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardVPPMedDeptInvoice_GetListCost(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVPPMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardVPPMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVPPMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardVPPMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVPPMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardVPPMedDeptTemp(IAsyncResult asyncResult);

        #endregion

        #region Kho Vật tư

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardVTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, AsyncCallback callback, object state);
        List<OutwardDrugClinicDeptInvoice> EndOutwardVTTHMedDeptInvoice_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTTHMedDeptInvoice(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardVTTHMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndUpdateInwardVTTHMedDeptInvoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVTTHMedDept_ByIDInvoice(out int totalCount, out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTTHMedDept_ByIDInvoiceNotPaging(long inviID, AsyncCallback callback, object state);
        IList<InwardDrugMedDept> EndGetInwardVTTHMedDept_ByIDInvoiceNotPaging(out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInwardVTTHMedDeptInvoice_ByID(long ID, AsyncCallback callback, object state);
        InwardDrugMedDeptInvoice EndGetInwardVTTHMedDeptInvoice_ByID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndAddInwardVTTHMedDeptInvoice(out long inwardid, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, AsyncCallback callback, object state);
        bool EndInwardVTTHMedDept_InsertList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTTHMedDept(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardVTTHMedDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchInwardVTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<InwardDrugMedDeptInvoice> EndSearchInwardVTTHMedDeptInvoice(out int totalCount, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTTHMedDeptInvoice_GetListCost(long inviID, AsyncCallback callback, object state);
        IList<CostTableMedDeptList> EndInwardVTTHMedDeptInvoice_GetListCost(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInwardVTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndUpdateInwardVTTHMedDept(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardVTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndInwardVTTHMedDeptInvoice_SaveXML(out long id, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardVTTHMedDeptTemp(long InID, AsyncCallback callback, object state);
        bool EndDeleteInwardVTTHMedDeptTemp(IAsyncResult asyncResult);
        #endregion

        #region Báo giá
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetGenMedProductAndPrice(bool IsCode, string BrandName, long V_MedProductType, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndGetGenMedProductAndPrice(IAsyncResult asyncResult);
        #endregion

        #region Cân bằng
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugMedDeptInvoice_SaveByType_Balance(OutwardDrugMedDeptInvoice Invoice, int ViewCase, AsyncCallback callback, object state);
        bool EndDrugMedDeptInvoice_SaveByType_Balance(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForBalanceCompleteFromCategory(bool IsSearchByGenericName, bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2,
                    long? RequestTemplateID, bool IsGetOnlyRemain, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForBalanceCompleteFromCategory(IAsyncResult asyncResult);
        #endregion

        #region
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptStockTake_SaveNew(DrugDeptStockTakes StockTake, bool IsConfirmFinished, bool IsAlreadyRefresh, AsyncCallback callback, object state);
        bool EndDrugDeptStockTake_SaveNew(out long ID, out string StrError, IAsyncResult asyncResult);
        #endregion

        #region Hảng ký gửi
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugMedDeptInvoiceConsignment_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic, AsyncCallback callback, object state);
        List<OutwardDrugMedDeptInvoice> EndOutwardDrugMedDeptInvoiceConsignment_Cbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateReceipt(InwardDrugMedDeptInvoice InvoiceDrug, AsyncCallback callback, object state);
        bool EndUpdateReceipt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_ForConsignment(bool IsSearchByGenericName, bool? IsCost, string BrandName, long StoreID
            , long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2
            , long? RequestTemplateID, bool IsGetOnlyRemain, bool? IsCOVID, bool? IsReturn, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_ForConsignment(IAsyncResult asyncResult);
        #endregion

        #region Đơn thuốc điện tử
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRegistrationsForElectronicPrescription(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase, int PageIndex
            , int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationsForElectronicPrescription(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPreviewElectronicPrescription(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, AsyncCallback callback, object state);
        IList<DTDT_don_thuoc> EndPreviewElectronicPrescription(out string ErrText, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPreviewElectronicPrescription_Cancel(long PtRegistrationID, long IssueID, long V_RegistrationType, long? OutPtTreatmentProgramID, AsyncCallback callback, object state);
        IList<DTDT_don_thuoc> EndPreviewElectronicPrescription_Cancel(out string ErrText, IAsyncResult asyncResult);

        //▼==== #007
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRegistrationsForElectronicPrescriptionPharmacy(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, bool IsTotalTab, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationsForElectronicPrescriptionPharmacy(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPreviewElectronicPrescriptionPharmacy(long PtRegistrationID, long V_RegistrationType, long IssueID, long? OutPtTreatmentProgramID, AsyncCallback callback, object state);
        IList<DQG_don_thuoc> EndPreviewElectronicPrescriptionPharmacy(out string ErrText, IAsyncResult asyncResult);
        #endregion
        //▲==== #007
    }
}
