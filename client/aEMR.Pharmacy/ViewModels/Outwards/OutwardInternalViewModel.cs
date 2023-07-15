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
using aEMR.Common.BaseModel;
/*
 * 20190810 #001 TTM:   BM 0011791: Fix lỗi nút xoá hàng không sử dụng được, Fix lại trước khi thực hiện sẽ lấy lại Date theo giờ hệ thống.
 * 20190913 #002 TNHX [BM0014334]: Filter ToStore base on OutwardStore + TypID + Refactor code
 * 20200903 #003 TNHX [BM]: Cho phép xuất thuốc nhà thuốc tìm thuốc bằng tên hoạt chất.
 * 20210923 #004 QTD : Filter Storage
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IOutwardInternal)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardInternalViewModel : ViewModelBase, IOutwardInternal
        , IHandle<PharmacyCloseSearchRequestEvent>, IHandle<PharmacyCloseSearchDemageDrugEvent>
        , IHandle<ChooseBatchNumberVisitorEvent>, IHandle<ChooseBatchNumberVisitorResetQtyEvent>
        , IHandle<PharmacyPayEvent>, IHandle<PharmacyCloseEditPayed>
    {
        public string TitleForm { get; set; }

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3,
            HamLuong = 4,
            DVT = 5,
            LoSX = 6,
            SLYC = 7,
            ThucXuat = 8,
            DonGia = 9,
            ThanhTien = 10
        }

        [ImportingConstructor]
        public OutwardInternalViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();

            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_All());
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            Coroutine.BeginExecute(DoGetByOutPriceLookups());
            Coroutine.BeginExecute(DoRefOutputType_All());

            SearchCriteria = new SearchOutwardInfo
            {
                TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO
            };

            RefeshData();
            SetDefaultForStore();
            GetAllStaffContrain();
            GetHospital_IsFriends();
            ToStoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
        }

        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice
            {
                OutDate = Globals.ServerDate.Value,
                SelectedStaff = GetStaffLogin(),
                ToStaffID = 0,
                HosID = 0
            };
            ClearData();
            TotalPrice = 0;
            HideShowColumnDelete();
        }

        private void ClearData()
        {
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.OutInvID = "";

            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }
        }

        private void SetDefaultForStore()
        {
            if (FromStoreCbx != null)
            {
                StoreID = FromStoreCbx.FirstOrDefault().StoreID;
                //--▼-- 29/12/2020 DatTB Gán biến mặc định
                var selectedStore = (RefStorageWarehouseLocation)FromStoreCbx.FirstOrDefault();
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 29/12/2020 DatTB 

                //▼======= #004
                IsMainStorage = selectedStore.IsMain;
                IsSubStorage = selectedStore.IsSubStorage;
                //▲======= #004
                if (SearchCriteria != null)
                {
                    SearchCriteria.StoreID = StoreID;
                }
            }
            if (ToStoreCbx != null && SelectedOutInvoice != null && ToStoreCbx.Count > 0)
            {
                SelectedOutInvoice.ToStoreID = ToStoreCbx.FirstOrDefault().StoreID;
            }
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mXuatNoiBo_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_Tim, (int)ePermission.mView);
            mXuatNoiBo_ThongTinPhieuXuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThongTinPhieuXuat, (int)ePermission.mView);
            mXuatNoiBo_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_Them, (int)ePermission.mView);
            mXuatNoiBo_ThucHien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThucHien, (int)ePermission.mView);
            mXuatNoiBo_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ThuTien, (int)ePermission.mView);
            mXuatNoiBo_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_ReportIn, (int)ePermission.mView);
            mXuatNoiBo_CapNhatSauBaoCao = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatNoiBo,
                                               (int)oPharmacyEx.mXuatNoiBo_CapNhatSauBaoCao, (int)ePermission.mView);
        }

        #region checking account

        private bool _mXuatNoiBo_Tim = true;
        private bool _mXuatNoiBo_ThongTinPhieuXuat = true;
        private bool _mXuatNoiBo_Them = true;
        private bool _mXuatNoiBo_ThucHien = true;
        private bool _mXuatNoiBo_ThuTien = true;
        private bool _mXuatNoiBo_ReportIn = true;
        private bool _mXuatNoiBo_CapNhatSauBaoCao = true;

        public bool mXuatNoiBo_Tim
        {
            get
            {
                return _mXuatNoiBo_Tim;
            }
            set
            {
                if (_mXuatNoiBo_Tim == value)
                    return;
                _mXuatNoiBo_Tim = value;
            }
        }
        public bool mXuatNoiBo_ThongTinPhieuXuat
        {
            get
            {
                return _mXuatNoiBo_ThongTinPhieuXuat;
            }
            set
            {
                if (_mXuatNoiBo_ThongTinPhieuXuat == value)
                    return;
                _mXuatNoiBo_ThongTinPhieuXuat = value;
            }
        }
        public bool mXuatNoiBo_Them
        {
            get
            {
                return _mXuatNoiBo_Them;
            }
            set
            {
                if (_mXuatNoiBo_Them == value)
                    return;
                _mXuatNoiBo_Them = value;
            }
        }
        public bool mXuatNoiBo_ThucHien
        {
            get
            {
                return _mXuatNoiBo_ThucHien;
            }
            set
            {
                if (_mXuatNoiBo_ThucHien == value)
                    return;
                _mXuatNoiBo_ThucHien = value;
            }
        }
        public bool mXuatNoiBo_ThuTien
        {
            get
            {
                return _mXuatNoiBo_ThuTien;
            }
            set
            {
                if (_mXuatNoiBo_ThuTien == value)
                    return;
                _mXuatNoiBo_ThuTien = value;
            }
        }
        public bool mXuatNoiBo_ReportIn
        {
            get
            {
                return _mXuatNoiBo_ReportIn;
            }
            set
            {
                if (_mXuatNoiBo_ReportIn == value)
                    return;
                _mXuatNoiBo_ReportIn = value;
            }
        }

        public bool mXuatNoiBo_CapNhatSauBaoCao
        {
            get
            {
                return _mXuatNoiBo_CapNhatSauBaoCao;
            }
            set
            {
                if (_mXuatNoiBo_CapNhatSauBaoCao == value)
                    return;
                _mXuatNoiBo_CapNhatSauBaoCao = value;
            }
        }

        #endregion

        #region Properties Member

        private bool _CanSelectByOutPrice = true;
        public bool CanSelectByOutPrice
        {
            get
            {
                return _CanSelectByOutPrice;
            }
            set
            {
                _CanSelectByOutPrice = value;
                NotifyOfPropertyChange(() => CanSelectByOutPrice);
            }
        }

        private bool _IsEnableToStore = false;
        public bool IsEnableToStore
        {
            get
            {
                return _IsEnableToStore;
            }
            set
            {
                _IsEnableToStore = value;
                NotifyOfPropertyChange(() => IsEnableToStore);
            }
        }

        private bool? IsCost = true;
        private bool _IsInternalForStore = true;
        public bool IsInternalForStore
        {
            get
            {
                return _IsInternalForStore && !IsBalanceView;
            }
            set
            {
                _IsInternalForStore = value;
                if (_IsInternalForStore)
                {
                    IsInternalForDoctor = false;
                    IsInternalForHospital = false;
                    //if (SelectedOutInvoice != null)
                    //    SelectedOutInvoice.TypID = 0;
                }
                NotifyOfPropertyChange(() => IsInternalForStore);
            }
        }

        private bool _IsInternalForDoctor = false;
        public bool IsInternalForDoctor
        {
            get
            {
                return _IsInternalForDoctor && !IsBalanceView;
            }
            set
            {
                _IsInternalForDoctor = value;
                if (_IsInternalForDoctor)
                {
                    IsInternalForStore = false;
                    IsInternalForHospital = false;
                }
                NotifyOfPropertyChange(() => IsInternalForDoctor);
            }
        }

        private bool _IsInternalForHospital = false;
        public bool IsInternalForHospital
        {
            get
            {
                return _IsInternalForHospital && !IsBalanceView;
            }
            set
            {
                _IsInternalForHospital = value;
                if (_IsInternalForHospital)
                {
                    IsInternalForStore = false;
                    IsInternalForDoctor = false;
                }
                NotifyOfPropertyChange(() => IsInternalForHospital);
            }
        }

        private ObservableCollection<RefOutputType> _RefOutputTypeList;
        public ObservableCollection<RefOutputType> RefOutputTypeList
        {
            get { return _RefOutputTypeList; }
            set
            {
                _RefOutputTypeList = value;
                NotifyOfPropertyChange(() => RefOutputTypeList);
            }
        }

        private ObservableCollection<Hospital> _Hospitals;
        public ObservableCollection<Hospital> Hospitals
        {
            get { return _Hospitals; }
            set
            {
                if (_Hospitals != value)
                {
                    _Hospitals = value;
                    NotifyOfPropertyChange(() => Hospitals);
                }
            }
        }

        private ObservableCollection<Staff> _ListStaff;
        public ObservableCollection<Staff> ListStaff
        {
            get { return _ListStaff; }
            set
            {
                if (_ListStaff != value)
                {
                    _ListStaff = value;
                    NotifyOfPropertyChange(() => ListStaff);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _AllStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> AllStoreCbx
        {
            get
            {
                return _AllStoreCbx;
            }
            set
            {
                if (_AllStoreCbx != value)
                {
                    _AllStoreCbx = value;
                    NotifyOfPropertyChange(() => AllStoreCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _ToStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> ToStoreCbx
        {
            get
            {
                return _ToStoreCbx;
            }
            set
            {
                if (_ToStoreCbx != value)
                {
                    _ToStoreCbx = value;
                    NotifyOfPropertyChange(() => ToStoreCbx);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _FromStoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> FromStoreCbx
        {
            get
            {
                return _FromStoreCbx;
            }
            set
            {
                if (_FromStoreCbx != value)
                {
                    _FromStoreCbx = value;
                    NotifyOfPropertyChange(() => FromStoreCbx);
                }
            }
        }

        private ObservableCollection<Lookup> _ByOutPriceLookups;
        public ObservableCollection<Lookup> ByOutPriceLookups
        {
            get
            {
                return _ByOutPriceLookups;
            }
            set
            {
                if (_ByOutPriceLookups != value)
                {
                    _ByOutPriceLookups = value;
                    NotifyOfPropertyChange(() => ByOutPriceLookups);
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

        private Staff GetStaffLogin()
        {
            StaffName = Globals.LoggedUserAccount.Staff.FullName;
            return Globals.LoggedUserAccount.Staff;
        }

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
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
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private ObservableCollection<OutwardDrug> OutwardDrugsCopy;
        private OutwardDrugInvoice SelectedOutInvoiceCoppy;
        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private decimal _TotalPrice;
        public decimal TotalPrice
        {
            get
            {
                return _TotalPrice;
            }
            set
            {
                _TotalPrice = value;
                NotifyOfPropertyChange(() => TotalPrice);
            }
        }

        //--▼-- 28/12/2020 DatTB
        private long _V_GroupTypes = (long)AllLookupValues.V_GroupTypes.TINH_GTGT;
        public long V_GroupTypes
        {
            get => _V_GroupTypes; set
            {
                _V_GroupTypes = value;
                NotifyOfPropertyChange(() => V_GroupTypes);
            }
        }
        //--▲-- 28/12/2020 DatTB

        //--▼--17/12/2020 DatTB
        private bool _Is_Enabled = false;
        public bool Is_Enabled
        {
            get
            {
                return _Is_Enabled;
            }
            set
            {
                if (_Is_Enabled != value)
                {
                    _Is_Enabled = value;
                    NotifyOfPropertyChange(() => Is_Enabled);
                }
            }
        }
        //--▲--17/12/2020 DatTB

        #endregion

        private IEnumerator<IResult> DoRefOutputType_All()
        {
            var paymentTypeTask = new LoadOutputListTask(false, false, true);
            yield return paymentTypeTask;
            //KMx: Vì chức năng "Bán" trong xuất nội bộ còn bị lỗi khi trả hàng thì không insert vào bảng PatientTransactionDetail (khi trả hàng không hiện trong báo cáo).
            //     Nên tạm thời khóa chức năng "Bán" (TypID = 2). Khi nào sửa được thì mở ra lại.
            //RefOutputTypeList = paymentTypeTask.RefOutputTypeList.Where(x => x.IsSelectedPharmacyInternal == true && x.TypID != 2).ToObservableCollection();
            //KMx: Do nhà thuốc yêu cầu mở lại chức năng Bán nên sửa lại (14/02/2014 17:09)
            if (!IsBalanceView)
            {
                RefOutputTypeList = paymentTypeTask.RefOutputTypeList.Where(x => x.IsSelectedPharmacyInternal == true).ToObservableCollection();
            } else
            {
                RefOutputTypeList = paymentTypeTask.RefOutputTypeList.Where(x =>  x.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO).ToObservableCollection(); // 29/12/2020 DatTB x.IsSelectedPharmacyInternal == true &&
            }
            //SetDefaultForStore();
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, false);
            yield return paymentTypeTask;
            FromStoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_All()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false, null, false, false);
            yield return paymentTypeTask;
            AllStoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            yield break;
        }

        private IEnumerator<IResult> DoGetByOutPriceLookups()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_ByOutPrice, false, false);
            yield return paymentTypeTask;
            ByOutPriceLookups = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetAllStaffContrain()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffContain(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllStaffContain(asyncResult);
                                if (results != null)
                                {
                                    ListStaff = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (ListStaff != null)
                                    {
                                        ListStaff.Clear();
                                    }
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

        private void GetHospital_IsFriends()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHopital_IsFriends(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndHopital_IsFriends(asyncResult);
                                Hospitals = results.ToObservableCollection();
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

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice != null && (SelectedOutInvoice.outiID == 0))
            {
                if (grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) is Button colBatchNumber)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                if (grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) is Button colBatchNumber)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        //ChangedWPF-CMN
        //public void grdPrescription_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        //{
        //    SumTotalPrice();
        //}

        private bool CheckValid()
        {
            bool result = true;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.OutwardDrugs == null)
                {
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                {
                    if (item.Validate() == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        AxComboBox toStoreCbxCtrl = null;
        public void AxToStoreCbx_Loaded(object source, RoutedEventArgs eventArgs)
        {
            toStoreCbxCtrl = (AxComboBox)source;
        }

        AxComboBox CbxNguoiNhan = null;
        public void AxCbxNguoiNhan_Loaded(object source, RoutedEventArgs eventArgs)
        {
            CbxNguoiNhan = (AxComboBox)source;
        }

        AxComboBox CbxBVBan = null;
        public void AxCbxBVBan_Loaded(object source, RoutedEventArgs eventArgs)
        {
            CbxBVBan = (AxComboBox)source;
        }

        private void GetOutWardDrugInvoiceVisitorByID(long OutwardID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var SelectedOutInvoiceTmp = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                                RefeshOutwardInvoicesCbx(SelectedOutInvoiceTmp);
                                //co khong cho vao cac su kien 
                                CheckAccept();
                                OutwardDrugDetails_Load(SelectedOutInvoice.outiID);

                                HideShowColumnDelete();
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

        public void BtnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugInvoice_Search(0, 20);
        }

        private void OutwardDrugInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new SearchOutwardInfo();
            }
            if (SelectedOutInvoice.TypID == null || SelectedOutInvoice.TypID == 0)
            {
                MessageBox.Show("Vui lòng chọn loại xuất để tìm kiếm.");
                return;
            }
            SearchCriteria.StoreID = StoreID;
            SearchCriteria.TypID = SelectedOutInvoice.TypID;

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutWardDrugInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndOutWardDrugInvoice_SearchByType(out int TotalCount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        void onInitDlg(IPharmacyDamageExpiryDrugSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.OutwardDrugInvoiceList.Clear();
                                            proAlloc.OutwardDrugInvoiceList.TotalItemCount = TotalCount;
                                            proAlloc.OutwardDrugInvoiceList.PageIndex = 0;
                                            proAlloc.OutwardDrugInvoiceList.PageSize = 20;
                                            proAlloc.pageTitle = eHCMSResources.G2893_G1_XuatNBoTimPhCu;
                                            foreach (OutwardDrugInvoice p in results)
                                            {
                                                proAlloc.OutwardDrugInvoiceList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IPharmacyDamageExpiryDrugSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        ToStoreCbx = AllStoreCbx;
                                        RefeshOutwardInvoicesCbx(results.FirstOrDefault());
                                        HideShowColumnDelete();
                                        //load detail
                                        CheckAccept();
                                        OutwardDrugDetails_Load(SelectedOutInvoice.outiID);
                                    }
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
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

        private void OutwardDrugDetails_Load(long outiID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugDetailsByOutwardInvoice(outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                                //load danh sach thuoc theo hoa don 
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                SumTotalPrice();
                                DeepCopyOutwardDrug();
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

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.OutInvID = (sender as TextBox).Text;
                }
                OutwardDrugInvoice_Search(0, 20);
            }
        }

        private void SumTotalPrice()
        {
            TotalPrice = 0;
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.TotalInvoicePrice = 0;
                if (SelectedOutInvoice.OutwardDrugs != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        SelectedOutInvoice.TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                    }
                }
                TotalPrice = SelectedOutInvoice.TotalInvoicePrice;
            }
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(ItemsControl.ItemsSourceProperty, null);
        }

        #region printing member
        public void BtnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = SelectedOutInvoice.outiID;
            DialogView.eItem = ReportName.PHARMACY_XUATNOIBO;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void BtnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private readonly bool IsHIPatient = false;

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList;
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

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp;
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
                //tim theo ten
                //if (ViewCase == 0 || ViewCase == 1)
                if (ViewCase == 0)
                {
                    SearchGetDrugForSellVisitor(e.Parameter, false);
                }
                else
                {
                    SearchGetDrugForSellVisitorFromCategory(e.Parameter, false);
                }
            }
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new { hd.DrugID, hd.DrugCode, hd.BrandName, hd.UnitName, hd.Qty } into hdgroup
                      where hdgroup.Sum(i => i.Remaining) > 0 //20210402 QTD thêm điều kiện bỏ thuốc không còn remaining sử dụng khi giảm kiểm kê
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          hdgroup.Key.DrugID,
                          hdgroup.Key.DrugCode,
                          hdgroup.Key.UnitName,
                          hdgroup.Key.BrandName,
                          hdgroup.Key.Qty
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor
                {
                    DrugID = hhh.ToList()[i].DrugID,
                    DrugCode = hhh.ToList()[i].DrugCode,
                    BrandName = hhh.ToList()[i].BrandName,
                    UnitName = hhh.ToList()[i].UnitName,
                    Remaining = hhh.ToList()[i].Remaining,
                    Qty = hhh.ToList()[i].Qty
                };
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

        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode)
        {
            long? RequestID = null;
            if (SelectedOutInvoice != null)
            {
                RequestID = SelectedOutInvoice.ReqDrugInID;
            }
            //this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #003
                        contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(IsCost, Name, StoreID, RequestID, IsCode
                            , IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                        {
                            //▲======= #003
                            try
                            {
                                var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestPharmacy(asyncResult);
                                GetDrugForSellVisitorList.Clear();
                                GetDrugForSellVisitorListSum.Clear();
                                GetDrugForSellVisitorTemp.Clear();
                                foreach (GetDrugForSellVisitor s in results)
                                {
                                    GetDrugForSellVisitorTemp.Add(s);
                                }
                                if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
                                {
                                    foreach (OutwardDrug d in OutwardDrugsCopy)
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
                                    if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                                    {
                                        foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
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
                    //this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (ViewCase == 0 || ViewCase == 1)
            {
                if (temp == null)
                {
                    return false;
                }
                return !temp.HasErrors;
            }
            return true;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p1 in SelectedOutInvoice.OutwardDrugs)
                {
                    if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        // p1.IsLoad = 0;
                        p1.QtyOffer += p.QtyOffer;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetDrugForSellVisitor.InsuranceCover;

                    if (p.InwardDrug == null)
                    {
                        p.InwardDrug = new InwardDrug
                        {
                            InID = p.InID.GetValueOrDefault(),
                            DrugID = p.DrugID
                        };
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrug.NormalPrice = p.OutPrice;
                    p.InwardDrug.HIPatientPrice = p.OutPrice;
                    p.InwardDrug.HIAllowedPrice = p.HIAllowedPrice;

                    SelectedOutInvoice.OutwardDrugs.Add(p);
                }
                txt = "";
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
        }

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID && x.BrandName == value.BrandName).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();
                if (item.Remaining > 0)
                {
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        if (value.Qty > item.Remaining)
                        {
                            p.QtyOffer = item.Remaining;
                            value.Qty = value.Qty - item.Remaining;
                        }
                        else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
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
                        p.IsNotVat = item.IsNotVat;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        //if (value.Qty > (int)value.RequiredNumber)
                        //{
                        //    p.QtyOffer = (int)value.RequiredNumber;
                        //    value.Qty = value.Qty - (int)value.RequiredNumber;
                        //}
                        //else
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        p.IsNotVat = item.IsNotVat;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrug(GetDrugForSellVisitor value)
        {
            //20201229 QTD: Thêm lại điều kiện không cho xuất số lẻ thập phân
            int intOutput = 0;
            if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
            {
            if (CheckValidDrugAuto(value))
            {
                if (ViewCase == 0 || ViewCase == 1)
                {
                    ChooseBatchNumber(value);
                }
                else
                {
                    var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
                    foreach (GetDrugForSellVisitor item in items)
                    {
                        OutwardDrug p = new OutwardDrug();
                        p.GetDrugForSellVisitor = item;
                        p.DrugID = item.DrugID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        {
                            p.QtyOffer = value.Qty;
                            value.Qty = 0;
                        }
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.VAT = item.VAT;
                        p.IsNotVat = item.IsNotVat;
                        SelectedOutInvoice.OutwardDrugs.Add(p);
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
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0037_G1_ThuocDaBiLoi, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            }
        }
            else
            {
                MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0.");
            }
        }

        private void ReCountQtyRequest(GetDrugForSellVisitor SelectedSellVisitor)
        {
            if (SelectedOutInvoice.OutwardDrugs == null)
            {
                SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
            }
            var results1 = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == SelectedSellVisitor.DrugID);
            if (results1 != null && results1.Count() > 0)
            {
                foreach (OutwardDrug p in results1)
                {
                    if (p.QtyOffer > p.OutQuantity)
                    {
                        p.QtyOffer = p.OutQuantity;
                    }
                    SelectedSellVisitor.Qty = SelectedSellVisitor.Qty - p.QtyOffer;
                }
            }
        }

        //Kiên: Check Selected Sell Visitor equal null.
        //      Check quantity of Selected Sell Visitor equal 0.
        public bool CheckQtySellUnequalZero(GetDrugForSellVisitor SelectedSellVisitor)
        {
            //20201229 QTD: Màn hình cân bằng sử dụng chung hàm check CheckQtySellUnequalZero, không cho nhập xuất số lẻ nữa 
            //if (IsBalanceView)
            //{
            //return true;
            //}
            if (SelectedOutInvoice != null && SelectedSellVisitor != null)
            {
                if (SelectedSellVisitor.RequiredNumber <= 0
                    || !int.TryParse(SelectedSellVisitor.RequiredNumber.ToString(), out int intOutput) //Nếu số lượng không phải là số nguyên.
                    || SelectedSellVisitor.RequiredNumber > SelectedSellVisitor.Remaining) //Nếu số lượng muốn thêm > số lượng còn trong kho.
                {
                    MessageBox.Show(eHCMSResources.Z0890_G1_SLgKgHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.K0400_G1_ChonThuocCanThem, eHCMSResources.T0074_G1_I);
                return false;
            }
        }

        //Kiên: Combine 2 functions in 1.
        public void ReCountQtyAndAddList(GetDrugForSellVisitor SelectedSellVisitor)
        {
            if (!CheckQtySellUnequalZero(SelectedSellVisitor))
                return;
            else
            {
                ReCountQtyRequest(SelectedSellVisitor);
                AddListOutwardDrug(SelectedSellVisitor);
            }
        }

        //Kiên: Những ViewModel có hàm tương tự (Khi VM này sai thì phải sửa những VM khác).
        //1. Bán thuốc theo toa.
        //2. Xuất nội bộ.
        public void AddItem(object sender, RoutedEventArgs e)
        {
            //ReCountQtyRequest();
            //AddListOutwardDrug(SelectedSellVisitor);
            //Kiên: Combine 2 functions: ReCountQtyRequest and AddListOutwardDrug
            ReCountQtyAndAddList(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

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
        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorBatchNumber(DrugID, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
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
                            Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByDrugIDFirst = OutwardDrugsCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                }
                GetDrugForSellVisitorBatchNumber(DrugID);
            }
        }

        #region IHandle<ChooseBatchNumberVisitorEvent> Members
        public void Handle(ChooseBatchNumberVisitorEvent message)
        {
            if (message != null && IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                }
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPrice();
            }
        }
        #endregion

        #region IHandle<ChooseBatchNumberVisitorResetQtyEvent> Members
        public void Handle(ChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor = message.BatchNumberVisitorSelected;
                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                }
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.VAT = message.BatchNumberVisitorSelected.VAT;
                SumTotalPrice();
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
                        p.VAT = d.VAT;
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
                void onInitDlg(IChooseBatchNumberVisitor proAlloc)
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
                }
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }
        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrug p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(SelectedOutwardDrug);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrug != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                DeleteInvoiceDrugInObject();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0915_G1_Msg_InfoPhChiXem, eHCMSResources.G0442_G1_TBao);
            }
        }

        //private int iicountStore = 0;
        public void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null /*&& iicountStore > 0*/)
            {
                //--▼-- 28/12/2020 DatTB Gán biến Kho đã chọn để so sánh
                var selectedStore = (RefStorageWarehouseLocation)cbx.SelectedItem;
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 28/12/2020 DatTB

                //▼======= #004
                IsMainStorage = selectedStore.IsMain;
                IsSubStorage = selectedStore.IsSubStorage;
                //▲======= #004

                RefeshData();
                SelectedSellVisitor = new GetDrugForSellVisitor(); //20200418 TBL: Khi đổi kho thì dữ liệu trên autocomplete phải clear
            }
            //iicountStore++;
        }

        #region IHandle<PharmacyCloseSearchRequestEvent> Members

        public void Handle(PharmacyCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                if (message.SelectedRequest is RequestDrugInward Request)
                {
                    ClearData();
                    SelectedOutInvoice.ReqDrugInID = Request.ReqDrugInID;
                    SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;
                    SelectedOutInvoice.ToStoreID = Request.InDeptStoreID;
                    SelectedOutInvoice.ToStaffID = Request.StaffID;
                    CheckAccept();
                    spGetInBatchNumberAndPrice_ByRequestID(Request.ReqDrugInID, StoreID);
                }
            }
        }

        private void CheckAccept()
        {
            if (SelectedOutInvoice.ToStoreID > 0)
            {
                IsInternalForStore = true;
            }
            else if (SelectedOutInvoice.ToStaffID > 0)
            {
                IsInternalForDoctor = true;
            }
            else
            {
                IsInternalForHospital = true;
            }
        }

        #endregion

        public void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginspGetInBatchNumberAndPrice_ByRequestPharmacy(IsCost, RequestID, StoreID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndspGetInBatchNumberAndPrice_ByRequestPharmacy(asyncResult);
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                OutwardDrugsCopy = null;
                                SumTotalPrice();
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

        public void BtnFindRequest(object sender, RoutedEventArgs e)
        {
            void onInitDlg(IRequestSearchPharmacy proAlloc)
            {
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new RequestSearchCriteria();
                }
                proAlloc.SearchCriteria.DaNhanHang = true;
                proAlloc.SearchRequestDrugInward(0, Globals.PageSize);
            }
            GlobalsNAV.ShowDialog<IRequestSearchPharmacy>(onInitDlg);
        }

        private void CountMoneyForVisitorPharmacy(long outiID, bool bThuTien)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCountMoneyForVisitorPharmacy(outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool value = contract.EndCountMoneyForVisitorPharmacy(out decimal AmountPaid, asyncResult);
                                //goi ham tinh tien
                                void onInitDlg(ISimplePayPharmacy proAlloc)
                                {
                                    proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                    proAlloc.TotalPayForSelectedItem = TotalPrice.DeepCopy();
                                    proAlloc.TotalPaySuggested = AmountPaid;
                                }
                                GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
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

        private void OutwardDrugInvoice_SaveByType(OutwardDrugInvoice OutwardInvoice, bool bThuTien)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginOutwardDrugInvoice_SaveByType_Pst(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                //bool value = contract.EndOutwardDrugInvoice_SaveByType(out OutID, out StrError, asyncResult);
                                bool value = contract.EndOutwardDrugInvoice_SaveByType_Pst(out long OutID, out string StrError, asyncResult);

                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                    if (bThuTien)
                                    {
                                        ShowFormCountMoney();
                                        GetOutWardDrugInvoiceVisitorByID(OutID);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                        RefeshData();
                                        GetOutWardDrugInvoiceVisitorByID(OutID);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
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

        public void BtnSave()
        {
            if (CheckData())
            {
                //▼===== #001: Trước khi lưu thông tin phiếu xuất sẽ lấy ngày giờ hệ thống lại .
                SelectedOutInvoice.OutDate = Globals.GetCurServerDateTime(); //14/12/2020 DatTB
                //▲===== 
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugInvoice_SaveByType(SelectedOutInvoice, true);
                }
                //20190912 TNHX: Đã lọc Kho nhan theo Loai Xuat + Kho Xuat => bo chan nay
                //20190315 TBL: Hien tai chan viec luan chuyen kho cho kho nha thuoc
                //else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO && SelectedOutInvoice.StoreID == 2)
                //{
                //    MessageBox.Show(eHCMSResources.Z2618_G1_KhongTheLuanChuyenKhoTuKhoNhaThuoc);
                //    return;
                //}
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO 
                    && SelectedOutInvoice.StoreID == Globals.ServerConfigSection.PharmacyElements.HIStorageID)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugInvoice_SaveByType(SelectedOutInvoice, false);
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugInvoice_SaveByType_Balance(SelectedOutInvoice, false);
                }
                else
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugInvoice_SaveByType(SelectedOutInvoice, false);
                }
            }
        }

        private bool CheckData()
        {
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                SelectedOutInvoice.StoreID = StoreID;
            }
            if (SelectedOutInvoice.TypID == null || SelectedOutInvoice.TypID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0345_G1_ChonLoaiXuat);
                return false;
            }
            if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.CANBANGKHO)
            {
                if (IsInternalForStore)
                {
                    if (SelectedOutInvoice.ToStoreID == null || SelectedOutInvoice.ToStoreID == 0)
                    {
                        MessageBox.Show(eHCMSResources.A0092_G1_Msg_InfoChuaChonKhoDen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    if (SelectedOutInvoice.StoreID == SelectedOutInvoice.ToStoreID)
                    {
                        MessageBox.Show(eHCMSResources.A0618_G1_Msg_InfoKhoXuatKhacKhoNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    SelectedOutInvoice.ToStaffID = 0;
                    SelectedOutInvoice.HosID = 0;
                }
                else if (IsInternalForDoctor)
                {
                    if (SelectedOutInvoice.ToStaffID == null || SelectedOutInvoice.ToStaffID == 0)
                    {
                        MessageBox.Show(eHCMSResources.A0098_G1_Msg_InfoChuaChonNguoiNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    SelectedOutInvoice.ToStoreID = 0;
                    SelectedOutInvoice.HosID = 0;
                }
                else
                {
                    if (SelectedOutInvoice.HosID == null || SelectedOutInvoice.HosID == 0)
                    {
                        MessageBox.Show(eHCMSResources.A0089_G1_Msg_InfoChuaChonBVNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    SelectedOutInvoice.ToStaffID = 0;
                    SelectedOutInvoice.ToStoreID = 0;
                }
            }
            if (SelectedOutInvoice.OutwardDrugs == null || SelectedOutInvoice.OutwardDrugs.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            else
            {
                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                if (SelectedOutInvoice.OutwardDrugs != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugs[i].OutPrice <= 0)
                        {
                            MessageBox.Show("Đơn giá bán phải > 0", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        if (SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor != null && SelectedOutInvoice.OutwardDrugs[i].OutQuantity <= 0)
                        {
                            MessageBox.Show(eHCMSResources.Z1174_G1_SLgXuatLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        //neu ngay het han lon hon ngay hien tai
                        if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, SelectedOutInvoice.OutwardDrugs[i].InExpiryDate) == 1)
                        {
                            strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.BrandName, (i + 1).ToString());
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        public void BtnMoney(object sender, RoutedEventArgs e)
        {
            Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutInvoice.outiID, SelectedOutInvoice));
        }

        public void OutWardDrugInvoice_Delete(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                                    RefeshData();
                                }
                                else if (results == 1)
                                {
                                    Globals.ShowMessage(eHCMSResources.Z1635_G1_PhKgTheXoa, eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0577_G1_PhKgTonTai, eHCMSResources.T0074_G1_I);
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

        public void BtnDeletePhieu()
        {
            if (SelectedOutInvoice != null)
            {
                OutWardDrugInvoice_Delete(SelectedOutInvoice.outiID);
            }
        }

        public void BtnNew()
        {
            //if (MessageBox.Show(eHCMSResources.A0148_G1_Msg_ConfTaoMoiPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            RefeshData();
            if (au != null)
            {
                au.Text = "";
            }
            //}
        }

        private void DeepCopyOutwardDrug()
        {
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
            }
            else
            {
                OutwardDrugsCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }
        }

        //private int icount = 0;
        public void SelectedByOutPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SelectedOutInvoice != null && icount > 0)
            if (SelectedOutInvoice != null)
            {
                GetIsCost();

                if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIATHONGTHUONG)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugs != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].OutPrice = SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.SellingPrice;
                        }
                    }
                }
                else if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIAVON)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugs != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].OutPrice = SelectedOutInvoice.OutwardDrugs[i].GetDrugForSellVisitor.InCost;
                        }
                    }
                }
                else
                {
                    //mo cot gia len cho sua 
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                }
                SumTotalPrice();
            }
            //icount++;
        }

        public void SelectedOutputType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                RefOutputType temp = (RefOutputType)e.AddedItems[0];
                if (temp.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO_LUANCHUYENKHO)
                {
                    if (!IsInternalForStore)
                    {
                        IsInternalForStore = true;
                    }
                    if (StoreID != Globals.ServerConfigSection.PharmacyElements.HIStorageID && SelectedOutInvoice != null && SelectedOutInvoice.OutInvID == "")
                    {
                        ToStoreCbx = AllStoreCbx.Where(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL && x.V_GroupTypes != 0 && x.V_GroupTypes == V_GroupTypes && x.IsMain != IsMainStorage && x.IsSubStorage != IsSubStorage).ToObservableCollection();
                        if (ToStoreCbx.Count > 0)
                        {
                            SelectedOutInvoice.ToStoreID = ToStoreCbx.FirstOrDefault().StoreID;
                        }
                    }
                    else if (StoreID == Globals.ServerConfigSection.PharmacyElements.HIStorageID && SelectedOutInvoice != null && SelectedOutInvoice.OutInvID == "")
                    {
                        ToStoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
                        foreach (var storeTemp in AllStoreCbx)
                        {
                            if (storeTemp.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && storeTemp.IsMain && storeTemp.ListV_MedProductType != null 
                                && storeTemp.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())
                                && !storeTemp.IsSubStorage)
                            {
                                ToStoreCbx.Add(storeTemp);
                            }
                        }
                        if (ToStoreCbx.Count > 0)
                        {
                            SelectedOutInvoice.ToStoreID = ToStoreCbx.FirstOrDefault().StoreID;
                        }
                    }
                    else
                    {
                        ToStoreCbx = AllStoreCbx;
                        if (SelectedOutInvoice != null && SelectedOutInvoice.ToStoreID == 0)
                        {
                            SelectedOutInvoice.ToStoreID = ToStoreCbx.FirstOrDefault().StoreID;
                        }
                    }
                    if (SelectedOutInvoice != null && SelectedOutInvoice.outiID == 0)
                    {
                        SelectedOutInvoice.V_ByOutPrice = (long)AllLookupValues.V_ByOutPrice.GIAVON;
                    }
                    CanSelectByOutPrice = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.CanSaveAndPaid : false;
                    IsEnableToStore = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.IsEnableToStore : false;
                }
                else if (temp.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO)
                {
                    RefStorageWarehouseLocation firstItem = new RefStorageWarehouseLocation();
                    firstItem.StoreID = 0;
                    firstItem.swhlName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T2144_G1_Kho);
                    SelectedOutInvoice.StoreID = firstItem.StoreID;

                    //--▼--29/12/2020 DatTB
                    Is_Enabled = true;
                    //--▲--29/12/2020 DatTB
                }
                else
                {
                    //ToStoreCbx = AllStoreCbx;

                    ToStoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
                    foreach (var storeTemp in AllStoreCbx)
                    {
                        if (storeTemp.V_GroupTypes != 0 && storeTemp.V_GroupTypes == V_GroupTypes && storeTemp.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString()))
                        {
                            ToStoreCbx.Add(storeTemp);
                        }
                    }

                    IsEnableToStore = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.IsEnableToStore : true;
                    CanSelectByOutPrice = SelectedOutInvoice != null && SelectedOutInvoice.outiID > 0 ? SelectedOutInvoice.CanSaveAndPaid : true;
                }
            } 
        }

        private void GetIsCost()
        {
            if (SelectedOutInvoice != null)
            {
                //if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUATNOIBO)
                //{
                //    IsCost = true;
                //}
                //else
                //{
                if (SelectedOutInvoice.V_ByOutPrice == (long)AllLookupValues.V_ByOutPrice.GIAVON)
                {
                    IsCost = true;
                }
                else
                {
                    IsCost = false;
                }
                //}
            }
        }

        //dung lai from tim kiem cua Demage
        #region IHandle<PharmacyCloseSearchDemageDrugEvent> Members

        public void Handle(PharmacyCloseSearchDemageDrugEvent message)
        {
            if (message != null && IsActive)
            {
                SelectedOutInvoice = message.SelectedOutwardDrugInvoice as OutwardDrugInvoice;
                HideShowColumnDelete();
                CheckAccept();
                OutwardDrugDetails_Load(SelectedOutInvoice.outiID);
            }
        }

        #endregion

        #region IHandle<PharmacyPayEvent> Members

        private long PaymentID = 0;

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            var paymentTypeTask = new AddTracsactionForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID);
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            var paymentTypeTask = new AddTracsactionForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            yield break;
        }

        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (IsActive && message != null)
            {
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
                    });
                }
                else
                {
                    Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        BtnPreview();
                        GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
                    });
                }
            }
        }
        #endregion

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                if (ViewCase == 0 || ViewCase == 1)
                {
                    SearchGetDrugForSellVisitor(txt, true);
                }
                else
                {
                    SearchGetDrugForSellVisitorFromCategory(txt, true);
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
                    if (ViewCase == 0 || ViewCase == 1)
                    {
                        SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
                    }
                    else
                    {
                        SearchGetDrugForSellVisitorFromCategory((sender as TextBox).Text, true);
                    }
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
                if (SelectedOutInvoice != null)
                {
                    return _VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value && SelectedOutInvoice.CanSaveAndPaid;
                    _VisibilityCode = !_VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
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
        private bool? IsCode = false;
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
            if (grdPrescription != null)
            {
                if (SelectedOutInvoice.CanSaveAndPaid && mXuatNoiBo_ThucHien)
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    grdPrescription.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void AllCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = true;
                }
            }
        }

        private void UnCheckedfc()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                {
                    SelectedOutInvoice.OutwardDrugs[i].Checked = false;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
            {
                var items = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    var lstremaning = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == false).ToObservableCollection().DeepCopy();
                    var lstDelete = SelectedOutInvoice.OutwardDrugs.Where(x => x.Checked == true).ToObservableCollection().DeepCopy();

                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (OutwardDrug p in lstDelete)
                        {
                            foreach (OutwardDrug item in lstremaning)
                            {
                                if (item.DrugID == p.DrugID)
                                {
                                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                                    break;
                                }
                            }
                        }
                        SumTotalPrice();
                        ListDisplayAutoComplete();
                    }
                    SelectedOutInvoice.OutwardDrugs = lstremaning;
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

        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPrice();
            }
        }

        public void BtnUpdate()
        {
            //Phần kiểm tra này có 3 chổ sử dụng tương tự (Bán thuốc lẻ, bán thuốc theo toa, xuất nội bộ). Nếu sửa ở đây thì phải sửa cho 2 chổ kia luôn. (24/02/2014 16:25)
            //KMx: Nếu phiếu xuất đã báo cáo rồi.
            //(OutwardDrugInvoice) SelectedOutInvoice
            //SelectedOutInvoice.ToStoreID = 2;
            if (SelectedOutInvoice.AlreadyReported)
            {
                //Nếu user không có quyền cập nhật phiếu xuất đã báo cáo rồi thì chặn lại.
                if (!mXuatNoiBo_CapNhatSauBaoCao)
                {
                    MessageBox.Show("Phiếu xuất này đã được báo cáo rồi. Nếu muốn cập nhật vui lòng liên hệ người quản lý!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //Nếu user có quyền cập nhật phiếu xuất đã báo cáo rồi thì confirm lại.
                else
                {
                    if (MessageBox.Show("Phiếu xuất này đã được báo cáo rồi. Bạn có muốn cập nhật không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }
            void onInitDlg(IEditOutwardInternal proAlloc)
            {
                proAlloc.TitleForm = eHCMSResources.Z1651_G1_CapNhatPhXuat;
                proAlloc.SelectedOutInvoice = SelectedOutInvoice.DeepCopy();
                proAlloc.SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
                proAlloc.OutwardDrugListCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                proAlloc.ListOutwardDrugFirstCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                proAlloc.ListOutwardDrugFirst = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                proAlloc.SumTotalPrice = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
                proAlloc.IDFirst = SelectedOutInvoice.outiID.DeepCopy();
                proAlloc.IsInternalForStore = IsInternalForStore;
                proAlloc.IsInternalForDoctor = IsInternalForDoctor;
                proAlloc.IsInternalForHospital = IsInternalForHospital;
                proAlloc.StoreID = StoreID;
            }
            GlobalsNAV.ShowDialog<IEditOutwardInternal>(onInitDlg);
        }

        public void BtnCancel(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0116_G1_Msg_ConfHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CancelOutwardInvoice();
            }
        }

        private void CancelOutwardInvoice()
        {
            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutWardDrugInvoiceVisitor_Cancel_Pst(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndOutWardDrugInvoiceVisitor_Cancel_Pst(out long TransItemID, asyncResult);
                            if (results == 0)
                            {
                                MessageBox.Show(eHCMSResources.A0613_G1_Msg_InfoHuyOK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0907_G1_HuyThatBai);
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

        public void BtnHoanTien()
        {
            Coroutine.BeginExecute(DoGetAmountPaided(SelectedOutInvoice.outiID, SelectedOutInvoice));
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
            void onInitDlg(ISimplePayPharmacy proAlloc)
            {
                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -AmountPaided;
                }
                else
                {
                    proAlloc.TotalPayForSelectedItem = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
                    proAlloc.TotalPaySuggested = SelectedOutInvoice.TotalInvoicePrice.DeepCopy() - AmountPaided;
                }
                proAlloc.StartCalculating();
            }
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        public void Handle(PharmacyCloseEditPayed message)
        {
            if (message != null && this.IsActive && message.SelectedOutwardInvoice != null)
            {
                GetOutWardDrugInvoiceVisitorByID(message.SelectedOutwardInvoice.outiID);
            }
        }

        private OutwardDrugInvoice SelectedOutwardInfo;
        public void BtnReturnDrug()
        {
            if (MessageBox.Show("Bạn có chắc đã nhận được thuốc trả chưa?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SelectedOutwardInfo = SelectedOutInvoice.DeepCopy();
                SelectedOutwardInfo.StaffID = GetStaffLogin().StaffID;
                if (SelectedOutwardInfo != null && SelectedOutwardInfo.OutwardDrugs != null)
                {
                    foreach (var item in SelectedOutwardInfo.OutwardDrugs)
                    {
                        item.OutQuantityReturn = item.OutQuantity;
                    }
                    AddOutwardDrugReturnVisitor();
                }
            }
        }

        private void AddOutwardDrugReturnVisitor()
        {
            long OutiID = 0;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginAddOutWardDrugInvoiceReturnVisitor(SelectedOutwardInfo, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginAddOutWardDrugInvoiceReturnVisitor_Pst(SelectedOutwardInfo, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                //contract.EndAddOutWardDrugInvoiceReturnVisitor(out OutiID, asyncResult);
                                contract.EndAddOutWardDrugInvoiceReturnVisitor_Pst(out OutiID, asyncResult);
                                PrintReturn(OutiID);
                                GetOutWardDrugInvoiceVisitorByID(SelectedOutwardInfo.outiID);
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

        public void PrintReturn(long MaPhieuMuon)
        {
            //chi dung cho tra thuoc da muon 
            void onInitDlg(IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = MaPhieuMuon;
                proAlloc.eItem = ReportName.PHARMACY_TRATHUOC;
            }
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }

        public void rdtSelling_Checked(object sender, RoutedEventArgs e)
        {

        }

        public void rdtLoan_Checked(object sender, RoutedEventArgs e)
        {

        }

        public void rdtRotation_Checked(object sender, RoutedEventArgs e)
        {

        }

        private long _StoreType = (long)AllLookupValues.StoreType.STORAGE_EXTERNAL;
        public long StoreType
        {
            get
            {
                return _StoreType;
            }
            set
            {
                if (_StoreType != value)
                {
                    _StoreType = value;
                    NotifyOfPropertyChange(() => StoreType);
                }
            }
        }

        public void RefeshOutwardInvoicesCbx(OutwardDrugInvoice currentInvoices)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                int nSelIdx = 0;
                foreach (var theItem in ToStoreCbx)
                {
                    if (theItem.StoreID == currentInvoices.ToStoreID)
                    {
                        toStoreCbxCtrl.SelectedIndex = nSelIdx;
                    }
                    ++nSelIdx;
                }
                nSelIdx = 0;
                foreach (var theItem in ListStaff)
                {
                    if (theItem.StaffID == currentInvoices.ToStaffID)
                    {
                        CbxNguoiNhan.SelectedIndex = nSelIdx;
                    }
                    ++nSelIdx;
                }
                nSelIdx = 0;
                foreach (var theItem in Hospitals)
                {
                    if (theItem.HosID == currentInvoices.HosID)
                    {
                        CbxBVBan.SelectedIndex = nSelIdx;
                    }
                    ++nSelIdx;
                }
                SelectedOutInvoice = currentInvoices;
            });
        }

        //▼====== #003
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
        //▲======= #003
        /// <summary>
        /// Giá trị ViewCase: 
        /// 0: Xuất nội bộ
        /// 1: Xuất cân bằng
        /// 2: Nhập cân bằng
        /// </summary>
        private int _ViewCase = 0;
        public int ViewCase
        {
            get { return _ViewCase; }
            set
            {
                if (_ViewCase != value)
                {
                    _ViewCase = value;
                    if (_ViewCase >= 1)
                    {
                        IsBalanceView = true;
                    }
                    else
                    {
                        IsBalanceView = false;
                    }
                    NotifyOfPropertyChange(() => ViewCase);
                    NotifyOfPropertyChange(() => IsBalanceView);
                }
            }
        }
        private bool _IsBalanceView = false;
        public bool IsBalanceView
        {
            get { return _IsBalanceView; }
            set
            {
                if (_IsBalanceView != value)
                {
                    _IsBalanceView = value;
                    NotifyOfPropertyChange(() => IsBalanceView);
                }
            }
        }

        private void OutwardDrugInvoice_SaveByType_Balance(OutwardDrugInvoice OutwardInvoice, bool bThuTien)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugInvoice_SaveByType_Balance(OutwardInvoice, ViewCase, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool value = contract.EndOutwardDrugInvoice_SaveByType_Balance(out long OutID, out string StrError, asyncResult);

                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                    if (bThuTien)
                                    {
                                        ShowFormCountMoney();
                                        GetOutWardDrugInvoiceVisitorByID(OutID);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                        RefeshData();
                                        GetOutWardDrugInvoiceVisitorByID(OutID);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
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
        private void SearchGetDrugForSellVisitorFromCategory(string Name, bool? IsCode)
        {
            long? RequestID = null;
            if (SelectedOutInvoice != null)
            {
                RequestID = SelectedOutInvoice.ReqDrugInID;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForSellVisitorAutoCompleteFromCategory(IsCost, Name, StoreID, RequestID, IsCode
                            , IsSearchByGenericName, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetDrugForSellVisitorAutoCompleteFromCategory(asyncResult);
                                    GetDrugForSellVisitorList.Clear();
                                    GetDrugForSellVisitorListSum.Clear();
                                    GetDrugForSellVisitorTemp.Clear();
                                    foreach (GetDrugForSellVisitor s in results)
                                    {
                                        GetDrugForSellVisitorTemp.Add(s);
                                    }
                                    if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
                                    {
                                        foreach (OutwardDrug d in OutwardDrugsCopy)
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
                                            }
                                        }
                                    }
                                    foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
                                    {
                                        if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                                        {
                                            foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
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
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        //▼====== #004
        private bool _IsMainStorage;
        public bool IsMainStorage
        {
            get { return _IsMainStorage; }
            set
            {
                if (_IsMainStorage != value)
                {
                    _IsMainStorage = value;
                    NotifyOfPropertyChange(() => IsMainStorage);
                }
            }
        }

        private bool _IsSubStorage;
        public bool IsSubStorage
        {
            get { return _IsSubStorage; }
            set
            {
                if (_IsSubStorage != value)
                {
                    _IsSubStorage = value;
                    NotifyOfPropertyChange(() => IsSubStorage);
                }
            }
        }
        //▲======= #004
    }
}
