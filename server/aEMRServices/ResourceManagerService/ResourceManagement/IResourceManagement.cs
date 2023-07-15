/*
 * 29052018 TTM #001: Thêm parameter HIRepResourceCode.
 * 08062018 TTM #002
* 20230424 #003 DatTB: 
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi cách truyền biến một số function
* + Thêm function xuất excel thiết bị 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;

using ErrorLibrary;



namespace ResourcesManagementService
{
    
    [ServiceContract]
    public interface IResourcesManagementService
    {
        #region Common

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupRsrcUnit();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupResGroupCategory();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupSupplierType();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceGroup> GetAllResourceGroup();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceType> GetAllResourceType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceType> GetAllResourceTypeByGID(long RscrGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResource();
        //▼=====#002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResources();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID);
        //▲=====#002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal,out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResourceByChoice(int mChoice, long RscrID, string Text);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                ,long V_ResGroupCategory
                                              , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Resources> GetAllResourceByAllFilterPage(long GroupID
                                               , long TypeID
                                               , long SuplierID
                                               , string RsName
                                               , string RsBrand
                                               , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> GetAllSupplier();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResourceGroup(long RscrGroupID, string GroupName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteResourceGroup(long RscrGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceType(long RscrGroupID,string TypeName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResourceType(long RscrTypeID, long RscrGroupID, string TypeName, string Description);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteResourceType(long RscrTypeID);

        //▼=====#003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResources(Resources resource, string MServiceTypeListIDStr);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResources(Resources resource, string MServiceTypeListIDStr);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceType> GetMedicalServiceTypes_ByResourceGroup(long RscrGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelAllResources();
        //▲=====#003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteResources(long RscrID);

        #region Resources Allocations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupAllocStatus();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> GetLookupStorageStatus();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartment> GetAllRefDepartments();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Location> GetAllLocationsByType(int type);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartment> GetAllRefDepartmentsByType(int type);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Location> GetAllLocations();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RoomType> GetAllRoomType();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocation();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationByType(int choice);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<RefStaffCategory> GetRefStaffCategoriesByType(long V_StaffCatType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefStaffCategory> GetAllRefStaffCategories();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Staff> GetAllStaff(long StaffCatgID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Staff> GetAllStaffByStaffCategory(long StaffCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceAllocations> GetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceStorages> GetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceAllocations(long RscrID,string GuidID,long DeptLocationID
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
                                        , bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResourceAllocations(long RscrAllocID,long RscrID, string GuidID, long DeptLocationID
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
                                        , bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewStoragesAllocations(
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
                                    , bool IsDeleted);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResourceStorages(long RscrStorageID
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
                                                    , bool IsDeleted);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ResourceAllocationsTranferExec(int Choice
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
                                                               , int QtyInUseEx);

        #endregion
        #region New Resource Allocation
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceProperty(long RscrID
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
                                                       , long ResponsibleStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool MoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool BreakResourceProperty(List<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourcePropLocations> GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourcePropLocations> GetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourcePropLocations> GetResourcePropLocationsPagingByFilter(string RscrGUID 
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourcePropLocations> GetResourcePropLocationsPagingByMove(long RscrPropertyID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        long GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID);
        #endregion

        #endregion


        #region Resource Maintenance
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Staff> GetStaffs_All();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Supplier> GetSupplierIsMaintenance_All();        


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj, out long RscrMaintLogID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ResourceMaintenanceLog> GetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ResourceMaintenanceLogCanEdit(Int64 RscrMainLogStatusID);

        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        ResourceMaintenanceLog GetResourceMaintenanceLogByID(Int64 RscrMaintLogID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateResourceMaintenanceLog_FinalStatus(
            Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckRscrDiBaoTri(Int64 RscrPropertyID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ResourceMaintenanceLog_Delete(Int64 RscrMaintLogID);

        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool GetCheckForVerified(Int64 RscrMaintLogID);        


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteAndCreateNewResourceMaintenanceLog(long RscrMaintLogID_Delete, ResourceMaintenanceLog obj);






        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatus_Detail_Paging(
            ResourceMaintenanceLogStatusSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        DateTime GetFixDateLast(Int64 RscrMaintLogID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatusByID(long RscrMaintLogID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteResourceMaintenanceLogStatus(Int64 RscrMainLogStatusID);



        


        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Supplier> GetAllSupplierType(long V_SupplierType);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ResourceGroup> GetAllResourceGroupType(long V_ResGroupCategory);
        
    }

   
  
}
