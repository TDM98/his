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
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;
using System.Windows.Data;
using aEMR.Common.ConfigurationManager.Printer;
using System.IO;
/*
 * 20190424 #001 TNHX:   [BM0006716] [BM0006777] Create PhieuNhanThuoc, PhieuNhanThuocBHYT, PhieuNhanThuocSummary for Thermal, Apply InNhiet When Save Prescription at Screen "BanThuocLe"
 * 20190509 #002 TTM:    BM 0006846: Bổ sung thêm trường giá vốn cho màn hình bán lẻ
 * 20191211 #003 TTML    Khi cập nhật phiếu sẽ lấy kho của phiếu cần cập nhật làm nguồn xuất thuốc chứ không lấy mặc định kho đầu tiên tìm thấy làm nguốn xuất thuốc.
 * 20200903 #004 TNHX [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 * 20220217 #005 QTD:    Bỏ kho BHYT trong màn hình bán thuốc lẻ
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IVisitor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class VisitorViewModel : Conductor<object>, IVisitor
        , IHandle<PharmacyCloseSearchVisitorEvent>, IHandle<ChooseBatchNumberVisitorEvent>, IHandle<ChooseBatchNumberVisitorResetQtyEvent>
        , IHandle<PharmacyPayEvent>, IHandle<PharmacyCloseEditPayed>
        , IHandle<PharmacyCloseFormReturnEvent>
    {
        #region Indicator Member

        // TxD 04/09/2014 : Get rid of the following flags for the OLD busy indicator binding , use this.ShowBusyIndicator instead
        //                  isLoadingGetStore || isLoadingFullOperator || isLoadingCount || isLoadingAddTrans || isLoadingGetID || isLoadingSearch || isLoadingDetail
        public bool IsLoading
        {
            get
            {
                return false;
                //return (isLoadingGetStore || isLoadingFullOperator || isLoadingCount || isLoadingAddTrans || isLoadingGetID || isLoadingSearch || isLoadingDetail);
            }
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
        public VisitorViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetStaffLogin();

            RefeshData();
            SetDefaultForStore();
            GetDrugForSellVisitorDisplays = new PagedSortableCollectionView<GetDrugForSellVisitor>();
            GetDrugForSellVisitorDisplays.PageSize = Globals.PageSize;
            GetDrugForSellVisitorDisplays.OnRefresh += new EventHandler<RefreshEventArgs>(GetDrugForSellVisitorDisplays_OnRefresh);

            LoadGenderCollection();
        }

        void GetDrugForSellVisitorDisplays_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchGetDrugForSellVisitor(BrandName, IsCode, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
        }

        private void RefeshData()
        {
            SelectedOutwardInfo = new OutwardDrugInvoice();
            SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            SelectedOutwardInfo.TypID = (long)AllLookupValues.RefOutputType.BANLE;
            SelectedOutwardInfo.OutwardDrugs = new ObservableCollection<OutwardDrug>();
            SelectedOutwardInfo.MainICD10 = new DiseasesReference();

            SearchCriteria = null;
            SearchCriteria = new SearchOutwardInfo();
            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.BANLE;

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
            HideShowColumnDelete();
        }

        #region Properties Member
        public string TitleForm { get; set; }

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

        private ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy;

        private ObservableCollection<OutwardDrug> OutwardDrugListCopy;

        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
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
                    if (_SelectedOutwardInfo != null)
                    {
                        strOutDrugInvStatus = _SelectedOutwardInfo.OutDrugInvStatus;
                    }
                    CheckPermissionForOutward();
                    NotifyOfPropertyChange(() => SelectedOutwardInfo);
                    NotifyOfPropertyChange(() => strOutDrugInvStatus);
                    NotifyOfPropertyChange(() => VisibilityName);
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        private OutwardDrugInvoice SelectedOutwardInfoCoppy;

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

        private PagedSortableCollectionView<GetDrugForSellVisitor> _GetDrugForSellVisitorDisplays;
        public PagedSortableCollectionView<GetDrugForSellVisitor> GetDrugForSellVisitorDisplays
        {
            get
            {
                return _GetDrugForSellVisitorDisplays;
            }
            set
            {
                if (_GetDrugForSellVisitorDisplays != value)
                {
                    _GetDrugForSellVisitorDisplays = value;
                    NotifyOfPropertyChange(() => GetDrugForSellVisitorDisplays);
                }

            }
        }

        private bool _IsPrescriptionCollect = false;
        public bool IsPrescriptionCollect
        {
            get
            {
                return _IsPrescriptionCollect;
            }
            set
            {
                _IsPrescriptionCollect = value;
                NotifyOfPropertyChange(() => IsPrescriptionCollect);
            }
        }

        private bool gICD10IsDropDown = false;
        #endregion

        public void authorization()
        {
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
            {
                mBanThuocLe_ThermalReportIn = true;
            }
            if (!Globals.isAccountCheck)
            {
                return;
            }

            //dinh sua lai phan nay
            mBanThuocLe_CapNhatPhieu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_CapNhatPhieu, (int)ePermission.mView);
            mBanThuocLe_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_Tim, (int)ePermission.mView)
                                               || mBanThuocLe_CapNhatPhieu;
            mBanThuocLe_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_ThongTin, (int)ePermission.mView)
                                               || mBanThuocLe_CapNhatPhieu; ;
            mBanThuocLe_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_PhieuMoi, (int)ePermission.mView);
            mBanThuocLe_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_ThuTien, (int)ePermission.mView);
            mBanThuocLe_HuyPhieu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_HuyPhieu, (int)ePermission.mView);

            mBanThuocLe_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_ReportIn, (int)ePermission.mView);
            mBanThuocLe_CapNhatSauBaoCao = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocLe,
                                               (int)oPharmacyEx.mBanThuocLe_CapNhatSauBaoCao, (int)ePermission.mView);
        }

        #region checking account

        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;
        //private bool _bTinhTien = true;
        //private bool _bLuuTinhTien = true;


        private bool _mBanThuocLe_Tim = true;
        private bool _mBanThuocLe_ThongTin = true;
        private bool _mBanThuocLe_PhieuMoi = true;
        private bool _mBanThuocLe_ThuTien = true;
        private bool _mBanThuocLe_HuyPhieu = true;
        private bool _mBanThuocLe_CapNhatPhieu = true;
        private bool _mBanThuocLe_ReportIn = true;
        private bool _mBanThuocLe_CapNhatSauBaoCao = true;
        private bool _mBanThuocLe_ThermalReportIn = false;

        public bool mBanThuocLe_Tim
        {
            get
            {
                return _mBanThuocLe_Tim;
            }
            set
            {
                if (_mBanThuocLe_Tim == value)
                    return;
                _mBanThuocLe_Tim = value;
            }
        }
        public bool mBanThuocLe_ThongTin
        {
            get
            {
                return _mBanThuocLe_ThongTin;
            }
            set
            {
                if (_mBanThuocLe_ThongTin == value)
                    return;
                _mBanThuocLe_ThongTin = value;
            }
        }
        public bool mBanThuocLe_PhieuMoi
        {
            get
            {
                return _mBanThuocLe_PhieuMoi;
            }
            set
            {
                if (_mBanThuocLe_PhieuMoi == value)
                    return;
                _mBanThuocLe_PhieuMoi = value;
            }
        }
        public bool mBanThuocLe_ThuTien
        {
            get
            {
                return _mBanThuocLe_ThuTien;
            }
            set
            {
                if (_mBanThuocLe_ThuTien == value)
                    return;
                _mBanThuocLe_ThuTien = value;
            }
        }
        public bool mBanThuocLe_HuyPhieu
        {
            get
            {
                return _mBanThuocLe_HuyPhieu;
            }
            set
            {
                if (_mBanThuocLe_HuyPhieu == value)
                    return;
                _mBanThuocLe_HuyPhieu = value;
            }
        }
        public bool mBanThuocLe_CapNhatPhieu
        {
            get
            {
                return _mBanThuocLe_CapNhatPhieu;
            }
            set
            {
                if (_mBanThuocLe_CapNhatPhieu == value)
                    return;
                _mBanThuocLe_CapNhatPhieu = value;
            }
        }
        public bool mBanThuocLe_ReportIn
        {
            get
            {
                return _mBanThuocLe_ReportIn;
            }
            set
            {
                if (_mBanThuocLe_ReportIn == value)
                    return;
                _mBanThuocLe_ReportIn = value;
            }
        }

        public bool mBanThuocLe_CapNhatSauBaoCao
        {
            get
            {
                return _mBanThuocLe_CapNhatSauBaoCao;
            }
            set
            {
                if (_mBanThuocLe_CapNhatSauBaoCao == value)
                    return;
                _mBanThuocLe_CapNhatSauBaoCao = value;
            }
        }

        public bool mBanThuocLe_ThermalReportIn
        {
            get
            {
                return _mBanThuocLe_ThermalReportIn;
            }
            set
            {
                if (_mBanThuocLe_ThermalReportIn == value)
                    return;
                _mBanThuocLe_ThermalReportIn = value;
            }
        }
        //public bool bEdit
        //{
        //    get
        //    {
        //        return _bEdit;
        //    }
        //    set
        //    {
        //        if (_bEdit == value)
        //            return;
        //        _bEdit = value;
        //    }
        //}
        //public bool bAdd
        //{
        //    get
        //    {
        //        return _bAdd;
        //    }
        //    set
        //    {
        //        if (_bAdd == value)
        //            return;
        //        _bAdd = value;
        //    }
        //}
        //public bool bDelete
        //{
        //    get
        //    {
        //        return _bDelete;
        //    }
        //    set
        //    {
        //        if (_bDelete == value)
        //            return;
        //        _bDelete = value;
        //    }
        //}
        //public bool bView
        //{
        //    get
        //    {
        //        return _bView;
        //    }
        //    set
        //    {
        //        if (_bView == value)
        //            return;
        //        _bView = value;
        //    }
        //}
        //public bool bPrint
        //{
        //    get
        //    {
        //        return _bPrint;
        //    }
        //    set
        //    {
        //        if (_bPrint == value)
        //            return;
        //        _bPrint = value;
        //    }
        //}
        //public bool bTinhTien
        //{
        //    get
        //    {
        //        return _bTinhTien;
        //    }
        //    set
        //    {
        //        if (_bTinhTien == value)
        //            return;
        //        _bTinhTien = value;
        //    }
        //}
        //public bool bLuuTinhTien
        //{
        //    get
        //    {
        //        return _bLuuTinhTien;
        //    }
        //    set
        //    {
        //        if (_bLuuTinhTien == value)
        //            return;
        //        _bLuuTinhTien = value;
        //    }
        //}
        //public bool bReport
        //{
        //    get
        //    {
        //        return _bReport;
        //    }
        //    set
        //    {
        //        if (_bReport == value)
        //            return;
        //        _bReport = value;
        //    }
        //}
        #endregion

        #region binding visibilty

        //public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as Button;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
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
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode == txt);
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

        AxTextBox SearchTextBox;
        public void SearchTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchTextBox = (AxTextBox)sender;
            SearchTextBox.Focus();
        }

        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode, int PageSize, int PageIndex)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //▼====== #004
                    contract.BeginRefGennericDrugDetails_GetRemaining_Paging(Name, StoreID, IsCode, PageSize, PageIndex
                        , IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                    {
                        //▲======= #004
                        try
                        {
                            int Total = 0;
                            var results = contract.EndRefGennericDrugDetails_GetRemaining_Paging(out Total, asyncResult);
                            GetDrugForSellVisitorTemp = results.ToObservableCollection().DeepCopy();
                            GetDrugForSellVisitorListSum.Clear();

                            //danh sach thuoc da xuat (da luu vao database) dua vao GetDrugForSellVisitorTemp
                            if (ListOutwardDrugFirstCopy != null && ListOutwardDrugFirstCopy.Count > 0)
                            {
                                var ListOutwardDrugFirstCopyGroup = from hd in ListOutwardDrugFirstCopy
                                                                    group hd by new { hd.DrugID, hd.GetDrugForSellVisitor.BrandName, hd.GetDrugForSellVisitor.DrugCode, hd.GetDrugForSellVisitor.UnitName } into hdgroup
                                                                    select new
                                                                    {
                                                                        OutQuantityOld = hdgroup.Sum(groupItem => groupItem.OutQuantityOld),
                                                                        DrugID = hdgroup.Key.DrugID,
                                                                        BrandName = hdgroup.Key.BrandName,
                                                                        DrugCode = hdgroup.Key.DrugCode,
                                                                        UnitName = hdgroup.Key.UnitName
                                                                    };

                                foreach (var d in ListOutwardDrugFirstCopyGroup)
                                {
                                    var value = results.Where(x => x.DrugID == d.DrugID);
                                    if (value.Count() > 0)
                                    {
                                        //neu co trong danh sach thuoc da xuat thi + so luong vao so SL ton
                                        foreach (GetDrugForSellVisitor s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    //vi lay toan bo danh muc nen se khong con truong hop nay nua
                                    //else
                                    //{
                                    //    //neu trog ds da xuat co,ma load len ko co thi add vao
                                    //    GetDrugForSellVisitor p = new GetDrugForSellVisitor();
                                    //    p.DrugID = d.DrugID.GetValueOrDefault();
                                    //    p.DrugCode = d.DrugCode;
                                    //    p.BrandName=d.BrandName;
                                    //    p.UnitName = d.UnitName;
                                    //    p.Remaining = d.OutQuantityOld;
                                    //    p.RemainingFirst = d.OutQuantityOld;
                                    //    GetDrugForSellVisitorTemp.Add(p);
                                    //    // d = null;
                                    //}
                                }
                            }

                            //sau do tru so luong hien co tren luoi,de co ds sau cung
                            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                            {
                                if (SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
                                {
                                    var ListOutwardDrugFirstCopyGroup = from hd in SelectedOutwardInfo.OutwardDrugs
                                                                        group hd by new { hd.DrugID, hd.GetDrugForSellVisitor.BrandName, hd.GetDrugForSellVisitor.DrugCode, hd.GetDrugForSellVisitor.UnitName } into hdgroup
                                                                        select new
                                                                        {
                                                                            OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                                                                            DrugID = hdgroup.Key.DrugID,
                                                                            BrandName = hdgroup.Key.BrandName,
                                                                            DrugCode = hdgroup.Key.DrugCode,
                                                                            UnitName = hdgroup.Key.UnitName
                                                                        };
                                    foreach (var d in ListOutwardDrugFirstCopyGroup)
                                    {
                                        if (d.DrugID == s.DrugID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                GetDrugForSellVisitorListSum.Add(s);
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
                                    GetDrugForSellVisitorDisplays.Clear();
                                    GetDrugForSellVisitorDisplays.TotalItemCount = Total;
                                    foreach (GetDrugForSellVisitor p in GetDrugForSellVisitorListSum)
                                    {
                                        GetDrugForSellVisitorDisplays.Add(p);
                                    }

                                    //GetDrugForSellVisitorDisplays = new PagedSortableCollectionView<GetDrugForSellVisitor>(GetDrugForSellVisitorListSum);
                                    au.ItemsSource = GetDrugForSellVisitorDisplays;// GetDrugForSellVisitorListSum;
                                    au.PopulateComplete();
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
                                if (AxQty != null)
                                {
                                    AxQty.Focus();
                                }
                            }
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
            if (!IsCode.GetValueOrDefault() && e.Parameter != null)
            {
                BrandName = e.Parameter;
                //tim theo ten
                GetDrugForSellVisitorDisplays.PageIndex = 0;
                SearchGetDrugForSellVisitor(e.Parameter, false, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }
        private decimal _SumTotalPrice = 0;
        public decimal SumTotalPrice
        {
            get { return _SumTotalPrice; }
            set
            {
                _SumTotalPrice = value;
                NotifyOfPropertyChange(() => SumTotalPrice);
            }
        }
        private void SumTotalPriceOutward()
        {
            SelectedOutwardInfo.TotalInvoicePrice = 0;
            SumTotalPrice = 0;
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count != 0)
            {
                foreach (OutwardDrug p in SelectedOutwardInfo.OutwardDrugs)
                {
                    SelectedOutwardInfo.TotalInvoicePrice = SelectedOutwardInfo.TotalInvoicePrice + p.TotalPrice;
                    SumTotalPrice += p.TotalPrice;
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            SelectedOutwardInfo.OutwardDrugs.Remove(SelectedOutwardDrug);
            SelectedOutwardInfo.OutwardDrugs = SelectedOutwardInfo.OutwardDrugs.ToObservableCollection();
            SumTotalPriceOutward();
            // ListDisplayAutoComplete();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrug != null && SelectedOutwardInfo.CanSaveAndPaid)
            {
                //if (MessageBox.Show("Bạn có muốn xóa thuốc này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                DeleteInvoiceDrugInObject();
                // }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0904_G1_PhDaThuTienHoacKC, eHCMSResources.G0442_G1_TBao);
            }
        }



        DataGrid GridInward;
        public void GridInward_Loaded(object sender, RoutedEventArgs e)
        {
            GridInward = sender as DataGrid;
            InitColumns();
        }

        public void GridInward_Unloaded(object sender, RoutedEventArgs e)
        {
            GridInward.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void GridInward_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
            if (SelectedOutwardInfo != null && SelectedOutwardInfo.outiID == 0)
            {
                Button colBatchNumber = GridInward.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = GridInward.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        int Qty = 0;
        public void GridInward_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.SoLuong)
            {
                // TxD 04/09/2014 No Need to DeepCopy here
                //Qty = (e.Row.DataContext as OutwardDrug).OutQuantity.DeepCopy();
                Qty = (e.Row.DataContext as OutwardDrug).OutQuantity;
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
            txt = "";
            SelectedSellVisitor = null;
        }

        private void ChooseBatchNumber_OLD(GetDrugForSellVisitor value)
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
                        //▼====== #002
                        p.InCost = item.InCost;
                        //▲====== #002
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
                        //▼====== #002
                        p.InCost = item.InCost;
                        //▲====== #002
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPriceOutward();
        }

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            //lay so luong theo lo

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugForSellVisitorBatchNumber(value.DrugID, StoreID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber(asyncResult);
                            GetDrugForSellVisitorTemp = results.ToObservableCollection().DeepCopy();
                            GetDrugForSellVisitorList.Clear();

                            //danh sach thuoc da xuat (da luu vao database) dua vao GetDrugForSellVisitorTemp
                            if (ListOutwardDrugFirstCopy != null && ListOutwardDrugFirstCopy.Count > 0)
                            {
                                var itemListOutwardDrug = ListOutwardDrugFirstCopy.Where(x => x.DrugID == value.DrugID);
                                foreach (OutwardDrug d in itemListOutwardDrug)
                                {
                                    var value1 = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value1.Count() > 0)
                                    {
                                        //neu co trong danh sach thuoc da xuat thi + so luong vao so SL ton
                                        foreach (GetDrugForSellVisitor s in value1.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    else
                                    {
                                        //neu trog ds da xuat co,ma load len ko co thi add vao
                                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                                        p.Remaining = d.OutQuantity;
                                        p.RemainingFirst = d.OutQuantity;
                                        p.InBatchNumber = d.InBatchNumber;
                                        p.SellingPrice = d.OutPrice;
                                        p.InID = Convert.ToInt64(d.InID);
                                        p.STT = d.STT;
                                        p.VAT = d.VAT;
                                        GetDrugForSellVisitorTemp.Add(p);
                                        // d = null;
                                    }
                                }
                            }

                            //sau do tru so luong hien co tren luoi,de co ds sau cung
                            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                            {
                                if (SelectedOutwardInfo.OutwardDrugs != null && SelectedOutwardInfo.OutwardDrugs.Count > 0)
                                {
                                    var outTemp = SelectedOutwardInfo.OutwardDrugs.Where(x => x.DrugID == value.DrugID);
                                    foreach (OutwardDrug d in outTemp)
                                    {
                                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                GetDrugForSellVisitorList.Add(s);
                            }


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
                                        //▼====== #002
                                        p.InCost = item.InCost;
                                        //▲====== #002
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
                                        //▼====== #002
                                        p.InCost = item.InCost;
                                        //▲====== #002
                                        p.VAT = item.VAT;
                                        CheckBatchNumberExists(p);
                                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                                        break;
                                    }
                                }
                            }
                            SumTotalPriceOutward();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();

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
                                    au.Text = null;
                                    // au.Text = "";
                                    au.Focus();
                                }
                            }

                        }

                    }), null);

                }

            });

            t.Start();
        }
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        private void AddListOutwardDrug(GetDrugForSellVisitor value)
        {
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
                                //SearchGetDrugForSellVisitor(BrandName);
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0972_G1_Msg_InfoSLgKhHopLe);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0972_G1_Msg_InfoSLgKhHopLe);
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
                au.Focus();
            }
        }
        public void AddItem_Click(object sender, object e)
        {
            AddListOutwardDrug(SelectedSellVisitor);
        }
        public void AddItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (RootAxGrid != null)
                {
                    RootAxGrid.DisableFirstNextFocus = true;
                }
            }
        }
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;

            //▼===== 20191210 TTM: Sửa lại code của anh Thắng, vì bệnh viện Thanh Vũ mới có sử dụng SubStorage nên cần phải làm động để bệnh viện nào cũng xài đc.
            // 20191207 TNHX Dựa trên cấu hình Kho giữa của Khoa duoc app cho nhà thuốc (TV dung chung cấu hình)
            //if (Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
            //{
            //    StoreCbx = paymentTypeTask.LookupList.Where(x => x.IsSubStorage).ToObservableCollection();
            //}
            //else
            //{
            //    StoreCbx = paymentTypeTask.LookupList;
            //}

            if (paymentTypeTask != null && paymentTypeTask.LookupList != null && paymentTypeTask.LookupList.Where(x => x.IsSubStorage).Count() > 0)
            {
                StoreCbx = paymentTypeTask.LookupList.OrderByDescending(x => x.IsSubStorage).ToObservableCollection();
            }
            else
            {
                StoreCbx = paymentTypeTask.LookupList;
            }
            //▲===== 

            if (!IsHIOutPt)
            {
                StoreCbx.Remove(StoreCbx.FirstOrDefault(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_HIDRUGs));
            }

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

        private bool CheckValid(OutwardDrugInvoice temp)
        {
            if (temp == null)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0112_G1_Msg_InfoTTPhBanLeKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (!temp.Validate())
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0112_G1_Msg_InfoTTPhBanLeKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (IsPrescriptionCollect)
            {
                if (temp.OutwardDrugs.Any(x => x.DrugID > 0 && string.IsNullOrEmpty(x.DoseString)))
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1069_G1_LieuDungKgHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (temp.MainICD10 == null || string.IsNullOrEmpty(temp.MainICD10.ICD10Code))
                {
                    MessageBox.Show(eHCMSResources.Z2623_G1_ChuaChonICD10, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (string.IsNullOrEmpty(temp.DOBString))
                {
                    MessageBox.Show(eHCMSResources.A0854_G1_Msg_InfoNgSinhKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (string.IsNullOrEmpty(temp.IssuedStaffFullName))
                {
                    MessageBox.Show(eHCMSResources.A0376_G1_Msg_InfoChuaChonBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (temp.PatientGender == null)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z2669_G1_ChuaChonGioiTinh), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
                if (string.IsNullOrEmpty(temp.FullName) || string.IsNullOrEmpty(temp.Address))
                {
                    MessageBox.Show(eHCMSResources.A0902_G1_Msg_InfoNhapTTin1, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
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

        private IEnumerator<IResult> GetOutwardDrugInvoiceAndItems(long outiID, bool bShowPaymentDlg)
        {
            yield return GenericCoRoutineTask.StartTask(LoadOutwardDrugInvoiceByIDAction, outiID);

            if (SelectedOutwardInfo != null)
            {
                yield return GenericCoRoutineTask.StartTask(GetOutwardDrugDetailsByOutwardInvoiceAction, SelectedOutwardInfo.outiID);
            }

            if (bShowPaymentDlg)
            {
                ShowFormCountMoney();
            }

            yield break;
        }

        private void SaveDrugs(OutwardDrugInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator();
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
                                //goi ham tinh tien
                                //CountMoneyForVisitorPharmacy(OutID, true);
                                AmountPaided = 0;

                                // TxD 04/09/2014 Moved the following 2 methods into GetOutwardDrugInvoiceAndItems CoRoutine above
                                // ShowFormCountMoney();
                                // LoadOutwardDrugInvoiceByID(OutID);

                                Coroutine.BeginExecute(GetOutwardDrugInvoiceAndItems(OutID, true));

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
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private decimal AmountPaided = 0;
        private IEnumerator<IResult> DoGetAmountPaided(long outiID, OutwardDrugInvoice Invoice)
        {
            var paymentTypeTask = new CalcMoneyPaidedForDrugInvoiceTask(outiID);
            yield return paymentTypeTask;
            AmountPaided = paymentTypeTask.Amount;
            ShowFormCountMoney();
            yield break;
        }

        private void ShowFormCountMoney()
        {
            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
            {
                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -AmountPaided;
                }
                else
                {
                    // TxD 04/09/2014 No Need to DeepCopy here
                    //proAlloc.TotalPayForSelectedItem = SelectedOutwardInfo.TotalInvoicePrice.DeepCopy();
                    //proAlloc.TotalPaySuggested = SelectedOutwardInfo.TotalInvoicePrice.DeepCopy() - AmountPaided;
                    proAlloc.TotalPayForSelectedItem = SelectedOutwardInfo.TotalInvoicePrice;
                    proAlloc.TotalPaySuggested = SelectedOutwardInfo.TotalInvoicePrice - AmountPaided;
                }

                proAlloc.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        private void AddOutwardInvoiceAndDetail(OutwardDrugInvoice OutwardInvoice)
        {
            if (OutwardInvoice.outiID > 0)
            {
                SaveDrugs(OutwardInvoice);
            }
            else
            {
                if (SelectedOutwardInfo.OutwardDrugs.Count != 0)
                {
                    SaveDrugs(OutwardInvoice);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.K0451_G1_NhapThuocCanBan);
                }
            }
        }

        public void btnSaveMoney()
        {
            string strError = "";

            if (SelectedOutwardInfo == null)
            {
                return;
            }

            if (SelectedOutwardInfo.OutwardDrugs == null || SelectedOutwardInfo.OutwardDrugs.Count <= 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0639_G1_Msg_InfoKhCoCTietPhBanLe), eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (!this.CheckValid(SelectedOutwardInfo))
            {
                return;
            }

            if (!CheckValidInvoice(SelectedOutwardInfo))
            {
                return;
            }

            if (GridInward == null)
            {
                Globals.ShowMessage(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }

            SelectedOutwardInfo.StaffID = GetStaffLogin().StaffID;
            SelectedOutwardInfo.SelectedStorage = new RefStorageWarehouseLocation();
            SelectedOutwardInfo.SelectedStorage.StoreID = StoreID;
            SelectedOutwardInfo.StoreID = StoreID;
            SelectedOutwardInfo.TypID = (long)AllLookupValues.RefOutputType.BANLE;
            SelectedOutwardInfo.V_ByOutPrice = (long)AllLookupValues.V_ByOutPrice.GIATHONGTHUONG;
            SelectedOutwardInfo.ColectDrugSeqNumType = 1;

            for (int i = 0; i < SelectedOutwardInfo.OutwardDrugs.Count; i++)
            {
                if (SelectedOutwardInfo.OutwardDrugs[i].OutPrice <= 0)
                {
                    MessageBox.Show(eHCMSResources.Z1709_G1_GiaBanLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (SelectedOutwardInfo.OutwardDrugs[i].OutQuantity <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0979_G1_Msg_InfoSLgXuatMoiDongLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

            SelectedOutwardInfo.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;

            AddOutwardInvoiceAndDetail(SelectedOutwardInfo);
        }

        public void btnMoney(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardInfo == null)
            {
                return;
            }

            if (SelectedOutwardInfo.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0931_G1_Msg_InfoKhTheTinhTien2, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                if (SelectedOutwardInfo.RefundTime == null)
                {
                    MessageBox.Show(eHCMSResources.A0946_G1_Msg_InfoChonHTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Phiếu xuất này đã hủy. Không thu tiền được.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                return;
            }

            if (SelectedOutwardInfo.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.A0950_G1_Msg_InfoKhTheThuTienNua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutwardInfo.outiID, SelectedOutwardInfo));
            // ShowFormCountMoney();
        }

        private void GetOutwardDrugDetailsByOutwardInvoiceAction(GenericCoRoutineTask theTask, object objOutID)
        {
            long OutwardID = (long)objOutID;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    bool bContinue = true;
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
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            bContinue = false;
                        }
                        finally
                        {
                            theTask.ActionComplete(bContinue);
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void GetOutwardDrugDetailsByOutwardInvoice(long OutwardID)
        {
            this.ShowBusyIndicator();
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
                            //Khi chưa trả tiền và phiếu xuất hôm nay thì mặc định không check quyền còn lại sẽ theo quyền được phân
                          
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

            });

            t.Start();
        }

        public void SearchOutwardInfo(int PageIndex, int PageSize)
        {

            int Total = 0;

            //KMx: Nếu tìm kiếm mà không theo tiêu chí nào hết thì phải giới hạn ngày (08/08/2014 09:51).
            if (SearchCriteria == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchCriteria.OutInvID))
            {
                SearchCriteria.fromdate = Globals.GetCurServerDateTime().AddDays(-1);
                SearchCriteria.todate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.fromdate = null;
                SearchCriteria.todate = null;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchCriteria, PageIndex, PageSize, true, null, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IVisitorSearch> onInitDlg = delegate (IVisitorSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardInfoList.Clear();
                                        proAlloc.OutwardInfoList.TotalItemCount = Total;
                                        proAlloc.OutwardInfoList.PageIndex = 0;
                                        proAlloc.OutwardInfoList.PageSize = PageSize;
                                        foreach (OutwardDrugInvoice p in results)
                                        {
                                            proAlloc.OutwardInfoList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IVisitorSearch>(onInitDlg);
                                }
                                else
                                {
                                    SelectedOutwardInfo = results.FirstOrDefault();
                                    HideShowColumnDelete();
                                    GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                            }
                            //KMx: Sau khi set new thì phải set TypID lại, để lần sau tìm phiếu xuất của bán lẻ thôi. Nếu không là bị lỗi tìm phiếu xuất của bán theo toa luôn (24/02/2013 17:06)
                            SearchCriteria = new SearchOutwardInfo();
                            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.BANLE;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            DisableSearch = true;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool _DisableSearch = true;
        public bool DisableSearch
        {
            get { return _DisableSearch; }
            set
            {
                if (_DisableSearch != value)
                {
                    _DisableSearch = value;
                    NotifyOfPropertyChange(() => DisableSearch);
                }
            }
        }
        public void btnSearch()
        {
            DisableSearch = false;
            SearchOutwardInfo(0, 20);
        }

        public void btnSearchAdvance()
        {
            Action<IVisitorSearch> onInitDlg = delegate (IVisitorSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            };
            GlobalsNAV.ShowDialog<IVisitorSearch>(onInitDlg);

        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            DisableSearch = false;
            if (e.Key == Key.Enter)
            {
                //if (SearchCriteria != null)
                //{
                //    SearchCriteria.OutInvID = (sender as TextBox).Text;
                //}
                //SearchOutwardInfo(0, Globals.PageSize);
                btnSearch();
            }
            else { DisableSearch = true; }
        }
        public void btnClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //mai coi lai
            if (e.ClickCount == 1)
            {
                DisableSearch = false;
                SearchOutwardInfo(0, Globals.PageSize);
            }
        }

        public void btnDeletePhieu()
        {
            if (SelectedOutwardInfo != null)
            {
                OutWardDrugInvoice_Delete(SelectedOutwardInfo.outiID);
            }
        }

        public void OutWardDrugInvoice_Delete(long ID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutWardDrugInvoice_Delete(ID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int results = contract.EndOutWardDrugInvoice_Delete(asyncResult);
                            if (results == 0)
                            {
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                InitializeOutwardDrugInvoice();
                            }
                            else if (results == 1)
                            {
                                MessageBox.Show("Phiếu này đã được thu tiền nên không thể xóa!");
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0577_G1_PhKgTonTai);
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
                HideShowColumnDelete();
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
            SetDefaultCustomerName();
            HideShowColumnDelete();
            SumTotalPrice = 0;
        }

        private void CallTestMethod1()
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    string strOutMsg = "LoL";
                    contract.BeginTestMethod1(88, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndTestMethod1(out strOutMsg, asyncResult);
                            string curDateTime = Globals.ServerDate.Value.ToString("dd/MM/yyyy h:mm:ss.ff");
                            System.Diagnostics.Debug.WriteLine("[" + curDateTime + "] ===>> :" + strOutMsg);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void CallTestMethod1_Successively(int nTime)
        {
            for (int nCnt = 0; nCnt < nTime; ++nCnt)
            {
                CallTestMethod1();
            }
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0148_G1_Msg_ConfTaoMoiPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                InitializeOutwardDrugInvoice();
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
            this.ShowBusyIndicator();

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
                                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
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

        public void btnEditPayed()
        {
            //KMx: Kiểm tra thời gian tối đa cho phép cập nhật (23/07/2014 14:55).
            int AllowTimeUpdateOutInvoice = Globals.ServerConfigSection.PharmacyElements.AllowTimeUpdateOutInvoice;
            if (AllowTimeUpdateOutInvoice > 0)
            {
                TimeSpan t = Globals.ServerDate.Value - SelectedOutwardInfo.OutDate;
                if (t.TotalHours >= AllowTimeUpdateOutInvoice)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1291_G1_TGianToiDaChoPhepCNhat, AllowTimeUpdateOutInvoice.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            //Phần kiểm tra này có 3 chổ sử dụng tương tự (Bán thuốc lẻ, bán thuốc theo toa, xuất nội bộ). Nếu sửa ở đây thì phải sửa cho 2 chổ kia luôn. (24/02/2014 16:25)
            //KMx: Nếu phiếu xuất đã báo cáo rồi.
            if (SelectedOutwardInfo.AlreadyReported)
            {
                //Nếu user không có quyền cập nhật phiếu xuất đã báo cáo rồi thì chặn lại.
                if (!mBanThuocLe_CapNhatSauBaoCao)
                {
                    MessageBox.Show(eHCMSResources.Z1653_G1_PhXuatDaBCLHeQLy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //Nếu user có quyền cập nhật phiếu xuất đã báo cáo rồi thì confirm lại.
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z1654_G1_PhXuatDaBCCoMuonCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
            //▼===== #003
            Action<IEditVisitor> onInitDlg = delegate (IEditVisitor proAlloc)
            {
                proAlloc.SelectedOutwardInfo = SelectedOutwardInfo.DeepCopy();
                proAlloc.SelectedOutwardInfoCoppy = SelectedOutwardInfo.DeepCopy();
                proAlloc.OutwardDrugListCopy = SelectedOutwardInfo.OutwardDrugs;
                proAlloc.ListOutwardDrugFirstCopy = SelectedOutwardInfo.OutwardDrugs.DeepCopy();
                proAlloc.ListOutwardDrugFirst = SelectedOutwardInfo.OutwardDrugs.DeepCopy();
                proAlloc.SumTotalPrice = SelectedOutwardInfo.TotalInvoicePrice;
                proAlloc.IDFirst = SelectedOutwardInfo.outiID;
                proAlloc.SetDefaultForStore();
            };
            GlobalsNAV.ShowDialog<IEditVisitor>(onInitDlg);
            //▲===== #003
        }

        #region printing member

        public void btnPreview()
        {
            this.ShowBusyIndicator();

            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutwardInfo.outiID;

                if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
                {
                    //▼====: #001
                    var printerConfigManager = new PrinterConfigurationManager();
                    var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
                    if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != "")
                    {
                        proAlloc.eItem = ReportName.PHIEUNHANTHUOC_THERMAL;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC;
                    }
                    //▲====: #001
                }
                else
                {
                    proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_PRIVATE;
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);

            this.HideBusyIndicator();
        }

        public void btnPrintReceipt()
        {
            GenericPayment rptGenpayment = new GenericPayment();
            rptGenpayment.PersonName = SelectedOutwardInfo.FullName;
            rptGenpayment.PersonAddress = SelectedOutwardInfo.Address;
            rptGenpayment.PhoneNumber = SelectedOutwardInfo.NumberPhone;
            rptGenpayment.GenericPaymentCode = SelectedOutwardInfo.OutInvID;
            rptGenpayment.V_GenericPaymentReason = "Thu tiền bán thuốc";
            rptGenpayment.StaffName = StaffName;
            rptGenpayment.PaymentDate = SelectedOutwardInfo.OutDate;
            rptGenpayment.DOB = SelectedOutwardInfo.DOBString;
            rptGenpayment.PaymentAmount = SelectedOutwardInfo.TotalInvoicePrice;
            rptGenpayment.VATAmount = SelectedOutwardInfo.TotalInvoicePrice - (SelectedOutwardInfo.TotalInvoicePrice / (decimal)1.05);
            rptGenpayment.VATPercent = 1.05;

            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.CurGenPaymt = rptGenpayment;
                reportVm.eItem = ReportName.PHIEU_THU_KHAC_V4;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    // 20192002 TNHX Update Report for Printer Bisollon (In Nhiệt)
                    contract.BeginGetCollectionDrugForThermalInPdfFormat(SelectedOutwardInfo.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetCollectionDrugForThermalInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, results, ActiveXPrintType.ByteArray, "A5");
                            Globals.EventAggregator.Publish(printEvt);
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

            });
            t.Start();
        }
        #endregion

        #region IHandle<ChooseBatchNumberVisitorEvent> Members

        public void Handle(ChooseBatchNumberVisitorEvent message)
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

        #region IHandle<ChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(ChooseBatchNumberVisitorResetQtyEvent message)
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
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                RefeshData();
                SelectedSellVisitor = new GetDrugForSellVisitor();  //20200418 TBL: Khi đổi kho thì dữ liệu trên autocomplete phải clear
            }
        }

        #region IHandle<PharmacyPayEvent> Members

        private long PaymentID = 0;
        GenericCoRoutineTask taskPayment = null;

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            // TxD 12/06/2014 Fixed Printing of OutwardInvoice (Ban Thuoc le) even when it's not been paid for

            taskPayment = new GenericCoRoutineTask(AddTransactionVisitorAction, payment);
            yield return taskPayment;
            PaymentID = (long)taskPayment.GetResultObj(0);

            //KMx: Load lại Invoice vô điều kiện để lấy lại PaidTime => Quyết định xem có cho thu tiền nữa không (tránh trường hợp thu tiền 2 lần) (03/09/2014 17:12).

            yield return GenericCoRoutineTask.StartTask(LoadOutwardDrugInvoiceByIDAction, SelectedOutwardInfo.outiID);

            // TxD 04/09/2014 Added the following yield return GetOutwardDrugDetailsByOutwardInvoiceAction
            if (SelectedOutwardInfo != null)
            {
                yield return GenericCoRoutineTask.StartTask(GetOutwardDrugDetailsByOutwardInvoiceAction, SelectedOutwardInfo.outiID);
            }

            btnPreview();

            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            var paymentTypeTask = new AddTracsactionForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;

            //KMx: Load lại Invoice vô điều kiện để lấy lại PaidTime => Quyết định xem có cho thu tiền không (tránh trường hợp thu tiền 2 lần) (03/09/2014 17:12).

            yield return GenericCoRoutineTask.StartTask(LoadOutwardDrugInvoiceByIDAction, SelectedOutwardInfo.outiID);

            // TxD 04/09/2014 Added the following yield return GetOutwardDrugDetailsByOutwardInvoiceAction
            if (SelectedOutwardInfo != null)
            {
                yield return GenericCoRoutineTask.StartTask(GetOutwardDrugDetailsByOutwardInvoiceAction, SelectedOutwardInfo.outiID);
            }

            yield break;
        }


        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (this.IsActive == false || message == null)
            {
                return;
            }

            if (SelectedOutwardInfo.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                // TxD 12/06/2014 Also modified the calling of LoadOutwardDrugInvoiceByID by using GenericCouroutine

                Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutwardInfo), null, null);

            }
            else
            {
                // TxD 12/06/2014 Fixed Printing of OutwardInvoice (Ban Thuoc le) even when it's not been paid for

                // TxD 04/09/2014: Added the following checking to Ensure that an OutwardDrugInvoice CANNOT be PAID twice 
                //                  This happens when the Payment Dialog is poped up twice caused by very very fast mouse double clicking or faulty mouse
                if (SelectedOutwardInfo != null && SelectedOutwardInfo.PaidTime.HasValue)
                {
                    MessageBox.Show("Phieu Xuat Thuoc nay da thanh toan tien.");
                    return;
                }
                Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutwardInfo), null, null);
            }

        }

        private void AddTransactionVisitorAction(GenericCoRoutineTask genTask, object _payment)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bContinue = true;
                        contract.BeginAddTransactionVisitor((PatientTransactionPayment)_payment, SelectedOutwardInfo, Globals.LoggedUserAccount.StaffID, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long _PaymentID = 0;
                                bool value = contract.EndAddTransactionVisitor(out _PaymentID, asyncResult);
                                genTask.AddResultObj(_PaymentID);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void LoadOutwardDrugInvoiceByIDAction(GenericCoRoutineTask genTask, object OutwardID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    bool bContinue = true;
                    contract.BeginGetOutWardDrugInvoiceVisitorByID((long)OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutwardInfo = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            HideShowColumnDelete();

                            // TxD 04/09/2014 : Commented out the following and replace it with a separate call to GetOutwardDrugDetailsByOutwardInvoiceAction in CoRoutine
                            //if (SelectedOutwardInfo != null)
                            //{
                            //    GetOutwardDrugDetailsByOutwardInvoice(SelectedOutwardInfo.outiID);
                            //}
                        }
                        catch (Exception ex)
                        {
                            bContinue = false;
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            genTask.ActionComplete(bContinue);
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }



        #endregion

        #region IHandle<PharmacyCloseEditPayed> Members

        public void Handle(PharmacyCloseEditPayed message)
        {
            if (message == null || this.IsActive == false)
            {
                return;
            }

            // TxD 05/09/2014 Replaced the following with call to generic CoRoutine GetOutwardDrugInvoiceAndItems
            //LoadOutwardDrugInvoiceByID(message.SelectedOutwardInvoice.outiID);
            Coroutine.BeginExecute(GetOutwardDrugInvoiceAndItems(message.SelectedOutwardInvoice.outiID, false));
        }

        #endregion

        #region Huy Phieu Member
        public void btnCancel(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0116_G1_Msg_ConfHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CancelOutwardInvoiceVisitor();
                //CountMoneyForVisitorPharmacy(SelectedOutwardInfo.outiID, false);
            }
        }

        private void CancelOutwardInvoiceVisitor()
        {
            SelectedOutwardInfo.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;
            //Thêm staffid để lưu lại người hủy phiếu
            SelectedOutwardInfo.StaffID = Globals.LoggedUserAccount.StaffID;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //contract.BeginOutWardDrugInvoiceVisitor_Cancel(SelectedOutwardInfo, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginOutWardDrugInvoiceVisitor_Cancel_Pst(SelectedOutwardInfo, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long TransItemID = 0;
                            //var results = contract.EndOutWardDrugInvoiceVisitor_Cancel(out TransItemID, asyncResult);
                            var results = contract.EndOutWardDrugInvoiceVisitor_Cancel_Pst(out TransItemID, asyncResult);
                            if (TransItemID > 0)
                            {
                                Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutwardInfo.outiID, SelectedOutwardInfo));
                            }

                            // TxD 05/09/2014 Replaced the following with call to generic CoRoutine GetOutwardDrugInvoiceAndItems
                            //LoadOutwardDrugInvoiceByID(SelectedOutwardInfo.outiID);
                            Coroutine.BeginExecute(GetOutwardDrugInvoiceAndItems(SelectedOutwardInfo.outiID, false));

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

            });

            t.Start();
        }

        public void btnHoanTien()
        {
            Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutwardInfo.outiID, SelectedOutwardInfo));
        }

        #endregion

        #region Tra Hang Member

        public void btnReturn()
        {
            Action<IReturnDrug> onInitDlg = delegate (IReturnDrug proAlloc)
            {
                proAlloc.IsChildWindow = true;
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new DataEntities.SearchOutwardInfo();
                }
                proAlloc.SearchCriteria.ID = SelectedOutwardInfo.outiID;
                proAlloc.btnSearch();
            };
            GlobalsNAV.ShowDialog<IReturnDrug>(onInitDlg);
        }

        #endregion

        #region IHandle<PharmacyCloseFormReturnEvent> Members

        public void Handle(PharmacyCloseFormReturnEvent message)
        {
            if (IsActive == false || message == null)
            {
                return;
            }

            // TxD 05/09/2014 Replaced the following with call to generic CoRoutine GetOutwardDrugInvoiceAndItems
            // LoadOutwardDrugInvoiceByID(SelectedOutwardInfo.outiID);
            Coroutine.BeginExecute(GetOutwardDrugInvoiceAndItems(SelectedOutwardInfo.outiID, false));

        }

        #endregion


        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                SearchGetDrugForSellVisitor(txt, true, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
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
                    SearchGetDrugForSellVisitor((sender as TextBox).Text, true, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
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
                if (SelectedOutwardInfo != null)
                {
                    return _VisibilityName && SelectedOutwardInfo.CanSaveAndPaid;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutwardInfo != null)
                {
                    _VisibilityName = value && SelectedOutwardInfo.CanSaveAndPaid;
                    _VisibilityCode = !_VisibilityName && SelectedOutwardInfo.CanSaveAndPaid;
                }
                else
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
                }
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                if (SelectedOutwardInfo != null)
                {
                    return !_VisibilityName && SelectedOutwardInfo.CanSaveAndPaid;
                }
                return _VisibilityCode;
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

        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
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

        private void HideShowColumnDelete()
        {
            if (GridInward != null)
            {
                if (SelectedOutwardInfo.CanSaveAndPaid && mBanThuocLe_PhieuMoi)
                {
                    GridInward.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    GridInward.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    GridInward.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    GridInward.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
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
                    if (MessageBox.Show("Bạn có chắc muốn xóa những hàng đã chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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

        AxTextBox Axtbx = null;
        public void AxTextBoxCustomerName_Loaded(object sender, RoutedEventArgs e)
        {
            Axtbx = sender as AxTextBox;
            SetDefaultCustomerName();
        }
        private void SetDefaultCustomerName()
        {
            Axtbx.Text = "KL";
            BindingExpression be = Axtbx.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
        public void AxTextBoxCustomerName_LostFocus(object sender, RoutedEventArgs e)
        {
            BindingExpression be = Axtbx.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            InitColumns();
        }
        public void cboICD10_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            LoadRefDiseases(sender as AxAutoComplete, e.Parameter, 0, 0, 100);
        }
        public void cboICD10_DropDownClosing(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gICD10IsDropDown = true;
        }
        public void cboICD10_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!gICD10IsDropDown)
            {
                return;
            }
            gICD10IsDropDown = false;
            if (sender != null)
            {
                SelectedOutwardInfo.MainICD10 = new DiseasesReference();
                SelectedOutwardInfo.MainICD10 = (sender as AxAutoComplete).SelectedItem as DiseasesReference;
            }
        }
        public void btnExportEInvoice()
        {
            if (SelectedOutwardInfo == null || SelectedOutwardInfo.outiID == 0)
            {
                return;
            }
            IEditOutPtTransactionFinalization TransactionFinalizationView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            TransactionFinalizationView.InvoiceType = 2;
            TransactionFinalizationView.TransactionFinalizationObj = new OutPtTransactionFinalization
            {
                TaxMemberName = SelectedOutwardInfo.FullName,
                TaxMemberAddress = SelectedOutwardInfo.Address,
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PatientFullName = SelectedOutwardInfo.FullName,
                V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                outiID = SelectedOutwardInfo.outiID,
                V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                eInvoiceKey = SelectedOutwardInfo.OutInvID
            };
            GlobalsNAV.ShowDialog_V3(TransactionFinalizationView);
            if (!TransactionFinalizationView.IsSaveCompleted)
            {
                return;
            }
            //IEditOutPtTransactionFinalization TransactionFinalizationView = Globals.GetViewModel<IEditOutPtTransactionFinalization>();
            //TransactionFinalizationView.TransactionFinalizationObj = new OutPtTransactionFinalization
            //{
            //    V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT
            //};
            //TransactionFinalizationView.ViewCase = 1;
            //GlobalsNAV.ShowDialog_V3(TransactionFinalizationView);
            //if (!TransactionFinalizationView.IsSaveCompleted)
            //{
            //    return;
            //}
            Coroutine.BeginExecute(AddOrUpdateTransactionFinalization_Routine(SelectedOutwardInfo, TransactionFinalizationView.TransactionFinalizationObj, false));
        }
        #endregion
        #region Methods
        private void InitColumns()
        {
            if (GridInward == null)
            {
                return;
            }
            if (GridInward.GetColumnByName("clDose") != null && !IsPrescriptionCollect)
            {
                GridInward.GetColumnByName("clDose").Visibility = Visibility.Collapsed;
            }
        }
        public void LoadRefDiseases(AxAutoComplete aComboBox, string aName, byte aType, int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDiseases(aName, PageIndex, PageSize, aType
                        , 0
                        , Globals.GetCurServerDateTime()
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 10;
                            var results = contract.EndSearchRefDiseases(out Total, asyncResult);
                            ObservableCollection<DiseasesReference> mIDC10CodeCollection = new ObservableCollection<DiseasesReference>();
                            if (results != null)
                            {
                                mIDC10CodeCollection = results.ToObservableCollection();
                            }
                            aComboBox.ItemsSource = mIDC10CodeCollection;
                            aComboBox.PopulateComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                    }), null);
                }
            });
            t.Start();
        }
        private ObservableCollection<eHCMS.Services.Core.Gender> _GenderCollection;
        public ObservableCollection<eHCMS.Services.Core.Gender> GenderCollection
        {
            get { return _GenderCollection; }
            set
            {
                _GenderCollection = value;
                NotifyOfPropertyChange(() => GenderCollection);
            }
        }
        public void LoadGenderCollection()
        {
            if (Globals.allGenders != null)
            {
                GenderCollection = Globals.allGenders.ToObservableCollection();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllGenders(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                IList<eHCMS.Services.Core.Gender> allItems = contract.EndGetAllGenders(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allGenders = allItems.ToList();
                                    GenderCollection = Globals.allGenders.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0692_G1_Msg_InfoKhTheLayDSGioiTinh);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> AddOrUpdateTransactionFinalization_Routine(OutwardDrugInvoice aOutwardDrugInvoice, OutPtTransactionFinalization TransactionFinalizationObj, bool aIsUpdate)
        {
            TransactionFinalizationObj.eInvoiceKey = aOutwardDrugInvoice.OutInvID;
            ILoggerDialog mLogView = Globals.GetViewModel<ILoggerDialog>();
            var mThread = new Thread(() =>
            {
                GlobalsNAV.ShowDialog_V4(mLogView, null, null, false, true);
            });
            mThread.Start();
            Patient mPatient = new Patient
            {
                PatientCode = aOutwardDrugInvoice.OutInvID,
                FullName = aOutwardDrugInvoice.FullName,
                PatientFullStreetAddress = aOutwardDrugInvoice.Address,
                PatientPhoneNumber = aOutwardDrugInvoice.NumberPhone
            };
            List<RptOutPtTransactionFinalizationDetail> mTransactionFinalizationDetail = new List<RptOutPtTransactionFinalizationDetail>();
            mTransactionFinalizationDetail.Add(new RptOutPtTransactionFinalizationDetail
            {
                HITypeDescription = eHCMSResources.Z2669_G1_ThuocDieuTri,
                IsHasVAT = true,
                VATPercent = 1.05,
                TotalPatientPayment = Math.Round(aOutwardDrugInvoice.TotalInvoicePrice, 0, MidpointRounding.AwayFromZero)
            });
            mLogView.AppendLogMessage(string.Format("{0}: {1}", eHCMSResources.Z2651_G1_TienHanhThemMoiBN, mPatient.PatientCode));
            yield return GenericCoRoutineTask.StartTask(CommonGlobals.ImportPatientEInvoice, mPatient, TransactionFinalizationObj, mLogView);
            yield return GenericCoRoutineTask.StartTask(CommonGlobals.AddEInvoice, new VNPTCustomer(mPatient), TransactionFinalizationObj, mTransactionFinalizationDetail, mLogView);
            var AddOrUpdateTransactionFinalizationTask = new GenericCoRoutineTask(CommonGlobals.AddOrUpdateTransactionFinalization, true, TransactionFinalizationObj);
            yield return AddOrUpdateTransactionFinalizationTask;
            mLogView.IsFinished = true;
            if (AddOrUpdateTransactionFinalizationTask.Error != null)
            {
                yield break;
            }
            string mFileName = string.Format("{0}.pdf", TransactionFinalizationObj.eInvoiceKey);
            string mFilePath = Path.Combine(Path.GetTempPath(), mFileName);
            CommonGlobals.ExportInvoiceToPdfNoPay(TransactionFinalizationObj.eInvoiceKey, mFilePath);
        }
        #endregion
        private string _strOutDrugInvStatus = "";
        public string strOutDrugInvStatus
        {
            get
            {
                return _strOutDrugInvStatus;
            }
            set
            {
                if (_strOutDrugInvStatus != value)
                {
                    _strOutDrugInvStatus = value;
                    NotifyOfPropertyChange(() => strOutDrugInvStatus);
                }
            }
        }

        //▼====== #004
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
        private bool _CanCancel;
        public bool CanCancel
        {
            get { return _CanCancel; }
            set
            {
                if (_CanCancel != value)
                {
                    _CanCancel = value;
                    NotifyOfPropertyChange(() => CanCancel);
                }
            }
        }
        private bool _CanEditPayed;
        public bool CanEditPayed
        {
            get { return _CanEditPayed; }
            set
            {
                if (_CanEditPayed != value)
                {
                    _CanEditPayed = value;
                    NotifyOfPropertyChange(() => CanEditPayed);
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
        //▲======= #004
        private void CheckPermissionForOutward()
        {
            if (SelectedOutwardInfo == null)
            {
                return;
            }
            if (SelectedOutwardInfo.OutDate.Date != Globals.GetCurServerDateTime().Date || SelectedOutwardInfo.V_OutDrugInvStatus == 15003)
            {
                CanCancel = false;
                CanEditPayed = false;
            }
            else if (SelectedOutwardInfo.PaidTime == null)
            {
                CanCancel = SelectedOutwardInfo.CanCancel;
                CanEditPayed = SelectedOutwardInfo.CanEditPayed;
            }
            else
            {
                CanCancel = mBanThuocLe_HuyPhieu;
                CanEditPayed = mBanThuocLe_CapNhatPhieu;
            }
        }

        //▼==== #005
        private bool _IsHIOutPt = false;
        public bool IsHIOutPt
        {
            get { return _IsHIOutPt && Globals.ServerConfigSection.CommonItems.EnableHIStore; }
            set
            {
                if (_IsHIOutPt != value)
                {
                    _IsHIOutPt = value;
                    NotifyOfPropertyChange(() => IsHIOutPt);
                }
            }
        }
        //▲==== #005
    }
}