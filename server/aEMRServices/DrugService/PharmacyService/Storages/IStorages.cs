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
    public interface IStorages
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefStorageWarehouseLocation GetStorageByID(long storageID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteStorageByID(long storageID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int UpdateStorage(RefStorageWarehouseLocation storage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int AddNewStorage(RefStorageWarehouseLocation storage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefStorageWarehouseLocation> GetAllStorages(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefStorageWarehouseLocation> GetAllStorages_ForRespon(List<long> allListStoreID, long? type,
                                                                           bool? bNo);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefStorageWarehouseLocation> GetAllStoragesNotPaging(long? type,bool? bNo,long? DeptID, bool? IsNotSubStorage);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefStorageWarehouseLocation> SearchStorage(string StorageName, long V_MedProductType, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefStorageWarehouseLocation> GetStoragesByStaffID(long StaffID, long? StoreTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefStorageWarehouseType> GetRefStorageWarehouseType_All();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartment> GetRefDepartment_ByDeptType(long DeptType);
   }
}
