using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
 *                      gọi về Service tốn thời gian.
 */
namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropTranfer)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropTranferViewModel : Conductor<object>, IPropTranfer, IHandle<DeptLocSelectedTranferEvent>, IHandle<lstResourcePropLocationsGridToFormEvent>
    {
        [ImportingConstructor]
        public PropTranferViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            GetAllStaffByStaffCategory(6102);
            GetLookupAllocStatus();
            GetLookupStorageStatus();
            _refLookupAllocStatus=new ObservableCollection<Lookup>();
            _refLookupStorageReason = new ObservableCollection<Lookup>();
            _refStaffList=new ObservableCollection<Staff>();
            _selectedResourcePropLocations = new ResourcePropLocations();
            _curResourcePropLocations = new ResourcePropLocations();
            _lstNewResourcePropLocations = new ObservableCollection<ResourcePropLocations>();

            var gridPr = Globals.GetViewModel<IPropGridEx>();
            gridPr.lstNewResourcePropLocations = lstNewResourcePropLocations;
            var deptTree = Globals.GetViewModel<IDepartmentTreeEx>();

            gridProp = gridPr;
            treeDept = deptTree;
            this.ActivateItem(gridPr);
            this.ActivateItem(deptTree);
            Globals.EventAggregator.Subscribe(this);
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            
            curResourcePropLocations = new ResourcePropLocations();
            //_selectedResourcePropLocations = new ResourcePropLocations();
            lstNewResourcePropLocations = new ObservableCollection<ResourcePropLocations>();
            if (curResourcePropLocations.VRscrProperty == null)
            {
                curResourcePropLocations.VRscrProperty = new ResourceProperty();
            }
            curResourcePropLocations.VRscrProperty = ObjectCopier.DeepCopy(selectedResourcePropLocations.VRscrProperty);

            var gridPr = Globals.GetViewModel<IPropGridEx>();
            gridPr.lstNewResourcePropLocations = lstNewResourcePropLocations;
            var deptTree = Globals.GetViewModel<IDepartmentTreeEx>();

            gridProp = gridPr;
            treeDept = deptTree;
            this.ActivateItem(gridPr);
            this.ActivateItem(deptTree);
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            
        }
        private object _ActiveContent;
        public object ActiveContent
        {
            get
            {
                return _ActiveContent;
            }
            set
            {
                _ActiveContent = value;
                NotifyOfPropertyChange(()=>ActiveContent);
            }
        }

        private object _gridProp;
        private object _treeDept;

        public object gridProp
        {
            get
            {
                return _gridProp;
            }
            set
            {
                if (_gridProp == value)
                    return;
                _gridProp = value;
                NotifyOfPropertyChange(() => gridProp);
            }
        }
        public object treeDept
        {
            get
            {
                return _treeDept;
            }
            set
            {
                if (_treeDept == value)
                    return;
                _treeDept = value;
                NotifyOfPropertyChange(() => treeDept);
            }
        }

#region property
        private ResourcePropLocations _selectedResourcePropLocations;
        public ResourcePropLocations selectedResourcePropLocations
        {
            get
            {
                return _selectedResourcePropLocations;
            }
            set
            {
                if (_selectedResourcePropLocations == value)
                    return;
                _selectedResourcePropLocations = value;
                NotifyOfPropertyChange(() => selectedResourcePropLocations);
            }
        }

        private int _curQty;
        public int curQty
        {
            get
            {
                return _curQty;
            }
            set
            {
                if (_curQty == value)
                    return;
                _curQty = value;
                if (curResourcePropLocations.VRscrProperty == null)
                {
                    curResourcePropLocations.VRscrProperty = new ResourceProperty();
                }
                curResourcePropLocations.VRscrProperty.QtyAlloc = curQty;
                NotifyOfPropertyChange(() => curQty);
                NotifyOfPropertyChange(() => CanTempAdd); 
            }
        }

        private ResourcePropLocations _curResourcePropLocations;
        public ResourcePropLocations curResourcePropLocations
        {
            get
            {
                return _curResourcePropLocations;
            }
            set
            {
                if (_curResourcePropLocations == value)
                    return;
                _curResourcePropLocations = value;
                NotifyOfPropertyChange(() => curResourcePropLocations);
            }
        }

        private ObservableCollection<ResourcePropLocations> _lstNewResourcePropLocations;
        public ObservableCollection<ResourcePropLocations> lstNewResourcePropLocations
        {
            get
            {
                return _lstNewResourcePropLocations;
            }
            set
            {
                if (_lstNewResourcePropLocations == value)
                    return;
                _lstNewResourcePropLocations = value;
                NotifyOfPropertyChange(() => lstNewResourcePropLocations);
            }
        }
        public  void CountQty()
        {
            int curQt = 0;
            foreach (var view in _lstNewResourcePropLocations)
            {
                curQt += view.VRscrProperty.QtyAlloc;
            }
            if (selectedResourcePropLocations.VRscrProperty==null)
            {
                selectedResourcePropLocations.VRscrProperty=new ResourceProperty();
            }
            curQty = selectedResourcePropLocations.VRscrProperty.QtyAlloc - curQt;
            
        }

        private ObservableCollection<Staff> _refStaffList;
        public ObservableCollection<Staff> refStaffList
        {
            get
            {
                return _refStaffList;
            }
            set
            {
                if (_refStaffList == value)
                    return;
                _refStaffList = value;
                NotifyOfPropertyChange(() => refStaffList);
            }
        }

        private IList<Lookup> _refLookupAllocStatus;
        public IList<Lookup> refLookupAllocStatus
        {
            get
            {
                return _refLookupAllocStatus;
            }
            set
            {
                if (_refLookupAllocStatus == value)
                    return;
                _refLookupAllocStatus = value;
            }
        }
        private IList<Lookup> _refLookupStorageReason;
        public IList<Lookup> refLookupStorageReason
        {
            get
            {
                return _refLookupStorageReason;
            }
            set
            {
                if (_refLookupStorageReason == value)
                    return;
                _refLookupStorageReason = value;
            }
        }
