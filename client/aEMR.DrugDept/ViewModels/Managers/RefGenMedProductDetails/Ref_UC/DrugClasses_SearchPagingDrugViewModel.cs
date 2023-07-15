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
    [Export(typeof(IDrugClasses_SearchPaging_Drug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugClasses_SearchPaging_DrugViewModel : Conductor<object>, IDrugClasses_SearchPaging_Drug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        private Int64 _V_MedProductType;
        public Int64 V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }

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

        [ImportingConstructor]
        public DrugClasses_SearchPaging_DrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ObjList = new PagedSortableCollectionView<DrugClass>();
        }
        
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IDrugClasses_SearchPaging_DrugView;
        }
        IDrugClasses_SearchPaging_DrugView _currentView;

        protected override void OnActivate()
        {
            base.OnActivate();
            
            ObjList.OnRefresh += new EventHandler<RefreshEventArgs>(ObjList_OnRefresh);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        void ObjList_OnRefresh(object sender, RefreshEventArgs e)
        {
            DrugClasses_SearchPaging(ObjList.PageIndex, ObjList.PageSize, false);
        }

        public DrugClassSearchCriteria _criteria;
        public DrugClassSearchCriteria criteria
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
            if (criteria.FaName != source.SearchText)
            {
                criteria.FaName = source.SearchText;
                DrugClasses_SearchPaging(0, ObjList.PageSize, true);
            }
        }

        private PagedSortableCollectionView<DrugClass> _ObjList;
        public PagedSortableCollectionView<DrugClass> ObjList
        {
            get { return _ObjList; }
            set
            {   
                _ObjList = value;
                NotifyOfPropertyChange(()=>ObjList);
            }
        }

        private void DrugClasses_SearchPaging(int pageIndex, int pageSize, bool bCountTotal)
        {
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugClasses_SearchPaging(criteria, pageIndex,pageSize,bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<DataEntities.DrugClass> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDrugClasses_SearchPaging(out totalCount, asyncResult);
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
                            //var view = this.GetView() as IDrugClasses_SearchPaging_DrugView;
                            //view.PopulateComplete();
                            if(_currentView != null)
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


        private DrugClass _ObjDrugClasses_Selected;
        public DrugClass ObjDrugClasses_Selected
        {
            get { return _ObjDrugClasses_Selected; }
            set
            {
                _ObjDrugClasses_Selected = value;
                NotifyOfPropertyChange(() => ObjDrugClasses_Selected);
            }
        }

        public void SelectionChangedCmd(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                ObjDrugClasses_Selected = e.AddedItems[0] as DrugClass;
                Globals.EventAggregator.Publish(new DrugClasses_SearchPagingDrugViewModel_Selected_Event() { ObjectSelected = ObjDrugClasses_Selected });
            }
            else
            {
                Globals.EventAggregator.Publish(new DrugClasses_SearchPagingDrugViewModel_Selected_Event() { ObjectSelected = null });
            }
        }

    }
}
