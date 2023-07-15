using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataEntities;

namespace EstimateDrugDeptProxy
{
    [ServiceContract]
    public interface IEstimateDrugDeptService
    {
        #region 0. Estimation For Drug Dept Member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEstimationForMonth(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO, AsyncCallback callback, object state);
        IList<DrugDeptEstimationForPoDetail> EndGetEstimationForMonth(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEstimationForMonthByBid(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO, AsyncCallback callback, object state);
        IList<DrugDeptEstimationForPoDetail> EndGetEstimationForMonthByBid(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPoDetail_GenMedProductID(long GenMedProductID, string Code, DateTime EstimateDate, long? V_EstimateType, long V_MedProductType, long? RefGenDrugCatID_1, AsyncCallback callback, object state);
        DrugDeptEstimationForPoDetail EndDrugDeptEstimationForPoDetail_GenMedProductID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_FullOperator(long V_MedProductType, DrugDeptEstimationForPO Estimate, AsyncCallback callback, object state);
        long EndDrugDeptEstimationForPO_FullOperator(out DrugDeptEstimationForPO EstimateOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_FullOperator_ByBid(long V_MedProductType, DrugDeptEstimationForPO Estimate, AsyncCallback callback, object state);
        long EndDrugDeptEstimationForPO_FullOperator_ByBid(out DrugDeptEstimationForPO EstimateOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_ByID(long ID, AsyncCallback callback, object state);
        DrugDeptEstimationForPO EndDrugDeptEstimationForPO_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, long V_MedProductType, AsyncCallback callback, object state);
        bool EndDrugDeptEstimationForPO_CheckExists(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPoDetail_ByParentID(long ID, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPoDetail> EndDrugDeptEstimationForPoDetail_ByParentID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPoDetail_ByParentIDAndByBid(long ID, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPoDetail> EndDrugDeptEstimationForPoDetail_ByParentIDAndByBid(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPO> EndDrugDeptEstimationForPO_Search(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_SearchByBid(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPO> EndDrugDeptEstimationForPO_SearchByBid(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_Load(long V_MedProductType, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPO> EndDrugDeptEstimationForPO_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPoDetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, long? RefGenDrugCatID_1, string BrandName, int pageIndex, int pageSize, bool? IsCode, AsyncCallback callback, object state);
        List<DrugDeptEstimationForPoDetail> EndDrugDeptEstimationForPoDetail_AllDrugAuto(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPO_Delete(long DrugDeptEstimatePoID, bool IsFromRequest, AsyncCallback callback, object state);
        bool EndDrugDeptEstimationForPO_Delete(IAsyncResult asyncResult);
        #endregion

        #region 1. Estimation For Pharmacy Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEstimationForMonthPharmacy(long V_MedProductType, DateTime EstimateDate, long? V_EstimateType, bool IsHIStorage, AsyncCallback callback, object state);
        IList<PharmacyEstimationForPODetail> EndGetEstimationForMonthPharmacy(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_FullOperator(long V_MedProductType, PharmacyEstimationForPO Estimate, bool IsHIStorage, AsyncCallback callback, object state);
        long EndPharmacyEstimationForPO_FullOperator(out PharmacyEstimationForPO EstimateOut, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, bool IsHIStorage, AsyncCallback callback, object state);
        bool EndPharmacyEstimationForPO_CheckExists(IAsyncResult asyncResult);



        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_ByID(long ID, AsyncCallback callback, object state);
        PharmacyEstimationForPO EndPharmacyEstimationForPO_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPODetail_ByParentID(long ID, AsyncCallback callback, object state);
        List<PharmacyEstimationForPODetail> EndPharmacyEstimationForPODetail_ByParentID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, bool IsHIStorage, AsyncCallback callback, object state);
        List<PharmacyEstimationForPO> EndPharmacyEstimationForPO_Search(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_Load(long V_MedProductType, AsyncCallback callback, object state);
        List<PharmacyEstimationForPO> EndPharmacyEstimationForPO_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPODetail_ByDrugID(DateTime EstimationDate, long DrugID, string DrugCode, long? V_EstimateType, bool IsHIStorage, AsyncCallback callback, object state);
        PharmacyEstimationForPODetail EndPharmacyEstimationForPODetail_ByDrugID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPODetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, string BrandName, int pageIndex, int pageSize, bool? IsCode, AsyncCallback callback, object state);
        List<PharmacyEstimationForPODetail> EndPharmacyEstimationForPODetail_AllDrugAuto(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyEstimationForPO_Delete(long PharmacyEstimatePoID, AsyncCallback callback, object state);
        bool EndPharmacyEstimationForPO_Delete(IAsyncResult asyncResult);
        #endregion

        #region 2. Purchase Order For Drug

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrderDetail_GetFirst(long? PharmacyEstimatePoID, long? SupplierID, long? PCOID, AsyncCallback callback, object state);
        List<PharmacyPurchaseOrderDetail> EndPharmacyPurchaseOrderDetail_GetFirst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrderDetail_UpdateNoWaiting(long PharmacyPoDetailID, bool? NoWaiting, AsyncCallback callback, object state);
        bool EndPharmacyPurchaseOrderDetail_UpdateNoWaiting(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrder_FullOperator(PharmacyPurchaseOrder PurchaseOrder, byte IsInput, AsyncCallback callback, object state);
        long EndPharmacyPurchaseOrder_FullOperator(out PharmacyPurchaseOrder OutPurchaseOrder, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrder_ByID(long ID, AsyncCallback callback, object state);
        PharmacyPurchaseOrder EndPharmacyPurchaseOrder_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus, AsyncCallback callback, object state);
        bool EndPharmacyPurchaseOrder_UpdateStatus(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrderDetail_ByParentID(long ID, byte IsInput, AsyncCallback callback, object state);
        List<PharmacyPurchaseOrderDetail> EndPharmacyPurchaseOrderDetail_ByParentID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrder_Search(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<PharmacyPurchaseOrder> EndPharmacyPurchaseOrder_Search(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrder_BySupplierID(long SupplierID, AsyncCallback callback, object state);
        List<PharmacyPurchaseOrder> EndPharmacyPurchaseOrder_BySupplierID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyPurchaseOrders_Delete(long PharmacyPoID, AsyncCallback callback, object state);
        bool EndPharmacyPurchaseOrders_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenericDrugDetail_AutoRequest(string BrandName, long? PharmacyEstimatePoID, long? SupplierID, int PageIndex, int PageSize, bool? IsCode, AsyncCallback callback, object state);
        List<RefGenericDrugDetail> EndRefGenericDrugDetail_AutoRequest(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenericDrugDetail_WarningOrder(int PageIndex, int PageSize, long? SupplierID, bool IsAll, AsyncCallback callback, object state);
        List<RefGenericDrugDetail> EndRefGenericDrugDetail_WarningOrder(out int totalcount, IAsyncResult asyncResult);

        #endregion

        #region 3.DrugDept Purchase Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrder_BySupplierID(long SupplierID, long V_MedProductType, long BidID, AsyncCallback callback, object state);
        List<DrugDeptPurchaseOrder> EndDrugDeptPurchaseOrder_BySupplierID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrderDetail_ByParentID(long ID, byte IsInput, long? BidID, AsyncCallback callback, object state);
        List<DrugDeptPurchaseOrderDetail> EndDrugDeptPurchaseOrderDetail_ByParentID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrder_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, AsyncCallback callback, object state);
        List<DrugDeptPurchaseOrder> EndDrugDeptPurchaseOrder_Search(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrderDetail_GetFirst(long? DrugDeptEstimatePoID, long? SupplierID, long? PCOID, long V_MedProductType, long? BidID, AsyncCallback callback, object state);
        List<DrugDeptPurchaseOrderDetail> EndDrugDeptPurchaseOrderDetail_GetFirst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrder_FullOperator(DrugDeptPurchaseOrder PurchaseOrder, byte IsInput, long? BidID, AsyncCallback callback, object state);
        long EndDrugDeptPurchaseOrder_FullOperator(out DrugDeptPurchaseOrder OutPurchaseOrder, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrder_ByID(long ID, AsyncCallback callback, object state);
        DrugDeptPurchaseOrder EndDrugDeptPurchaseOrder_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_AutoPurchaseOrder(string BrandName, long? DrugDeptEstimatePoID, long V_MedProductType, long? SupplierID, int PageIndex, int PageSize, bool? IsCode, long? BidID, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_AutoPurchaseOrder(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_WarningOrder(long V_MedProductType, int PageIndex, int PageSize, long? SupplierID, bool IsAll, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_WarningOrder(out int totalcount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrders_Delete(long DrugDeptPoID, AsyncCallback callback, object state);
        bool EndDrugDeptPurchaseOrders_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrderDetail_UpdateNoWaiting(long DrugDeptPoDetailID, bool? NoWaiting, AsyncCallback callback, object state);
        bool EndDrugDeptPurchaseOrderDetail_UpdateNoWaiting(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPurchaseOrders_UpdateStatus(long ID, long V_PurchaseOrderStatus, AsyncCallback callback, object state);
        bool EndDrugDeptPurchaseOrders_UpdateStatus(IAsyncResult asyncResult);
        #endregion

        //Chỉnh sửa chức năng Dự trù 20210830
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetEstimation_V2(long V_MedProductType, DateTime? FromDate, DateTime? ToDate, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO, bool? IsByBid, bool? IsFromRequest, AsyncCallback callback, object state);
        IList<DrugDeptEstimationForPoDetail> EndGetEstimation_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptEstimationForPoDetail_GenMedProductID_V2(long GenMedProductID, string Code, DateTime FromDate, DateTime ToDate, long V_MedProductType, long? RefGenDrugCatID_1, AsyncCallback callback, object state);
        DrugDeptEstimationForPoDetail EndDrugDeptEstimationForPoDetail_GenMedProductID_V2(IAsyncResult asyncResult);
    }
}