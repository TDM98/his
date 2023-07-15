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
using System.Text;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
/*
 * 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
 * 20211102 #002 QTD: Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IXuatHangKyGoi)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XuatHangKyGoiViewModel : ViewModelBase, IXuatHangKyGoi, IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent>
        , IHandle<DrugDeptChooseBatchNumberEvent>, IHandle<DrugDeptChooseBatchNumberResetQtyEvent>
        , IHandle<DrugDeptCloseSearchStorageEvent>
        , IHandle<DrugDeptCloseSearchHospitalEvent>
        , IHandle<ItemSelected<Patient>>
        , IHandle<DrugDeptCloseSearchStaffEvent>
        , IHandle<PharmacyPayEvent>
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


        private bool _IsLoadingRefGenericDrugCategory = false;
        public bool IsLoadingRefGenericDrugCategory
        {
            get { return _IsLoadingRefGenericDrugCategory; }
            set
            {
                if (_IsLoadingRefGenericDrugCategory != value)
                {
                    _IsLoadingRefGenericDrugCategory = value;
                    NotifyOfPropertyChange(() => IsLoadingRefGenericDrugCategory);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private bool _isLoadingSupplier = false;
        public bool isLoadingSupplier
        {
            get { return _isLoadingSupplier; }
            set
            {
                if (_isLoadingSupplier != value)
                {
                    _isLoadingSupplier = value;
                    NotifyOfPropertyChange(() => isLoadingSupplier);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail || IsLoadingRefGenericDrugCategory || isLoadingSupplier); }
        }

        #endregion

        private enum DataGridCol
        {
            ColDelete = 0,
            MaThuoc = 1,
            TenThuoc = 2,
            HamLuong = 3,
            DVT = 4,
            LoSX = 5,
            SLYC = 6,
            ThucXuat = 7,
            DonGia = 8,
            ThanhTien = 9
        }

        private bool _mIsInputTemp;
        public bool mIsInputTemp
        {
            get { return _mIsInputTemp; }
            set
            {
                if (_mIsInputTemp != value)
                {
                    _mIsInputTemp = value;
                    NotifyOfPropertyChange(() => mIsInputTemp);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;


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

        [ImportingConstructor]
        public XuatHangKyGoiViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            Globals.EventAggregator.Subscribe(this);

            RefGenMedProductDisplays = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDisplays.PageSize = Globals.PageSize;
            RefGenMedProductDisplays.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenMedProductDisplays_OnRefresh);

           
            Coroutine.BeginExecute(GetLookupOutputTo());
            Coroutine.BeginExecute(DoGetByOutPriceLookups());

            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            //LoadRefOutputType();Bang bỏ chỗ này
            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            RefeshData();
            SetDefaultForStore();
            Is_Enabled = false; //--17/12/2020 DatTB
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            //Coroutine.BeginExecute(DoGetStore_ClinicDept());
            //Coroutine.BeginExecute(DoGetStore_OrtherClinic());
            //GetAllStaffContrain();
            //GetHospital_IsFriends();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Coroutine.BeginExecute(DoGetStoreToSell());
        }
        void RefGenMedProductDisplays_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefGenMedProductDetails_HangKyGoi(BrandName, IsCode, RefGenMedProductDisplays.PageSize, RefGenMedProductDisplays.PageIndex);
        }


        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
            SelectedOutInvoice.OutDate = DateTime.Now;
            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StaffName = GetStaffLogin().FullName;

            SetDefaultOutputTo();
            SetDefaultOutputType();
            SetDefultRefGenericDrugCategory();
            ClearData();
            TotalPrice = 0;
        }

        private void ClearData()
        {
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.OutInvID = "";

            OutwardDrugMedDeptsCopy = null;

            if (RefGenMedProductDetailsList == null)
            {
                RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsList.Clear();
            }
            if (RefGenMedProductDetailsListSum == null)
            {
                RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsListSum.Clear();
            }

            if (RefGenMedProductDetailsTemp == null)
            {
                RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();
            }
            else
            {
                RefGenMedProductDetailsTemp.Clear();
            }
        }



        private void SetDefaultForStore()
        {
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
                if (SearchCriteria != null)
                {
                    SearchCriteria.StoreID = StoreID;
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account

        private bool _mXuatHangKyGui_Tim = true;
        private bool _mXuatHangKyGui_PhieuMoi = true;
        private bool _mXuatHangKyGui_Save = true;
        private bool _mXuatHangKyGui_ThuTien = true;
        private bool _mXuatHangKyGui_XemIn = true;
        private bool _mXuatHangKyGui_In = true;
        private bool _mXuatHangKyGui_DeleteInvoice = true;
        private bool _mXuatHangKyGui_PrintReceipt = true;

        public bool mXuatHangKyGui_Tim
        {
            get
            {
                return _mXuatHangKyGui_Tim;
            }
            set
            {
                if (_mXuatHangKyGui_Tim == value)
                    return;
                _mXuatHangKyGui_Tim = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Tim);
            }
        }

        public bool mXuatHangKyGui_PhieuMoi
        {
            get
            {
                return _mXuatHangKyGui_PhieuMoi;
            }
            set
            {
                if (_mXuatHangKyGui_PhieuMoi == value)
                    return;
                _mXuatHangKyGui_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_PhieuMoi);
            }
        }

        public bool mXuatHangKyGui_Save
        {
            get
            {
                return _mXuatHangKyGui_Save;
            }
            set
            {
                if (_mXuatHangKyGui_Save == value)
                    return;
                _mXuatHangKyGui_Save = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_Save);
            }
        }

        public bool mXuatHangKyGui_ThuTien
        {
            get
            {
                return _mXuatHangKyGui_ThuTien;
            }
            set
            {
                if (_mXuatHangKyGui_ThuTien == value)
                    return;
                _mXuatHangKyGui_ThuTien = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_ThuTien);
            }
        }

        public bool mXuatHangKyGui_XemIn
        {
            get
            {
                return _mXuatHangKyGui_XemIn;
            }
            set
            {
                if (_mXuatHangKyGui_XemIn == value)
                    return;
                _mXuatHangKyGui_XemIn = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_XemIn);
            }
        }

        public bool mXuatHangKyGui_In
        {
            get
            {
                return _mXuatHangKyGui_In;
            }
            set
            {
                if (_mXuatHangKyGui_In == value)
                    return;
                _mXuatHangKyGui_In = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_In);
            }
        }

        public bool mXuatHangKyGui_DeleteInvoice
        {
            get
            {
                return _mXuatHangKyGui_DeleteInvoice;
            }
            set
            {
                if (_mXuatHangKyGui_DeleteInvoice == value)
                    return;
                _mXuatHangKyGui_DeleteInvoice = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_DeleteInvoice);
            }
        }

        public bool mXuatHangKyGui_PrintReceipt
        {
            get
            {
                return _mXuatHangKyGui_PrintReceipt;
            }
            set
            {
                if (_mXuatHangKyGui_PrintReceipt == value)
                    return;
                _mXuatHangKyGui_PrintReceipt = value;
                NotifyOfPropertyChange(() => mXuatHangKyGui_PrintReceipt);
            }
        }
        #endregion

        #region Properties Member
        private ObservableCollection<RefGenericDrugCategory_1> _RefGenericDrugCategory_1s;
        public ObservableCollection<RefGenericDrugCategory_1> RefGenericDrugCategory_1s
        {
            get
            {
                return _RefGenericDrugCategory_1s;
            }
            set
            {
                if (_RefGenericDrugCategory_1s != value)
                {
                    _RefGenericDrugCategory_1s = value;
                    NotifyOfPropertyChange(() => RefGenericDrugCategory_1s);
                }
            }
        }

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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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
                    NotifyOfPropertyChange(() => CanSelectedRefGenDrugCatID_1);
                }

            }
        }

        public bool CanSelectedRefGenDrugCatID_1
        {
            get { return V_MedProductType == (long)AllLookupValues.MedProductType.THUOC; }
        }

        private bool? IsCost = true;

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
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptsCopy;

        private OutwardDrugMedDeptInvoice SelectedOutInvoiceCoppy;

        private OutwardDrugMedDeptInvoice _SelectedOutInvoice;
        public OutwardDrugMedDeptInvoice SelectedOutInvoice
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


        #endregion


        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            IsLoadingRefGenericDrugCategory = true;
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            IsLoadingRefGenericDrugCategory = false;
            yield break;
        }
        private void SetDefultRefGenericDrugCategory()
        {
            if (SelectedOutInvoice != null && RefGenericDrugCategory_1s != null)
            {
                SelectedOutInvoice.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private IEnumerator<IResult> DoGetStoreToSell()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #002
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx == null || StoreCbx.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #002
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice != null && SelectedOutInvoice.outiID == 0)
            {
                Button colBatchNumber = grdPrescription.Columns[5].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = grdPrescription.Columns[5].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
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
                if (SelectedOutInvoice.OutwardDrugMedDepts == null)
                {
                    return false;
                }
                foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
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

        private void GetOutwardDrugMedDeptInvoice(long OutwardID)
        {
            isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetOutwardDrugMedDeptInvoice(OutwardID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                SelectedOutInvoice = contract.EndGetOutwardDrugMedDeptInvoice(asyncResult);
                            //co khong cho vao cac su kien 
                            OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                isLoadingGetID = false;
                                // Globals.IsBusy = false;
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

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugMedDeptInvoice_Search(0, Globals.PageSize);
        }

        private void OutwardDrugMedDeptInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new MedDeptInvoiceSearchCriteria();
            }
            SearchCriteria.StoreID = StoreID;
            if (mIsInputTemp)
            {
                SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI;
            }
            else
            {
                SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;
            }
            SearchCriteria.V_MedProductType = V_MedProductType;

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                int TotalCount = 0;
                                var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out TotalCount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        this.DlgHideBusyIndicator();
                                        Action<IXuatNoiBoSearch> onInitDlg = delegate (IXuatNoiBoSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.OutwardMedDeptInvoiceList.Clear();
                                            proAlloc.OutwardMedDeptInvoiceList.TotalItemCount = TotalCount;
                                            proAlloc.OutwardMedDeptInvoiceList.PageIndex = 0;
                                            foreach (OutwardDrugMedDeptInvoice p in results)
                                            {
                                                proAlloc.OutwardMedDeptInvoiceList.Add(p);
                                            }
                                        };
                                        GlobalsNAV.ShowDialog<IXuatNoiBoSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        this.DlgHideBusyIndicator();
                                        ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        SelectedOutInvoice = results.FirstOrDefault();
                                        //load detail
                                        OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                                    }
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.G0442_G1_TBao);
                                }

                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                isLoadingSearch = false;
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

        private void OutwardDrugMedDeptDetails_Load(long outiID)
        {
            isLoadingDetail = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(outiID, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                                SumTotalPrice();
                                DeepCopyOutwardDrugMedDept();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                isLoadingDetail = false;
                                // Globals.IsBusy = false;
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

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeInvoice = (sender as TextBox).Text;
                }
                OutwardDrugMedDeptInvoice_Search(0, Globals.PageSize);
            }
        }

        private void SumTotalPrice()
        {
            TotalPrice = 0;
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    TotalPrice += SelectedOutInvoice.OutwardDrugMedDepts[i].OutAmount.GetValueOrDefault();
                }
            }
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;

        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        #region printing member

        public void btnPreview()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1474_G1_PhXuatKhoThuocGayNghien;
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTADDICTIVE;
                    }
                    else if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z1475_G1_PhXuatKhoThuocHuongTamThan;
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTADDICTIVE;
                    }
                    else
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G0787_G1_Thuoc.ToUpper());
                        if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G2907_G1_YCu.ToUpper());
                    if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                    }
                }
                else
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.T1616_G1_HC.ToUpper());
                    if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                    }
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new ReportServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;

            //        contract.BeginGetOutwardInternalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
            //        {
            //            try
            //            {
            //                var results = contract.EndGetOutwardInternalInPdfFormat(asyncResult);
            //                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
            //                Globals.EventAggregator.Publish(results);
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});
            //t.Start();
        }


        public void btnPreviewPayment()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0933_G1_Msg_InfoPhChuaLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.PaidTime == null)
            {
                MessageBox.Show(eHCMSResources.A0936_G1_Msg_InfoChuaTraTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //KMx: Dùng chung cho In Quyết Toán Nội Trú và In Hóa Đơn Bán Lẻ Của Khoa Dược (26/12/2014 14:47)
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;

                switch (Globals.ServerConfigSection.CommonItems.ReceiptVersion)
                {
                    case 1:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT;
                            break;
                        }
                    case 2:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V2;
                            break;
                        }
                    case 4:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
                            proAlloc.ReceiptType = ReceiptType.OUTWARD_MEDDEPT;
                            break;
                        }
                    default:
                        {
                            proAlloc.eItem = ReportName.INPATIENT_SETTLEMENT_V4;
                            proAlloc.ReceiptType = ReceiptType.OUTWARD_MEDDEPT;
                            break;
                        }
                }

                proAlloc.flag = 1; //0: In Quyết Toán, 1: In Hóa Đơn Bán Lẻ Của Khoa Dược.
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private bool IsHIPatient = false;

        private ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsList;

        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetailsSum;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsListSum
        {
            get { return _RefGenMedProductDetailsSum; }
            set
            {
                if (_RefGenMedProductDetailsSum != value)
                    _RefGenMedProductDetailsSum = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailsListSum);
            }
        }

        private ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsTemp;

        private RefGenMedProductDetails _SelectedSellVisitor;
        public RefGenMedProductDetails SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDisplays;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDisplays
        {
            get
            {
                return _RefGenMedProductDisplays;
            }
            set
            {
                if (_RefGenMedProductDisplays != value)
                {
                    _RefGenMedProductDisplays = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDisplays);
                }
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
                RefGenMedProductDisplays.PageIndex = 0;
                if (mIsInputTemp)
                {
                    SearchRefGenMedProductDetails_HangKyGoi(e.Parameter, false, RefGenMedProductDisplays.PageSize, RefGenMedProductDisplays.PageIndex);
                }
                else
                {
                    SearchRefGenMedProductDetails(e.Parameter, false);
                }
            }
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as RefGenMedProductDetails;
            }
        }

        private void ListDisplayAutoComplete(int TotalItemCount)
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code, hd.BidCode } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty,
                          BidCode = hdgroup.Key.BidCode
                      };
            //KMx: for chạy lâu hơn foreach (đã test trên 1000 items) (07/07/2014 11:39).
            //for (int i = 0; i < hhh.Count(); i++)
            //{
            //    RefGenMedProductDetails item = new RefGenMedProductDetails();
            //    item.GenMedProductID = hhh.ToList()[i].GenMedProductID;
            //    item.BrandName = hhh.ToList()[i].BrandName;
            //    item.SelectedUnit = new RefUnit();
            //    item.SelectedUnit.UnitName = hhh.ToList()[i].UnitName;
            //    item.Code = hhh.ToList()[i].Code;
            //    item.Remaining = hhh.ToList()[i].Remaining;
            //    item.RequestQty = hhh.ToList()[i].Qty;
            //    item.TypID = (long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI;
            //    RefGenMedProductDetailsListSum.Add(item);
            //}

            //KMx: Phải new rồi mới add. Nếu clear rồi add thì bị chậm (05/07/2014 16:55).
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var i in hhh)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = i.GenMedProductID;
                item.BrandName = i.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = i.UnitName;
                item.Code = i.Code;
                item.Remaining = i.Remaining;
                item.RequestQty = i.Qty;
                item.BidCode = i.BidCode;
                item.TypID = (long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI;
                RefGenMedProductDetailsListSum.Add(item);
            }

            if (IsCode.GetValueOrDefault())
            {
                if (RefGenMedProductDetailsListSum != null && RefGenMedProductDetailsListSum.Count > 0)
                {
                    SelectedSellVisitor = RefGenMedProductDetailsListSum.ToList()[0];
                }
                else
                {
                    SelectedSellVisitor = null;
                    MessageBox.Show(eHCMSResources.Z1407_G1_KgTimThayMatHgNay);
                }
            }
            else
            {
                if (au != null)
                {
                    //KMx: Khi tìm thuốc với tên chỉ có 1 ký tự. VD: "a". Thì kết quả trả về rất nhiều và lâu, khi kết quả chưa trả về kịp mà người dùng xóa và đánh lại chữ "b" thì sẽ bị đứng chương trình.
                    //Cách giải quyết: Làm phân trang, thời gian trả kết quả từ server về client sẽ nhanh hơn (03/07/2014 15:15).

                    //KMx: Lưu ý: New rồi add sẽ nhanh hơn Clear rồi add (khi PageSize > 200). Đối với PageSize nhỏ thì không sao.(07/07/2014 14:24).
                    //Nhưng khi new PagedSortableCollectionView thì PageSize và PageIndex sẽ bị xóa.
                    //Cho nên đối với PageSize nhỏ thì không cần New, chỉ cần Clear là được rồi.
                    RefGenMedProductDisplays.Clear();

                    RefGenMedProductDisplays.SourceCollection = RefGenMedProductDetailsListSum;

                    RefGenMedProductDisplays.TotalItemCount = TotalItemCount;

                    //au.ItemsSource = RefGenMedProductDetailsListSum;
                    au.ItemsSource = RefGenMedProductDisplays;
                    au.PopulateComplete();
                }
            }
        }

        private void SearchRefGenMedProductDetails(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            long? RequestID = null;
            long? RefGenDrugCatID_1 = null;

            RequestID = SelectedOutInvoice.ReqDrugInClinicDeptID;

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                RefGenDrugCatID_1 = SelectedOutInvoice.RefGenDrugCatID_1;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(IsCost, Name, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(asyncResult);
                            RefGenMedProductDetailsList.Clear();
                            RefGenMedProductDetailsListSum.Clear();
                            RefGenMedProductDetailsTemp.Clear();
                            //foreach (RefGenMedProductDetails s in results)
                            //{
                            //    RefGenMedProductDetailsTemp.Add(s);
                            //}
                            RefGenMedProductDetailsTemp = results.ToObservableCollection();
                            if (OutwardDrugMedDeptsCopy != null && OutwardDrugMedDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugMedDept d in OutwardDrugMedDeptsCopy)
                                {
                                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                                    if (value.Count() > 0)
                                    {
                                        foreach (RefGenMedProductDetails s in value.ToList())
                                        {
                                            s.Remaining = s.Remaining + d.OutQuantityOld;
                                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                                        }
                                    }
                                    else
                                    {
                                        RefGenMedProductDetails p = d.RefGenericDrugDetail;
                                        p.Remaining = d.OutQuantity;
                                        p.RemainingFirst = d.OutQuantity;
                                        p.InBatchNumber = d.InBatchNumber;
                                        p.OutPrice = d.OutPrice;
                                        p.InID = Convert.ToInt64(d.InID);
                                        p.STT = d.STT;
                                        RefGenMedProductDetailsTemp.Add(p);
                                        // d = null;
                                    }
                                }
                            }
                            foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
                            {
                                if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
                                {
                                    foreach (OutwardDrugMedDept d in SelectedOutInvoice.OutwardDrugMedDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            //KMx: Hàm SearchRefGenMedProductDetails() chỉ sử dụng bên Xuất Nội Bộ, còn Xuất Ký Gửi dùng hàm khác nên truyền khống -1 vào hàm ListDisplayAutoComplete() (03/07/2014 18:07).
                            ListDisplayAutoComplete(-1);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //20180710 TBL: Comment lai vi khi ra ket qua no mat focus vao AutoCompleteBox
                            //if (IsCode.GetValueOrDefault())
                            //{
                            //    isLoadingDetail = false;
                            //    if (AxQty != null)
                            //    {
                            //        AxQty.Focus();
                            //    }
                            //}
                            //else
                            //{
                            //    if (au != null)
                            //    {
                            //        au.Focus();
                            //    }
                            //}
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckValidDrugAuto(RefGenMedProductDetails temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrugMedDept p)
        {
            bool kq = false;

            //KMx: If này trong hàm ReCountQtyRequest. Nhưng vì không dùng hàm ReCountQtyRequest() nữa, nên copy cái if bỏ vào đây.
            if (SelectedOutInvoice.OutwardDrugMedDepts == null)
            {
                SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
            }

            foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
            {
                if (p.GenMedProductID == p1.GenMedProductID)
                {
                    p.ObjSupplierList = p1.ObjSupplierList;

                    SetDefaultSupplier(p);


                    //KMx: Cho phép 1 dòng có SL lớn hơn 1 (27/05/2015 09:21).
                    ////KMx: Nếu số lượng lớn hơn 1 thì tách ra nhiều dòng, không để chung 1 dòng.
                    ////Nếu không sẽ bị lỗi khi sáp nhập hàng: Số lượng sáp nhập ít hơn số lượng đã xuất thì phiếu nhập, xuất cũ tự tách ra thành 2 phiếu. Nhưng phiếu xuất của kho phòng không tách ra .
                    ////Nếu muốn sử dụng lại cách cũ thì gọi lại hàm CheckBatchNumberExists() cũ (10/02/2014 14:43).
                    //for (int i = 0; i < p.OutQuantity; i++)
                    //{
                    //    OutwardDrugMedDept SplitItem = new OutwardDrugMedDept();
                    //    SplitItem = p.DeepCopy();
                    //    SplitItem.OutQuantity = 1;
                    //    SelectedOutInvoice.OutwardDrugMedDepts.Add(SplitItem);
                    //}

                    SelectedOutInvoice.OutwardDrugMedDepts.Add(p);

                    kq = true;
                    break;
                }
            }

            if (!kq)
            {
                SupplierGenMedProduct_LoadDrugIDNotPaging(p, p.InwardDrugMedDept.GenMedProductID.Value);
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

        //private void CheckBatchNumberExists(OutwardDrugMedDept p)
        //{
        //    bool kq = false;

        //    //KMx: If này trong hàm ReCountQtyRequest. Nhưng vì không dùng hàm ReCountQtyRequest() nữa, nên copy cái if bỏ vào đây.
        //    if (SelectedOutInvoice.OutwardDrugMedDepts == null)
        //    {
        //        SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
        //    }

        //    foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
        //    {
        //        if (p.GenMedProductID == p1.GenMedProductID)
        //        {
        //            p1.OutQuantity += p.OutQuantity;

        //            //KMx: Nếu Remaining > 0 thì cảnh báo rằng trong kho còn thuốc (08/07/2014 16:20).
        //            p1.Remaining = p.Remaining;
        //            kq = true;
        //            break;
        //        }
        //    }

        //    if (!kq)
        //    {
        //        //KMx: Chuyển code bên dưới ra hàm  ChooseBatchNumber_HangKyGoi() để set NormalPrice, HIPatientPrice, HIAllowedPrice (30/09/2014 11:48).
        //        //if (p.InwardDrugMedDept == null)
        //        //{
        //        //    p.InwardDrugMedDept = new InwardDrugMedDept();
        //        //    p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
        //        //    p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
        //        //}
        //        //p.InvoicePrice = p.OutPrice;
        //        //p.InwardDrugMedDept.NormalPrice = p.OutPrice;
        //        //p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
        //        //p.InwardDrugMedDept.HIAllowedPrice = p.HIAllowedPrice;

        //        SupplierGenMedProduct_LoadDrugIDNotPaging(p, p.InwardDrugMedDept.GenMedProductID.Value);

        //    }

        //    txt = "";
        //    SelectedSellVisitor = null;
        //    if (IsCode.GetValueOrDefault())
        //    {
        //        if (tbx != null)
        //        {
        //            tbx.Text = "";
        //            tbx.Focus();
        //        }

        //    }
        //    else
        //    {
        //        if (au != null)
        //        {
        //            au.Text = "";
        //            au.Focus();
        //        }
        //    }
        //}


        //KMx: Xem lại hàm AddListOutwardDrugMedDept() cũ, trước khi muốn sửa hàm này (08/07/2014 14:21).
        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
        {
            if (value == null)
            {
                MessageBox.Show(eHCMSResources.K0408_G1_ChonThuocCanXuat);
                return;
            }

            //20201229 QTD: Thêm điều kiện không cho xuất số thập phân
            if (value.RequiredNumber <= 0 && !int.TryParse(value.RequiredNumber.ToString(), out int intOutPut))
            {
                //MessageBox.Show(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
                MessageBox.Show("Số lượng xuất phải là số nguyên và lớn hơn 0!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ChooseBatchNumber_HangKyGoi(value);

        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            if (CheckSelectKhoNguon() == false)
                return;

            //ReCountQtyRequest();
            AddListOutwardDrugMedDept(SelectedSellVisitor);
        }

        private bool CheckSelectKhoNguon()
        {
            if (StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K1973_G1_ChonKho);
                return false;
            }
            return true;
        }

        private void SetDefaultSupplier(OutwardDrugMedDept item)
        {
            if (item == null || item.ObjSupplierList == null)
            {
                return;
            }

            if (item.ObjSupplierList.Count != 1)
            {
                item.ObjSupplierID = new DrugDeptSupplier();
                item.ObjSupplierID.SupplierID = 0;
                item.ObjSupplierID.SupplierName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0342_G1_Msg_InfoChonNCC);
            }
            else
            {
                item.ObjSupplierID = item.ObjSupplierList.FirstOrDefault();
            }
        }


        //Danh Sách NCC của GenMedProductID
        public void SupplierGenMedProduct_LoadDrugIDNotPaging(OutwardDrugMedDept p, long GenMedProductID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //ko dung indicator vi lam mat focus
            //isLoadingSupplier = true;
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptSupplier_LoadDrugIDNotPaging(GenMedProductID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDrugDeptSupplier_LoadDrugIDNotPaging(asyncResult);
                            p.ObjSupplierList = results.ToObservableCollection();

                            //KMx: Nếu như thuốc đó chỉ có 1 nhà cung cấp thì chọn NCC đó làm mặc định luôn, không bắt người dùng phải chọn (02/07/2014 09:44).
                            SetDefaultSupplier(p);

                            //KMx: Cho phép 1 dòng có SL lớn hơn 1 (27/05/2015 09:21).
                            ////KMx: Nếu số lượng lớn hơn 1 thì tách ra nhiều dòng, không để chung 1 dòng.
                            ////Nếu không sẽ bị lỗi khi sáp nhập hàng: Số lượng sáp nhập ít hơn số lượng đã xuất thì phiếu nhập, xuất cũ tự tách ra thành 2 phiếu. Nhưng phiếu xuất của kho phòng không tách ra.
                            ////Nếu muốn sử dụng lại cách cũ thì bỏ vòng lặp for và sử dụng lại câu Add bên dưới (10/02/2014 14:43). 
                            //for (int i = 0; i < p.OutQuantity; i++)
                            //{
                            //    OutwardDrugMedDept SplitItem = new OutwardDrugMedDept();
                            //    SplitItem = p.DeepCopy();
                            //    SplitItem.OutQuantity = 1;
                            //    SelectedOutInvoice.OutwardDrugMedDepts.Add(SplitItem);
                            //}

                            SelectedOutInvoice.OutwardDrugMedDepts.Add(p);

                            SumTotalPrice();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSupplier = false;
                            //Globals.IsBusy = false;
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
                                    au.Text = "";
                                    au.Focus();
                                }
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }
        //Danh Sách NCC của GenMedProductID



        #region Properties member
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListTemp;
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListShow;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptListByGenMedProductID;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptListByGenMedProductIDFirst;

        private OutwardDrugMedDept _SelectedOutwardDrugMedDept;
        public OutwardDrugMedDept SelectedOutwardDrugMedDept
        {
            get { return _SelectedOutwardDrugMedDept; }
            set
            {
                if (_SelectedOutwardDrugMedDept != value)
                    _SelectedOutwardDrugMedDept = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrugMedDept);
            }
        }
        #endregion

        private void RefGenMedProductDetailsBatchNumber(long GenMedProductID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginspGetInBatchNumberAllDrugDept_ByGenMedProductID(GenMedProductID, V_MedProductType, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndspGetInBatchNumberAllDrugDept_ByGenMedProductID(asyncResult);
                                BatchNumberListTemp = results.ToObservableCollection();
                                if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 1)
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
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                isLoadingDetail = false;
                                // Globals.IsBusy = false;
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

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long GenMedProductID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugMedDeptListByGenMedProductID = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                if (OutwardDrugMedDeptsCopy != null)
                {
                    OutwardDrugMedDeptListByGenMedProductIDFirst = OutwardDrugMedDeptsCopy.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                }
                RefGenMedProductDetailsBatchNumber(GenMedProductID);
            }
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugMedDeptListByGenMedProductIDFirst != null)
            {
                foreach (OutwardDrugMedDept d in OutwardDrugMedDeptListByGenMedProductIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (RefGenMedProductDetails s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        RefGenMedProductDetails p = d.RefGenericDrugDetail;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.OutPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (RefGenMedProductDetails s in BatchNumberListTemp)
            {
                if (OutwardDrugMedDeptListByGenMedProductID.Count > 0)
                {
                    foreach (OutwardDrugMedDept d in OutwardDrugMedDeptListByGenMedProductID)
                    {
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrugMedDept.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 1)
            {
                Action<IChooseBatchNumber> onInitDlg = delegate (IChooseBatchNumber proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrugMedDept.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugMedDeptListByGenMedProductID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugMedDeptListByGenMedProductID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumber>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }

        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrugMedDept p = SelectedOutwardDrugMedDept.DeepCopy();

            //KMx: Thêm ngày 26/09/2014 09:52.
            if (SelectedOutInvoice.OutwardDrugMedDepts_Delete == null)
            {
                SelectedOutInvoice.OutwardDrugMedDepts_Delete = new ObservableCollection<OutwardDrugMedDept>();
            }

            if (SelectedOutwardDrugMedDept.OutID > 0)
            {
                SelectedOutInvoice.OutwardDrugMedDepts_Delete.Add(SelectedOutwardDrugMedDept);
            }

            SelectedOutInvoice.OutwardDrugMedDepts.Remove(SelectedOutwardDrugMedDept);
            foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    item.RequestQty = item.RequestQty + p.RequestQty;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugMedDepts = SelectedOutInvoice.OutwardDrugMedDepts.ToObservableCollection();
            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrugMedDept != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                DeleteInvoiceDrugInObject();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0915_G1_Msg_InfoPhChiXem, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                RefeshData();
            }
        }

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardClinicDept Request = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (Request != null)
                {
                    ClearData();

                    //KMx: 2 dòng này phải để sau dòng assign SelectedOutInvoice.V_OutputTo, nếu không SelectedOutInvoice.ReqNumCode sẽ bị mất giá trị.
                    //Vì khi assign V_OutputTo thì XuatCho_SelectionChanged() sẽ được gọi và ReqNumCode bị thay đổi (27/06/2014 15:03).
                    //SelectedOutInvoice.ReqDrugInClinicDeptID = Request.ReqDrugInClinicDeptID;
                    //SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;

                    if (Request.InDeptStoreID.GetValueOrDefault(-1) > 0)
                    {
                        SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.KHO_KHAC;
                        SelectedOutInvoice.OutputToID = Request.InDeptStoreID;
                        if (Request.InDeptStoreObject != null)
                        {
                            SelectedOutInvoice.FullName = Request.InDeptStoreObject.swhlName;
                            SelectedOutInvoice.Address = "";
                            SelectedOutInvoice.NumberPhone = "";
                        }
                    }
                    else
                    {
                        SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.BACSI;
                        SelectedOutInvoice.OutputToID = Request.StaffID;
                        if (Request.SelectedStaff != null)
                        {
                            SelectedOutInvoice.FullName = Request.SelectedStaff.FullName;
                            SelectedOutInvoice.Address = Request.SelectedStaff.SStreetAddress;
                            SelectedOutInvoice.NumberPhone = Request.SelectedStaff.SPhoneNumber;
                        }
                    }

                    SelectedOutInvoice.ReqDrugInClinicDeptID = Request.ReqDrugInClinicDeptID;
                    SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;

                    ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, Request.RefGenDrugCatID_1);
                    SelectedOutInvoice.RefGenDrugCatID_1 = Request.RefGenDrugCatID_1;
                    spGetInBatchNumberAndPrice_ByRequestID(Request.ReqDrugInClinicDeptID, StoreID);
                }
            }
        }

        #endregion

        //public void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID)
        //{
        //    this.ShowBusyIndicator();

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyMedDeptServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetRequestDrugDeptList_ForDepositGoods(RequestID, StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndGetRequestDrugDeptList_ForDepositGoods(asyncResult);

        //                    SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();

        //                    //KMx: Nếu số lượng lớn hơn 1 thì tách ra nhiều dòng, không để chung 1 dòng.
        //                    //Nếu không sẽ bị lỗi khi sáp nhập hàng: Số lượng sáp nhập ít hơn số lượng đã xuất thì phiếu nhập, xuất cũ tự tách ra thành 2 phiếu. Nhưng phiếu xuất của kho phòng không tách ra .
        //                    //Nếu muốn sử dụng lại cách cũ thì gọi lại hàm spGetInBatchNumberAndPrice_ByRequestID() cũ (10/02/2014 14:43).
        //                    foreach (OutwardDrugMedDept item in results)
        //                    {
        //                        SetDefaultSupplier(item);

        //                        for (int i = 0; i < item.OutQuantity; i++)
        //                        {
        //                            OutwardDrugMedDept SplitItem = new OutwardDrugMedDept();
        //                            SplitItem = item.DeepCopy();
        //                            SplitItem.OutQuantity = 1;
        //                            SelectedOutInvoice.OutwardDrugMedDepts.Add(SplitItem);
        //                        }
        //                    }

        //                    OutwardDrugMedDeptsCopy = null;
        //                    SumTotalPrice();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        //KMx: Muốn tách ra từng dòng (SL 1) thì dùng lại hàm cũ spGetInBatchNumberAndPrice_ByRequestID (16/01/2016 17:16).
        public void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID)
        {
            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRequestDrugDeptList_ForDepositGoods(RequestID, StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRequestDrugDeptList_ForDepositGoods(asyncResult);
                            SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                            if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
                            {
                                foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
                                {
                                    SetDefaultSupplier(item);
                                }
                            }
                            OutwardDrugMedDeptsCopy = null;
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

            });

            t.Start();
        }

        //KMx: Hàm này chỉ thích hợp với xuất bình thường, không thích hợp với xuất ký gởi, vì không load nhà cung cấp (02/07/2014 18:25).
        //public void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID)
        //{
        //    //isLoadingGetID = true;
        //    // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    this.ShowBusyIndicator();

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyMedDeptServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginspGetInBatchNumberAndPrice_ListForRequest(IsCost, RequestID, StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndspGetInBatchNumberAndPrice_ListForRequest(asyncResult);
        //                    SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();

        //                    //KMx:Load nhà cung cấp.
        //                    if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
        //                    {
        //                        foreach (OutwardDrugMedDept p in SelectedOutInvoice.OutwardDrugMedDepts)
        //                        {
        //                            SupplierGenMedProduct_LoadDrugIDNotPaging(p, p.GenMedProductID.Value, true);
        //                        }
        //                    }

        //                    OutwardDrugMedDeptsCopy = null;
        //                    SumTotalPrice();
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //isLoadingGetID = false;
        //                    //Globals.IsBusy = false;
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        public void btnFindRequest(object sender, RoutedEventArgs e)
        {
            //IsChanged = true;
            Action<IRequestSearch> onInitDlg = delegate (IRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria.IsApproved = true;

                //KMx: Bên xuất hàng tìm phiếu yêu cầu theo ngày duyệt phiếu, không phải ngày tạo phiếu (18/12/2014 09:53).
                proAlloc.SearchCriteria.FindByApprovedDate = true;

                //proAlloc.SearchCriteria.DaNhanHang = true;
                proAlloc.SearchCriteria.DaNhanHang = null;
                proAlloc.SearchCriteria.FromDate = Globals.ServerDate.Value.AddDays(-1);
                proAlloc.SearchCriteria.ToDate = Globals.ServerDate.Value;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
            };
            GlobalsNAV.ShowDialog<IRequestSearch>(onInitDlg);
        }

        private bool CheckData()
        {
            if (SelectedOutInvoice == null)
            {
                return false;
            }

            if (SelectedOutInvoice.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.A0953_G1_Msg_InfoKhTheLuu2, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(eHCMSResources.A0944_G1_Msg_InfoKhTheLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (SelectedOutInvoice.OutDate == null || SelectedOutInvoice.OutDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.A0863_G1_Msg_InfoNgXuatKhHopLe4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                SelectedOutInvoice.StoreID = StoreID;
            }

            if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC && SelectedOutInvoice.StoreID == SelectedOutInvoice.OutputToID)
            {
                Globals.ShowMessage(eHCMSResources.A0618_G1_Msg_InfoKhoXuatKhacKhoNhan, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            if (SelectedOutInvoice.OutputToID == null || SelectedOutInvoice.OutputToID <= 0)
            {
                if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                {
                    Globals.ShowMessage(eHCMSResources.K0330_G1_ChonKhoNhan, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                {
                    Globals.ShowMessage(eHCMSResources.K0296_G1_ChonBVNhan, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI)
                {
                    if (string.IsNullOrEmpty(SelectedOutInvoice.FullName))
                    {
                        Globals.ShowMessage(eHCMSResources.K0448_G1_NhapTenNguoiNhan, eHCMSResources.G0442_G1_TBao);
                        return false;
                    }
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN)
                {
                    Globals.ShowMessage(eHCMSResources.K0284_G1_ChonBNNhan, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI)
                {
                    Globals.ShowMessage(eHCMSResources.K0375_G1_ChonNguoiNhan, eHCMSResources.G0442_G1_TBao);
                    return false;
                }
            }

            if (SelectedOutInvoice.OutwardDrugMedDepts == null || SelectedOutInvoice.OutwardDrugMedDepts.Count == 0)
            {
                Globals.ShowMessage(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            else
            {
                if (SelectedOutInvoice.IsInternal)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED;
                }
                else
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                }

                if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        OutwardDrugMedDept item = SelectedOutInvoice.OutwardDrugMedDepts[i];
                        int intOutput = 0;

                        //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                        //if (item.OutPrice <= 0)
                        //{
                        //    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + ": Giá bán phải > 0", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        //    return false;
                        //}

                        //if (item.RefGenericDrugDetail != null && item.OutQuantity <= 0)
                        if (item.OutQuantity <= 0 || !Int32.TryParse(item.OutQuantity.ToString(), out intOutput))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1400_G1_SLgXuatLaSoNguyen, (i + 1).ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        if (item.ObjSupplierID == null || item.ObjSupplierID.SupplierID <= 0)
                        {
                            MessageBox.Show(eHCMSResources.A0396_G1_Msg_InfoChuaChonNCCTaiDong + (i + 1).ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        if (string.IsNullOrEmpty(item.BidCode) && item.InwardDrugMedDept.HIAllowedPrice > 0)
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z2823_G1_KhongCoTTThauGiaBHQD, (i + 1).ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return false;
                        }
                        //else/*Cho chắc*/
                        //{
                        //    SelectedOutInvoice.OutwardDrugMedDepts[i].SupplierID = SelectedOutInvoice.OutwardDrugMedDepts[i].ObjSupplierID.SupplierID;
                        //}
                    }
                }
            }
            return true;
        }

        //public void btnSave()
        //{
        //    if (CheckData())
        //    {
        //        SelectedOutInvoice.V_MedProductType = V_MedProductType;
        //        OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(SelectedOutInvoice, false);
        //    }
        //}

        private bool CheckGoodsAvailable(out string msg)
        {
            StringBuilder sb = new StringBuilder();

            msg = "";

            bool Result = true;

            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugMedDepts == null || SelectedOutInvoice.OutwardDrugMedDepts.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0927_G1_Msg_InfoPhKhCoData, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }

            if (SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
            {
                int STT = 0;

                //KMx: Cho phép 1 dòng có SL lớn hơn 1 (27/05/2015 09:21).
                //if (SelectedOutInvoice.OutwardDrugMedDepts.Any(x => x.OutQuantity > 1))
                //{
                //    MessageBox.Show(eHCMSResources.A0971_G1_Msg_InfoSLgKhLonHon1, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //    return false;
                //}

                List<long> GenMedProductIDList = new List<long>();

                foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    if (item.Remaining <= 0)
                    {
                        continue;
                    }

                    //KMx: Nếu loại hàng đó có nằm trong danh sách thông báo rồi thì thôi.
                    if (GenMedProductIDList.Any(x => x == item.GenMedProductID))
                    {
                        continue;
                    }

                    GenMedProductIDList.Add(item.GenMedProductID.GetValueOrDefault());

                    STT++;
                    sb.AppendLine(STT + ". " + item.RefGenericDrugDetail.BrandName.Trim() + ": " + item.Remaining + " " + item.RefGenericDrugDetail.SelectedUnit.UnitName);
                    Result = false;
                }
            }

            if (!Result)
            {
                msg = string.Format(eHCMSResources.Z1476_G1_TiepTucXuatHgKyGui, sb.ToString());
            }
            else
            {
                msg = sb.ToString();
            }

            return Result;
        }

        WarningWithConfirmMsgBoxTask warnGoodsAvailable = null;

        public IEnumerator<IResult> CoroutineOutwardDrugMedDeptInvoice_Add()
        {
            string msg = "";
            if (!CheckGoodsAvailable(out msg))
            {
                if (string.IsNullOrEmpty(msg))
                {
                    yield break;
                }
                warnGoodsAvailable = new WarningWithConfirmMsgBoxTask(msg, eHCMSResources.Z0627_G1_TiepTucLuu);
                yield return warnGoodsAvailable;
                if (!warnGoodsAvailable.IsAccept)
                {
                    warnGoodsAvailable = null;
                    yield break;
                }
            }
            warnGoodsAvailable = null;

            OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(SelectedOutInvoice, false);
            yield break;
        }

        public void btnSave()
        {
            if (CheckData())
            {
                SelectedOutInvoice.V_MedProductType = V_MedProductType;
                Coroutine.BeginExecute(CoroutineOutwardDrugMedDeptInvoice_Add());
                
            }
        }

        public void btnMoney()
        {
            bThuTien = true;
            ShowFormCountMoney();
        }

        public void btnNew()
        {
            RefeshData();
        }


        public void btnRequireUnlockInvoice()
        {
            this.ShowBusyIndicator();

            if (SelectedOutInvoice == null)
            {
                this.HideBusyIndicator();
                MessageBox.Show(eHCMSResources.A0664_G1_Msg_InfoKhCoPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRequireUnlockOutMedDeptInvoice(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndRequireUnlockOutMedDeptInvoice(asyncResult);
                            if (result)
                            {
                                MessageBox.Show(eHCMSResources.K0556_G1_YCMoKhoaCNhatPXOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0558_G1_YCMoKhoaCNhatPXFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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


        private void DeepCopyOutwardDrugMedDept()
        {
            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                OutwardDrugMedDeptsCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
            }
            else
            {
                OutwardDrugMedDeptsCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }

        }

        private int icount = 0;
        public void SelectedByOutPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedOutInvoice != null && icount > 0)
            {
                GetIsCost();

                if (SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.GIATHONGTHUONG)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.NormalPrice;
                        }
                    }
                }
                else if (SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                        {
                            SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.InCost;
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
            icount++;

        }

        private void GetIsCost()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPrice.GIAVON)
            {
                IsCost = true;
            }
            else
            {
                IsCost = false;
            }
        }

        //dung lai from tim kiem cua Demage
        #region IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent> Members

        public void Handle(DrugDeptCloseSearchOutMedDeptInvoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugMedDeptInvoice temp = message.SelectedOutMedDeptInvoice as OutwardDrugMedDeptInvoice;
                if (temp != null)
                {
                    ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, temp.RefGenDrugCatID_1);
                    SelectedOutInvoice = temp;
                    OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                }
            }
        }

        #endregion


        private bool bThuTien = true;
        private void ShowFormCountMoney()
        {
            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
            {
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -TotalPrice.DeepCopy();
                }
                else
                {
                    proAlloc.TotalPayForSelectedItem = TotalPrice.DeepCopy();
                    proAlloc.TotalPaySuggested = TotalPrice.DeepCopy();
                }
                proAlloc.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        #region IHandle<DrugDeptChooseBatchNumberEvent> Members

        public void Handle(DrugDeptChooseBatchNumberEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugMedDept.RefGenericDrugDetail = message.BatchNumberSelected;
                SelectedOutwardDrugMedDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugMedDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugMedDept.InID = message.BatchNumberSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.NormalPrice;
                }
                SelectedOutwardDrugMedDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<DrugDeptChooseBatchNumberResetQtyEvent> Members

        public void Handle(DrugDeptChooseBatchNumberResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugMedDept.RefGenericDrugDetail = message.BatchNumberSelected;
                SelectedOutwardDrugMedDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugMedDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugMedDept.InID = message.BatchNumberSelected.InID;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.NormalPrice;
                }
                SelectedOutwardDrugMedDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SelectedOutwardDrugMedDept.OutQuantity = message.BatchNumberSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        #region RefOutputType member

        private ObservableCollection<RefOutputType> _RefOutputTypes;
        public ObservableCollection<RefOutputType> RefOutputTypes
        {
            get { return _RefOutputTypes; }
            set
            {
                if (_RefOutputTypes != value)
                {
                    _RefOutputTypes = value;
                    NotifyOfPropertyChange(() => RefOutputTypes);
                }
            }
        }

        public void LoadRefOutputType()
        {
            if (mIsInputTemp)
            {
                RefOutputType item = new RefOutputType();
                item.TypID = (long)AllLookupValues.RefOutputType.XUAT_HANGKYGOI;
                item.TypName = eHCMSResources.G2886_G1_XuatHgKG;

                if (RefOutputTypes == null) RefOutputTypes = new ObservableCollection<RefOutputType>();

                RefOutputTypes.Add(item);

                SelectedOutInvoice.TypID = item.TypID;

                return;
            }

            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRefOutputType_Get(false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndRefOutputType_Get(asyncResult);
                            RefOutputTypes = results.ToObservableCollection();
                            SetDefaultOutputType();
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

        private void SetDefaultOutputType()
        {
            if (RefOutputTypes != null && SelectedOutInvoice != null)
            {
                SelectedOutInvoice.TypID = RefOutputTypes.FirstOrDefault().TypID;
            }
        }

        #endregion

        #region Lookup OutputTo member

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


        private ObservableCollection<Lookup> _OutputTos;
        public ObservableCollection<Lookup> OutputTos
        {
            get { return _OutputTos; }
            set
            {
                if (_OutputTos != value)
                {
                    _OutputTos = value;
                    NotifyOfPropertyChange(() => OutputTos);
                }
            }
        }

        private IEnumerator<IResult> GetLookupOutputTo()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_OutputTo, false, false);
            yield return paymentTypeTask;
            OutputTos = paymentTypeTask.LookupList;
            SetDefaultOutputTo();
            yield break;
        }

        private IEnumerator<IResult> DoGetByOutPriceLookups()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_ByOutPriceMedDept, false, false);
            yield return paymentTypeTask;
            ByOutPriceLookups = paymentTypeTask.LookupList;
            yield break;
        }


        private void SetDefaultOutputTo()
        {
            if (OutputTos != null && SelectedOutInvoice != null)
            {
                SelectedOutInvoice.V_OutputTo = OutputTos.FirstOrDefault().LookupID;
            }
        }
        #endregion

        private void ClearSelectedOutInvoice()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.ReqDrugInClinicDeptID.GetValueOrDefault(-1) > 0 && !String.IsNullOrEmpty(SelectedOutInvoice.ReqNumCode))
            {
                SelectedOutInvoice.OutwardDrugMedDepts.Clear();
            }

            SelectedOutInvoice.OutputToID = 0;
            SelectedOutInvoice.FullName = "";
            SelectedOutInvoice.Address = "";
            SelectedOutInvoice.NumberPhone = "";

            SelectedOutInvoice.ReqDrugInClinicDeptID = 0;
            SelectedOutInvoice.ReqNumCode = "";
        }

        public void XuatCho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //KMx: Copy từ Xuất nội bộ (22/01/2015 10:19).
            if (SelectedOutInvoice != null && SelectedOutInvoice.outiID == 0 && (sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                ClearSelectedOutInvoice();
            }

            //if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaid && (sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            //{
            //    SelectedOutInvoice.OutputToID = 0;
            //    SelectedOutInvoice.FullName = "";
            //    SelectedOutInvoice.Address = "";
            //    SelectedOutInvoice.NumberPhone = "";

            //    SelectedOutInvoice.ReqDrugInClinicDeptID = 0;
            //    SelectedOutInvoice.ReqNumCode = "";

            //    if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            //    {
            //        SelectedOutInvoice.OutwardDrugMedDepts.Clear();
            //    }
            //}
        }

        public void btnChoose()
        {
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                {
                    IDrugDeptStorage DialogView = Globals.GetViewModel<IDrugDeptStorage>();
                    DialogView.IsChildWindow = true;
                    DialogView.V_MedProductType = V_MedProductType;
                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                {
                    IDrugDeptHospitals DialogView = Globals.GetViewModel<IDrugDeptHospitals>();
                    DialogView.IsChildWindow = true;
                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN)
                {
                    GlobalsNAV.ShowDialog<IFindPatient>();
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI)
                {
                    IDrugDeptStaffs DialogView = Globals.GetViewModel<IDrugDeptStaffs>();
                    DialogView.IsChildWindow = true;
                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                }
            }
        }

        #region IHandle<DrugDeptCloseSearchStorageEvent> Members

        public void Handle(DrugDeptCloseSearchStorageEvent message)
        {
            if (this.IsActive && message != null)
            {
                RefStorageWarehouseLocation sto = message.SelectedStorage as RefStorageWarehouseLocation;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.StoreID;
                    SelectedOutInvoice.FullName = sto.swhlName;
                    SelectedOutInvoice.Address = "";
                    SelectedOutInvoice.NumberPhone = "";
                }
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchHospitalEvent> Members

        public void Handle(DrugDeptCloseSearchHospitalEvent message)
        {
            if (this.IsActive && message != null)
            {
                Hospital sto = message.SelectedHospital as Hospital;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.HosID;
                    SelectedOutInvoice.FullName = sto.HosName;
                    SelectedOutInvoice.Address = sto.HosAddress;
                    SelectedOutInvoice.NumberPhone = sto.HosPhone;
                }
            }
        }

        #endregion

        #region IHandle<ItemSelected<Patient>> Members

        public void Handle(ItemSelected<Patient> message)
        {
            if (this.IsActive && message != null)
            {
                Patient sto = message.Item as Patient;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.PatientID;
                    SelectedOutInvoice.FullName = sto.FullName;
                    SelectedOutInvoice.Address = sto.PatientStreetAddress;
                    SelectedOutInvoice.NumberPhone = sto.PatientPhoneNumber;
                }
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchStaffEvent> Members

        public void Handle(DrugDeptCloseSearchStaffEvent message)
        {
            if (this.IsActive && message != null)
            {
                Staff sto = message.SelectedStaff as Staff;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.StaffID;
                    //if (sto.RefStaffCategory != null)
                    //{
                    //    SelectedOutInvoice.FullName = sto.RefStaffCategory.StaffCatgDescription + " " + sto.FullName;
                    //}
                    //else
                    //{
                        SelectedOutInvoice.FullName = sto.FullName;
                    //}
                    SelectedOutInvoice.Address = sto.SStreetAddress;
                    SelectedOutInvoice.NumberPhone = sto.SPhoneNumber;
                }
            }
        }

        #endregion

        private void ChangeValue(long value1, long value2)
        {
            if (value1 != value2)
            {
                flag = false;
            }
            else
            {
                flag = true;
            }
        }

        private bool flag = true;
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flag)
            {
                if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugMedDepts != null)
                {
                    SelectedOutInvoice.OutwardDrugMedDepts.Clear();
                }
            }
            flag = true;
        }

        public void LoaiXuat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private bool? IsCode = false;
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    if (mIsInputTemp == false)
                    {
                        SearchRefGenMedProductDetails(Code, true);
                    }
                    else
                    {
                        SearchRefGenMedProductDetails_HangKyGoi(Code, true, RefGenMedProductDisplays.PageSize, RefGenMedProductDisplays.PageIndex);
                    }
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
                    if (mIsInputTemp == false)
                    {
                        SearchRefGenMedProductDetails((sender as TextBox).Text, true);
                    }
                    else
                    {
                        SearchRefGenMedProductDetails_HangKyGoi((sender as TextBox).Text, true, RefGenMedProductDisplays.PageSize, RefGenMedProductDisplays.PageIndex);
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

        #region "Xuất Hàng Ký Gởi"
        private void SearchRefGenMedProductDetails_HangKyGoi(string Name, bool? IsCode, int PageSize, int PageIndex)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (07/07/2014 11:40).
            if (IsCode == false && Name.Length < 1)
            {
                return;
            }

            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetail = true;
            }

            long? RequestID = null;
            long? RefGenDrugCatID_1 = null;

            RequestID = SelectedOutInvoice.ReqDrugInClinicDeptID;

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                RefGenDrugCatID_1 = SelectedOutInvoice.RefGenDrugCatID_1;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(IsCost, Name, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(out Total, asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsList = results.ToObservableCollection();

                            if (IsCode.GetValueOrDefault())
                            {
                                if (RefGenMedProductDetailsList != null && RefGenMedProductDetailsList.Count > 0)
                                {
                                    SelectedSellVisitor = RefGenMedProductDetailsList.ToList()[0];
                                }
                                else
                                {
                                    SelectedSellVisitor = null;

                                    if (tbx != null)
                                    {
                                        txt = "";
                                        tbx.Text = "";
                                    }
                                    if (au != null)
                                    {
                                        au.Text = "";
                                    }

                                    MessageBox.Show(eHCMSResources.Z1407_G1_KgTimThayMatHgNay);
                                }
                            }
                            else
                            {
                                if (au != null)
                                {
                                    //KMx: Lưu ý: New rồi add sẽ nhanh hơn Clear rồi add (khi PageSize > 200). Đối với PageSize nhỏ thì không sao.(07/07/2014 14:24).
                                    //Nhưng khi new PagedSortableCollectionView thì PageSize và PageIndex sẽ bị xóa.
                                    //Cho nên đối với PageSize nhỏ thì không cần New, chỉ cần Clear là được rồi.
                                    RefGenMedProductDisplays.Clear();

                                    RefGenMedProductDisplays.SourceCollection = RefGenMedProductDetailsList;

                                    RefGenMedProductDisplays.TotalItemCount = Total;

                                    //au.ItemsSource = RefGenMedProductDetailsListSum;
                                    au.ItemsSource = RefGenMedProductDisplays;
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
                            //if (IsCode.GetValueOrDefault())
                            //{
                            //    isLoadingDetail = false;
                            //    if (AxQty != null)
                            //    {
                            //        AxQty.Focus();
                            //    }
                            //}
                            //else
                            //{
                            //    if (au != null)
                            //    {
                            //        au.Focus();
                            //    }
                            //}
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void ChooseBatchNumber_HangKyGoi(RefGenMedProductDetails value)
        {
            var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);

            if (items != null && items.Count() > 0)
            {
                RefGenMedProductDetails item = items.ToList()[0];

                OutwardDrugMedDept p = new OutwardDrugMedDept();
                p.ObjSupplierList = new ObservableCollection<DrugDeptSupplier>();
                p.RefGenericDrugDetail = item;
                p.GenMedProductID = item.GenMedProductID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.RequestQty = value.RequestQty;
                value.RequestQty = 0;
                p.OutQuantity = value.RequiredNumber;
                p.OutPrice = item.OutPrice;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.Remaining = item.Remaining;
                p.BidCode = value.BidCode;
                p.VAT = item.VAT;
                //KMx: Hàm SetDefaultSupplier làm chuyện này rồi (03/07/2014 09:27)
                //p.ObjSupplierID = new DrugDeptSupplier();
                //p.ObjSupplierID.SupplierID = 0;
                //p.ObjSupplierID.SupplierName = eHCMSResources.A0015_G1_Chon;

                //KMx: Dời code từ trong hàm CheckBatchNumberExists() ra đây để set NormalPrice, HIPatientPrice, HIAllowedPrice (30/09/2014 11:50).
                if (p.InwardDrugMedDept == null)
                {
                    p.InwardDrugMedDept = new InwardDrugMedDept();
                    p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
                    p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
                }
                p.InvoicePrice = p.OutPrice;
                p.InwardDrugMedDept.NormalPrice = item.NormalPrice;
                p.InwardDrugMedDept.HIPatientPrice = item.HIPatientPrice;
                p.InwardDrugMedDept.HIAllowedPrice = item.HIAllowedPrice;

                CheckBatchNumberExists(p);
            }
        }


        private void OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardDrugMedDeptInvoice OutwardInvoice, bool _bThuTien)
        {
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugMedDepts == null)
            {
                return;
            }

            this.ShowBusyIndicator();

            var NewOutwardDrugMedDepts = new List<OutwardDrugMedDept>();
            var UpdateOutwardDrugMedDepts = new List<OutwardDrugMedDept>();
            var DeleteOutwardDrugMedDepts = new List<OutwardDrugMedDept>();

            NewOutwardDrugMedDepts = OutwardInvoice.OutwardDrugMedDepts.Where(x => x.RecordState == RecordState.DETACHED).ToList();
            UpdateOutwardDrugMedDepts = OutwardInvoice.OutwardDrugMedDepts.Where(x => x.RecordState == RecordState.MODIFIED).ToList();
            if (OutwardInvoice.OutwardDrugMedDepts_Delete != null)
            {
                DeleteOutwardDrugMedDepts = OutwardInvoice.OutwardDrugMedDepts_Delete.ToList();
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardInvoice, NewOutwardDrugMedDepts, UpdateOutwardDrugMedDepts, DeleteOutwardDrugMedDepts
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                bThuTien = _bThuTien;
                                if (bThuTien)
                                {
                                    ShowFormCountMoney();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.G0442_G1_TBao);
                                    RefeshData();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                            }
                            else
                            {
                                MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        AxComboBox cbo = new AxComboBox();
        public void cboSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            cbo = sender as AxComboBox;
        }

        #endregion

        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var item = e.Row.DataContext as MedRegItemBase;
            //KMx: Thêm ngày 26/09/2014 09:52.
            if (item is OutwardDrugMedDept)
            {
                OutwardDrugMedDept CurrentOutMedDept = item as OutwardDrugMedDept;

                if (CurrentOutMedDept == null)
                {
                    return;
                }

                if (CurrentOutMedDept.OutID > 0)
                {
                    CurrentOutMedDept.RecordState = RecordState.MODIFIED;
                }

                //▼====== #001
                //string CurrentColumnName = e.Column.GetValue(FrameworkElement.NameProperty).ToString();

                //if (CurrentColumnName == "colNormalPrice" && CurrentOutMedDept.InwardDrugMedDept != null)
                if (e.Column.Equals(grdPrescription.GetColumnByName("colNormalPrice")))
                {
                    CurrentOutMedDept.InwardDrugMedDept.HIPatientPrice = CurrentOutMedDept.InwardDrugMedDept.NormalPrice;
                }
                //▲====== #001
            }

            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPrice();
            }
        }

        #region PharmacyPayEvent
        private long PaymentID = 0;

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionMedDeptForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionMedDeptForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            isLoadingGetStore = false;
            yield break;
        }


        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (this.IsActive && message != null)
            {
                if (message.CurPatientPayment != null && message.CurPatientPayment.PayAmount < 0)
                {
                    Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        GetOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID);
                    });
                }
                else
                {
                    Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        //KMx: Chuyển từ in phiếu xuất thành in hóa đơn tài chính (26/12/2014 15:00).
                        //Ở đây đang gặp lỗi: Nếu tính tiền không thành công, thì chương trình vẫn tự động in hóa đơn. Khi nào có thời gian thì sửa lại (26/12/2014 15:01).
                        //btnPreview();
                        GetOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID);
                    });
                }
            }
        }
        #endregion

        private void DeleteOutwardDrugMedDeptInvoice()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoices_HangKyGoi_Delete(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool value = contract.EndOutwardDrugMedDeptInvoices_HangKyGoi_Delete(asyncResult);

                            if (value)
                            {
                                MessageBox.Show(eHCMSResources.A0465_G1_Msg_InfoDaHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                GetOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0612_G1_Msg_InfoHuyPhFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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


        public void btnDelete()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0929_G1_Msg_InfoKhTheHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.A0952_G1_Msg_InfoKhTheHuy3, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(eHCMSResources.Z1409_G1_PhXuatDaHuyKgTheHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(eHCMSResources.Z0906_G1_CoMuonHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteOutwardDrugMedDeptInvoice();
            }
        }
    }
}


//private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
//{
//    if (value != null)
//    {
//        if (value.RequiredNumber > 0)
//        {
//            //int intOutput = 0;
//            //if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
//            //{
//            //    decimal a = value.RequiredNumber;

//            if (CheckValidDrugAuto(value))
//            {
//                ChooseBatchNumber_HangKyGoi(value, StoreID);
//            }


//            //}
//            //else
//            //{
//            //    MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0");
//            //}
//        }
//        else
//        {
//            MessageBox.Show("Số lượng phải lớn hơn > 0");
//        }
//    }
//    else
//    {
//        MessageBox.Show(eHCMSResources.K0389_G1_ChonThuocCanBan);
//    }
//}


//private void CheckBatchNumberExists(OutwardDrugMedDept p)
//{
//    bool kq = false;

//    if (SelectedOutInvoice.OutwardDrugMedDepts != null)
//    {
//        foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
//        {
//            if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
//            {
//                p1.OutQuantity += p.OutQuantity;
//                // p1.IsLoad = 0;
//                p1.RequestQty += p.RequestQty;
//                kq = true;
//                break;
//            }
//        }
//        if (!kq)
//        {
//            //p.h= p.RefGenMedProductDetails.InsuranceCover;

//            if (p.InwardDrugMedDept == null)
//            {
//                p.InwardDrugMedDept = new InwardDrugMedDept();
//                p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
//                p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
//            }
//            p.InvoicePrice = p.OutPrice;
//            p.InwardDrugMedDept.NormalPrice = p.OutPrice;
//            p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
//            p.InwardDrugMedDept.HIAllowedPrice = p.HIAllowedPrice;

//            SupplierGenMedProduct_LoadDrugIDNotPaging(p, p.InwardDrugMedDept.GenMedProductID.Value);

//        }
//        txt = "";
//        SelectedSellVisitor = null;
//        if (IsCode.GetValueOrDefault())
//        {
//            if (tbx != null)
//            {
//                tbx.Text = "";
//                tbx.Focus();
//            }

//        }
//        else
//        {
//            if (au != null)
//            {
//                au.Text = "";
//                au.Focus();
//            }
//        }
//    }
//}


//private void ReCountQtyRequest()
//{
//    if (SelectedOutInvoice != null && SelectedSellVisitor != null)
//    {
//        if (SelectedOutInvoice.OutwardDrugMedDepts == null)
//        {
//            SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
//        }
//        var results1 = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == SelectedSellVisitor.GenMedProductID);
//        if (results1 != null && results1.Count() > 0)
//        {
//            foreach (OutwardDrugMedDept p in results1)
//            {
//                if (p.RequestQty > p.OutQuantity)
//                {
//                    p.RequestQty = p.OutQuantity;
//                }
//                SelectedSellVisitor.RequestQty = SelectedSellVisitor.RequestQty - p.RequestQty;
//            }
//        }
//    }
//}


//private void SearchRefGenMedProductDetails_HangKyGoi(string Name, bool? IsCode, int PageSize, int PageIndex)
//{
//    //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (07/07/2014 11:40).
//    if (IsCode == false && Name.Length < 1)
//    {
//        return;
//    }

//    if (IsCode.GetValueOrDefault())
//    {
//        isLoadingDetail = true;
//    }
//    long? RequestID = null;
//    long? RefGenDrugCatID_1 = null;
//    if (SelectedOutInvoice != null)
//    {
//        RequestID = SelectedOutInvoice.ReqDrugInClinicDeptID;
//        RefGenDrugCatID_1 = SelectedOutInvoice.RefGenDrugCatID_1;
//    }
//    var t = new Thread(() =>
//    {
//        using (var serviceFactory = new PharmacyMedDeptServiceClient())
//        {
//            var contract = serviceFactory.ServiceInstance;
//            contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(IsCost, Name, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
//            {

//                try
//                {
//                    int Total = 0;
//                    var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(out Total, asyncResult);

//                    RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
//                    RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

//                    RefGenMedProductDetailsTemp = results.ToObservableCollection();

//                    //KMx: OutwardDrugMedDeptsCopy: Phiếu sau khi lưu (Xem lại phiếu xuất cũ) (05/07/2014 16:30).
//                    //Nếu hàng trong grid (đã lưu rồi) có trong autocomplete thì phải + remaining cho hàng trong AutoComplete.
//                    //Nếu không thì s.Remaining = s.Remaining - d.OutQuantity sẽ bị sai.
//                    if (OutwardDrugMedDeptsCopy != null && OutwardDrugMedDeptsCopy.Count > 0)
//                    {
//                        foreach (OutwardDrugMedDept d in OutwardDrugMedDeptsCopy)
//                        {
//                            var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
//                            if (value.Count() > 0)
//                            {
//                                foreach (RefGenMedProductDetails s in value.ToList())
//                                {
//                                    s.Remaining = s.Remaining + d.OutQuantityOld;
//                                    s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
//                                }
//                            }
//                            //KMx: Nếu hàng trong grid không có trong AutoComplete thì thôi, bỏ vào AutoComplete cũng không hiển thị được (05/07/2014 16:51).
//                            //else
//                            //{
//                            //    RefGenMedProductDetails p = d.RefGenericDrugDetail;
//                            //    p.Remaining = d.OutQuantity;
//                            //    p.RemainingFirst = d.OutQuantity;
//                            //    p.InBatchNumber = d.InBatchNumber;
//                            //    p.OutPrice = d.OutPrice;
//                            //    p.InID = Convert.ToInt64(d.InID);
//                            //    p.STT = d.STT;
//                            //    RefGenMedProductDetailsTemp.Add(p);
//                            //    // d = null;
//                            //}
//                        }
//                    }
//                    foreach (RefGenMedProductDetails s in RefGenMedProductDetailsTemp)
//                    {
//                        if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
//                        {
//                            foreach (OutwardDrugMedDept d in SelectedOutInvoice.OutwardDrugMedDepts)
//                            {
//                                if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
//                                {
//                                    s.Remaining = s.Remaining - d.OutQuantity;
//                                }
//                            }
//                        }
//                        RefGenMedProductDetailsList.Add(s);
//                    }
//                    ListDisplayAutoComplete(Total);

//                }
//                catch (Exception ex)
//                {
//                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                }
//                finally
//                {
//                    if (IsCode.GetValueOrDefault())
//                    {
//                        isLoadingDetail = false;
//                        if (AxQty != null)
//                        {
//                            AxQty.Focus();
//                        }
//                    }
//                    else
//                    {
//                        if (au != null)
//                        {
//                            au.Focus();
//                        }
//                    }
//                }

//            }), null);

//        }

//    });

//    t.Start();
//}