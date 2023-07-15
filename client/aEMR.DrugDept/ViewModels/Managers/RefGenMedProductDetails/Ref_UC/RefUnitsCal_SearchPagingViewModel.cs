using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Threading;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.Common;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRefUnitsCal_SearchPaging)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefUnitsCal_SearchPagingViewModel : Conductor<object>, IRefUnitsCal_SearchPaging
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefUnitsCal_SearchPagingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            criteria = new RefUnitSearchCriteria();
            ObjList = new PagedSortableCollectionView<RefUnit>();
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IRefUnitsCal_SearchPagingView;
        }
        IRefUnitsCal_SearchPagingView _currentView;


        protected override void OnActivate()
        {
            base.OnActivate();

            ObjList.OnRefresh += new EventHandler<RefreshEventArgs>(ObjList_OnRefresh);
        }

        void ObjList_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefUnits_SearchPaging(ObjList.PageIndex, ObjList.PageSize, false);
        }

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public RefUnitSearchCriteria _criteria;
        public RefUnitSearchCriteria criteria
        {
            get
            {
                return _criteria;
            }
            set
            {
                _criteria = value;
                NotifyOfPropertyChange(() => criteria);
            }
        }

        public void PopulatingCmd(object sender, object eventArgs)
        {
            var source = sender as AutoCompleteBox;
            if (criteria.UnitName != source.SearchText)
            {
                criteria.UnitName = source.SearchText;
                RefUnits_SearchPaging(0, ObjList.PageSize, true);
            }
        }

        private PagedSortableCollectionView<RefUnit> _ObjList;
        public PagedSortableCollectionView<RefUnit> ObjList
        {
            get { return _ObjList; }
            set
            {
                _ObjList = value;
                NotifyOfPropertyChange(() => ObjList);
            }
        }

        private void RefUnits_SearchPaging(int pageIndex, int pageSize, bool bCountTotal)
        {
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefUnits_SearchPaging(criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<DataEntities.RefUnit> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefUnits_SearchPaging(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                            ObjList.Clear();

                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    ObjList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjList.Add(item);
                                    }

                                }
                            }
                            //var view = this.GetView() as IRefUnitsCal_SearchPagingView;
                            //view.PopulateComplete();
                            if(_currentView!=null)
                            {
                                _currentView.PopulateComplete();
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }


        private RefUnit _ObjRefUnits_Selected;
        public RefUnit ObjRefUnits_Selected
        {
            get { return _ObjRefUnits_Selected; }
            set
            {
                _ObjRefUnits_Selected = value;
                NotifyOfPropertyChange(() => ObjRefUnits_Selected);
            }
        }

        public void SelectionChangedCmd(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                ObjRefUnits_Selected = e.AddedItems[0] as RefUnit;
                Globals.EventAggregator.Publish(new RefUnitsCal_SearchPagingView_Selected_Event() { ObjectSelected = ObjRefUnits_Selected });
            }
            else
            {
                Globals.EventAggregator.Publish(new RefUnitsCal_SearchPagingView_Selected_Event() { ObjectSelected = null });
            }
        }

    }
}
