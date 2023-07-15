using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Threading;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IPropGrid)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class PropGridViewModel : Conductor<object>, IPropGrid, IHandle<DeptLocSelectedEvent>, IHandle<TranferEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PropGridViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            _allResourcePropLocations = new PagedSortableCollectionView<ResourcePropLocations>();
           //Globals.EventAggregator.Subscribe(this);
            allResourcePropLocations.OnRefresh += new EventHandler<RefreshEventArgs>(allResourcePropLocations_OnRefresh);
        }

        void allResourcePropLocations_OnRefresh(object sender, RefreshEventArgs e)
        {
            //TTM 17072018 do CurrentDeptLoc bi null nen new no de khoi null 
            //(nhung co van de la no lai load tat ca len grid thay vi khong load)
            //if (CurrentDeptLoc != null)
            //{
            CurrentDeptLoc = new RefDepartmentsTree();
                GetResourcePropLocationsPagingByDLID(CurrentDeptLoc.NodeID, allResourcePropLocations.PageSize
                        , allResourcePropLocations.PageIndex, "", true);
            //}
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            //_allResourcePropLocations =new ObservableCollection<ResourcePropLocations>();
        }
#region Property
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
                allResourcePropLocations.PageIndex = 0;
                GetResourcePropLocationsPagingByDLID(CurrentDeptLoc.NodeID, allResourcePropLocations.PageSize
                    , allResourcePropLocations.PageIndex, "", true);
            }
        }
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
                if (selectedResourcePropLocations != null)
                {
                    Globals.EventAggregator.Publish(new ResourcePropLocationsEvent() { selResourcePropLocations = selectedResourcePropLocations });
                }
            }
        }

        private PagedSortableCollectionView<ResourcePropLocations> _allResourcePropLocations;
        public PagedSortableCollectionView<ResourcePropLocations> allResourcePropLocations
        {
            get
            {
                return _allResourcePropLocations;
            }
            set
            {
                if (_allResourcePropLocations == value)
                    return;
                _allResourcePropLocations = value;
                NotifyOfPropertyChange(() => allResourcePropLocations);
            }
        }
#endregion 

        public void Handle(DeptLocSelectedEvent Obj)
        {
            if (Obj != null)
            {
                CurrentDeptLoc = (RefDepartmentsTree)Obj.curDeptLoc;
            }
        }
        public void Handle(TranferEvent Obj)
        {
            if (Obj != null)
            {
                allResourcePropLocations.PageIndex = 0;
                GetResourcePropLocationsPagingByDLID(CurrentDeptLoc.NodeID, allResourcePropLocations.PageSize
                    , allResourcePropLocations.PageIndex, "", true);
            }
        }    
        public void GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetResourcePropLocationsPagingByDLID(DeptLocationID, PageSize, PageIndex, OrderBy, CountTotal,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            allResourcePropLocations.Clear();
                            int total = 0;
                            var ResPro = contract.EndGetResourcePropLocationsPagingByDLID(out total,asyncResult);

                            if (ResPro != null)
                            {
                                foreach (var rt in ResPro)
                                {
                                    allResourcePropLocations.Add(rt);
                                }
                                allResourcePropLocations.TotalItemCount = total;
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
    }
}
