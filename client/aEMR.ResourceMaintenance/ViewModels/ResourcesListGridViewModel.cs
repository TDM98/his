using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using aEMR.ServiceClient;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using Castle.Windsor;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Linq;
using aEMR.Common.ExportExcel;
using System.Windows;
/*
* 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
*                      gọi về Service tốn thời gian.
* 20230424 #002 DatTB:
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi cách truyền biến một số function
* + Thêm function xuất excel thiết bị 
* + Chỉnh sửa bộ lọc tìm kiếm
* //Recommit
*/
namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesListGrid)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourcesListGridViewModel : Conductor<object>, IResourcesListGrid
        , IHandle<ResourceEvent>, IHandle<ResourceEditEvent>, IHandle<ResourceNewGroupEvent>
    {
        private int mChoice = 1;
        private long mRscrID = 0;
        private string OrderBy = "";
        private bool CountTotal = true;
        private int Total = 0;
        private bool mAllFilter = false;

        public long mGroupID = 0;
        public long mTypeID = 0;
        public long mSupplierID = 0;
        public long mDeptID = 0;

        public string stFilter = "";
        [ImportingConstructor]
        public ResourcesListGridViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            
            _refResourceGroup=new ObservableCollection<ResourceGroup>();

            //▼==== #002
            //_refResourceType = new ObservableCollection<ResourceType>();
            _refResourceType = new ObservableCollection<RefMedicalServiceType>();
            Coroutine.BeginExecute(LoadDepartments());
            //▲==== #002

            _refSuplier = new ObservableCollection<Supplier>();
            _AllResources = new PagedSortableCollectionView<Resources>();
            _refResourceUnit=new ObservableCollection<Lookup>();
            AllResources.OnRefresh += new EventHandler<RefreshEventArgs>(AllResources_OnRefresh);

            GetLookupRsrcUnit();
        }

        void AllResources_OnRefresh(object sender, RefreshEventArgs e)
        {
            //kiem tra cac ham o day
            if (!mAllFilter)
            {
                GetAllResourceByChoicePaging(mChoice
                                      , mRscrID
                                      , stFilter
                                        , ResourceCategoryEnum
                                      , AllResources.PageSize
                                       , AllResources.PageIndex
                                       , OrderBy
                                       , CountTotal);

            }
            else
            {
                GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                    , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Publish(new ResourceCategoryEnumEvent() { ResourceCategoryEnum = ResourceCategoryEnum });
            GetAllResourceGroupCategory(ResourceCategoryEnum);
        }
        #region properties
        private void resetAll()
        {
            RscrName = "";
            RscrBrand = "";
            selectedResourceGroup = null;
            selectedResourceType = null;
            selectedSupplier = null;
            selectedDepartment = null;
            refResourceType.Clear();
            AllResources.PageIndex = 0;
        }
    
        public void OnlyChoice()
        {
            mAllFilter = false;
            resetAll();
        }
        public void MultiChoice()
        {
            mAllFilter = true;
            resetAll();
        }

        private string _RscrName;
        private string _RscrBrand;
        
        public string RscrName
        {
            get
            {
                return _RscrName;
            }
            set
            {
                if (_RscrName == value)
                    return;
                _RscrName = value;
                NotifyOfPropertyChange(() => RscrName);
            }
        }
        public string RscrBrand
        {
            get
            {
                return _RscrBrand;
            }
            set
            {
                if (_RscrBrand == value)
                    return;
                _RscrBrand = value;
                NotifyOfPropertyChange(() => RscrBrand);
            }
        }
        private long _ResourceCategoryEnum;
        public long ResourceCategoryEnum
        {
            get
            {
                return _ResourceCategoryEnum;
            }
            set
            {
                if (_ResourceCategoryEnum == value)
                    return;
                _ResourceCategoryEnum = value;
                switch (_ResourceCategoryEnum)
                {
                    case (int)AllLookupValues.ResGroupCategory.THIET_BI_Y_TE:
                        _VSupplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
                        //selectedGroupName = "Nhóm Thiết Bị Y Tế"; 
                        break;
                    case (int)AllLookupValues.ResGroupCategory.THIET_BI_VAN_PHONG:
                        _VSupplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_VAN_PHONG;
                        //selectedGroupName = "Nhóm Thiết Bị Văn Phòng"; 
                        break;
                    case (int)AllLookupValues.ResGroupCategory.KHAC:
                        _VSupplierType = (int)AllLookupValues.SupplierType.KHAC;
                        //selectedGroupName = "Nhóm Khác"; 
                        break;
                }
                GetAllSupplierType(_VSupplierType);
                //GetAllSupplierType(_VSupplierType);
                //GetAllResourceGroupType(_ResourceCategoryEnum);
                //RaisePropertyChanged("ResourceCategoryEnum");
            }
        }
        private int _VSupplierType;
        public int VSupplierType
        {
            get
            {
                return _VSupplierType;
            }
            set
            {
                if (_VSupplierType == value)
                    return;
                _VSupplierType = value;
                NotifyOfPropertyChange(() => VSupplierType);
            }
        }
        //THIET_BI_Y_TE = 1,
        //THIET_BI_VAN_PHONG =2,
        //KHAC = 3
        public void RscrName_KeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AllResources.PageIndex = 0;
                //PagedResourcesVM.PageIndex = 0;
                if (!mAllFilter)
                {
                    mChoice= 4;
                    mRscrID= 0;
                    stFilter = RscrName;
                    GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);

                    RscrName= "";
                    RscrBrand = "";
                    selectedResourceGroup= null;
                    selectedResourceType = null;
                    selectedSupplier = null;
                    selectedDepartment = null;
                    refResourceType.Clear();
                }
                else
                {
                    //mName = txtName.Text.ToString();
                    GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);
                }
            }
        }

        public void RscrBrand_KeyUp(KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                AllResources.PageIndex = 0;
                if (!mAllFilter)
                {
                    mChoice= 5;
                    mRscrID= 0;
                    stFilter = RscrBrand;
                    GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);

                    RscrBrand = "";
                    RscrName = "";
                    selectedResourceGroup = null;
                    selectedResourceType = null;
                    selectedSupplier = null;
                    selectedDepartment = null;
                    refResourceType.Clear();
                }
                else
                {
                    //mName = txtName.Text.ToString();
                    GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                            , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                }
            }
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhpkNewresource = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                   , (int)eResources.mPtDashboardResource
                                                   , (int)oResourcesEx.mResourceList
                                                   , (int)ePermission.mAdd);
            bhpkEditresource = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources
                                                   , (int)eResources.mPtDashboardResource
                                                   , (int)oResourcesEx.mResourceList
                                                   , (int)ePermission.mEdit);
        }

