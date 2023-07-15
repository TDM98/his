using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Runtime.Serialization;
using ErrorLibrary;
using DataEntities;

using AxLogging;
using ErrorLibrary.Resources;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace ConfigurationManagerService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class BedAllocationsService : eHCMS.WCFServiceCustomHeader, IBedAllocations
    {

        public BedAllocationsService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region IRefDepartments Members

        public RefDepartments GetRefDepartmentsByID(long ID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartmentsByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDepartmentsByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_REFDEPARTMENTS_BY_ID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefDepartments> GetRefDepartments()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDepartments_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_REFDEPARTMENTS_ALL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<RefDepartmentsTree> list = new List<RefDepartmentsTree>();
        public List<RefDepartmentsTree> GetDeptLocationTreeView()
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
                foreach (RefDepartments item in familytherapies)
                {
                    RefDepartmentsTree genericItem = new RefDepartmentsTree();
                    if (item.ParDeptID != null)
                        genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
                    else
                        genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));

                    this.list.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == null);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (RefDepartmentsTree node in rootNodes)
                {
                    this.FindChildren(node);
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDepartments_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_REFDEPARTMENTS_ALL);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        private void FindChildren(RefDepartmentsTree item)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    //add vao depLocation
                    child.Childrens = new List<DeptLocation>();
                    child.Childrens = GetAllDeptLocationByDeptID(child.NodeID);
                    //FindChildren(child);
                    int level = 2;
                    foreach (DeptLocation dl in child.Childrens)
                    {
                        RefDepartmentsTree subItem = new RefDepartmentsTree();
                        subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, child.NodeID, child.V_DeptType, child.V_DeptTypeOperation, dl.Location.LocationDescription, GetRefDepartmentsByID(child.NodeID), level);
                        child.Children.Add(subItem);
                    }
                    item.Children.Add(child);
                    //item.Children.Add(subItem);
                }
            }
            catch
            { }
        }
        public List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocationByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALL_DEPTLOCATION_BY_DEPTID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool AddNewRoomPrices(long DeptLocationID
                                                , long StaffID
                                                , DateTime? EffectiveDate
                                                , Decimal NormalPrice
                                                , Decimal PriceForHIPatient
                                                , Decimal HIAllowedPrice
                                                , string Note
                                                , bool IsActive)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.AddNewRoomPrices(DeptLocationID
                                                , StaffID
                                                , EffectiveDate
                                                , NormalPrice
                                                , PriceForHIPatient
                                                , HIAllowedPrice
                                                , Note
                                                , IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewRoomPrices. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_ADDNEW_ROOMPRICES);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<RoomPrices> GetAllRoomPricesByDeptID(long DeptLocationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllRoomPricesByDeptID(DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRoomPricesByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALL_ROOMPRICES_BY_DEPTID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewBedAllocation(int times, long DeptLocationID
                                                   , string BedNumber
                                                   , long MedServiceID
                                                    , string BAGuid
                                                   , long V_BedLocType
                                                   , bool IsActive)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.AddNewBedAllocation(times, DeptLocationID
                                                   , BedNumber
                                                   , MedServiceID
                                                   , BAGuid
                                                   , V_BedLocType
                                                   , IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewBedAllocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_ADDNEW_BEDALLOCATION);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateBedAllocationMedSer(string BAGuid, long MedServiceID, long V_BedLocType)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.UpdateBedAllocationMedSer(BAGuid, MedServiceID, V_BedLocType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateBedAllocationMedSer. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDALLOCATIONMEDSER);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<BedAllocation> GetCountBedAllocByDeptID(long DeptLocationID, int Choice)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetCountBedAllocByDeptID(DeptLocationID, Choice);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetCountBedAllocByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_COUNT_BEDALLOC_BY_DEPTID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<BedAllocation> GetAllBedAllocationByDeptID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllBedAllocationByDeptID(DeptLocationID, PageSize, PageIndex, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                Total = -1;
                AxLogger.Instance.LogInfo("End of GetAllBedAllocationByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALLBEDALLOC_BY_DEPTID);
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
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALL_REFSTAFFCATEGORIES);
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
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALL_STAFF);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteBedAllocation(long BedAllocationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.DeleteBedAllocation(BedAllocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteBedAllocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_DELETE_BEDALLOCATION);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public string UpdateBedAllocation(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.UpdateBedAllocation(LstBedAllocation);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateBedAllocation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDALLOCATION);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }
        public bool AddNewBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.AddNewBedAllocationList(LstBedAllocation);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewBedAllocationList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_ADDNEW_BEDALLOCATION_LIST);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }
        public bool UpdateBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.UpdateBedAllocationList(LstBedAllocation);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateBedAllocationList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDALLOCATION_LIST);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }
        public List<MedServiceItemPrice> GetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllDeptMSItemsByDeptIDSerTypeID(DeptID, MedicalServiceTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptMSItemsByDeptIDSerTypeID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALLDEPTMSITEMS_BY_DEPTID_SERTYPEID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }
        #endregion
        #region
        public bool UpdateBedLocType(long BedLocTypeID
                                                , string BedLocTypeName
                                                , string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.UpdateBedLocType(BedLocTypeID
                                                , BedLocTypeName
                                                , Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateBedLocType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDLOCTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }

        public bool AddNewBedLocType(string BedLocTypeName, string Description)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.AddNewBedLocType(BedLocTypeName, Description);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewBedLocType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_ADDNEW_BEDLOCTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }

        public bool DeleteBedLocType(long BedLocTypeID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.DeleteBedLocType(BedLocTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteBedLocType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_DELETE_BEDLOCTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }

        public List<BedAllocType> GetAllBedLocType()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllBedLocType();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllBedLocType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALLBEDLOCTYPE);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }
        public List<BedAllocation> GetAllBedAllocByDeptID(long DeptLocationID, int IsActive, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllBedAllocByDeptID(DeptLocationID, IsActive, out Total);
            }
            catch (Exception ex)
            {
                Total = 0;
                AxLogger.Instance.LogInfo("End of GetAllBedAllocByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALLBEDALLOC_BY_DEPTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }

        }

        #endregion

        #region bed patient alloc
        public bool UpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.UpdateBedPatientAllocs(BedPatientID
                                                  , BedAllocationID
                                                  , PtRegistrationID
                                                  , AdmissionDate
                                                  , ExpectedStayingDays
                                                  , DischargeDate
                                                  , IsActive);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateBedPatientAllocs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDPATIENT_ALLOCS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }

        public int AddNewBedPatientAllocs(BedPatientAllocs alloc)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.AddNewBedPatientAllocs(alloc);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewBedPatientAllocs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_ADDNEW_BEDPATIENT_ALLOCS);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));

            }
        }

        public bool EditBedPatientAllocs(BedPatientAllocs alloc, long LoggedStaffID)
        {
            try
            {
                return BedAllocations.Instance.EditBedPatientAllocs(alloc, LoggedStaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditBedPatientAllocs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_UPDATE_BEDPATIENT_ALLOCS);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteBedPatientAllocs(long BedPatientID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.DeleteBedPatientAllocs(BedPatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteBedPatientAllocs. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_DELETE_BEDPATIENT_ALLOCS);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));

            }
        }

        public List<BedPatientAllocs> GetAllBedPatientAllocByDeptID(long DeptLocationID, bool IsReadOnly, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.BedAllocations.Instance.GetAllBedPatientAllocByDeptID(DeptLocationID, IsReadOnly, out Total);
            }
            catch (Exception ex)
            {
                Total = 0;
                AxLogger.Instance.LogInfo("End of GetAllBedPatientAllocByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_GET_ALLBEDALLOC_BY_DEPTID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        #endregion


        public bool MarkDeleteBedPatientAlloc(long bedPatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start marking BedPatientAlloc deleted.", CurrentUser);
                bool bOk = aEMR.DataAccessLayer.Providers.BedAllocations.Instance.MarkDeleteBedPatientAlloc(bedPatientID);
                AxLogger.Instance.LogInfo("End of marking BedPatientAlloc deleted.", CurrentUser);
                return bOk;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of marking BedPatientAlloc deleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.BED_ALLOCATION_MarkDeleteBedPatientAlloc, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
