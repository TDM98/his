/*
 * 20170512 #001 CMN: Added method to get pcl room by catid
 * 20190603 #002 TNHX: [BM0011782] get all RefPurposeForAccountant
 * 20190926 #003 TNHX: [BM] get list FilterPrescriptionsHasHIPay
 * 20220620 #004 DatTB: Thêm filter danh mục xét nghiệm
 * 20230509 #005 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
 * 20230518 #006 DatTB: Thêm service cho mẫu bệnh phẩm
 * 20230531 #007 QTD:   Thêm quản lý danh mục
 * 20230601 #008 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
 * 20230622 #009 DatTB:
 * + Thêm filter theo mã ICD đã lưu của nhóm bệnh
 * + Thêm function chỉnh sửa ICD của nhóm bệnh
 * + Thay đổi điều kiện gàng buộc ICD
*/
using System;
using System.Collections.Generic;
using System.Linq;

using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Runtime.Serialization;
using ErrorLibrary;
using DataEntities;

using AxLogging;
using ErrorLibrary.Resources;

using System.Collections.ObjectModel;

using eHCMS.Configurations;
using eHCMS.Caching;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;
using DataEntities.MedicalInstruction;

namespace ConfigurationManagerService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class ConfigurationManagerService : eHCMS.WCFServiceCustomHeader, IConfigurationManagerService
    {

        public ConfigurationManagerService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;

        }

        #region RefDepartments Members

        public RefDepartments GetRefDepartmentsByID(long ID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartmentsByID(ID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetRefDepartmentsByID(ID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartmentsByID(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDepartmentsByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetRefDepartmentsByID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefDepartmentsTree> list = new List<RefDepartmentsTree>();

        public bool AddNewRefDepartments(RefDepartments obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AddNewRefDepartments(obj);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.AddNewRefDepartments(obj);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AddNewRefDepartments(obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddNewRefDepartments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_AddNewRefDepartments);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void UpdateRefDepartments(RefDepartments obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.UpdateRefDepartments(obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.UpdateRefDepartments(obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.UpdateRefDepartments(obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateRefDepartments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_UpdateRefDepartments);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteRefDepartments(long DeptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeleteRefDepartmentsByID(DeptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeleteRefDepartmentsByID(DeptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeleteRefDepartmentsByID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteRefDepartmentsByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeleteRefDepartmentsByID);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region tree khoa theo loai phong kham
        public List<RefDepartmentsTree> GetRefDepartments_TreeFunction(string strV_DeptType, bool ShowDeptLocation, long RoomFunction)
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
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
                    results.Add(node);
                    this.FindChildrenRoomFunction(node, ShowDeptLocation, RoomFunction);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        int iLevelR = 0;
        private void FindChildrenRoomFunction(RefDepartmentsTree item, bool ShowDeptLocation, long RoomFunction)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);
                if (children.Count() > 0)
                {
                    iLevelR++;
                }
                else//nhánh đã hết
                {
                    if (ShowDeptLocation)
                    {
                        //add lá Vào(DepLocation) vào nếu có
                        List<DeptLocation> ListDeptLocation = GetAllDeptLocationByDeptIDFunction(item.NodeID, RoomFunction);

                        if (ListDeptLocation.Count() > 0)
                        {
                            iLevelR++;

                            foreach (DeptLocation dl in ListDeptLocation)
                            {
                                RefDepartmentsTree subItem = new RefDepartmentsTree();
                                subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, item.NodeID, item.V_DeptType, item.V_DeptTypeOperation, dl.Location.LocationDescription, item, iLevelR, true, "/eHCMSCal;component/Assets/Images/Home-icon.png");
                                item.HasDeptLocation = true;
                                item.Children.Add(subItem);
                            }
                        }
                        else//Nhánh không có Lá thì reset Level lại
                        {
                            iLevelR = 0;
                        }
                    }
                    else//Không Cho Phép Hiển Thị Lá
                    {
                        iLevelR = 0;
                    }
                }
                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    child.Parent = item;
                    child.Level = iLevelR;
                    item.Children.Add(child);
                    FindChildrenRoomFunction(child, ShowDeptLocation, RoomFunction);
                }
            }
            catch
            { }
        }

        #endregion

        #region tree Ca Kham theo loai phong kham
        public List<RefDepartmentsTree> GetRefDepartments_TreeSegment(string strV_DeptType, bool ShowDeptLocation, long RoomFunction)
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
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
                    results.Add(node);
                    this.FindChildrenRoomSegment(node, ShowDeptLocation, RoomFunction);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        int iLevelS = 0;
        private void FindChildrenRoomSegment(RefDepartmentsTree item, bool ShowDeptLocation, long RoomFunction)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);
                if (children.Count() > 0)
                {
                    iLevelS++;
                }
                else//nhánh đã hết
                {
                    if (ShowDeptLocation)
                    {
                        //add lá Vào(DepLocation) vào nếu có
                        List<DeptLocation> ListDeptLocation = GetAllDeptLocationByDeptIDFunction(item.NodeID, RoomFunction);

                        if (ListDeptLocation.Count() > 0)
                        {
                            iLevelS++;

                            foreach (DeptLocation dl in ListDeptLocation)
                            {
                                RefDepartmentsTree subItem = new RefDepartmentsTree();
                                subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, item.NodeID, item.V_DeptType, item.V_DeptTypeOperation, dl.Location.LocationDescription, item, iLevelS, true, "/eHCMSCal;component/Assets/Images/Home-icon.png");
                                item.HasDeptLocation = true;
                                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                                //{
                                //    subItem.LstConsultationRoomTarget = aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(dl.DeptLocationID, 0);
                                //    subItem.LstConsultationRoomStaffAllocations = aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(dl.DeptLocationID, 0);
                                //}
                                //else
                                //{
                                //    subItem.LstConsultationRoomTarget = ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(dl.DeptLocationID, 0);
                                //    subItem.LstConsultationRoomStaffAllocations = ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(dl.DeptLocationID, 0);
                                //}
                                subItem.LstConsultationRoomTarget = aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(dl.DeptLocationID, 0);
                                subItem.LstConsultationRoomStaffAllocations = aEMR.DataAccessLayer.Providers.ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(dl.DeptLocationID, 0);
                                item.Children.Add(subItem);
                            }
                        }
                        else//Nhánh không có Lá thì reset Level lại
                        {
                            iLevelS = 0;
                        }
                    }
                    else//Không Cho Phép Hiển Thị Lá
                    {
                        iLevelS = 0;
                    }
                }
                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    child.Parent = item;
                    child.Level = iLevelS;
                    item.Children.Add(child);
                    FindChildrenRoomSegment(child, ShowDeptLocation, RoomFunction);
                }
            }
            catch
            { }
        }

        #endregion


        #region Tree Khoa, Phòng, Kho theo chuỗi loại V_DeptType đưa vô có dạng:"7000,7001"
        //public List<RefDepartmentsTree> RefDepartments_Tree(string strV_DeptType, bool ShowDeptLocation, string V_DeptTypeOperation)
        //{
        //    try
        //    {
        //        list.Clear();
        //        IList<RefDepartments> familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, V_DeptTypeOperation);
        //        List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
        //        foreach (RefDepartments item in familytherapies)
        //        {
        //            RefDepartmentsTree genericItem = new RefDepartmentsTree();
        //            if (item.ParDeptID != null)
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
        //            else
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));

        //            this.list.Add(genericItem);
        //        }

        //        // Get all the first level nodes. In our case it is only one - House M.D.
        //        var rootNodes = this.list.Where(x => x.ParentID == null);

        //        // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
        //        // see bellow how the FindChildren method works
        //        foreach (RefDepartmentsTree node in rootNodes)
        //        {
        //            results.Add(node);
        //            this.FindChildren(node, ShowDeptLocation);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        public List<RefDepartmentsTree> RefDepartments_Tree(string strV_DeptType, bool ShowDeptLocation, string V_DeptTypeOperation)
        {
            try
            {
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, V_DeptTypeOperation);
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, V_DeptTypeOperation);
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, V_DeptTypeOperation);
                List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
                foreach (RefDepartments item in familytherapies)
                {
                    RefDepartmentsTree genericItem = new RefDepartmentsTree();
                    if (item.ParDeptID != null)
                        genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, familytherapies.FirstOrDefault(x => x.DeptID == item.ParDeptID.Value), item.DeptNameEng);
                    else
                        genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, familytherapies.FirstOrDefault(x => x.DeptID == item.DeptID), item.DeptNameEng);
                    results.Add(genericItem);
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                List<RefDepartmentsTree> rootNodes = results.Where(x => x.ParentID == null).ToList();

                //
                List<DeptLocation> ListDeptLocation = GetAllDeptLocationByDeptID(0);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (RefDepartmentsTree node in rootNodes)
                {
                    results.Add(node);
                    //this.FindChildren(node, ShowDeptLocation);
                    CreateChildrens(node, results, ListDeptLocation, ShowDeptLocation, 0);
                }

                return rootNodes;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        private void CreateChildrens(RefDepartmentsTree aRefDepartment, List<RefDepartmentsTree> aRefDepartmentCollection, List<DeptLocation> aLocation, bool ShowDeptLocation = false, int iLevel = 0)
        {
            foreach (var item in aRefDepartmentCollection.Where(x => x.ParentID == aRefDepartment.NodeID))
            {
                item.Parent = aRefDepartment;
                // 20181019 TNHX [BM0003193] - Update DepartmentTree - using ShowDeptLocation to show DeptLocation - Refactor code
                if (ShowDeptLocation)
                {
                    CreateChildrens(item, aRefDepartmentCollection, aLocation, ShowDeptLocation, iLevel + 1);
                }
                aRefDepartment.Children.Add(item);
            }
            foreach (var item in aLocation.Where(x => x.DeptID == aRefDepartment.NodeID))
            {
                var subitem = new RefDepartmentsTree(item.Location.LocationName, item.DeptLocationID, aRefDepartment.NodeID, aRefDepartment.V_DeptType, aRefDepartment.V_DeptTypeOperation, item.Location.LocationDescription, aRefDepartment, iLevel, true, "/eHCMSCal;component/Assets/Images/Home-icon.png");
                aRefDepartment.Children.Add(subitem);
            }
        }

        public List<RefDepartmentsTree> RefDepartments_Tree_ByDeptID(string strV_DeptType, bool ShowDeptLocation, long DeptID, long DeptLocID)
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType(strV_DeptType, null);
                List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
                foreach (RefDepartments item in familytherapies)
                {
                    RefDepartmentsTree genericItem = new RefDepartmentsTree();
                    if (item.ParDeptID != null)
                    {
                        if (DeptID > 0 && item.DeptID == DeptID)
                        {
                            genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
                            this.list.Add(genericItem);
                        }
                    }
                    else
                    {
                        genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.V_DeptTypeOperation, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));
                        this.list.Add(genericItem);
                    }
                }

                // Get all the first level nodes. In our case it is only one - House M.D.
                var rootNodes = this.list.Where(x => x.ParentID == null);

                // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
                // see bellow how the FindChildren method works
                foreach (RefDepartmentsTree node in rootNodes)
                {
                    results.Add(node);
                    {
                        this.FindChildren(node, DeptLocID, ShowDeptLocation);
                    }

                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        int iLevel = 0;
        private void FindChildren(RefDepartmentsTree item, bool ShowDeptLocation)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);
                if (children.Count() > 0)
                {
                    iLevel++;
                }
                else//nhánh đã hết
                {
                    if (ShowDeptLocation)
                    {
                        //add lá Vào(DepLocation) vào nếu có
                        List<DeptLocation> ListDeptLocation = GetAllDeptLocationByDeptID(item.NodeID);

                        if (ListDeptLocation.Count() > 0)
                        {
                            iLevel++;

                            foreach (DeptLocation dl in ListDeptLocation)
                            {
                                RefDepartmentsTree subItem = new RefDepartmentsTree();
                                subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, item.NodeID, item.V_DeptType, item.V_DeptTypeOperation, dl.Location.LocationDescription, item, iLevel, true, "/eHCMSCal;component/Assets/Images/Home-icon.png");
                                item.HasDeptLocation = true;
                                item.Children.Add(subItem);
                            }
                        }
                        else//Nhánh không có Lá thì reset Level lại
                        {
                            iLevel = 0;
                        }
                    }
                    else//Không Cho Phép Hiển Thị Lá
                    {
                        iLevel = 0;
                    }
                }
                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    child.Parent = item;
                    child.Level = iLevel;
                    item.Children.Add(child);
                    FindChildren(child, ShowDeptLocation);
                }
            }
            catch
            { }
        }

        private void FindChildren(RefDepartmentsTree item, long DeptLocID, bool ShowDeptLocation)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);
                if (children.Count() > 0)
                {
                    iLevel++;
                }
                else//nhánh đã hết
                {
                    if (ShowDeptLocation)
                    {
                        //add lá Vào(DepLocation) vào nếu có
                        List<DeptLocation> ListDeptLocation = GetAllDeptLocationByDeptID(item.NodeID);

                        if (ListDeptLocation.Count() > 0)
                        {
                            iLevel++;

                            foreach (DeptLocation dl in ListDeptLocation)
                            {
                                if (DeptLocID > 0 && dl.DeptLocationID == DeptLocID)
                                {
                                    RefDepartmentsTree subItem = new RefDepartmentsTree();
                                    subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, item.NodeID, item.V_DeptType, item.V_DeptTypeOperation, dl.Location.LocationDescription, item, iLevel, true, "/eHCMSCal;component/Assets/Images/Home-icon.png");
                                    item.HasDeptLocation = true;
                                    item.Children.Add(subItem);
                                }
                            }
                        }
                        else//Nhánh không có Lá thì reset Level lại
                        {
                            iLevel = 0;
                        }
                    }
                    else//Không Cho Phép Hiển Thị Lá
                    {
                        iLevel = 0;
                    }
                }
                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    if (child.IsDeptLocation)
                    {
                        if (DeptLocID > 0 && child.NodeID == DeptLocID)
                        {
                            child.Parent = item;
                            child.Level = iLevel;
                            item.Children.Add(child);
                            FindChildren(child, DeptLocID, ShowDeptLocation);
                        }
                    }
                    else
                    {
                        child.Parent = item;
                        child.Level = iLevel;
                        item.Children.Add(child);
                        FindChildren(child, DeptLocID, ShowDeptLocation);
                    }
                }
            }
            catch
            { }
        }
        #endregion


        //Đệ qui đọc hết con ra
        public List<RefDepartments> ListRefDepartments_RecursiveByDeptID = new List<RefDepartments>();
        public List<RefDepartments> RefDepartments_RecursiveByDeptID(Int64 DeptID)
        {
            List<RefDepartments> AllByParentID = new List<RefDepartments>();
            //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
            //{
            //    AllByParentID = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(DeptID);
            //}
            //else
            //{
            //    AllByParentID = ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(DeptID);
            //}
            AllByParentID = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(DeptID);
            AddChild(AllByParentID);

            return ListRefDepartments_RecursiveByDeptID;
        }

        private void AddChild(IList<RefDepartments> ObjList)
        {
            foreach (RefDepartments child in ObjList)
            {
                ListRefDepartments_RecursiveByDeptID.Add(child);
                List<RefDepartments> child2 = new List<RefDepartments>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    child2 = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(child.DeptID);
                //}
                //else
                //{
                //    child2 = ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(child.DeptID);
                //}
                child2 = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(child.DeptID);
                if (child2.Count > 0)
                {
                    AddChild(child2);
                }
            }
        }
        //Đệ qui đọc hết con ra


        //Ngắt bỏ 1 nhánh và các con nó
        public List<RefDepartments> RefDepartment_SubtractAllChild_ByDeptID(Int64 DeptID)
        {
            if (DeptID > 0)
            {
                List<RefDepartments> objListResultIDAllCon = new List<RefDepartments>();

                List<RefDepartments> list = new List<RefDepartments>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    list = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                //}
                //else
                //{
                //    list = ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                //}
                list = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                var Node = list.Where(x => x.DeptID == DeptID);

                RefDepartments ObjNode = Node.FirstOrDefault();

                objListResultIDAllCon.Add(ObjNode);

                //Tìm con.
                FindChild_SubtractAllChild_ByDeptID(Node.FirstOrDefault<RefDepartments>(), list, objListResultIDAllCon);

                var Result = from p in list
                             where !(from s in objListResultIDAllCon
                                     select s.DeptID)
                                     .Contains(p.DeptID)
                             select p;

                return Result.ToList<RefDepartments>();
            }
            else
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetRefDepartments_All();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartments_All();
            }
        }
        private void FindChild_SubtractAllChild_ByDeptID(RefDepartments item, List<RefDepartments> ObjList, List<RefDepartments> ObjListResult)
        {
            var ListNode = ObjList.Where(x => x.ParDeptID == item.DeptID);
            foreach (RefDepartments itemchild in ListNode.ToList<RefDepartments>())
            {
                ObjListResult.Add(itemchild);
                FindChild_SubtractAllChild_ByDeptID(itemchild, ObjList, ObjListResult);
            }
        }
        //Ngắt bỏ 1 nhánh và các con nó

        public List<RefDepartmentsTree> GetSearchTreeView(RefDepartmentsSearchCriteria SearchCriteria)
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SearchRefDepartments(SearchCriteria);
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.SearchRefDepartments(SearchCriteria);
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SearchRefDepartments(SearchCriteria);
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
                    results.Add(node);
                    this.FindChildren(node, SearchCriteria.ShowDeptLocation);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRefDepartments. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_SearchRefDepartments);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllDeptLocationByDeptIDFunction(long DeptID, long RoomFunction)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptIDFunction(DeptID, RoomFunction);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptIDFunction(DeptID, RoomFunction);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptIDFunction(DeptID, RoomFunction);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocationByDeptIDFunction. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllDeptLocationByDeptIDFunction);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptID(DeptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptID(DeptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocationByDeptID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocationByDeptID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllDeptLocationByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllDeptLocLaboratory()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocLaboratory();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllDeptLocLaboratory();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllDeptLocLaboratory();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllDeptLocLaboratory. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllDeptLocLaboratory);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region Khoa, Phòng hiện có trong DeptMedServiceItems
        public List<RefDepartmentsTree> TreeRefDepartment_InTable_DeptMedServiceItems()
        {
            try
            {
                list.Clear();
                IList<RefDepartments> familytherapies;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartmentTree_InTable_DeptMedServiceItems();
                //}
                //else
                //{
                //    familytherapies = ConfigurationManagerProviders.Instance.GetRefDepartmentTree_InTable_DeptMedServiceItems();
                //}
                familytherapies = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetRefDepartmentTree_InTable_DeptMedServiceItems();
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
                    this.FindChildrenRefDepartment_InTable_DeptMedServiceItems(node);
                    results.Add(node);
                }

                return results;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRefDepartmentTree_InTable_DeptMedServiceItems. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetRefDepartmentTree_InTable_DeptMedServiceItems);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        private void FindChildrenRefDepartment_InTable_DeptMedServiceItems(RefDepartmentsTree item)
        {
            try
            {
                // find all the children of the item
                var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

                // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
                foreach (RefDepartmentsTree child in children)
                {
                    //add vao depLocation
                    //////child.Childrens = new List<DeptLocation>();
                    //////child.Childrens = GetAllDeptLocationByDeptID(child.NodeID);
                    ////////FindChildren(child);
                    //////int level = 2;
                    //////foreach (DeptLocation dl in child.Childrens)
                    //////{
                    //////    RefDepartmentsTree subItem = new RefDepartmentsTree();
                    //////    subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, child.NodeID, dl.Location.LocationDescription, null, level);
                    //////    child.Children.Add(subItem);
                    //////}
                    item.Children.Add(child);
                    //item.Children.Add(subItem);
                }
            }
            catch
            { }
        }
        //</spTreeRefDepartmentForDeptMedServiceItems>
        #endregion

        //#region Tree 2 nút cha Khoa và Phòng
        //public List<RefDepartmentsTree> TreeRefDepartment_Khoa_Phong()
        //{
        //    try
        //    {
        //        list.Clear();
        //        IList<RefDepartments> familytherapies = ConfigurationManagerProviders.Instance.RefDepartments_ByInStrV_DeptType("7000,7001");
        //        List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
        //        foreach (RefDepartments item in familytherapies)
        //        {
        //            RefDepartmentsTree genericItem = new RefDepartmentsTree();
        //            if (item.ParDeptID != null)
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
        //            else
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));

        //            this.list.Add(genericItem);
        //        }

        //        // Get all the first level nodes. In our case it is only one - House M.D.
        //        var rootNodes = this.list.Where(x => x.ParentID == null);

        //        // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
        //        // see bellow how the FindChildren method works
        //        foreach (RefDepartmentsTree node in rootNodes)
        //        {
        //            this.FindChildren_TreeRefDepartment_Khoa_Phong(node);
        //            results.Add(node);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RefDepartments_ByInStrV_DeptType. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByInStrV_DeptType);


        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        //private void FindChildren_TreeRefDepartment_Khoa_Phong(RefDepartmentsTree item)
        //{
        //    try
        //    {
        //        // find all the children of the item
        //        var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

        //        // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
        //        foreach (RefDepartmentsTree child in children)
        //        {
        //            //add vao depLocation
        //            //////child.Childrens = new List<DeptLocation>();
        //            //////child.Childrens = GetAllDeptLocationByDeptID(child.NodeID);
        //            ////////FindChildren(child);
        //            //////int level = 2;
        //            //////foreach (DeptLocation dl in child.Childrens)
        //            //////{
        //            //////    RefDepartmentsTree subItem = new RefDepartmentsTree();
        //            //////    subItem = new RefDepartmentsTree(dl.Location.LocationName, dl.DeptLocationID, child.NodeID, dl.Location.LocationDescription, null, level);
        //            //////    child.Children.Add(subItem);
        //            //////}
        //            item.Children.Add(child);
        //            //item.Children.Add(subItem);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}
        //#endregion

        ////<GetRefDepartmentForDeptMedServiceItems>
        //public List<RefDepartments> GetRefDepartmentForDeptMedServiceItems()
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.GetRefDepartmentForDeptMedServiceItems();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetRefDepartmentForDeptMedServiceItems. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetRefDepartmentForDeptMedServiceItems);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        ////</GetRefDepartmentForDeptMedServiceItems>


        //<spTreeRefDepartmentForDeptMedServiceItems>
        ////Tree from Table RefDepartments isDeleted=0
        //public List<RefDepartmentsTree> TreeRefDepartmentisDeleted0()
        //{
        //    try
        //    {
        //        list.Clear();
        //        IList<RefDepartments> familytherapies = ConfigurationManagerProviders.Instance.GetRefDepartments_All();
        //        List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
        //        foreach (RefDepartments item in familytherapies)
        //        {
        //            RefDepartmentsTree genericItem = new RefDepartmentsTree();
        //            if (item.ParDeptID != null)
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
        //            else
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));

        //            this.list.Add(genericItem);
        //        }

        //        // Get all the first level nodes. In our case it is only one - House M.D.
        //        var rootNodes = this.list.Where(x => x.ParentID == null);

        //        // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
        //        // see bellow how the FindChildren method works
        //        foreach (RefDepartmentsTree node in rootNodes)
        //        {
        //            this.FindChildrenForDeptMedServiceItems(node);
        //            results.Add(node);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetRefDepartments_All. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetRefDepartments_All);


        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        ////Tree from Table RefDepartments isDeleted=0

        //Get List Root

        //public List<RefDepartments> GetRefDepartments_AllParent()
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.GetRefDepartments_AllParent();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of GetRefDepartments_AllParent. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetRefDepartments_AllParent);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        //#region Cây Khoa Trong DeptMedServiceItems là PCL
        //public List<RefDepartmentsTree> DeptMedServiceItems_TreeDeptHasMedserviceIsPCL()
        //{
        //    try
        //    {
        //        list.Clear();
        //        IList<RefDepartments> familytherapies = ConfigurationManagerProviders.Instance.DeptMedServiceItems_GetDeptHasMedserviceIsPCL();
        //        List<RefDepartmentsTree> results = new List<RefDepartmentsTree>();
        //        foreach (RefDepartments item in familytherapies)
        //        {
        //            RefDepartmentsTree genericItem = new RefDepartmentsTree();
        //            if (item.ParDeptID != null)
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.DeptDescription, GetRefDepartmentsByID(item.ParDeptID.Value));
        //            else
        //                genericItem = new RefDepartmentsTree(item.DeptName, item.DeptID, item.ParDeptID, item.V_DeptType, item.DeptDescription, GetRefDepartmentsByID(item.DeptID));

        //            this.list.Add(genericItem);
        //        }

        //        // Get all the first level nodes. In our case it is only one - House M.D.
        //        var rootNodes = this.list.Where(x => x.ParentID == null);

        //        // Foreach root node, get all its children and add the node to the HierarchicalDataSource.
        //        // see bellow how the FindChildren method works
        //        foreach (RefDepartmentsTree node in rootNodes)
        //        {
        //            this.FindChildren_DeptMedServiceItems_TreeDeptHasMedserviceIsPCL(node);
        //            results.Add(node);
        //        }

        //        return results;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of DeptMedServiceItems_GetDeptHasMedserviceIsPCL. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_GetDeptHasMedserviceIsPCL);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        //private void FindChildren_DeptMedServiceItems_TreeDeptHasMedserviceIsPCL(RefDepartmentsTree item)
        //{
        //    try
        //    {
        //        // find all the children of the item
        //        var children = list.Where(x => x.ParentID == item.NodeID && x.NodeID != item.NodeID);

        //        // add the child to the item's children collection and call the FindChildren recursively, in case the child has children
        //        foreach (RefDepartmentsTree child in children)
        //        {
        //            item.Children.Add(child);
        //        }
        //    }
        //    catch (Exception ex)
        //    { }
        //}
        //#endregion


        public List<RefDepartments> RefDepartments_ByParDeptID(long ParDeptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(ParDeptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(ParDeptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefDepartments_ByParDeptID(ParDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefDepartments_ByParDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefDepartments_ByParDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region PCLExamGroup
        public IList<PCLExamGroup> GetPCLExamGroup_ByMedServiceID_NoPaging(long MedServiceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamGroup_ByMedServiceID_NoPaging(MedServiceID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetPCLExamGroup_ByMedServiceID_NoPaging(MedServiceID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamGroup_ByMedServiceID_NoPaging(MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamGroup_ByMedServiceID_NoPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetPCLExamGroup_ByMedServiceID_NoPaging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefDepartments> PCLExamGroup_GetListDeptID()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamGroup_GetListDeptID();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamGroup_GetListDeptID();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamGroup_GetListDeptID();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamGroup_GetListDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamGroup_GetListDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region RefMedicalServiceItem
        public List<RefMedicalServiceItem> RefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItemsByMedicalServiceTypeID(MedicalServiceTypeID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItemsByMedicalServiceTypeID(MedicalServiceTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItemsByMedicalServiceTypeID(MedicalServiceTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItemsByMedicalServiceTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItemsByMedicalServiceTypeID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefMedicalServiceItem> RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<RefMedicalServiceItem> RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_ByMedicalServiceTypeID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_ByMedicalServiceTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefMedicalServiceItem> RefMedicalServiceItems_IsPCLByMedServiceID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCLByMedServiceID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCLByMedServiceID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCLByMedServiceID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_IsPCLByMedServiceID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_IsPCLByMedServiceID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public List<RefMedicalServiceItem> RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<RefMedicalServiceItem> RefMedicalServiceItems_In_DeptLocMedServices(RefMedicalServiceItemsSearchCriteria SearchCriteria)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_In_DeptLocMedServices. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_In_DeptLocMedServices);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<RefMedicalServiceItem> GetMedServiceItems_Paging(
            RefMedicalServiceItemsSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetMedServiceItems_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetMedServiceItems_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetMedServiceItems_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedServiceItems_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_In_DeptLocMedServices);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region PCLExamTypes

        public List<PCLExamType> PCLExamTypes_NotYetPCLLabResultSectionID_Paging(PCLExamTypeSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_NotYetPCLLabResultSectionID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypes_NotYetPCLLabResultSectionID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_NotYetPCLLabResultSectionID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_NotYetPCLLabResultSectionID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> GetPCLExamTypes_Paging(PCLExamTypeSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetPCLExamTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamTypes_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetPCLExamTypes_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public List<RefMedicalServiceItem> PCLExamTypes_GetListMedServiceID()
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.PCLExamTypes_GetListMedServiceID();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PCLExamTypes_GetListMedServiceID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_GetListMedServiceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}



        public List<PCLExamType> PCLExamTypesAndPriceIsActive_Paging(PCLExamTypeSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesAndPriceIsActive_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypesAndPriceIsActive_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesAndPriceIsActive_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypesAndPriceIsActive_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypesAndPriceIsActive_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void PCLExamTypes_Save_NotIsLab(PCLExamType Obj, bool IsInsert, long StaffID, out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_Save_NotIsLab(Obj, out Result, out PCLExamTypeID_New);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypes_Save_NotIsLab(Obj, out Result, out PCLExamTypeID_New);
                //}
                string mainCacheKey = "AllPCLExamType";
                if (ServerAppConfig.CachingEnabled && (List<PCLExamType>)AxCache.Current[mainCacheKey] != null)
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_Save_NotIsLab(Obj, IsInsert, StaffID, out Result, out PCLExamTypeID_New);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_Save_NotIsLab. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_Save_NotIsLab);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void PCLExamTypes_Save_IsLab(
            PCLExamType Obj,
            bool IsInsert,
            /*ExamType la ExamTest*/
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            /*ExamType la ExamTest*/
            long StaffID,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
            out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_Save_IsLab(Obj,
                //                                                                    TestItemIsExamType,
                //                                                                    PCLExamTestItemUnitForPCLExamType,
                //                                                                    PCLExamTestItemRefScaleForPCLExamType,
                //                                                                    DataPCLExamTestItems_Insert,
                //                                                                    DataPCLExamTestItems_Update,
                //                                                                    DataPCLExamTestItems_Delete,
                //                                                                    out Result, out PCLExamTypeID_New);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypes_Save_IsLab(Obj,
                //                                                                    TestItemIsExamType,
                //                                                                    PCLExamTestItemUnitForPCLExamType,
                //                                                                    PCLExamTestItemRefScaleForPCLExamType,
                //                                                                    DataPCLExamTestItems_Insert,
                //                                                                    DataPCLExamTestItems_Update,
                //                                                                    DataPCLExamTestItems_Delete,
                //                                                                    out Result, out PCLExamTypeID_New);
                //}
                string mainCacheKey = "AllPCLExamType";
                if (ServerAppConfig.CachingEnabled && (List<PCLExamType>)AxCache.Current[mainCacheKey] != null)
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_Save_IsLab(Obj, IsInsert,
                                                                                    TestItemIsExamType,
                                                                                    PCLExamTestItemUnitForPCLExamType,
                                                                                    PCLExamTestItemRefScaleForPCLExamType,
                                                                                    StaffID,
                                                                                    DataPCLExamTestItems_Insert,
                                                                                    DataPCLExamTestItems_Update,
                                                                                    DataPCLExamTestItems_Delete,
                                                                                    out Result, out PCLExamTypeID_New);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_Save_IsLab. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_Save_IsLab);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }


        public void PCLExamTypes_MarkDelete(long PCLExamTypeID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_MarkDelete(PCLExamTypeID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypes_MarkDelete(PCLExamTypeID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_MarkDelete(PCLExamTypeID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<PCLExamType> PCLExamTypes_List_Paging(
            PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_List_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_List_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> PCLExamTypesByDeptLocationID_LAB(long DeptLocationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_LAB(DeptLocationID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_LAB(DeptLocationID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_LAB(DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypesByDeptLocationID_LAB. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_List_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> PCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID, DeptLocationID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID, DeptLocationID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypesByDeptLocationID_NotLAB(V_PCLMainCategory, PCLExamTypeSubCategoryID, DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypesByDeptLocationID_NotLAB. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_List_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypes_Save_General(
           PCLExamType Obj,
           bool IsInsert,
           bool TestItemIsExamType,
           string PCLExamTestItemUnitForPCLExamType,
           string PCLExamTestItemRefScaleForPCLExamType,
           long StaffID,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
           out string Result, out long PCLExamTypeID_New)
        {
            try
            {
                string mainCacheKey = "AllPCLExamType";
                if (ServerAppConfig.CachingEnabled && (List<PCLExamType>)AxCache.Current[mainCacheKey] != null)
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_Save_General(Obj, IsInsert,
                                                                                    TestItemIsExamType,
                                                                                    PCLExamTestItemUnitForPCLExamType,
                                                                                    PCLExamTestItemRefScaleForPCLExamType,
                                                                                    StaffID,
                                                                                    DataPCLExamTestItems_Insert,
                                                                                    DataPCLExamTestItems_Update,
                                                                                    DataPCLExamTestItems_Delete,
                                                                                    out Result, out PCLExamTypeID_New);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_Save_IsLab. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_Save_IsLab);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion

        #region PCLExamTypeCombo

        public List<PCLExamTypeCombo> PCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Search(SearchCriteria);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Search(SearchCriteria);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Search(SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeCombo_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamTypeComboItem> PCLExamTypeComboItems_ByComboID(long ComboID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_ByComboID(ComboID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_ByComboID(ComboID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_ByComboID(ComboID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeComboItems_ByComboID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamTypeComboItem> PCLExamTypeComboItems_All(long? PCLExamTypePriceListID)
        {
            try
            {
                List<PCLExamTypeComboItem> allExamTypes = new List<PCLExamTypeComboItem>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_All();
                //}
                //else
                //{
                //    allExamTypes = ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_All();
                //}
                allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeComboItems_All(PCLExamTypePriceListID);
                if (allExamTypes != null && allExamTypes.Count > 0)
                {
                    var allExams = allExamTypes.Select(x => x.PCLExamType);
                    List<PCLExamTypeLocation> deptLocations = new List<PCLExamTypeLocation>();
                    if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    {
                        deptLocations = aEMR.DataAccessLayer.Providers.ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allExams.ToList());
                    }
                    else
                    {
                        deptLocations = ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allExams.ToList());
                    }
                    if (deptLocations != null)
                    {
                        foreach (var examType in allExamTypes)
                        {
                            if (examType.PCLExamType != null)
                            {
                                examType.PCLExamType.PCLExamTypeLocations = new ObservableCollection<PCLExamTypeLocation>(deptLocations.Where(dl => dl.PCLExamTypeID == examType.PCLExamType.PCLExamTypeID));
                            }
                        }
                    }
                }
                return allExamTypes;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeComboItems_All. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, out long ID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Save(item, ComboXML_Insert, ComboXML_Update, ComboXML_Delete, out ID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Save(item, ComboXML_Insert, ComboXML_Update, ComboXML_Delete, out ID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Save(item, ComboXML_Insert, ComboXML_Update, ComboXML_Delete, out ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeCombo_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTypeCombo_Delete(long ID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Delete(ID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Delete(ID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeCombo_Delete(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeCombo_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region PCLExamTypeMedServiceDefItem

        public List<PCLExamType> PCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_ByMedServiceID(SearchCriteria, MedServiceID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_ByMedServiceID(SearchCriteria, MedServiceID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_ByMedServiceID(SearchCriteria, MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeMedServiceDefItems_ByMedServiceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeMedServiceDefItems_ByMedServiceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        //Save danh sach vua chon tu UI
        public bool PCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> objCollect)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_XMLInsert(MedServiceID, objCollect);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_XMLInsert(MedServiceID, objCollect);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeMedServiceDefItems_XMLInsert(MedServiceID, objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeMedServiceDefItems_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeMedServiceDefItems_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Save danh sach vua chon tu UI

        #endregion

        #region DeptMedServiceItems
        //DeptMedServiceItems
        public List<MedServiceItemPrice> GetDeptMedServiceItems_Paging(DeptMedServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptMedServiceItems_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptMedServiceItems> GetDeptMedServiceItems_DeptIDPaging(DeptMedServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_DeptIDPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_DeptIDPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptMedServiceItems_DeptIDPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptMedServiceItems_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<MedServiceItemPrice> GetMedServiceItemPrice_Paging(
            MedServiceItemPriceSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            )
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetMedServiceItemPrice_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetMedServiceItemPrice_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetMedServiceItemPrice_Paging(Criteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptMedServiceItems_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeptMedServiceItems_TrueDelete(long DeptMedServItemID, long MedServItemPriceID, long MedServiceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_TrueDelete(DeptMedServItemID, MedServItemPriceID, MedServiceID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptMedServiceItems_TrueDelete(DeptMedServItemID, MedServItemPriceID, MedServiceID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_TrueDelete(DeptMedServItemID, MedServItemPriceID, MedServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //DeptMedServiceItems

        public bool DeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_InsertXML(lstDeptMedServiceItems);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptMedServiceItems_InsertXML(lstDeptMedServiceItems);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_InsertXML(lstDeptMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_DeleteXML(lstDeptMedServiceItems);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptMedServiceItems_DeleteXML(lstDeptMedServiceItems);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_DeleteXML(lstDeptMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region RefMedicalServiceTypes
        public IList<RefMedicalServiceType> GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(DeptID, MedServiceName);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(DeptID, MedServiceName);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(DeptID, MedServiceName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GroupRefMedicalServiceTypes_ByMedicalServiceTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GroupRefMedicalServiceTypes_ByMedicalServiceTypeID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefMedicalServiceType> GetAllMedicalServiceTypes()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllMedicalServiceTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllMedicalServiceTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefMedicalServiceType> RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefMedicalServiceType> GetAllMedicalServiceTypes_SubtractPCL()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes_SubtractPCL();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes_SubtractPCL();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllMedicalServiceTypes_SubtractPCL();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllMedicalServiceTypes_SubtractPCL. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllMedicalServiceTypes_SubtractPCL);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefMedicalServiceType> RefMedicalServiceTypes_Paging(RefMedicalServiceTypeSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceTypes_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceTypes_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_AddEdit(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_AddEdit(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_AddEdit(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceTypes_AddEdit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceTypes_AddEdit);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RefMedicalServiceTypes_MarkDelete(long MedicalServiceTypeID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_MarkDelete(MedicalServiceTypeID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_MarkDelete(MedicalServiceTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_MarkDelete(MedicalServiceTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceTypes_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceTypes_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefMedicalServiceTypes_CheckBeforeInsertUpdate(RefMedicalServiceType Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_CheckBeforeInsertUpdate(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_CheckBeforeInsertUpdate(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_CheckBeforeInsertUpdate(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceTypes_CheckBeforeInsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceTypes_CheckBeforeInsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefMedicalServiceItems_IsPCL_Save(RefMedicalServiceItem Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCL_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCL_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_IsPCL_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_IsPCL_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_IsPCL_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void RefMedicalServiceItems_EditInfo(RefMedicalServiceItem Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_EditInfo(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceItems_EditInfo(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_EditInfo(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_EditInfo. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_EditInfo);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefMedicalServiceItems_MarkDeleted(long MedServiceID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_MarkDeleted(MedServiceID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceItems_MarkDeleted(MedServiceID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_MarkDeleted(MedServiceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_MarkDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_MarkDeleted);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void RefMedicalServiceItems_NotPCL_Add(DeptMedServiceItems Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_NotPCL_Add. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_NotPCL_Add);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RefMedicalServiceItems_NotPCL_Insert(RefMedicalServiceItem Obj, long StaffID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj);
                //}
                string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null)
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Add(Obj, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_NotPCL_Add. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_NotPCL_Add);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RefMedicalServiceItems_NotPCL_Update(RefMedicalServiceItem Obj)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Update(Obj);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Update(Obj);
                //}
                string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null)
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceItems_NotPCL_Update(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceItems_NotPCL_Add. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceItems_NotPCL_Add);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<RefMedicalServiceType> RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(V_RefMedicalServiceTypes);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(V_RefMedicalServiceTypes);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(V_RefMedicalServiceTypes);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceTypes_ByV_RefMedicalServiceTypes. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceTypes_ByV_RefMedicalServiceTypes);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region "MedServiceItemPrice"
        public List<MedServiceItemPrice> MedServiceItemPriceByDeptMedServItemID_Paging(MedServiceItemPriceSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceByDeptMedServItemID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.MedServiceItemPriceByDeptMedServItemID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceByDeptMedServItemID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceByDeptMedServItemID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceByDeptMedServItemID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void MedServiceItemPrice_Save(MedServiceItemPrice Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPrice_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPrice_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPrice_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_MarkDelete(MedServItemPriceID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPrice_MarkDelete(MedServItemPriceID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_MarkDelete(MedServItemPriceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPrice_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPrice_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public MedServiceItemPrice MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_ByMedServItemPriceID(MedServItemPriceID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.MedServiceItemPrice_ByMedServItemPriceID(MedServItemPriceID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPrice_ByMedServItemPriceID(MedServItemPriceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPrice_ByMedServItemPriceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPrice_ByMedServItemPriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region PCLItemID
        //public void PCLItemsInsertUpdate(PCLItem Obj, bool SaveToDB, out string Result)
        //{
        //    try
        //    {
        //        ConfigurationManagerProviders.Instance.PCLItemsInsertUpdate(Obj, SaveToDB, out Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PCLItemsInsertUpdate. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItemsInsertUpdate);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public bool PCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_XMLInsert(PCLFormID, ObjPCLExamTypeList);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLItems_XMLInsert(PCLFormID, ObjPCLExamTypeList);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_XMLInsert(PCLFormID, ObjPCLExamTypeList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLItems_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public List<PCLItem> PCLItems_GetPCLExamTypeIDByMedServiceID(long MedServiceID)
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.PCLItems_GetPCLExamTypeIDByMedServiceID(MedServiceID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PCLItems_GetPCLExamTypeIDByMedServiceID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_GetPCLExamTypeIDByMedServiceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public List<PCLExamType> PCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_GetPCLExamTypeIDByPCLSectionID(PCLExamTypeName, PCLSectionID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLItems_GetPCLExamTypeIDByPCLSectionID(PCLExamTypeName, PCLSectionID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_GetPCLExamTypeIDByPCLSectionID(PCLExamTypeName, PCLSectionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLItems_GetPCLExamTypeIDByPCLSectionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_GetPCLExamTypeIDByPCLSectionID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PCLExamTypeLocation> GetPCLExamTypeLocations(List<PCLExamType> allPCLExamTypes)
        {
            try
            {
                List<PCLExamTypeLocation> deptLocations = new List<PCLExamTypeLocation>();
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    deptLocations = aEMR.DataAccessLayer.Providers.ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allPCLExamTypes);
                }
                else
                {
                    deptLocations = ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allPCLExamTypes);
                }
                return deptLocations;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading pclexamtype locations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_GetPCLExamTypeLocations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PCLExamType> PCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID, long? PCLExamTypePriceListID)
        {
            try
            {
                List<PCLExamType> allExamTypes = new List<PCLExamType>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_ByPCLFormID(SearchCriteria, PCLFormID);
                //}
                //else
                //{
                //    allExamTypes = ConfigurationManagerProviders.Instance.PCLItems_ByPCLFormID(SearchCriteria, PCLFormID);
                //}
                string mainCacheKey = "AllPCLExamType";
                if (PCLExamTypePriceListID > 0)
                {
                    allExamTypes = ConfigurationManagerProviders.Instance.PCLItems_ByPCLFormID(SearchCriteria, PCLFormID, PCLExamTypePriceListID);
                }
                else if (ServerAppConfig.CachingEnabled && (List<PCLExamType>)AxCache.Current[mainCacheKey] != null)
                {
                    allExamTypes = (List<PCLExamType>)AxCache.Current[mainCacheKey];
                }
                else
                {
                    allExamTypes = ConfigurationManagerProviders.Instance.PCLItems_ByPCLFormID(SearchCriteria, PCLFormID, null);
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allExamTypes;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allExamTypes, new TimeSpan(0, 0, int.MaxValue), true);
                    }
                }
                //allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_ByPCLFormID(SearchCriteria, PCLFormID);
                if (allExamTypes != null && allExamTypes.Count > 0)
                {
                    List<PCLExamTypeLocation> deptLocations = new List<PCLExamTypeLocation>();
                    if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                    {
                        deptLocations = aEMR.DataAccessLayer.Providers.ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allExamTypes);
                    }
                    else
                    {
                        deptLocations = ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allExamTypes);
                    }
                    if (deptLocations != null)
                    {
                        foreach (var examType in allExamTypes)
                        {
                            examType.PCLExamTypeLocations = new ObservableCollection<PCLExamTypeLocation>(deptLocations.Where(dl => dl.PCLExamTypeID == examType.PCLExamTypeID));
                        }
                    }
                }
                return allExamTypes;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLItems_ByPCLFormID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_ByPCLFormID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> PCLExamType_WithDeptLocIDs_GetAll()
        {
            try
            {
                List<PCLExamType> allExamTypes = new List<PCLExamType>();
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamType_WithDeptLocIDs_GetAll();
                //}
                //else
                //{
                //    allExamTypes = ConfigurationManagerProviders.Instance.PCLExamType_WithDeptLocIDs_GetAll();
                //}
                allExamTypes = aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamType_WithDeptLocIDs_GetAll();
                return allExamTypes;
            }
            catch (Exception ex)
            {
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLItems_ByPCLFormID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            //return null;
        }

        public List<PCLExamType> PCLItems_SearchAutoComplete(
           PCLExamTypeSearchCriteria SearchCriteria,

           int PageIndex,
           int PageSize,
           string OrderBy,
           bool CountTotal,
           out int Total
           )
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_SearchAutoComplete(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLItems_SearchAutoComplete(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLItems_SearchAutoComplete(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLItems_SearchAutoComplete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PCL_PCLItems_SearchAutoComplete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> GetPCLExamType_byComboID(long PCLExamTypeComboID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamType_byComboID(PCLExamTypeComboID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetPCLExamType_byComboID(PCLExamTypeComboID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamType_byComboID(PCLExamTypeComboID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamType_byComboID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_ApptService_XMLSave);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PCLExamType> GetPCLExamType_byHosID(long HosID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamType_byHosID(HosID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetPCLExamType_byHosID(HosID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPCLExamType_byHosID(HosID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLExamType_byHosID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.APPOINTMENT_ApptService_XMLSave);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region PCLForms

        public List<PCLForm> PCLForms_GetList_Paging(PCLFormsSearchCriteria SearchCriteria,
   int PageIndex,
   int PageSize,
   string OrderBy,
   bool CountTotal,
   out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLForms_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLForms_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLForms_Save(PCLForm Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLForms_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLForms_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLForms_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLForms_MarkDelete(Int64 PCLFormID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_MarkDelete(PCLFormID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLForms_MarkDelete(PCLFormID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLForms_MarkDelete(PCLFormID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLForms_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLForms_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region PCLSections

        public List<PCLSection> PCLSections_All()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_All();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLSections_All();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLSections_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLSections_All);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼==== #004
        public List<PCLExamType> PCLExamTypes_All()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypes_All();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypes_All. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetPCLExamTypes_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #004

        //public List<PCLSection> PCLSectionsByPCLFormID(long PCLFormID)
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.PCLSectionsByPCLFormID(PCLFormID);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of PCLSectionsByPCLFormID. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLSectionsByPCLFormID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public List<PCLSection> PCLSections_GetList_Paging(PCLSectionsSearchCriteria SearchCriteria,
int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLSections_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLSections_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLSections_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLSections_Save(PCLSection Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLSections_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLSections_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLSections_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLSections_MarkDelete(Int64 PCLSectionID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_MarkDelete(PCLSectionID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLSections_MarkDelete(PCLSectionID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLSections_MarkDelete(PCLSectionID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLSections_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLSections_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region Locations
        public void Locations_InsertUpdate(Location Obj, bool SaveToDB, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_InsertUpdate(Obj, SaveToDB, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.Locations_InsertUpdate(Obj, SaveToDB, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_InsertUpdate(Obj, SaveToDB, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool Locations_XMLInsert(Location objCollect)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_XMLInsert(objCollect);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.Locations_XMLInsert(objCollect);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_XMLInsert(objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //list paging
        public List<Location> Locations_ByRmTypeID_Paging(LocationSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_ByRmTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.Locations_ByRmTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_ByRmTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_ByRmTypeID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //list paging

        public void Locations_MarkDeleted(Int64 LID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_MarkDeleted(LID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.Locations_MarkDeleted(LID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_MarkDeleted(LID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_MarkDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_MarkDeleted);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region RoomType
        public List<RoomType> RoomType_GetAll()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetAll();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RoomType_GetAll();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RoomType> RoomType_GetList_Paging(RoomTypeSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RoomType_Save(RoomType Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RoomType_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RoomType_MarkDelete(long RmTypeID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region ICD
        public List<ICD> ICD_GetAll()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DiseaseChapters> DiseaseChapters_GetAll()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseChapters_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseChapters_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DiseaseChapters> Chapter_Paging(string SearchChapter, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Chapter_Paging(SearchChapter, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Chapter_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<Diseases> Diseases_Paging(int DiseaseChapterID, string SearchDisease, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Diseases_Paging(DiseaseChapterID, SearchDisease, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Diseases_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ICD> ICD_ByIDCode_Paging(ICDSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_ByIDCode_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_ByIDCode_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ICD> ICD_ByDiseaseID_Paging(long Disease_ID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_ByDiseaseID_Paging(Disease_ID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_ByIDCode_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<Diseases> Diseases_ByChapterID(int DiseaseChapterID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Diseases_ByChapterID(DiseaseChapterID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Diseases_ByChapterID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void Chapter_Save(DiseaseChapters Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Chapter_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseChapters_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void Diseases_Save(Diseases Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Diseases_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Diseases_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void ICD_Save(ICD Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void ICD_MarkDelete(long IDCode, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_MarkDelete(IDCode, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool ICD_XMLInsert(Int64 Disease_ID, IEnumerable<ICD> objCollect)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_XMLInsert(DeptLocationID, objCollect);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocMedServices_XMLInsert(DeptLocationID, objCollect);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_XMLInsert(Disease_ID, objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ICD_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<ICD> SearchICD_Paging(long DiseaseChapterID, long Disease_ID, string ICD10Code, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SearchICD_Paging(DiseaseChapterID,Disease_ID, ICD10Code, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchICD_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region InsuranceBenefit

        public List<InsuranceBenefitCategories_Data> InsuranceBenefitPaging(string HIPCode, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.InsuranceBenefitPaging(HIPCode, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsuranceBenefitCategories_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void InsuranceBenefitCategories_Save(InsuranceBenefitCategories_Data Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Locations_InsertUpdate(Obj, SaveToDB, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.Locations_InsertUpdate(Obj, SaveToDB, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.InsuranceBenefitCategories_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsuranceBenefitCategories_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public void ICD_MarkDelete(long IDCode, out string Result)
        //{
        //    try
        //    {
        //        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
        //        //{
        //        //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
        //        //}
        //        //else
        //        //{
        //        //    ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
        //        //}
        //        aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_MarkDelete(IDCode, out Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        #endregion

        #region Hospital
        public List<Hospital> HospitalPaging(HospitalSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.HospitalPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of HospitalPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public void ICD_MarkDelete(long IDCode, out string Result)
        //{
        //    try
        //    {
        //        //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
        //        //{
        //        //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
        //        //}
        //        //else
        //        //{
        //        //    ConfigurationManagerProviders.Instance.RoomType_MarkDelete(RmTypeID, out Result);
        //        //}
        //        aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_MarkDelete(IDCode, out Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        #endregion

        #region CitiesProvince

        public List<CitiesProvince> CitiesProvince_Paging(string SearchCitiesProvinces, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.CitiesProvince_Paging(SearchCitiesProvinces, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<SuburbNames> SuburbNames_Paging(long CityProvinceID, string SearchSuburbNames, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SuburbNames_Paging(CityProvinceID, SearchSuburbNames, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<WardNames> WardNames_Paging(long CityProvinceID, long SuburbNameID, string SearchWardNames, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.WardNames_Paging(CityProvinceID, SuburbNameID, SearchWardNames, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void CitiesProvinces_Save(CitiesProvince Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.CitiesProvinces_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CitiesProvinces_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void SuburbNames_Save(SuburbNames Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SuburbNames_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CitiesProvinces_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void WardNames_Save(WardNames Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.WardNames_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CitiesProvinces_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //public void ICD_MarkDelete(long IDCode, out string Result)
        //{
        //    try
        //    {
        //        aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_MarkDelete(IDCode, out Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        #endregion

        #region Job
        public List<Lookup> Job_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Job_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void Job_Save(Lookup Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Job_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //public void ICD_MarkDelete(long IDCode, out string Result)
        //{
        //    try
        //    {
        //        aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ICD_MarkDelete(IDCode, out Result);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        #endregion

        #region AdmissionCriteria
        public List<AdmissionCriteria> AdmissionCriteria_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AdmissionCriteria_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AdmissionCriteria Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void AdmissionCriteria_Save(AdmissionCriteria Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AdmissionCriteria_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AdmissionCriteria InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<AdmissionCriteria> GetListAdmissionCriteria()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetListAdmissionCriteria();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion
        #region AdmissionCriterion
        public List<SymptomCategory> SymptomCategory_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SymptomCategory_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SymptomCategory Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void SymptomCategory_Save(SymptomCategory Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SymptomCategory_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SymptomCategory InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<SymptomCategory> GetAllSymptom()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllSymptom();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllSymptom Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<AdmissionCriterion> AdmissionCriterion_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AdmissionCriterion_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AdmissionCriterion Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void AdmissionCriterion_Save(AdmissionCriterion Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.AdmissionCriterion_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AdmissionCriterion InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<AdmissionCriterionAttachICD> GetICDListByAdmissionCriterionID(long AdmissionCriterionID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetICDListByAdmissionCriterionID(AdmissionCriterionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetICDListBySymptomID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertAdmissionCriterionAttachICD(List<AdmissionCriterionAttachICD> listAdmissionCriterionAttachICD, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.InsertAdmissionCriterionAttachICD(listAdmissionCriterionAttachICD, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERT AdmissionCriterionAttachICD. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeleteAdmissionCriterionAttachICD(long ACAI_ID, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeleteAdmissionCriterionAttachICD(ACAI_ID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteSymptomAttachICD. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        } 

        public List<AdmissionCriterionAttachSymptom> GetSymptomListByAdmissionCriterionID(long AdmissionCriterionID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetSymptomListByAdmissionCriterionID(AdmissionCriterionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSymptomListByAdmissionCriterionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertAdmissionCriterionAttachSymptom(List<AdmissionCriterionAttachSymptom> listAdmissionCriterionAttachSymptom, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.InsertAdmissionCriterionAttachSymptom(listAdmissionCriterionAttachSymptom, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERT AdmissionCriterionAttachSymptom. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteAdmissionCriterionAttachSymptom(long ACAS_ID, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeleteAdmissionCriterionAttachSymptom(ACAS_ID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteAdmissionCriterionAttachSymptom. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<AdmissionCriterionAttachICD> GetAllAdmissionCriterionAttachICD()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllAdmissionCriterionAttachICD();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllAdmissionCriterionAttachICD. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<GroupPCLs> GroupPCLs_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GroupPCLs_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GroupPCLs Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void GroupPCLs_Save(GroupPCLs Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GroupPCLs_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GroupPCLs InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<GroupPCLs> GetAllGroupPCLs()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllGroupPCLs();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllGroupPCLs . Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<AdmissionCriterionAttachGroupPCL> GetGroupPCLListByAdmissionCriterionID(long AdmissionCriterionID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetGroupPCLListByAdmissionCriterionID(AdmissionCriterionID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetGroupPCLListBySymptomID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool InsertAdmissionCriterionAttachGroupPCL(List<AdmissionCriterionAttachGroupPCL> listAdmissionCriterionAttachGroupPCL, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.InsertAdmissionCriterionAttachGroupPCL(listAdmissionCriterionAttachGroupPCL, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of INSERT AdmissionCriterionAttachGroupPCL. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteAdmissionCriterionAttachGroupPCL(long ACAG_ID, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeleteAdmissionCriterionAttachGroupPCL(ACAG_ID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteAdmissionCriterionAttachGroupPCL. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PCLExamType> PCLExamType_ByGroupPCLID(long GroupPCLID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamType_ByGroupPCLID(GroupPCLID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamType_ByGroupPCLID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool PCLExamTypeGroupPCL_XMLInsert(long GroupPCLID, IEnumerable<PCLExamType> ObjList, long StaffID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeGroupPCL_XMLInsert(GroupPCLID, ObjList,StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamType_ByGroupPCLID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<GroupPCLs> GroupPCL_PCLExamType_ByAdmissionCriterionID(long AdmissionCriterionID,long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GroupPCL_PCLExamType_ByAdmissionCriterionID(AdmissionCriterionID, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GroupPCL_PCLExamType_ByAdmissionCriterionID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public AdmissionCriterionDetail GetAdmissionCriterionDetailByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAdmissionCriterionDetailByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAdmissionCriterionDetailByPtRegistrationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "GetAdmissionCriterionDetailByPtRegistrationID");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool SaveAdmissionCriterionDetail(AdmissionCriterionDetail CurrentAdmissionCriterionDetail)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SaveAdmissionCriterionDetail(CurrentAdmissionCriterionDetail);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveAdmissionCriterionDetail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "SaveAdmissionCriterionDetail");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool SaveAdmissionCriterionDetail_PCLResult(AdmissionCriterionDetail_PCLResult CurrentAdmissionCriterionDetail_PCLResult)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.SaveAdmissionCriterionDetail_PCLResult(CurrentAdmissionCriterionDetail_PCLResult);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveAdmissionCriterionDetail_PCLResult. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "SaveAdmissionCriterionDetail_PCLResult");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region BedCategory
        public List<BedCategory> BedCategory_Paging(BedCategorySearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.BedCategory_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<BedCategory> BedCategory_ByDeptLocID_Paging(BedCategorySearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.BedCategory_ByDeptLocID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RoomType_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool BedCategory_InsertXML(IList<BedCategory> lstBedCategory)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.BedCategory_InsertXML(lstBedCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of BedCategory Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool BedCategory_DeleteXML(IList<BedCategory> lstBedCategory)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.BedCategory_DeleteXML(lstBedCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of BedCategory Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool CheckValidBedCategory(long BedCategoryID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.CheckValidBedCategory(BedCategoryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of BedCategory Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void BedCategory_Save(BedCategory Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.BedCategory_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Locations_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region DeptLocation
        public List<DeptLocation> GetDeptLocationFunc(long V_DeptType, long V_RoomFunction)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptLocationFunc(V_DeptType, V_RoomFunction);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetDeptLocationFunc(V_DeptType, V_RoomFunction);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptLocationFunc(V_DeptType, V_RoomFunction);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptLocationFunc. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptLocationFunc);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptLocationFuncExt(V_DeptType, V_RoomFunction, V_DeptTypeOperation);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetDeptLocationFuncExt(V_DeptType, V_RoomFunction, V_DeptTypeOperation);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetDeptLocationFuncExt(V_DeptType, V_RoomFunction, V_DeptTypeOperation);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptLocationFunc. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptLocationFunc);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<Location> DeptLocation_ByDeptID(long DeptID, long RmTypeID, string LocationName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_ByDeptID(DeptID, RmTypeID, LocationName);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocation_ByDeptID(DeptID, RmTypeID, LocationName);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_ByDeptID(DeptID, RmTypeID, LocationName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_ByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_ByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RoomType> DeptLocation_GetRoomTypeByDeptID(long DeptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_GetRoomTypeByDeptID(DeptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocation_GetRoomTypeByDeptID(DeptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_GetRoomTypeByDeptID(DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_GetRoomTypeByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_GetRoomTypeByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeptLocation_CheckLIDExists(long DeptID, long LID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_CheckLIDExists(DeptID, LID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocation_CheckLIDExists(DeptID, LID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_CheckLIDExists(DeptID, LID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_CheckLIDExists. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_CheckLIDExists);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeptLocation_XMLInsert(long DeptID, IEnumerable<Location> objCollect)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_XMLInsert(DeptID, objCollect);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocation_XMLInsert(DeptID, objCollect);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_XMLInsert(DeptID, objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void DeptLocation_MarkDeleted(long DeptLocationID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_MarkDeleted(DeptLocationID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.DeptLocation_MarkDeleted(DeptLocationID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_MarkDeleted(DeptLocationID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_MarkDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_MarkDeleted);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> DeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_ByMedicalServiceTypeIDDeptID(MedicalServiceTypeID, DeptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocation_ByMedicalServiceTypeIDDeptID(MedicalServiceTypeID, DeptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocation_ByMedicalServiceTypeIDDeptID(MedicalServiceTypeID, DeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocation_ByMedicalServiceTypeIDDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_ByMedicalServiceTypeIDDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllLocationsByDeptID(long? deptID, long? V_RoomFunction = null)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID, V_RoomFunction);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID, V_RoomFunction);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID, V_RoomFunction);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllLocationsByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllLocationsByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllLocationsByDeptIDOld(long? deptID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllLocationsByDeptID(deptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllLocationsByDeptID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetAllLocationsByDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> ListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID)
        {
            try
            {
                if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                {
                    return aEMR.DataAccessLayer.Providers.DataProviderBase.MAPPCLExamTypeDeptLoc[PCLExamTypeID].ObjDeptLocationList.ToList();
                }
                else
                {
                    return DataProviderBase.MAPPCLExamTypeDeptLoc[PCLExamTypeID].ObjDeptLocationList.ToList();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ListDeptLocation_ByPCLExamTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocation_ByMedicalServiceTypeIDDeptID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region DeptLocMedServices
        //public List<RefMedicalServiceItem> DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(long DeptID, long MedicalServiceTypeID, string MedServiceName)
        //{
        //    try
        //    {
        //        return ConfigurationManagerProviders.Instance.DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(DeptID, MedicalServiceTypeID, MedServiceName);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public List<RefMedicalServiceType> DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(long DeptID, long MedicalServiceTypeID, string MedServiceName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(DeptID, MedicalServiceTypeID, MedServiceName);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(DeptID, MedicalServiceTypeID, MedServiceName);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(DeptID, MedicalServiceTypeID, MedServiceName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_GroupMedSerTypeID_AllForChoose. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_GroupMedSerTypeID_AllForChoose);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Ở DB
        public List<RefMedicalServiceItem> DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(long DeptID, Int64 DeptLocationID, long MedicalServiceTypeID, string MedServiceName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefMedicalServiceType> DeptLocMedServices_GroupMedSerTypeID_HasChoose(long DeptID, Int64 DeptLocationID, long MedicalServiceTypeID, string MedServiceName)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_GroupMedSerTypeID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocMedServices_GroupMedSerTypeID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_GroupMedSerTypeID_HasChoose(DeptID, DeptLocationID, MedicalServiceTypeID, MedServiceName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocMedServices_GroupMedSerTypeID_HasChoose. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_GroupMedSerTypeID_HasChoose);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //Ở DB

        public bool DeptLocMedServices_XMLInsert(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_XMLInsert(DeptLocationID, objCollect);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.DeptLocMedServices_XMLInsert(DeptLocationID, objCollect);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DeptLocMedServices_XMLInsert(DeptLocationID, objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptLocMedServices_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region PCLExamTypePrices
        public List<PCLExamTypePrice> PCLExamTypePrices_ByPCLExamTypeID_Paging(PCLExamTypePriceSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypeID_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePrices_ByPCLExamTypeID_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePrices_ByPCLExamTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypePrices_Save(PCLExamTypePrice Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePrices_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePrices_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePrices_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypePrices_MarkDelete(long PCLExamTypePriceID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_MarkDelete(PCLExamTypePriceID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePrices_MarkDelete(PCLExamTypePriceID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_MarkDelete(PCLExamTypePriceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePrices_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePrices_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PCLExamTypePrice PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypePriceID(PCLExamTypePriceID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypePriceID(PCLExamTypePriceID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePrices_ByPCLExamTypePriceID(PCLExamTypePriceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePrices_ByPCLExamTypePriceID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePrices_ByPCLExamTypePriceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region PCLGroups
        public IList<PCLGroup> PCLGroups_GetAll(long? V_PCLCategory)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_GetAll(V_PCLCategory);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLGroups_GetAll(V_PCLCategory);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_GetAll(V_PCLCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLGroups_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<PCLGroup> PCLGroups_GetList_Paging(PCLGroupsSearchCriteria SearchCriteria,
  int PageIndex,
  int PageSize,
  string OrderBy,
  bool CountTotal,
  out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLGroups_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLGroups_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLGroups_Save(PCLGroup Obj, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_Save(Obj, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLGroups_Save(Obj, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLGroups_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLGroups_MarkDelete(Int64 PCLGroupID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_MarkDelete(PCLGroupID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLGroups_MarkDelete(PCLGroupID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLGroups_MarkDelete(PCLGroupID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLGroups_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        #endregion

        #region MedServiceItemPriceList
        public void MedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, out string Result_PriceList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_AddNew(Obj, ObjMedServiceItemPrice, out Result_PriceList);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPriceList_AddNew(Obj, ObjMedServiceItemPrice, out Result_PriceList);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_AddNew(Obj, ObjMedServiceItemPrice, out Result_PriceList);
                string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                if (ServerAppConfig.CachingEnabled && ((List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null))
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_AddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_AddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<MedServiceItemPriceList> MedServiceItemPriceList_GetList_Paging(MedServiceItemPriceListSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total
            , out DateTime CurDate)
        {
            try
            {
                CurDate = DateTime.Now;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.MedServiceItemPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public void MedServiceItemPriceList_MarkDelete(Int64 MedServiceItemPriceListID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_MarkDelete(MedServiceItemPriceListID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPriceList_MarkDelete(MedServiceItemPriceListID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_MarkDelete(MedServiceItemPriceListID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_MarkDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //MedServiceItemPriceList_Detail
        public List<MedServiceItemPrice> MedServiceItemPriceList_Detail(DeptMedServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Detail(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Detail(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Detail(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_Detail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_Detail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Update
        public void MedServiceItemPriceList_Update(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, out string Result_PriceList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Update(Obj, ObjCollection, ObjCollection_Insert, ObjCollection_Update, out Result_PriceList);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Update(Obj, ObjCollection, ObjCollection_Insert, ObjCollection_Update, out Result_PriceList);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_Update(Obj, ObjCollection, ObjCollection_Insert, ObjCollection_Update, out Result_PriceList);
                string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                if (ServerAppConfig.CachingEnabled && ((List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null))
                {
                    AxCache.Current.Remove(mainCacheKey);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Check CanAddNew
        public void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, out bool CanAddNew)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_CheckCanAddNew(DeptID, MedicalServiceTypeID, out CanAddNew);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.MedServiceItemPriceList_CheckCanAddNew(DeptID, MedicalServiceTypeID, out CanAddNew);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MedServiceItemPriceList_CheckCanAddNew(DeptID, MedicalServiceTypeID, out CanAddNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MedServiceItemPriceList_CheckCanAddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MedServiceItemPriceList_CheckCanAddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        #region "PCLExamTypePriceList"
        //Check CanAddNew
        public void PCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_CheckCanAddNew(out CanAddNew);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePriceList_CheckCanAddNew(out CanAddNew);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_CheckCanAddNew(out CanAddNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_CheckCanAddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_CheckCanAddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //AddNew
        public void PCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, out string Result_PriceList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_AddNew(Obj, ObjPCLExamType, out Result_PriceList);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePriceList_AddNew(Obj, ObjPCLExamType, out Result_PriceList);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_AddNew(Obj, ObjPCLExamType, out Result_PriceList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_AddNew. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_AddNew);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //PriceList
        public List<PCLExamTypePriceList> PCLExamTypePriceList_GetList_Paging(
        PCLExamTypePriceListSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total, out DateTime curDate
        )
        {
            try
            {
                curDate = DateTime.Now;
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypePriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Delete PriceList
        public void PCLExamTypePriceList_Delete(Int64 PCLExamTypePriceListID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Delete(PCLExamTypePriceListID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Delete(PCLExamTypePriceListID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Delete(PCLExamTypePriceListID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_Delete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //PCLExamTypePriceList_Detail
        public List<PCLExamType> PCLExamTypePriceList_Detail(long PCLExamTypePriceListID,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Detail(PCLExamTypePriceListID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Detail(PCLExamTypePriceListID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Detail(PCLExamTypePriceListID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_Detail. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_Detail);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //Update
        public void PCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, out string Result_PriceList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Update(Obj, ObjCollection_Update, out Result_PriceList);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Update(Obj, ObjCollection_Update, out Result_PriceList);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypePriceList_Update(Obj, ObjCollection_Update, out Result_PriceList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePriceList_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypePriceList_Update);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion


        #region PCLExamTestItems
        public List<PCLExamTypeTestItems> PCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTestItems_ByPCLExamTypeID(PCLExamTypeID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTestItems_ByPCLExamTypeID(PCLExamTypeID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTestItems_ByPCLExamTypeID(PCLExamTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTestItems_ByPCLExamTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTestItems_ByPCLExamTypeID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region PCLResultParamImplementations
        public List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLResultParamImplementations_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLResultParamImplementations_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /*==== #001 ====*/
        public List<PCLResultParamImplementations> GetPCLResultParamByCatID(long? PCLExamTypeSubCategoryID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll(PCLExamTypeSubCategoryID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll(PCLExamTypeSubCategoryID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLResultParamImplementations_GetAll(PCLExamTypeSubCategoryID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLResultParamImplementations_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLResultParamImplementations_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        /*==== #001 ====*/
        #endregion

        #region PCLExamTypeSubCategory
        public List<PCLExamTypeSubCategory> PCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeSubCategory_ByV_PCLMainCategory(V_PCLMainCategory);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeSubCategory_ByV_PCLMainCategory(V_PCLMainCategory);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeSubCategory_ByV_PCLMainCategory(V_PCLMainCategory);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeSubCategory_ByV_PCLMainCategory. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeSubCategory_ByV_PCLMainCategory);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region PCLExamTypeLocations
        public IList<PCLExamType> PCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_ByDeptLocationID(PCLExamTypeName, DeptLocationID);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeLocations_ByDeptLocationID(PCLExamTypeName, DeptLocationID);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_ByDeptLocationID(PCLExamTypeName, DeptLocationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeLocations_ByDeptLocationID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_XMLInsert(DeptLocationID, ObjList);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeLocations_XMLInsert(DeptLocationID, ObjList);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_XMLInsert(DeptLocationID, ObjList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeLocations_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_MarkDeleted(PCLExamTypeID, DeptLocationID, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypeLocations_MarkDeleted(PCLExamTypeID, DeptLocationID, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeLocations_MarkDeleted(PCLExamTypeID, DeptLocationID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeLocations_MarkDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_MarkDeleted);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        #region "HITransactionType"
        public IList<HITransactionType> HITransactionType_GetListNoParentID()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of HITransactionType_GetListNoParentID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_HITransactionType_GetListNoParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== 25072018 TTM
        public IList<HITransactionType> HITransactionType_GetListNoParentID_New()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID_New();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID_New();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.HITransactionType_GetListNoParentID_New();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of HITransactionType_GetListNoParentID_New. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_HITransactionType_GetListNoParentID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== 25072018 TTM
        #endregion


        #region "RefMedicalServiceGroups"
        public IList<RefMedicalServiceGroups> RefMedicalServiceGroups_GetAll()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceGroups_GetAll();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.RefMedicalServiceGroups_GetAll();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefMedicalServiceGroups_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefMedicalServiceGroups_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RefMedicalServiceGroups_GetAll);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region PCLExamTypeExamTestPrint
        public IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrint_GetList_Paging(
            PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_GetList_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExamTestPrint_GetList_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrintIndex_GetAll()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_GetAll();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_GetAll();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExamTestPrintIndex_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypeExamTestPrint_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_Save(ObjList, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_Save(ObjList, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrint_Save(ObjList, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExamTestPrint_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PCLExamTypeExamTestPrintIndex_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result)
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_Save(ObjList, out Result);
                //}
                //else
                //{
                //    ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_Save(ObjList, out Result);
                //}
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExamTestPrintIndex_Save(ObjList, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExamTestPrintIndex_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLGroups_Save);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion


        #region MAPPCLExamTypeDeptLoc
        public Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc()
        {
            try
            {
                //if (Globals.AxServerSettings.Servers.UseDataAccessLayerProvider)
                //{
                //    return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MAPPCLExamTypeDeptLoc();
                //}
                //else
                //{
                //    return ConfigurationManagerProviders.Instance.MAPPCLExamTypeDeptLoc();
                //}
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.MAPPCLExamTypeDeptLoc();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CONFIG_MANAGE_MAPPCLExamTypeDeptLoc. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_MAPPCLExamTypeDeptLoc);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //▼====: #002
        #region RefPurposeForAccountant
        public List<RefPurposeForAccountant> GetAllRefPurposeForAccountant()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetAllRefPurposeForAccountant();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllRefPurposeForAccountant. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //▲====: #002
        //▼====: #003
        #region FilterPrescriptionsHasHIPay
        public List<FilterPrescriptionsHasHIPay> GetFilterPrescriptionsHasHIPay()
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.GetFilterPrescriptionsHasHIPay();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetFilterPrescriptionsHasHIPay. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        //▲====: #003

        public bool ReCaching()
        {
            try
            {
                AxLogger.Instance.LogInfo("Service OnStart: BuildPCLExamTypeDeptLocMap");
                if (!PatientProvider.Instance.BuildPCLExamTypeDeptLocMap())
                {
                    AxLogger.Instance.LogInfo("Error on building: BuildPCLExamTypeDeptLocMap");
                }
                AxLogger.Instance.LogInfo("Service OnStart: BuildPclDeptLocationList ");
                if (!PatientProvider.Instance.BuildPclDeptLocationList())
                {
                    AxLogger.Instance.LogInfo("Error on building: BuildPclDeptLocationList");
                }
                AxLogger.Instance.LogInfo("Service OnStart: BuildAllServiceIdDeptLocMap ");
                if (!PatientProvider.Instance.BuildAllServiceIdDeptLocMap())
                {
                    AxLogger.Instance.LogInfo("Error on building: BuildAllServiceIdDeptLocMap");
                }
                AxLogger.Instance.LogInfo("Service OnStart: BuildAllRefGenericRelationMap ");
                try
                {
                    PatientProvider.Instance.BuildAllRefGenericRelationMap();
                }
                catch
                {
                    AxLogger.Instance.LogInfo("Error on building: BuildAllRefGenericRelationMap");
                }
                AxLogger.Instance.LogInfo("Service OnStart: Before Loading All Config Values");
                Globals.AxServerSettings = PatientProvider.Instance.GetApplicationConfigValues();
                AxLogger.Instance.LogInfo("Service OnStart: After Loading All Config Values");
                AxLogger.Instance.LogInfo("Service OnStart: Before Loading All Insurance Benefit Caterories");
                List<eHCMS.Configurations.InsuranceBenefitCategories> AxInsuranceBenefitCategory = PatientProvider.Instance.GetInsuranceBenefitCategoriesValues();
                AxLogger.Instance.LogInfo("Service OnStart: After Loading All Insurance Benefit Caterories");
                if (AxInsuranceBenefitCategory != null && AxInsuranceBenefitCategory.Count > 0 && !string.IsNullOrEmpty(Globals.AxServerSettings.CommonItems.ValidHIPattern))
                {
                    Globals.AxServerSettings.CommonItems.ValidHIPattern = Globals.AxServerSettings.CommonItems.ValidHIPattern.Replace("{0}", string.Join("|", AxInsuranceBenefitCategory.Select(x => x.HIPCode).ToArray()));
                    AxLogger.Instance.LogInfo(string.Format("Generated Check HI Card Valid Pattern: {0}", Globals.AxServerSettings.CommonItems.ValidHIPattern));
                    try
                    {
                        Globals.AxServerSettings.CommonItems.InsuranceBenefitCategories = AxInsuranceBenefitCategory.Select(x => new string[] { x.HIPCode, x.BenefitCode, x.IBeID.ToString() }).ToList();
                        AxLogger.Instance.LogInfo("Generated InsuranceBenefitCategories Dictionary");
                    }
                    catch
                    {
                        Globals.AxServerSettings.CommonItems.InsuranceBenefitCategories = null;
                    }
                }
                else
                {
                    Globals.AxServerSettings.CommonItems.ValidHIPattern = null;
                    AxLogger.Instance.LogInfo("Error on building: AxInsuranceBenefitCategory");
                }
                if (AxCache.Current["MedicalServiceItemsAndDeptLoc"] != null)
                {
                    AxCache.Current.Remove("MedicalServiceItemsAndDeptLoc");
                }
                if (AxCache.Current["AllPCLExamType"] != null)
                {
                    AxCache.Current.Remove("AllPCLExamType");
                }
                if (AxCache.Current["LookupValues_0"] != null)
                {
                    AxCache.Current.Remove("LookupValues_0");
                }
                if (AxCache.Current["AllPositionInHospital"] != null)
                {
                    AxCache.Current.Remove("AllPositionInHospital");
                }
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ReCaching. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_CLINICDEPT_SearchRequestDrugInwardClinicDept);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<MedServiceItemPrice> GetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria SearchCriteria)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.GetBedAllocationAll_ByDeptID(SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDeptMedServiceItems_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PCLExamTestItems> PCLExamTestItem_ByPCLExamTypeID(long PCLExamTypeID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PCLExamTestItem_ByPCLExamTypeID(PCLExamTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTestItem_ByPCLExamTypeID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error PCLExamTestItem_ByPCLExamTypeID");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SavePCLExamTestItem(List<PCLExamTestItems> listDetail)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.SavePCLExamTestItem(listDetail);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SavePCLExamTestItem. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error SavePCLExamTestItem");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region Exemptions


        public bool Exemptions_InsertUpdate(PromoDiscountProgram objCollect, out string Result, out long NewID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Exemptions_InsertUpdate(objCollect, out Result, out NewID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Exemptions XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool ExemptionsMedServiceItems_InsertXML(IList<PromoDiscountItems> lstMedServiceItems)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ExemptionsMedServiceItems_InsertXML(lstMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool ExemptionsMedServiceItems_DeleteXML(IList<PromoDiscountItems> lstMedServiceItems)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ExemptionsMedServiceItems_DeleteXML(lstMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool PCLExamTypeExemptions_XMLInsert(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExemptions_XMLInsert(PromoDiscProgID, ObjList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExemptions XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //list paging
        public List<PromoDiscountProgram> Exemptions_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Exemptions_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Exemptions Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PromoDiscountItems> GetExemptionsMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetExemptionsMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetExemptionsMedServiceItems Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PCLExamType> PCLExamTypeExemptions(string PCLExamTypeName, Int64 PromoDiscProgID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.PCLExamTypeExemptions(PCLExamTypeName, PromoDiscProgID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypeExemptions. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //list paging

        public void Exemptions_MarkDeleted(Int64 PromoDiscProgID, out string Result)
        {
            try
            {
                
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.Exemptions_MarkDeleted(PromoDiscProgID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Exemptions MarkDeleted. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_MarkDeleted);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PromoDiscountProgram> GetAllExemptions()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllExemptions();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllExemptions Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PromoDiscountItems> GetPromoDiscountItems_ByID(long PromoDiscProgID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetPromoDiscountItems_ByID(PromoDiscProgID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPromoDiscountItems_ByID Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region  DiseaseProgression
        public List<DiseaseProgression> DiseaseProgression_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgression_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDiseaseProgression Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void DiseaseProgression_Save(DiseaseProgression Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgression_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseProgression_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void DiseaseProgression_MarkDelete(long DiseaseProgressionID, long StaffID, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgression_MarkDelete(DiseaseProgressionID, StaffID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseProgression MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DiseaseProgressionDetails> DiseaseProgressionDetails_Paging(long DiseaseProgressionID, string SearchDiseaseProgressionDetails, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgressionDetails_Paging(DiseaseProgressionID, SearchDiseaseProgressionDetails, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseProgressionDetails Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void DiseaseProgressionDetails_Save(DiseaseProgressionDetails Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgressionDetails_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseProgressionDetails InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void DiseaseProgressionDetail_MarkDelete(long DiseaseProgressionDetailID, long StaffID, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.DiseaseProgressionDetail_MarkDelete(DiseaseProgressionDetailID, StaffID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DiseaseProgressionDetail MarkDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Gói DVKT PackageTechnicalService
        public bool PackageTechnicalService_InsertUpdate(PackageTechnicalService objCollect, long LoggedStaffID, out string Result, out long NewID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalService_InsertUpdate(objCollect, LoggedStaffID, out Result, out NewID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PackageTechnicalService XMLInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_XMLInsert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PackageTechnicalServiceMedServiceItems_InsertXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalServiceMedServiceItems_InsertXML(lstMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeptMedServiceItems_TrueDelete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PackageTechnicalServiceMedServiceItems_DeleteXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalServiceMedServiceItems_DeleteXML(lstMedServiceItems);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PackageTechnicalServiceMedServiceItems_DeleteXML. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptMedServiceItems_TrueDelete);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLExamTypePackageTechnicalService_XMLInsert(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PCLExamTypePackageTechnicalService_XMLInsert(PackageTechnicalServiceID, ObjList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePackageTechnicalService XMLInsert. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_XMLInsert);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalService> PackageTechnicalService_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalService_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PackageTechnicalService Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalServiceDetail> GetPackageTechnicalServiceMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.GetPackageTechnicalServiceMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPackageTechnicalServiceMedServiceItems Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_GetDeptMedServiceItems_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PCLExamType> PCLExamTypePackageTechnicalService(string PCLExamTypeName, long PackageTechnicalServiceID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PCLExamTypePackageTechnicalService(PCLExamTypeName, PackageTechnicalServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PCLExamTypePackageTechnicalService. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypeLocations_ByDeptLocationID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void PackageTechnicalService_MarkDeleted(long PackageTechnicalServiceID, out string Result)
        {
            try
            {
                ConfigurationManagerProviders.Instance.Exemptions_MarkDeleted(PackageTechnicalServiceID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Exemptions MarkDeleted. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_MarkDeleted);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalService> GetAllPackageTechnicalServices()
        {
            try
            {
                return ConfigurationManagerProviders.Instance.GetAllPackageTechnicalServices();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllPackageTechnicalService Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_ByID(long PackageTechnicalServiceID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalServiceDetail_ByID(PackageTechnicalServiceID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPackageTechnicalService_ByID Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalService> PackageTechnicalService_Search(GeneralSearchCriteria SearchCriteria)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalService_Search(SearchCriteria);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PackageTechnicalService_Search. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_PCLExamTypes_NotYetPCLLabResultSectionID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_All(long? PCLExamTypePriceListID)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.PackageTechnicalServiceDetail_All(PCLExamTypePriceListID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PackageTechnicalServiceDetail_All Paging. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion
        #region Từ điển viết tắt
        public List<ShortHandDictionary> ShortHandDictionary_Paging(string SearchValue, long StaffID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ShortHandDictionary_Paging(SearchValue, StaffID, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ShortHandDictionary_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void ShortHandDictionary_Save(ShortHandDictionary Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.ShortHandDictionary_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ShortHandDictionary_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Cấu hình mã nhóm bệnh
        public List<OutpatientTreatmentType> GetOutpatientTreatmentType_Paging(string SearchCode, string SearchName, bool IsDelete, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetOutpatientTreatmentType_Paging(SearchCode, SearchName, IsDelete, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutpatientTreatmentType_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void OutPatientTreatmentType_Save(OutpatientTreatmentType Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutPatientTreatmentType_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutpatientTreatmentType_InsertUpdate. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutpatientTreatmentType> OutpatientTreatmentType_GetAll()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutpatientTreatmentType_GetAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutpatientTreatmentType_GetAll. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼==== #009
        public List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(long OutpatientTreatmentTypeID, string ICD10, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(OutpatientTreatmentTypeID, ICD10, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutpatientTreatmentType_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(long OutpatientTreatmentTypeID)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(OutpatientTreatmentTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetOutpatientTreatmentType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_RoomType_GetList_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #009

        public bool OutpatientTreatmentTypeICD10Link_XMLInsert(ObservableCollection<OutpatientTreatmentTypeICD10Link> objCollect)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutpatientTreatmentTypeICD10Link_XMLInsert(objCollect);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutpatientTreatmentTypeICD10Link_XMLInsert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼==== #009
        public bool OutpatientTreatmentTypeICD10Link_Edit(OutpatientTreatmentTypeICD10Link Obj)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.OutpatientTreatmentTypeICD10Link_Edit(Obj);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutpatientTreatmentTypeICD10Link_Edit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_DeptLocMedServices_XMLInsert);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #009
        #endregion
        #region Cấu hình RefApplicationConfig
        public List<RefApplicationConfig> RefApplicationConfig_Paging(string SearchRefApplicationConfig, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefApplicationConfig_Paging(SearchRefApplicationConfig, PageIndex, PageSize, OrderBy, CountTotal, out Total);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefApplicationConfig_Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error RefApplicationConfig_Paging");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RefApplicationConfig_Save(RefApplicationConfig Obj, out string Result)
        {
            try
            {
                aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.RefApplicationConfig_Save(Obj, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RefApplicationConfig_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error RefApplicationConfig_Save");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        //▼==== #006
        public List<Specimen> GetAllSpecimen()
        {
            try
            {
                return aEMR.DataAccessLayer.Providers.ConfigurationManagerProviders.Instance.GetAllSpecimen();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllSpecimen Paging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_ByRmTypeID_Paging);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #006

        //▼==== #008
        public List<List<string>> ExportExcelConfigurationManager(ConfigurationReportParams Params)
        {
            try
            {
                return ConfigurationManagerProviders.Instance.ExportExcelConfigurationManager(Params);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ExportExcelConfigurationManager. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Failed_ExportExcelConfigurationManager");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #008

        //▼==== #007
        public void Lookup_Save(Lookup Obj, long StaffID, out string Result)
        {
            try
            {
                ConfigurationManagerProviders.Instance.Lookup_Save(Obj, StaffID, out Result);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Lookup_InsertUpdate. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CONFIG_MANAGE_Locations_InsertUpdate);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #007
    }
}