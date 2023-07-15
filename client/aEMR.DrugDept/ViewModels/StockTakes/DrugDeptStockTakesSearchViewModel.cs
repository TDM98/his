using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.CommonTasks;
using eHCMSLanguage;
using Castle.Windsor;
using System.Linq;
using aEMR.Common.BaseModel;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptStockTakesSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptStockTakesSearchViewModel : ViewModelBase, IDrugDeptStockTakesSearch
    {
        [ImportingConstructor]
        public DrugDeptStockTakesSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            DrugDeptStockTakeList = new PagedSortableCollectionView<DrugDeptStockTakes>();
            DrugDeptStockTakeList.OnRefresh += DrugDeptStockTakeList_OnRefresh;
            DrugDeptStockTakeList.PageSize = Globals.PageSize;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStore_DrugDept());
        }

        void DrugDeptStockTakeList_OnRefresh(object sender, RefreshEventArgs e)
        {
            DrugDeptStockTakesSearchCriteria(DrugDeptStockTakeList.PageIndex, DrugDeptStockTakeList.PageSize);
        }

        #region Properties member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private DrugDeptStockTakesSearchCriteria _searchCriteria;
        public DrugDeptStockTakesSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }


        private PagedSortableCollectionView<DrugDeptStockTakes> _DrugDeptStockTakeList;
        public PagedSortableCollectionView<DrugDeptStockTakes> DrugDeptStockTakeList
        {
            get
            {
                return _DrugDeptStockTakeList;
            }
            set
            {
                if (_DrugDeptStockTakeList != value)
                {
                    _DrugDeptStockTakeList = value;
                    NotifyOfPropertyChange(() => DrugDeptStockTakeList);
                }
            }
        }


        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }
        #endregion

        private IEnumerator<IResult> DoGetStore_DrugDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            yield break;
        }

        public void DrugDeptStockTakesSearchCriteria(int PageIndex, int PageSize)
        {
            int Total = 0;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptStockTakes_Search(out Total, asyncResult);
                                if (results != null)
                                {
                                    DrugDeptStockTakeList.Clear();
                                    DrugDeptStockTakeList.TotalItemCount = Total;
                                    foreach (DrugDeptStockTakes p in results)
                                    {
                                        DrugDeptStockTakeList.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => DrugDeptStockTakeList);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgShowBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void BtnSearch(object sender, RoutedEventArgs e)
        {
            DrugDeptStockTakeList.PageIndex = 0;
            DrugDeptStockTakesSearchCriteria(DrugDeptStockTakeList.PageIndex, DrugDeptStockTakeList.PageSize);
        }

        public void DataGrid1_DblClick(object sender, EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchStockTakesEvent { SelectedDrugDeptStockTakes = e.Value });
            TryClose();
        }
    }
}