#endregion
        public  void Exit()
        {
            TryClose();
            //base.OnDeactivate(true);
        }
        public void Tranfer()
        {
            //dieu chuyen o day
            //validate
            if (lstNewResourcePropLocations.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0424_G1_Msg_InfoChuaThemMotDCVTMoi);
                return;
            }

            //----Kiem tra break co du hay ko
            if (curQty> 0)
            {
                //tao mot new resource property location moi de add vao list
                var selectedNewResourcePropLocations = new ResourcePropLocations();
                
                selectedNewResourcePropLocations = ObjectCopier.DeepCopy(curResourcePropLocations);
                selectedNewResourcePropLocations.VRscrProperty.QtyAlloc = curQty;
                selectedNewResourcePropLocations.VDeptLocation = selectedResourcePropLocations.VDeptLocation;
                
                lstNewResourcePropLocations.Add(selectedNewResourcePropLocations);
            }

            //------Insert vao co so du lieu
            if (lstNewResourcePropLocations.Count > 1)
            {
                BreakResourceProperty(lstNewResourcePropLocations
                                    , selectedResourcePropLocations.VRscrProperty.RscrPropertyID
                                    , selectedResourcePropLocations.RscrPropLocID);
            }
            else
            {
                if (lstNewResourcePropLocations.Count > 0)
                {
                    if(lstNewResourcePropLocations[0].VAllocStatus==null)
                        lstNewResourcePropLocations[0].VAllocStatus=new Lookup();
                    if(lstNewResourcePropLocations[0].VStorageReason==null)
                        lstNewResourcePropLocations[0].VStorageReason=new Lookup();
                    if(lstNewResourcePropLocations[0].VResponsibleStaff==null)
                        lstNewResourcePropLocations[0].VResponsibleStaff=new Staff();

                    MoveResourcePropertyLocation(
                        selectedResourcePropLocations.RscrPropLocID
                    , selectedResourcePropLocations.VRscrProperty.RscrPropertyID

                    , lstNewResourcePropLocations[0].VDeptLocation.DeptLocationID
                    , lstNewResourcePropLocations[0].RscrMoveRequestID
                    , 2//lstNewResourcePropLocations[0].VAllocStaff.StaffID  chon gia tri tam thoi
                    , lstNewResourcePropLocations[0].AllocDate
                    , lstNewResourcePropLocations[0].AllocDate
                    , lstNewResourcePropLocations[0].VAllocStatus.LookupID
                    , lstNewResourcePropLocations[0].VStorageReason.LookupID
                    , lstNewResourcePropLocations[0].VResponsibleStaff.StaffID);
                }
            }
            TryClose();
            Globals.EventAggregator.Publish(new TranferEvent{});
        }
    
        public bool CanTempAdd
        {
            get { return curQty > 0; }
        }

        public  void TempAdd()
        {
            if (curResourcePropLocations.VRscrProperty.QtyAlloc <1)
            {
                Globals.ShowMessage(eHCMSResources.Z1728_G1_SLgDieuChuyen, "");
            }else
                if (curResourcePropLocations.VRscrProperty.QtyAlloc > curQty)
            {
                Globals.ShowMessage(eHCMSResources.Z1729_G1_SLGDieuChuyen2, "");
            }else
                {
                    if (curResourcePropLocations.VRscrMoveRequest == null)
                        curResourcePropLocations.VRscrMoveRequest = new ResourceMoveRequests();
                    if (curResourcePropLocations.VAllocStaff == null)
                        curResourcePropLocations.VAllocStaff = new Staff();
                    if (curResourcePropLocations.VAllocStatus == null)
                        curResourcePropLocations.VAllocStatus = new Lookup();
                    if (curResourcePropLocations.VStorageReason == null)
                        curResourcePropLocations.VStorageReason = new Lookup();
                    if (curResourcePropLocations.VResponsibleStaff == null)
                        curResourcePropLocations.VResponsibleStaff = new Staff();
                    ResourcePropLocations temp = ObjectCopier.DeepCopy(curResourcePropLocations);
                    lstNewResourcePropLocations.Add(temp);
                    CountQty();        
                }
            
            
            //Globals.EventAggregator.Publish(new lstResourcePropLocationsEvent() { lstResPropLocations = lstNewResourcePropLocations });
        }
    
        public void Handle(DeptLocSelectedTranferEvent obj)
        {
            if(obj!=null)
            {
                CountQty();
                DeptLocation dL=new DeptLocation();
                dL.DeptLocationID = ((RefDepartmentsTree) obj.curDeptLoc).NodeID;
                dL.RefDepartment=new RefDepartment();
                dL.RefDepartment.DeptID=((RefDepartmentsTree)obj.curDeptLoc).Parent.NodeID;
                dL.RefDepartment.DeptName = ((RefDepartmentsTree)obj.curDeptLoc).Parent.NodeText;
                
                dL.Location=new Location();
                dL.Location.LID = ((RefDepartmentsTree)obj.curDeptLoc).NodeID;
                dL.Location.LocationName = ((RefDepartmentsTree)obj.curDeptLoc).NodeText;
                if (curResourcePropLocations.VDeptLocation!=null)
                {
                    curResourcePropLocations.VDeptLocation = new DeptLocation();
                }
                curResourcePropLocations.VDeptLocation = dL;
                //curResourcePropLocations.VRscrProperty=new ResourceProperty();
                //curResourcePropLocations.VRscrProperty.QtyAlloc = 1;
            }
        }
        public void Handle(lstResourcePropLocationsGridToFormEvent obj)
        {
            if (obj!=null)
            {
                lstNewResourcePropLocations = (ObservableCollection<ResourcePropLocations>)obj.lstResPropLocations;
                CountQty();
            }
        }