#region checking account

        private bool _bhpkNewresource = true;
        private bool _bhpkEditresource = true;
        public bool bhpkNewresource
        {
            get
            {
                return _bhpkNewresource;
            }
            set
            {
                if (_bhpkNewresource == value)
                    return;
                _bhpkNewresource = value;
            }
        }
        public bool bhpkEditresource
        {
            get
            {
                return _bhpkEditresource;
            }
            set
            {
                if (_bhpkEditresource == value)
                    return;
                _bhpkEditresource = value;
            }
        }
#endregion
        public void hpkNewresource()
        {
            //▼==== 002
            Action<IResourcesEdit> onInitDlg = delegate (IResourcesEdit ResNewVM)
            {
                //gui kem gia tri ne
                ResNewVM.ResourceCategoryEnum = ResourceCategoryEnum;
                ResNewVM.refResourceUnit = refResourceUnit;
                ResNewVM.refResourceGroup = refResourceGroup;
                ResNewVM.refSuplier = refSuplier;
            };
            GlobalsNAV.ShowDialog<IResourcesEdit>(onInitDlg);
            //▲==== 002
        }


        private bool _MultyChoice;
        public bool MultyChoice
        {
            get
            {
                return _MultyChoice;
            }
            set
            {
                _MultyChoice = value;
                NotifyOfPropertyChange(() => _MultyChoice);
            }
        }

        private string _GroupCategoryTypeName;
        public string GroupCategoryTypeName
        {
            get
            {
                return _GroupCategoryTypeName;
            }
            set
            {
                if (_GroupCategoryTypeName == value)
                    return;
                _GroupCategoryTypeName = value;
            }
        }

        private Resources _selectedResources;
        public Resources selectedResources
        {
            get
            {
                return _selectedResources;
            }
            set
            {
                _selectedResources = value;
                NotifyOfPropertyChange(() => selectedResources);
                Globals.EventAggregator.Publish(new ResourceSelectedEvent() { curResource = selectedResources });
            }
        }

        private ObservableCollection<ResourceGroup> _refResourceGroup;
        public ObservableCollection<ResourceGroup> refResourceGroup
        {
            get
            {
                return _refResourceGroup;
            }
            set
            {
                _refResourceGroup = value;
                NotifyOfPropertyChange(() => refResourceGroup);
            }
        }

        //▼==== #002
        //private ObservableCollection<ResourceType> _refResourceType;
        //public ObservableCollection<ResourceType> refResourceType
        //{
        //    get
        //    {
        //        return _refResourceType;
        //    }
        //    set
        //    {
        //        _refResourceType = value;
        //        NotifyOfPropertyChange(() => refResourceType);
        //    }
        //}
        //▲==== #002

        private ObservableCollection<Supplier> _refSuplier;
        public ObservableCollection<Supplier> refSuplier
        {
            get
            {
                return _refSuplier;
            }
            set
            {
                _refSuplier = value;
                NotifyOfPropertyChange(() => refSuplier);
            }
        }

        private PagedSortableCollectionView<Resources> _AllResources;
        public PagedSortableCollectionView<Resources> AllResources
        {
            get
            {
                return _AllResources;
            }
            set
            {
                if (_AllResources == value)
                    return;
                _AllResources = value;
                NotifyOfPropertyChange(() => AllResources);
            }
        }

        private ResourceGroup _selectedResourceGroup;
        private RefMedicalServiceType _selectedResourceType;
        private Supplier _selectedSupplier;

        public ResourceGroup selectedResourceGroup
        {
            get
            {
                return _selectedResourceGroup;
            }
            set
            {
                if (_selectedResourceGroup == value)
                    return;
                _selectedResourceGroup = value;
                NotifyOfPropertyChange(() => selectedResourceGroup);

                if (selectedResourceGroup!=null
                    && selectedResourceGroup.RscrGroupID >0)
                {
                    AllResources.PageIndex = 0;
                    //▼==== #002
                    //GetAllResourceTypeByGroupID(selectedResourceGroup.RscrGroupID);

                    if (selectedResourceGroup != null && selectedResourceGroup.GroupName != null)
                    {
                        if (selectedResourceGroup.GroupName.Contains("Máy Thủ Thuật"))
                        {
                            GetMedicalServiceTypes_ByResourceGroup(1);
                        }
                        else if (selectedResourceGroup.GroupName.Contains("Máy Phẫu Thuật"))
                        {
                            GetMedicalServiceTypes_ByResourceGroup(2);
                        }
                    }                 
                    //▲==== #002

                    mRscrID = selectedResourceGroup.RscrGroupID;
                    if (!mAllFilter)
                    {
                        mChoice= 1;
                        stFilter = "";
                        GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);
                        selectedSupplier= null;
                        selectedDepartment = null;

                    }
                    else
                    {
                        mGroupID = selectedResourceGroup.RscrGroupID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                    }

                }else
                {
                    mGroupID= 0;
                }
            }
        }
        public RefMedicalServiceType selectedResourceType
        {
            get
            {
                return _selectedResourceType;
            }
            set
            {
                if (_selectedResourceType == value)
                    return;
                _selectedResourceType = value;
                NotifyOfPropertyChange(() => selectedResourceType);
                if (selectedResourceType!=null
                    && selectedResourceType.MedicalServiceTypeID > 0)
                {
                    AllResources.PageIndex = 0;
                    if (!mAllFilter)
                    {
                        mChoice = 2;
                        mRscrID = selectedResourceType.MedicalServiceTypeID;
                        stFilter = "";
                        GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);
                        
                    }
                    else
                    {
                        mTypeID = selectedResourceType.MedicalServiceTypeID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                            , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                    }

                }
                else
                {
                    mTypeID = 0;
                }
            }
        }
        public Supplier selectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
                if (_selectedSupplier == value)
                    return;
                _selectedSupplier = value;
                NotifyOfPropertyChange(() => selectedSupplier);
                if (selectedSupplier!=null
                &&  selectedSupplier.SupplierID>0)
                {
                    AllResources.PageIndex = 0;
                    if (!mAllFilter)
                    {
                        mChoice = 3;
                        mRscrID= selectedSupplier.SupplierID;
                        stFilter = "";
                        GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);
                        refResourceType.Clear();
                        selectedResourceGroup = null;
                    }
                    else
                    {
                        mSupplierID = selectedSupplier.SupplierID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
                                                            , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                    }
                }
                else
                {
                    mSupplierID = 0;
                }
            }
        }

        private ObservableCollection<Lookup> _refResourceUnit;
        public ObservableCollection<Lookup> refResourceUnit
        {
            get
            {
                return _refResourceUnit;
            }
            set
            {
                if (_refResourceUnit == value)
                    return;
                _refResourceUnit = value;
            }
        }
