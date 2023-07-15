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
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Common.BaseModel;

namespace aEMR.StoreDept.Outwards.ViewModels
{
    [Export(typeof(IXuatNoiBoSearchClinicDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XuatNoiBoSearchViewModel : ViewModelBase, IXuatNoiBoSearchClinicDept
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public XuatNoiBoSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            OutwardClinicDeptInvoiceList = new PagedSortableCollectionView<OutwardDrugClinicDeptInvoice>();
            OutwardClinicDeptInvoiceList.OnRefresh += OutwardClinicDeptInvoiceList_OnRefresh;
            OutwardClinicDeptInvoiceList.PageSize = 20;
        }

        void OutwardClinicDeptInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchOutwardDrugClinicDeptInvoice(OutwardClinicDeptInvoiceList.PageIndex, OutwardClinicDeptInvoiceList.PageSize);
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

        private long _V_MedProductType = 11001; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private PagedSortableCollectionView<OutwardDrugClinicDeptInvoice> _OutwardClinicDeptInvoiceList;
        public PagedSortableCollectionView<OutwardDrugClinicDeptInvoice> OutwardClinicDeptInvoiceList
        {
            get
            {
                return _OutwardClinicDeptInvoiceList;
            }
            set
            {
                if (_OutwardClinicDeptInvoiceList != value)
                {
                    _OutwardClinicDeptInvoiceList = value;
                    NotifyOfPropertyChange(() => OutwardClinicDeptInvoiceList);
                }
            }
        }


        private MedDeptInvoiceSearchCriteria _SearchCriteria;
        public MedDeptInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(()=>SearchCriteria);
                }
            }
        }

        #endregion

        public void btnSearch()
        {
            OutwardClinicDeptInvoiceList.PageIndex = 0;
            SearchOutwardDrugClinicDeptInvoice(OutwardClinicDeptInvoiceList.PageIndex, OutwardClinicDeptInvoiceList.PageSize);
        }

        public void SearchOutwardDrugClinicDeptInvoice(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugClinicDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int Total;
                                var results = contract.EndOutwardDrugClinicDeptInvoice_SearchByType(out Total, asyncResult);
                                OutwardClinicDeptInvoiceList.Clear();
                                OutwardClinicDeptInvoiceList.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (OutwardDrugClinicDeptInvoice p in results)
                                    {
                                        OutwardClinicDeptInvoiceList.Add(p);
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
                                IsLoading = false;
                                //Globals.IsBusy = false;
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
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchOutClinicDeptInvoiceEvent { SelectedOutClinicDeptInvoice = e.Value });
        }

        public void Search_KeyUp_CodeInvoice(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeInvoice = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void Search_KeyUp_CodeRequest(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeRequest = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardNo = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch();
            }
        }
    }
}