#region method
        
        private void BreakResourceProperty(ObservableCollection<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID)
        {
            // su dung ham nay ne RefDepartments_Tree(strV_DeptType, ShowDeptLocation);
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    

                    contract.BeginBreakResourceProperty(lstResourcePropLocations, RscrPropertyID, RscrPropLocID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndBreakResourceProperty(asyncResult);
                            if (ResVal == true)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1730_G1_PhanChiaVTuThCong, "");
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z1731_G1_PhanChiaLoaiVTu, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        private void MoveResourcePropertyLocation(long RscrPropLocID, long RscrPropertyID, 
            long DeptLocationID, long RscrMoveRequestID, 
            long AllocStaffID, DateTime? AllocDate,
            DateTime? StartUseDate, long V_AllocStatus, long V_StorageReason, long ResponsibleStaffID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z1026_G1_DSRoomType });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginMoveResourcePropertyLocation(RscrPropLocID, RscrPropertyID
                        , DeptLocationID, RscrMoveRequestID,AllocStaffID, AllocDate
                        , StartUseDate, V_AllocStatus, V_StorageReason, ResponsibleStaffID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndMoveResourcePropertyLocation(asyncResult);
                            if (ResVal == true)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1732_G1_DieuChuyenVTuThCong, "");
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z1733_G1_DieuChuyenLoaiVTu, "");
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        
        private void GetAllStaffByStaffCategory(long StaffCategory)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    
                    contract.BeginGetAllStaffByStaffCategory(StaffCategory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllStaffByStaffCategory(asyncResult);
                            if(items!=null)
                            {
                                foreach (var staff in items)
                                {
                                    refStaffList.Add(staff);    
                                }
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        
        private void GetLookupAllocStatus()
        {
            //▼====== #001
            refLookupAllocStatus.Clear();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.RESOURCE_ALLOC_STATUS))
                {
                    refLookupAllocStatus.Add(tmpLookup);
                }
            }
            //▲====== #001
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ResourcesManagementServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;


            //        contract.BeginGetLookupAllocStatus(Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var items = contract.EndGetLookupAllocStatus(asyncResult);

            //                if (items != null)
            //                {
            //                    refLookupAllocStatus.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refLookupAllocStatus.Add(tp);
            //                    }

            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                Globals.IsBusy = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }

        private void GetLookupStorageStatus()
        {

            refLookupStorageReason.Clear();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.RESOURCE_STORE_REASON))
                {
                    refLookupStorageReason.Add(tmpLookup);
                }
            }

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ResourcesManagementServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

                    
            //        contract.BeginGetLookupStorageStatus(Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var items = contract.EndGetLookupStorageStatus(asyncResult);
            //                if (items != null)
            //                {
            //                    refLookupStorageReason.Clear();
            //                    foreach (var tp in items)
            //                    {
            //                        refLookupStorageReason.Add(tp);
            //                    }

            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                Globals.IsBusy = false;
            //            }
            //        }), null);
            //    }


            //});
            //t.Start();
        }
#endregion
    }
}