#endregion
#region method
        //load GetAllResourceGroup,GetAllResourceType
        public void GetAllResourceGroupCategory(long V_ResGroupCategory)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceGroupType(V_ResGroupCategory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceGroup = contract.EndGetAllResourceGroupType(asyncResult);

                            if (ResourceGroup != null)
                            {
                                refResourceGroup.Clear();
                                foreach (ResourceGroup rg in ResourceGroup )
                                {
                                    refResourceGroup.Add(rg);
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

        //▼==== #002
        //public void GetAllResourceType()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ResourcesManagementServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetAllResourceType(Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var ResourceType = contract.EndGetAllResourceType(asyncResult);

        //                    if (ResourceType != null)
        //                    {
        //                        refResourceType.Clear();
        //                        foreach (ResourceType rt in ResourceType)
        //                        {
        //                            refResourceType.Add(rt);
        //                        }
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        //public void GetAllResourceTypeByGroupID(long GroupID)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ResourcesManagementServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetAllResourceTypeByGID(GroupID,Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var ResourceType = contract.EndGetAllResourceTypeByGID(asyncResult);

        //                    if (ResourceType != null)
        //                    {
        //                        refResourceType.Clear();
        //                        foreach (ResourceType rt in ResourceType)
        //                        {
        //                            refResourceType.Add(rt);
        //                        }
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}
        //▲==== #002

        public void GetAllSupplier()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllSupplier(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Supplier = contract.EndGetAllSupplier(asyncResult);

                            if (Supplier != null)
                            {
                                refSuplier.Clear();
                                foreach (Supplier sp in Supplier)
                                {
                                    refSuplier.Add(sp);
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

        public void GetAllSupplierType(long V_SupplierType)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllSupplierType(V_SupplierType,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Supplier = contract.EndGetAllSupplierType(asyncResult);

                            if (Supplier != null)
                            {
                                refSuplier.Clear();
                                foreach (Supplier sp in Supplier)
                                {
                                    refSuplier.Add(sp);
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

        public void GetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                ,long V_ResGroupCategory
                                              , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceByChoicePaging(mChoice
                                              , RscrID
                                              , Text
                                                ,V_ResGroupCategory
                                              , PageSize
                                               , PageIndex
                                               , OrderBy
                                               , CountTotal
                                               ,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var refAllResources = contract.EndGetAllResourceByChoicePaging(out Total, asyncResult);

                            if (refAllResources != null)
                            {
                                AllResources.Clear();
                                foreach (Resources sp in refAllResources)
                                {
                                    AllResources.Add(sp);
                                }
                                AllResources.TotalItemCount = Total;
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

        private void GetAllResourceByAllFilterPage(long mGroupID, long mTypeID, long mDeptID
                                                , string mName, string mBrand
                                                , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceByAllFilterPage(mGroupID, mTypeID,mDeptID, mName, mBrand
                                            , PageSize, PageIndex, OrderBy, CountTotal
                                               , Globals.DispatchCallback((asyncResult) =>
                                               {
                                                   try
                                                   {
                                                       var refAllResources = contract.EndGetAllResourceByAllFilterPage(out Total, asyncResult);

                                                       if (refAllResources != null)
                                                       {
                                                           AllResources.Clear();
                                                           foreach (Resources sp in refAllResources)
                                                           {
                                                               AllResources.Add(sp);
                                                           }
                                                           AllResources.TotalItemCount = Total;
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
        private void GetLookupRsrcUnit()
        {
            //▼====== #001
            //refResourceUnit.Clear();
            refResourceUnit = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.RESOURCE_UNIT))
                {
                    refResourceUnit.Add(tmpLookup);
                }
            }
            //▲====== #001

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ResourcesManagementServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        contract.BeginGetLookupRsrcUnit( Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    var ResourceUnit = contract.EndGetLookupRsrcUnit(asyncResult);

            //                    if (ResourceUnit != null)
            //                    {
            //                        refResourceUnit.Clear();
            //                        foreach (var sp in ResourceUnit)
            //                        {
            //                            refResourceUnit.Add(sp);
            //                        }
            //                    }

            //                }
            //                catch (Exception ex)
            //                {
            //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //                    Globals.IsBusy = false;
            //                }
            //                finally
            //                {
            //                    Globals.IsBusy = false;
            //                }
            //            }), null);
            //    }


            //});
            //t.Start();
        }
#endregion

        public void Handle(ResourceEvent message)
        {
            if(message !=null)
            {
                
            }
        }

        public void Handle(ResourceEditEvent e)
        {
            AllResources.PageIndex = 0;
            if (!mAllFilter)
            {
                GetAllResourceByChoicePaging(mChoice
                                      , mRscrID
                                      , stFilter
                                        , ResourceCategoryEnum
                                      , AllResources.PageSize
                                       , AllResources.PageIndex
                                       , OrderBy
                                       , CountTotal);

            }
            else
            {
                GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                    , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
            }
        }
        
        public void DoubleClick(object e)
        {
            if(!bhpkEditresource)
            {
                Globals.ShowMessage(eHCMSResources.Z1748_G1_KgDcPhanQuyenChSuaVTu, "");
                return;
            }
            Action<IResourcesEdit> onInitDlg = delegate (IResourcesEdit resourceEdit)
            {
                //▼==== 002
                resourceEdit.isEdit = true;
                resourceEdit.ResourceCategoryEnum = ResourceCategoryEnum;
                resourceEdit.refResourceType = refResourceType;
                //▲==== 002
                resourceEdit.curResource = ObjectCopier.DeepCopy((Resources)((EventArgs<object>)(e)).Value);
                resourceEdit.refResourceGroup = refResourceGroup;
                resourceEdit.refSuplier = refSuplier;
                resourceEdit.refResourceUnit = refResourceUnit;
            };
            GlobalsNAV.ShowDialog<IResourcesEdit>(onInitDlg);

        }
        public void Handle(ResourceNewGroupEvent obj)
        {
            if (obj!=null)
            {
                GetAllResourceGroupCategory(ResourceCategoryEnum);
            }
        }

        //▼==== #002
        private ObservableCollection<RefMedicalServiceType> _refResourceType;
        public ObservableCollection<RefMedicalServiceType> refResourceType
        {
            get
            {
                return _refResourceType;
            }
            set
            {
                _refResourceType = value;
                NotifyOfPropertyChange(() => refResourceType);
            }
        }

        public void GetMedicalServiceTypes_ByResourceGroup(long GroupID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetMedicalServiceTypes_ByResourceGroup(GroupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceType = contract.EndGetMedicalServiceTypes_ByResourceGroup(asyncResult);

                            if (ResourceType != null)
                            {
                                refResourceType.Clear();
                                foreach (RefMedicalServiceType rt in ResourceType)
                                {
                                    refResourceType.Add(rt);
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

        private ObservableCollection<RefDepartment> _RefDepartmentCollection;
        public ObservableCollection<RefDepartment> RefDepartmentCollection
        {
            get => _RefDepartmentCollection; set
            {
                _RefDepartmentCollection = value;
                NotifyOfPropertyChange(() => RefDepartmentCollection);
            }
        }

        private IEnumerator<IResult> LoadDepartments()
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(new List<long> { (long)V_DeptTypeOperation.KhoaNgoaiTru, (long)V_DeptTypeOperation.KhoaNoi });
            yield return departmentTask;
            RefDepartmentCollection = departmentTask.Departments.Where(x => x.V_DeptTypeOperation != (long)V_DeptTypeOperation.KhoaNgoaiTru || x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();
            if (RefDepartmentCollection == null) RefDepartmentCollection = new ObservableCollection<RefDepartment>();
            RefDepartmentCollection.Insert(0, new RefDepartment { DeptID = 0, DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa) });
            yield break;
        }

        private RefDepartment _selectedDepartment;

        public RefDepartment selectedDepartment
        {
            get
            {
                return _selectedDepartment;
            }
            set
            {
                if (_selectedDepartment == value)
                    return;
                _selectedDepartment = value;
                NotifyOfPropertyChange(() => selectedDepartment);
                if (selectedDepartment != null)
                {
                    AllResources.PageIndex = 0;
                    if (!mAllFilter)
                    {
                        mChoice = 3;
                        mRscrID = selectedDepartment.DeptID;
                        stFilter = "";
                        GetAllResourceByChoicePaging(mChoice
                                              , mRscrID
                                              , stFilter
                                                , ResourceCategoryEnum
                                              , AllResources.PageSize
                                               , AllResources.PageIndex
                                               , OrderBy
                                               , CountTotal);
                        refResourceType.Clear();
                        selectedResourceGroup = null;
                    }
                    else
                    {
                        mDeptID = selectedDepartment.DeptID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mDeptID, RscrName, RscrBrand
                                                            , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                    }
                }
                else
                {
                    mDeptID = 0;
                }
            }
        }

        public void BtnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelAllResources(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                //strNameExcel = string.Format("{0} ", eHCMSResources.Z2374_G1_ChoXacNhan);
                                var results = contract.EndExportExcelAllResources(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲==== #002

    }
}
