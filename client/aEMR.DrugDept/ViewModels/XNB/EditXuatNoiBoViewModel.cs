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
using aEMR.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
* 20190608 #001 TTM:   BM 0011796: Không mặc định kho khi cập nhật phiếu xuất mà phải lấy thông tin kho ban đầu của phiếu xuất.
* 20190608 #002 TTM:   BM 0011793: P1: Fix lỗi khi gõ tìm thuốc => tự động bôi đen toàn bộ thông tin trong AutoComplete.
*                                  P2: Khi phiếu xuất cũ là xuất nội bộ thì không được đổi thông tin giá.
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IEditXuatNoiBo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditXuatNoiBoViewModel : ViewModelBase, IEditXuatNoiBo, IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent>
        , IHandle<DrugDeptEditChooseBatchNumberEvent>, IHandle<DrugDeptEditChooseBatchNumberResetQtyEvent>
        , IHandle<DrugDeptCloseSearchStorageEvent>
        , IHandle<DrugDeptCloseSearchHospitalEvent>
        , IHandle<ItemSelected<Patient>>
        , IHandle<DrugDeptCloseSearchStaffEvent>
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

        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail || IsLoadingRefGenericDrugCategory); }
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

        [ImportingConstructor]
        public EditXuatNoiBoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            
            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            Coroutine.BeginExecute(GetLookupOutputTo());
            Coroutine.BeginExecute(DoGetByOutPriceLookups());

            LoadRefOutputType();
            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            RefeshData();
            //▼====== #001
            //SetDefaultForStore();
            //▲====== #001
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            Coroutine.BeginExecute(DoGetStoreToSell());
        }
        private void RefeshData()
        {
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugMedDeptInvoice();
            SelectedOutInvoice.OutDate = DateTime.Now;
            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StaffName = GetStaffLogin().FullName;
            SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
            SetDefaultOutputTo();
            SetDefaultOutputType();
            SetDefultRefGenericDrugCategory();

            RefGenMedProductDetailsList = null;
            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            RefGenMedProductDetailsListSum = null;
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();

            RefGenMedProductDetailsTemp = null;
            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

            ListOutwardDrugFirst = null;
            ListOutwardDrugFirst = new ObservableCollection<OutwardDrugMedDept>();

            ListOutwardDrugFirstCopy = null;
            ListOutwardDrugFirstCopy = new ObservableCollection<OutwardDrugMedDept>();

            BrandName = "";

            SumTotalPrice = 0;
            SumTotalPriceNotVAT = 0;
        }
        //▼====== #001
        //private void SetDefaultForStore()
        //{
        //    if (StoreCbx != null && StoreCbx.Count > 0)
        //    {
        //        StoreID = StoreCbx.FirstOrDefault().StoreID;
        //        if (SearchCriteria != null)
        //        {
        //            SearchCriteria.StoreID = StoreID;
        //        }
        //    }
        //}
        //▲====== #001
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account

        private bool _mTim = true;
        private bool _mPhieuMoi = true;
        private bool _mThucHien = true;
        private bool _mThuTien = true;
        private bool _mIn = true;

        public bool mTim
        {
            get
            {
                return _mTim && _IsOutClinicDept;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }
        public bool mPhieuMoi
        {
            get
            {
                return _mPhieuMoi && _IsOutClinicDept;
            }
            set
            {
                if (_mPhieuMoi == value)
                    return;
                _mPhieuMoi = value;
                NotifyOfPropertyChange(() => mPhieuMoi);
            }
        }
        public bool mThucHien
        {
            get
            {
                return _mThucHien;
            }
            set
            {
                if (_mThucHien == value)
                    return;
                _mThucHien = value;
                NotifyOfPropertyChange(() => mThucHien);
            }
        }
        public bool mThuTien
        {
            get
            {
                return _mThuTien;
            }
            set
            {
                if (_mThuTien == value)
                    return;
                _mThuTien = value;
                NotifyOfPropertyChange(() => mThuTien);
            }
        }
        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }


        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

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


        #region Properties Member

        public bool IsInternalForStore
        {
            get
            {
                return _IsInternalForStore;
            }
            set
            {
                _IsInternalForStore = value;
                if (_IsInternalForStore)
                {
                    IsInternalForDoctor = false;
                    IsInternalForHospital = false;
                }
                NotifyOfPropertyChange(() => IsInternalForStore);
            }
        }
        private bool _IsInternalForStore = true;

        public bool IsInternalForDoctor
        {
            get
            {
                return _IsInternalForDoctor;
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
        private bool _IsInternalForDoctor = false;

        public bool IsInternalForHospital
        {
            get
            {
                return _IsInternalForHospital;
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
        private bool _IsInternalForHospital = false;

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


        private decimal _SumTotalPriceNotVAT;
        public decimal SumTotalPriceNotVAT
        {
            get { return _SumTotalPriceNotVAT; }
            set
            {
                if (_SumTotalPriceNotVAT != value)
                    _SumTotalPriceNotVAT = value;
                NotifyOfPropertyChange(() => SumTotalPriceNotVAT);
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

        private ObservableCollection<OutwardDrugMedDept> _ListOutwardDrugFirst;
        public ObservableCollection<OutwardDrugMedDept> ListOutwardDrugFirst
        {
            get { return _ListOutwardDrugFirst; }
            set
            {
                if (_ListOutwardDrugFirst != value)
                    _ListOutwardDrugFirst = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirst);
            }
        }

        private ObservableCollection<OutwardDrugMedDept> _ListOutwardDrugFirstCopy;
        public ObservableCollection<OutwardDrugMedDept> ListOutwardDrugFirstCopy
        {
            get { return _ListOutwardDrugFirstCopy; }
            set
            {
                if (_ListOutwardDrugFirstCopy != value)
                    _ListOutwardDrugFirstCopy = value;
                NotifyOfPropertyChange(() => ListOutwardDrugFirstCopy);
            }
        }

        private ObservableCollection<OutwardDrugMedDept> _OutwardDrugListCopy;
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugListCopy
        {
            get { return _OutwardDrugListCopy; }
            set
            {
                if (_OutwardDrugListCopy != value)
                    _OutwardDrugListCopy = value;
                NotifyOfPropertyChange(() => OutwardDrugListCopy);
            }
        }


        private OutwardDrugMedDeptInvoice _SelectedOutInvoiceCoppy;
        public OutwardDrugMedDeptInvoice SelectedOutInvoiceCoppy
        {
            get
            {
                return _SelectedOutInvoiceCoppy;
            }
            set
            {
                if (_SelectedOutInvoiceCoppy != value)
                {
                    _SelectedOutInvoiceCoppy = value;
                    NotifyOfPropertyChange("SelectedOutInvoiceCoppy");
                }
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

        #region Properties Member

        public Visibility ElseVisibilityHuy
        {
            get
            {
                return VisibilityHuy == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool _IsGetProductHuy = false;
        public bool IsGetProductHuy
        {
            get
            {
                return _IsGetProductHuy;
            }
            set
            {
                if (_IsGetProductHuy != value)
                {
                    _IsGetProductHuy = value;
                    NotifyOfPropertyChange(() => IsGetProductHuy);
                }
                if (_IsGetProductHuy)
                {
                    //goi ham load len ne!
                    if (Type == 0)
                    {
                        rdtExpiry_Checked(null, null);
                    }
                    else if (Type == 1)
                    {
                        rdtPreExpiry_Checked(null, null);
                    }
                    else
                    {
                        rdtAll_Checked(null, null);
                    }
                }
            }
        }

        private Visibility _VisibilityHuy = Visibility.Visible;
        public Visibility VisibilityHuy
        {
            get
            {
                return _VisibilityHuy;
            }
            set
            {
                _VisibilityHuy = value;
                NotifyOfPropertyChange(() => VisibilityHuy);
                NotifyOfPropertyChange(() => ElseVisibilityHuy);
            }
        }

        private bool _NotEditCost = true;

        public bool IsEditCost
        {
            get
            {
                return _NotEditCost;
            }
        }

        private bool _IsOutClinicDept = true;
        public bool IsOutClinicDept
        {
            get
            {
                return _IsOutClinicDept;
            }
            set
            {
                _IsOutClinicDept = value;
                NotifyOfPropertyChange(() => IsOutClinicDept);
            }
        }

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

        private ObservableCollection<OutwardDrugMedDept> _OutwardDrugMedDeptsCopy;
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptsCopy
        {
            get
            {
                return _OutwardDrugMedDeptsCopy;
            }
            set
            {
                if (_OutwardDrugMedDeptsCopy != value)
                {
                    _OutwardDrugMedDeptsCopy = value;
                    NotifyOfPropertyChange(() => OutwardDrugMedDeptsCopy);
                }
            }
        }

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
                if (_SelectedOutInvoice != null && grdPrescription != null && _SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.KHAC)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        #endregion



        private bool Equal(OutwardDrugMedDept a, OutwardDrugMedDept b)
        {
            if (a.InID == b.InID && a.GenMedProductID == b.GenMedProductID && a.InBatchNumber == b.InBatchNumber && a.InExpiryDate == b.InExpiryDate && a.OutPrice == b.OutPrice && a.OutQuantity == b.OutQuantity && a.OutNotes.Equals(b.OutNotes))
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
            if (SelectedOutInvoice != null && SelectedOutInvoiceCoppy != null)
            {
                if (SelectedOutInvoice.OutDate != SelectedOutInvoiceCoppy.OutDate)
                {
                    return false;
                }
                //if (SelectedOutInvoice.ToStaffID != SelectedOutInvoiceCoppy.ToStaffID || SelectedOutInvoice.ToStoreID != SelectedOutInvoiceCoppy.ToStoreID || SelectedOutInvoice.HosID != SelectedOutInvoiceCoppy.HosID || SelectedOutInvoice.TypID != SelectedOutInvoiceCoppy.TypID || SelectedOutInvoice.V_ByOutPrice != SelectedOutInvoiceCoppy.V_ByOutPrice)
                //{
                //    return false;
                //}
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private bool Compare2Object()
        {
            if (SelectedOutInvoice.OutwardDrugMedDepts != null && OutwardDrugListCopy != null && SelectedOutInvoice.OutwardDrugMedDepts.Count == OutwardDrugListCopy.Count && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
            {

                int icount = 0;
                for (int i = 0; i < OutwardDrugListCopy.Count; i++)
                {
                    for (int j = 0; j < SelectedOutInvoice.OutwardDrugMedDepts.Count; j++)
                    {
                        if (Equal(OutwardDrugListCopy[i], SelectedOutInvoice.OutwardDrugMedDepts[j]))
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

        private IEnumerator<IResult> DoGetRefGenericDrugCategory_1List()
        {
            IsLoadingRefGenericDrugCategory = true;
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            
            //KMX: Không cần set default phân loại, vì bị lỗi chạy đua. Ở ngoài truyền vô là phân loại "Gây nghiện", vào đây set lại phân loại "Khác", dẫn đến update sai (10/01/2015 11:45).
            //SetDefultRefGenericDrugCategory();
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
            StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼====== #001
            //SetDefaultForStore();
            //▲====== #001
            isLoadingGetStore = false;
            yield break;
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //ChangedWPF-CMN
        //public void grdPrescription_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        //{
        //    SumTotalPriceOutward();
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
            var t = new Thread(() =>
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
                            HideHuyHang();
                            OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }



        private void HideHuyHang()
        {
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
            {
                SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                VisibilityHuy = Visibility.Collapsed;
                _NotEditCost = false;
            }
            else
            {
                VisibilityHuy = Visibility.Visible;
                _NotEditCost = true;
            }
            IsGetProductHuy = false;
            NotifyOfPropertyChange(() => IsEditCost);
        }

        private void OutwardDrugMedDeptDetails_Load(long outiID)
        {
            isLoadingDetail = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugMedDeptDetailByInvoice(outiID, V_MedProductType, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                            SumTotalPriceOutward();
                            DeepCopyOutwardDrugMedDept();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }



        private void SumTotalPriceOutward()
        {
            if (!Compare2Object() || !Compare2ObjectInvoice())
            {
                IsOther = true;
            }
            else
            {
                IsOther = false;
            }
            if (SelectedOutInvoice == null)
            {
                return;
            }
            SumTotalPriceNotVAT = 0;
            SumTotalPrice = 0;
            SelectedOutInvoice.TotalInvoicePrice = 0;
            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    //SumTotalPrice += SelectedOutInvoice.OutwardDrugMedDepts[i].OutAmount.GetValueOrDefault();
                    SumTotalPriceNotVAT += SelectedOutInvoice.OutwardDrugMedDepts[i].OutAmount.GetValueOrDefault();
                }
                SumTotalPrice = SumTotalPriceNotVAT * (decimal)SelectedOutInvoice.VAT;
                SelectedOutInvoice.TotalInvoicePrice = SumTotalPrice;
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
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                {
                    if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0830_G1_PhXuatHuyThuoc.ToUpper();
                    }
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0831_G1_PhXuatHuyYCu.ToUpper();
                    }
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                    {
                        proAlloc.LyDo = eHCMSResources.Z3211_G1_PhXuatHuyDDuong.ToUpper();
                    }
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z0832_G1_PhXuatHuyHChat.ToUpper();
                    }

                    proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
                    proAlloc.IsLiquidation = true;
                }
                else
                {
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
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z3206_G1_DinhDuong.ToUpper());
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
            //                MessageBox.Show(ex.Message);
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
                SearchRefGenMedProductDetails(e.Parameter, false);
            }
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as RefGenMedProductDetails;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty
                      };
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
                RefGenMedProductDetailsListSum.Add(item);
            }
            if (IsCode.GetValueOrDefault())
            {
                if (RefGenMedProductDetailsListSum != null && RefGenMedProductDetailsListSum.Count > 0)
                {
                    //KMx: Lý do comment code bên dưới: Khi người dùng tìm bằng code, nhưng không nhập prefix. VD: Hàng có code = "ma0001", nhưng người dùng chỉ nhập "0001".
                    //Nên câu Where(x => x.Code == txt) không thích hợp (07/07/2014 21:50).

                    //var item = RefGenMedProductDetailsListSum.Where(x => x.Code == txt);
                    //if (item != null && item.Count() > 0)
                    //{
                        SelectedSellVisitor = RefGenMedProductDetailsListSum.ToList()[0];
                    //}
                    //else
                    //{
                    //    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    //}
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
                    au.ItemsSource = RefGenMedProductDetailsListSum;
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
            //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (05/07/2014 17:00).
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
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(IsCost, Name, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(asyncResult);

                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            //KMx: OutwardDrugMedDeptsCopy: Phiếu sau khi lưu (Xem lại phiếu xuất cũ) (05/07/2014 16:30).
                            //Nếu hàng trong grid (đã lưu rồi) có trong autocomplete thì phải + remaining cho hàng trong AutoComplete.
                            //Nếu không thì s.Remaining = s.Remaining - d.OutQuantity sẽ bị sai.
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
                                    //KMx: Nếu hàng trong grid không có trong AutoComplete thì thôi, bỏ vào AutoComplete cũng không hiển thị được (05/07/2014 16:51).
                                    //else
                                    //{
                                    //    RefGenMedProductDetails p = d.RefGenericDrugDetail;
                                    //    p.Remaining = d.OutQuantity;
                                    //    p.RemainingFirst = d.OutQuantity;
                                    //    p.InBatchNumber = d.InBatchNumber;
                                    //    p.OutPrice = d.OutPrice;
                                    //    p.InID = Convert.ToInt64(d.InID);
                                    //    p.STT = d.STT;
                                    //    RefGenMedProductDetailsTemp.Add(p);
                                    //    // d = null;
                                    //}
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
                            ListDisplayAutoComplete();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //▼====== #002: P1
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
                            //▲====== #002
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
            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                foreach (OutwardDrugMedDept p1 in SelectedOutInvoice.OutwardDrugMedDepts)
                {
                    if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        // p1.IsLoad = 0;
                        p1.RequestQty += p.RequestQty;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    //p.h= p.RefGenMedProductDetails.InsuranceCover;

                    if (p.InwardDrugMedDept == null)
                    {
                        p.InwardDrugMedDept = new InwardDrugMedDept();
                        p.InwardDrugMedDept.InID = p.InID.GetValueOrDefault();
                        p.InwardDrugMedDept.GenMedProductID = p.GenMedProductID;
                    }
                    p.InvoicePrice = p.OutPrice;

                    //KMx: Set giá vào InwardDrug để có thể cập nhật từ giá vốn thành giá thường và ngược lại (03/03/2015 11:05).
                    //p.InwardDrugMedDept.NormalPrice = p.OutPrice;
                    //p.InwardDrugMedDept.HIPatientPrice = p.OutPrice;
                    //p.InwardDrugMedDept.HIAllowedPrice = p.HIAllowedPrice;

                    if (p.RefGenericDrugDetail != null)
                    {
                        p.InwardDrugMedDept.InCost = p.RefGenericDrugDetail.InCost;
                        p.InwardDrugMedDept.NormalPrice = p.RefGenericDrugDetail.NormalPrice;
                        p.InwardDrugMedDept.HIPatientPrice = p.RefGenericDrugDetail.HIPatientPrice;
                        p.InwardDrugMedDept.HIAllowedPrice = p.RefGenericDrugDetail.HIAllowedPrice;
                    }
                    else
                    {
                        p.InwardDrugMedDept.InCost = 0;
                        p.InwardDrugMedDept.NormalPrice = 0;
                        p.InwardDrugMedDept.HIPatientPrice = 0;
                        p.InwardDrugMedDept.HIAllowedPrice = 0;
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

        private void ChooseBatchNumber(RefGenMedProductDetails value)
        {
            var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
            foreach (RefGenMedProductDetails item in items)
            {
                OutwardDrugMedDept p = new OutwardDrugMedDept();
                if (item.Remaining > 0)
                {
                    if (item.Remaining - value.RequiredNumber < 0)
                    {
                        if (value.RequestQty > item.Remaining)
                        {
                            p.RequestQty = item.Remaining;
                            value.RequestQty = value.RequestQty - item.Remaining;
                        }
                        else
                        {
                            p.RequestQty = value.RequestQty;
                            value.RequestQty = 0;
                        }
                        value.RequiredNumber = value.RequiredNumber - item.Remaining;
                        p.RefGenericDrugDetail = item;
                        p.GenMedProductID = item.GenMedProductID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.OutPrice = item.OutPrice;
                        p.OutQuantity = item.Remaining;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.Visa = item.Visa;
                        p.BidCode = item.BidCode;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = 0;
                    }
                    else
                    {
                        p.RefGenericDrugDetail = item;
                        p.GenMedProductID = item.GenMedProductID;
                        p.InBatchNumber = item.InBatchNumber;
                        p.InID = item.InID;
                        p.RequestQty = value.RequestQty;
                        value.RequestQty = 0;
                        p.OutQuantity = (int)value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.Visa = item.Visa;
                        p.BidCode = item.BidCode;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - (int)value.RequiredNumber;
                        break;
                    }
                }
            }
            SumTotalPriceOutward();
        }

        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
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

        private void ReCountQtyRequest()
        {
            if (SelectedOutInvoice != null && SelectedSellVisitor != null)
            {
                if (SelectedOutInvoice.OutwardDrugMedDepts == null)
                {
                    SelectedOutInvoice.OutwardDrugMedDepts = new ObservableCollection<OutwardDrugMedDept>();
                }
                var results1 = SelectedOutInvoice.OutwardDrugMedDepts.Where(x => x.GenMedProductID == SelectedSellVisitor.GenMedProductID);
                if (results1 != null && results1.Count() > 0)
                {
                    foreach (OutwardDrugMedDept p in results1)
                    {
                        if (p.RequestQty > p.OutQuantity)
                        {
                            p.RequestQty = p.OutQuantity;
                        }
                        SelectedSellVisitor.RequestQty = SelectedSellVisitor.RequestQty - p.RequestQty;
                    }
                }
            }
        }

        public void AddItem(object sender, RoutedEventArgs e)
        {
            ReCountQtyRequest();
            AddListOutwardDrugMedDept(SelectedSellVisitor);
        }

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
            var t = new Thread(() =>
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
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count >= 1)
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
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
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
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrugMedDept.InID)
                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count >= 1)
            {
                Action<IChooseBatchNumber> onInitDlg = delegate (IChooseBatchNumber proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrugMedDept.DeepCopy();
                    proAlloc.FormType = 2;//chinh sua
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
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac);
            }
        }

        #endregion

        private void DeleteInvoiceDrugInObject()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

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
            SumTotalPriceOutward();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteInvoiceDrugInObject();
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

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardClinicDept Request = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (Request != null)
                {
                    ClearData();
                    SelectedOutInvoice.ReqDrugInClinicDeptID = Request.ReqDrugInClinicDeptID;
                    SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;
                    if (StoreID <= 0)
                    {
                        StoreID = Request.OutFromStoreID.GetValueOrDefault();
                    }
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
                    ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, Request.RefGenDrugCatID_1);
                    SelectedOutInvoice.RefGenDrugCatID_1 = Request.RefGenDrugCatID_1;

                    spGetInBatchNumberAndPrice_ByRequestID(Request.ReqDrugInClinicDeptID, StoreID, true, null);
                }
            }
        }

        #endregion

        private void spGetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID, bool IsOutClinicDept, RequestDrugInwardClinicDept Req)
        {
            //isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();

            GetIsCost();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginspGetInBatchNumberAndPrice_ListForRequest(IsCost, RequestID, StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAndPrice_ListForRequest(asyncResult);
                            if (!IsOutClinicDept)
                            {
                                if (Req != null)
                                {
                                    SelectedOutInvoice.ReqDrugInClinicDeptID = Req.ReqDrugInClinicDeptID;
                                    SelectedOutInvoice.ReqNumCode = Req.ReqNumCode;
                                    if (Req.InDeptStoreID.GetValueOrDefault(-1) > 0)
                                    {
                                        SelectedOutInvoice.V_OutputTo = (long)AllLookupValues.V_OutputTo.KHO_KHAC;
                                        SelectedOutInvoice.OutputToID = Req.InDeptStoreID;
                                        if (Req.InDeptStoreObject != null)
                                        {
                                            SelectedOutInvoice.FullName = Req.InDeptStoreObject.swhlName;
                                            SelectedOutInvoice.Address = "";
                                            SelectedOutInvoice.NumberPhone = "";
                                        }
                                    }
                                }
                            }
                            SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                            NotifyOfPropertyChange(() => SelectedOutInvoice.OutwardDrugMedDepts);
                            OutwardDrugMedDeptsCopy = null;
                            SumTotalPriceOutward();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //isLoadingGetID = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnFindRequest(object sender, RoutedEventArgs e)
        {
            //hien thi nhung phieu da duyet ma chua xuat de xuat hang;
            Action<IRequestSearch> onInitDlg = delegate (IRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria.IsApproved = true;
                proAlloc.SearchCriteria.DaNhanHang = false;
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
            };
            GlobalsNAV.ShowDialog<IRequestSearch>(onInitDlg);
        }

        private void OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice OutwardInvoice, bool _bThuTien)
        {

            this.DlgShowBusyIndicator();
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugMedDepts == null)
            {
                this.DlgHideBusyIndicator();
                return;
            }

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
                    contract.BeginOutwardDrugMedDeptInvoice_Update(OutwardInvoice, NewOutwardDrugMedDepts, UpdateOutwardDrugMedDepts, DeleteOutwardDrugMedDepts
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugMedDeptInvoice_Update(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                OutwardInvoice.outiID = OutID;
                                //phat su kien de form o duoi load lai du lieu 
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditMedDeptInvoiceEvent { SelectedOutMedDeptInvoice = OutwardInvoice });
                                TryClose();
                            }
                            else
                            {
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }


        //private void OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice OutwardInvoice, bool _bThuTien)
        //{
        //    //isLoadingFullOperator = true;
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyMedDeptServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginOutwardDrugMedDeptInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    long OutID = 0;
        //                    string StrError;
        //                    bool value = contract.EndOutwardDrugMedDeptInvoice_SaveByType(out OutID, out StrError, asyncResult);

        //                    if (string.IsNullOrEmpty(StrError) && value)
        //                    {
        //                        OutwardInvoice.outiID = OutID;
        //                        //phat su kien de form o duoi load lai du lieu 
        //                        Globals.EventAggregator.Publish(new DrugDeptCloseEditMedDeptInvoiceEvent { SelectedOutMedDeptInvoice = OutwardInvoice });
        //                        TryClose();
        //                    }
        //                    else
        //                    {
        //                        MessageBox.Show(StrError);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                }
        //                finally
        //                {
        //                    //isLoadingFullOperator = false;
        //                    // Globals.IsBusy = false;
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        private void UpdateInvoiceInfo(OutwardDrugMedDeptInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginMedDeptInvoice_UpdateInvoiceInfo(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool value = contract.EndMedDeptInvoice_UpdateInvoiceInfo(asyncResult);
                            Globals.EventAggregator.Publish(new DrugDeptCloseEditPayedEvent { SelectedOutMedDeptInvoice = SelectedOutInvoice });
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
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UpdateInvoicePayed(OutwardDrugMedDeptInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginMedDeptInvoice_UpdateInvoicePayed(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            long PaymemtID = 0;
                            string StrError = "";
                            bool value = contract.EndMedDeptInvoice_UpdateInvoicePayed(out OutID, out PaymemtID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                SelectedOutInvoice.outiID = OutID;
                                //phat su kien de form o duoi load lai du lieu 
                                Globals.EventAggregator.Publish(new DrugDeptCloseEditPayedEvent { SelectedOutMedDeptInvoice = SelectedOutInvoice });
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
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckData()
        {
            if (SelectedOutInvoice == null)
            {
                return false;
            }

            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                SelectedOutInvoice.StoreID = StoreID;
            }
            string strError = "";
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
            {
                SelectedOutInvoice.ReqDrugInClinicDeptID = 0;
                SelectedOutInvoice.OutputToID = 0;
                SelectedOutInvoice.Address = "";
                SelectedOutInvoice.NumberPhone = "";
                SelectedOutInvoice.FullName = "";
                SelectedOutInvoice.V_OutputTo = 0;
                if (string.IsNullOrWhiteSpace(SelectedOutInvoice.Notes))
                {
                    if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC)
                    {
                        MessageBox.Show(eHCMSResources.Z2189_G1_Msg_InfoNhapLyDoTra);
                    }
                    else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                    {
                        MessageBox.Show(eHCMSResources.Z2738_G1_Msg_InfoNhapLyDoXuat);
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0198_G1_Msg_InfoNhapLyDoHuy);
                    }
                    return false;
                }
            }
            else
            {
                if (SelectedOutInvoice.ReqDrugInClinicDeptID > 0)
                {
                    SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;
                }
                if (SelectedOutInvoice.OutputToID == null || SelectedOutInvoice.OutputToID <= 0)
                {
                    if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                    {
                        MessageBox.Show(eHCMSResources.K0330_G1_ChonKhoNhan);
                        return false;
                    }
                    else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                    {
                        MessageBox.Show(eHCMSResources.K0296_G1_ChonBVNhan);
                        return false;
                    }
                    else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHACH_VANG_LAI)
                    {
                        if (string.IsNullOrEmpty(SelectedOutInvoice.FullName))
                        {
                            MessageBox.Show(eHCMSResources.K0448_G1_NhapTenNguoiNhan);
                            return false;
                        }
                    }
                    else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN)
                    {
                        MessageBox.Show(eHCMSResources.K0284_G1_ChonBNNhan);
                        return false;
                    }
                    else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI)
                    {
                        MessageBox.Show(eHCMSResources.K0375_G1_ChonNguoiNhan);
                        return false;
                    }
                }

                if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC && SelectedOutInvoice.StoreID == SelectedOutInvoice.OutputToID)
                {
                    MessageBox.Show(eHCMSResources.A0618_G1_Msg_InfoKhoXuatKhacKhoNhan);
                    return false;
                }

            }
            if (SelectedOutInvoice.OutwardDrugMedDepts == null || SelectedOutInvoice.OutwardDrugMedDepts.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat);
                return false;
            }
            else
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    OutwardDrugMedDept item = SelectedOutInvoice.OutwardDrugMedDepts[i];

                    if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.HUYHANG
                        && SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                        && SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                    {
                        //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                        //if (item.OutPrice <= 0)
                        //{
                        //    MessageBox.Show(string.Format("{0} ", eHCMSResources.A0522_G1_Msg_Dong) + (i + 1).ToString() + ": Giá bán phải > 0.Vui lòng cập nhật bảng giá!");
                        //    return false;
                        //}
                        //neu ngay het han lon hon ngay hien tai
                        if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                        {
                            if (eHCMS.Services.Core.AxHelper.CompareDate(DateTime.Now, item.InExpiryDate.GetValueOrDefault()) == 1)
                            {
                                strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, item.RefGenericDrugDetail.BrandName, (i + 1).ToString());
                            }
                        }
                    }

                    int intOutput = 0;
                    //if (item.RefGenericDrugDetail != null && item.OutQuantity <= 0)
                    if (item.OutQuantity <= 0 || !Int32.TryParse(item.OutQuantity.ToString(), out intOutput))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1400_G1_SLgXuatLaSoNguyen, (i + 1).ToString()));
                        return false;
                    }

                    if (item.OutQuantity > item.RefGenericDrugDetail.RemainingFirst)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1402_G1_SLgConLaiKgDuXuat, (i + 1).ToString(), SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.RemainingFirst.ToString("#,##0.##")));
                        return false;
                    }
                    if (SelectedOutInvoice.ReqDrugInClinicDeptID > 0 && item.RequestQty != item.OutQuantity
                        && Globals.ServerConfigSection.MedDeptElements.BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty)
                    {
                        MessageBox.Show("SL xuất khác SL yêu cầu. Vui lòng kiểm tra lại!");
                        return false;
                    }
                    //spOutwardDrugMedDeptInvoices_InsertByType
                }
            }
            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0942_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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


        public void btnSave()
        {
            this.DlgShowBusyIndicator();
            if (SelectedOutInvoice == null)
            {
                return;
            }
            //20190315 TBL: Hien tai chan viec xuat tu khoa duoc cho kho nha thuoc
            if (SelectedOutInvoice.OutputToID == 2)
            {
                MessageBox.Show(eHCMSResources.Z2617_G1_KhongTheXuatChoKhoNay);
                return;
            }

            if (SelectedOutInvoice.OutDate == null || SelectedOutInvoice.OutDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.A0863_G1_Msg_InfoNgXuatKhHopLe4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutInvoice.OutwardDrugMedDepts != null && SelectedOutInvoice.OutwardDrugMedDepts.Count > 0)
            {
                if (!Compare2Object())
                {
                    string strError = "";
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                    {
                        //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                        //if (SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice <= 0)
                        //{
                        //    MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0);
                        //    return;
                        //}
                        //neu ngay het han lon hon ngay hien tai
                        //20191017 TBL: Nếu hạn sử dụng null thì không cần kiểm tra
                        if (SelectedOutInvoice.OutwardDrugMedDepts[i].InExpiryDate != null && eHCMS.Services.Core.AxHelper.CompareDate(Globals.GetCurServerDateTime(), SelectedOutInvoice.OutwardDrugMedDepts[i].InExpiryDate.GetValueOrDefault()) == 1)
                        {
                            strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.BrandName, (i + 1).ToString());
                        }
                    }
                    if (!string.IsNullOrEmpty(strError))
                    {
                        if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                        {
                            return;
                        }
                    }
                    if (CheckData())
                    {
                        if (this.CheckValid())
                        {
                            SelectedOutInvoice.V_MedProductType = V_MedProductType;
                            //KMx: Dời TypID == HUYHANG vào trong Get của Property IsInternal luôn cho dễ quản lý (22/09/2014 14:37).
                            //if (SelectedOutInvoice.IsInternal || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
                            if (SelectedOutInvoice.IsInternal)
                            {
                                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                            }
                            else
                            {
                                SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                            }

                            if (SelectedOutInvoice.PaidTime == null)
                            {
                                OutwardDrugMedDeptInvoice_SaveByType(SelectedOutInvoice, false);
                            }
                            else
                            {
                                UpdateInvoicePayed(SelectedOutInvoice);
                            }
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe);
                        }
                    }
                }
                else
                {
                    //goi ham cap nhat hoa don
                     UpdateInvoiceInfo(SelectedOutInvoice);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0639_G1_Msg_InfoKhCoCTietPhBanLe);
            }

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

        //KMx: Không hiểu để icount để làm gì, dẫn đến không đổi được giá (15/07/2014 15:02).
        //private int icount = 0;
        public void SelectedByOutPrice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SelectedOutInvoice != null && icount > 0)
            if (SelectedOutInvoice != null)
            {
                GetIsCost();

                if (SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.GIATHONGTHUONG)
                {
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = true;
                    if (SelectedOutInvoice.OutwardDrugMedDepts != null)
                    {
                        for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                        {
                            //KMx: Khi load lại phiếu cũ, không có RefGenericDrugDetail.NormalPrice nên dùng InwardDrugMedDept.NormalPrice (03/03/2015 11:07).
                            //SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.NormalPrice;
                            SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.NormalPrice;
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
                            //SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].RefGenericDrugDetail.InCost;
                            SelectedOutInvoice.OutwardDrugMedDepts[i].OutPrice = SelectedOutInvoice.OutwardDrugMedDepts[i].InwardDrugMedDept.InCost;
                        }
                    }
                }
                else
                {
                    //mo cot gia len cho sua 
                    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                }
                SumTotalPriceOutward();
            }
            //icount++;

        }

        private void GetIsCost()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON)
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
                    HideHuyHang();
                    OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                }
            }
        }

        #endregion

        #region IHandle<DrugDeptChooseBatchNumberEvent> Members

        public void Handle(DrugDeptEditChooseBatchNumberEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugMedDept.RefGenericDrugDetail = message.BatchNumberSelected;
                SelectedOutwardDrugMedDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugMedDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugMedDept.InID = message.BatchNumberSelected.InID;
                SelectedOutwardDrugMedDept.VAT = message.BatchNumberSelected.VAT;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugMedDept.OutPrice = message.BatchNumberSelected.NormalPrice;
                }
                SelectedOutwardDrugMedDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SumTotalPriceOutward();
            }
        }

        #endregion

        #region IHandle<DrugDeptChooseBatchNumberResetQtyEvent> Members

        public void Handle(DrugDeptEditChooseBatchNumberResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugMedDept.RefGenericDrugDetail = message.BatchNumberSelected;
                SelectedOutwardDrugMedDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugMedDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugMedDept.InID = message.BatchNumberSelected.InID;
                SelectedOutwardDrugMedDept.VAT = message.BatchNumberSelected.VAT;
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
                SumTotalPriceOutward();
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
            RefOutputTypes = Globals.RefOutputType.Where(x => x.IsSelected == true).ToObservableCollection();
            SetDefaultOutputType();
        }

        //KMx: Khi login đã lấy RefOutputType hết rồi, không cần về server nữa (15/07/2014 11:58).
        //public void LoadRefOutputType()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyMedDeptServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginRefOutputType_Get(false, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndRefOutputType_Get(asyncResult);
        //                    RefOutputTypes = results.ToObservableCollection();
        //                    SetDefaultOutputType();
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

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

        private int xcount = 0;
        public void XuatCho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xcount > 0 && SelectedOutInvoice != null && (sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                SelectedOutInvoice.OutputToID = 0;
                SelectedOutInvoice.FullName = "";
                SelectedOutInvoice.Address = "";
                SelectedOutInvoice.NumberPhone = "";

                SelectedOutInvoice.ReqDrugInClinicDeptID = 0;
                SelectedOutInvoice.ReqNumCode = "";
            }
            xcount++;
        }

        public void btnChoose()
        {
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                {
                    void onInitDlg(IDrugDeptStorage proAlloc1)
                    {
                        proAlloc1.IsChildWindow = true;
                    }
                    GlobalsNAV.ShowDialog<IDrugDeptStorage>(onInitDlg, null, true, false, Globals.GetDefaultDialogViewSize());
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BVBAN)
                {
                    void onInitDlg(IDrugDeptHospitals proAlloc2)
                    {
                        proAlloc2.IsChildWindow = true;
                    }
                    GlobalsNAV.ShowDialog<IDrugDeptHospitals>(onInitDlg, null, true, false, Globals.GetDefaultDialogViewSize());
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BENHNHAN)
                {
                    GlobalsNAV.ShowDialog<IFindPatient>();
                }
                else if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.BACSI)
                {
                    void onInitDlg(IDrugDeptStaffs proAlloc3)
                    {
                        proAlloc3.IsChildWindow = true;
                    }
                    GlobalsNAV.ShowDialog<IDrugDeptStaffs>(onInitDlg, null, true, false, Globals.GetDefaultDialogViewSize());
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
                    if (sto.RefStaffCategory != null)
                    {
                        SelectedOutInvoice.FullName = sto.RefStaffCategory.StaffCatgDescription + " " + sto.FullName;
                    }
                    else
                    {
                        SelectedOutInvoice.FullName = sto.FullName;
                    }
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

        private bool flag = false;
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
            VisibilityHuy = Visibility.Visible;
            _NotEditCost = true;
            IsGetProductHuy = false;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIATHONGTHUONG;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                    //▼====== #002: P2
                    _NotEditCost = false;
                    //▲====== #002
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                    VisibilityHuy = Visibility.Collapsed;
                    _NotEditCost = false;
                }
            }
            NotifyOfPropertyChange(() => IsEditCost);
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
                    SearchRefGenMedProductDetails(txt, true);
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
                    SearchRefGenMedProductDetails((sender as TextBox).Text, true);
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
                    return _VisibilityName;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
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
            }

            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPriceOutward();
            }
        }
        #region Huy Thuoc Het Han Dung
        private int Type = 0;

        public void rdtExpiry_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 0;
                SelectedOutInvoice.OutwardDrugMedDepts = null;
                OutwardDrugDetails_Get(StoreID);
            }
        }

        public void rdtPreExpiry_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 1;
                SelectedOutInvoice.OutwardDrugMedDepts = null;
                OutwardDrugDetails_Get(StoreID);
            }

        }
        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {
            if (IsGetProductHuy)
            {
                ClearData();
                Type = 2;
                SelectedOutInvoice.OutwardDrugMedDepts = null;
                OutwardDrugDetails_Get(StoreID);
            }
        }

        private void OutwardDrugDetails_Get(long ID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetListDrugExpiryDate_DrugDept(ID, Type, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetListDrugExpiryDate_DrugDept(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            if (results == null || results.Count == 0)
                            {
                                if (Type == 0)
                                {
                                    MessageBox.Show(eHCMSResources.A0651_G1_Msg_InfoKhCoHgHetHan);
                                }
                                else if (Type == 1)
                                {
                                    MessageBox.Show(eHCMSResources.A0649_G1_Msg_InfoKhCoHgDenHanHuy);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0650_G1_Msg_InfoKhCoHgHetHoacDenHan);
                                }
                            }

                            SelectedOutInvoice.OutwardDrugMedDepts = results.ToObservableCollection();
                            //tinh tong tien 
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
        #endregion

        //KMx: Popup cập nhật phiếu xuất không nhận event tính tiền (24/12/2014 10:48).
        //#region PharmacyPayEvent
        //private long PaymentID = 0;

        //private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        //{
        //    isLoadingGetStore = true;
        //    var paymentTypeTask = new AddTracsactionMedDeptForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
        //    yield return paymentTypeTask;
        //    PaymentID = paymentTypeTask.PaymentID;
        //    isLoadingGetStore = false;
        //    yield break;
        //}

        //private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        //{
        //    isLoadingGetStore = true;
        //    var paymentTypeTask = new AddTracsactionMedDeptForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
        //    yield return paymentTypeTask;
        //    PaymentID = paymentTypeTask.PaymentID;
        //    isLoadingGetStore = false;
        //    yield break;
        //}


        //public void Handle(PharmacyPayEvent message)
        //{
        //    //thu tien
        //    if (this.IsActive && message != null)
        //    {
        //        if (message.CurPatientPayment != null && message.CurPatientPayment.PayAmount < 0)
        //        {
        //            Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
        //            {
        //                GetOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID);
        //            });
        //        }
        //        else
        //        {
        //            Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
        //            {
        //                btnPreview();
        //                GetOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID);
        //            });
        //        }
        //    }
        //}
        //#endregion

        public void btnCancel()
        {
            TryClose();
        }

        public void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedOutInvoice.VAT < 1 || SelectedOutInvoice.VAT > 2)
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.K0263_G1_VATKhongHopLe2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedOutInvoice.VAT = 1;
            }
            SumTotalPriceOutward();
        }
    }
}
