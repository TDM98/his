using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace StorageProxy
{
    [ServiceContract]
    public interface IStorages
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefStorageWarehouseType_All(AsyncCallback callback, object state);
        List<RefStorageWarehouseType> EndGetRefStorageWarehouseType_All(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartment_ByDeptType(long DeptType, AsyncCallback callback, object state);
        List<RefDepartment> EndGetRefDepartment_ByDeptType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStorageByID(long storageID, AsyncCallback callback, object state);
        RefStorageWarehouseLocation EndGetStorageByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteStorageByID(long storageID, AsyncCallback callback, object state);
        int EndDeleteStorageByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateStorage(RefStorageWarehouseLocation storage, AsyncCallback callback, object state);
        int EndUpdateStorage(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewStorage(RefStorageWarehouseLocation storage, AsyncCallback callback, object state);
        int EndAddNewStorage(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStorages(int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        IList<RefStorageWarehouseLocation> EndGetAllStorages(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStoragesNotPaging(long? type, bool? bNo,long? DeptID, bool? IsNotSubStorage, AsyncCallback callback, object state);
        IList<RefStorageWarehouseLocation> EndGetAllStoragesNotPaging(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStorages_ForRespon(List<long> allListStoreID, long? type,
                                                                           bool? bNo, AsyncCallback callback, object state);
        IList<RefStorageWarehouseLocation> EndGetAllStorages_ForRespon(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchStorage(string StorageName, long V_MedProductType, AsyncCallback callback, object state);
        IList<RefStorageWarehouseLocation> EndSearchStorage(out int totalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept, AsyncCallback callback, object state);
        IList<GetDrugForSellVisitor> EndGetDrugForPrescription_Auto(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStoragesByStaffID(long StaffID, long? StoreTypeID, AsyncCallback callback, object state);
        IList<RefStorageWarehouseLocation> EndGetStoragesByStaffID(IAsyncResult asyncResult);
    }
}
