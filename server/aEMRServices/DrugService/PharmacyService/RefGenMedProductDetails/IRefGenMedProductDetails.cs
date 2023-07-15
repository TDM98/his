/*
 * 20170810 #001 CMN: Added Bid Service
 * 20180801 #002 TTM: Add void RefGenMedProductDetails_Save_New
 * 20210122 #003 TNHX: Tách danh sách thêm/xóa/sửa + ghi log user cập nhật + thêm Mã gói thầu
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;

namespace PharmacyService
{
    [ServiceContract]
    public interface IRefGenMedProductDetails
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> GetRefGenMedProductDetails_GetALL(long V_MedProductType);

        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_Paging(
            RefGenMedProductDetailsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrugs_Paging(long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefGenMedProductDetails_Save(DataEntities.RefGenMedProductDetails Obj, out string Res, out long GenMedProductID, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType);

        //▼===== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefGenMedProductDetails_Save_New(DataEntities.RefGenMedProductDetails Obj, out string Res, out long GenMedProductID, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType);
        //▲===== #002

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefGenMedProductDetails_MarkDelete(Int64 GenMedProductID, out string Res);
        
        //Properties Navigate
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugClass> DrugClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugClass> DrugDeptClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount); 

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefCountry> RefCountries_SearchPaging(RefCountrySearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);        

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefUnit> RefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefUnit> DrugDeptRefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? BidID, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_V2(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? RefGenDrugCatID_2, long? BidID
            , bool IsGetDrugDeptItemsOnly
            , out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging(bool? IsCode,
                                                                                              string BrandName,
                                                                                              long V_MedProductType,
                                                                                              long? RefGenDrugCatID_1,
                                                                                              int PageSize,
                                                                                              int PageIndex,
                                                                                              out int TotalCount);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging_New(            bool? IsCode,
                                                                                              string BrandName,
                                                                                              long V_MedProductType,
                                                                                              long? RefGenDrugCatID_1,
                                                                                              int PageSize,
                                                                                              int PageIndex,
                                                                                              out int TotalCount);
        //▲===== 25072018 TTM
        #region Supplier Gen Med Product
        
     
        [OperationContract]
        [FaultContract(typeof(AxException))]
        int SupplierGenMedProduct_Insert(SupplierGenMedProduct Supplier);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int SupplierGenMedProduct_Update(SupplierGenMedProduct Supplier);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierGenMedProduct_Delete(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID, out List<RefMedicalServiceItem> ServiceList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> DrugDeptSupplier_LoadDrugIDNotPaging(long GenMedProductID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugID(long GenMedProductID, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenMedProduct> SupplierGenMedProduct_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        int SupplierGenericDrug_Insert(SupplierGenericDrug Supplier);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int SupplierGenericDrug_Update(SupplierGenericDrug Supplier);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierGenericDrug_Delete(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID);
        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging_New(long DrugID);
        //▲===== 25072018 TTM


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugID(long DrugID, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrug> SupplierGenericDrug_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        #endregion

        #region RefGenMedProductSellingPrices
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductSellingPrices> RefGenMedProductSellingPrices_ByGenMedProductID_Paging(
            RefGenMedProductSellingPricesSearchCriteria Criteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefGenMedProductSellingPrices_Save(RefGenMedProductSellingPrices Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefGenMedProductSellingPrices_MarkDelete(Int64 GenMedSellPriceID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefGenMedProductSellingPrices RefGenMedProductSellingPrices_ByGenMedSellPriceID(Int64 GenMedSellPriceID);

        #endregion 


        #region "37.RefGenDrugBHYT_Category"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Load(bool? IsClassification, bool? IsCombined);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Combine_RefGenDrugBHYT_Category(string CategoryCheckedListXml, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefGenDrugBHYT_CategoryCombine(long RefGenDrugBHYT_CatID, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out);
        #endregion

        #region ContraIndicatorMedProductsRelToMedCond member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteConIndicatorMedProductsRelToMedCond(long MedProductsMCTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCondList(int MCTypeID,
                                                                                                 long GenMedProductID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RouteOfAdministrationContactDrug> GetRouteOfAdministrationList(long DrugROAID, long GenMedProductID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetConIndicatorMedProductsRelToMedCondAll(int MCTypeID, long GenMedProductID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void InsertConIndicatorMedProductsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition,
        //                                                        long GenMedProductID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(IList<ContraIndicatorMedProductsRelToMedCond> lstInsert
                                                            , IList<ContraIndicatorMedProductsRelToMedCond> lstDelete
                                                       , IList<ContraIndicatorMedProductsRelToMedCond> lstUpdate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertDeleteUpdateRouteOfAdministrationContactDrugXML(IList<RouteOfAdministrationContactDrug> lstInsert
                                                            , IList<RouteOfAdministrationContactDrug> lstDelete
                                                       , IList<RouteOfAdministrationContactDrug> lstUpdate);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void InsertConIndicatorMedProductsRelToMedCondEx(IList<RefGenMedProductDetails> lstRefGenericDrugDetail,
        //                                                          long MCTypeID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCond(IList<long> lstMCTpe,
                                                                                              long GenMedProductID);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Bid> GetAllBids(long V_MedProductType);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Bid> GetInUsingBidCollectionFromSupplierID(long SupplierID, long V_MedProductType);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BidDetail> GetBidDetails(long BidID, string DrugCode, bool IsMedDept, long V_MedProductType);
        //▼====: #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveBidDetails(long BidID, List<BidDetail> AddBidDetails, List<BidDetail> ModBidDetails, List<BidDetail> RemovedBidDetails
            , bool IsMedDept, long LoggedStaffID);
        //▲====: #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RemoveBidDetails(long BidID, long DrugID, bool IsMedDept);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RemoveBidDetails_New(long BidID, long DrugID, bool IsMedDept);
        //▼===== 25072018 TTM

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool EditDrugBid(Bid aBid, out long? BidIDOut);
        /*▲====: #001*/

        // 20200713 TNHX: Thêm điều kiệm tìm theo tên dùng chung
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_ForHIStore(bool IsSearchByGenericName, bool? IsCode, string BrandName, long? StoreID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_Choose(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, long? BidID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BidDetail> GetRefGenMedProductDetails_ForBid(long BidID, string BrandName, bool IsMedDept, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveListPCLExamTypeContactDrugs(List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Add,List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Del, long StaffID);

    }
}
