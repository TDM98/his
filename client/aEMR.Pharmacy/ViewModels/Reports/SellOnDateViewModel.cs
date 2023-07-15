using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Collections;
using aEMR.Pharmacy.Views;
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
/*
 * 20200218 #001 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISellOnDate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SellOnDateViewModel : Conductor<object>, ISellOnDate
    {
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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SellOnDateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            OutwardDrugList = new PagedSortableCollectionView<OutwardDrug>();
            OutwardDrugList.OnRefresh += OutwardDrugList_OnRefresh;
            OutwardDrugList.PageSize = 25;

            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugSimple>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
        }

        void OutwardDrugList_OnRefresh(object sender, RefreshEventArgs e)
        {
            OutwardDrugs_SellOnDate(OutwardDrugList.PageIndex, OutwardDrugList.PageSize);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
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
                                               , (int)ePharmacy.mThongKe_BanTheoNgay,
                                               (int)oPharmacyEx.mThongKe_BanTheoNgay_Xem, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mThongKe_BanTheoNgay,
                                               (int)oPharmacyEx.mThongKe_BanTheoNgay_In, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mThongKe_BanTheoNgay,
                                               (int)oPharmacyEx.mThongKe_BanTheoNgay_ChinhSua, (int)ePermission.mView);
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

        private PagedSortableCollectionView<OutwardDrug> _OutwardDrugList;
        public PagedSortableCollectionView<OutwardDrug> OutwardDrugList
        {
            get
            {
                return _OutwardDrugList;
            }
            set
            {
                if (_OutwardDrugList != value)
                {
                    _OutwardDrugList = value;
                    NotifyOfPropertyChange(() => OutwardDrugList);
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

        private DateTime _FromDate = Globals.ServerDate.Value;
        public DateTime FromDate
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

        private DateTime _ToDate = Globals.ServerDate.Value;
        public DateTime ToDate
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
        private void OutwardDrugs_SellOnDate(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugs_SellOnDate(BrandName, StoreID, FromDate, ToDate, IsDetail, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutwardDrugs_SellOnDate(out int Total, out decimal Money, asyncResult);
                                TotalMoney = Money;
                                OutwardDrugList.Clear();
                                OutwardDrugList.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (OutwardDrug p in results)
                                    {
                                        OutwardDrugList.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => OutwardDrugList);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                }
            });

            t.Start();
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }

        #endregion

        private bool CheckData()
        {
            if (FromDate == null || ToDate == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1672_G1_NhapNgCanXem, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            else
            {
                if (FromDate.Year < Globals.ServerDate.Value.Year - 25 || ToDate.Year < Globals.ServerDate.Value.Year - 25)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z1673_G1_NamCanXemKgDcNhoHon0, (Globals.ServerDate.Value.Year - 25).ToString()), eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }
            return true;
        }

        private void HideShowColumns(DataGrid dg)
        {
            if (dg == null)
            {
                return;
            }
            if (IsDetail)
            {
                dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Visible;
            }
            else
            {
                dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Collapsed;
            }
        }

        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1 + ".";
        }

        public void btnView()
        {
            if (CheckData())
            {
                HideShowColumns(((SellOnDateView)this.GetView()).GridInward);
                OutwardDrugList.PageIndex = 0;
                OutwardDrugs_SellOnDate(OutwardDrugList.PageIndex, OutwardDrugList.PageSize);
            }
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

        private PagedSortableCollectionView<RefGenericDrugSimple> _RefGenericDrugDetails;
        public PagedSortableCollectionView<RefGenericDrugSimple> RefGenericDrugDetails
        {
            get
            {
                return _RefGenericDrugDetails;
            }
            set
            {
                if (_RefGenericDrugDetails != value)
                {
                    _RefGenericDrugDetails = value;
                }
                NotifyOfPropertyChange(() => RefGenericDrugDetails);
            }
        }

        private void SearchRefDrugGenericDetails_AutoPaging(string Name, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRefGenericDrugName_SimpleAutoPaging(null, Name, PageIndex, PageSize, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ListUnits = contract.EndSearchRefGenericDrugName_SimpleAutoPaging(out int totalCount, asyncResult);
                                if (ListUnits != null)
                                {
                                    RefGenericDrugDetails.Clear();
                                    RefGenericDrugDetails.TotalItemCount = totalCount;
                                    RefGenericDrugDetails.ItemCount = totalCount;
                                    foreach (RefGenericDrugSimple p in ListUnits)
                                    {
                                        RefGenericDrugDetails.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => RefGenericDrugDetails);
                                }
                                AuDrug.ItemsSource = RefGenericDrugDetails;
                                AuDrug.PopulateComplete();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        AutoCompleteBox AuDrug;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            AuDrug = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            RefGenericDrugDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, RefGenericDrugDetails.PageSize);
        }
        #endregion
    }
}
