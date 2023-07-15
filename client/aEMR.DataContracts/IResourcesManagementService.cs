using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using DataEntities;
using System.Collections.Generic;
using aEMR.DataContracts;
/*
* 20180529 TTM #001: Thêm parameter HIRepResourceCode
* 20180601 TTM #002: Thêm mới Hàm Begin & End cho GetAllResources, theo cấu trúc mới khi thêm Begin & End phải khai báo [FaultContractAttribute(typeof(AxException))]
*                    Thêm mới Hàm Begin & End cho GetResourcesForMedicalServices_LoadNotPaging
* 20230424 #003 DatTB: 
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi cách truyền biến một số function
* + Thêm function xuất excel thiết bị 
*/
namespace ResourcesManagementProxy
{
    [ServiceContract]
    public interface IResourcesManagementService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAuthenticateUser(string userName, string password, AsyncCallback callback,object state);
        UserAccount EndAuthenticateUser(out List<refModule> lstModules, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetLookupRsrcUnit(AsyncCallback callback,object state);
        IList<Lookup> EndGetLookupRsrcUnit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetLookupResGroupCategory(AsyncCallback callback,object state);
        List<Lookup> EndGetLookupResGroupCategory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetLookupSupplierType(AsyncCallback callback,object state);
        List<Lookup> EndGetLookupSupplierType(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceGroup(AsyncCallback callback,object state);
        List<ResourceGroup> EndGetAllResourceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceType(AsyncCallback callback, object state);
        List<ResourceType> EndGetAllResourceType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceTypeByGID(long RscrGroupID, AsyncCallback callback, object state);
        List<ResourceType> EndGetAllResourceTypeByGID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResource(AsyncCallback callback, object state);
        List<Resources> EndGetAllResource(IAsyncResult asyncResult);

        //▼=====#002
        //Thêm mới Hàm Begin & End cho GetAllResources, theo cấu trúc mới khi thêm Begin & End phải khai báo [FaultContractAttribute(typeof(AxException))]
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllResources(AsyncCallback callback, object state);
        List<Resources> EndGetAllResources(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID, AsyncCallback callback, object state);
        List<Resources> EndGetResourcesForMedicalServices_LoadNotPaging(IAsyncResult asyncResult);
        //▲=====#002

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback, object state);
        List<Resources> EndGetAllResource_GetAllPage(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceByChoice(int mChoice, long RscrID, string Text, AsyncCallback callback, object state);
        List<Resources> EndGetAllResourceByChoice(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                ,long V_ResGroupCategory
                                              , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , AsyncCallback callback, object state);
        List<Resources> EndGetAllResourceByChoicePaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceByAllFilterPage(long GroupID
                                               , long TypeID
                                               , long SuplierID
                                               , string RsName
                                               , string RsBrand
                                               , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               ,AsyncCallback callback,object state);
        List<Resources> EndGetAllResourceByAllFilterPage(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllSupplier(AsyncCallback callback, object state);
        List<Supplier> EndGetAllSupplier(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory, AsyncCallback callback, object state);
        bool EndAddNewResourceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdateResourceGroup(long RscrGroupID, string GroupName, string Description, AsyncCallback callback, object state);
        bool EndUpdateResourceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginDeleteResourceGroup(long RscrGroupID, AsyncCallback callback, object state);
        bool EndDeleteResourceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceType(long RscrGroupID, string TypeName, string Description, AsyncCallback callback, object state);
        bool EndAddNewResourceType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdateResourceType(long RscrTypeID, long RscrGroupID, string TypeName, string Description, AsyncCallback callback, object state);
        bool EndUpdateResourceType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginDeleteResourceType(long RscrTypeID, AsyncCallback callback, object state);
        bool EndDeleteResourceType(IAsyncResult asyncResult);
        
        //▼==== #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewResources(Resources resource, string MServiceTypeListIDStr, AsyncCallback callback, object state);
        bool EndAddNewResources(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateResources(Resources resource, string MServiceTypeListIDStr, AsyncCallback callback, object state);
        bool EndUpdateResources(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedicalServiceTypes_ByResourceGroup(long RscrGroupID, AsyncCallback callback, object state);
        List<RefMedicalServiceType> EndGetMedicalServiceTypes_ByResourceGroup(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginExportExcelAllResources(AsyncCallback callback, object state);
        List<List<string>> EndExportExcelAllResources(IAsyncResult asyncResult);
        //▲==== #003

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginDeleteResources(long RscrID, AsyncCallback callback, object state);
        bool EndDeleteResources(IAsyncResult asyncResult);

        #region Resources Allocations
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetLookupAllocStatus(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupAllocStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetLookupStorageStatus(AsyncCallback callback, object state);
        List<Lookup> EndGetLookupStorageStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllRefDepartments(AsyncCallback callback, object state);
        List<RefDepartment> EndGetAllRefDepartments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllLocationsByType(int type, AsyncCallback callback, object state);
        List<Location> EndGetAllLocationsByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllRefDepartmentsByType(int type, AsyncCallback callback, object state);
        List<RefDepartment> EndGetAllRefDepartmentsByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllLocations(AsyncCallback callback, object state);
        List<Location> EndGetAllLocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllRoomType(AsyncCallback callback, object state);
        List<RoomType> EndGetAllRoomType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllDeptLocation(AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllDeptLocationByType(int choice, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocationByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefStaffCategoriesByType(long V_StaffCatType, AsyncCallback callback, object state);
        List<RefStaffCategory> EndGetRefStaffCategoriesByType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllRefStaffCategories(AsyncCallback callback, object state);
        List<RefStaffCategory> EndGetAllRefStaffCategories(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllStaff(long StaffCatgID, AsyncCallback callback, object state);
        List<Staff> EndGetAllStaff(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllStaffByStaffCategory(long StaffCategory, AsyncCallback callback, object state);
        List<Staff> EndGetAllStaffByStaffCategory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllDeptLocationPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocationPage(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllPageDeptLocationByType(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback,object state);
        List<DeptLocation> EndGetAllPageDeptLocationGetByChoicePaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ResourceAllocations> EndGetResourceAllocationsPagingByDLID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ResourceStorages> EndGetResourceStoragesPagingByDLID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceAllocations(long RscrID, string GuidID, long DeptLocationID
                                        , long RscrMoveRequestID
                                        , long AllocStaffID
                                        , DateTime? AllocDate
                                        , DateTime? StartUseDate
                                        , long V_AllocStatus
                                        , bool HasIdentity
                                        , string RscrCode
                                        , string RscrBarcode
                                        , string SerialNumber
                                        , int QtyAlloc
                                        , int QtyInUse
                                        , long ResponsibleStaffID
                                        , bool IsActive, AsyncCallback callback,object state);
        bool EndAddNewResourceAllocations( IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdateResourceAllocations(long RscrAllocID, long RscrID, string GuidID, long DeptLocationID
                                        , long RscrMoveRequestID
                                        , long AllocStaffID
                                        , DateTime? AllocDate
                                        , DateTime? StartUseDate
                                        , long V_AllocStatus
                                        , bool HasIdentity
                                        , string RscrCode
                                        , string RscrBarcode
                                        , string SerialNumber
                                        , int QtyAlloc
                                        , int QtyInUse
                                        , long ResponsibleStaffID
                                        , bool IsActive, AsyncCallback callback,object state);
        bool EndUpdateResourceAllocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewStoragesAllocations(
                                        long RscrID, string GuidID
                                        , long DeptLocationID
                                        , long RscrMoveRequestID
                                        , long StorageStaffID
                                        , DateTime? StorageDate
                                        , long V_StorageStatus
                                        , long V_StorageReason
                                        , bool HasIdentity
                                        , string RscrCode
                                        , string RscrBarcode
                                        , string SerialNumber
                                        , int QtyStorage
                                        , long ResponsibleStaffID
                                        , bool IsActive
                                    , bool IsDeleted, AsyncCallback callback,object state);
        bool EndAddNewStoragesAllocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdateResourceStorages(long RscrStorageID
                                                    , long RscrID
                                                    , string RscrGUID
                                                    , long DeptLocationID
                                                    , DateTime RecDateCreated
                                                    , long RscrMoveRequestID
                                                    , long StorageStaffID
                                                    , DateTime? StorageDate
                                                    , long V_StorageStatus
                                                    , long V_StorageReason
                                                    , bool HasIdentity
                                                    , string RscrCode
                                                    , String RscrBarcode
                                                    , string SerialNumber
                                                    , int QtyStorage
                                                    , long ResponsibleStaffID
                                                    , bool IsActive
                                                    , bool IsDeleted, AsyncCallback callback,object state);
        bool EndUpdateResourceStorages(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginResourceAllocationsTranferExec(int Choice
                                                        , long RscrAllocID, string GuidID
                                                                , long RscrID
                                                               , long DeptLocationID
                                                               , long RscrMoveRequestID
                                                               , long AllocStaffID
                                                               , DateTime? AllocDate
                                                               , DateTime? StartUseDate
                                                               , long V_AllocStatus
                                                               , bool HasIdentity
                                                               , string RscrCode
                                                               , string RscrBarcode
                                                               , string SerialNumber
                                                                , int NewQtyAlloc
                                                                , int NewQtyInUse
                                                               , long ResponsibleStaffID
                                                               , bool IsActive
            ,long V_StorageReason 
                                                                , long RequestStaffID
                                                               , string MoveReason
                                                               , DateTime? EffectiveMoveDate
                                                               , long FromDeptLocID
                                                               , long ToDeptLocID
                                                               , long ApprovedStaffID
                                                               , string Note
                                                               

                                                               , int QtyAllocEx
                                                               , int QtyInUseEx, AsyncCallback callback,object state);
        bool EndResourceAllocationsTranferExec(IAsyncResult asyncResult);

        #endregion
        #region New Resource Allocation
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceProperty(long RscrID
                                                    , string RscrGUID
                                                    , bool HasIdentity
                                                    , string RscrCode
                                                    , string RscrBarcode
                                                    , string SerialNumber
                                                    , int QtyAlloc
                                                    , bool IsActive
                                                    , bool IsDeleted
            //location
                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID, AsyncCallback callback,object state);
        bool EndAddNewResourceProperty(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginMoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID, AsyncCallback callback,object state);
        bool EndMoveResourcePropertyLocation(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginBreakResourceProperty(ObservableCollection<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID, AsyncCallback callback, object state);
        bool EndBreakResourceProperty(IAsyncResult asyncResult);
        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ResourcePropLocations> EndGetResourcePropLocationsPagingByDLID(out int Total, IAsyncResult asyncResult);
        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ResourcePropLocations> EndGetResourcePropLocationsPagingByVType(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourcePropLocationsPagingByFilter(string RscrGUID 
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, AsyncCallback callback,object state);
        List<ResourcePropLocations> EndGetResourcePropLocationsPagingByFilter(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourcePropLocationsPagingByMove(long RscrPropertyID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback,object state);
        List<ResourcePropLocations> EndGetResourcePropLocationsPagingByMove(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID, AsyncCallback callback, object state);
        long EndGetResourcePropertyFilterSum(IAsyncResult asyncResult);
        #endregion

        

        #region Resource Maintenance
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetStaffs_All(AsyncCallback callback, object state);
        IList<Staff> EndGetStaffs_All(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetSupplierIsMaintenance_All(AsyncCallback callback, object state);
        IList<Supplier> EndGetSupplierIsMaintenance_All(IAsyncResult asyncResult);        


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceMaintenanceLog(ResourceMaintenanceLog obj,  AsyncCallback callback, object state);
        bool EndAddNewResourceMaintenanceLog(out long RscrMaintLogID, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback,object state);
        IList<ResourceMaintenanceLog> EndGetResourceMaintenanceLogSearch_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginResourceMaintenanceLogCanEdit(Int64 RscrMainLogStatusID, AsyncCallback callback, object state);
        bool EndResourceMaintenanceLogCanEdit(IAsyncResult asyncResult);

        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceMaintenanceLogByID(Int64 RscrMaintLogID, AsyncCallback callback, object state);
        ResourceMaintenanceLog EndGetResourceMaintenanceLogByID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginSaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            , AsyncCallback callback,object state);
        bool EndSaveResourceMaintenanceLogOfFix(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginUpdateResourceMaintenanceLog_FinalStatus(
            Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            , AsyncCallback callback,object state);
        bool EndUpdateResourceMaintenanceLog_FinalStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginCheckRscrDiBaoTri(Int64 RscrPropertyID, AsyncCallback callback, object state);
        bool EndCheckRscrDiBaoTri(IAsyncResult asyncResult);
        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginResourceMaintenanceLog_Delete(Int64 RscrMaintLogID, AsyncCallback callback, object state);
        bool EndResourceMaintenanceLog_Delete(IAsyncResult asyncResult);

        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetCheckForVerified(Int64 RscrMaintLogID, AsyncCallback callback, object state);
        bool EndGetCheckForVerified(IAsyncResult asyncResult);        


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginDeleteAndCreateNewResourceMaintenanceLog(long RscrMaintLogID_Delete, ResourceMaintenanceLog obj, AsyncCallback callback, object state);
        bool EndDeleteAndCreateNewResourceMaintenanceLog(IAsyncResult asyncResult);

        
        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceMaintenanceLogStatus_Detail_Paging(
            ResourceMaintenanceLogStatusSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback,object state);
        IList<ResourceMaintenanceLogStatus> EndGetResourceMaintenanceLogStatus_Detail_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetFixDateLast(Int64 RscrMaintLogID, AsyncCallback callback, object state);
        DateTime EndGetFixDateLast(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj, AsyncCallback callback, object state);
        bool EndAddNewResourceMaintenanceLogStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginAddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj, AsyncCallback callback, object state);
        bool EndAddResourceMaintenanceLogStatus_New(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetResourceMaintenanceLogStatusByID(long RscrMaintLogID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal,  AsyncCallback callback,object state);
        List<ResourceMaintenanceLogStatus> EndGetResourceMaintenanceLogStatusByID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginDeleteResourceMaintenanceLogStatus(Int64 RscrMainLogStatusID, AsyncCallback callback, object state);
        bool EndDeleteResourceMaintenanceLogStatus(IAsyncResult asyncResult);

        


        #endregion

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllSupplierType(long V_SupplierType, AsyncCallback callback, object state);
        List<Supplier> EndGetAllSupplierType(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern=true)]
        IAsyncResult BeginGetAllResourceGroupType(long V_ResGroupCategory, AsyncCallback callback, object state);
        List<ResourceGroup> EndGetAllResourceGroupType(IAsyncResult asyncResult);
    }
}
