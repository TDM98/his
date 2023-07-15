/*
 * 20170810 #001 CMN: Added Bid Service
 * 20180801 #002 TTM: Add void RefGenMedProductDetails_Save_New
 * 20191106 #003 TNHX: [BM 0013306]: separate V_MedProductType
 * 20210122 #004 TNHX: Tách danh sách thêm/xóa/sửa + ghi log user cập nhật + thêm Mã gói thầu
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class RefGenMedProductDetailsService : eHCMS.WCFServiceCustomHeader, IRefGenMedProductDetails
    {
        public RefGenMedProductDetailsService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public List<RefGenMedProductDetails> GetRefGenMedProductDetails_GetALL(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenMedProductDetails_GetALL(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRefGenMedProductDetails_GetALL. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefGenMedProductDetails> RefGenMedProductDetails_Paging(RefGenMedProductDetailsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                List<RefGenMedProductDetails> item = null;
                item = RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return item;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrugs_Paging(long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                List<PCLExamTypeContactDrugs> item = null;
                item = RefDrugGenericDetailsProvider.Instance.ListPCLExamTypeContactDrugs_Paging(PCLExamTypeID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                return item;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ListPCLExamTypeContactDrugs_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void RefGenMedProductDetails_Save(RefGenMedProductDetails Obj, out string Res, out long GenMedProductID, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_Save(Obj, out Res, out GenMedProductID);
                if (GenMedProductID > 0 && lstRefMedicalConditionType != null && lstRefMedicalConditionType.Count > 0)
                {
                    //InsertConIndicatorMedProductsRelToMedCond(lstRefMedicalConditionType, GenMedProductID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼===== #002
        public void RefGenMedProductDetails_Save_New(RefGenMedProductDetails Obj, out string Res, out long GenMedProductID, IList<RefMedContraIndicationTypes> lstRefMedicalConditionType)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_Save_New(Obj, out Res, out GenMedProductID);
                if (GenMedProductID > 0 && lstRefMedicalConditionType != null && lstRefMedicalConditionType.Count > 0)
                {
                    //InsertConIndicatorMedProductsRelToMedCond(lstRefMedicalConditionType, GenMedProductID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_Save_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #002

        public void RefGenMedProductDetails_MarkDelete(long GenMedProductID, out string Res)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_MarkDelete(GenMedProductID, out Res);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //Properties Navigate
        public List<DrugClass> DrugClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugClasses_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugClasses_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_DrugClasses_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<DrugClass> DrugDeptClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptClasses_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugClasses_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_DrugClasses_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefCountry> RefCountries_SearchPaging(RefCountrySearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefCountries_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefCountries_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefCountries_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefUnit> RefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefUnits_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefUnits_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefUnits_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefUnit> DrugDeptRefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptRefUnits_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefUnits_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefUnits_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? BidID, out int TotalCount)
        {
            return RefGenMedProductDetails_SearchAutoPaging_V2(IsCode, BrandName, SupplierID, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, null, BidID, false, out TotalCount);
        }
        public List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_V2(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, long? RefGenDrugCatID_2, long? BidID
            , bool IsGetDrugDeptItemsOnly
            , out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_SearchAutoPaging_V2(IsCode, BrandName, SupplierID, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, out TotalCount, RefGenDrugCatID_2, BidID, IsGetDrugDeptItemsOnly);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_SearchAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging(bool? IsCode,
                                                                                              string BrandName,
                                                                                              long V_MedProductType,
                                                                                              long? RefGenDrugCatID_1,
                                                                                              int PageSize,
                                                                                              int PageIndex,
                                                                                              out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_SimpleAutoPaging(IsCode, BrandName, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_SimpleAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼===== 25072018 TTM
        public List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging_New(bool? IsCode,
            string BrandName,
            long V_MedProductType,
            long? RefGenDrugCatID_1,
            int PageSize,
            int PageIndex,
            out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_SimpleAutoPaging_New(IsCode, BrandName, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_SimpleAutoPaging_New. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_SearchAutoPaging);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== 25072018 TTM


        #region Supplier Gen Med Product


        public int SupplierGenMedProduct_Insert(SupplierGenMedProduct Supplier)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_Insert(Supplier);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int SupplierGenMedProduct_Update(SupplierGenMedProduct Supplier)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_Update(Supplier);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SupplierGenMedProduct_Delete(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_Delete(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<DrugDeptSupplier> DrugDeptSupplier_LoadDrugIDNotPaging(long GenMedProductID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSupplier_LoadDrugIDNotPaging(GenMedProductID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptSupplier_LoadDrugIDNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_LoadDrugID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID, out List<RefMedicalServiceItem> ServiceList)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_LoadDrugIDNotPaging(GenMedProductID, out ServiceList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_LoadDrugIDNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_LoadDrugID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugID(long GenMedProductID, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_LoadDrugID(GenMedProductID, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_LoadDrugID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_LoadDrugID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<SupplierGenMedProduct> SupplierGenMedProduct_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProduct_LoadSupplierID(SupplierID, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProduct_LoadSupplierID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenMedProduct_LoadSupplierID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public int SupplierGenericDrug_Insert(SupplierGenericDrug Supplier)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_Insert(Supplier);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenericDrug_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int SupplierGenericDrug_Update(SupplierGenericDrug Supplier)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_Update(Supplier);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenericDrug_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SupplierGenericDrug_Delete(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_Delete(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenericDrug_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_LoadDrugIDNotPaging(DrugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_LoadDrugIDNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼===== 25072018 TTM
        public List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging_New(long DrugID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_LoadDrugIDNotPaging_New(DrugID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_LoadDrugIDNotPaging_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== 25072018 TTM

        public List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugID(long DrugID, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_LoadDrugID(DrugID, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_LoadDrugID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenericDrug_LoadDrugID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<SupplierGenericDrug> SupplierGenericDrug_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrug_LoadSupplierID(SupplierID, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrug_LoadSupplierID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_SupplierGenericDrug_LoadSupplierID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public List<RefGenMedProductSellingPrices> RefGenMedProductSellingPrices_ByGenMedProductID_Paging(RefGenMedProductSellingPricesSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductSellingPrices_ByGenMedProductID_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductSellingPrices_ByGenMedProductID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedProductID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void RefGenMedProductSellingPrices_Save(RefGenMedProductSellingPrices Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefGenMedProductSellingPrices_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductSellingPrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void RefGenMedProductSellingPrices_MarkDelete(Int64 GenMedSellPriceID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.RefGenMedProductSellingPrices_MarkDelete(GenMedSellPriceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductSellingPrices_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public RefGenMedProductSellingPrices RefGenMedProductSellingPrices_ByGenMedSellPriceID(long GenMedSellPriceID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductSellingPrices_ByGenMedSellPriceID(GenMedSellPriceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductSellingPrices_ByGenMedSellPriceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region "37.RefGenDrugBHYT_Category"

        public List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Load(bool? IsClassification, bool? IsCombined)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenDrugBHYT_Category_Load(IsClassification, IsCombined);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenDrugBHYT_Category_Load. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenDrugBHYT_Category_Load);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool Combine_RefGenDrugBHYT_Category(string CategoryCheckedListXml, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving Combine_RefGenDrugBHYT_Category.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.Combine_RefGenDrugBHYT_Category(CategoryCheckedListXml, out RefGenDrugBHYT_Category_Out);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Combine_RefGenDrugBHYT_Category. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_Combine_RefGenDrugBHYT_Category);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteRefGenDrugBHYT_CategoryCombine(long RefGenDrugBHYT_CatID, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start of retrieving DeleteRefGenDrugBHYT_CategoryCombine.", CurrentUser);
                return RefDrugGenericDetailsProvider.Instance.DeleteRefGenDrugBHYT_CategoryCombine(RefGenDrugBHYT_CatID, out RefGenDrugBHYT_Category_Out);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteRefGenDrugBHYT_CategoryCombine. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_DeleteRefGenDrugBHYT_CategoryCombine);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        #region ContraIndicatorMedProductsRelToMedCond member

        public bool DeleteConIndicatorMedProductsRelToMedCond(long MedProductsMCTypeID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteConIndicatorMedProductsRelToMedCond(MedProductsMCTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteConIndicatorMedProductsRelToMedCond. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCondList(int MCTypeID,
                                                                                                 long GenMedProductID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetContraIndicatorMedProductsRelToMedCondList(MCTypeID, GenMedProductID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetContraIndicatorMedProductsRelToMedCondList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<RouteOfAdministrationContactDrug> GetRouteOfAdministrationList(long DrugROAID,
                                                                                                 long GenMedProductID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRouteOfAdministrationList(DrugROAID, GenMedProductID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetRouteOfAdministrationList. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool GetConIndicatorMedProductsRelToMedCondAll(int MCTypeID, long GenMedProductID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetConIndicatorMedProductsRelToMedCondAll(MCTypeID, GenMedProductID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetConIndicatorMedProductsRelToMedCondAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //public void InsertConIndicatorMedProductsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition, long GenMedProductID)
        //{
        //    try
        //    {
        //        RefDrugGenericDetailsProvider.Instance.InsertConIndicatorMedProductsRelToMedCond(lstRefMedicalCondition, GenMedProductID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving InsertConIndicatorMedProductsRelToMedCond. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
        //    }
        //}
        public bool InsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(IList<ContraIndicatorMedProductsRelToMedCond> lstInsert
                                                            , IList<ContraIndicatorMedProductsRelToMedCond> lstDelete
                                                       , IList<ContraIndicatorMedProductsRelToMedCond> lstUpdate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(lstInsert, lstDelete, lstUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool InsertDeleteUpdateRouteOfAdministrationContactDrugXML(IList<RouteOfAdministrationContactDrug> lstInsert
                                                            , IList<RouteOfAdministrationContactDrug> lstDelete
                                                       , IList<RouteOfAdministrationContactDrug> lstUpdate)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InsertDeleteUpdateRouteOfAdministrationContactDrugXML(lstInsert, lstDelete, lstUpdate);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InsertDeleteUpdateRouteOfAdministrationContactDrugXML. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //public void InsertConIndicatorMedProductsRelToMedCondEx(IList<RefGenMedProductDetails> lstRefGenericDrugDetail,
        //                                                          long MCTypeID)
        //{
        //    try
        //    {
        //        RefDrugGenericDetailsProvider.Instance.InsertConIndicatorMedProductsRelToMedCondEx(lstRefGenericDrugDetail, MCTypeID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving InsertConIndicatorMedProductsRelToMedCondEx. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
        //    }
        //}
        public List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCond(IList<long> lstMCTpe,
                                                                                              long GenMedProductID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetContraIndicatorMedProductsRelToMedCond(lstMCTpe, GenMedProductID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetContraIndicatorMedProductsRelToMedCond. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        //▼====: #003
        public List<Bid> GetAllBids(long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllBids(V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllBids. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #003

        public List<Bid> GetInUsingBidCollectionFromSupplierID(long SupplierID, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetInUsingBidCollectionFromSupplierID(SupplierID, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInUsingBidCollectionFromSupplierID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public List<BidDetail> GetBidDetails(long BidID, string DrugCode, bool IsMedDept, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetBidDetails(BidID, DrugCode, IsMedDept, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetBidDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼====: #004
        public bool SaveBidDetails(long BidID, List<BidDetail> AddBidDetails, List<BidDetail> ModBidDetails, List<BidDetail> RemovedBidDetails
            , bool IsMedDept, long LoggedStaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SaveBidDetails(BidID, AddBidDetails, ModBidDetails, RemovedBidDetails, IsMedDept, LoggedStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveBidDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #004

        public bool RemoveBidDetails(long BidID, long DrugID, bool IsMedDept)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RemoveBidDetails(BidID, DrugID, IsMedDept);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RemoveBidDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼===== 25072018 TTM
        public bool RemoveBidDetails_New(long BidID, long DrugID, bool IsMedDept)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RemoveBidDetails_New(BidID, DrugID, IsMedDept);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RemoveBidDetails_New. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== 25072018 TTM
        public bool EditDrugBid(Bid aBid, out long? BidIDOut)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.EditDrugBid(aBid, out BidIDOut);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditDrugBid. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▲====: #001*/
        //20181219 TTM: Hàm chỉ lấy thuốc thuộc danh mục khoa dược nhưng chỉ lấy xài chung giữa khoa dược và kho BHYT - nhà thuốc.
        // 20200713 TNHX: Thêm điều kiệm tìm theo tên dùng chung
        public List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_ForHIStore(bool IsSearchByGenericName, bool? IsCode, string BrandName, long? StoreID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                long? RefGenDrugCatID_2 = null;
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_SearchAutoPaging_ForHIStore(IsSearchByGenericName, IsCode, BrandName, StoreID, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, out TotalCount, RefGenDrugCatID_2);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_SearchAutoPaging_ForHIStore. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_Choose(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, long? BidID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_SearchAutoPaging_Choose(IsCode, BrandName, SupplierID, V_MedProductType, RefGenDrugCatID_1, BidID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_SearchAutoPaging_Choose. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductDetails_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<BidDetail> GetRefGenMedProductDetails_ForBid(long BidID, string BrandName, bool IsMedDept, long V_MedProductType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetRefGenMedProductDetails_ForBid(BidID, BrandName, IsMedDept, V_MedProductType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetBidDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SaveListPCLExamTypeContactDrugs(List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Add, List<PCLExamTypeContactDrugs> ListPCLExamTypeContactDrug_Del, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SaveListPCLExamTypeContactDrugs(ListPCLExamTypeContactDrug_Add, ListPCLExamTypeContactDrug_Del, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SaveListPCLExamTypeContactDrugs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.GENMED_PRODUCT_RefGenMedProductSellingPrices_ByGenMedSellPriceID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

    }
}