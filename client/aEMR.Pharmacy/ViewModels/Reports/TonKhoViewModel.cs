using eHCMSLanguage;
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
using System.Windows.Controls;
using System.Linq;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Microsoft.Win32;
/*
 * 20200218 #001 TNHX: [] Thêm điều kiện lọc danh mục NT + Kho BHYT Ngoại trú
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ITonKho)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TonKhoViewModel : Conductor<object>, ITonKho
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
        public TonKhoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            InwardDrugList = new PagedSortableCollectionView<InwardDrug>();
            InwardDrugList.OnRefresh += InwardDrugList_OnRefresh;
            InwardDrugList.PageSize = 25;

            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugSimple>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
        }

        void InwardDrugList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardDrugs_TonKho(InwardDrugList.PageIndex, InwardDrugList.PageSize);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public void Authorization()
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

        private PagedSortableCollectionView<InwardDrug> _InwardDrugList;
        public PagedSortableCollectionView<InwardDrug> InwardDrugList
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
        private void InwardDrugs_TonKho(int PageIndex,int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrugs_TonKho(BrandName, StoreID, IsDetail, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                          {
                              try
                              {
                                  var results = contract.EndInwardDrugs_TonKho(out int Total, out decimal Money, asyncResult);
                                  TotalMoney = Money;
                                  InwardDrugList.Clear();
                                  InwardDrugList.TotalItemCount = Total;
                                  if (results != null)
                                  {
                                      foreach (InwardDrug p in results)
                                      {
                                          InwardDrugList.Add(p);
                                      }
                                      NotifyOfPropertyChange(() => InwardDrugList);
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
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
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

        private void HideShowColumns(DataGrid dg)
        {
            if (dg == null)
            {
                return;
            }
            if (IsDetail)
            {
                dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Visible;
                dg.Columns[(int)DataGridCol.HanDung].Visibility = Visibility.Visible;
                dg.Columns[(int)DataGridCol.ViTri].Visibility = Visibility.Visible;
            }
            else
            {
                dg.Columns[(int)DataGridCol.LoSX].Visibility = Visibility.Collapsed;
                dg.Columns[(int)DataGridCol.HanDung].Visibility = Visibility.Collapsed;
                dg.Columns[(int)DataGridCol.ViTri].Visibility = Visibility.Collapsed;
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

        DataGrid GridInward=null;
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
            //20180412 TTM: Bỏ Busy ra vì busy sẽ làm cho AutoComplete ko gõ liên tiếp đc mà sẽ xảy ra hiện tượng đánh đc 1 chữ không đánh đc chữ thứ 2.
            //this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                 //this.HideBusyIndicator();
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

        AutoCompleteBox AuDrug;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            AuDrug = sender as AutoCompleteBox;
            BrandName = e.Parameter;
            RefGenericDrugDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, RefGenericDrugDetails.PageSize);
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
            RptParameters.FromDepartment = DepartmentReport.NHA_THUOC;
            RptParameters.StoreID = StoreID;
            RptParameters.BrandName = BrandName;
            RptParameters.IsDetail = IsDetail;

            RptParameters.Show = "TonKho_" + Globals.GetCurServerDateTime().Date.ToString("dd_MM_yyyy");

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
        }
    }
}
