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
using System.Windows.Media;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMS.DrugDept.Views;
using aEMR.Common.Converters;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
* 20170102 #001 CMN: Added properties for 4210 file
* 20211102 #002 QTD: Lọc kho theo cấu hình trách nhiệm
*/
namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IOutwardFromPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OutwardFromPrescriptionViewModel : ViewModelBase, IOutwardFromPrescription
        , IHandle<MedDeptCloseSearchPrescriptionEvent>
        , IHandle<MedDeptCloseSearchPrescriptionInvoiceEvent>
        , IHandle<ChooseBatchNumberForPrescriptionEvent>
        , IHandle<ChooseBatchNumberForPrescriptionResetQtyEvent>
        , IHandle<MedDeptCloseEditPrescription>
    {

        private enum DataGridCol
        {
            ColDelete = 0,
            BH = 1,
            //MaThuoc = 2,
            TenThuoc = 2,
            HamLuong = 3,
            DVT = 4,
            SLYC = 5,
            ThucXuat = 6,
            LoSX = 7,
            DonGiaBan = 8,
            DonGiaBH = 9
        }
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

        private bool _isLoadingInfoPatient = false;
        public bool isLoadingInfoPatient
        {
            get { return _isLoadingInfoPatient; }
            set
            {
                if (_isLoadingInfoPatient != value)
                {
                    _isLoadingInfoPatient = value;
                    NotifyOfPropertyChange(() => isLoadingInfoPatient);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetailPrescript = false;
        public bool isLoadingDetailPrescript
        {
            get { return _isLoadingDetailPrescript; }
            set
            {
                if (_isLoadingDetailPrescript != value)
                {
                    _isLoadingDetailPrescript = value;
                    NotifyOfPropertyChange(() => isLoadingDetailPrescript);
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

        private bool _isLoadingPrescriptID = false;
        public bool isLoadingPrescriptID
        {
            get { return _isLoadingPrescriptID; }
            set
            {
                if (_isLoadingPrescriptID != value)
                {
                    _isLoadingPrescriptID = value;
                    NotifyOfPropertyChange(() => isLoadingPrescriptID);
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
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingInfoPatient || isLoadingPrescriptID || isLoadingDetailPrescript || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }


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

        #region Khoi Tao Member
        private IPrescriptionView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPrescriptionView;
        }
        [ImportingConstructor]
        public OutwardFromPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            GetStaffLogin();

            RefeshData();
            SetDefaultForStore();
            Is_Enabled = false; //--17/12/2020 DatTB
        }
        //DateTime DateServer = DateTime.Now;
        DateTime DateServer = Globals.ServerDate.Value;
        private void RefeshData()
        {
            strNgayDung = "";
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
            SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
            SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();

            SearchCriteria = null;
            SearchCriteria = new PrescriptionSearchCriteria();

            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            SearchCriteria.FromDate = todayDate.AddDays(-1);
            SearchCriteria.ToDate = todayDate;
            DateServer = todayDate;

            SearchInvoiceCriteria = null;
            SearchInvoiceCriteria = new SearchOutwardInfo();
            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;

            PatientInfo = null;
            CurRegistration = null;
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);
            SelectedPrescription = null;
            SelectedPrescription = new Prescription();
            OutwardDrugsCopy = null;

            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }
            if (GetGenMedProductForSellListSum == null)
            {
                GetGenMedProductForSellListSum = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellListSum.Clear();
            }

            if (GetGenMedProductForSellTemp == null)
            {
                GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellTemp.Clear();
            }
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

        }
        private void ClearData()
        {
            OutwardDrugsCopy = null;

            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }
            if (GetGenMedProductForSellListSum == null)
            {
                GetGenMedProductForSellListSum = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellListSum.Clear();
            }

            if (GetGenMedProductForSellTemp == null)
            {
                GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellTemp.Clear();
            }

        }


        #endregion

        #region Properties Member

        public string TitleForm { get; set; }
        public class ListDrugAndQtySell
        {
            public long GenMedProductID;
            public int xban;
            public int RequestQty;
        }
        private ObservableCollection<ListDrugAndQtySell> ListDrugTemp;

        private string _strNgayDung;
        public string strNgayDung
        {
            get { return _strNgayDung; }
            set
            {
                if (_strNgayDung != value)
                {
                    _strNgayDung = value;
                    NotifyOfPropertyChange(() => strNgayDung);
                }
            }
        }

        private bool _IsNotCountInsurance;
        public bool IsNotCountInsurance
        {
            get { return _IsNotCountInsurance; }
            set
            {
                if (_IsNotCountInsurance != value)
                {
                    _IsNotCountInsurance = value;
                    NotifyOfPropertyChange(() => IsNotCountInsurance);
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

        private SearchOutwardInfo _SearchInvoiceCriteria;
        public SearchOutwardInfo SearchInvoiceCriteria
        {
            get
            {
                return _SearchInvoiceCriteria;
            }
            set
            {
                if (_SearchInvoiceCriteria != value)
                {
                    _SearchInvoiceCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchInvoiceCriteria);
            }
        }

        private PrescriptionSearchCriteria _SearchCriteria;
        public PrescriptionSearchCriteria SearchCriteria
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
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private Prescription _SelectedPrescription;
        public Prescription SelectedPrescription
        {
            get
            {
                return _SelectedPrescription;
            }
            set
            {
                if (_SelectedPrescription != value)
                {
                    _SelectedPrescription = value;
                }
                NotifyOfPropertyChange(() => SelectedPrescription);
            }
        }

        private ObservableCollection<OutwardDrugMedDept> OutwardDrugsCopy;

        private OutwardDrugMedDeptInvoice SelectedOutInvoiceCopy;

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

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
                if (_Visibility == Visibility.Collapsed)
                {
                    IsVisibility = Visibility.Visible;
                }
                else
                {
                    IsVisibility = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                }
                NotifyOfPropertyChange(() => IsVisibility);
            }
        }

        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                }
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);
                }
            }
        }

        private LoadRegistrationSwitch _loadRegisSwitch = new LoadRegistrationSwitch();
        public LoadRegistrationSwitch LoadRegisSwitch
        {
            get
            {
                return _loadRegisSwitch;
            }
            set
            {
                if (_loadRegisSwitch != value)
                {
                    _loadRegisSwitch = value;
                    NotifyOfPropertyChange(() => LoadRegisSwitch);
                }
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
                }

            }
        }

        #endregion

        #region checking account

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mXuatTheoToa_Thuoc_TimToa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                    , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                    (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_TimToa, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_ThemThuoc = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_ThemThuoc, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_TimPhieuXuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_TimPhieuXuat, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_Xuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_Xuat, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_HuyPhieu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_HuyPhieu, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_CapNhat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_CapNhat, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_InPhieuXuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_InPhieuXuat, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_TaoBill = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_TaoBill, (int)ePermission.mView);

            mXuatTheoToa_Thuoc_InBill = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mKhoaDuoc
                                   , (int)eKhoaDuoc.mXuatTheoToa_Thuoc,
                                   (int)oKhoaDuocEx.mXuatTheoToa_Thuoc_InBill, (int)ePermission.mView);



        }

        private bool _mXuatTheoToa_Thuoc_TimToa = true;
        public bool mXuatTheoToa_Thuoc_TimToa
        {
            get
            {
                return _mXuatTheoToa_Thuoc_TimToa;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_TimToa == value)
                    return;
                _mXuatTheoToa_Thuoc_TimToa = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_TimToa);
            }
        }

        private bool _mXuatTheoToa_Thuoc_ThemThuoc = true;
        public bool mXuatTheoToa_Thuoc_ThemThuoc
        {
            get
            {
                return _mXuatTheoToa_Thuoc_ThemThuoc;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_ThemThuoc == value)
                    return;
                _mXuatTheoToa_Thuoc_ThemThuoc = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_ThemThuoc);
            }
        }

        private bool _mXuatTheoToa_Thuoc_TimPhieuXuat = true;
        public bool mXuatTheoToa_Thuoc_TimPhieuXuat
        {
            get
            {
                return _mXuatTheoToa_Thuoc_TimPhieuXuat;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_TimPhieuXuat == value)
                    return;
                _mXuatTheoToa_Thuoc_TimPhieuXuat = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_TimPhieuXuat);
            }
        }

        private bool _mXuatTheoToa_Thuoc_Xuat = true;
        public bool mXuatTheoToa_Thuoc_Xuat
        {
            get
            {
                return _mXuatTheoToa_Thuoc_Xuat;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_Xuat == value)
                    return;
                _mXuatTheoToa_Thuoc_Xuat = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_Xuat);
            }
        }

        private bool _mXuatTheoToa_Thuoc_HuyPhieu = true;
        public bool mXuatTheoToa_Thuoc_HuyPhieu
        {
            get
            {
                return _mXuatTheoToa_Thuoc_HuyPhieu;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_HuyPhieu == value)
                    return;
                _mXuatTheoToa_Thuoc_HuyPhieu = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_HuyPhieu);
            }
        }

        private bool _mXuatTheoToa_Thuoc_CapNhat = true;
        public bool mXuatTheoToa_Thuoc_CapNhat
        {
            get
            {
                return _mXuatTheoToa_Thuoc_CapNhat;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_CapNhat == value)
                    return;
                _mXuatTheoToa_Thuoc_CapNhat = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_CapNhat);
            }
        }

        private bool _mXuatTheoToa_Thuoc_InPhieuXuat = true;
        public bool mXuatTheoToa_Thuoc_InPhieuXuat
        {
            get
            {
                return _mXuatTheoToa_Thuoc_InPhieuXuat;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_InPhieuXuat == value)
                    return;
                _mXuatTheoToa_Thuoc_InPhieuXuat = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_InPhieuXuat);
            }
        }

        private bool _mXuatTheoToa_Thuoc_TaoBill = true;
        public bool mXuatTheoToa_Thuoc_TaoBill
        {
            get
            {
                return _mXuatTheoToa_Thuoc_TaoBill;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_TaoBill == value)
                    return;
                _mXuatTheoToa_Thuoc_TaoBill = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_TaoBill);
            }
        }

        private bool _mXuatTheoToa_Thuoc_InBill = true;
        public bool mXuatTheoToa_Thuoc_InBill
        {
            get
            {
                return _mXuatTheoToa_Thuoc_InBill;
            }
            set
            {
                if (_mXuatTheoToa_Thuoc_InBill == value)
                    return;
                _mXuatTheoToa_Thuoc_InBill = value;
                NotifyOfPropertyChange(() => mXuatTheoToa_Thuoc_InBill);
            }
        }
        #endregion

        #region binding visibilty

        //public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as Button;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        #region DS Kho Ngoai Tru Member

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList;
            //▼===== #002
            var StoreTemp = paymentTypeTask.LookupList.Where(x=>!x.IsConsignment).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                SetDefaultForStore();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #002
            isLoadingGetStore = false;
            yield break;
        }

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                RefeshData();
            }
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }
        #endregion

        #region Kiem Tra Du Lieu Member

        private bool CheckQuantity(object outward)
        {
            try
            {
                OutwardDrugMedDept p = outward as OutwardDrugMedDept;
                if (p.RequestQty == p.OutQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public Nullable<double> ValueQuyenLoiBH
        {
            get
            {
                if (SelectedOutInvoice == null || SelectedOutInvoice.SelectedPrescription == null)
                {
                    return null;
                }
                if (SelectedOutInvoice.outiID == 0)
                {
                    if (SelectedOutInvoice.SelectedPrescription.IsSold)
                    {
                        return 0;
                    }
                    return CurRegistration.PtInsuranceBenefit;
                }
                else
                {
                    if (SelectedOutInvoice.IsHICount.GetValueOrDefault())
                    {
                        return CurRegistration.PtInsuranceBenefit;
                    }
                    return 0;
                }
            }
        }

        int DayRpts = 0;
        private void GetClassicationPatientPrescription()
        {
            //neu da ban 1 lan khong tinh BH,thi lan sau mua co dc tinh BH hay ko?
            //if (CurRegistration != null && CurRegistration.HealthInsurance != null && CurRegistration.HealthInsurance.ValidDateTo >= Globals.GetCurServerDateTime().Date
            //    && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && !SelectedOutInvoice.SelectedPrescription.IsSold)
            // TxD 15/03/2015: Due to new BHYT rules changed for 2015 InPatient is still covered even if their BHYT cards have expired during their stay at the hospital
            // so we DO NOT check for that now
            if (CurRegistration != null && CurRegistration.HealthInsurance != null && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && !SelectedOutInvoice.SelectedPrescription.IsSold)
            {
                int xNgayBHToiDa_NoiTru = 5;
                TimeSpan validDaysLeft = CurRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.GetCurServerDateTime().Date;
                if (validDaysLeft.Days <= 0)
                {
                    DayRpts = xNgayBHToiDa_NoiTru;
                }
                else
                {
                    DayRpts = Convert.ToInt32(validDaysLeft.TotalDays + 1);
                }
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);
        }

        private void GetClassicationPatientInvoice()
        {
            //if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= DateTime.Now.Date
            // && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.IsHICount.GetValueOrDefault())
            //Kiên thêm đk SelectedOutInvoice.IsHICount == true, vì khi load 1 phiếu xuất cũ lên, không có IsSold nên không biết là phiếu đó được bán lần 1 hay lần 2.
            //Nên phải dựa vào IsHICount, nếu IsHICount của phiếu đó = true tức là phiếu đó bán lần 1 và được tính bảo hiểm.

            // TxD 15/03/2015: Due to new BHYT rules changed for 2015 InPatient is still covered even if their BHYT cards have expired during their stay at the hospital
            // so we DO NOT check for that now
            //if (CurRegistration != null && CurRegistration.HealthInsurance != null && CurRegistration.HealthInsurance.ValidDateTo >= Globals.GetCurServerDateTime().Date
            //     && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.IsHICount == true)

            if (CurRegistration != null && CurRegistration.HealthInsurance != null && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.IsHICount == true)
            {                
                int xNgayBHToiDa_NoiTru = 5;
                TimeSpan validDaysLeft = CurRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.GetCurServerDateTime().Date;
                if (validDaysLeft.Days <= 0)
                {
                    DayRpts = xNgayBHToiDa_NoiTru;
                }
                else
                {
                    DayRpts = Convert.ToInt32(validDaysLeft.TotalDays + 1);
                }
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);

        }



        private bool CheckValid()
        {
            bool result = true;
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                if (Services.Core.AxHelper.CompareDate((DateTime)SelectedPrescription.IssuedDateTime, SelectedOutInvoice.OutDate.GetValueOrDefault()) == 1)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0835_G1_NgXuatKgNhoHonNgRaToa), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (Services.Core.AxHelper.CompareDate(SelectedOutInvoice.OutDate.GetValueOrDefault(), Globals.ServerDate.Value) == 1)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0836_G1_NgXuatKgLonHonNgHTai), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugMedDepts == null)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugMedDepts.Count == 0)
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                    //if (item.OutPrice <= 0)
                    //{
                    //    MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    //    result = false;
                    //    break;
                    //}
                    if (item.OutQuantity <= 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0837_G1_SLgXuatMoiDongLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.Validate() == false)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0838_G1_DLieuKgHopLe), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    //neu ngay het han lon hon ngay hien tai
                    if (Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, item.InExpiryDate.GetValueOrDefault()) == 1)
                    {
                        strError += item.GetGenMedProductForSell.BrandName + string.Format(" {0}.", eHCMSResources.Z0868_G1_DaHetHanDung) + Environment.NewLine;
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
            }
            return result;
        }

        //KMx: 2 hàm GetValueFromAnonymousType(), CalScheduleQty() giống với 2 hàm cùng tên ở EditPrescriptionViewModel (08/06/2014 17:41).
        //Ban đầu tính đem 2 hàm này sang Globals để 2 ViewModels sử dụng chung. Nhưng đem qua Globals thì bị lỗi ở dòng (T)type.GetProperty(itemKey).GetValue(dataItem, null);
        public static T GetValueFromAnonymousType<T>(object dataItem, string itemKey)
        {
            System.Type type = dataItem.GetType();

            T itemValue = (T)type.GetProperty(itemKey).GetValue(dataItem, null);

            return itemValue;
        }

        public static float CalScheduleQty(object item, int nNumDay)
        {
            if (item == null)
            {
                return 0;
            }

            float[] WeeklySchedule = new float[7];
            byte? SchedBeginDOW;
            double DispenseVolume;

            SchedBeginDOW = GetValueFromAnonymousType<byte?>(item, "SchedBeginDOW");

            DispenseVolume = GetValueFromAnonymousType<double>(item, "DispenseVolume");

            WeeklySchedule[0] += GetValueFromAnonymousType<float>(item, "QtySchedMon");
            WeeklySchedule[1] += GetValueFromAnonymousType<float>(item, "QtySchedTue");
            WeeklySchedule[2] += GetValueFromAnonymousType<float>(item, "QtySchedWed");
            WeeklySchedule[3] += GetValueFromAnonymousType<float>(item, "QtySchedThu");
            WeeklySchedule[4] += GetValueFromAnonymousType<float>(item, "QtySchedFri");
            WeeklySchedule[5] += GetValueFromAnonymousType<float>(item, "QtySchedSat");
            WeeklySchedule[6] += GetValueFromAnonymousType<float>(item, "QtySchedSun");

            return Globals.CalcWeeklySchedulePrescription(SchedBeginDOW, nNumDay, WeeklySchedule, (float)DispenseVolume);
        }


        //KMx: Hàm này được viết lại từ hàm CheckInsurance. Lý do đổi cách tính thuốc lịch (08/06/2014 15:54)
        private bool CheckInsurance_New(int BHValidDays)
        {
            if (SelectedPrescription == null)
            {
                return true;
            }
            if (SelectedOutInvoice == null)
            {
                return true;
            }
            if (ListDrugTemp == null)
            {
                ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
            }
            ListDrugTemp.Clear();

            var hhh = from hd in SelectedOutInvoice.OutwardDrugMedDepts
                        where hd.GetGenMedProductForSell.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
                        group hd by new
                        { 
                            hd.GenMedProductID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.QtyMaxAllowed, hd.QtySchedMon, hd.QtySchedTue,
                            hd.QtySchedWed, hd.QtySchedThu, hd.QtySchedFri, hd.QtySchedSat, hd.QtySchedSun, hd.SchedBeginDOW, hd.DispenseVolume,
                            hd.GetGenMedProductForSell.InsuranceCover, hd.GetGenMedProductForSell.BrandName
                        } into hdgroup

                        select new
                        {
                            RequestQty = hdgroup.Sum(groupItem => groupItem.RequestQty),
                            OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                            GenMedProductID = hdgroup.Key.GenMedProductID,
                            DayRpts = hdgroup.Key.DayRpts,
                            V_DrugType = hdgroup.Key.V_DrugType,
                            QtyForDay = hdgroup.Key.QtyForDay,

                            QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                            QtySchedMon = hdgroup.Key.QtySchedMon,
                            QtySchedTue = hdgroup.Key.QtySchedTue,
                            QtySchedWed = hdgroup.Key.QtySchedWed,
                            QtySchedThu = hdgroup.Key.QtySchedThu,
                            QtySchedFri = hdgroup.Key.QtySchedFri,
                            QtySchedSat = hdgroup.Key.QtySchedSat,
                            QtySchedSun = hdgroup.Key.QtySchedSun,
                            SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                            DispenseVolume = hdgroup.Key.DispenseVolume,

                            InsuranceCover = hdgroup.Key.InsuranceCover,
                            BrandName = hdgroup.Key.BrandName
                        };

            string strError = "";
            int xNgayToiDa = 30;
            int xNgayBHToiDa_NgoaiTru = 30;
            int xNgayBHToiDa_NoiTru = 5;

            xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;
            xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

            if (IsBenhNhanNoiTru() == false)
            {
                xNgayToiDa = xNgayBHToiDa_NgoaiTru;
            }
            else
            {
                xNgayToiDa = xNgayBHToiDa_NoiTru;
            }

            //Cảnh báo cho thuốc Cần.
            string strDrugsNotHaveDayRpts = "";
            //Những thuốc mà hết hạn bảo hiểm. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
            string strInvalidDrugs = "";

            for (int i = 0; i < hhh.Count(); i++)
            {
                //QtyMaxAllowed: Số lượng tối đa được bán (Minimum của SLYC và SL BH cho phép).
                //QtyHIValidateTo: Số lượng thuốc tính đến ngày BH hết hạn. Nếu SL bán > SL tính đến hết ngày BH hết hạn thì cảnh báo.
                //RequestQty: Số lượng bác sĩ yêu cầu. Bác sĩ yêu cầu bao nhiêu viên thì đưa lên bấy nhiêu.

                int QtyMaxAllowed = 0;
                int QtyHIValidateTo = 0;

                string strReasonCannotSell;

                if (hhh.ToList()[i].RequestQty <= 0)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(" {0}.", eHCMSResources.K0015_G1_ThuocKhongCoSLgYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }

                QtyMaxAllowed = Convert.ToInt32(hhh.ToList()[i].QtyMaxAllowed);

                //KMx: Nếu là thuốc Cần thì so sánh "số lượng xuất" và "số lượng được bán tối đa."
                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
                {
                    if (hhh.ToList()[i].OutQuantity > QtyMaxAllowed)
                    {
                        strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                        strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].RequestQty + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.xban = QtyMaxAllowed;
                        ListDrugTemp.Add(item);
                    }
                    else
                    {
                        strDrugsNotHaveDayRpts += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                }

                //KMx: Nếu là thuốc Thường hoặc thuốc Lịch.
                else
                {
                    //KMx: QtyHIValidateTo: Số lượng tính đến ngày BH còn hiệu lực.
                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                    {
                        //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
                        double QtyRounded = Math.Round((float)hhh.ToList()[i].QtyForDay * BHValidDays, 2);
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(QtyRounded));
                    }

                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], BHValidDays)));
                    }

                    //Nếu số lượng muốn bán > số lượng mà bảo hiểm còn hiệu lực. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
                    if (hhh.ToList()[i].OutQuantity > QtyHIValidateTo)
                    {
                        strInvalidDrugs += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }

                    //Lấy Min của SLYC và SL tối đa để khống chế số lượng thực xuất.
                    int QtyAllowSell = Math.Min((int)hhh.ToList()[i].RequestQty, QtyMaxAllowed);

                    if (hhh.ToList()[i].OutQuantity > QtyAllowSell)
                    {
                        if (hhh.ToList()[i].RequestQty > QtyMaxAllowed)
                        {
                            strReasonCannotSell = string.Format(eHCMSResources.Z0876_G1_BHChiTraToiDa, xNgayToiDa.ToString());
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0877_G1_SLgDuocBan) + QtyMaxAllowed + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }
                        else
                        {
                            strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                            strError += string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(". {0} ", eHCMSResources.Z0875_G1_SLgBSiKe) + hhh.ToList()[i].RequestQty + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }

                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.xban = QtyAllowSell;
                        ListDrugTemp.Add(item);
                    }
                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
                //sua lai so luong xuat cho hop li
                for (int i = 0; i < ListDrugTemp.Count; i++)
                {
                    var values = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == ListDrugTemp[i].GenMedProductID).OrderBy(x => x.InExpiryDate);

                    if (values == null || values.Count() <= 0)
                    {
                        continue;
                    }

                    int xban = ListDrugTemp[i].xban;

                    foreach (OutwardDrugMedDept p in values)
                    {
                        if (xban <= 0)
                        {
                            DeleteOutwardDrugs(p);
                            continue;
                        }

                        if (p.OutQuantity <= xban)
                        {
                            if (p.RequestQty <= xban)
                            {
                                p.OutQuantity = p.RequestQty;
                            }
                            xban = xban - (int)p.OutQuantity;
                        }
                        else //p.OutQuantity > xban
                        {
                            p.OutQuantity = xban;
                            xban = 0;
                        }
                    }
                }

                SumTotalPrice();
                return false;
            }

            if (!string.IsNullOrEmpty(strInvalidDrugs))
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0878_G1_BHBNConHieuLuc0Ng, BHValidDays) + Environment.NewLine + string.Format("{0}: ", eHCMSResources.Z0879_G1_NhungThuoc) + Environment.NewLine + strInvalidDrugs + string.Format("{0}.", eHCMSResources.Z0880_G1_BanHonSoNgBH) + Environment.NewLine + eHCMSResources.Z0881_G1_CoDongYBanKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(strDrugsNotHaveDayRpts))
            {
                if (MessageBox.Show(string.Format("{0}: ", eHCMSResources.Z0882_G1_I) + Environment.NewLine + Environment.NewLine + strDrugsNotHaveDayRpts + Environment.NewLine + eHCMSResources.Z0881_G1_CoDongYBanKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }


        public void btnNgayDung_New()
        {
            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedOutInvoice == null)
            {
                return;
            }

            int NgayDung = 0;

            if (!Int32.TryParse(strNgayDung, out NgayDung))
            {
                MessageBox.Show(eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (NgayDung <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0837_G1_Msg_InfoNgDungLonHon0));
                return;
            }

            if (IsHIPatient && !IsNotCountInsurance) //toa bao hiem
            {
                int xNgayBHToiDa_NoiTru = 5;

                xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

                //KMx: Lấy thuốc có ngày dùng nhỏ nhất (không tính thuốc cần). Nếu ngày dùng muốn cập nhật > ngày dùng nhỏ nhất thì không cho cập nhật.
                var DrugMinDayRpts = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.DayRpts > 0 && x.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN).OrderBy(x => x.DayRpts).Take(1);

                if (NgayDung > xNgayBHToiDa_NoiTru)
                {
                    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0207_G1_Msg_InfoBHTraToiDa) + xNgayBHToiDa_NoiTru + string.Format(" {0}.", eHCMSResources.Z0883_G1_NhapNgDung), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return;
                }

                foreach (OutwardDrugMedDept d in DrugMinDayRpts)
                {
                    if (NgayDung > d.DayRpts)
                    {
                        MessageBox.Show(string.Format("{0} ", eHCMSResources.A0080_G1_Msg_SubBacSiYCThuoc) + d.GetGenMedProductForSell.BrandName + " (" + d.DayRpts + string.Format(" {0}).", eHCMSResources.N0045_G1_Ng) + Environment.NewLine + string.Format("{0}.", eHCMSResources.Z0884_G1_KgDuocCNhatNgDung), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        return;
                    }
                }

            }


            if (GetGenMedProductForSellList == null)
            {
                GetGenMedProductForSellList = new ObservableCollection<GetGenMedProductForSell>();
            }
            else
            {
                GetGenMedProductForSellList.Clear();
            }

            //dung de lay ton hien tai va ton ban dau cua tung thuoc theo lo
            ObservableCollection<GetGenMedProductForSell> temp = GetGenMedProductForSellListByPrescriptID.DeepCopy();
            LaySoLuongTheoNgayDung(temp);

            //dung de lay ton hien tai va ton ban dau cua tung thuoc da duoc sum
            var ObjSumRemaining = from hd in GetGenMedProductForSellList
                                    group hd by new { hd.GenMedProductID, hd.BrandName } into hdgroup
                                    select new
                                    {
                                        Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                                        RemainingFirst = hdgroup.Sum(groupItem => groupItem.RemainingFirst),
                                        GenMedProductID = hdgroup.Key.GenMedProductID,
                                        BrandName = hdgroup.Key.BrandName
                                    };

            //lay tong so luong yc cua cac thuoc co trong toa (dua vao ngay dung > 0) 
            var hhh = from hd in SelectedOutInvoice.OutwardDrugMedDepts
                      where hd.DayRpts > 0 && hd.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN
                      group hd by new
                      {
                          hd.GenMedProductID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetGenMedProductForSell.BrandName
                      } into hdgroup
                      select new
                      {
                          RequestQty = hdgroup.Sum(groupItem => groupItem.RequestQty),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          BrandName = hdgroup.Key.BrandName
                      };

            string ThongBao = "";
            //Lưu ý: Khi SelectedOutInvoice.OutwardDrugs add thêm 1 đối tượng (OutwardDrug) thì hhh.Count() tự động tăng lên 1.

            for (int i = 0; i < hhh.Count(); i++)
            {
                int QtyWillChange = 0;

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                {
                    //KMx: Sau khi lấy QtyForDay * NgayDung thì phải Round lại rồi sau đó mới Ceiling.
                    //Nếu không sẽ bị sai trong trường hợp thuốc có QtyForDay = 0.1666667. Khi nhân lên 30 ngày sẽ ra kết quả 5.00001 và Ceiling liền thì sẽ ra 6.
                    double QtyRounded = Math.Round((double)hhh.ToList()[i].QtyForDay * NgayDung, 2);
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(QtyRounded));
                }

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                {
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], NgayDung)));
                }

                //KMx: Dòng thuốc có số lượng muốn thay đổi bằng 0 thì sẽ xóa dòng thuốc đó đi (14/06/2014 16:54)
                //if (QtyWillChange <= 0)
                //{
                //    continue;
                //}

                var values = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == hhh.ToList()[i].GenMedProductID.GetValueOrDefault()).OrderBy(x => x.InExpiryDate);

                if (values == null || values.Count() <= 0)
                {
                    continue;
                }

                //Neu so luong < So luong hien co tren luoi thi chi can ham ben duoi
                foreach (OutwardDrugMedDept p in values)
                {
                    if (QtyWillChange <= 0)
                    {
                        DeleteOutwardDrugs(p);
                    }
                    else if (p.OutQuantity > QtyWillChange)
                    {
                        p.OutQuantity = QtyWillChange;
                        QtyWillChange = 0;
                    }
                    else if (p.OutQuantity <= QtyWillChange)
                    {
                        QtyWillChange = QtyWillChange - (int)p.OutQuantity;
                    }
                }

                if (QtyWillChange <= 0)
                {
                    continue;
                }

                //else neu > so luong hien co tren luoi thi them vao

                var Obj = ObjSumRemaining.Where(x => x.GenMedProductID == hhh.ToList()[i].GenMedProductID.GetValueOrDefault());
                if (Obj != null && Obj.Count() > 0)
                {
                    if (Obj.ToArray()[0].Remaining > 0 && Obj.ToArray()[0].Remaining >= QtyWillChange)
                    {
                        GetGenMedProductForSell item = new GetGenMedProductForSell();
                        item.GenMedProductID = hhh.ToList()[i].GenMedProductID.GetValueOrDefault();
                        item.BrandName = hhh.ToList()[i].BrandName;
                        //KMx: Phải assign Remaining trước RequiredNumber. Nếu không sẽ bị lỗi, vì khi assign RequiredNumber thì nó sẽ so sánh với Remaining.
                        item.Remaining = Obj.ToArray()[0].Remaining;
                        item.RequiredNumber = QtyWillChange;
                        item.Qty = (int)hhh.ToList()[i].RequestQty;
                        //item.Remaining = Obj.ToArray()[0].Remaining;

                        ReCountQtyAndAddList(item);
                    }
                    else
                    {
                        if (Obj.ToArray()[0].RemainingFirst > 0)
                        {
                            ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + Obj.ToArray()[0].BrandName + string.Format(": {0} ", eHCMSResources.Z0885_G1_SLgCanBan) + QtyWillChange.ToString() + string.Format(", {0} ", eHCMSResources.Z0886_G1_SLgConLai) + Obj.ToArray()[0].RemainingFirst.ToString() + Environment.NewLine;
                        }
                        else
                        {
                            ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + Obj.ToArray()[0].BrandName + string.Format(": {0} ", eHCMSResources.Z0885_G1_SLgCanBan) + QtyWillChange.ToString() + ", SL còn lại 0" + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    ThongBao = ThongBao + string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + hhh.ToList()[i].BrandName + string.Format(" {0}!", eHCMSResources.Z0887_G1_DaHet) + Environment.NewLine;
                }

            }
            //KMx: Nếu để dòng dưới thì sẽ bị lỗi khi thêm cùng 1 loại thuốc có trong toa nhưng khác lô.
            //SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
            if (!string.IsNullOrEmpty(ThongBao))
            {
                MessageBox.Show(ThongBao);
            }
        }


        #endregion

        #region Luu, Thu Tien Member
        public void btnSaveMoney()
        {
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
                SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
            }
            if (!CheckValid())
            {
                return;
            }

            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedOutInvoice.outiID <= 0)
            {
                SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                SelectedOutInvoice.IssueID = SelectedPrescription.IssueID;
                SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                SelectedOutInvoice.SelectedStorage = new RefStorageWarehouseLocation();
                SelectedOutInvoice.SelectedStorage.StoreID = StoreID;
                SelectedOutInvoice.SelectedStaff = GetStaffLogin();
                SelectedOutInvoice.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
                SelectedOutInvoice.StoreID = StoreID;
                SelectedOutInvoice.V_MedProductType = V_MedProductType;
                SelectedOutInvoice.RefGenDrugCatID_1 = SelectedPrescription.RefGenDrugCatID_1;
            }

            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
            SelectedOutInvoice.IsHICount = IsHIPatient && !IsNotCountInsurance;

            if (IsHIPatient && !IsNotCountInsurance)//toa bao hiem
            {
                if (DayRpts > 0)
                {
                    //if (CheckInsurance(DayRpts))
                    if (CheckInsurance_New(DayRpts))
                    {
                        //KMx: Khoa Dược không sử dụng STT
                        //SelectedOutInvoice.ColectDrugSeqNumType = 2;//co BH
                        SaveDrugNew(SelectedOutInvoice);
                    }
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z0955_G1_TheBHBNDaHetHan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        //gan gia BH = 0 het
                        if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                        {
                            foreach (OutwardDrugMedDept p in SelectedOutInvoice.OutwardDrugMedDepts)
                            {
                                p.HIAllowedPrice = 0;
                                p.InwardDrugMedDept.HIAllowedPrice = 0;

                                p.OutPrice = p.NormalPrice;
                            }
                            //KMx: Khoa Dược không sử dụng STT
                            //SelectedOutInvoice.ColectDrugSeqNumType = 1;//khong bao hiem
                            SaveDrugNew(SelectedOutInvoice);
                        }

                    }
                }
            }
            else
            {
                //KMx: Khoa Dược không sử dụng STT
                //SelectedOutInvoice.ColectDrugSeqNumType = 1;//khong bao hiem
                SaveDrugNew(SelectedOutInvoice);
            }

        }

        private void SaveDrugNew(OutwardDrugMedDeptInvoice Invoice)
        {
            Coroutine.BeginExecute(DoSaveDrugs(SelectedOutInvoice, true));
        }



        #endregion

        #region Total Price Member
        private decimal _TotalInvoicePrice;
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }
        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }
        private void SumTotalPrice()
        {
            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugMedDepts == null)
            {
                return;
            }


            double HIBenefit = 0;

            if (CurRegistration != null)
            {
                HIBenefit = CurRegistration.PtInsuranceBenefit.GetValueOrDefault();
            }

            /*▼====: #001*/
            if (SelectedOutInvoice != null && CurRegistration.HealthInsurance != null && (SelectedOutInvoice.OutDate < CurRegistration.HealthInsurance.ValidDateFrom || SelectedOutInvoice.OutDate > CurRegistration.HealthInsurance.ValidDateTo))
            {
                if (CurRegistration.HealthInsurance_3 != null && SelectedOutInvoice.OutDate >= CurRegistration.HealthInsurance_3.ValidDateFrom && SelectedOutInvoice.OutDate <= CurRegistration.HealthInsurance_3.ValidDateTo)
                {
                    HIBenefit = CurRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0);
                }
                else if (CurRegistration.HealthInsurance_2 != null && SelectedOutInvoice.OutDate >= CurRegistration.HealthInsurance_2.ValidDateFrom && SelectedOutInvoice.OutDate <= CurRegistration.HealthInsurance_2.ValidDateTo)
                {
                    HIBenefit = CurRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0);
                }
            }
            /*▲====: #001*/

            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

            SelectedOutInvoice.TotalInvoicePrice = 0;
            SelectedOutInvoice.TotalPatientPayment = 0;
            SelectedOutInvoice.TotalHIPayment = 0;
            SelectedOutInvoice.TotalPriceDifference = 0;
            SelectedOutInvoice.TotalCoPayment = 0;

            if (!onlyRoundResultForOutward)
            {
                if (SelectedOutInvoice.outiID == 0)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() > 0)
                        {
                            SelectedOutInvoice.OutwardDrugMedDepts[i].HIBenefit = HIBenefit;
                        }
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalInvoicePrice = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment = (decimal)Math.Floor((double)(SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity * (decimal)HIBenefit));
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;
                        if (SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment > 0)
                        {
                            SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment = (decimal)Math.Ceiling((double)(SelectedOutInvoice.OutwardDrugMedDepts[i].TotalInvoicePrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity)));
                        }

                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                else
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalInvoicePrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                SelectedOutInvoice.TotalInvoicePrice = TotalInvoicePrice;
                SelectedOutInvoice.TotalPatientPayment = TotalPatientPayment;
                SelectedOutInvoice.TotalHIPayment = TotalHIPayment;
            }
            else
            {
                if (SelectedOutInvoice.outiID == 0)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() > 0)
                        {
                            SelectedOutInvoice.OutwardDrugMedDepts[i].HIBenefit = HIBenefit;
                        }

                        //KMx: Phải tính TotalPrice, vì bên nhà thuốc, khi set OutQuantity thì chương trình tự tính TotalPrice, còn khoa Dược thì không (29/11/2014 17:21).
                        //SelectedOutInvoice.OutwardDrugMedDepts[i].TotalInvoicePrice = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;


                        //SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = (decimal)Math.Floor((double)(SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit));
                        //KMx: Không được làm tròn giá BH trả, nếu không mẫu 25 sẽ bị lệch với báo cáo nhà thuốc khi tính lại tiền bảo hiểm mà bệnh nhân được hưởng (trên mẫu 25). (31/07/2014 17:43).
                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity * (decimal)HIBenefit;

                        SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;
                        if (SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment > 0)
                        {
                            //SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = (decimal)Math.Ceiling((double)(SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity)));
                            SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugMedDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity - SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                        }

                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalPrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment;
                    }

                    TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                    TotalHIPayment = MathExt.Round(TotalHIPayment, aEMR.Common.Converters.MidpointRounding.AwayFromZero);

                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;

                }
                else
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalInvoicePrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugMedDepts[i].PriceDifference * SelectedOutInvoice.OutwardDrugMedDepts[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugMedDepts[i].TotalCoPayment;
                    }

                    TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                    TotalHIPayment = MathExt.Round(TotalHIPayment, aEMR.Common.Converters.MidpointRounding.AwayFromZero);


                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                SelectedOutInvoice.TotalInvoicePrice = TotalInvoicePrice;
                SelectedOutInvoice.TotalPatientPayment = TotalPatientPayment;
                SelectedOutInvoice.TotalHIPayment = TotalHIPayment;
            }

        }
        #endregion

        #region Tim Toa Thuoc De Ban Member

        public void Search_KeyUp_Pre(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchPrescriptionToSell();
            }
        }

        public void Search_KeyUp_PreHICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchPrescriptionToSell();
            }
        }

        //Search_KeyUp_PrePrescriptID
        public void Search_KeyUp_PrePrescriptID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                SearchPrescriptionToSell();
            }
        }



        private void LoadInfoCommon()
        {

            if (SelectedPrescription != null)
            {
                //Kien: Delete data of previous prescription
                SelectedSellVisitor = null;
                strNgayDung = "";
                ((OutwardFromPrescriptionView)this.GetView()).AutoDrug_Text.Text = "";
                ((OutwardFromPrescriptionView)this.GetView()).chbBH.IsChecked = false;
                //---------------------------------------------------------
                SelectedOutInvoice = null;
                SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
                SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
                SelectedOutInvoice.SelectedPrescription = SelectedPrescription;
                SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                SelectedOutInvoice.IssueID = SelectedPrescription.IssueID;
                SelectedOutInvoice.OutInvID = "";
                SelectedOutInvoice.outiID = 0;
                SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                SelectedOutInvoice.CheckedPoint = false;
                SelectedOutInvoice.RefGenDrugCatID_1 = SelectedPrescription.RefGenDrugCatID_1;
                SelectedOutInvoice.CategoryName = SelectedPrescription.CategoryName;
                HideShowColumnDelete();

                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

                //KMx: Hàm SearchGetGenMedProductForSellByPrescriptID() để trong Coroutine DoGetInfoPatientPrescription() luôn.
                //Vì SearchGetGenMedProductForSellByPrescriptID() có dùng biến IsHIPatient. Mà biến IsHIPatient do Coroutine quyết định.
                //Nên sau khi Coroutine quyết định IsHIPatient xong thì mới gọi hàm SearchGetGenMedProductForSellByPrescriptID()
                //SearchGetGenMedProductForSellByPrescriptID();
                if (SelectedPrescription.PtRegistrationID.HasValue)
                {
                    Coroutine.BeginExecute(DoGetInfoPatientPrescription());
                }
            }
        }


        private IEnumerator<IResult> DoGetInfoPatientPrescription()
        {
            isLoadingInfoPatient = true;

            var regInfo = new LoadRegistrationInfo_InPtTask(SelectedPrescription.PtRegistrationID.GetValueOrDefault(), (int)AllLookupValues.PatientFindBy.NOITRU, LoadRegisSwitch);
            yield return regInfo;

            if (regInfo == null || regInfo.Registration == null)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            CurRegistration = regInfo.Registration;

            PatientInfo = regInfo.Registration.Patient;

            if (!PatientInfo.AgeOnly.GetValueOrDefault())
            {
                PatientInfo.DOBText = PatientInfo.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
            }

            GetClassicationPatientPrescription();
            SearchGetGenMedProductForSellByPrescriptID(null);
            //KMx: Sau khi load toa thuốc sẽ kiểm tra.
            //Trường hợp toa đã bán rồi, user vẫn đồng ý cập nhật lại toa thuốc.
            if (SelectedPrescription.V_PrescriptionIssuedCase == (long)AllLookupValues.V_PrescriptionIssuedCase.UPDATE_FROM_PRESCRIPT_WAS_SOLD)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0894_G1_ToaDaDuocCNhatTuToaDaXuat), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }

            Int16 isobject = 0;
            if (IsHIPatient)
            {
                //if (SubDate(SelectedPrescription.IssuedDateTime.Value.Date, DateServer) >= Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.MaxDaySellPrescriptInsurance]))
                // Txd 25/05/2014 Replaced ConfigList
                if (SubDate(SelectedPrescription.IssuedDateTime.Value.Date, DateServer) >= Globals.ServerConfigSection.HealthInsurances.MaxDaySellPrescriptInsurance)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0895_G1_BHYTKgTraTienCHoToa));
                    IsHIPatient = false;
                }
            }
            if (IsHIPatient && !IsNotCountInsurance)
            {
                isobject = 1;
            }
            else
            {
                isobject = 0;
            }
            spGetInBatchNumberAndPrice_ByPresciption_InPt(SelectedPrescription.IssueID, StoreID, isobject, SelectedPrescription.RefGenDrugCatID_1);
            isLoadingInfoPatient = false;
            yield break;
        }

        public void btnSearchNangCao()
        {
            Action<IOutwardFromPrescriptionSearch> onInitDlg = delegate (IOutwardFromPrescriptionSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
            };
            GlobalsNAV.ShowDialog<IOutwardFromPrescriptionSearch>(onInitDlg);
        }

        private void SearchPrescription(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int Total = 0;
            //isLoadingSearch = true;

            
            if (SearchCriteria == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0644_G1_Msg_InfoKhCoGTriDeTim), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //KMx: Nếu tìm kiếm mà không có ngày thì phải gán ngày (11/08/2014 09:49).
            if (SearchCriteria.FromDate == null || SearchCriteria.FromDate == DateTime.MinValue)
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-1);
            }
            if (SearchCriteria.ToDate == null || SearchCriteria.ToDate == DateTime.MinValue)
            {
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }


            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSearchPrescription_InPt(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchPrescription_InPt(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    // mo pop up tim
                                    Action<IOutwardFromPrescriptionSearch> onInitDlg = delegate (IOutwardFromPrescriptionSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.PrescriptionList.Clear();
                                        proAlloc.PrescriptionList.TotalItemCount = Total;
                                        proAlloc.PrescriptionList.PageIndex = 0;
                                        proAlloc.PrescriptionList.PageSize = 20;
                                        foreach (Prescription p in results)
                                        {
                                            proAlloc.PrescriptionList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IOutwardFromPrescriptionSearch>(onInitDlg);
                                }
                                else
                                {
                                    SelectedPrescription = results.FirstOrDefault();
                                    if (SelectedPrescription.IsSold)
                                    {
                                        if (MessageBox.Show(eHCMSResources.Z0896_G1_ToaDaBanCoMuonBanTiepKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                        {
                                            return;
                                        }
                                    }
                                    if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                                    {
                                        //Đọc thông tin đăng ký lên
                                        Coroutine.BeginExecute(GetRegistrationInfoTask(SelectedPrescription.PtRegistrationID.Value));
                                    }
                                    else
                                    {
                                        Coroutine.BeginExecute(GetRegistrationInfo_InPtTask(SelectedPrescription.PtRegistrationID.Value));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }

                            SearchCriteria = new PrescriptionSearchCriteria();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        public void spGetInBatchNumberAndPrice_ByPresciption_InPt(long IssueID, long StoreID, Int16 IsObject, long RefGenDrugCatID_1)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginspGetInBatchNumberAndPrice_ByPresciption_InPt(IssueID, StoreID, IsObject, V_MedProductType, RefGenDrugCatID_1, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAndPrice_ByPresciption_InPt(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                                OutwardDrugsCopy = null;
                                SumTotalPrice();
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

        public void btnClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //mai coi lai
            if (e.ClickCount == 1)
            {
                SearchPrescriptionToSell();
            }
        }

        public void SearchPrescriptionToSell()
        {
            if (_currentView != null)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = _currentView.GetValuePreHICode();
                    long value = 0;
                    long.TryParse(_currentView.GetValuePreID(), out value);
                    SearchCriteria.PrescriptID = value;
                }
            }
            SearchPrescription(0, 20);
            //GetCurrentDate();
        }

        #region IHandle<MedDeptCloseSearchPrescriptionEvent> Members

        public void Handle(MedDeptCloseSearchPrescriptionEvent message)
        {
            if (message != null)
            {
                SelectedPrescription = message.SelectedPrescription as Prescription;
                if (SelectedPrescription.IsSold)
                {
                    if (MessageBox.Show(eHCMSResources.Z0896_G1_ToaDaBanCoMuonBanTiepKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
                if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    //Đọc thông tin đăng ký lên
                    Coroutine.BeginExecute(GetRegistrationInfoTask(SelectedPrescription.PtRegistrationID.Value));
                }
                else
                {
                    Coroutine.BeginExecute(GetRegistrationInfo_InPtTask(SelectedPrescription.PtRegistrationID.Value));
                }
            }
        }

        #endregion

        #endregion

        #region Phieu Xuat Member

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchInvoiceOld();
            }
        }

        public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchInvoiceOld();
            }
        }

        //OutInvID
        public void Search_KeyUp_OutInvID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchInvoiceOld();
            }
        }

        public void SearchInvoiceOld()
        {
            if (_currentView != null)
            {
                if (SearchInvoiceCriteria != null)
                {
                    SearchInvoiceCriteria.HICardCode = _currentView.GetValueHICode();
                    SearchInvoiceCriteria.OutInvID = _currentView.GetValueInvoiceID();
                }
            }
            SearchOutwardPrescriptionIndex0(0, Globals.PageSize);
        }

        private void LoadInfoCommonInvoice()
        {
            if (SelectedOutInvoice != null)
            {
                //Kien: Delete data of previous prescription
                SelectedSellVisitor = null;
                strNgayDung = "";
                //20191212 TBL: Anh Tuấn nói comment ra vì không biết để làm gì, khi qua màn hình khác rồi vào lại màn hình này thì (OutwardFromPrescriptionView)this.GetView bị null nên bị lỗi
                //((OutwardFromPrescriptionView)this.GetView()).AutoDrug_Text.Text = "";
                //((OutwardFromPrescriptionView)this.GetView()).chbBH.IsChecked = false;
                //---------------------------------------------------------

                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
                HideShowColumnDelete();
                if (SelectedOutInvoice.SelectedPrescription != null)
                {
                    //KMx: Hàm SearchGetGenMedProductForSellByPrescriptID() để trong Coroutine DoGetInfoPatientPrescription() luôn.
                    //Vì SearchGetGenMedProductForSellByPrescriptID() có dùng biến IsHIPatient. Mà biến IsHIPatient do Coroutine quyết định.
                    //Nên sau khi Coroutine quyết định IsHIPatient xong thì mới gọi hàm SearchGetGenMedProductForSellByPrescriptID()
                    //SearchGetGenMedProductForSellByPrescriptID();
                    if (SelectedOutInvoice.SelectedPrescription.PtRegistrationID.HasValue)
                    {
                        Coroutine.BeginExecute(DoGetInfoPatientInvoice());
                    }
                    //OutwardDrug_GetMaxDayBuyInsurance(SelectedOutInvoice.SelectedPrescription.PatientID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                    //KMx: Không được dùng SelectedOutInvoice.SelectedPrescription.PatientID vì sau khi tính tiền thì PatientID = null.
                }
            }
        }

        private IEnumerator<IResult> DoGetInfoPatientInvoice()
        {
            isLoadingInfoPatient = true;

            if (SelectedOutInvoice == null || SelectedOutInvoice.SelectedPrescription == null || SelectedOutInvoice.SelectedPrescription.PtRegistrationID.GetValueOrDefault() <= 0)
            {
                MessageBox.Show(eHCMSResources.A0656_G1_Msg_InfoKhCoMaDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            var regInfo = new LoadRegistrationInfo_InPtTask(SelectedOutInvoice.SelectedPrescription.PtRegistrationID.GetValueOrDefault(), (int)AllLookupValues.PatientFindBy.NOITRU, LoadRegisSwitch);
            yield return regInfo;

            if (regInfo == null || regInfo.Registration == null)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            CurRegistration = regInfo.Registration;

            PatientInfo = regInfo.Registration.Patient;

            if (!PatientInfo.AgeOnly.GetValueOrDefault())
            {
                PatientInfo.DOBText = PatientInfo.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
            }

            GetClassicationPatientInvoice();
            SearchGetGenMedProductForSellByPrescriptID(null);
            LoadPrescriptionDetailByInvoice(SelectedOutInvoice.outiID);
            isLoadingInfoPatient = false;
            yield break;
        }

        //load chi tiet phieu xuat ban hang theo toa dua vao ma phieu xuat
        private void LoadPrescriptionDetailByInvoice(long OutiID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice_InPt(OutiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice_InPt(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                                if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                                {
                                    OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                                }
                                SumTotalPrice();
                            }
                            SelectedOutInvoiceCopy = SelectedOutInvoice.DeepCopy();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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

        //tim phieu xuat ban theo toa
        private void SearchOutwardPrescriptionIndex0(int PageIndex, int PageSize)
        {
            int Total = 0;
            this.ShowBusyIndicator();

            //KMx: Nếu tìm kiếm mà không theo tiêu chí nào hết thì phải giới hạn ngày (08/08/2014 09:51).
            if (SearchInvoiceCriteria == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchInvoiceCriteria.PatientCode) && string.IsNullOrEmpty(SearchInvoiceCriteria.CustomerName) && string.IsNullOrEmpty(SearchInvoiceCriteria.HICardCode) && string.IsNullOrEmpty(SearchInvoiceCriteria.OutInvID))
            {
                SearchInvoiceCriteria.fromdate = Globals.GetCurServerDateTime().AddDays(-1);
                SearchInvoiceCriteria.todate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchInvoiceCriteria.fromdate = null;
                SearchInvoiceCriteria.todate = null;
            }


            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchInvoiceCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus_InPt(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {

                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IOutwardFromPrescriptionSearchInvoice> onInitDlg = delegate (IOutwardFromPrescriptionSearchInvoice proAlloc)
                                    {
                                        proAlloc.SearchInvoiceCriteria = SearchInvoiceCriteria.DeepCopy();
                                        proAlloc.OutwardDrugMedDeptInvoices.Clear();
                                        proAlloc.OutwardDrugMedDeptInvoices.TotalItemCount = Total;
                                        proAlloc.OutwardDrugMedDeptInvoices.PageIndex = 0;
                                        foreach (OutwardDrugMedDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardDrugMedDeptInvoices.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IOutwardFromPrescriptionSearchInvoice>(onInitDlg);
                                }
                                else
                                {
                                    SelectedOutInvoice = results.FirstOrDefault();
                                    //KMx: Phải cập nhật SelectedPrescription, nếu không toa vừa load lên sẽ lấy PrescriptID của toa cũ. 
                                    SelectedPrescription = SelectedOutInvoice.SelectedPrescription;
                                    LoadInfoCommonInvoice();
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            //KMx: Sau khi set new thì phải set TypID lại, để lần sau tìm phiếu xuất của bán theo toa thôi. Nếu không là bị lỗi tìm phiếu xuất của bán lẻ luôn (25/02/2013 18:03)
                            SearchInvoiceCriteria = new SearchOutwardInfo();
                            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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


        private void GetOutWardDrugInvoiceByID_InPt(long OutwardID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceByID_InPt(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutInvoice = contract.EndGetOutWardDrugInvoiceByID_InPt(asyncResult);
                            LoadInfoCommonInvoice();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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

        #region IHandle<MedDeptCloseSearchPrescriptionInvoiceEvent> Members

        public void Handle(MedDeptCloseSearchPrescriptionInvoiceEvent message)
        {
            if (message != null)
            {
                SelectedOutInvoice = message.SelectedInvoice as OutwardDrugMedDeptInvoice;
                SelectedPrescription = SelectedOutInvoice.SelectedPrescription;
                LoadInfoCommonInvoice();
            }

        }

        #endregion
        #endregion

        #region DataGrid Event Member

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;

        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (!CheckQuantity(e.Row.DataContext))
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {

                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        public void grdPrescription_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            SumTotalPrice();
            //tam thoi ko can hien chi tiet nen ko can goi ham tinh nay
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0661_G1_Msg_InfoKhCoPhXuatDeIn,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0897_G1_PhXuatDaHuyKgTheIn),eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0898_G1_ThuocGayNghien.ToUpper();
                    }
                    else if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0899_G1_ThuocHuongTamThan.ToUpper();
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.G0787_G1_Thuoc.ToUpper();
                    }
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.G2907_G1_YCu.ToUpper();

                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.T1616_G1_HC.ToUpper();

                }
                proAlloc.eItem = ReportName.MEDDEPT_OUTWARD_FROM_PRESCRIPTION;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        private void PrintSilient()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCollectionDrugInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetCollectionDrugInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, "A5");
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        #endregion

        #region Tạo bill

        private void DoCreateBill(OutwardDrugMedDeptInvoice Invoice)
        {

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginCreateBillForOutwardFromPrescription(Invoice, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long InPatientBillingInvID = 0;
                            bool result = contract.EndCreateBillForOutwardFromPrescription(out InPatientBillingInvID, asyncResult);

                            if (result && InPatientBillingInvID > 0)
                            {
                                MessageBox.Show(eHCMSResources.A1000_G1_Msg_InfoTaoBillOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                Invoice.InPatientBillingInvID = InPatientBillingInvID;
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A1001_G1_Msg_InfoTaoBillFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        private void DoDeleteBill(OutwardDrugMedDeptInvoice Invoice)
        {

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteBillForOutwardFromPrescription(Invoice.InPatientBillingInvID.GetValueOrDefault(), Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndDeleteBillForOutwardFromPrescription(asyncResult);

                            if (result)
                            {
                                MessageBox.Show(eHCMSResources.K0470_G1_XoaBillOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                Invoice.InPatientBillingInvID = 0;
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0471_G1_XoaBillFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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

        public void btnCreateBill()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0662_G1_Msg_InfoKhCoPhXuatDeTaoBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0945_G1_Msg_InfoKhTheTaoBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault() > 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0901_G1_PhXuatDaTaoBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.Z0902_G1_TaoBillKgTheCNhatPhXuat, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DoCreateBill(SelectedOutInvoice);
            }
        }

        public void btnDeleteBill()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0663_G1_Msg_InfoKhCoPhXuatDeXoaBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault() <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0934_G1_Msg_InfoKhTheXoaPh2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            DoDeleteBill(SelectedOutInvoice);
        }

        public void btnPrintBill()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0660_G1_Msg_InfoKhCoPhXuatDeInBiill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0897_G1_PhXuatDaHuyKgTheIn), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault() <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0903_G1_PhXuatChuaTaoBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault();
                proAlloc.eItem = ReportName.MEDDEPT_PRINT_BILL;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }


        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private byte HI;
        private bool IsHIPatient = false;

        private ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellListByPrescriptID;
        private ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellList;

        private ObservableCollection<GetGenMedProductForSell> _GetGenMedProductForSellSum;
        public ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellListSum
        {
            get { return _GetGenMedProductForSellSum; }
            set
            {
                if (_GetGenMedProductForSellSum != value)
                    _GetGenMedProductForSellSum = value;
                NotifyOfPropertyChange(() => GetGenMedProductForSellListSum);
            }
        }

        private ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellTemp;

        private GetGenMedProductForSell _SelectedSellVisitor;
        public GetGenMedProductForSell SelectedSellVisitor
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
                SearchGetGenMedProductForSell(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetGenMedProductForSell;
            }

        }
        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetGenMedProductForSellList
                      group hd by new
                      {
                          hd.GenMedProductID,
                          hd.Code,
                          hd.BrandName,
                          hd.UnitName,
                          hd.Qty,
                          hd.DayRpts,

                          hd.QtyForDay,
                          hd.V_DrugType,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,

                          hd.HIPaymentPercent
                      } into hdgroup

                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          Code = hdgroup.Key.Code,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Qty = hdgroup.Key.Qty,
                          DayRpts = hdgroup.Key.DayRpts,

                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,
                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,
                          HIPaymentPercent = hdgroup.Key.HIPaymentPercent
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetGenMedProductForSell item = new GetGenMedProductForSell();
                item.GenMedProductID = hhh.ToList()[i].GenMedProductID;
                item.Code = hhh.ToList()[i].Code;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                item.Qty = hhh.ToList()[i].Qty;
                item.DayRpts = hhh.ToList()[i].DayRpts;

                item.V_DrugType = hhh.ToList()[i].V_DrugType;
                item.QtyForDay = hhh.ToList()[i].QtyForDay;
                item.QtyMaxAllowed = hhh.ToList()[i].QtyMaxAllowed;
                item.QtySchedMon = hhh.ToList()[i].QtySchedMon;
                item.QtySchedTue = hhh.ToList()[i].QtySchedTue;
                item.QtySchedWed = hhh.ToList()[i].QtySchedWed;
                item.QtySchedThu = hhh.ToList()[i].QtySchedThu;
                item.QtySchedFri = hhh.ToList()[i].QtySchedFri;
                item.QtySchedSat = hhh.ToList()[i].QtySchedSat;
                item.QtySchedSun = hhh.ToList()[i].QtySchedSun;
                item.SchedBeginDOW = hhh.ToList()[i].SchedBeginDOW;
                item.DispenseVolume = hhh.ToList()[i].DispenseVolume;

                item.HIPaymentPercent = hhh.ToList()[i].HIPaymentPercent;
                GetGenMedProductForSellListSum.Add(item);
            }
            if (IsCode.GetValueOrDefault())
            {
                if (GetGenMedProductForSellListSum != null && GetGenMedProductForSellListSum.Count > 0)
                {
                    //KMx: Lý do comment code bên dưới: Khi người dùng tìm bằng code, nhưng không nhập prefix. VD: Hàng có code = "med0001", nhưng người dùng chỉ nhập "0001" (14/12/2014 14:23).
                    //var item = GetGenMedProductForSellListSum.Where(x => x.Code == txt);
                    //if (item != null && item.Count() > 0)
                    //{
                    //    SelectedSellVisitor = item.ToList()[0];
                    //}
                    //else
                    //{
                    //    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    //}
                    SelectedSellVisitor = GetGenMedProductForSellListSum[0];
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
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetGenMedProductForSellListSum;
                    au.PopulateComplete();
                }
            }

        }

        private int SubDate(DateTime date1, DateTime date2)
        {
            TimeSpan subDate = date2 - date1;
            return subDate.Days;
        }


        private void SearchGetGenMedProductForSellByPrescriptID(GenericCoRoutineTask genTask)
        {
            if(SelectedOutInvoice == null)
            {
                return;
            }

            this.ShowBusyIndicator();

            //long? IssueID = null;
            //if (SelectedOutInvoice != null)
            //{
            //    IssueID = SelectedOutInvoice.IssueID;
            //}

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetGenMedProductForSellAutoComplete_ForPrescriptionByID(HI, IsHIPatient, StoreID, V_MedProductType, SelectedOutInvoice.IssueID, SelectedOutInvoice.RefGenDrugCatID_1,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetGenMedProductForSellAutoComplete_ForPrescriptionByID(asyncResult);
                                GetGenMedProductForSellListByPrescriptID = results.ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }



        private bool? IsCode = false;
        private void SearchGetGenMedProductForSell(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetailPrescript = true;
            }

            //long? IssueID = null;
            //if (SelectedOutInvoice != null)
            //{
            //    IssueID = SelectedOutInvoice.IssueID;
            //}

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetGenMedProductForSellAutoComplete_ForPrescription(HI, (IsHIPatient && !IsNotCountInsurance), Name, StoreID, SelectedOutInvoice.IssueID, IsCode, V_MedProductType, SelectedOutInvoice.RefGenDrugCatID_1
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetGenMedProductForSellAutoComplete_ForPrescription(asyncResult);
                            GetGenMedProductForSellList.Clear();
                            GetGenMedProductForSellListSum.Clear();
                            LaySoLuongTheoNgayDung(results);
                            ListDisplayAutoComplete();

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsCode.GetValueOrDefault())
                            {
                                isLoadingDetailPrescript = false;
                                if (AxQty != null)
                                {
                                    AxQty.Focus();
                                }
                            }
                            else
                            {
                                if (au != null)
                                {
                                    au.Focus();
                                }
                            }
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void LaySoLuongTheoNgayDung(IList<GetGenMedProductForSell> results)
        {
            GetGenMedProductForSellTemp = results.ToObservableCollection();

            if (GetGenMedProductForSellTemp == null)
            {
                GetGenMedProductForSellTemp = new ObservableCollection<GetGenMedProductForSell>();
            }

            if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
            {
                foreach (OutwardDrugMedDept d in OutwardDrugsCopy)
                {
                    var value = results.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetGenMedProductForSell s in value.ToList())
                        {
                            s.Remaining = s.Remaining + (int)d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + (int)d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetGenMedProductForSell p = d.GetGenMedProductForSell;
                        p.Remaining = (int)d.OutQuantity;
                        p.RemainingFirst = (int)d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        GetGenMedProductForSellTemp.Add(p);
                        // d = null;
                    }
                }
            }

            foreach (GetGenMedProductForSell s in GetGenMedProductForSellTemp)
            {
                if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
                {
                    foreach (OutwardDrugMedDept d in SelectedOutInvoice.OutwardDrugMedDepts)
                    {
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - (int)d.OutQuantity;
                        }
                    }
                }
                GetGenMedProductForSellList.Add(s);
            }
        }

        private bool CheckValidDrugAuto(GetGenMedProductForSell temp)
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
            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        p1.RequestQty += p.RequestQty;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetGenMedProductForSell.InsuranceCover;

                    if (p.InwardDrugMedDept == null)
                    {
                        p.InwardDrugMedDept = new InwardDrugMedDept();
                        p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
                        p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrugMedDept.NormalPrice = p.OutPrice;
                    p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
                    p.InwardDrugMedDept.HIAllowedPrice = p.HIAllowedPrice;
                    if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                    {
                        p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
                    }

                    SelectedOutInvoice.OutwardDrugMedDepts.Add(p);
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

        private void ChooseBatchNumber(GetGenMedProductForSell value)
        {
            var items = GetGenMedProductForSellList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
            foreach (GetGenMedProductForSell item in items)
            {
                if (item.Remaining <= 0)
                {
                    continue;
                }

                OutwardDrugMedDept p = new OutwardDrugMedDept();

                //KMx: Nếu không new class bên dưới, thì không thể lưu thuốc thêm bằng tay, lỗi khi convert to xml gừi xuống database (29/11/2014 17:20).
                p.RefGenericDrugDetail = new RefGenMedProductDetails();
                p.RefGenericDrugDetail.GenMedProductID = item.GenMedProductID;
                /////////////////////////

                item.GenMedProductIDChanged = value.GenMedProductIDChanged;
                p.GetGenMedProductForSell = item;
                p.GenMedProductID = item.GenMedProductID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.OutPrice = item.OutPrice;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.HIAllowedPrice = item.HIAllowedPrice;

                //KMx: Nếu thuốc muốn thêm vào giống với thuốc đã có trong phiếu phải gán lại những thuộc tính bên dưới.
                //Nếu không gán lại thì hàm CheckInsurance_New(), btnNgayDung_New() và ListDisplayAutoComplete() sẽ sai ở dòng 
                //group hd by new
                //    { 
                //        hd.GenMedProductID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.QtyMaxAllowed, hd.QtySchedMon, hd.QtySchedTue,
                //        hd.QtySchedWed, hd.QtySchedThu, hd.QtySchedFri, hd.QtySchedSat, hd.QtySchedSun, hd.SchedBeginDOW, hd.DispenseVolume,
                //        hd.GetGenMedProductForSell.InsuranceCover, hd.GetGenMedProductForSell.BrandName
                //    } into hdgroup

                p.V_DrugType = item.V_DrugType;
                p.QtyForDay = (decimal)item.QtyForDay;
                p.DayRpts = item.DayRpts;
                p.QtyMaxAllowed = item.QtyMaxAllowed;
                p.QtySchedMon = item.QtySchedMon;
                p.QtySchedTue = item.QtySchedTue;
                p.QtySchedWed = item.QtySchedWed;
                p.QtySchedThu = item.QtySchedThu;
                p.QtySchedFri = item.QtySchedFri;
                p.QtySchedSat = item.QtySchedSat;
                p.QtySchedSun = item.QtySchedSun;
                p.SchedBeginDOW = item.SchedBeginDOW;
                p.DispenseVolume = item.DispenseVolume;

                p.HIPaymentPercent = item.HIPaymentPercent;

                if (item.Remaining - value.RequiredNumber < 0)
                {
                    if (value.Qty > item.Remaining)
                    {
                        p.RequestQty = item.Remaining;
                        value.Qty = value.Qty - item.Remaining;
                    }
                    else
                    {
                        p.RequestQty = value.Qty;
                        value.Qty = 0;
                    }
                    value.RequiredNumber = value.RequiredNumber - item.Remaining;

                    p.OutQuantity = item.Remaining;

                    CheckBatchNumberExists(p);
                    item.Remaining = 0;
                }
                else
                {
                    p.RequestQty = value.Qty;
                    value.Qty = 0;

                    p.OutQuantity = (int)value.RequiredNumber;

                    CheckBatchNumberExists(p);
                    item.Remaining = item.Remaining - (int)value.RequiredNumber;
                    break;
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrug(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (CheckValidDrugAuto(SelectedDrugForSell))
            {
                ChooseBatchNumber(SelectedDrugForSell);
            }
            else
            {
                string ErrorMessage = string.Format("{0}.", eHCMSResources.Z0888_G1_ThuocThaoTacBiLoi);
                if (SelectedDrugForSell.BrandName != null)
                {
                    ErrorMessage = string.Format("{0} ", eHCMSResources.G0787_G1_Thuoc) + string.Format(eHCMSResources.Z0915_G1_0DaBiLoiKTraLai, SelectedDrugForSell.BrandName);
                }
                MessageBox.Show(ErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            }
        }

        private void ReCountQtyRequest(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (SelectedOutInvoice.OutwardDrugMedDepts == null)
            {
                SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
            }
            var results1 = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == SelectedDrugForSell.GenMedProductID);
            if (results1 != null && results1.Count() > 0)
            {
                foreach (OutwardDrugMedDept p in results1)
                {
                    if (p.RequestQty > p.OutQuantity)
                    {
                        p.RequestQty = (int)p.OutQuantity;
                    }
                    SelectedDrugForSell.Qty = SelectedDrugForSell.Qty - (int)p.RequestQty;
                }
            }
        }

        public bool CheckValidQty(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (SelectedOutInvoice != null && SelectedDrugForSell != null)
            {
                int intOutput = 0;
                if (SelectedDrugForSell.RequiredNumber <= 0
                    || !Int32.TryParse(SelectedDrugForSell.RequiredNumber.ToString(), out intOutput) //Nếu số lượng không phải là số nguyên.
                    || SelectedDrugForSell.RequiredNumber > SelectedDrugForSell.Remaining) //Nếu số lượng muốn thêm > số lượng còn trong kho.
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0890_G1_SLgKgHopLe), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0400_G1_ChonThuocCanThem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
        }


        //Kết hợp 2 hàm ReCountQtyRequest() và AddListOutwardDrug() (09/06/2014 16:40)
        public void ReCountQtyAndAddList(GetGenMedProductForSell SelectedDrugForSell)
        {
            if (!CheckValidQty(SelectedDrugForSell))
                return;
            else
            {
                ReCountQtyRequest(SelectedDrugForSell);
                AddListOutwardDrug(SelectedDrugForSell);
            }
        }

        //Kiên: Những module có hàm tương tự (Khi module này sai thì phải sửa những module khác).
        //1. Bán thuốc theo toa.
        //2. Xuất nội bộ.
        public void AddItem()
        {
            //if (DrugInPrescriptionItem != null && DrugInPrescriptionItem.GenMedProductID > 0 && SelectedSellVisitor != null)
            //{
            //    SelectedSellVisitor.GenMedProductIDChanged = DrugInPrescriptionItem.GenMedProductID;
            //    SelectedSellVisitor.Qty = DrugInPrescriptionItem.Qty.DeepCopy();
            //    if (lstDrugInPrescriptionDeleted == null)
            //    {
            //        lstDrugInPrescriptionDeleted = new ObservableCollection<GetGenMedProductForSell>();
            //    }
            //    lstDrugInPrescriptionDeleted.Add(DrugInPrescriptionItem.DeepCopy());
            //    lstDrugInPrescription.Remove(DrugInPrescriptionItem);
            //}
            //ReCountQtyRequest(SelectedSellVisitor);
            //AddListOutwardDrug(SelectedSellVisitor, true);
            // DrugInPrescriptionItem = null;

            //Kiên: Combine 2 functions: ReCountQtyRequest and AddListOutwardDrug
            ReCountQtyAndAddList(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetGenMedProductForSell> BatchNumberListTemp;
        private ObservableCollection<GetGenMedProductForSell> BatchNumberListShow;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductID;
        private ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductIDFirst;

        private OutwardDrugMedDept _SelectedOutwardDrug;
        public OutwardDrugMedDept SelectedOutwardDrug
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

        private void GetGenMedProductForSellBatchNumber(long GenMedProductID)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(GenMedProductID, V_MedProductType, StoreID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugListByGenMedProductIDFirst != null)
            {
                foreach (OutwardDrugMedDept d in OutwardDrugListByGenMedProductIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.GenMedProductID == d.GenMedProductID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetGenMedProductForSell s in value.ToList())
                        {
                            s.Remaining = s.Remaining + (int)d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + (int)d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetGenMedProductForSell p = d.GetGenMedProductForSell;
                        p.Remaining = (int)d.OutQuantity;
                        p.RemainingFirst = (int)d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetGenMedProductForSell s in BatchNumberListTemp)
            {
                if (OutwardDrugListByGenMedProductID.Count > 0)
                {
                    foreach (OutwardDrugMedDept d in OutwardDrugListByGenMedProductID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - (int)d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberForPrescription> onInitDlg = delegate (IChooseBatchNumberForPrescription proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByGenMedProductID != null)
                    {
                        proAlloc.OutwardDrugListByGenMedProductID = OutwardDrugListByGenMedProductID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberForPrescription>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        #endregion

        #region Chon Lo Member

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long GenMedProductID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByGenMedProductID = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByGenMedProductIDFirst = OutwardDrugsCopy.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                }
                GetGenMedProductForSellBatchNumber(GenMedProductID);
            }
        }

        #region IHandle<ChooseBatchNumberForPrescriptionEvent> Members

        public void Handle(ChooseBatchNumberForPrescriptionEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetGenMedProductForSell.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetGenMedProductForSell.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetGenMedProductForSell.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetGenMedProductForSell.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetGenMedProductForSell.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetGenMedProductForSell.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetGenMedProductForSell.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;

                //SelectedOutwardDrug.GetGenMedProductForSell.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<ChooseBatchNumberForPrescriptionResetQtyEvent> Members

        public void Handle(ChooseBatchNumberForPrescriptionResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetGenMedProductForSell.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetGenMedProductForSell.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetGenMedProductForSell.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetGenMedProductForSell.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetGenMedProductForSell.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetGenMedProductForSell.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetGenMedProductForSell.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                //SelectedOutwardDrug.GetGenMedProductForSell.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                //SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        #endregion

        #region RadioButton Event Member
        public void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            HI = 0;
        }
        public void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            HI = 1;
        }
        public void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            HI = 2;
        }

        #endregion

        #region Xoa OutwardDrug Details Va refesh Du Lieu Member
        private void DeleteOutwardDrugs(OutwardDrugMedDept temp)
        {
            OutwardDrugMedDept p = temp.DeepCopy();
            SelectedOutInvoice.OutwardDrugMedDepts.Remove(temp);
            foreach (OutwardDrugMedDept item in SelectedOutInvoice.OutwardDrugMedDepts)
            {
                if (item.GenMedProductID == p.GenMedProductID)
                {
                    item.RequestQty = item.RequestQty + p.RequestQty;
                    break;
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrugMedDept p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugMedDepts.Remove(SelectedOutwardDrug);

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
            if (SelectedOutwardDrug != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
            {
                //if (MessageBox.Show(eHCMSResources.Z0892_CoMuonXoaThuocKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                DeleteInvoiceDrugInObject();
                // }
            }
            else
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0904_G1_PhDaThuTienHoacKC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btnNew()
        {
            // if (MessageBox.Show(eHCMSResources.A0141_G1_Msg_ConfTaoMoiPhBanThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            // {
            RefeshData();
            //  }
        }

        #endregion


        #region "Dược Sĩ Sửa Lại Toa Thuốc"

        private bool _CanEditPrescription = false;
        public bool CanEditPrescription
        {
            get { return _CanEditPrescription; }
            set
            {
                if (_CanEditPrescription != value)
                {
                    _CanEditPrescription = value;
                    NotifyOfPropertyChange(() => CanEditPrescription);
                }
            }
        }


        //KMx: Không có sử dụng
        public void btEditPrescriptions()
        {
            //Đọc lại thông tin Toa Thuốc Này rồi Popup Sửa
            //Globals.PatientAllDetails.PatientInfo = PatientInfo;
            Action<IePrescriptionOld> onInitDlg = delegate (IePrescriptionOld typeInfo)
            {
                typeInfo.DuocSi_IsEditingToaThuoc = true;
                typeInfo.btDuocSiEditVisibility = true;
                typeInfo.btChonChanDoanVisibility = false;
                Globals.EventAggregator.Publish(new DuocSi_EditingToaThuocEvent { SelectedPrescription = PrecriptionsBeforeEdit });
            };
            GlobalsNAV.ShowDialog<IePrescriptionOld>(onInitDlg);
        }

        private Prescription _PrecriptionsBeforeEdit;
        public Prescription PrecriptionsBeforeEdit
        {
            get
            {
                return _PrecriptionsBeforeEdit;
            }
            set
            {
                if (_PrecriptionsBeforeEdit != value)
                {
                    _PrecriptionsBeforeEdit = value;
                    NotifyOfPropertyChange(() => PrecriptionsBeforeEdit);
                }
            }
        }

        private void Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID)
        {
            isLoadingPrescriptID = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.G0696_G1_TTinToaThuoc) });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescription_ByPrescriptIDIssueID(PrescriptID, IssueID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PrecriptionsBeforeEdit = contract.EndPrescription_ByPrescriptIDIssueID(asyncResult);
                            if (SelectedPrescription != null)
                            {
                                SelectedPrescription.IssuedDateTime = PrecriptionsBeforeEdit.IssuedDateTime;
                            }
                            CanEditPrescription = PrecriptionsBeforeEdit.CanEdit;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingPrescriptID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        #region Huy Phieu Member
        public void btnCancel(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault(0) > 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0940_G1_Msg_InfoKhTheHuy2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.Z0906_G1_CoMuonHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CancalOutwardInvoice();
            }
        }

        private void CancalOutwardInvoice()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteOutwardInvoice(SelectedOutInvoice.outiID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool result = contract.EndDeleteOutwardInvoice(asyncResult);

                            if (result)
                            {
                                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0613_G1_Msg_InfoHuyOK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                GetOutWardDrugInvoiceByID_InPt(SelectedOutInvoice.outiID);
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0907_G1_HuyThatBai), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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


        #region Cap Nhat Phieu Da Thu Tien Member
        public void btnEditPayed()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            //KMx: Kiểm tra thời gian tối đa cho phép cập nhật (23/07/2014 14:55).
            int AllowTimeUpdateOutInvoice = Globals.ServerConfigSection.PharmacyElements.AllowTimeUpdateOutInvoice;
            if (AllowTimeUpdateOutInvoice > 0)
            {
                TimeSpan t = Globals.ServerDate.Value - SelectedOutInvoice.OutDate.GetValueOrDefault();
                if (t.TotalHours >= AllowTimeUpdateOutInvoice)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1291_G1_TGianToiDaChoPhepCNhat, AllowTimeUpdateOutInvoice.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault(0) > 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0921_G1_PhXuatDaTaoBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.outiID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0922_G1_PhXuatChuaLuu), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0947_G1_Msg_InfoKhTheCNhat3), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            Action<IEditOutwardFromPrescription> onInitDlg = delegate (IEditOutwardFromPrescription proAlloc)
            {
                proAlloc.SelectedOutInvoice = SelectedOutInvoice.DeepCopy();
                proAlloc.PatientInfo = PatientInfo.DeepCopy();
                proAlloc.CurRegistration = CurRegistration.DeepCopy();
                proAlloc.GetGenMedProductForSellListByPrescriptID = GetGenMedProductForSellListByPrescriptID.DeepCopy();
                proAlloc.SelectedPrescription = SelectedPrescription.DeepCopy();
                proAlloc.SelectedOutInvoiceCopy = SelectedOutInvoice.DeepCopy();
                proAlloc.OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                proAlloc.GetClassicationPatientInvoice();
                proAlloc.TotalInvoicePrice = TotalInvoicePrice.DeepCopy();
                proAlloc.TotalHIPayment = TotalHIPayment.DeepCopy();
                proAlloc.TotalPatientPayment = TotalPatientPayment.DeepCopy();
                //var instance = proAlloc as Conductor<object>;
                //instance.DisplayName = eHCMSResources.Z0918_G1_SuaPhXuatToaDaDuocThuTien;
            };
            GlobalsNAV.ShowDialog<IEditOutwardFromPrescription>(onInitDlg);
        }

        #endregion

        #region IHandle<MedDeptCloseEditPrescription> Members

        public void Handle(MedDeptCloseEditPrescription message)
        {
            if (message != null && this.IsActive)
            {
                if (message.SelectedOutwardInvoice != null)
                {
                    OutwardDrugsCopy = message.SelectedOutwardInvoice.OutwardDrugMedDepts;
                    GetOutWardDrugInvoiceByID_InPt(message.SelectedOutwardInvoice.outiID);
                }
            }
        }

        #endregion

        public void CheckBoxBH_Checked(object sender, RoutedEventArgs e)
        {
            FunctionNotCountBHYT();
        }

        private void FunctionNotCountBHYT()
        {
            IsNotCountInsurance = true;
            //neu la benh nhan BH,khong muon tinh BH thi cap nhat co la da ban va thay doi gia
            if (SelectedOutInvoice != null && IsHIPatient && SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                foreach (OutwardDrugMedDept p in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    //vi khi luu xuong a Tuyen lay gia trong p.InwardDrug nen phai gan gia tri lai nhu vay

                    //if (p.GetGenMedProductForSell != null && (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowedPharmacyChangeHIPrescript]) == 1 || p.RequestQty > 0))
                    // Txd 25/05/2014 Replaced ConfigList
                    if (p.GetGenMedProductForSell != null && (Globals.ServerConfigSection.PharmacyElements.AllowedPharmacyChangeHIPrescript == 1 || p.RequestQty > 0))
                    {
                        p.OutPrice = p.GetGenMedProductForSell.NormalPrice;
                        p.ChargeableItem.HIAllowedPrice = 0;
                        p.InwardDrugMedDept.NormalPrice = p.OutPrice;
                        p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
                        p.HIAllowedPrice = 0;
                        p.PriceDifference = 0;
                        p.TotalPriceDifference = 0;
                        SumTotalPrice();
                    }
                }
            }
        }

        public void CheckBoxBH_Unchecked(object sender, RoutedEventArgs e)
        {
            IsNotCountInsurance = false;
            //neu la benh nhan BH,khong muon tinh BH thi cap nhat co la da ban va thay doi gia
            if (SelectedOutInvoice != null && IsHIPatient && SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                foreach (OutwardDrugMedDept p in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    //vi khi luu xuong a Tuyen lay gia trong p.InwardDrug nen phai gan gia tri lai nhu vay
                    
                    //if (p.HI.GetValueOrDefault() && p.GetGenMedProductForSell != null && (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowedPharmacyChangeHIPrescript]) == 1 || p.RequestQty > 0))
                    // Txd 25/05/2014 Replaced ConfigList
                    if (p.HI.GetValueOrDefault() && p.GetGenMedProductForSell != null && (Globals.ServerConfigSection.PharmacyElements.AllowedPharmacyChangeHIPrescript == 1 || p.RequestQty > 0))
                    {
                        p.OutPrice = p.GetGenMedProductForSell.PriceForHIPatient;
                        p.InwardDrugMedDept.NormalPrice = p.OutPrice;
                        p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;

                        p.ChargeableItem.HIAllowedPrice = p.GetGenMedProductForSell.HIAllowedPriceNoChange;
                        p.HIAllowedPrice = p.ChargeableItem.HIAllowedPrice;
                        if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                        {
                            p.PriceDifference = p.OutPrice - p.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0);
                            p.TotalPriceDifference = p.PriceDifference * p.OutQuantity;
                        }
                    }
                    SumTotalPrice();
                }
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
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    SearchGetGenMedProductForSell(Code, true);
                }
            }
        }

        public void Code_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchGetGenMedProductForSell((sender as TextBox).Text, true);
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
                    return _VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value && SelectedOutInvoice.CanSaveAndPaidPrescript;
                    _VisibilityCode = !_VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
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
                if (SelectedOutInvoice != null)
                {
                    return !_VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
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

        //Tìm phiếu xuất cũ.
        public void btnClick_MouseLeftButtonDown_Invoice(object sender, MouseButtonEventArgs e)
        {
            //mai coi lai
            if (e.ClickCount == 1)
            {
                SearchInvoiceOld();
            }
        }

        public void btnSearchInvoiceAdvance()
        {
            Action<IOutwardFromPrescriptionSearchInvoice> onInitDlg = delegate (IOutwardFromPrescriptionSearchInvoice proAlloc)
            {
                proAlloc.SearchInvoiceCriteria = SearchInvoiceCriteria.DeepCopy();
            };
            GlobalsNAV.ShowDialog<IOutwardFromPrescriptionSearchInvoice>(onInitDlg);
        }

        private void HideShowColumnDelete()
        {
            if (grdPrescription != null)
            {
                //KMx: Hiện tại khoa dược chưa có phân quyền (03/12/2014 10:35).
                //if (SelectedOutInvoice.CanSaveAndPaidPrescript && mBanThuocTheoToa_Them)
                if (SelectedOutInvoice.CanSaveAndPaidPrescript)
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool IsBenhNhanNoiTru()
        {
            if (SelectedPrescription != null)
            {
                if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private IEnumerator<IResult> GetRegistrationInfoTask(long PtRegistrationID)
        {
            //Đọc thông tin coi đc Sửa không
            Prescription_ByPrescriptIDIssueID(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID);
            LoadInfoCommon();

            yield break;
        }

        private IEnumerator<IResult> GetRegistrationInfo_InPtTask(long PtRegistrationID)
        {
            //Đọc thông tin coi đc Sửa không
            //Prescription_ByPrescriptIDIssueID(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID);
            LoadInfoCommon();

            yield break;
        }
        #region Thu Tien Thuoc Doc Lap Member

        #region IHandle<PharmacyPayEvent> Members

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID);
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }


        #endregion

        #endregion

        public void btnUp()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex > 0)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrugMedDept Up = SelectedOutInvoice.OutwardDrugMedDepts[i - 1].DeepCopy();
                OutwardDrugMedDept Down = SelectedOutInvoice.OutwardDrugMedDepts[i].DeepCopy();
                SelectedOutInvoice.OutwardDrugMedDepts[i] = Up;
                SelectedOutInvoice.OutwardDrugMedDepts[i - 1] = Down;
                grdPrescription.SelectedIndex = i - 1;
            }
        }

        public void btnDown()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex < SelectedOutInvoice.OutwardDrugMedDepts.Count() - 1)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrugMedDept Up = SelectedOutInvoice.OutwardDrugMedDepts[i].DeepCopy();
                OutwardDrugMedDept Down = SelectedOutInvoice.OutwardDrugMedDepts[i + 1].DeepCopy();
                SelectedOutInvoice.OutwardDrugMedDepts[i] = Down;
                SelectedOutInvoice.OutwardDrugMedDepts[i + 1] = Up;
                grdPrescription.SelectedIndex = i + 1;
            }
        }

        private IEnumerator<IResult> DoSaveDrugs(OutwardDrugMedDeptInvoice Invoice, bool value)
        {
            var res = new SaveGenMedProductTask(Invoice);
            yield return res;

            SelectedOutInvoice = res.SavedOutwardInvoice;

            ClearData();
            OutwardDrugsCopy = res.SavedOutwardInvoice.OutwardDrugMedDepts;

            NotifyOfPropertyChange(() => VisibilityName);
            NotifyOfPropertyChange(() => VisibilityCode);
            HideShowColumnDelete();

            if (SelectedOutInvoice.SelectedPrescription != null)
            {
                //lay tat ca nhung thuoc ton (tung lo) theo ma toa thuoc(co bao nhieu lo lay het ra luon) 
                //var OutWardDrugInvoiceVisitor = new SearchGetGenMedProductForSellByPrescriptIDTask(SelectedOutInvoice.PrescriptID, HI, IsHIPatient, StoreID);
                //yield return OutWardDrugInvoiceVisitor;

                //GetGenMedProductForSellListByPrescriptID = OutWardDrugInvoiceVisitor.DrugForSellVisitorList;

                yield return GenericCoRoutineTask.StartTask(SearchGetGenMedProductForSellByPrescriptID);

                GetClassicationPatientInvoice();

                SumTotalPrice();

            }
            else
            {
                ClientLoggerHelper.LogError("SelectedOutInvoice.SelectedPrescription == null");
            }

            yield break;


        }

    }
}


