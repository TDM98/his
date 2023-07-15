using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptDamageExpiryDrugSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptDamageExpiryDrugSearchViewModel : Conductor<object>, IDrugDeptDamageExpiryDrugSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        #region Indicator Member

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

        #endregion
        public enum DataGridCol
        {
            MAPHIEU = 0,
            NGAY = 1,
            TENKHOXUAT = 2,
            NHANVIENXUAT = 3,
            XUATDENKHO = 4,
            NHANVIENNHAN=5,
            BVBAN = 6,
            MAPHIEUYC= 7,
        }

        [ImportingConstructor]
        public DrugDeptDamageExpiryDrugSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            OutwardDrugMedDeptInvoiceList = new PagedSortableCollectionView<OutwardDrugMedDeptInvoice>();
            OutwardDrugMedDeptInvoiceList.OnRefresh += OutwardDrugMedDeptInvoiceList_OnRefresh;
            OutwardDrugMedDeptInvoiceList.PageSize = Globals.PageSize;
        }

        void OutwardDrugMedDeptInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            MedDeptInvoiceSearchCriteria(OutwardDrugMedDeptInvoiceList.PageIndex, OutwardDrugMedDeptInvoiceList.PageSize);
        }

      
        #region Properties member

        private MedDeptInvoiceSearchCriteria _searchCriteria;
        public MedDeptInvoiceSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

          
        private PagedSortableCollectionView<OutwardDrugMedDeptInvoice> _OutwardDrugMedDeptInvoiceList;
        public PagedSortableCollectionView<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoiceList
        {
            get
            {
                return _OutwardDrugMedDeptInvoiceList;
            }
            set
            {
                if (_OutwardDrugMedDeptInvoiceList != value)
                {
                    _OutwardDrugMedDeptInvoiceList = value;
                    NotifyOfPropertyChange(() => OutwardDrugMedDeptInvoiceList);
                }
            }
        }
        #endregion

        public void MedDeptInvoiceSearchCriteria(int PageIndex, int PageSize)
        {
            IsLoading = true;
            int Total = 0;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize,true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out Total, asyncResult);
                            if (results != null)
                            {
                                OutwardDrugMedDeptInvoiceList.Clear();
                                OutwardDrugMedDeptInvoiceList.TotalItemCount = Total;
                                foreach (OutwardDrugMedDeptInvoice p in results)
                                {
                                    OutwardDrugMedDeptInvoiceList.Add(p);
                                }
                                NotifyOfPropertyChange(() => OutwardDrugMedDeptInvoiceList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugMedDeptInvoiceList.PageIndex = 0;
            MedDeptInvoiceSearchCriteria(OutwardDrugMedDeptInvoiceList.PageIndex, OutwardDrugMedDeptInvoiceList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchDemageDrugEvent { SelectedOutwardDrugMedDeptInvoice = e.Value });
            TryClose();
        }
        DataGrid dataGrid1 = null;
        public void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1 = sender as DataGrid;
            if (SearchCriteria.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
            {
                dataGrid1.Columns[(int)DataGridCol.XUATDENKHO].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.BVBAN].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUYC].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.NHANVIENNHAN].Visibility = Visibility.Collapsed;
            }
            else if (SearchCriteria.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
            {
                dataGrid1.Columns[(int)DataGridCol.XUATDENKHO].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.BVBAN].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUYC].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.NHANVIENNHAN].Visibility = Visibility.Visible;
            }
        }

        public void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (dataGrid1 != null)
            {
                dataGrid1.SetValue(DataGrid.ItemsSourceProperty, null);
            }
        }
    }
}
