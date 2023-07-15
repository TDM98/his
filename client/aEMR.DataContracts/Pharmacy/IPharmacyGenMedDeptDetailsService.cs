/*
 * 20170810 #001 CMN: Added Bid Service
 * 20180801 #002 TTM: Add RefGenMedProductDetails_Save_New
 * 20181219 #003 TTM: Tạo mới hàm lấy dữ liệu cho AutoComplete lấy thuốc chỉ lấy thuốc dùng chung.
 * 20191106 #004 TNHX: [BM 0013306]: separate V_MedProductType
 * 20210122 #005 TNHX: [BM ]: Tách danh sách thêm/xóa/sửa + ghi log user cập nhật
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using aEMR.DataContracts;

namespace RefGenMedProductDetailsServiceProxy
{
    [ServiceContract]
    public interface IRefGenMedProductDetails
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenMedProductDetails_GetALL(long V_MedProductType, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndGetRefGenMedProductDetails_GetALL(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_Paging(RefGenMedProductDetailsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_Save(RefGenMedProductDetails Obj,IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, AsyncCallback callback, object state);
        void EndRefGenMedProductDetails_Save(out string Res, out long GenMedProductID, IAsyncResult asyncResult);

        //▼===== #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_Save_New(RefGenMedProductDetails Obj, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, AsyncCallback callback, object state);
        void EndRefGenMedProductDetails_Save_New(out string Res, out long GenMedProductID, IAsyncResult asyncResult);
        //▲===== #002

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_MarkDelete(Int64 GenMedProductID, AsyncCallback callback, object state);
        void EndRefGenMedProductDetails_MarkDelete(out string Res, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<DrugClass> EndDrugClasses_SearchPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<DrugClass> EndDrugDeptClasses_SearchPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefCountries_SearchPaging(RefCountrySearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<RefCountry> EndRefCountries_SearchPaging(out int totalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<RefUnit> EndRefUnits_SearchPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptRefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<RefUnit> EndDrugDeptRefUnits_SearchPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SearchAutoPaging(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? BidID, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_SearchAutoPaging(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SearchAutoPaging_V2(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? RefGenDrugCatID_2, long? BidID, bool IsGetDrugDeptItemsOnly, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_SearchAutoPaging_V2(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SimpleAutoPaging(bool? IsCode, string BrandName, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<RefGenMedProductSimple> EndRefGenMedProductDetails_SimpleAutoPaging(out int TotalCount, IAsyncResult asyncResult);


        #region Supplier Gen Med Product

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_Insert(SupplierGenMedProduct Supplier, AsyncCallback callback, object state);
        int EndSupplierGenMedProduct_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_Update(SupplierGenMedProduct Supplier, AsyncCallback callback, object state);
        int EndSupplierGenMedProduct_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_Delete(long ID, AsyncCallback callback, object state);
        bool EndSupplierGenMedProduct_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID, AsyncCallback callback, object state);
        List<SupplierGenMedProduct> EndSupplierGenMedProduct_LoadDrugIDNotPaging(out List<RefMedicalServiceItem> ServiceList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSupplier_LoadDrugIDNotPaging(long GenMedProductID, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndDrugDeptSupplier_LoadDrugIDNotPaging(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_LoadDrugID(long GenMedProductID, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierGenMedProduct> EndSupplierGenMedProduct_LoadDrugID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProduct_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierGenMedProduct> EndSupplierGenMedProduct_LoadSupplierID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_Insert(SupplierGenericDrug Supplier, AsyncCallback callback, object state);
        int EndSupplierGenericDrug_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_Update(SupplierGenericDrug Supplier, AsyncCallback callback, object state);
        int EndSupplierGenericDrug_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_Delete(long ID, AsyncCallback callback, object state);
        bool EndSupplierGenericDrug_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_LoadDrugIDNotPaging(long DrugID, AsyncCallback callback, object state);
        List<SupplierGenericDrug> EndSupplierGenericDrug_LoadDrugIDNotPaging(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_LoadDrugID(long DrugID, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierGenericDrug> EndSupplierGenericDrug_LoadDrugID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<SupplierGenericDrug> EndSupplierGenericDrug_LoadSupplierID(out int TotalCount, IAsyncResult asyncResult);

        #endregion

        #region RefGenMedProductSellingPrices

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductSellingPrices_ByGenMedProductID_Paging(RefGenMedProductSellingPricesSearchCriteria Criteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
               AsyncCallback callback, object state);
        List<RefGenMedProductSellingPrices> EndRefGenMedProductSellingPrices_ByGenMedProductID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductSellingPrices_Save(RefGenMedProductSellingPrices Obj, AsyncCallback callback, object state);
        void EndRefGenMedProductSellingPrices_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductSellingPrices_MarkDelete(Int64 GenMedSellPriceID, AsyncCallback callback, object state);
        void EndRefGenMedProductSellingPrices_MarkDelete(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductSellingPrices_ByGenMedSellPriceID(Int64 GenMedSellPriceID, AsyncCallback callback, object state);
        RefGenMedProductSellingPrices EndRefGenMedProductSellingPrices_ByGenMedSellPriceID(IAsyncResult asyncResult);


        #endregion

        #region "37.RefGenDrugBHYT_Category"

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenDrugBHYT_Category_Load(bool? IsClassification, bool? IsCombined, AsyncCallback callback, object state);
        List<RefGenDrugBHYT_Category> EndRefGenDrugBHYT_Category_Load(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCombine_RefGenDrugBHYT_Category(string CategoryCheckedListXml, AsyncCallback callback, object state);
        bool EndCombine_RefGenDrugBHYT_Category(out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefGenDrugBHYT_CategoryCombine(long RefGenDrugBHYT_CatID, AsyncCallback callback, object state);
        bool EndDeleteRefGenDrugBHYT_CategoryCombine(out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out, IAsyncResult asyncResult);

        #endregion

        #region ContraIndicatorMedProductsRelToMedCond member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteConIndicatorMedProductsRelToMedCond(long MedProductsMCTypeID, AsyncCallback callback, object state);
        bool EndDeleteConIndicatorMedProductsRelToMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetContraIndicatorMedProductsRelToMedCondList(int MCTypeID, long GenMedProductID, AsyncCallback callback, object state);
        List<ContraIndicatorMedProductsRelToMedCond> EndGetContraIndicatorMedProductsRelToMedCondList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRouteOfAdministrationList(long DrugROAID, long GenMedProductID, AsyncCallback callback, object state);
        List<RouteOfAdministrationContactDrug> EndGetRouteOfAdministrationList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConIndicatorMedProductsRelToMedCondAll(int MCTypeID, long GenMedProductID, AsyncCallback callback, object state);
        bool EndGetConIndicatorMedProductsRelToMedCondAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConIndicatorMedProductsRelToMedCond(IList<RefMedContraIndicationTypes> lstRefMedicalCondition, long GenMedProductID, AsyncCallback callback, object state);
        void EndInsertConIndicatorMedProductsRelToMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(IList<ContraIndicatorMedProductsRelToMedCond> lstInsert
                                                            , IList<ContraIndicatorMedProductsRelToMedCond> lstDelete
                                                       , IList<ContraIndicatorMedProductsRelToMedCond> lstUpdate, AsyncCallback callback, object state);

        bool EndInsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertDeleteUpdateRouteOfAdministrationContactDrugXML(IList<RouteOfAdministrationContactDrug> lstInsert
                                                            , IList<RouteOfAdministrationContactDrug> lstDelete
                                                       , IList<RouteOfAdministrationContactDrug> lstUpdate, AsyncCallback callback, object state);
        bool EndInsertDeleteUpdateRouteOfAdministrationContactDrugXML(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConIndicatorMedProductsRelToMedCondEx(IList<RefGenMedProductDetails> lstRefGenericDrugDetail, long MCTypeID, AsyncCallback callback, object state);
        void EndInsertConIndicatorMedProductsRelToMedCondEx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetContraIndicatorMedProductsRelToMedCond(IList<long> lstMCTpe, long GenMedProductID, AsyncCallback callback, object state);
        List<ContraIndicatorMedProductsRelToMedCond> EndGetContraIndicatorMedProductsRelToMedCond(IAsyncResult asyncResult);
        #endregion

        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllBids(long V_MedProductType, AsyncCallback callback, object state);
        List<Bid> EndGetAllBids(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetInUsingBidCollectionFromSupplierID(long SupplierID, long V_MedProductType, AsyncCallback callback, object state);
        List<Bid> EndGetInUsingBidCollectionFromSupplierID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBidDetails(long BidID, string DrugCode, bool IsMedDept, long V_MedProductType, AsyncCallback callback, object state);
        List<BidDetail> EndGetBidDetails(IAsyncResult asyncResult);
        //▼====: #005
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveBidDetails(long BidID, List<BidDetail> AddBidDetails, List<BidDetail> ModBidDetails, List<BidDetail> RemovedBidDetails
            , bool IsMedDept, long LoggedStaffID, AsyncCallback callback, object state);
        bool EndSaveBidDetails(IAsyncResult asyncResult);
        //▲====: #005
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRemoveBidDetails(long BidID, long DrugID, bool IsMedDept, AsyncCallback callback, object state);
        bool EndRemoveBidDetails(IAsyncResult asyncResult);
        /*▲====: #001*/
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditDrugBid(Bid aBid, AsyncCallback callback, object state);
        bool EndEditDrugBid(out long? BidIDOut, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenMedProductDetails_ForBid(long BidID, string BrandName, bool IsMedDept, long V_MedProductType, AsyncCallback callback, object state);
        List<BidDetail> EndGetRefGenMedProductDetails_ForBid(IAsyncResult asyncResult);

        #region Xáp nhập Nhà thuốc - Khoa dược
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrug_LoadDrugIDNotPaging_New(long DrugID, AsyncCallback callback, object state);
        List<SupplierGenericDrug> EndSupplierGenericDrug_LoadDrugIDNotPaging_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SimpleAutoPaging_New(bool? IsCode, string BrandName, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<RefGenMedProductSimple> EndRefGenMedProductDetails_SimpleAutoPaging_New(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRemoveBidDetails_New(long BidID, long DrugID, bool IsMedDept, AsyncCallback callback, object state);
        bool EndRemoveBidDetails_New(IAsyncResult asyncResult);
        #endregion
        //▼====== #003
        // 20200713 TNHX: Thêm điều kiệm tìm theo tên dùng chung
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SearchAutoPaging_ForHIStore(bool IsSearchByGenericName, bool? IsCode, string BrandName, long? StoreID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_SearchAutoPaging_ForHIStore(out int TotalCount, IAsyncResult asyncResult);
        //▲====== #003

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_SearchAutoPaging_Choose(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, long? BidID, AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_SearchAutoPaging_Choose(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginListPCLExamTypeContactDrugs_Paging(long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<PCLExamTypeContactDrugs> EndListPCLExamTypeContactDrugs_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveListPCLExamTypeContactDrugs(List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Add,
          List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Del, long StaffID,
          AsyncCallback callback, object state);
        bool EndSaveListPCLExamTypeContactDrugs(IAsyncResult asyncResult);
    }
}
