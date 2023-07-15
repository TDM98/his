using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Collections;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Microsoft.Win32;
using aEMR.Common;
/*
 * 20191106 #001 TNHX: [BM 0013306]: separate V_MedProductType
 * 20211102 #002 QTD:  Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IInStock)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InStockViewModel : Conductor<object>, IInStock
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public string TitleForm { get; set; }

        private enum DataGridCol
        {
            MaThuoc = 0,
            TenThuoc = 1,
            HamLuong = 2,
            DVT = 3,
            SLTon = 4,
            GiaBan = 5,
            ThanhTien = 6,
            LoSX = 7,
            HanDung = 8,
            ViTri = 9
        }

        [ImportingConstructor]
        public InStockViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            //KMx: Chưa làm phân quyền (15/01/2015 15:00)
            //authorization();
            InwardDrugList = new PagedSortableCollectionView<InwardDrugMedDept>();
            InwardDrugList.OnRefresh += InwardDrugList_OnRefresh;
            InwardDrugList.PageSize = 25;

            RefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductSimple>();
            RefGenMedProductDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenMedProductDetails.PageSize = Globals.PageSize;

            LoadBids();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            TitleForm = Globals.TitleForm;
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

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
        }

        void InwardDrugList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardDrugs_TonKho(InwardDrugList.PageIndex, InwardDrugList.PageSize);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bXem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mThongKe_TonKho,
                                               (int)oPharmacyEx.mThongKe_TonKho_Xem, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mThongKe_TonKho,
                                               (int)oPharmacyEx.mThongKe_TonKho_In, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mThongKe_TonKho,
                                               (int)oPharmacyEx.mThongKe_TonKho_ChinhSua, (int)ePermission.mView);
        }

        #region checking account

        private bool _bXem = true;
        private bool _bIn = true;
        private bool _bChinhSua = true;

        public bool bXem
        {
            get
            {
                return _bXem;
            }
            set
            {
                if (_bXem == value)
                    return;
                _bXem = value;
            }
        }

        public bool bIn
        {
            get
            {
                return _bIn;
            }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
            }
        }

        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        #endregion

        #region Properties Member

        private PagedSortableCollectionView<InwardDrugMedDept> _InwardDrugList;
        public PagedSortableCollectionView<InwardDrugMedDept> InwardDrugList
        {
            get
            {
                return _InwardDrugList;
            }
            set
            {
                if (_InwardDrugList != value)
                {
                    _InwardDrugList = value;
                    NotifyOfPropertyChange(() => InwardDrugList);
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private bool _IsDetail;
        public bool IsDetail
        {
            get
            {
                return _IsDetail;
            }
            set
            {
                if (_IsDetail != value)
                {
                    _IsDetail = value;
                    NotifyOfPropertyChange(() => IsDetail);
                }
            }
        }

        private decimal _TotalMoney;
        public decimal TotalMoney
        {
            get
            {
                return _TotalMoney;
            }
            set
            {
                if (_TotalMoney != value)
                {
                    _TotalMoney = value;
                    NotifyOfPropertyChange(() => TotalMoney);
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

        #endregion

        #region function member
        private void InwardDrugs_TonKho(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInStock_MedDept(BrandName, StoreID, IsDetail, V_MedProductType, PageIndex, PageSize, SelectedBid == null ? null : (long?)SelectedBid.BidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndInStock_MedDept(out int Total, out decimal Money, asyncResult);
                                TotalMoney = Money;
                                InwardDrugList.Clear();
                                InwardDrugList.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (InwardDrugMedDept p in results)
                                    {
                                        InwardDrugList.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => InwardDrugList);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });

            t.Start();
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #002
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if(StoreCbx != null && StoreCbx.Count > 0)
            {
                SetDefaultForStore();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #002
            yield break;
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }
        #endregion

        private void HideShowColumns(DataGrid dg)
        {
            if (dg == null)
            {
                return;
            }

            //if (IsDetail)
            //{
            //    dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Visible;
            //    dg.Columns[(int)DataGridCol.HanDung].Visibility = Visibility.Visible;
            //    dg.Columns[(int)DataGridCol.ViTri].Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Collapsed;
            //    dg.Columns[(int)DataGridCol.HanDung].Visibility = Visibility.Collapsed;
            //    dg.Columns[(int)DataGridCol.ViTri].Visibility = Visibility.Collapsed;
            //}

            var colNormalPrice = dg.GetColumnByName("colNormalPrice");
            var colHIPatientPrice = dg.GetColumnByName("colHIPatientPrice");
            var colHIAllowedPrice = dg.GetColumnByName("colHIAllowedPrice");
            var colInBatchNumber = dg.GetColumnByName("colInBatchNumber");
            var colInExpiryDate = dg.GetColumnByName("colInExpiryDate");
            var colShelfName = dg.GetColumnByName("colShelfName");

            if (IsDetail)
            {
                if (colNormalPrice != null)
                {
                    colNormalPrice.Visibility = Visibility.Visible;
                }
                if (colHIPatientPrice != null)
                {
                    colHIPatientPrice.Visibility = Visibility.Visible;
                }
                if (colHIAllowedPrice != null)
                {
                    colHIAllowedPrice.Visibility = Visibility.Visible;
                }
                if (colInBatchNumber != null)
                {
                    colInBatchNumber.Visibility = Visibility.Visible;
                }
                if (colInExpiryDate != null)
                {
                    colInExpiryDate.Visibility = Visibility.Visible;
                }
                if (colShelfName != null)
                {
                    colShelfName.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (colNormalPrice != null)
                {
                    colNormalPrice.Visibility = Visibility.Collapsed;
                }
                if (colHIPatientPrice != null)
                {
                    colHIPatientPrice.Visibility = Visibility.Collapsed;
                }
                if (colHIAllowedPrice != null)
                {
                    colHIAllowedPrice.Visibility = Visibility.Collapsed;
                }
                if (colInBatchNumber != null)
                {
                    colInBatchNumber.Visibility = Visibility.Collapsed;
                }
                if (colInExpiryDate != null)
                {
                    colInExpiryDate.Visibility = Visibility.Collapsed;
                }
                if (colShelfName != null)
                {
                    colShelfName.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1 + ".";
        }

        public void btnView()
        {
            if (GridInward != null)
            {
                HideShowColumns(GridInward);
            }
            InwardDrugList.PageIndex = 0;
            InwardDrugs_TonKho(InwardDrugList.PageIndex, InwardDrugList.PageSize);
        }

        DataGrid GridInward = null;
        public void GridInward_Loaded(object sender, RoutedEventArgs e)
        {
            GridInward = sender as DataGrid;
        }

        public void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsDetail = true;
        }

        public void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsDetail = false;
        }

        #region Auto for Drug Member

        private string _BrandName;
        public string BrandName
        {
            get
            {
                return _BrandName;
            }
            set
            {
                if (_BrandName != value)
                {
                    _BrandName = value;
                    NotifyOfPropertyChange(() => BrandName);
                }
            }
        }

        private PagedSortableCollectionView<RefGenMedProductSimple> _RefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductSimple> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                }
                NotifyOfPropertyChange(() => RefGenMedProductDetails);
            }
        }

        private void SearchRefDrugGenericDetails_AutoPaging(string Name, int PageIndex, int PageSize)
        {
            //KMx: Hàm này được copy và chỉnh sửa giống bên Thẻ Kho (16/01/2015 10:22).
            int totalCount = 0;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefGenMedProductDetails_SimpleAutoPaging(null, Name, V_MedProductType, null, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var ListUnits = contract.EndRefGenMedProductDetails_SimpleAutoPaging(out totalCount, asyncResult);
                                if (ListUnits != null)
                                {
                                    RefGenMedProductDetails.Clear();
                                    RefGenMedProductDetails.TotalItemCount = totalCount;
                                    RefGenMedProductDetails.ItemCount = totalCount;
                                    foreach (RefGenMedProductSimple p in ListUnits)
                                    {
                                        RefGenMedProductDetails.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
                                }
                                AuDrug.ItemsSource = RefGenMedProductDetails;
                                AuDrug.PopulateComplete();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });

            t.Start();
        }

        AutoCompleteBox AuDrug;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            AuDrug = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            RefGenMedProductDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, RefGenMedProductDetails.PageSize);
        }

        #endregion

        public void btnExportExcel()
        {
            if (StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0325_G1_ChonKhoDeXuatExcel, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.TON_KHO;
            RptParameters.FromDepartment = DepartmentReport.KHOA_DUOC;
            RptParameters.StoreID = StoreID;
            RptParameters.BrandName = BrandName;
            RptParameters.IsDetail = IsDetail;
            RptParameters.V_MedProductType = V_MedProductType;
            RptParameters.Show = "TonKho_" + Globals.GetCurServerDateTime().Date.ToString("dd_MM_yyyy");
            RptParameters.BidID = SelectedBid == null ? null : (long?)SelectedBid.BidID;
            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
        }
        private ObservableCollection<Bid> _gBidCollection;
        public ObservableCollection<Bid> gBidCollection
        {
            get
            {
                return _gBidCollection;
            }
            set
            {
                if (_gBidCollection != value)
                {
                    _gBidCollection = value;
                    NotifyOfPropertyChange(() => gBidCollection);
                }
            }
        }
        private Bid _SelectedBid;
        public Bid SelectedBid
        {
            get
            {
                return _SelectedBid;
            }
            set
            {
                if (_SelectedBid == value)
                {
                    return;
                }
                _SelectedBid = value;
                NotifyOfPropertyChange(() => SelectedBid);
            }
        }
        private void LoadBids()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetAllBids(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                gBidCollection = new ObservableCollection<Bid>();
                                var ItemCollection = contract.EndGetAllBids(asyncResult);
                                if (ItemCollection != null)
                                {
                                    gBidCollection = new ObservableCollection<Bid>(ItemCollection);
                                }
                                gBidCollection.Insert(0, new Bid { BidID = 0, BidName = eHCMSResources.A0015_G1_Chon });
                                SelectedBid = gBidCollection.FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }
    }
}