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
using System.ServiceModel;
using ErrorLibrary;
using System.ServiceModel.Activation;
using AxLogging;
using DataEntities;
using ErrorLibrary.Resources;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace ResourcesManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [ServiceKnownType(typeof(AxException))]
    public class ResourcesManagementService : eHCMS.WCFServiceCustomHeader, IResourcesManagementService
    {
        public ResourcesManagementService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        public IList<Lookup> GetLookupRsrcUnit()
        {

            try
            {
                return ResourcesManagement.Instance.GetLookupRsrcUnit();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupRsrcUnit. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_LOOKUP_RSRCUNIT);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<Lookup> GetLookupResGroupCategory()
        {

            try
            {
                return ResourcesManagement.Instance.GetLookupResGroupCategory();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupResGroupCategory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_LOOKUP_RESGROUPCATEGORY);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }


        public List<Lookup> GetLookupSupplierType()
        {

            try
            {
                return ResourcesManagement.Instance.GetLookupSupplierType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupSupplierType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_LOOKUP_SUPPLIERTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<ResourceGroup> GetAllResourceGroup()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllResourceGroup();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResourceGroup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCEGROUP);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<ResourceType> GetAllResourceType()
        {

            try
            {
                return ResourcesManagement.Instance.GetAllResourceType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResourceType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCETYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<ResourceType> GetAllResourceTypeByGID(long RscrGroupID)
        {

            try
            {
                return ResourcesManagement.Instance.GetAllResourceTypeByGID(RscrGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResourceTypeByGID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCETYPEBYGID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<Resources> GetAllResource()
        {

            try
            {
                return ResourcesManagement.Instance.GetAllResource();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResource. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        //▼=====#002
        public List<Resources> GetAllResources()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllResources();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResources. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<Resources> GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcesForMedicalServices_LoadNotPaging(PCLExamTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForMedicalServices_LoadNotPaging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲=====#002
        public List<Resources> GetAllResourceByChoice(int mChoice, long RscrID, string Text)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllResourceByChoice(mChoice, RscrID, Text);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResourceByChoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCEBYCHOICE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<Resources> GetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllResource_GetAllPage(PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResource_GetAllPage. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE_GET_ALL_PAGE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public List<Resources> GetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                , long V_ResGroupCategory
                                              , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total)
        {
            {
                try
                {
                    return ResourcesManagement.Instance.GetAllResourceByChoicePaging(mChoice
                                                , RscrID
                                                , Text
                                                , V_ResGroupCategory
                                                , PageSize
                                                 , PageIndex
                                                 , OrderBy
                                                 , CountTotal
                                                 , out Total);

                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of GetAllResourceByChoicePaging. Status: Failed.", CurrentUser);
                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE_BYCHOICEPAGING);
                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }


            }
        }
        public List<Resources> GetAllResourceByAllFilterPage(long GroupID
                                               , long TypeID
                                               , long SuplierID
                                               , string RsName
                                               , string RsBrand
                                               , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total)
        {
            {
                try
                {
                    return ResourcesManagement.Instance.GetAllResourceByAllFilterPage(GroupID, TypeID, SuplierID, RsName
                                                                                    , RsBrand, PageSize, PageIndex, OrderBy, CountTotal, out Total);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("End of GetAllResourceByAllFilterPage. Status: Failed.", CurrentUser);
                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE_BYALLFILTERPAGE);
                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }


            }
        }
        public List<Supplier> GetAllSupplier()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllSupplier();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllSupplier. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_SUPPLIER);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public bool AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewResourceGroup(GroupName, Description, V_ResGroupCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceGroup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCEGROUP);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool UpdateResourceGroup(long RscrGroupID, string GroupName, string Description)
        {

            try
            {
                return ResourcesManagement.Instance.UpdateResourceGroup(RscrGroupID, GroupName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResourceGroup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCEGROUP);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool DeleteResourceGroup(long RscrGroupID)
        {

            try
            {
                return ResourcesManagement.Instance.DeleteResourceGroup(RscrGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteResourceGroup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_DELETE_RESOURCEGROUP);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool AddNewResourceType(long RscrGroupID, string TypeName, string Description)
        {

            try
            {
                return ResourcesManagement.Instance.AddNewResourceType(RscrGroupID, TypeName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCETYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool UpdateResourceType(long RscrTypeID, long RscrGroupID, string TypeName, string Description)
        {

            try
            {
                return ResourcesManagement.Instance.UpdateResourceType(RscrTypeID, RscrGroupID, TypeName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResourceType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCETYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool DeleteResourceType(long RscrTypeID)
        {
            try
            {
                return ResourcesManagement.Instance.DeleteResourceType(RscrTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteResourceType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_DELETE_RESOURCETYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        //▼=====#003
        public bool AddNewResources(Resources resource, string MServiceTypeListIDStr)
        {

            try
            {
                return ResourcesManagement.Instance.AddNewResources(resource, MServiceTypeListIDStr);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResources. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool UpdateResources(Resources resource, string MServiceTypeListIDStr)
        {

            try
            {
                return ResourcesManagement.Instance.UpdateResources(resource, MServiceTypeListIDStr);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResources. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        
        public List<RefMedicalServiceType> GetMedicalServiceTypes_ByResourceGroup(long RscrGroupID)
        {

            try
            {
                return ResourcesManagement.Instance.GetMedicalServiceTypes_ByResourceGroup(RscrGroupID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedicalServiceTypes_ByResourceGroup. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCETYPEBYGID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        
        public List<List<string>> ExportExcelAllResources()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ExportExcelAllResources.", CurrentUser);
                List<List<string>> bRet = ResourcesManagement.Instance.ExportExcelAllResources();
                AxLogger.Instance.LogInfo("End loading ExportExcelAllResources.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ExportExcelAllResources. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.HI_REPORT_CANNOT_CREATE, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲=====#003
        public bool DeleteResources(long RscrID)
        {
            try
            {
                return ResourcesManagement.Instance.DeleteResources(RscrID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteResources. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_DELETE_RESOURCES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }

        #region Resources Allocations
        public IList<Lookup> GetLookupAllocStatus()
        {

            try
            {
                return ResourcesManagement.Instance.GetLookupAllocStatus();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupAllocStatus. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_LOOKUP_ALLOC_STATUS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<Lookup> GetLookupStorageStatus()
        {
            try
            {
                return ResourcesManagement.Instance.GetLookupStorageStatus();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLookupStorageStatus. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_LOOKUP_STORAGESTATUS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<RefDepartment> GetAllRefDepartments()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllRefDepartments();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefDepartments. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_REFDEPARTMENTS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<Location> GetAllLocationsByType(int type)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllLocationsByType(type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllLocationsByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_LOCATIONSBYTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<RefDepartment> GetAllRefDepartmentsByType(int type)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllRefDepartmentsByType(type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefDepartmentsByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_REFDEPARTMENTSBYTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<Location> GetAllLocations()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllLocations();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllLocations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_LOCATIONS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<RoomType> GetAllRoomType()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllRoomType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRoomType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_ROOMTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<DeptLocation> GetAllDeptLocation()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllDeptLocation();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_DEPTLOCATION);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<DeptLocation> GetAllDeptLocationByType(int choice)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllDeptLocationByType(choice);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocationByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_DEPTLOCATIONBYTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<RefStaffCategory> GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            try
            {
                return ResourcesManagement.Instance.GetRefStaffCategoriesByType(V_StaffCatType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefStaffCategoriesByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_REFSTAFFCATEGORIESBYTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<RefStaffCategory> GetAllRefStaffCategories()
        {
            try
            {
                return ResourcesManagement.Instance.GetAllRefStaffCategories();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefStaffCategories. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_REFSTAFFCATEGORIES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<Staff> GetAllStaffByStaffCategory(long StaffCategory)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllStaffByStaffCategory(StaffCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllStaffByStaffCategory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_STAFFBYSTAFFCATEGORY);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<Staff> GetAllStaff(long refStaffCateID)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllStaff(refStaffCateID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllStaff. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_STAFF);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<DeptLocation> GetAllDeptLocationPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllPageDeptLocation(PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPageDeptLocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_PAGEDEPTLOCATION);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public List<DeptLocation> GetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllPageDeptLocationByType(choice, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPageDeptLocationByType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_PAGEDEPTLOCATION_BY_TYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public List<DeptLocation> GetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllPageDeptLocationGetByChoicePaging(Choice, RsID, Text, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPageDeptLocationGetByChoicePaging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_PAGEDEPTLOCATIONGET_BY_CHOICEPAGING);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public List<ResourceAllocations> GetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourceAllocationsPagingByDLID(DeptLocationID, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourceAllocationsPagingByDLID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_ALLOCATIONS_PAGINGBYDLID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public List<ResourceStorages> GetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourceStoragesPagingByDLID(DeptLocationID, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourceStoragesPagingByDLID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_STORAGES_PAGINGBYDLID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        public bool AddNewResourceAllocations(long RscrID, string GuidID, long DeptLocationID
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
                                        , bool IsActive)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewResourceAllocations(RscrID, GuidID, DeptLocationID
                                    , RscrMoveRequestID
                                    , AllocStaffID
                                    , AllocDate
                                    , StartUseDate
                                    , V_AllocStatus
                                    , HasIdentity
                                    , RscrCode
                                    , RscrBarcode
                                    , SerialNumber
                                    , QtyAlloc
                                    , QtyInUse
                                    , ResponsibleStaffID
                                    , IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceAllocations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCE_ALLOCATIONS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool UpdateResourceAllocations(long RscrAllocID, long RscrID, string GuidID, long DeptLocationID
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
                                       , bool IsActive)
        {
            try
            {
                return ResourcesManagement.Instance.UpdateResourceAllocations(RscrAllocID, RscrID, GuidID, DeptLocationID
                                    , RscrMoveRequestID
                                    , AllocStaffID
                                    , AllocDate
                                    , StartUseDate
                                    , V_AllocStatus
                                    , HasIdentity
                                    , RscrCode
                                    , RscrBarcode
                                    , SerialNumber
                                    , QtyAlloc
                                    , QtyInUse
                                    , ResponsibleStaffID
                                    , IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResourceAllocations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCE_ALLOCATIONS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool AddNewStoragesAllocations(long RscrID, string GuidID
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
                                                   //,int QtyInUse 
                                                   , long ResponsibleStaffID
                                                   , bool IsActive
                                                , bool IsDeleted)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewStoragesAllocations(RscrID, GuidID
                                                    , DeptLocationID
                                                   , RscrMoveRequestID
                                                   , StorageStaffID
                                                   , StorageDate
                                                    , V_StorageStatus
                                                    , V_StorageReason
                                                   , HasIdentity
                                                   , RscrCode
                                                   , RscrBarcode
                                                   , SerialNumber
                                                   , QtyStorage
                                                   , ResponsibleStaffID
                                                   , IsActive
                                                    , IsDeleted);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewStoragesAllocations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_STORAGES_ALLOCATIONS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool UpdateResourceStorages(long RscrStorageID
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
                                                    , bool IsDeleted)

        {
            try
            {
                return ResourcesManagement.Instance.UpdateResourceStorages(RscrStorageID
                                                , RscrID
                                                , RscrGUID
                                                , DeptLocationID
                                                , RecDateCreated
                                                , RscrMoveRequestID
                                                , StorageStaffID
                                                , StorageDate
                                                , V_StorageStatus
                                                , V_StorageReason
                                                , HasIdentity
                                                , RscrCode
                                                , RscrBarcode
                                                , SerialNumber
                                                , QtyStorage
                                                , ResponsibleStaffID
                                                , IsActive
                                                , IsDeleted);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResourceStorages. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCE_STORAGES);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool ResourceAllocationsTranferExec(int Choice
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
                                                                , long V_StorageReason
                                                                , long RequestStaffID
                                                               , string MoveReason
                                                               , DateTime? EffectiveMoveDate
                                                               , long FromDeptLocID
                                                               , long ToDeptLocID
                                                               , long ApprovedStaffID
                                                               , string Note
                                                               , int QtyAllocEx
                                                               , int QtyInUseEx
                                                               )
        {
            try
            {
                return ResourcesManagement.Instance.ResourceAllocationsTranferExec(Choice
                                                                        , RscrAllocID, GuidID
                                                                        , RscrID
                                                                        , DeptLocationID
                                                                        , RscrMoveRequestID
                                                                        , AllocStaffID
                                                                        , AllocDate
                                                                        , StartUseDate
                                                                        , V_AllocStatus
                                                                        , HasIdentity
                                                                        , RscrCode
                                                                        , RscrBarcode
                                                                        , SerialNumber
                                                                        , NewQtyAlloc
                                                                        , NewQtyInUse
                                                                        , ResponsibleStaffID
                                                                        , IsActive
                                                                        , V_StorageReason
                                                                        , RequestStaffID
                                                                        , MoveReason
                                                                        , EffectiveMoveDate
                                                                        , FromDeptLocID
                                                                        , ToDeptLocID
                                                                        , ApprovedStaffID
                                                                        , Note
                                                                        , QtyAllocEx
                                                                        , QtyInUseEx
                                                                        );
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ResourceAllocationsTranferExec. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_RESOURCE_ALLOCATION_STRANFEREXEC);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        #endregion

        #region New Resource Allocation
        public bool AddNewResourceProperty(long RscrID
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
                                                       , long ResponsibleStaffID)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewResourceProperty(RscrID
                                                , RscrGUID
                                                , HasIdentity
                                                , RscrCode
                                                , RscrBarcode
                                                , SerialNumber
                                                , QtyAlloc
                                                , IsActive
                                                , IsDeleted
                                                   //location
                                                   , DeptLocationID
                                                   , RscrMoveRequestID
                                                   , AllocStaffID
                                                   , AllocDate
                                                   , StartUseDate
                                                   , V_AllocStatus
                                                    , V_StorageReason
                                                   , ResponsibleStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceProperty. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCE_PROPERTY);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool MoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID)
        {
            try
            {
                return ResourcesManagement.Instance.MoveResourcePropertyLocation(RscrPropLocID
                                                , RscrPropertyID

                                                   , DeptLocationID
                                                   , RscrMoveRequestID
                                                   , AllocStaffID
                                                   , AllocDate
                                                   , StartUseDate
                                                   , V_AllocStatus
                                                    , V_StorageReason
                                                   , ResponsibleStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MoveResourcePropertyLocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_MOVE_RESOURCE_PROPERTYLOCATION);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public bool BreakResourceProperty(List<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID)
        {
            try
            {
                return ResourcesManagement.Instance.BreakResourceProperty(lstResourcePropLocations, RscrPropertyID, RscrPropLocID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of BreakResourceProperty. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_BREAK_RESOURCEPROPERTY);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }
        public List<ResourcePropLocations> GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcePropLocationsPagingByDLID(DeptLocationID, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcePropLocationsPagingByDLID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_PROPLOCATIONS_PAGINGBYDLID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }


        }
        public List<ResourcePropLocations> GetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcePropLocationsPagingByVType(Choice, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcePropLocationsPagingByVType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_PROPLOCATIONS_PAGINGBYVTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }


        }
        public List<ResourcePropLocations> GetResourcePropLocationsPagingByFilter(string RscrGUID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcePropLocationsPagingByFilter(0, RscrGUID, 0, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcePropLocationsPagingByFilter. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_PROPLOCATIONS_PAGINGBYFILTER);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }


        }
        public List<ResourcePropLocations> GetResourcePropLocationsPagingByMove(long RscrPropertyID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcePropLocationsPagingByFilter(1, "", RscrPropertyID, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcePropLocationsPagingByFilter. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_PROPLOCATIONS_PAGINGBYFILTER);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }


        }

        public long GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourcePropertyFilterSum(Choice, RscrGUID, RscrID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcePropertyFilterSum. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_PROPERTY_FILTERSUM);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        #endregion

        #region Resource Maintenance
        public IList<Staff> GetStaffs_All()
        {
            try
            {
                return ResourcesManagement.Instance.GetStaffs_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetStaffs_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_STAFFS_ALL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj, out long RscrMaintLogID)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewResourceMaintenanceLog(obj, out RscrMaintLogID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceMaintenanceLog. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCE_MAINTENANCE_LOG);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj)
        {
            try
            {
                return ResourcesManagement.Instance.AddResourceMaintenanceLogStatus_New(obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddResourceMaintenanceLogStatus_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADD_RESOURCE_MAINTENANCE_LOGSTATUS_NEW);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<ResourceMaintenanceLog> GetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourceMaintenanceLogSearch_Paging(
                    Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourceMaintenanceLogSearch_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_MAINTENANCE_LOGSEARCH_PAGING);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatus_Detail_Paging(ResourceMaintenanceLogStatusSearchCriteria Criteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourceMaintenanceLogStatus_Detail_Paging(
             Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourceMaintenanceLogStatus_Detail_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_MAINTENANCE_LOGSTATUS_DETAIL_PAGING);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public DateTime GetFixDateLast(long RscrMaintLogID)
        {
            try
            {
                return ResourcesManagement.Instance.GetFixDateLast(RscrMaintLogID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFixDateLast. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_FIXDATELAST);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool AddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj)
        {
            try
            {
                return ResourcesManagement.Instance.AddNewResourceMaintenanceLogStatus(obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewResourceMaintenanceLogStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_ADDNEW_RESOURCE_MAINTENANCE_LOGSTATUS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatusByID(long RscrMaintLogID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ResourcesManagement.Instance.GetspResourceMaintenanceLogStatusByID(RscrMaintLogID
                                                        , PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetspResourceMaintenanceLogStatusByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_SPRESOURCE_MAINTENANCE_LOGSTATUSBYID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }


        }
        #endregion

        #region IResourcesManagementService Members


        public bool DeleteResourceMaintenanceLogStatus(long RscrMainLogStatusID)
        {
            try
            {
                return ResourcesManagement.Instance.DeleteResourceMaintenanceLogStatus(RscrMainLogStatusID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteResourceMaintenanceLogStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_DELETE_RESOURCE_MAINTENANCE_LOGSTATUS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members

        public IList<Supplier> GetSupplierIsMaintenance_All()
        {
            try
            {
                return ResourcesManagement.Instance.GetSupplierIsMaintenance_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSupplierIsMaintenance_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_SUPPLIER_IS_MAINTENANCE_ALL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool ResourceMaintenanceLogCanEdit(long RscrMainLogStatusID)
        {
            try
            {
                return ResourcesManagement.Instance.ResourceMaintenanceLogCanEdit(RscrMainLogStatusID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ResourceMaintenanceLogCanEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_RESOURCE_MAINTENANCE_LOGCANEDIT);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region IResourcesManagementService Members


        public ResourceMaintenanceLog GetResourceMaintenanceLogByID(long RscrMaintLogID)
        {
            try
            {
                return ResourcesManagement.Instance.GetResourceMaintenanceLogByID(RscrMaintLogID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourceMaintenanceLogByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_RESOURCE_MAINTENANCE_LOGBYID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        #region IResourcesManagementService Members


        public bool SaveResourceMaintenanceLogOfFix(long RscrMaintLogID, Nullable<Int64> FixStaffID, Nullable<Int64> FixSupplierID, DateTime FixDate, string FixSolutions, string FixComments, long VerifiedStaffID, long V_CurrentStatus)
        {
            try
            {
                return ResourcesManagement.Instance.SaveResourceMaintenanceLogOfFix(
RscrMaintLogID, FixStaffID, FixSupplierID, FixDate, FixSolutions, FixComments, VerifiedStaffID, V_CurrentStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveResourceMaintenanceLogOfFix. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_SAVE_RESOURCE_MAINTENANCE_LOGOFFIX);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool UpdateResourceMaintenanceLog_FinalStatus(long RscrMaintLogID, long VerifiedStaffID, long V_CurrentStatus)
        {
            try
            {
                return ResourcesManagement.Instance.UpdateResourceMaintenanceLog_FinalStatus(
RscrMaintLogID, VerifiedStaffID, V_CurrentStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateResourceMaintenanceLog_FinalStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_UPDATE_RESOURCE_MAINTENANCE_LOG_FINALSTATUS);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool DeleteAndCreateNewResourceMaintenanceLog(long RscrMaintLogID_Delete, ResourceMaintenanceLog obj)
        {
            try
            {
                return ResourcesManagement.Instance.DeleteAndCreateNewResourceMaintenanceLog(
                 RscrMaintLogID_Delete, obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteAndCreateNewResourceMaintenanceLog. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_DELETE_AND_CREATENEW_RESOURCE_MAINTENANCE_LOG);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool CheckRscrDiBaoTri(long RscrPropertyID)
        {
            try
            {
                return ResourcesManagement.Instance.CheckRscrDiBaoTri(RscrPropertyID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CheckRscrDiBaoTri. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_CHECK_RSCR_DIBAOTRI);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool ResourceMaintenanceLog_Delete(long RscrMaintLogID)
        {
            try
            {
                return ResourcesManagement.Instance.ResourceMaintenanceLog_Delete(RscrMaintLogID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ResourceMaintenanceLog_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_RESOURCE_MAINTENANCE_LOG_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region IResourcesManagementService Members


        public bool GetCheckForVerified(long RscrMaintLogID)
        {
            try
            {
                return ResourcesManagement.Instance.GetCheckForVerified(RscrMaintLogID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetCheckForVerified. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_CHECK_FOR_VERIFIED);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public List<ResourceGroup> GetAllResourceGroupType(long V_ResGroupCategory)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllResourceGroupType(V_ResGroupCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllResourceGroupType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_RESOURCE_GROUPTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<Supplier> GetAllSupplierType(long V_SupplierType)
        {
            try
            {
                return ResourcesManagement.Instance.GetAllSupplierType(V_SupplierType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllSupplierType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.RESOURCE_GET_ALL_SUPPLIERTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
    }
}
