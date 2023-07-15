using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using DataEntities;
/*
 * 20200218 #001 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 */
namespace DrugProxy
{
    [ServiceContract]
    public interface IDrugs
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugByID(long drugID, AsyncCallback callback, object state);
        RefGenericDrugDetail EndGetDrugByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrug(RefGenericDrugDetail drug, AsyncCallback callback, object state);
        bool EndDeleteDrug(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrugByID(long drugID, AsyncCallback callback, object state);
        bool EndDeleteDrugByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateActiveDrugByID(long drugID, AsyncCallback callback, object state);
        bool EndUpdateActiveDrugByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugsJoin(int IsMedDept, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndGetDrugsJoin(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchDrugs(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugGenericDetails_Simple(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugGenericDetails_Simple(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugGenericDetails_ItemPrice(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugGenericDetails_ItemPrice(out int totalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string brandName, long? SupplierID, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugGenericDetails_AutoPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefGenericDrugName_SimpleAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, bool IsHIStore, AsyncCallback callback, object state);
        IList<RefGenericDrugSimple> EndSearchRefGenericDrugName_SimpleAutoPaging(out int totalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugGenericDetails_RefAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugGenericDetails_RefAutoPaging(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDrug(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, AsyncCallback callback, object state);
        bool EndAddNewDrug(out long DrugID, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrug(RefGenericDrugDetail DrugRecord, AsyncCallback callback, object state);
        bool EndUpdateDrug(IAsyncResult asyncResult);

        #region DrugClass member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFamilyTherapies(long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetFamilyTherapies(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFamilyTherapyByID(long ID, AsyncCallback callback, object state);
        DrugClass EndGetFamilyTherapyByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFamilyTherapyParent(long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetFamilyTherapyParent(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreeViewFamilyTherapyParent(long V_MedProductType, AsyncCallback callback, object state);
        List<DrugClass> EndGetTreeViewFamilyTherapyParent(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSearchFamilyTherapies(string faname, long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetSearchFamilyTherapies(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewFamilyTherapy(DrugClass newfamily, AsyncCallback callback, object state);
        bool EndAddNewFamilyTherapy(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateFamilyTherapy(DrugClass updatefamily, AsyncCallback callback, object state);
        bool EndUpdateFamilyTherapy(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteFamilyTherapy(long deletefamily, AsyncCallback callback, object state);
        bool EndDeleteFamilyTherapy(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreeView(long V_MedProductType, AsyncCallback callback, object state);
        List<TherapyTree> EndGetTreeView(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSearchTreeView(string faname, long V_MedProductType, AsyncCallback callback, object state);
        List<TherapyTree> EndGetSearchTreeView(IAsyncResult asyncResult);

        #endregion

        #region DrugDeptClass member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptClasses(long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetDrugDeptClasses(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptClassesByID(long ID, AsyncCallback callback, object state);
        DrugClass EndGetDrugDeptClassesByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptClassesParent(long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetDrugDeptClassesParent(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreeViewDrugDeptClassesParent(long V_MedProductType, AsyncCallback callback, object state);
        List<DrugClass> EndGetTreeViewDrugDeptClassesParent(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSearchDrugDeptClasses(string faname, long V_MedProductType, AsyncCallback callback, object state);
        IList<DrugClass> EndGetSearchDrugDeptClasses(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDrugDeptClasses(DrugClass newfamily, AsyncCallback callback, object state);
        bool EndAddNewDrugDeptClasses(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrugDeptClasses(DrugClass updatefamily, AsyncCallback callback, object state);
        bool EndUpdateDrugDeptClasses(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrugDeptClasses(DrugClass deletefamily, AsyncCallback callback, object state);
        bool EndDeleteDrugDeptClasses(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreeView_DrugDept(long V_MedProductType, AsyncCallback callback, object state);
        List<TherapyTree> EndGetTreeView_DrugDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSearchTreeView_DrugDept(string faname, long V_MedProductType, AsyncCallback callback, object state);
        List<TherapyTree> EndGetSearchTreeView_DrugDept(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRefGeneric(AsyncCallback callback, object state);
        IList<DrugClass> EndGetAllRefGeneric(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetParRefGeneric(AsyncCallback callback, object state);
        IList<RefGeneric> EndGetParRefGeneric(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertRefGenericRelation(TherapyTree CurrentTherapyTree, ObservableCollection<TherapyTree> ObsRefGenericRelation, AsyncCallback callback, object state);
        bool EndInsertRefGenericRelation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenericRelation_ForGenericID(long GenericID, AsyncCallback callback, object state);
        List<TherapyTree> EndGetRefGenericRelation_ForGenericID(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPharmaceuticalCompanyCbx(AsyncCallback callback, object state);
        IList<PharmaceuticalCompany> EndGetPharmaceuticalCompanyCbx(IAsyncResult asyncResult);


        #region contraindicator

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedicalConditionTypesAllPaging(int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<RefMedContraIndicationTypes> EndGetRefMedicalConditionTypesAllPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConIndicatorDrugsRelToMedCondAll(int MCTypeID, long DrugID, AsyncCallback callback, object state);
        bool EndGetConIndicatorDrugsRelToMedCondAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConIndicatorDrugsRelToMedCond(ObservableCollection<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID, AsyncCallback callback, object state);
        bool EndInsertConIndicatorDrugsRelToMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate, AsyncCallback callback, object state);
        bool EndInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConIndicatorDrugsRelToMedCond(IList<long> lstMCTpe, long DrugID, AsyncCallback callback, object state);
        IList<ContraIndicatorDrugsRelToMedCond> EndGetConIndicatorDrugsRelToMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID, AsyncCallback callback, object state);
        List<ContraIndicatorDrugsRelToMedCond> EndGetContraIndicatorDrugsRelToMedCondList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllContrainIndicatorDrugs(AsyncCallback callback, object state);
        List<ContraIndicatorDrugsRelToMedCond> EndGetAllContrainIndicatorDrugs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDrugsContrainIndicator(AsyncCallback callback, object state);
        List<DrugAndConTra> EndGetAllDrugsContrainIndicator(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConIndicatorDrugsRelToMedCondEx(IList<RefGenericDrugDetail> lstRefGenericDrugDetail, long MCTypeID, AsyncCallback callback, object state);
        bool EndInsertConIndicatorDrugsRelToMedCondEx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedCondType(AsyncCallback callback, object state);
        List<RefMedContraIndicationTypes> EndGetRefMedCondType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetContraIndicatorDrugsRelToMedCondPaging(long MCTypeID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ContraIndicatorDrugsRelToMedCond> EndGetContraIndicatorDrugsRelToMedCondPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteConIndicatorDrugsRelToMedCond(long DrugsMCTypeID, AsyncCallback callback, object state);
        bool EndDeleteConIndicatorDrugsRelToMedCond(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRefGenericRelation(AsyncCallback callback, object state);
        Dictionary<long, List<RefGenericRelation>> EndGetAllRefGenericRelation(IAsyncResult asyncResult);
        #endregion

        #region Pharmacy Member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefGenMedProductDetails_Auto(bool? IsCode, string BrandName, long V_MedProductType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetRefGenMedProductDetails_Auto(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllRefGenMedProductDetail(long V_MedProductType, AsyncCallback callback, object state);
        IList<RefGenMedProductDetails> EndGetAllRefGenMedProductDetail(IAsyncResult asyncResult);

        #endregion

        #region RefDisposableMedicalResources

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDisposableMedicalResources_Paging(RefDisposableMedicalResourceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        IList<RefDisposableMedicalResource> EndRefDisposableMedicalResources_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDisposableMedicalResourcesInsertUpdate(RefDisposableMedicalResource Obj, AsyncCallback callback, object state);
        void EndRefDisposableMedicalResourcesInsertUpdate(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDisposableMedicalResources_MarkDelete(Int64 DMedRscrID, AsyncCallback callback, object state);
        void EndRefDisposableMedicalResources_MarkDelete(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDisposableMedicalResources_ByDMedRscrID(Int64 DMedRscrID, AsyncCallback callback, object state);
        RefDisposableMedicalResource EndRefDisposableMedicalResources_ByDMedRscrID(IAsyncResult asyncResult);

        #endregion

        #region DisposableMedicalResourceTypes
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDisposableMedicalResourceTypes_GetAll(AsyncCallback callback, object state);
        IList<DisposableMedicalResourceType> EndDisposableMedicalResourceTypes_GetAll(IAsyncResult asyncResult);

        #endregion

        #region 16*. Medical Condition

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedicalConditionTypes(AsyncCallback callback, object state);
        List<RefMedContraIndicationTypes> EndGetRefMedicalConditionTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefMedicalConditionTypes(int MCTypeID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteRefMedicalConditionTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertRefMedicalConditionTypes(string MedConditionType, int Idx, int? AgeFrom, int? AgeTo, long V_AgeUnit, AsyncCallback callback, object state);
        bool EndInsertRefMedicalConditionTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefMedicalConditionTypes(int MCTypeID, string MedConditionType, int? AgeFrom, int? AgeTo, long V_AgeUnit, AsyncCallback callback, object state);
        bool EndUpdateRefMedicalConditionTypes(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefMedicalConditions(int MCTypeID, AsyncCallback callback, object state);
        IList<RefMedContraIndicationICD> EndGetRefMedicalConditions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefMedicalConditions(int MCID, int MCTypeID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteRefMedicalConditions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertRefMedicalConditions(int MCTypeID, string ICD10Code, string DiseaseNameVN, AsyncCallback callback, object state);
        bool EndInsertRefMedicalConditions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefMedicalConditions(int MCID, int MCTypeID, string MCDescription, AsyncCallback callback, object state);
        bool EndUpdateRefMedicalConditions(IAsyncResult asyncResult);

        #endregion

        //<Giá Nhà Thuốc>
        #region Giá Bán Thuốc Của Nhà Thuốc PharmacySellingItemPrices 
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_ByDrugID_Paging(PharmacySellingItemPricesSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<PharmacySellingItemPrices> EndPharmacySellingItemPrices_ByDrugID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_Save(PharmacySellingItemPrices Obj, AsyncCallback callback, object state);
        void EndPharmacySellingItemPrices_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_Item_Save(PharmacySellingItemPrices Obj, AsyncCallback callback, object state);
        void EndPharmacySellingItemPrices_Item_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_SaveRow(PharmacySellingItemPrices Obj, AsyncCallback callback, object state);
        bool EndPharmacySellingItemPrices_SaveRow(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_MarkDelete(Int64 PharmacySellingItemPriceID, AsyncCallback callback, object state);
        void EndPharmacySellingItemPrices_MarkDelete(out string Result, IAsyncResult asyncResult);

        //KMx: Sau khi kiểm tra, thấy 3 dòng dưới không còn sử dụng nữa (31/05/2014 17:15).
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPharmacySellingItemPrices_ByPharmacySellingItemPriceID(Int64 PharmacySellingItemPriceID, AsyncCallback callback, object state);
        //PharmacySellingItemPrices EndPharmacySellingItemPrices_ByPharmacySellingItemPriceID(IAsyncResult asyncResult);

        #endregion

        #region Bảng Giá Bán Thuốc Của Nhà Thuốc PharmacySellingPriceList

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyRefPriceList_AutoCreate(AsyncCallback callback, object state);
        List<PharmacyReferenceItemPrice> EndPharmacyRefPriceList_AutoCreate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyRefPriceList_AddNew(PharmacyReferencePriceList Obj, AsyncCallback callback, object state);
        bool EndPharmacyRefPriceList_AddNew(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacyRefPriceList_Update(PharmacyReferencePriceList Obj, AsyncCallback callback, object state);
        bool EndPharmacyRefPriceList_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetReferencePriceList(PharmacySellingPriceListSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<PharmacyReferencePriceList> EndGetReferencePriceList(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPharmacyRefItemPrice(Int64 ReferencePriceListID, AsyncCallback callback, object state);
        List<PharmacyReferenceItemPrice> EndGetPharmacyRefItemPrice(IAsyncResult asyncResult);



        //Check CanAddNew
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_CheckCanAddNew(AsyncCallback callback, object state);
        void EndPharmacySellingPriceList_CheckCanAddNew(out bool CanAddNew, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_AutoCreate(AsyncCallback callback, object state);
        List<PharmacySellingItemPrices> EndPharmacySellingPriceList_AutoCreate(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_AutoCreate_V2(AsyncCallback callback, object state);
        List<PharmacySellingItemPrices> EndPharmacySellingPriceList_AutoCreate_V2(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_AddNew(PharmacySellingPriceList Obj, AsyncCallback callback, object state);
        void EndPharmacySellingPriceList_AddNew(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingItemPrices_EffectiveDateMax(AsyncCallback callback, object state);
        DateTime EndPharmacySellingItemPrices_EffectiveDateMax(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_GetList_Paging(
    PharmacySellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     AsyncCallback callback, object state);
        List<PharmacySellingPriceList> EndPharmacySellingPriceList_GetList_Paging(out int Total, out DateTime curDate, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_Delete(Int64 PharmacySellingPriceListID, AsyncCallback callback, object state);
        void EndPharmacySellingPriceList_Delete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_Detail(Int64 PharmacySellingPriceListID, AsyncCallback callback, object state);
        List<PharmacySellingItemPrices> EndPharmacySellingPriceList_Detail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_Detail_V2(Int64 PharmacySellingPriceListID, AsyncCallback callback, object state);
        List<PharmacySellingItemPrices> EndPharmacySellingPriceList_Detail_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellingPriceList_Update(PharmacySellingPriceList Obj, AsyncCallback callback, object state);
        void EndPharmacySellingPriceList_Update(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportToExcelAllItemsPriceList(Int64 PriceListID, int PriceListType, string StoreName, string ShowTitle, AsyncCallback callback, object state);
        byte[] EndExportToExcelAllItemsPriceList(IAsyncResult asyncResult);

        #endregion

        #region Thang Giá Bán Của Nhà Thuốc PharmacySellPriceProfitScale
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellPriceProfitScale_GetList_Paging(
            bool IsActive,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        IList<PharmacySellPriceProfitScale> EndPharmacySellPriceProfitScale_GetList_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellPriceProfitScale_AddEdit(PharmacySellPriceProfitScale Obj, AsyncCallback callback, object state);
        void EndPharmacySellPriceProfitScale_AddEdit(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmacySellPriceProfitScale_IsActive(Int64 PharmacySellPriceProfitScaleID, Boolean IsActive, AsyncCallback callback, object state);
        void EndPharmacySellPriceProfitScale_IsActive(out string Result, IAsyncResult asyncResult);

        #endregion
        //</Giá Nhà Thuốc>


        //<Giá Thuốc Khoa Dược>
        #region Giá Bán Thuốc Của Khoa Dược DrugDeptSellingItemPrices
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingItemPrices_ByDrugID_Paging(DrugDeptSellingItemPricesSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<DrugDeptSellingItemPrices> EndDrugDeptSellingItemPrices_ByDrugID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptSellingItemPriceDetails(long genMedProductId, AsyncCallback callback, object state);
        DrugDeptSellingItemPrices EndGetDrugDeptSellingItemPriceDetails(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingItemPrices_Save(DrugDeptSellingItemPrices Obj, AsyncCallback callback, object state);
        void EndDrugDeptSellingItemPrices_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingItemPrices_MarkDelete(Int64 DrugDeptSellingItemPriceID, AsyncCallback callback, object state);
        void EndDrugDeptSellingItemPrices_MarkDelete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPriceID, AsyncCallback callback, object state);
        DrugDeptSellingItemPrices EndDrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(IAsyncResult asyncResult);

        #endregion

        #region "Bảng Giá Thuốc Khoa Dược DrugDeptSellingPriceList"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_CheckCanAddNew(AsyncCallback callback, object state);
        void EndDrugDeptSellingPriceList_CheckCanAddNew(out bool CanAddNew, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_AutoCreate(long V_MedProductType, AsyncCallback callback, object state);
        List<DrugDeptSellingItemPrices> EndDrugDeptSellingPriceList_AutoCreate(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_AddNew(DrugDeptSellingPriceList Obj, AsyncCallback callback, object state);
        void EndDrugDeptSellingPriceList_AddNew(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingItemPrices_EffectiveDateMax(AsyncCallback callback, object state);
        DateTime EndDrugDeptSellingItemPrices_EffectiveDateMax(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_GetList_Paging(
    DrugDeptSellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     AsyncCallback callback, object state);
        List<DrugDeptSellingPriceList> EndDrugDeptSellingPriceList_GetList_Paging(out int Total, out DateTime curDate, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_Delete(Int64 DrugDeptSellingPriceListID, AsyncCallback callback, object state);
        void EndDrugDeptSellingPriceList_Delete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_Detail(Int64 DrugDeptSellingPriceListID, AsyncCallback callback, object state);
        List<DrugDeptSellingItemPrices> EndDrugDeptSellingPriceList_Detail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellingPriceList_Update(DrugDeptSellingPriceList Obj, AsyncCallback callback, object state);
        void EndDrugDeptSellingPriceList_Update(out string Result, IAsyncResult asyncResult);
        #endregion

        #region Thang Giá Bán Của Khoa Dược DrugDeptSellPriceProfitScale
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellPriceProfitScale_GetList_Paging(
            long V_MedProductType,
            bool IsActive,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        IList<DrugDeptSellPriceProfitScale> EndDrugDeptSellPriceProfitScale_GetList_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellPriceProfitScale_AddEdit(DrugDeptSellPriceProfitScale Obj, AsyncCallback callback, object state);
        void EndDrugDeptSellPriceProfitScale_AddEdit(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSellPriceProfitScale_IsActive(Int64 DrugDeptSellPriceProfitScaleID, Boolean IsActive, AsyncCallback callback, object state);
        void EndDrugDeptSellPriceProfitScale_IsActive(out string Result, IAsyncResult asyncResult);

        #endregion
        //</Giá Thuốc Khoa Dược>

        #region Xáp nhập khoa dược nhà thuốc
        //List danh mục thuốc
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRefDrugGenericDetails_Simple_New(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefGenericDrugDetail> EndSearchRefDrugGenericDetails_Simple_New(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrugByID_New(long drugID, AsyncCallback callback, object state);
        bool EndDeleteDrugByID_New(IAsyncResult asyncResult);

        //Add/Edit thuốc
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDrug_New(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, AsyncCallback callback, object state);
        bool EndAddNewDrug_New(out long DrugID, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrug_New(RefGenericDrugDetail DrugRecord, AsyncCallback callback, object state);
        bool EndUpdateDrug_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetContraIndicatorDrugsRelToMedCondList_New(int MCTypeID, long DrugID, AsyncCallback callback, object state);
        List<ContraIndicatorDrugsRelToMedCond> EndGetContraIndicatorDrugsRelToMedCondList_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConIndicatorDrugsRelToMedCond_New(ObservableCollection<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID, AsyncCallback callback, object state);
        bool EndInsertConIndicatorDrugsRelToMedCond_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                            , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                            , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate, AsyncCallback callback, object state);
        bool EndInsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugInformation(long? DrugID, AsyncCallback callback, object state);
        PrescriptionDetail EndGetDrugInformation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAutoBidDetailCollection(long V_MedProductType,
            DateTime FromDate,
            DateTime ToDate, long StoreID,
            float Factor, AsyncCallback callback, object state);
        List<BidDetail> EndGetAutoBidDetailCollection(IAsyncResult asyncResult);
    }
}