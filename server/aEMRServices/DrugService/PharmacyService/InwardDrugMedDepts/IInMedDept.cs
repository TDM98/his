/*
 * 20170821 #001 CMN: Added AdjustClinicPrice Service
 * 20181124 #002 TTM: BM 0005309: Load dữ liệu cbb phiếu xuất kho BHYT màn hình Nhập trả khoa dược
 * 20200807 #003 TNHX: Thêm loai để nhập hóa chất có thầu
 * 20220106 #004 TNHX: 887 Thêm điều kiện tìm kiếm thuốc/ vật tư trong danh mục COVID
 * 20221114 #005 QTD:  Thêm điều kiện xuất trả kho ký gửi
 * 20230211 #006 QTD:  Thêm DTDT Nhà thuốc
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using System.Xml.Linq;
using ErrorLibrary;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;

namespace PharmacyService
{
    [ServiceContract]
    public interface IInMedDept
    {
        #region 1. Inward Drug

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, bool IsNotCheckInvalid, out long inwardid);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrugMedDeptInvoice(long ID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrugMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardDrugMedDept(InwardDrugMedDept invoicedrug, long StaffID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrugMedDept(long invoicedrug, long? V_MedProductType);
        //▲===== #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardDrugMedDeptTemp(long InID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardDrugMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, bool IsConsignment, out int totalCount);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount
            , long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDDrugDeptInIDOrig(long inviID, long? V_MedProductType);
        //▲===== #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID(long ID, long? V_MedProductType);
        //▲===== #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID_V2(long ID, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrugMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, DateTime? DSPTModifiedDate_Outward, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefGenMedProductDetails GetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID);
        #endregion

        #region 2. Outward Drug

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetRequestDrugDeptList_ForDepositGoods(long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice(long ID, long V_MedProductType, bool FromClinicDept);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(long ID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetListDrugExpiryDate_DrugDept(long? StoreID, int Type, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByRequestID(long ReqDrugInClinicDeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetRequestDrugInwardClinicDeptDetailByRequestIDNew(long ReqDrugInClinicDeptID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugMedDeptInvoice GetOutwardDrugMedDeptInvoice(long ID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteOutwardDrugMedDeptInvoice(long outiID, long StaffID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugMedDept_Delete(long id, long StaffID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetRefGenMedProductDetailsAuto_ByRequestID(string BrandName, long V_MedProductType, long? RequestID, int pageIndex, int pageSize, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(bool IsSearchByGenericName, bool? IsCost
            , string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID
            , bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2, long? RequestTemplateID, bool IsGetOnlyRemain
            //▼====: #004
            , bool IsCOVID
            //▲====: #004
        );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long V_MedProductType, long? IssueID, long RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ByPresciption_InPt(long PrescriptID, long StoreID, Int16 IsObject, long V_MedProductType, long RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, out OutwardDrugMedDeptInvoice SavedOutwardInvoice);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateOutwardInvoice(OutwardDrugMedDeptInvoice OutwardInvoice, out OutwardDrugMedDeptInvoice SavedOutwardInvoice);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDeptInvoice> GetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetOutwardDrugDetailsByOutwardInvoice_InPt(long OutiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? IssueID, bool? IsCode, long V_MedProductType, long RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteOutwardInvoice(long outiID, long staffID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        OutwardDrugMedDeptInvoice GetOutWardDrugInvoiceByID_InPt(long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetGenMedProductForSell> GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CreateBillForOutwardFromPrescription(OutwardDrugMedDeptInvoice OutwardInvoice, long StaffID, out long InPatientBillingInvID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteBillForOutwardFromPrescription(long InPatientBillingInvID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1,
                                                                                                        long? RequestID, bool? IsCode, int PageSize, int PageIndex, out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugMedDeptInvoice_Update(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RequireUnlockOutMedDeptInvoice(long outiID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugMedDeptInvoices_HangKyGoi_Delete(long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefOutputType> RefOutputType_Get(bool? All);
        #endregion

        #region Outward From Prescription

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Prescription> SearchPrescription_InPt(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        #endregion


        #region 30.1 DrugDept StockTakes member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptStockTakes> DrugDeptStockTakes_Search(DrugDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate);

        //--▼--02-- 11/12/2020 DatTB
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptStockTakeDetails> ReGetDrugDeptStockTakeDetails(long StoreID, long V_MedProductType, DateTime StockTakeDate);
        //--▲--02-- 11/12/2020 DatTB

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Load(long DrugDeptStockTakeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugDeptStockTakeDetails> GetProductForDrugDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptStockTake_Save(DrugDeptStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError);

        //--▼--01-- 11/12/2020 DatTB 
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptStockTake_Resave(DrugDeptStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError);
        //--▲--01-- 11/12/2020 DatTB 

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptStockTake_Remove(long DrugDeptStockTakeID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool KetChuyenTonKho_DrugDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType);

        #endregion 

        #region 30.2 ClinicDept StockTakes member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ClinicDeptStockTakes> ClinicDeptStockTakes_Search(ClinicDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate);

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="storeID"></param>
        /// <param name="vMedProductType"></param>
        /// <param name="stockTakeDate"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ClinicDeptStockTakeDetails> ReGetClinicDeptStockTakeDetails(long storeID, long vMedProductType, DateTime stockTakeDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ClinicDeptLockAndUnlockStore(long StoreID, long V_MedProductType, long staffID, bool IsLock, out string msg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool IsLockedClinicWarehouse(long StoreID, long vMedProductType);

        /// <summary>
        /// Get the last clinic dept stock takes
        /// VuTTM
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="vMedProductType"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(AxException))]
        ClinicDeptStockTakes GetLastClinicDeptStockTakes(long storeId, long vMedProductType);

        //--▼--01-- 12/12/2020 DatTB
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptStockTakes GetLastDrugDeptStockTakes(long storeId, long vMedProductType);
        //--▲--01-- 12/12/2020 DatTB

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Load(long ClinicDeptStockTakeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ClinicDeptStockTakeDetails> GetProductForClinicDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ClinicDeptStockTake_Save(ClinicDeptStockTakes StockTake, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ClinicDeptStockTake_Resave(ClinicDeptStockTakes StockTake, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool KetChuyenTonKho_ClinicDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType, DateTime CheckPointDate);

        #endregion 

        #region 32.0 SupplierDrugDeptPaymentReqs Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_Details(InwardInvoiceSearchCriteria criteria, long? V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierDrugDeptPaymentReqs_Save(SupplierDrugDeptPaymentReqs PaymentReqs, out SupplierDrugDeptPaymentReqs OutPaymentReqs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_DetailsByReqID(long DrugDeptSupplierPaymentReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SupplierDrugDeptPaymentReqs SupplierDrugDeptPaymentReqs_ID(long DrugDeptSupplierPaymentReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierDrugDeptPaymentReqs> SupplierDrugDeptPaymentReqs_Search(RequestSearchCriteria Criteria, long? V_MedProductType, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierDrugDeptPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierDrugDeptPaymentReqs_Delete(long ID);
        #endregion

        #region 39.CostTableForMedDeptInvoice Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CostTableMedDept_Insert(CostTableMedDept Item, out long CoID, out string StrCoNumber);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDept> CostTableMedDept_Search(InwardInvoiceSearchCriteria criteria, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        CostTableMedDept CostTableMedDeptDetails_ByID(long CoID);

        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardDrugMedDeptInvoice_GetListCost(long inviID, long? V_MedProductType);
        #endregion
        //▲===== #003

        #region 40. Return MedDept Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutWardDrugMedDeptInvoiceReturn_Insert(OutwardDrugMedDeptInvoice InvoiceDrug, out long outwardid);

        #endregion 

        #region Xáp nhập phiếu nhập tạm
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> InwardDrugMedDeptIsInputTemp_BySupplierID(long SupplierID, long V_MedProductType);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardDrugInvoices_XapNhapInputTemp_Save(long inviIDJoin, IEnumerable<InwardDrugMedDept> ObjInwardDrugMedDeptList, out string Result);

        #endregion  

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPurchaseCheckOrder> DrugDeptPurchaseOrderDetails_CheckOrder(long GenMedProductID, long V_MedProductType, DateTime FromDate, DateTime ToDate, out List<DrugDeptPurchaseCheckOrderInward> InwardList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionMedDept(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID, long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddTransactionMedDeptHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID,
                                                 long? StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MedDeptInvoice_UpdateInvoicePayed(OutwardDrugMedDeptInvoice Outward, out long outiID, out long PaymentID,
                                           out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MedDeptInvoice_UpdateInvoiceInfo(OutwardDrugMedDeptInvoice Outward);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> InStock_MedDept(string BrandName, long StoreID, bool IsDetail, long V_MedProductType, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney, long? BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardDrugMedDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MedDeptAdjustOutPrice(ObservableCollection<InwardDrugMedDept> InwardDrugMedDeptList);
        /*▼====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugClinicDept> GetInwardDrugClinicDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ClinicDeptAdjustOutPrice(IList<InwardDrugClinicDept> InwardDrugMedDeptList, long? StaffID);
        /*▲====: #001*/

        //▼====== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugInvoice> OutwardDrugPharmacyInvoice_Cbx(long? StoreID);
        //▲====== #002

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx_V2(long? StoreID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<OutwardDrugMedDept> GetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long RequestDrugInwardHiStoreID, long StoreID, long V_MedProductType);

        #region 2. Inward VTYT Tieu Hao
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardVTYTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVTYTTHMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardVTYTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVTYTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVTYTTHMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardVTYTTHMedDeptTemp(long InID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<OutwardDrugClinicDeptInvoice> OutwardVTYTTHClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardVTYTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardVTYTTHMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardVTYTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardVTYTTHMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 3. Inward TiemNgua
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardTiemNguaMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardTiemNguaMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardTiemNguaMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardTiemNguaMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardTiemNguaMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardTiemNguaMedDeptTemp(long InID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<OutwardDrugClinicDeptInvoice> OutwardVTYTTHClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardTiemNguaMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardTiemNguaMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardTiemNguaMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardTiemNguaMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 4. Inward Chemical
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardChemicalMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardChemicalMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardChemicalMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardChemicalMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardChemicalMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardChemicalMedDeptTemp(long InID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<OutwardDrugClinicDeptInvoice> OutwardChemicalClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardChemicalMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardChemicalMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardChemicalMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardChemicalMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 5. Inward Blood
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardBloodMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardBloodMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardBloodMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardBloodMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardBloodMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardBloodMedDeptTemp(long InID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardBloodMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardBloodMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardBloodMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardBloodMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 6. Inward ThanhTrung
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardThanhTrungMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardThanhTrungMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardThanhTrungMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardThanhTrungMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardThanhTrungMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardThanhTrungMedDeptTemp(long InID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardThanhTrungMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardThanhTrungMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardThanhTrungMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardThanhTrungMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 7. Inward VPP
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardVPPMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVPPMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardVPPMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVPPMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVPPMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardVPPMedDeptTemp(long InID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardVPPMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardVPPMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardVPPMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardVPPMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 8. Inward VTTH
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardVTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVTTHMedDeptInvoice(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InwardVTTHMedDept_InsertList(List<DrugDeptPurchaseOrderDetail> OrderDetails, long inviID, long? DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateInwardVTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardVTTHMedDept(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardVTTHMedDeptTemp(long InID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDeptInvoice> SearchInwardVTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDrugMedDeptInvoice GetInwardVTTHMedDeptInvoice_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardVTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<CostTableMedDeptList> InwardVTTHMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> GetGenMedProductAndPrice(bool IsCode, string BrandName, long V_MedProductType);
        #endregion

        #region Cân bằng
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugMedDeptInvoice_SaveByType_Balance(OutwardDrugMedDeptInvoice Invoice, int ViewCase, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForBalanceCompleteFromCategory(bool IsSearchByGenericName, bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2,
            long? RequestTemplateID,
            bool IsGetOnlyRemain);
        #endregion

        #region
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptStockTake_SaveNew(DrugDeptStockTakes StockTake, bool IsConfirmFinished, bool IsAlreadyRefresh, out long ID, out string StrError);
        #endregion

        #region Hàng ký gửi
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoiceConsignment_Cbx(long? StoreID, long V_MedProductType, bool IsFromClinic);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateReceipt(InwardDrugMedDeptInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_ForConsignment(bool IsSearchByGenericName, bool? IsCost
           , string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID
           , bool? IsCode, long? OutputToStoreID, long? RefGenDrugCatID_2, long? RequestTemplateID, bool IsGetOnlyRemain
           //▼====: #004
           , bool IsCOVID
           //▲====: #004
           //▼====: #005
           , bool IsReturn
           //▲====: #005
         );
        #endregion

        #region Đơn thuốc điện tử
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsForElectronicPrescription(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DTDT_don_thuoc> PreviewElectronicPrescription(long PtRegistrationID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DTDT_don_thuoc> PreviewElectronicPrescription_Cancel(long PtRegistrationID, long IssueID, long V_RegistrationType, long? OutPtTreatmentProgramID, out string ErrText);
        #endregion

        //▼====== #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsForElectronicPrescriptionPharmacy(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, bool IsTotalTab, int pageIndex, int pageSize, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DQG_don_thuoc> PreviewElectronicPrescriptionPharmacy(long PtRegistrationID, long V_RegistrationType, long IssueID, long? OutPtTreatmentProgramID, out string ErrText);
        //▲====== #006
    }
}
