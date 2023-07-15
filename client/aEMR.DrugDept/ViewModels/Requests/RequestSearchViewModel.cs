using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using Service.Core.Common;
using aEMR.DrugDept.Views;
using System.Text;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.BaseModel;
/*
 * 20181219 #001 TTM: BM 0005443: Thêm điều kiện để lọc phiếu lĩnh từ kho BHYT - Nhà thuốc hay là phiếu lĩnh kho phòng.
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRequestSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestSearchViewModel : ViewModelBase, IRequestSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RequestSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();

            RequestDruglist = new PagedSortableCollectionView<RequestDrugInwardClinicDept>();
            RequestDruglist.OnRefresh += RequestDruglist_OnRefresh;
            RequestDruglist.PageSize = 20;

            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeForAllowUpdateMedicalInstruction)
            {
                MaxDay = Globals.GetCurServerDateTime();
            }
            else
            {
                MaxDay = DateTime.MaxValue;
            }
            
        }
        //▼====== #001
        public void SetList()
        {
            RequestDruglistHIStore = new PagedSortableCollectionView<RequestDrugInwardForHiStore>();
            RequestDruglistHIStore.OnRefresh += RequestDruglistHIStore_OnRefresh;
            RequestDruglistHIStore.PageSize = 20;
        }
        private DateTime? _MaxDay ;
        public DateTime? MaxDay
        {
            get
            {
                return _MaxDay;
            }
            set
            {
                if (_MaxDay != value)
                {
                    _MaxDay = value;
                    NotifyOfPropertyChange(() => MaxDay);
                }
            }
        }
        private PagedSortableCollectionView<RequestDrugInwardForHiStore> _RequestDruglistHIStore;
        public PagedSortableCollectionView<RequestDrugInwardForHiStore> RequestDruglistHIStore
        {
            get
            {
                return _RequestDruglistHIStore;
            }
            set
            {
                if (_RequestDruglistHIStore != value)
                {
                    _RequestDruglistHIStore = value;
                    NotifyOfPropertyChange(() => RequestDruglistHIStore);
                }
            }
        }
        private bool _vGrid = true;
        public bool vGrid
        {
            get
            {
                return _vGrid;
            }
            set
            {
                _vGrid = value;
                NotifyOfPropertyChange(() => vGrid);
            }
        }
        private bool _IsRequestFromHIStore = false;
        public bool IsRequestFromHIStore
        {
            get
            {
                return _IsRequestFromHIStore;
            }
            set
            {
                _IsRequestFromHIStore = value;
                if (_IsRequestFromHIStore)
                {
                    vGrid = false;
                }
                NotifyOfPropertyChange(() => IsRequestFromHIStore);
            }
        }
        void RequestDruglistHIStore_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
        }
        public void SearchRequestDrugInwardHIStore(int PageIndex, int PageSize)
        {
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardHIStore(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardHIStore(out Total, asyncResult);
                                RequestDruglistHIStore.Clear();
                                RequestDruglistHIStore.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (RequestDrugInwardForHiStore p in results)
                                    {
                                        RequestDruglistHIStore.Add(p);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                        }), null);

                    }

                });

                t.Start();

            }
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }
        //▲====== #001
        void RequestDruglist_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
        }

        #region Properties member

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }
        private PagedSortableCollectionView<RequestDrugInwardClinicDept> _RequestDrugList;
        public PagedSortableCollectionView<RequestDrugInwardClinicDept> RequestDruglist
        {
            get
            {
                return _RequestDrugList;
            }
            set
            {
                if (_RequestDrugList != value)
                {
                    _RequestDrugList = value;
                    NotifyOfPropertyChange(() => RequestDruglist);
                }
            }
        }

        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private bool _IsFromApprovePage = false;
        public bool IsFromApprovePage
        {
            get { return _IsFromApprovePage; }
            set
            {
                if (_IsFromApprovePage != value)
                {
                    _IsFromApprovePage = value;
                    NotifyOfPropertyChange(() => IsFromApprovePage);
                }
            }
        }
        #endregion

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            if(SearchCriteria.FromDate > MaxDay)
            {
                SearchCriteria.FromDate = MaxDay;
            }
            if (SearchCriteria.ToDate > MaxDay)
            {
                SearchCriteria.ToDate = MaxDay;
            }
            //▼====== #001
            if (!IsRequestFromHIStore)
            {
                RequestDruglist.PageIndex = 0;
                SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
            }
            else
            {
                RequestDruglistHIStore.PageIndex = 0;
                SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
            }
            //▲====== #001
        }

        public void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                         {

                             try
                             {
                                 int Total = 0;
                                 var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                                 RequestDruglist.Clear();
                                 RequestDruglist.TotalItemCount = Total;
                                 if (results != null)
                                 {
                                     foreach (RequestDrugInwardClinicDept p in results)
                                     {
                                         RequestDruglist.Add(p);
                                     }
                                 }
                             }
                             catch (Exception ex)
                             {
                                 Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                 _logger.Error(ex.Message);
                             }
                             finally
                             {
                                 this.DlgHideBusyIndicator();
                             }
                         }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            RequestDrugInwardClinicDept Request = e.Value as RequestDrugInwardClinicDept;
            if (Request.DaNhanHang == true && !IsFromApprovePage)
            {
                MessageBox.Show("Phiếu này đã xuất hàng. Không thể xuất tiếp", eHCMSResources.G0442_G1_TBao);
                return;
            }
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestEvent { SelectedRequest = e.Value });
        }
        //▼====== #001
        public void dataGrid2_DblClick(object sender, Common.EventArgs<object> e)
        {
            RequestDrugInwardForHiStore Request = e.Value as RequestDrugInwardForHiStore;
            if (Request.DaNhanHang == true && MessageBox.Show(eHCMSResources.A0955_G1_Msg_InfoXuatTiepPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }
            TryClose();
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestForHIStoreEvent { SelectedRequest = e.Value });
        }
        //▲====== #001
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                RequestDruglist.PageIndex = 0;
                //▼====== #001
                if (!IsRequestFromHIStore)
                {
                    SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
                }
                else
                {
                    SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
                }
                //▲====== #001
            }
        }
    }
}
