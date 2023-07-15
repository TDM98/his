using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IStoragesHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoragesHistoryViewModel : Conductor<object>, IStoragesHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StoragesHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //var gridHisTran = Globals.GetViewModel<IPropGridHistory>();
            //GridPropHis = gridHisTran;
            //this.ActivateItem(gridHisTran);
            _selectedResourcePropLocations =new ResourcePropLocations();
            _allRsrcPropLocBreak = new PagedSortableCollectionView<ResourcePropLocations>();
            allRsrcPropLocBreak.OnRefresh += new EventHandler<RefreshEventArgs>(allRsrcPropLocBreak_OnRefresh);
            
        }

        void allRsrcPropLocBreak_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetResourcePropLocationsPagingByFilter(selectedResourcePropLocations.VRscrProperty.RscrGUID
                , allRsrcPropLocBreak.PageSize, allRsrcPropLocBreak.PageIndex, "", true);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            GetResourcePropLocationsPagingByFilter(selectedResourcePropLocations.VRscrProperty.RscrGUID
                , allRsrcPropLocBreak.PageSize, allRsrcPropLocBreak.PageIndex, "", true);
            GetResourcePropertyFilterSum(1, "", selectedResourcePropLocations.VRscrProperty.RscrID);

            //var gridHisTran = Globals.GetViewModel<IPropGridHistory>();
            //gridHisTran.allRsrcPropLocBreak = allRsrcPropLocBreak;
            //GridPropHis = gridHisTran;
            //this.ActivateItem(gridHisTran);

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

        private object _GridPropHis ;
        public object GridPropHis
        {
            get
            {
                return _GridPropHis;
            }
            set
            {
                if (_GridPropHis == value)
                    return;
                _GridPropHis = value;
                NotifyOfPropertyChange(() => GridPropHis);
            }
        }
        public void Exit()
        {
            TryClose();
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

        private int _sum;
        private int _sumGuid;
        private long _sumAll;

        public int sum
        {
            get
            {
                return _sum;
            }
            set
            {
                if (_sum == value)
                    return;
                _sum = value;
                NotifyOfPropertyChange(() => sum);
            }
        }
        public int sumGuid
        {
            get
            {
                return _sumGuid;
            }
            set
            {
                if (_sumGuid == value)
                    return;
                _sumGuid = value;
                NotifyOfPropertyChange(() => sumGuid);
            }
        }
        public long sumAll
        {
            get
            {
                return _sumAll;
            }
            set
            {
                if (_sumAll == value)
                    return;
                _sumAll = value;
                NotifyOfPropertyChange(() => sumAll);
            }
        }
        private PagedSortableCollectionView<ResourcePropLocations> _allRsrcPropLocBreak;
        public PagedSortableCollectionView<ResourcePropLocations> allRsrcPropLocBreak
        {
            get
            {
                return _allRsrcPropLocBreak;
            }
            set
            {
                if (_allRsrcPropLocBreak == value)
                    return;
                _allRsrcPropLocBreak = value;
                NotifyOfPropertyChange(() => allRsrcPropLocBreak);
            }
        }

        private ResourcePropLocations _selectedRsrcPropLocMove;
        public ResourcePropLocations selectedRsrcPropLocMove
        {
            get
            {
                return _selectedRsrcPropLocMove;
            }
            set
            {
                if (_selectedRsrcPropLocMove == value)
                    return;
                _selectedRsrcPropLocMove = value;
                NotifyOfPropertyChange(() => selectedRsrcPropLocMove);
            }
        }
        #endregion

        #region method

        public void lnkMoveHistoryClick(RoutedEventArgs e)
        {
            if (selectedRsrcPropLocMove != null)
            {
                //var PropGridMoveHis = Globals.GetViewModel<IPropMoveHistory>();
                //PropGridMoveHis.RscrID = selectedRsrcPropLocMove.VRscrProperty.RscrPropertyID;
                //this.ActivateItem(PropGridMoveHis);
                //var instance = PropGridMoveHis as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<IPropMoveHistory> onInitDlg = (PropGridMoveHis) =>
                {
                    PropGridMoveHis.RscrID = selectedRsrcPropLocMove.VRscrProperty.RscrPropertyID;
                    this.ActivateItem(PropGridMoveHis);
                };
                GlobalsNAV.ShowDialog<IPropMoveHistory>(onInitDlg);
            }

        }
        private void GetResourcePropLocationsPagingByFilter(string RscrGUID 
                                    , int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    
                    contract.BeginGetResourcePropLocationsPagingByFilter(RscrGUID 
                                ,PageSize, PageIndex, OrderBy, CountTotal,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int total = 0;
                            var items = contract.EndGetResourcePropLocationsPagingByFilter(out total,asyncResult);
                            if (items != null)
                            {
                                
                                sum = 0;
                                allRsrcPropLocBreak.Clear();
                                foreach (var tp in items)
                                {
                                    allRsrcPropLocBreak.Add(tp);
                                    sum += tp.VRscrProperty.QtyAlloc;
                                    //sumGuid = sum;
                                }
                                sumGuid = sum;
                                allRsrcPropLocBreak.TotalItemCount = total;
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

        private void GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                 

                    contract.BeginGetResourcePropertyFilterSum(Choice, RscrGUID, RscrID,Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetResourcePropertyFilterSum(asyncResult);
                            if (items >0)
                            {
                                sumAll = (long)items;
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
        #endregion
    }
}
