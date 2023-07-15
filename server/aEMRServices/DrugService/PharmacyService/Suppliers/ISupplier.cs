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
    public interface ISupplier
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Supplier GetSupplierByID(long supplierID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteSupplierByID(long supplierID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateSupplier(Supplier supplier,out string StrError);
       
     
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewSupplier(Supplier newSupplier,out long SupplierID,out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> SearchSupplierAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> GetSupplier_NotPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> GetSupplier_ByPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> GetSupplier_ByPCOIDNotPaging(long? PCOID, long V_SupplierType);
       

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int PharmaceuticalSuppliers_Insert(PharmaceuticalSupplier S);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmaceuticalSuppliers_Delete(PharmaceuticalSupplier S);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int PharmaceuticalCompany_Insert(PharmaceuticalCompany Pharmaceatical);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int PharmaceuticalCompany_Update(PharmaceuticalCompany Pharmaceatical);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PharmaceuticalCompany_Delete(long PCOID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PharmaceuticalCompany> PharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount);

        #region  10.1 DrugDept Pharmaceutical member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPharmaceuticalCompany> GetDrugDeptPharmaceuticalCompanyCbx();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DrugDeptPharmaceuticalCompany_Insert(DrugDeptPharmaceuticalCompany Pharmaceatical);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DrugDeptPharmaceuticalCompany_Update(DrugDeptPharmaceuticalCompany Pharmaceatical);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptPharmaceuticalCompany_Delete(long PCOID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> GetSupplierDrugDept_NotPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOIDNotPaging(long? PCOID, long V_SupplierType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DrugDeptPharmaceuticalSuppliers_Insert(DrugDeptPharmaceuticalSupplier S);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DrugDeptPharmaceuticalSuppliers_Delete(DrugDeptPharmaceuticalSupplier S);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefGenMedProductDetails> RefGenMedProductDetails_ByPCOID(long PCOID, long? V_MedProductType, int PageIndex, int PageSize, bool bcount, out int totalcount);

        #endregion

        #region 0. Supplier member

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Supplier> GetAllSupplierCbx(int supplierType);
        #endregion

        #region SupplierGenericDrugPrice
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> SupplierGenericDrugPrice_GetListSupplier_Paging(
           SupplierGenericDrugPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(
           SupplierGenericDrugPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );
        

        //Quản lý giá
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListPrice_Paging(
            SupplierGenericDrugPriceSearchCriteria Criteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SupplierGenericDrugPrice_Save(SupplierGenericDrugPrice Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SupplierGenericDrugPrice_MarkDelete(Int64 PKID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SupplierGenericDrugPrice SupplierGenericDrugPrice_ByPKID(Int64 PKID);
        //Quản lý giá

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SupplierGenericDrugPrice_XMLSave(IList<SupplierGenericDrugPrice> ObjCollect);

        #region SupplierGenMedProductsPrice
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> SupplierGenMedProductsPrice_GetListSupplier_Paging(
           SupplierGenMedProductsPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(
           SupplierGenMedProductsPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        //Quản lý giá
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListPrice_Paging(
            SupplierGenMedProductsPriceSearchCriteria Criteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SupplierGenMedProductsPrice_Save(SupplierGenMedProductsPrice Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SupplierGenMedProductsPrice_MarkDelete(Int64 PKID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        SupplierGenMedProductsPrice SupplierGenMedProductsPrice_ByPKID(Int64 PKID);
        //Quản lý giá
        #endregion



        #endregion

        #region 27.4 DrugDept Supplier

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> DrugDeptSupplier_SearchAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> DrugDeptSupplierXapNhapPXTemp_SearchPaging(SupplierSearchCriteria Criteria, int PageIndex, int PageSize, bool CountTotal, out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteDrugDeptSupplierByID(long supplierID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrugDeptSupplier(DrugDeptSupplier supplier, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewDrugDeptSupplier(DrugDeptSupplier supplier, out long SupplierID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DrugDeptSupplier> DrugDeptSupplier_GetCbx(int supplierType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateDrugDeptSupplierPrice(List<SupplierGenMedProduct> ListSupplierProduct, long StaffID, long PriceChangeType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateSupplierPrice(List<SupplierGenericDrug> ListSupplierProduct, long StaffID, long PriceChangeType);
        #endregion
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelSupplier(long ExportFor);

    }
}
