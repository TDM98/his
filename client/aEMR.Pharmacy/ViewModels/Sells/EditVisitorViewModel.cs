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
using aEMR.CommonTasks;
using Castle.Windsor;
/*
 * 20191211 #001 TTML    Khi cập nhật phiếu sẽ lấy kho của phiếu cần cập nhật làm nguồn xuất thuốc chứ không lấy mặc định kho đầu tiên tìm thấy làm nguốn xuất thuốc.
 * 20200903 #002 TNHX: [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEditVisitor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditVisitorViewModel : Conductor<object>, IEditVisitor
        , IHandle<PharmacyCloseSearchVisitorEvent>
        , IHandle<EditChooseBatchNumberVisitorEvent>
        , IHandle<EditChooseBatchNumberVisitorResetQtyEvent>
    {
        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingAddTrans = false;
        public bool isLoadingAddTrans
        {
            get { return _isLoadingAddTrans; }
            set
            {
                if (_isLoadingAddTrans != value)
                {
                    _isLoadingAddTrans = value;
                    NotifyOfPropertyChange(() => isLoadingAddTrans);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }




        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingAddTrans || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            HamLuong = 4,
            DVT = 5,
            LoSX = 6,
            SoLuong = 7,
            DonGia = 8,
            ThanhTien = 9,
            HanDung = 10,
            ViTri = 11
        }
        [ImportingConstructor]
        public EditVisitorViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetStaffLogin();

            RefeshData();

            //20191211 TTM: Không set default mặc định mà phải load dữ liệu theo phiếu xuất cập nhật.
            //SetDefaultForStore();
        }

        private void RefeshData()
        {
            SelectedOutwardInfo = null;
            SelectedOutwardInfo = new OutwardDrugInvoice();
            SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            SelectedOutwardInfo.TypID = (long)AllLookupValues.RefOutputType.BANLE;
            SelectedOutwardInfo.OutwardDrugs = new ObservableCollection<OutwardDrug>();

            GetDrugForSellVisitorList = null;
            GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();

            GetDrugForSellVisitorListSum = null;
            GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();

            GetDrugForSellVisitorTemp = null;
            GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();

            ListOutwardDrugFirst = null;
            ListOutwardDrugFirst = new ObservableCollection<OutwardDrug>();

            ListOutwardDrugFirstCopy = null;
            ListOutwardDrugFirstCopy = new ObservableCollection<OutwardDrug>();

            BrandName = "";
        }

        #region Properties Member

        private SearchOutwardInfo _searchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private decimal _SumTotalPrice;
        public decimal SumTotalPrice
        {
            get { return _SumTotalPrice; }
            set
            {
                if (_SumTotalPrice != value)
                    _SumTotalPrice = value;
                NotifyOfPropertyChange(() => SumTotalPrice);
            }
        }

        public decimal _SumTotalPriceFirst;
        public decimal SumTotalPriceFirst
        {
            get { return _SumTotalPriceFirst; }
            set
            {
                if (_SumTotalPriceFirst != value)
                    _SumTotalPriceFirst = value;
                NotifyOfPropertyChange(() => SumTotalPriceFirst);
            }
        }

        public long _IDFirst;
        public long IDFirst
        {
            get { return _IDFirst; }
            set
            {
                if (_IDFirst != value)
                    _IDFirst = value;
                NotifyOfPropertyChange(() => IDFirst);
            }
        }

        private string BrandName;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitor;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList
        {
            get { return _GetDrugForSellVisitor; }
            set
            {
                if (_GetDrugForSellVisitor != value)
                    _GetDrugForSellVisitor = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorList);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorSum;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListSum
        {
            get { return _GetDrugForSellVisitorSum; }
            set
            {
                if (_GetDrugForSellVisitorSum != value)
                    _GetDrugForSellVisitorSum = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListSum);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorTemp;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp
        {
            get { return _GetDrugForSellVisitorTemp; }
            set
            {
                if (_GetDrugForSellVisitorTemp != value)
                    _GetDrugForSellVisitorTemp = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorTemp);
            }
        }

        private ObservableCollection<OutwardDrug> _ListOutwardDrugFirst;
        public ObservableCollection<OutwardDrug> ListOutwardDrugFirst
        {
            get { return _ListOutwardDrugFirst; }
            set
            {
                if (_ListOutwardDrugFirst != value)
                    _ListOutwardDrugFirst = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirst);
            }
        }

        private ObservableCollection<OutwardDrug> _ListOutwardDrugFirstCopy;
        public ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy
        {
            get { return _ListOutwardDrugFirstCopy; }
            set
            {
                if (_ListOutwardDrugFirstCopy != value)
                    _ListOutwardDrugFirstCopy = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirstCopy);
            }
        }

        private ObservableCollection<OutwardDrug> _OutwardDrugListCopy;
        public ObservableCollection<OutwardDrug> OutwardDrugListCopy
        {
            get { return _OutwardDrugListCopy; }
            set
            {
                if (_OutwardDrugListCopy != value)
                    _OutwardDrugListCopy = value;
                NotifyOfPropertyChange(() => OutwardDrugListCopy);
            }
        }


        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                {
                    _SelectedOutwardDrug = value;
                    SumTotalPriceOutward();
                }
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }

        private GetDrugForSellVisitor _SelectedSellVisitor;
        public GetDrugForSellVisitor SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
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

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private OutwardDrugInvoice _SelectedOutwardInfo;
        public OutwardDrugInvoice SelectedOutwardInfo
        {
            get
            {
                return _SelectedOutwardInfo;
            }
            set
            {
                if (_SelectedOutwardInfo != value)
                {
                    _SelectedOutwardInfo = value;
                    NotifyOfPropertyChange("SelectedOutwardInfo");
                }
            }
        }

        private OutwardDrugInvoice _SelectedOutwardInfoCoppy;
        public OutwardDrugInvoice SelectedOutwardInfoCoppy
        {
            get
            {
                return _SelectedOutwardInfoCoppy;
            }
            set
            {
                if (_SelectedOutwardInfoCoppy != value)
                {
                    _SelectedOutwardInfoCoppy = value;
                    NotifyOfPropertyChange("SelectedOutwardInfoCoppy");
                }
            }
        }


        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private bool _IsOther = false;
        public bool IsOther
        {
            get
            {
                return _IsOther;
            }
            set
            {
                _IsOther = value;
                NotifyOfPropertyChange(() => IsOther);
            }
        }
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;
        private bool _bTinhTien = true;
        private bool _bLuuTinhTien = true;
        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }
        public bool bTinhTien
        {
            get
            {
                return _bTinhTien;
            }
            set
            {
                if (_bTinhTien == value)
                    return;
                _bTinhTien = value;
            }
        }
        public bool bLuuTinhTien
        {
            get
            {
                return _bLuuTinhTien;
            }
            set
            {
                if (_bLuuTinhTien == value)
                    return;
                _bLuuTinhTien = value;
            }
        }
        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }


        #endregion

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }
        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new { hd.DrugID, hd.DrugCode, hd.BrandName, hd.UnitName } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          DrugID = hdgroup.Key.DrugID,
                          DrugCode = hdgroup.Key.DrugCode,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                item.DrugID = hhh.ToList()[i].DrugID;
                item.DrugCode = hhh.ToList()[i].DrugCode;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                GetDrugForSellVisitorListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (GetDrugForSellVisitorListSum != null && GetDrugForSellVisitorListSum.Count > 0)
                {
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode.ToUpper() == txt.ToUpper());
                    if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = item.ToList()[0];
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetDrugForSellVisitorListSum;
                    au.PopulateComplete();
                }
            }
        }

        private bool? IsCode = false;
        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_New(Name, StoreID, IsCode
                        //▼====== #002
                        , IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                        //▲======= #002
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_New(asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorListSum.Clear();
                            GetDrugForSellVisitorTemp.Clear();
                            foreach (GetDrugForSellVisitor s in results)
                            {
                                GetDrugForSellVisitorTemp.Add(s);
                            }
                            if (ListOutwardDrugFirstCopy != null && ListOutwardDrugFirstCopy.Count > 0)
                            {
                                foreach (OutwardDrug d in ListOutwardDrugFirstCopy)
                                {
                                    var value = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (GetDrugForSellVisitor s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    else
                                    {
                                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                                        p.Remaining = d.OutQuantity;
                                        p.RemainingFirst = d.OutQuantity;
                                        p.InBatchNumber = d.InBatchNumber;
                                        p.SellingPrice = d.OutPrice;
                                        p.InID = Convert.ToInt64(d.InID);
                                        p.STT = d.STT;
                                        GetDrugForSellVisitorTemp.Add(p);
                                        // d = null;
                                    }
                                }
                            }
                            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                            {
                                if (SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
                                {
                                    foreach (OutwardDrug d in SelectedOutwardInfo.OutwardDrugs)
                                    {
                                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                GetDrugForSellVisitorList.Add(s);
                            }
                            ListDisplayAutoComplete();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //TBL: Khi tim thuoc = ten do focus nen khong the nhap nhieu ky tu duoc
                            //if (au != null)
                            //{
                            //    au.Focus();
                            //}
                        }
                    }), null);
                }
            });

            t.Start();
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetDrugForSellVisitor(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }

        }

        public void SumTotalPriceOutward()
        {
            if (!Compare2Object() || !Compare2ObjectInvoice())
            {
                IsOther = true;
            }
            else
            {
                IsOther = false;
            }
            if (SelectedOutwardInfo != null)
            {
                SumTotalPrice = 0;
                SelectedOutwardInfo.TotalInvoicePrice = 0;
                if (SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count != 0)
                {
                    foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                    {
                        SumTotalPrice = SumTotalPrice + p.TotalPrice;
                        SelectedOutwardInfo.TotalInvoicePrice += p.TotalPrice;
                    }
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            if (MessageBox.Show(eHCMSResources.Z0892_CoMuonXoaThuocKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedOutwardInfo.OutwardDrugs.Remove(SelectedOutwardDrug);
                SelectedOutwardInfo.OutwardDrugs = SelectedOutwardInfo.OutwardDrugs.ToObservableCollection();
                SumTotalPriceOutward();
                ListDisplayAutoComplete();
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteInvoiceDrugInObject();
        }

        DataGrid GridInward;
        public void GridInward_Loaded(object sender, RoutedEventArgs e)
        {
            GridInward = sender as DataGrid;
        }
        public void GridInward_Unloaded(object sender, RoutedEventArgs e)
        {
            GridInward.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        int Qty = 0;
        public void GridInward_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.SoLuong)
            {
                Qty = (e.Row.DataContext as OutwardDrug).OutQuantity.DeepCopy();
                SumTotalPriceOutward();
            }
        }

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
            {
                foreach (OutwardDrug p1 in SelectedOutwardInfo.OutwardDrugs)
                {
                    if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        kq = true;
                        break;
                    }
                }
            }
            else
            {
                SelectedOutwardInfo.OutwardDrugs = new ObservableCollection<OutwardDrug>();
            }
            if (!kq)
            {
                SelectedOutwardInfo.OutwardDrugs.Add(p);
            }
            SelectedSellVisitor = null;
            if (IsCode.GetValueOrDefault())
            {
                if (tbx != null)
                {
                    tbx.Text = "";
                    tbx.Focus();
                }

            }
            else
            {
                if (au != null)
                {
                    au.Text = "";
                    au.Focus();
                }
            }
        }

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();
                if (item.Remaining > 0)
                {
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        value.RequiredNumber = value.RequiredNumber - item.Remaining;
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.OutPrice = item.OutPrice;
                        p.OutQuantity = item.Remaining;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPriceOutward();
        }

        private void AddListOutwardDrug(GetDrugForSellVisitor value)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            try
            {
                if (value != null)
                {
                    if (value.RequiredNumber > 0)
                    {
                        int intOutput = 0;
                        if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
                        {
                            int a = Convert.ToInt32(value.RequiredNumber);
                            if (CheckValidDrugAuto(value))
                            {
                                ChooseBatchNumber(value);
                                // SearchGetDrugForSellVisitor(BrandName);
                                //if (au != null)
                                //{
                                //    au.Text = "";
                                //    au.Focus();
                                //}
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0972_G1_Msg_InfoSLgKhHopLe);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
                }
            }
            catch
            {
                MessageBox.Show(eHCMSResources.T0074_G1_I);
            }
            finally
            {
                isLoadingFullOperator = false;
                //Globals.IsBusy = false;
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            AddListOutwardDrug(SelectedSellVisitor);
        }

        //▼===== #001
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            //SetDefaultForStore();
            if (paymentTypeTask != null && paymentTypeTask.LookupList != null && paymentTypeTask.LookupList.Where(x => x.IsSubStorage).Count() > 0)
            {
                StoreCbx = paymentTypeTask.LookupList.OrderByDescending(x => x.IsSubStorage).ToObservableCollection();
            }
            yield break;
        }

        public void SetDefaultForStore()
        {
            //if (StoreCbx != null)
            //{
            //    StoreID = StoreCbx.FirstOrDefault().StoreID;
            //}
            if (SelectedOutwardInfo != null)
            {
                //StoreID = StoreCbx.FirstOrDefault().StoreID;
                StoreID = (long)SelectedOutwardInfo.StoreID;
            }
        }
        //▲===== #001
        private bool CheckValid(OutwardDrugInvoice temp)
        {
            if (temp == null)
            {
                return false;
            }
            return temp.Validate();
        }

        private bool CheckValidInvoice(OutwardDrugInvoice temp)
        {
            if (temp == null)
            {
                return false;
            }

            int intOutput = 0;

            //KMx: Nếu có năm sinh thì mới kiểm tra, không thì thôi (09/04/2014 11:37).
            if (temp.DOBString != null && temp.DOBString.Length > 0 && (temp.DOBString.Length != 4 || !Int32.TryParse(temp.DOBString, out intOutput) || Convert.ToInt16(temp.DOBString) < 1900))
            {
                MessageBox.Show(eHCMSResources.A0822_G1_Msg_InfoNSinhKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        private void UpdateInvoiceInfo(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateInvoiceInfo(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool value = contract.EndUpdateInvoiceInfo(asyncResult);
                            Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = SelectedOutwardInfo });
                            TryClose();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SaveDrugs(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginOutwardDrugInvoice_SaveByType_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            //bool value = contract.EndOutwardDrugInvoice_SaveByType(out OutID, out StrError, asyncResult);
                            bool value = contract.EndOutwardDrugInvoice_SaveByType_Pst(out OutID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                OutwardInvoice.outiID = OutID;
                                //phat su kien de form o duoi load lai du lieu 
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = OutwardInvoice });
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateInvoicePayed(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingAddTrans = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginUpdateInvoicePayed(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginUpdateInvoicePayed_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            long PaymemtID = 0;
                            string StrError = "";
                            //bool value = contract.EndUpdateInvoicePayed(out OutID, out PaymemtID, out StrError, asyncResult);
                            bool value = contract.EndUpdateInvoicePayed_Pst(out OutID, out PaymemtID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                SelectedOutwardInfo.outiID = OutID;
                                //phat su kien de form o duoi load lai du lieu 
                                Globals.EventAggregator.Publish(new PharmacyCloseEditPayed { SelectedOutwardInvoice = SelectedOutwardInfo });
                                TryClose();
                                //  CountMoneyForVisitorPharmacy(OutID);
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingAddTrans = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSaveMoney()
        {
            if (!this.CheckValid(SelectedOutwardInfo))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0112_G1_Msg_InfoTTPhBanLeKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutwardInfo.OutwardDrugs == null || SelectedOutwardInfo.OutwardDrugs.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0639_G1_Msg_InfoKhCoCTietPhBanLe);
                return;
            }

            if (!CheckValidInvoice(SelectedOutwardInfo))
            {
                return;
            }

            if (Compare2Object())
            {
                //goi ham cap nhat hoa don
                UpdateInvoiceInfo(SelectedOutwardInfo);
                return;
            }

            string strError = "";
            for (int i = 0; i < SelectedOutwardInfo.OutwardDrugs.Count; i++)
            {
                if (SelectedOutwardInfo.OutwardDrugs[i].OutPrice <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0);
                    return;
                }
                //neu ngay het han lon hon ngay hien tai
                if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, SelectedOutwardInfo.OutwardDrugs[i].InExpiryDate) == 1)
                {
                    strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutwardInfo.OutwardDrugs[i].GetDrugForSellVisitor.BrandName, (i + 1).ToString()) + Environment.NewLine;
                }
            }
            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            if (GridInward == null || !this.CheckValid(SelectedOutwardInfo))
            {
                MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
                return;
            }

            SelectedOutwardInfo.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
            if (SelectedOutwardInfo.PaidTime == null)
            {
                SaveDrugs(SelectedOutwardInfo);
            }
            else
            {
                UpdateInvoicePayed(SelectedOutwardInfo);
            }
        }

        private void LoadOutwardDrugInvoiceByID(long OutwardID)
        {
            isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutwardInfo = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            if (SelectedOutwardInfo != null)
                            {
                                GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetOutwardDrugDetailsByOutwardInvoice(long OutwardID)
        {
            isLoadingDetail = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                            SelectedOutwardInfo.OutwardDrugs = results.ToObservableCollection();
                            ListOutwardDrugFirst = results.ToObservableCollection();
                            DeepCopyOutwardDrug();
                            SumTotalPriceOutward();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void DeepCopyOutwardDrug()
        {
            if (SelectedOutwardInfo.OutwardDrugs != null)
            {
                OutwardDrugListCopy = SelectedOutwardInfo.OutwardDrugs.DeepCopy();
            }
            else
            {
                OutwardDrugListCopy = null;
            }
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfoCoppy = SelectedOutwardInfo.DeepCopy();
            }
            if (ListOutwardDrugFirst != null)
            {
                ListOutwardDrugFirstCopy = ListOutwardDrugFirst.DeepCopy();
            }
            else
            {
                ListOutwardDrugFirstCopy = null;
            }
        }

        #region IHandle<PharmacyCloseSearchVisitorEvent> Members

        public void Handle(PharmacyCloseSearchVisitorEvent message)
        {
            if (message != null)
            {
                SelectedOutwardInfo = message.SelectedOutwardInfo as OutwardDrugInvoice;
                GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
            }
        }

        #endregion

        private void InitializeOutwardDrugInvoice()
        {
            SelectedOutwardInfo = new OutwardDrugInvoice();
            SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            SelectedOutwardInfo.SelectedStorage = new RefStorageWarehouseLocation();
            SelectedOutwardInfo.SelectedStorage.StoreID = StoreID;
            SelectedOutwardInfo.SelectedStaff = GetStaffLogin();
            if (SelectedOutwardInfo.OutwardDrugs != null)
            {
                SelectedOutwardInfo.OutwardDrugs.Clear();
            }
            ListOutwardDrugFirst.Clear();
            GetDrugForSellVisitorList.Clear();
            GetDrugForSellVisitorListSum.Clear();
            GetDrugForSellVisitorTemp.Clear();
            ListOutwardDrugFirstCopy.Clear();
            if (au != null)
            {
                au.Text = "";
            }
        }

        private bool Equal(OutwardDrug a, OutwardDrug b)
        {
            if (a.InID == b.InID && a.DrugID == b.DrugID && a.InBatchNumber == b.InBatchNumber && a.InExpiryDate == b.InExpiryDate && a.OutPrice == b.OutPrice && a.OutQuantity == b.OutQuantity)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Compare2ObjectInvoice()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfoCoppy != null)
            {
                if (SelectedOutwardInfo.OutDate != SelectedOutwardInfoCoppy.OutDate)
                {
                    return false;
                }
                if (SelectedOutwardInfo.FullName == SelectedOutwardInfoCoppy.FullName && SelectedOutwardInfo.NumberPhone == SelectedOutwardInfoCoppy.NumberPhone && SelectedOutwardInfo.Address == SelectedOutwardInfoCoppy.Address)
                {
                    return true;
                }
            }
            return false;
        }

        private bool Compare2Object()
        {
            if (SelectedOutwardInfo.OutwardDrugs != null && OutwardDrugListCopy != null && SelectedOutwardInfo.OutwardDrugs.Count == OutwardDrugListCopy.Count && SelectedOutwardInfo.OutwardDrugs.Count > 0)
            {

                int icount = 0;
                for (int i = 0; i < OutwardDrugListCopy.Count; i++)
                {
                    for (int j = 0; j < SelectedOutwardInfo.OutwardDrugs.Count; j++)
                    {
                        if (Equal(OutwardDrugListCopy[i], SelectedOutwardInfo.OutwardDrugs[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == OutwardDrugListCopy.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        #region Properties member

        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForSellVisitorBatchNumber(DrugID, StoreID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrug != null)
            {
                Button lnkBatchNumber = sender as Button;
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutwardInfo.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                OutwardDrugListByDrugIDFirst = ListOutwardDrugFirstCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                GetDrugForSellVisitorBatchNumber(DrugID);
            }
        }

        public void btnCancel()
        {
            TryClose();
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugListByDrugIDFirst != null)
            {
                foreach (OutwardDrug d in OutwardDrugListByDrugIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetDrugForSellVisitor s in BatchNumberListTemp)
            {
                if (OutwardDrugListByDrugID.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugListByDrugID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberVisitor> onInitDlg = delegate (IChooseBatchNumberVisitor proAlloc)
                {
                    proAlloc.FormType = 2;
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByDrugID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac);
            }
        }

        private int count = 0;//ban dau moi load len thi ko cho vao ham changed
        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (count == 1)
            {
                ComboBox cbx = sender as ComboBox;
                if (cbx.SelectedItem != null)
                {
                    RefeshData();
                }
            }
            count = 1;
        }

        #region IHandle<EditChooseBatchNumberVisitorEvent> Members

        public void Handle(EditChooseBatchNumberVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPriceOutward();
            }
        }

        #endregion

        #region IHandle<EditChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(EditChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPriceOutward();
            }
        }

        #endregion

        public void AxTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfo.FullName = (sender as TextBox).Text;
                SumTotalPriceOutward();
            }
        }

        public void AxTextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfo.NumberPhone = (sender as TextBox).Text;
                SumTotalPriceOutward();
            }
        }

        public void AxTextBox_KeyUp_2(object sender, KeyEventArgs e)
        {
            if (SelectedOutwardInfo != null)
            {
                SelectedOutwardInfo.Address = (sender as TextBox).Text;
                SumTotalPriceOutward();
            }
        }
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForSellVisitor(txt, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // au.IsEnabled = false;
                string text = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(text))
                {
                    SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
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
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }

        #region Checked All Member
        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private void AllCheckedfc()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutwardInfo.OutwardDrugs.Count; i++)
                {
                    SelectedOutwardInfo.OutwardDrugs[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutwardInfo.OutwardDrugs.Count; i++)
                {
                    SelectedOutwardInfo.OutwardDrugs[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
            {
                var items = SelectedOutwardInfo.OutwardDrugs.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        SelectedOutwardInfo.OutwardDrugs = SelectedOutwardInfo.OutwardDrugs.Where(x => x.Checked == false).ToObservableCollection();
                        SumTotalPriceOutward();
                        ListDisplayAutoComplete();
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }
        #endregion

        //▼====== #002
        private bool _isSearchByGenericName = false;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }

        private bool _visSearchByGenericName = Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName;
        public bool vIsSearchByGenericName
        {
            get { return _visSearchByGenericName; }
            set
            {
                if (_visSearchByGenericName != value)
                {
                    _visSearchByGenericName = value;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                }
            }
        }

        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.PharmacyElements.PharmacySearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        //▲======= #002
    }
}
