using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
using System.Collections.Generic;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyCheckOrder)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyCheckOrderViewModel : Conductor<object>, IPharmacyCheckOrder
    {
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

        #region Properties member

        private string BrandName { get; set; }
        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenMedProductDetaillst;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenMedProductDetaillst
        {
            get
            {
                return _RefGenMedProductDetaillst;
            }
            set
            {
                if (_RefGenMedProductDetaillst != value)
                {
                    _RefGenMedProductDetaillst = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetaillst);
                }
            }
        }


        private RefGenericDrugDetail _CurrentRefGenericDrugDetail;
        public RefGenericDrugDetail CurrentRefGenericDrugDetail
        {
            get
            {
                return _CurrentRefGenericDrugDetail;
            }
            set
            {
                if (_CurrentRefGenericDrugDetail != value)
                {
                    _CurrentRefGenericDrugDetail = value;
                    NotifyOfPropertyChange(() => CurrentRefGenericDrugDetail);
                }
            }
        }

        private ObservableCollection<PharmacyPurchaseCheckOrder> _PharmacyPurchaseCheckOrders;
        public ObservableCollection<PharmacyPurchaseCheckOrder> PharmacyPurchaseCheckOrders
        {
            get
            {
                return _PharmacyPurchaseCheckOrders;
            }
            set
            {
                if (_PharmacyPurchaseCheckOrders != value)
                {
                    _PharmacyPurchaseCheckOrders = value;
                    NotifyOfPropertyChange(() => PharmacyPurchaseCheckOrders);
                }
            }
        }

        private ObservableCollection<PharmacyPurchaseCheckOrderInward> _PharmacyPurchaseCheckOrderInwards;
        public ObservableCollection<PharmacyPurchaseCheckOrderInward> PharmacyPurchaseCheckOrderInwards
        {
            get
            {
                return _PharmacyPurchaseCheckOrderInwards;
            }
            set
            {
                if (_PharmacyPurchaseCheckOrderInwards != value)
                {
                    _PharmacyPurchaseCheckOrderInwards = value;
                    NotifyOfPropertyChange(() => PharmacyPurchaseCheckOrderInwards);
                }
            }
        }

        private DateTime? _FromDate = Globals.ServerDate.Value.AddDays(-30);
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private DateTime? _ToDate = Globals.ServerDate.Value;
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyCheckOrderViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RefGenMedProductDetaillst = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenMedProductDetaillst.PageSize = Globals.PageSize;
            RefGenMedProductDetaillst.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenMedProductDetaillst_OnRefresh);
        }

        void RefGenMedProductDetaillst_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenericDrugDetail_Auto(null, BrandName, RefGenMedProductDetaillst.PageIndex, RefGenMedProductDetaillst.PageSize);
        }


        private string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    //ham tim thuoc theo ma
                    RefGenMedProductDetaillst.PageIndex = 0;
                    GetRefGenericDrugDetail_Auto(true, txt, RefGenMedProductDetaillst.PageIndex, RefGenMedProductDetaillst.PageSize);
                }
            }
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                _VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            VisibilityName = true;
        }


        AutoCompleteBox auto;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            auto = sender as AutoCompleteBox;
        }

        private void GetRefGenericDrugDetail_Auto(bool? IsCode, string BrandName, int PageIndex, int PageSize)
        {
            if (IsCode.GetValueOrDefault())
            {
                IsLoading = true;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_RefAutoPaging(IsCode, BrandName, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndSearchRefDrugGenericDetails_RefAutoPaging(out Total, asyncResult);
                            RefGenMedProductDetaillst.Clear();
                            RefGenMedProductDetaillst.TotalItemCount = Total;
                            if (results == null || results.Count == 0)
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                foreach (RefGenericDrugDetail p in results)
                                {
                                    RefGenMedProductDetaillst.Add(p);
                                }
                                //auto.ItemsSource = RefGenMedProductDetaillst;
                                auto.PopulateComplete();
                                if (IsCode.GetValueOrDefault())
                                {
                                    CurrentRefGenericDrugDetail = RefGenMedProductDetaillst.FirstOrDefault();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsCode.GetValueOrDefault())
                            {
                                IsLoading = false;
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void AutoDrug_Text_Populating(object sender, PopulatingEventArgs e)
        {
            if (CurrentRefGenericDrugDetail != null)
            {
                if (e.Parameter == BrandName)
                {
                    return;
                }
            }
            BrandName = e.Parameter;
            //tim theo ten
            RefGenMedProductDetaillst.PageIndex = 0;
            GetRefGenericDrugDetail_Auto(false, e.Parameter, RefGenMedProductDetaillst.PageIndex, RefGenMedProductDetaillst.PageSize);
        }


        private void PharmacyPurchaseOrderDetails_CheckOrder(DateTime FromDate, DateTime ToDate)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrderDetails_CheckOrder(CurrentRefGenericDrugDetail.DrugID, FromDate, ToDate, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            List<PharmacyPurchaseCheckOrderInward> InwardList = null;
                            var results = contract.EndPharmacyPurchaseOrderDetails_CheckOrder(out InwardList, asyncResult);
                            PharmacyPurchaseCheckOrders = results.ToObservableCollection();
                            PharmacyPurchaseCheckOrderInwards = InwardList.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0438_G1_NhapNgThCanXem);
                return;
            }
            if (FromDate > ToDate)
            {
                MessageBox.Show(eHCMSResources.K0229_G1_TuNgKhongLonHonDenNg);
                return;
            }
            if (FromDate.GetValueOrDefault().Year <= (Globals.ServerDate.Value.Year - 10) || ToDate.GetValueOrDefault().Year <= (Globals.ServerDate.Value.Year - 10))
            {
                MessageBox.Show(eHCMSResources.A0859_G1_Msg_InfoNgThangKhHopLe4 + (Globals.ServerDate.Value.Year - 10).ToString());
                return;
            }
            if (CurrentRefGenericDrugDetail == null || CurrentRefGenericDrugDetail.DrugID==0)
            {
                MessageBox.Show(eHCMSResources.K0058_G1_ThuocKhongTonTai);
                return;
            }
            PharmacyPurchaseOrderDetails_CheckOrder(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault());

        }

        public void GridOrder_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
    }

}
