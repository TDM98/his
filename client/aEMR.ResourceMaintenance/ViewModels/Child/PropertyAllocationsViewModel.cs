using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using eHCMSLanguage;
using Castle.Windsor;
/*
 * 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
 *                      gọi về Service tốn thời gian.
 */
namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropertyAllocations)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropertyAllocationsViewModel : Conductor<object>, IPropertyAllocations, IHandle<PropDeptEvent>
    {
        public int userID = 2;
        [ImportingConstructor]
        public PropertyAllocationsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            GetAllStaffByStaffCategory(6102);
            GetLookupAllocStatus();
            GetLookupStorageStatus();
            _refLookupAllocStatus = new ObservableCollection<Lookup>();
            _refLookupStorageReason = new ObservableCollection<Lookup>();
            _refStaffList = new ObservableCollection<Staff>();
        }

        public void Exit()
        {
            TryClose();
        }
        //public void SaveProAlloc()
        //{
        //    TryClose();
        //}
#region property
        private Resources _CurrentResource;
        public Resources CurrentResource
        {
            get
            {
                return _CurrentResource;
            }
            set
            {
                if (_CurrentResource == value)
                    return;
                _CurrentResource = value;
                NotifyOfPropertyChange(() => CurrentResource);
            }
        }

        private RefDepartmentsTree _CurrentDeptLoc;
        public RefDepartmentsTree CurrentDeptLoc
        {
            get
            {
                return _CurrentDeptLoc;
            }
            set
            {
                if (_CurrentDeptLoc == value)
                    return;
                _CurrentDeptLoc = value;
                NotifyOfPropertyChange(() => CurrentDeptLoc);
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

        public void Handle(PropDeptEvent pd)
        {
            CurrentResource = (Resources)pd.curResource;
            CurrentDeptLoc = (RefDepartmentsTree)pd.curDeptLoc;
            _curResourcePropLocations=new ResourcePropLocations();
            curResourcePropLocations.VDeptLocation=new DeptLocation();
            curResourcePropLocations.VDeptLocation.DeptLocationID = ((RefDepartmentsTree) pd.curDeptLoc).NodeID;
            curResourcePropLocations.VDeptLocation.RefDepartment=new RefDepartment();
            curResourcePropLocations.VDeptLocation.Location=new Location();
            curResourcePropLocations.VDeptLocation.RefDepartment.DeptID=((RefDepartmentsTree)pd.curDeptLoc).Parent.NodeID;
            curResourcePropLocations.VDeptLocation.RefDepartment.DeptName =((RefDepartmentsTree) pd.curDeptLoc).Parent.NodeText;
            curResourcePropLocations.VDeptLocation.Location.LocationName=((RefDepartmentsTree)pd.curDeptLoc).NodeText;
            curResourcePropLocations.VRscrProperty=new ResourceProperty();
            curResourcePropLocations.VRscrProperty.VResources=new Resources();

            curResourcePropLocations.VRscrProperty.VResources= (Resources) pd.curResource;
            //curResourcePropLocations.VRscrProperty.VResources.ItemName = ((Resources) pd.curResource).ItemName;
            //curResourcePropLocations.VRscrProperty.VResources.ItemBrand = ((Resources)pd.curResource).ItemBrand;
            //curResourcePropLocations.VRscrProperty.VResources.VResourceGroup=new ResourceGroup();
            //curResourcePropLocations.VRscrProperty.VResources.VResourceGroup.GroupName = ((Resources)pd.curResource).VResourceGroup.GroupName;
        }
        public void SavePro()
        {
            //check
            //return;
            if (curResourcePropLocations != null)
            {
                //if (curResourcePropLocations.VRscrProperty.QtyAlloc < 1) 
                //{
                //    MessageBox.Show("Quantity of property is not less than 1!");
                //    return;
                //}

                //if (txtQtyAlloc.Text!="")
                //{
                //    if (Convert.ToInt32(txtQtyInUse.Text) > Convert.ToInt32(txtQtyAlloc.Text))
                //    {
                //        MessageBox.Show("Số lượng sử dụng lớn hơn số lượng phân bố!");
                //        return;
                //    }
                //}

                if (curResourcePropLocations.VResponsibleStaff == null)
                {
                    curResourcePropLocations.VResponsibleStaff = new Staff();
                }
                if (curResourcePropLocations.VAllocStatus == null)
                {
                    curResourcePropLocations.VAllocStatus = new Lookup();
                    curResourcePropLocations.VAllocStatus.LookupID = 5800;
                }
                if (curResourcePropLocations.VStorageReason == null)
                {
                    curResourcePropLocations.VStorageReason = new Lookup();
                }
                AddNewResourceProperty(
                                    curResourcePropLocations.VRscrProperty.VResources.RscrID
                                    , Guid.NewGuid().ToString()
                                    , curResourcePropLocations.VRscrProperty.HasIdentity
                                    , curResourcePropLocations.VRscrProperty.RscrCode
                                    , curResourcePropLocations.VRscrProperty.RscrBarcode
                                    , curResourcePropLocations.VRscrProperty.SerialNumber
                                    , curResourcePropLocations.VRscrProperty.QtyAlloc
                                    , true
                                    , false
                                    , curResourcePropLocations.VDeptLocation.DeptLocationID
                                    , 0
                                    , userID//GetLoggedUserIDSession()
                                    , curResourcePropLocations.AllocDate
                                    , curResourcePropLocations.StartUseDate
                                    , curResourcePropLocations.VAllocStatus.LookupID
                                    , curResourcePropLocations.VStorageReason.LookupID
                                    , curResourcePropLocations.VResponsibleStaff.StaffID
                                            );
            }
            TryClose();
        }
        public void AddNewResourceProperty(long RscrID
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResourceProperty(RscrID
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
                                                        ,V_StorageReason
                                                       , ResponsibleStaffID
                                               , Globals.DispatchCallback((asyncResult) =>
                                               {
                                                   try
                                                   {
                                                       var ResVal = contract.EndAddNewResourceProperty(asyncResult);
                                                       if (ResVal == true)
                                                       {
                                                           Globals.ShowMessage(eHCMSResources.Z1726_G1_PBoVTuThCong, "");
                                                       }
                                                       else
                                                       {
                                                           Globals.ShowMessage(eHCMSResources.Z1727_G1_PBoVTuBiLoi, "");
                                                       }
                                                   }
                                                   catch (Exception ex)
                                                   {
                                                       Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                                       Globals.IsBusy = false;
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
                            if (items != null)
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
            //refLookupAllocStatus.Clear();
            refLookupAllocStatus = new ObservableCollection<Lookup>();
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
            //▼====== #001
            if (refLookupStorageReason == null)
            {
                refLookupStorageReason = new ObservableCollection<Lookup>();
            }
            else
            {
                refLookupStorageReason.Clear();
            }
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.RESOURCE_STORE_REASON))
                {
                    refLookupStorageReason.Add(tmpLookup);
                }
            }
            //▲====== #001
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

    }
}
