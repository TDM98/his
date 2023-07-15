using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptCheckOrder)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptCheckOrderViewModel : Conductor<object>, IDrugDeptCheckOrder
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

        #region Properties member

        private string BrandName { get; set; }
        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetaillst;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetaillst
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


        private RefGenMedProductDetails _CurrentRefGenericDrugDetail;
        public RefGenMedProductDetails CurrentRefGenericDrugDetail
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

        private ObservableCollection<DrugDeptPurchaseCheckOrder> _DrugDeptPurchaseCheckOrders;
        public ObservableCollection<DrugDeptPurchaseCheckOrder> DrugDeptPurchaseCheckOrders
        {
            get
            {
                return _DrugDeptPurchaseCheckOrders;
            }
            set
            {
                if (_DrugDeptPurchaseCheckOrders != value)
                {
                    _DrugDeptPurchaseCheckOrders = value;
                    NotifyOfPropertyChange(() => DrugDeptPurchaseCheckOrders);
                }
            }
        }

        private ObservableCollection<DrugDeptPurchaseCheckOrderInward> _DrugDeptPurchaseCheckOrderInwards;
        public ObservableCollection<DrugDeptPurchaseCheckOrderInward> DrugDeptPurchaseCheckOrderInwards
        {
            get
            {
                return _DrugDeptPurchaseCheckOrderInwards;
            }
            set
            {
                if (_DrugDeptPurchaseCheckOrderInwards != value)
                {
                    _DrugDeptPurchaseCheckOrderInwards = value;
                    NotifyOfPropertyChange(() => DrugDeptPurchaseCheckOrderInwards);
                }
            }
        }

        private DateTime? _FromDate=DateTime.Now.AddDays(-30);
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

        private DateTime? _ToDate=DateTime.Now;
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

        [ImportingConstructor]
        public DrugDeptCheckOrderViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RefGenMedProductDetaillst = new PagedSortableCollectionView<RefGenMedProductDetails>();
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
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    //ham tim thuoc theo ma
                    RefGenMedProductDetaillst.PageIndex = 0;
                    GetRefGenericDrugDetail_Auto(true, Code, RefGenMedProductDetaillst.PageIndex, RefGenMedProductDetaillst.PageSize);
                }
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
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
                    contract.BeginGetRefGenMedProductDetails_Auto(IsCode, BrandName, V_MedProductType, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndGetRefGenMedProductDetails_Auto(out Total, asyncResult);
                            RefGenMedProductDetaillst.Clear();
                            RefGenMedProductDetaillst.TotalItemCount = Total;
                            if (results == null || results.Count == 0)
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    if (tbx != null)
                                    {
                                        txt = "";
                                        tbx.Text = "";
                                    }
                                    if (auto != null)
                                    {
                                        auto.Text = "";
                                    }

                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                foreach (RefGenMedProductDetails p in results)
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

        public long V_MedProductType { get; set; }

        private void DrugDeptPurchaseOrderDetails_CheckOrder(DateTime FromDate, DateTime ToDate)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptPurchaseOrderDetails_CheckOrder(CurrentRefGenericDrugDetail.GenMedProductID, V_MedProductType, FromDate, ToDate, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            List<DrugDeptPurchaseCheckOrderInward> InwardList = null;
                            var results = contract.EndDrugDeptPurchaseOrderDetails_CheckOrder(out InwardList,asyncResult);
                            DrugDeptPurchaseCheckOrders = results.ToObservableCollection();
                            DrugDeptPurchaseCheckOrderInwards = InwardList.ToObservableCollection();
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
            if (FromDate.GetValueOrDefault().Year <= (DateTime.Now.Year-10) || ToDate.GetValueOrDefault().Year <= (DateTime.Now.Year - 10) )
            {
                MessageBox.Show(eHCMSResources.A0859_G1_Msg_InfoNgThangKhHopLe4 + (DateTime.Now.Year - 10).ToString());
                return;
            }
            if (CurrentRefGenericDrugDetail==null || CurrentRefGenericDrugDetail.GenMedProductID==0)
            {
                MessageBox.Show(eHCMSResources.A0816_G1_Msg_InfoHgKhTonTai);
                return;
            }
            DrugDeptPurchaseOrderDetails_CheckOrder(FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault());

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
