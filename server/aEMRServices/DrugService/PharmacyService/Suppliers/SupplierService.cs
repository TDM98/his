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
/*
 * 20190911 #001 TTM:   BM 0013236: Cho phép sửa giá lẻ nhà cung cấp tại màn hình quản lý nhà cung cấp. Để phục vụ sửa giá hàng loạt cho nhà cung cấp.
 */
namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class SupplierService : eHCMS.WCFServiceCustomHeader, ISupplier
    {
        //private string _ModuleName = "Supplier Service";
        public SupplierService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public Supplier GetSupplierByID(long supplierID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplierByID(supplierID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplierByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SUPPLIER_CANNOT_GET);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteSupplierByID(long supplierID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteSupplierByID(supplierID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteSupplierByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SUPPLIER_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateSupplier(Supplier supplier, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateSupplier(supplier, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateSupplier. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SUPPLIER_CANNOT_UPDATE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Supplier> SearchSupplierAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SearchSupplierAutoPaging(Criteria, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchSupplierAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_DRUGCLASS_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewSupplier(Supplier newSupplier, out long SupplierID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewSupplier(newSupplier, out SupplierID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllSuppliers. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_SUPPLIER_CANNOT_ADD);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Supplier> GetAllSupplierCbx(int supplierType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllSupplierCbx(supplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllSupplierCbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetAllSupplierCbx);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Supplier> SupplierGenericDrugPrice_GetListSupplier_Paging(SupplierGenericDrugPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_GetListSupplier_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_GetListSupplier_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_GetListSupplier_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(SupplierGenericDrugPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_ListDrugBySupplierID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_ListDrugBySupplierID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Supplier> GetSupplier_NotPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplier_NotPCOID(PCOID, V_SupplierType, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplier_NotPCOID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplier_NotPCOID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Supplier> GetSupplier_ByPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplier_ByPCOID(PCOID, V_SupplierType, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplier_ByPCOID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplier_ByPCOID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Supplier> GetSupplier_ByPCOIDNotPaging(long? PCOID, long V_SupplierType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplier_ByPCOIDNotPaging(PCOID, V_SupplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplier_ByPCOIDNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplier_ByPCOIDNotPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int PharmaceuticalSuppliers_Insert(PharmaceuticalSupplier S)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalSuppliers_Insert(S);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalSuppliers_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalSuppliers_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool PharmaceuticalSuppliers_Delete(PharmaceuticalSupplier S)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalSuppliers_Delete(S);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalSuppliers_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalSuppliers_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int PharmaceuticalCompany_Insert(PharmaceuticalCompany Pharmaceatical)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalCompany_Insert(Pharmaceatical);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalCompany_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalCompany_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public int PharmaceuticalCompany_Update(PharmaceuticalCompany Pharmaceatical)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalCompany_Update(Pharmaceatical);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalCompany_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalCompany_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool PharmaceuticalCompany_Delete(long PCOID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalCompany_Delete(PCOID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalCompany_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalCompany_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PharmaceuticalCompany> PharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.PharmaceuticalCompany_SearchPaging(PCOName, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PharmaceuticalCompany_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_PharmaceuticalCompany_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Quản lý giá
        public List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListPrice_Paging(SupplierGenericDrugPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_ListPrice_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_ListPrice_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_ListPrice_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SupplierGenericDrugPrice_Save(SupplierGenericDrugPrice Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SupplierGenericDrugPrice_MarkDelete(long PKID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_MarkDelete(PKID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public SupplierGenericDrugPrice SupplierGenericDrugPrice_ByPKID(long PKID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_ByPKID(PKID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_ByPKID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_ByPKID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Quản lý giá


        public bool SupplierGenericDrugPrice_XMLSave(IList<SupplierGenericDrugPrice> ObjCollect)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenericDrugPrice_XMLSave(ObjCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenericDrugPrice_XMLSave. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenericDrugPrice_XMLSave);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<Supplier> SupplierGenMedProductsPrice_GetListSupplier_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_GetListSupplier_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_GetListSupplier_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_GetListSupplier_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Quản lý giá
        public List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListPrice_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_ListPrice_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_ListPrice_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_ListPrice_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SupplierGenMedProductsPrice_Save(SupplierGenMedProductsPrice Obj, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SupplierGenMedProductsPrice_MarkDelete(long PKID, out string Result)
        {
            try
            {
                RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_MarkDelete(PKID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public SupplierGenMedProductsPrice SupplierGenMedProductsPrice_ByPKID(long PKID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.SupplierGenMedProductsPrice_ByPKID(PKID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SupplierGenMedProductsPrice_ByPKID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_SupplierGenMedProductsPrice_ByPKID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Quản lý giá

        #region  10.1 DrugDept Pharmaceutical member

        public List<DrugDeptPharmaceuticalCompany> GetDrugDeptPharmaceuticalCompanyCbx()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetDrugDeptPharmaceuticalCompanyCbx();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetDrugDeptPharmaceuticalCompanyCbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetDrugDeptPharmaceuticalCompanyCbx);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DrugDeptPharmaceuticalCompany_Insert(DrugDeptPharmaceuticalCompany Pharmaceatical)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalCompany_Insert(Pharmaceatical);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalCompany_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalCompany_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DrugDeptPharmaceuticalCompany_Update(DrugDeptPharmaceuticalCompany Pharmaceatical)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalCompany_Update(Pharmaceatical);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalCompany_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalCompany_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptPharmaceuticalCompany_Delete(long PCOID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalCompany_Delete(PCOID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalCompany_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalCompany_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalCompany_SearchPaging(PCOName, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalCompany_SearchPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalCompany_SearchPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSupplier> GetSupplierDrugDept_NotPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplierDrugDept_NotPCOID(SearchText, PCOID, V_SupplierType, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplierDrugDept_NotPCOID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplierDrugDept_NotPCOID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplierDrugDept_ByPCOID(SearchText, PCOID, V_SupplierType, PageSize, PageIndex, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplierDrugDept_ByPCOID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplierDrugDept_ByPCOID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOIDNotPaging(long? PCOID, long V_SupplierType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetSupplierDrugDept_ByPCOIDNotPaging(PCOID, V_SupplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetSupplierDrugDept_ByPCOIDNotPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_GetSupplierDrugDept_ByPCOIDNotPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DrugDeptPharmaceuticalSuppliers_Insert(DrugDeptPharmaceuticalSupplier S)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalSuppliers_Insert(S);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalSuppliers_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalSuppliers_Insert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DrugDeptPharmaceuticalSuppliers_Delete(DrugDeptPharmaceuticalSupplier S)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptPharmaceuticalSuppliers_Delete(S);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptPharmaceuticalSuppliers_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalSuppliers_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenMedProductDetails> RefGenMedProductDetails_ByPCOID(long PCOID, long? V_MedProductType, int PageIndex, int PageSize, bool bcount, out int totalcount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.RefGenMedProductDetails_ByPCOID(PCOID, V_MedProductType, PageIndex, PageSize, bcount, out totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving RefGenMedProductDetails_ByPCOID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptPharmaceuticalSuppliers_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region 27.4 DrugDept Supplier

        public List<DrugDeptSupplier> DrugDeptSupplier_SearchAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSupplier_SearchAutoPaging(Criteria, PageSize, PageIndex, bcount, out TotalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptSupplier_SearchAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptSupplier_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<DrugDeptSupplier> DrugDeptSupplierXapNhapPXTemp_SearchPaging(SupplierSearchCriteria Criteria, int PageIndex, int PageSize, bool CountTotal, out int Total)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSupplierXapNhapPXTemp_SearchPaging(Criteria, PageIndex, PageSize, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptSupplier_SearchAutoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptSupplier_SearchAutoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool DeleteDrugDeptSupplierByID(long supplierID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteDrugDeptSupplierByID(supplierID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteDrugDeptSupplierByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DeleteDrugDeptSupplierByID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateDrugDeptSupplier(DrugDeptSupplier supplier, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateDrugDeptSupplier(supplier, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDrugDeptSupplier. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_UpdateDrugDeptSupplier);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewDrugDeptSupplier(DrugDeptSupplier supplier, out long SupplierID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddNewDrugDeptSupplier(supplier, out SupplierID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddNewDrugDeptSupplier. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_AddNewDrugDeptSupplier);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DrugDeptSupplier> DrugDeptSupplier_GetCbx(int supplierType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DrugDeptSupplier_GetCbx(supplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DrugDeptSupplier_GetCbx. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptSupplier_GetCbx);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //▼===== #001
        public bool UpdateDrugDeptSupplierPrice(List<SupplierGenMedProduct> ListSupplierProduct, long StaffID, long PriceChangeType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateDrugDeptSupplierPrice(ListSupplierProduct, StaffID, PriceChangeType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateDrugDeptSupplierPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_UpdateDrugDeptSupplier);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.ToString()));
            }
        }
        public bool UpdateSupplierPrice(List<SupplierGenericDrug> ListSupplierProduct, long StaffID, long PriceChangeType)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateSupplierPrice(ListSupplierProduct, StaffID, PriceChangeType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateSupplierPrice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_UpdateDrugDeptSupplier);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.ToString()));
            }
        }
        //▲===== #001

        public List<List<string>> ExportExcelSupplier(long ExportFor)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ExportExcelSupplier(ExportFor);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ExportExcelSupplier. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.SUPPLIER_DrugDeptSupplier_GetCbx);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
