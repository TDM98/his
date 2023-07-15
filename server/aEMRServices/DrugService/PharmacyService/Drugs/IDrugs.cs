using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
using System.IO;
using System.Collections.ObjectModel;
/*
 * 20200218 #001 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 */
namespace PharmacyService
{
    [ServiceContract]

    public interface IDrugs
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefGenericDrugDetail GetDrugByID(long drugID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDrug(RefGenericDrugDetail drug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDrugByID(long drugID); //Nếu thuốc chưa sử dụng

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateActiveDrugByID(long drugID);//tuong ung voi delete nếu thuốc đó đã được sử dụng rồi


        [OperationContract]
        [FaultContract(typeof(AxException))]
        //get all drug 
        IList<RefGenericDrugDetail> GetDrugsJoin(int IsMedDept, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple_New(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        //▲===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_ItemPrice(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string brandName, long? SupplierID, int pageIndex, int pageSize, out int totalCount);

        //▼====: #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugSimple> SearchRefGenericDrugName_SimpleAutoPaging(bool? IsCode, string BrandName,
                                                                                       int pageIndex, int pageSize, bool IsHIStore,
                                                                                     out int totalCount);
        //▲====: #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenericDrugDetail> SearchRefDrugGenericDetails_RefAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDrug(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, out long DrugID);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDrug_New(RefGenericDrugDetail newDrug, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType, out long DrugID);
        //▲===== 25072018 TTM

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrug(RefGenericDrugDetail DrugRecord);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrug_New(RefGenericDrugDetail DrugRecord);
        //▲===== 25072018 TTM

        //common service
        #region DrugClass member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetFamilyTherapies(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugClass GetFamilyTherapyByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetFamilyTherapyParent(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetTreeViewFamilyTherapyParent(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetSearchFamilyTherapies(string faname, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewFamilyTherapy(DrugClass newfamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateFamilyTherapy(DrugClass updatefamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteFamilyTherapy(long deletefamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetTreeView(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetSearchTreeView(string faname, long V_MedProductType);

        #endregion

        #region DrugDeptClass member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetDrugDeptClasses(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugClass GetDrugDeptClassesByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetDrugDeptClassesParent(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetTreeViewDrugDeptClassesParent(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetSearchDrugDeptClasses(string faname, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDrugDeptClasses(DrugClass newfamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrugDeptClasses(DrugClass updatefamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDrugDeptClasses(DrugClass deletefamily, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetTreeView_DrugDept(long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<TherapyTree> GetSearchTreeView_DrugDept(string faname, long V_MedProductType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugClass> GetAllRefGeneric();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGeneric> GetParRefGeneric();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertRefGenericRelation(TherapyTree CurrentTherapyTree, ObservableCollection<TherapyTree> ObsRefGenericRelation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TherapyTree> GetRefGenericRelation_ForGenericID(long GenericID);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmaceuticalCompany> GetPharmaceuticalCompanyCbx();


        #region contraindicator
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedContraIndicationTypes> GetRefMedicalConditionTypesAllPaging(
                                                        int PageSize
                                                        , int PageIndex
                                                        , string OrderBy
                                                        , bool CountTotal
                                                        , out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetConIndicatorDrugsRelToMedCondAll(int MCTypeID, long DrugID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool InsertConIndicatorDrugsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition, long DrugID);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConIndicatorDrugsRelToMedCond_New(IList<RefMedContraIndicationTypes> lstRefMedicalCondition, long DrugID);
        //▲===== 25072018 TTM

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate);
        //▲===== 25072018 TTM

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ContraIndicatorDrugsRelToMedCond> GetConIndicatorDrugsRelToMedCond(IList<long> lstMCTpe, long DrugID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID);

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList_New(int MCTypeID, long DrugID);
        //▲===== 25072018 TTM

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorDrugsRelToMedCond> GetAllContrainIndicatorDrugs();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugAndConTra> GetAllDrugsContrainIndicator();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Dictionary<long, List<RefGenericRelation>> GetAllRefGenericRelation();

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool InsertConIndicatorDrugsRelToMedCondEx(IList<RefGenericDrugDetail> lstRefGenericDrugDetail, long MCTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedContraIndicationTypes> GetRefMedCondType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondPaging(
                                                long MCTypeID
                                                , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteConIndicatorDrugsRelToMedCond(long DrugsMCTypeID);
        #endregion

        #region Pharmacy Member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetRefGenMedProductDetails_Auto(bool? IsCode, string BrandName, long V_MedProductType, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefGenMedProductDetails> GetAllRefGenMedProductDetail(long V_MedProductType);

        #endregion

        #region RefDisposableMedicalResources
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefDisposableMedicalResource> RefDisposableMedicalResources_Paging(
            RefDisposableMedicalResourceSearchCriteria Criteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefDisposableMedicalResourcesInsertUpdate(
        RefDisposableMedicalResource Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefDisposableMedicalResources_MarkDelete(
        Int64 DMedRscrID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefDisposableMedicalResource RefDisposableMedicalResources_ByDMedRscrID(Int64 DMedRscrID);
        #endregion

        #region DisposableMedicalResourceTypes
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DisposableMedicalResourceType> DisposableMedicalResourceTypes_GetAll();
        #endregion

        #region 16*. Medical Condition

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedContraIndicationTypes> GetRefMedicalConditionTypes();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefMedicalConditionTypes(int MCTypeID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertRefMedicalConditionTypes(string MedConditionType, int Idx, int? AgeFrom, int? AgeTo, long V_AgeUnit);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefMedicalConditionTypes(int MCTypeID, string MedConditionType, int? AgeFrom, int? AgeTo, long V_AgeUnit);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedContraIndicationICD> GetRefMedicalConditions(int MCTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefMedicalConditions(int MCID, int MCTypeID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertRefMedicalConditions(int MCTypeID, string ICD10Code, string DiseaseNameVN);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefMedicalConditions(int MCID, int MCTypeID, string MCDescription);

        #endregion


        //<Giá Nhà Thuốc>
        #region Giá Bán Thuốc Của Nhà Thuốc PharmacySellingItemPrices
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingItemPrices> PharmacySellingItemPrices_ByDrugID_Paging(
        PharmacySellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingItemPrices_Save(PharmacySellingItemPrices Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingItemPrices_Item_Save(PharmacySellingItemPrices Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacySellingItemPrices_SaveRow(PharmacySellingItemPrices Obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingItemPrices_MarkDelete(Int64 PharmacySellingItemPriceID, out string Result);

        //KMx: Sau khi kiểm tra, thấy 3 dòng dưới không còn sử dụng nữa (31/05/2014 17:15).
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //PharmacySellingItemPrices PharmacySellingItemPrices_ByPharmacySellingItemPriceID(Int64 PharmacySellingItemPriceID);
        #endregion

        #region Bảng Giá Bán Thuốc Của Nhà Thuốc PharmacySellingPriceList

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyReferenceItemPrice> PharmacyRefPriceList_AutoCreate();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyRefPriceList_AddNew(PharmacyReferencePriceList Obj, out long ReferencePriceListID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmacyRefPriceList_Update(PharmacyReferencePriceList Obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyReferencePriceList> GetReferencePriceList(PharmacySellingPriceListSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacyReferenceItemPrice> GetPharmacyRefItemPrice(Int64 ReferencePriceListID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingPriceList_CheckCanAddNew(out bool CanAddNew);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate(out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate_V2(out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingPriceList_AddNew(PharmacySellingPriceList Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Nullable<DateTime> PharmacySellingItemPrices_EffectiveDateMax();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingPriceList> PharmacySellingPriceList_GetList_Paging(
     PharmacySellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total, out DateTime curDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingPriceList_Delete(Int64 PharmacySellingPriceListID, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail(Int64 PharmacySellingPriceListID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail_V2(Int64 PharmacySellingPriceListID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellingPriceList_Update(PharmacySellingPriceList Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Stream ExportToExcelAllItemsPriceList(Int64 PriceListID, int PriceListType, string StoreName, string ShowTitle);

        #endregion

        #region Thang Giá Bán Thuốc Của Nhà Thuốc PharmacySellPriceProfitScale
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PharmacySellPriceProfitScale> PharmacySellPriceProfitScale_GetList_Paging(
bool IsActive,

int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellPriceProfitScale_AddEdit(PharmacySellPriceProfitScale Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PharmacySellPriceProfitScale_IsActive(Int64 PharmacySellPriceProfitScaleID, Boolean IsActive, out string Result);


        #endregion
        //</Giá Nhà Thuốc>


        //<Giá Khoa Dược>
        #region Giá Bán Thuốc Của Nhà Thuốc DrugDeptSellingItemPrices
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSellingItemPrices> DrugDeptSellingItemPrices_ByDrugID_Paging(
        DrugDeptSellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
            );

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="genMedProductId"></param>
        /// <returns>DrugDeptSellingItemPrices</returns>
        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptSellingItemPrices GetDrugDeptSellingItemPriceDetails(long genMedProductId);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingItemPrices_Save(DrugDeptSellingItemPrices Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingItemPrices_MarkDelete(Int64 DrugDeptSellingItemPriceID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DrugDeptSellingItemPrices DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPriceID);
        #endregion

        #region Bảng Giá Thuốc Khoa Dược DrugDeptSellingPriceList
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingPriceList_CheckCanAddNew(out bool CanAddNew);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_AutoCreate(long V_MedProductType, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingPriceList_AddNew(DrugDeptSellingPriceList Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        Nullable<DateTime> DrugDeptSellingItemPrices_EffectiveDateMax();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSellingPriceList> DrugDeptSellingPriceList_GetList_Paging(
     DrugDeptSellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total, out DateTime curDate);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingPriceList_Delete(Int64 DrugDeptSellingPriceListID, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_Detail(Int64 DrugDeptSellingPriceListID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellingPriceList_Update(DrugDeptSellingPriceList Obj, out string Result);
        #endregion

        #region Thang Giá Bán Thuốc Của Khoa Dược DrugDeptSellPriceProfitScale
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<DrugDeptSellPriceProfitScale> DrugDeptSellPriceProfitScale_GetList_Paging(
long V_MedProductType,
bool IsActive,

int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellPriceProfitScale_AddEdit(DrugDeptSellPriceProfitScale Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DrugDeptSellPriceProfitScale_IsActive(Int64 DrugDeptSellPriceProfitScaleID, Boolean IsActive, out string Result);


        #endregion
        //<Giá Khoa Dược>

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PrescriptionDetail GetDrugInformation(long? DrugID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BidDetail> GetAutoBidDetailCollection(long V_MedProductType,
            DateTime FromDate,
            DateTime ToDate, long StoreID,
            float Factor);
    }
}