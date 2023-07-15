using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataEntities;
using ErrorLibrary;

namespace PharmacyService
{
    [ServiceContract]
    public interface IEstimateDrugDeptService
    {
        #region 0. Estimation For Drug Dept Member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugDeptEstimationForPoDetail> GetEstimationForMonth(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugDeptEstimationForPoDetail> GetEstimationForMonthByBid(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail_GenMedProductID(long GenMedProductID, string Code, DateTime EstimateDate, long? V_EstimateType, long V_MedProductType, long? RefGenDrugCatID_1);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long DrugDeptEstimationForPO_FullOperator(long V_MedProductType, DrugDeptEstimationForPO Estimate, out DrugDeptEstimationForPO EstimateOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long DrugDeptEstimationForPO_FullOperator_ByBid(long V_MedProductType, DrugDeptEstimationForPO Estimate, out DrugDeptEstimationForPO EstimateOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptEstimationForPO DrugDeptEstimationForPO_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_ByParentID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_ByParentIDAndByBid(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_SearchByBid(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Load(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, long? RefGenDrugCatID_1, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptEstimationForPO_Delete(long DrugDeptEstimatePoID, bool IsFromRequest);
        #endregion 

        #region 1. Estimation For Pharmacy Member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacyEstimationForPODetail> GetEstimationForMonthPharmacy(long V_MedProductType, DateTime EstimateDate, long? V_EstimateType, bool IsHIStorage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long PharmacyEstimationForPO_FullOperator(long V_MedProductType, PharmacyEstimationForPO Estimate, bool IsHIStorage, out PharmacyEstimationForPO EstimateOut);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, bool IsHIStorage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PharmacyEstimationForPO PharmacyEstimationForPO_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_ByParentID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyEstimationForPO> PharmacyEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, bool IsHIStorage, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyEstimationForPO> PharmacyEstimationForPO_Load(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PharmacyEstimationForPODetail PharmacyEstimationForPODetail_ByDrugID(DateTime EstimationDate, long DrugID, string DrugCode, long? V_EstimateType, bool IsHIStorage);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyEstimationForPO_Delete(long PharmacyEstimatePoID);
        #endregion 

        #region 2. Purchase Order For Drug

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_GetFirst(long? PharmacyEstimatePoID, long? SupplierID, long? PCOID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyPurchaseOrderDetail_UpdateNoWaiting(long PharmacyPoDetailID, bool? NoWaiting);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long PharmacyPurchaseOrder_FullOperator(PharmacyPurchaseOrder PurchaseOrder, byte IsInput, out PharmacyPurchaseOrder OutPurchaseOrder);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PharmacyPurchaseOrder PharmacyPurchaseOrder_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_ByParentID(long ID, byte IsInput);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_Search(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_BySupplierID(long SupplierID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyPurchaseOrders_Delete(long PharmacyPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenericDrugDetail> RefGenericDrugDetail_AutoRequest(string BrandName, long? PharmacyEstimatePoID, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenericDrugDetail> RefGenericDrugDetail_WarningOrder(int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount);

        #endregion

        #region 3.DrugDept Purchase Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_BySupplierID(long SupplierID, long V_MedProductType, long BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_ByParentID(long ID, byte IsInput, long? BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_GetFirst(long? DrugDeptEstimatePoID, long? SupplierID, long? PCOID, long V_MedProductType, long? BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long DrugDeptPurchaseOrder_FullOperator(DrugDeptPurchaseOrder PurchaseOrder, byte IsInput, long? BidID, out DrugDeptPurchaseOrder OutPurchaseOrder);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptPurchaseOrder DrugDeptPurchaseOrder_ByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_AutoPurchaseOrder(string BrandName, long? DrugDeptEstimatePoID, long V_MedProductType, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode, long? BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_WarningOrder(long V_MedProductType, int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptPurchaseOrders_Delete(long DrugDeptPoID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptPurchaseOrderDetail_UpdateNoWaiting(long DrugDeptPoDetailID, bool? NoWaiting);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptPurchaseOrders_UpdateStatus(long ID, long V_PurchaseOrderStatus);
        #endregion

        //Chỉnh sửa chức năng Dự trù 20210830
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugDeptEstimationForPoDetail> GetEstimation_V2(long V_MedProductType, DateTime FromDate, DateTime ToDate, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO, bool IsByBid = false, bool IsFromRequest = false);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail_GenMedProductID_V2(long GenMedProductID, string Code, DateTime FromDate, DateTime ToDate, long V_MedProductType, long? RefGenDrugCatID_1);
    }
}