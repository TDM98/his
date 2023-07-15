using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace SupplierProxy
{
    [ServiceContract]
    public interface ISupplier
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplierByID(long supplierID, AsyncCallback callback, object state);
        Supplier EndGetSupplierByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteSupplierByID(long supplierID, AsyncCallback callback, object state);
        bool EndDeleteSupplierByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateSupplier(Supplier supplier, AsyncCallback callback, object state);
        bool EndUpdateSupplier(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchSupplierAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<Supplier> EndSearchSupplierAutoPaging(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewSupplier(Supplier newSupplier, AsyncCallback callback, object state);
        bool EndAddNewSupplier(out long SupplierID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplier_NotPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<Supplier> EndGetSupplier_NotPCOID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplier_ByPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<Supplier> EndGetSupplier_ByPCOID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplier_ByPCOIDNotPaging(long? PCOID, long V_SupplierType, AsyncCallback callback, object state);
        List<Supplier> EndGetSupplier_ByPCOIDNotPaging(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalSuppliers_Insert(PharmaceuticalSupplier S, AsyncCallback callback, object state);
        int EndPharmaceuticalSuppliers_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalSuppliers_Delete(PharmaceuticalSupplier S, AsyncCallback callback, object state);
        bool EndPharmaceuticalSuppliers_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalCompany_Insert(PharmaceuticalCompany Pharmaceatical, AsyncCallback callback, object state);
        int EndPharmaceuticalCompany_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalCompany_Update(PharmaceuticalCompany Pharmaceatical, AsyncCallback callback, object state);
        int EndPharmaceuticalCompany_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalCompany_Delete(long PCOID, AsyncCallback callback, object state);
        bool EndPharmaceuticalCompany_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<PharmaceuticalCompany> EndPharmaceuticalCompany_SearchPaging(out int TotalCount, IAsyncResult asyncResult);

        #region 0. Supplier member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllSupplierCbx(int supplierType, AsyncCallback callback, object state);
        IList<Supplier> EndGetAllSupplierCbx(IAsyncResult asyncResult);

        #endregion

        #region SupplierGenericDrugPrice

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_GetListSupplier_Paging(SupplierGenericDrugPriceSearchCriteria Criteria,
             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        List<Supplier> EndSupplierGenericDrugPrice_GetListSupplier_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_ListDrugBySupplierID_Paging(SupplierGenericDrugPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        List<SupplierGenericDrugPrice> EndSupplierGenericDrugPrice_ListDrugBySupplierID_Paging(out int Total, IAsyncResult asyncResult);


        //Quản lý giá
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_ListPrice_Paging(SupplierGenericDrugPriceSearchCriteria Criteria,

               int PageIndex,
               int PageSize,
               string OrderBy,
               bool CountTotal, AsyncCallback callback, object state);
        List<SupplierGenericDrugPrice> EndSupplierGenericDrugPrice_ListPrice_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_Save(SupplierGenericDrugPrice Obj, AsyncCallback callback, object state);
        void EndSupplierGenericDrugPrice_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_MarkDelete(Int64 PKID, AsyncCallback callback, object state);
        void EndSupplierGenericDrugPrice_MarkDelete(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_ByPKID(Int64 PKID, AsyncCallback callback, object state);
        SupplierGenericDrugPrice EndSupplierGenericDrugPrice_ByPKID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenericDrugPrice_XMLSave(IList<SupplierGenericDrugPrice> ObjCollect, AsyncCallback callback, object state);
        bool EndSupplierGenericDrugPrice_XMLSave(IAsyncResult asyncResult);
        #endregion

        #region SupplierGenMedProductsPrice

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_GetListSupplier_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria,
             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        List<Supplier> EndSupplierGenMedProductsPrice_GetListSupplier_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria,
             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        List<SupplierGenMedProductsPrice> EndSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(out int Total, IAsyncResult asyncResult);

        //Quản lý giá

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_ListPrice_Paging(SupplierGenMedProductsPriceSearchCriteria Criteria,
             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal, AsyncCallback callback, object state);
        List<SupplierGenMedProductsPrice> EndSupplierGenMedProductsPrice_ListPrice_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_Save(SupplierGenMedProductsPrice Obj, AsyncCallback callback, object state);
        void EndSupplierGenMedProductsPrice_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_MarkDelete(Int64 PKID, AsyncCallback callback, object state);
        void EndSupplierGenMedProductsPrice_MarkDelete(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSupplierGenMedProductsPrice_ByPKID(Int64 PKID, AsyncCallback callback, object state);
        SupplierGenMedProductsPrice EndSupplierGenMedProductsPrice_ByPKID(IAsyncResult asyncResult);

        //Quản lý giá
        #endregion

        #region  10.1 DrugDept Pharmaceutical member

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugDeptPharmaceuticalCompanyCbx(AsyncCallback callback, object state);
        List<DrugDeptPharmaceuticalCompany> EndGetDrugDeptPharmaceuticalCompanyCbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalCompany_Insert(DrugDeptPharmaceuticalCompany Pharmaceatical, AsyncCallback callback, object state);
        int EndDrugDeptPharmaceuticalCompany_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalCompany_Update(DrugDeptPharmaceuticalCompany Pharmaceatical, AsyncCallback callback, object state);
        int EndDrugDeptPharmaceuticalCompany_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalCompany_Delete(long PCOID, AsyncCallback callback, object state);
        bool EndDrugDeptPharmaceuticalCompany_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<DrugDeptPharmaceuticalCompany> EndDrugDeptPharmaceuticalCompany_SearchPaging(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplierDrugDept_NotPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndGetSupplierDrugDept_NotPCOID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplierDrugDept_ByPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndGetSupplierDrugDept_ByPCOID(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSupplierDrugDept_ByPCOIDNotPaging(long? PCOID, long V_SupplierType, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndGetSupplierDrugDept_ByPCOIDNotPaging(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalSuppliers_Insert(DrugDeptPharmaceuticalSupplier S, AsyncCallback callback, object state);
        int EndDrugDeptPharmaceuticalSuppliers_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptPharmaceuticalSuppliers_Delete(DrugDeptPharmaceuticalSupplier S, AsyncCallback callback, object state);
        bool EndDrugDeptPharmaceuticalSuppliers_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefGenMedProductDetails_ByPCOID(long PCOID, long? V_MedProductType, int PageIndex, int PageSize, bool bcount,  AsyncCallback callback, object state);
        List<RefGenMedProductDetails> EndRefGenMedProductDetails_ByPCOID(out int totalcount, IAsyncResult asyncResult);

        #endregion

        #region 27.4 DrugDept Supplier

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSupplier_SearchAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndDrugDeptSupplier_SearchAutoPaging(out int TotalCount, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSupplierXapNhapPXTemp_SearchPaging(SupplierSearchCriteria Criteria, int PageIndex, int PageSize, bool CountTotal, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndDrugDeptSupplierXapNhapPXTemp_SearchPaging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteDrugDeptSupplierByID(long supplierID, AsyncCallback callback, object state);
        bool EndDeleteDrugDeptSupplierByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrugDeptSupplier(DrugDeptSupplier supplier, AsyncCallback callback, object state);
        bool EndUpdateDrugDeptSupplier(out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewDrugDeptSupplier(DrugDeptSupplier supplier, AsyncCallback callback, object state);
        bool EndAddNewDrugDeptSupplier(out long SupplierID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDrugDeptSupplier_GetCbx(int supplierType, AsyncCallback callback, object state);
        List<DrugDeptSupplier> EndDrugDeptSupplier_GetCbx(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateDrugDeptSupplierPrice(List<SupplierGenMedProduct> ListSupplierProduct, long StaffID, long PriceChangeType, AsyncCallback callback, object state);
        bool EndUpdateDrugDeptSupplierPrice(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateSupplierPrice(List<SupplierGenericDrug> ListSupplierProduct, long StaffID, long PriceChangeType, AsyncCallback callback, object state);
        bool EndUpdateSupplierPrice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportExcelSupplier(long ExportFor, AsyncCallback callback, object state);
        List<List<string>> EndExportExcelSupplier(IAsyncResult asyncResult);

    }
}
