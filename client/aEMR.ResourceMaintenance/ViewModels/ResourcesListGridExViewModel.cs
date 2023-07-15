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
using aEMR.Common.Collections;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesListGridEx))]
    public class ResourcesListGridExViewModel : Conductor<object>, IResourcesListGridEx
        ,IHandle<ResourceEvent>
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
        
        public string stFilter = "";
        [ImportingConstructor]
        public ResourcesListGridExViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            _refResourceGroup=new ObservableCollection<ResourceGroup>();
            _refResourceType = new ObservableCollection<ResourceType>();
            _refSuplier = new ObservableCollection<Supplier>();
            _refResourceType = new ObservableCollection<ResourceType>();
            _AllResources = new PagedSortableCollectionView<Resources>();
            AllResources.OnRefresh += new EventHandler<RefreshEventArgs>(AllResources_OnRefresh);
            //GetAllResourceType();
            GetAllSupplier();
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
                GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
                                                    , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllResourceGroupType(ResourceCategoryEnum);
        }
        #region properties
        private void resetAll()
        {
            RscrName = "";
            RscrBrand = "";
            selectedResourceGroup = null;
            selectedResourceType = null;
            selectedSupplier = null;
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
                    refResourceType.Clear();
                }
                else
                {
                    //mName = txtName.Text.ToString();
                    GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
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
                    refResourceType.Clear();
                }
                else
                {
                    //mName = txtName.Text.ToString();
                    GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
                                            , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                }
            }
        }

        public void hpkNewresource_Click()
        {
            GlobalsNAV.ShowDialog<IResourcesNew>();
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

        private ObservableCollection<ResourceType> _refResourceType;
        public ObservableCollection<ResourceType> refResourceType
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
        private ResourceType _selectedResourceType;
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
                    GetAllResourceTypeByGroupID(selectedResourceGroup.RscrGroupID);
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
                        
                    }
                    else
                    {
                        mGroupID = selectedResourceGroup.RscrGroupID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
                                                , AllResources.PageSize, AllResources.PageIndex, OrderBy, CountTotal);
                    }

                }else
                {
                    mGroupID= 0;
                }
            }
        }
        public ResourceType selectedResourceType
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
                    && selectedResourceType.RscrTypeID > 0)
                {
                    AllResources.PageIndex = 0;
                    
                    if (!mAllFilter)
                    {
                        mChoice = 2;
                        mRscrID = selectedResourceType.RscrTypeID;
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
                        mTypeID = selectedResourceType.RscrTypeID;
                        GetAllResourceByAllFilterPage(mGroupID, mTypeID, mSupplierID, RscrName, RscrBrand
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
#endregion
#region method
        //load GetAllResourceGroup,GetAllResourceType
        public void GetAllResourceGroupType(long ResourceCategoryEnum)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceGroupType(ResourceCategoryEnum, Globals.DispatchCallback((asyncResult) =>
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
        public void GetAllResourceType()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceType(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceType = contract.EndGetAllResourceType(asyncResult);

                            if (ResourceType != null)
                            {
                                refResourceType.Clear();
                                foreach (ResourceType rt in ResourceType)
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
        public void GetAllResourceTypeByGroupID(long GroupID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceTypeByGID(GroupID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceType = contract.EndGetAllResourceTypeByGID(asyncResult);

                            if (ResourceType != null)
                            {
                                refResourceType.Clear();
                                foreach (ResourceType rt in ResourceType)
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

        
        public void GetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                , long ResourceCategoryEnum
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
                                                , ResourceCategoryEnum
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

        private void GetAllResourceByAllFilterPage(long mGroupID, long mTypeID, long mSupplierID, string mName, string mBrand
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

                    contract.BeginGetAllResourceByAllFilterPage(mGroupID, mTypeID,mSupplierID, mName, mBrand
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
#endregion

        public void Handle(ResourceEvent message)
        {
            if(message !=null)
            {
                
            }
        }
    }
}
