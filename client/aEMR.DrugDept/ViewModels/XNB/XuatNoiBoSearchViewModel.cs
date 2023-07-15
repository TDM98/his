using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IXuatNoiBoSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XuatNoiBoSearchViewModel : ViewModelBase, IXuatNoiBoSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public XuatNoiBoSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            OutwardMedDeptInvoiceList = new PagedSortableCollectionView<OutwardDrugMedDeptInvoice>();
            OutwardMedDeptInvoiceList.OnRefresh += OutwardMedDeptInvoiceList_OnRefresh;
            OutwardMedDeptInvoiceList.PageSize = 20;
        }

        void OutwardMedDeptInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchOutwardDrugMedDeptInvoice(OutwardMedDeptInvoiceList.PageIndex, OutwardMedDeptInvoiceList.PageSize);
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

        private PagedSortableCollectionView<OutwardDrugMedDeptInvoice> _OutwardMedDeptInvoiceList;
        public PagedSortableCollectionView<OutwardDrugMedDeptInvoice> OutwardMedDeptInvoiceList
        {
            get
            {
                return _OutwardMedDeptInvoiceList;
            }
            set
            {
                if (_OutwardMedDeptInvoiceList != value)
                {
                    _OutwardMedDeptInvoiceList = value;
                    NotifyOfPropertyChange(() => OutwardMedDeptInvoiceList);
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
            OutwardMedDeptInvoiceList.PageIndex = 0;
            SearchOutwardDrugMedDeptInvoice(OutwardMedDeptInvoiceList.PageIndex, OutwardMedDeptInvoiceList.PageSize);
        }

        public void SearchOutwardDrugMedDeptInvoice(int PageIndex, int PageSize)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int Total;
                                var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out Total, asyncResult);
                                OutwardMedDeptInvoiceList.Clear();
                                OutwardMedDeptInvoiceList.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (OutwardDrugMedDeptInvoice p in results)
                                    {
                                        OutwardMedDeptInvoiceList.Add(p);
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
        public void dataGrid1_DblClick(object sender, EventArgs<object> e)
        {
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchOutMedDeptInvoiceEvent { SelectedOutMedDeptInvoice = e.Value });
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
        public void Search_KeyUp_CustomerName(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CustomerName = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
    }
}
