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
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Common.BaseModel;
/*
 * 20180725 #001 CMN:   Added Parameter to get drugs inside medical products
 * 20181011 #002 TBL:   BM 0002164: Khong cho huy phieu xuat da duoc khoa phong nhap khi cau hinh tu dong nhap tat
 * 20181219 #003 TTM:   BM 0005443: Lọc phiếu yêu cầu, load dữ liệu cho đúng của khoa phòng hoặc của kho BHYT - nhà thuốc.
 * 20190729 #004 TTM:   BM 0013024: cho phép chọn tìm thuốc bằng tên hoạt chất hoặc tên thương mại
 * 20190730 #005 TTM:   BM 0013008: Sửa lỗi Focus của AutoComplete không chính xác.
 * 20200617 #006 TNHX:  BM? Sửa lỗi xem/in của xuất điều chuyển của loại khác thuốc, y cụ thì sai thông tin
 * 20200623 #007 TNHX:  BM? Thêm report Biên bản giao nhận vaccine cho khoa dược
 * 20210823 #008 QTD:   Thêm loại Dinh dưỡng
 * 20210922 #009 QTD:   Filter Storage
 * 20210925 #010 QTD:   Add checkbox Xuất chống dịch
 * 20220106 #011 TNHX: 887 Cho tích lọc thuốc theo danh mục COVID ( không có tìm bệnh nhân nên mặc định false)
 * 20220122 #012 QTD:   Lọc kho theo cấu hình trách nhiệm
 * 20230701 #013 QTD:   Cho phép chỉnh số lượng xuất khi load phiếu lĩnh VPP/VTTH
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IXuatNoiBo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XuatNoiBoViewModel : ViewModelBase, IXuatNoiBo, IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent>
        , IHandle<DrugDeptChooseBatchNumberEvent>, IHandle<DrugDeptChooseBatchNumberResetQtyEvent>
        , IHandle<DrugDeptCloseSearchStorageEvent>
        , IHandle<DrugDeptCloseSearchHospitalEvent>
        , IHandle<ItemSelected<Patient>>
        , IHandle<DrugDeptCloseSearchStaffEvent>
        , IHandle<PharmacyPayEvent>
        , IHandle<DrugDeptCloseEditMedDeptInvoiceEvent>
        , IHandle<DrugDeptCloseEditPayedEvent>
        , IHandle<DrugDeptCloseSearchRequestForHIStoreEvent>
    {
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
        public XuatNoiBoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();

            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            Coroutine.BeginExecute(GetLookupOutputTo());
            Coroutine.BeginExecute(DoGetByOutPriceLookups());

            LoadRefOutputType();

            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            RefeshData();
            SetDefaultForStore();
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
            // 20191107 TNHX: Vì lý do gì đó mà default sau khi tạo mới V_ByOutPriceMedDept = GIATHONGTHUONG 
            // và ảnh hưởng đến dòng thuốc dù khi lưu xuống vẫn là GIAVON
            // set mặc định lại thành giá vốn
            SelectedOutInvoice = new OutwardDrugMedDeptInvoice
            {
                V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON,
                OutDate = Globals.GetCurServerDateTime(),
                StaffID = GetStaffLogin().StaffID,
                StaffName = GetStaffLogin().FullName
            };
            SetDefaultOutputTo();
            SetDefaultOutputType();
            SetDefultRefGenericDrugCategory();
            ClearData();
            TotalPrice = 0;
            TotalPriceNotVAT = 0;

            IsEnableCbxStoreOut = true;
            IsEnableCheckboxXCD = false;
            IsXuatChongDich = false;
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

        //20190130 TTM: Hiện tại đang set code cứng cần phải sửa lại sau khi ổn định mọi chuyện cho Thanh Vũ.
        private void SetDefaultForStore()
        {
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
                //--▼-- 29/12/2020 DatTB Gán biến mặc định
                var selectedStore = (RefStorageWarehouseLocation)StoreCbx.FirstOrDefault();
                V_GroupTypes = selectedStore.V_GroupTypes;
                IsMainStorage = selectedStore.IsMain;
                IsSubStorage = selectedStore.IsSubStorage;
                StoreTypeID = (long)selectedStore.StoreTypeID;
                //--▲-- 29/12/2020 DatTB 
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
            eCanChange_DatetimeExProduct = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mModulesGen,
                  (int)eModuleGeneral.mCanChange_DatetimeExOrImProduct_DrugDept, (int)oModuleGeneralEX.mCanChange_DatetimeExProduct_DrugDept, (int)ePermission.mView);
        }

        #region checking account

        private bool _mTim = true;
        private bool _mPhieuMoi = true;
        private bool _mThucHien = true;
        private bool _mThuTien = true;
        private bool _mIn = true;
        private bool _mDeleteInvoice = true;
        private bool _mPrintReceipt = true;

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

        public bool mDeleteInvoice
        {
            get
            {
                return _mDeleteInvoice;
            }
            set
            {
                if (_mDeleteInvoice == value)
                    return;
                _mDeleteInvoice = value;
                NotifyOfPropertyChange(() => mDeleteInvoice);
            }
        }

        public bool mPrintReceipt
        {
            get
            {
                return _mPrintReceipt;
            }
            set
            {
                if (_mPrintReceipt == value)
                    return;
                _mPrintReceipt = value;
                NotifyOfPropertyChange(() => mPrintReceipt);
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
        //▼====: #011
        private bool _IsCOVID = false;
        public bool IsCOVID
        {
            get
            {
                return _IsCOVID;
            }
            set
            {
                if (_IsCOVID != value)
                {
                    _IsCOVID = value;
                    NotifyOfPropertyChange(() => IsCOVID);
                }
            }
        }
        //▲====: #011
        private bool _eCanChange_DatetimeExProduct = true;
        public bool eCanChange_DatetimeExProduct
        {
            get
            {
                return _eCanChange_DatetimeExProduct;
            }
            set
            {
                if (_eCanChange_DatetimeExProduct == value)
                {
                    return;
                }
                _eCanChange_DatetimeExProduct = value;
                NotifyOfPropertyChange(() => eCanChange_DatetimeExProduct);
            }
        }
        public Visibility ElseVisibilityHuy
        {
            get
            {
                return VisibilityHuy == Visibility.Collapsed && SelectedOutInvoice.CanSaveAndPaid ? Visibility.Visible : Visibility.Collapsed;
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
        //20190114 TTM: mặc định bằng false, do loại xuất ban đầu của mình mặc định là xuất nội bộ mà xuất
        //              nội bộ thì giá phải là giá vốn và không được chỉnh sửa.
        private bool _NotEditCost = false;

        public bool IsEditCost
        {
            get
            {
                //KMx: Sau khi lưu là không cho chỉnh sửa Giá nữa. Muốn chỉnh sửa thì vào phần cập nhật (15/07/2014 17:11).
                //return _NotEditCost && SelectedOutInvoice.CanSaveAndPaid;
                return _NotEditCost && SelectedOutInvoice.CanSave;
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

        //▼====: #001
        private long? _OutputToStoreID = null;
        public long? OutputToStoreID
        {
            get => _OutputToStoreID;
            set
            {
                _OutputToStoreID = value;
                NotifyOfPropertyChange(() => OutputToStoreID);
            }
        }
        //▲====: #001
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

                //if (_SelectedOutInvoice != null && grdPrescription != null && _SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.KHAC)
                //{
                //    grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                //}

                if (grdPrescription != null)
                {
                    if (_SelectedOutInvoice != null && _SelectedOutInvoice.V_ByOutPriceMedDept == (long)AllLookupValues.V_ByOutPriceMedDept.KHAC)
                    {
                        grdPrescription.Columns[(int)DataGridCol.DonGia].IsReadOnly = false;
                    }

                    var colDelete = grdPrescription.GetColumnByName("colDelete");

                    if (_SelectedOutInvoice != null && _SelectedOutInvoice.outiID > 0)
                    {
                        colDelete.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        colDelete.Visibility = Visibility.Visible;
                    }
                }

                NotifyOfPropertyChange(() => SelectedOutInvoice);
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private decimal _TotalPriceNotVAT;
        public decimal TotalPriceNotVAT
        {
            get
            {
                return _TotalPriceNotVAT;
            }
            set
            {
                _TotalPriceNotVAT = value;
                NotifyOfPropertyChange(() => TotalPriceNotVAT);
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
            var paymentTypeTask = new LoadRefGenericDrugCategory_1ListTask(V_MedProductType, false, false);
            yield return paymentTypeTask;
            RefGenericDrugCategory_1s = paymentTypeTask.RefGenericDrugCategory_1List;
            SetDefultRefGenericDrugCategory();
            yield break;
        }

        private void SetDefultRefGenericDrugCategory()
        {
            if (SelectedOutInvoice != null && RefGenericDrugCategory_1s != null && RefGenericDrugCategory_1s.Count() > 0)
            {
                SelectedOutInvoice.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private IEnumerator<IResult> DoGetStoreToSell()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #012
            
            var StoreTemp = paymentTypeTask.LookupList.Where(x => V_MedProductType != 0 
                                                                   && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT 
                                                                   && x.ListV_MedProductType != null 
                                                                   && x.ListV_MedProductType.Contains(V_MedProductType.ToString())
                                                                   && !x.IsConsignment).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx == null || StoreCbx.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #012
            //StoreCbx = StoreCbx.Where(x => !x.IsSubStorage && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT).ToObservableCollection();
            SetDefaultForStore();
            yield break;
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.CANBANGKHO || ViewCase != 1)
            {
                grdPrescription.Columns[10].Visibility = Visibility.Hidden;
                grdPrescription.Columns[11].Visibility = Visibility.Hidden;
                grdPrescription.Columns[12].Visibility = Visibility.Hidden;
            }

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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                if (!IsBalanceView)
                                {
                                    HideHuyHang();
                                }
                                OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugMedDeptInvoice_Search(0, 20);
        }

        private void OutwardDrugMedDeptInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new MedDeptInvoiceSearchCriteria();
            }
            SearchCriteria.StoreID = StoreID;
            // 20190222 TNHX: Them Dky loai xuat de tim kiem phieu xuat
            SearchCriteria.TypID = (SelectedOutInvoice != null && SelectedOutInvoice.TypID != null) ? SelectedOutInvoice.TypID : (long)AllLookupValues.RefOutputType.XUATNOIBO;
            SearchCriteria.V_MedProductType = V_MedProductType;

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                        void onInitDlg(IXuatNoiBoSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.OutwardMedDeptInvoiceList.Clear();
                                            proAlloc.OutwardMedDeptInvoiceList.TotalItemCount = TotalCount;
                                            proAlloc.OutwardMedDeptInvoiceList.PageIndex = 0;
                                            proAlloc.OutwardMedDeptInvoiceList.PageSize = 20;
                                            foreach (OutwardDrugMedDeptInvoice p in results)
                                            {
                                                proAlloc.OutwardMedDeptInvoiceList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IXuatNoiBoSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        SelectedOutInvoice = results.FirstOrDefault();
                                        HideHuyHang();
                                        //load detail
                                        OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void HideHuyHang()
        {
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                SumTotalPrice();
                                DeepCopyOutwardDrugMedDept();
                                //if (SelectedOutInvoice.Notes != null)
                                if (SelectedOutInvoice.Notes != null & SelectedOutInvoice.Notes.Contains("[XCD]"))
                                {
                                    if (SelectedOutInvoice.Notes.Contains("[XCD]"))
                                    {
                                        IsXuatChongDich = true;
                                    }
                                    else
                                    {
                                        IsXuatChongDich = false;
                                    }
                                }
                                else
                                {
                                    IsXuatChongDich = false;
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
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
                OutwardDrugMedDeptInvoice_Search(0, 20);
            }
        }

        private void SumTotalPrice()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            TotalPriceNotVAT = 0;
            TotalPrice = 0;
            SelectedOutInvoice.TotalInvoicePrice = 0;

            if (SelectedOutInvoice.OutwardDrugMedDepts != null)
            {
                for (int i = 0; i < SelectedOutInvoice.OutwardDrugMedDepts.Count; i++)
                {
                    //TotalPrice += SelectedOutInvoice.OutwardDrugMedDepts[i].OutAmount.GetValueOrDefault();
                    TotalPriceNotVAT += SelectedOutInvoice.OutwardDrugMedDepts[i].OutAmount.GetValueOrDefault();
                }
                TotalPrice = TotalPriceNotVAT * (decimal)SelectedOutInvoice.VAT;
                SelectedOutInvoice.TotalInvoicePrice = TotalPrice;
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
                //▼====== #005
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.THANHLY
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                {
                    proAlloc.IsLiquidation = true;
                }
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                {
                    proAlloc.LyDo = "Xuất điều chuyển - ";
                    switch (SelectedOutInvoice.V_MedProductType)
                    {
                        case (long)AllLookupValues.MedProductType.THUOC:
                            proAlloc.LyDo += eHCMSResources.G0787_G1_Thuoc.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.Y_CU:
                            proAlloc.LyDo += eHCMSResources.G2907_G1_YCu.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.HOA_CHAT:
                            proAlloc.LyDo += eHCMSResources.T1616_G1_HC.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.MAU:
                            proAlloc.LyDo += eHCMSResources.T3709_G1_Mau.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                            proAlloc.LyDo += eHCMSResources.Z2464_G1_TiemNgua.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM:
                            proAlloc.LyDo += eHCMSResources.Z2520_G1_VPP.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                            proAlloc.LyDo += eHCMSResources.Z2470_G1_VTYTTH.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.VATTU_TIEUHAO:
                            proAlloc.LyDo += eHCMSResources.Z2521_G1_VTTH.ToUpper();
                            break;
                        case (long)AllLookupValues.MedProductType.THANHTRUNG:
                            proAlloc.LyDo += eHCMSResources.Z2500_G1_ThanhTrung.ToUpper();
                            break;
                        //▼====== #008
                        case (long)AllLookupValues.MedProductType.NUTRITION:
                            proAlloc.LyDo += eHCMSResources.Z3206_G1_DinhDuong.ToUpper();
                            break;
                            //▲====== #008
                    }
                    proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
                }
                //▲====== #005
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
                {
                    if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0830_G1_PhXuatHuyThuoc.ToUpper();
                    }
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        proAlloc.LyDo = eHCMSResources.Z0831_G1_PhXuatHuyYCu.ToUpper();
                    }
                    //▼====== #008
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                    {
                        proAlloc.LyDo = eHCMSResources.Z3211_G1_PhXuatHuyDDuong.ToUpper();
                    }
                    //▲====== #008
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z0832_G1_PhXuatHuyHChat.ToUpper();
                    }
                    proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC)
                {
                    if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraThuoc.ToUpper();
                    }
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraYCu.ToUpper();
                    }
                    //▼====== #008
                    else if (SelectedOutInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                    {
                        proAlloc.LyDo = eHCMSResources.Z3212_G1_PhXuatTraDDuong.ToUpper();
                    }
                    //▲====== #008
                    else
                    {
                        proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraHChat.ToUpper();
                    }
                    proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
                }
                else
                {
                    if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                        {
                            proAlloc.LyDo = eHCMSResources.Z1474_G1_PhXuatKhoThuocGayNghien;
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                        else if (SelectedOutInvoice.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                        {
                            proAlloc.LyDo = eHCMSResources.Z1475_G1_PhXuatKhoThuocHuongTamThan;
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                        else
                        {
                            proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G0787_G1_Thuoc.ToUpper());
                            //if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                            //{
                            //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                            //}
                            //else
                            //{
                            //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                            //}

                            //KMx: Nếu là phiếu bán lẻ thì phải in report có giá tiền, bất kể là xuất cho kho nội bộ hay khách vãng lai (05/02/2015 17:19).
                            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                            {
                                proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                            }
                            else
                            {
                                proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                            }
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G2907_G1_YCu.ToUpper());
                        //if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                        //{
                        //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        //}
                        //else
                        //{
                        //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        //}
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    //▼====== #008                   
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z3206_G1_DinhDuong.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    //▲====== #008
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.VTYT_TIEUHAO)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z2470_G1_VTYTTH.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.R0810_G1_Vaccine.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.MAU)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.T3709_G1_Mau.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.THANHTRUNG)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z2500_G1_ThanhTrung.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z2520_G1_VPP.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.VATTU_TIEUHAO)
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z2521_G1_VTTH.ToUpper());
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                    else
                    {
                        proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.T1616_G1_HC.ToUpper());
                        //if (SelectedOutInvoice.V_OutputTo == (long)AllLookupValues.V_OutputTo.KHO_KHAC)
                        //{
                        //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        //}
                        //else
                        //{
                        //    proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        //}
                        if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL;
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.DRUGDEPT_OUTINTERNAL_TOCLINICDEPT;
                        }
                    }
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
            if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.HUYHANG
                && SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.THANHLY
                && SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN
                && SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                && IsSubStorage
                && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                Action<IDrugDeptReportDocumentPreview> onInitDlg2 = delegate (IDrugDeptReportDocumentPreview proAlloc)
                {
                    proAlloc.ID = SelectedOutInvoice.outiID;
                    proAlloc.V_MedProductType = V_MedProductType;
                    proAlloc.eItem = ReportName.XRptRequestDrugDeptDetailsGroupByPatient;
                };
                GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg2);
            }
        }

        //▼====: #007
        public void btnPreviewBBGiaoNhanVaccine()
        {
            if (V_MedProductType == (long)AllLookupValues.MedProductType.TIEM_NGUA && SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
            {
                void onInitDlg1(IDrugDeptReportDocumentPreview proAlloc)
                {
                    proAlloc.ID = SelectedOutInvoice.outiID;
                    proAlloc.V_MedProductType = V_MedProductType;
                    proAlloc.eItem = ReportName.BBGiaoNhanVaccine;
                }
                GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg1);
            }
        }
        //▲====: #007

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
                    SearchRefGenMedProductDetails(e.Parameter, false);
                }
                else
                {
                    GetDrugForBalanceCompleteFromCategory(e.Parameter, false);
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

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code, hd.ProductCodeRefNum } into hdgroup
                      where hdgroup.Sum(i => i.Remaining) > 0 //20210402 QTD thêm điều kiện bỏ thuốc không còn remaining sử dụng khi giảm kiểm kê
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty,
                          ProductCodeRefNum = hdgroup.Key.ProductCodeRefNum
                      };
            //KMx: for chạy lâu hơn foreach (đã test trên 1000 items) (05/07/2014 16:54).
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
                item.ProductCodeRefNum = i.ProductCodeRefNum;
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
            //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (05/07/2014 11:53).
            if (IsCode == false && Name.Length < 1)
            {
                return;
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
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, IsCost, Name, StoreID
                            , V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, OutputToStoreID, null, null, true
                            //▼====: #011
                            , IsCOVID
                            //▲====: #011
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);

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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }

        //private void SearchRefGenMedProductDetails(string Name, bool? IsCode)
        //{
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
        //            contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(IsCost, Name, StoreID, V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept(asyncResult);
        //                    RefGenMedProductDetailsList.Clear();
        //                    RefGenMedProductDetailsListSum.Clear();
        //                    RefGenMedProductDetailsTemp.Clear();
        //                    foreach (RefGenMedProductDetails s in results)
        //                    {
        //                        RefGenMedProductDetailsTemp.Add(s);
        //                    }
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
        //                            else
        //                            {
        //                                RefGenMedProductDetails p = d.RefGenericDrugDetail;
        //                                p.Remaining = d.OutQuantity;
        //                                p.RemainingFirst = d.OutQuantity;
        //                                p.InBatchNumber = d.InBatchNumber;
        //                                p.OutPrice = d.OutPrice;
        //                                p.InID = Convert.ToInt64(d.InID);
        //                                p.STT = d.STT;
        //                                RefGenMedProductDetailsTemp.Add(p);
        //                                // d = null;
        //                            }
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
        //                    ListDisplayAutoComplete();

        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
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
            var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID && x.BrandName == value.BrandName).OrderBy(p => p.STT);
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
                        p.OutQuantity = value.RequiredNumber;
                        p.OutPrice = item.OutPrice;
                        p.InExpiryDate = item.InExpiryDate;
                        p.SdlDescription = item.SdlDescription;
                        p.Visa = item.Visa;
                        p.BidCode = item.BidCode;
                        p.VAT = item.VAT;
                        CheckBatchNumberExists(p);
                        item.Remaining = item.Remaining - value.RequiredNumber;
                        break;
                    }
                    p.DrugDeptInIDOrig = item.DrugDeptInIDOrig.GetValueOrDefault(0) > 0 ? item.DrugDeptInIDOrig : item.InID;
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
        {
            if (value != null)
            {
                if (ViewCase == 0 || ViewCase == 1)
                {
                    if (value.RequiredNumber > 0 && int.TryParse(value.RequiredNumber.ToString(), out int intOutPut))//20201228 QTD:Thêm điều kiện check số lẻ View xuất
                    {
                        if (CheckValidDrugAuto(value))
                        {
                            ChooseBatchNumber(value);
                        }
                    }
                    else
                    {
                        //MessageBox.Show(eHCMSResources.A0776_G1_Msg_InfoSLgLonHon0);
                        MessageBox.Show(eHCMSResources.Z0890_G1_SLgKgHopLe);
                    }
                }
                else
                {
                    if (int.TryParse(value.RequiredNumber.ToString(), out int intOutPutNhapCB))//20201228 QTD:Thêm điều kiện check số lẻ View nhập cân bằng
                    {
                        var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
                        foreach (RefGenMedProductDetails item in items)
                        {
                            OutwardDrugMedDept p = new OutwardDrugMedDept();
                            p.RefGenericDrugDetail = item;
                            p.GenMedProductID = item.GenMedProductID;
                            p.InBatchNumber = item.InBatchNumber;
                            p.InID = item.InID;
                            p.OutPrice = item.OutPrice;
                            p.OutQuantity = value.RequiredNumber;
                            p.InExpiryDate = item.InExpiryDate;
                            p.SdlDescription = item.SdlDescription;
                            p.Visa = item.Visa;
                            p.BidCode = item.BidCode;
                            p.VAT = item.VAT;
                            SelectedOutInvoice.OutwardDrugMedDepts.Add(p);
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
                    else
                    {
                        MessageBox.Show("Số lượng nhập phải là số nguyên!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
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
                        p.VAT = Convert.ToDouble(d.VAT);
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

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                void onInitDlg(IChooseBatchNumber proAlloc)
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
                }
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
            OutwardDrugMedDept p = SelectedOutwardDrugMedDept.DeepCopy();
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
                MessageBox.Show(eHCMSResources.A0915_G1_Msg_InfoPhChiXem);
            }
        }

        //int count = 0;
        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null /*&& count > 0*/)
            {
                //--▼-- 28/12/2020 DatTB Gán biến Kho đã chọn để so sánh
                var selectedStore = (RefStorageWarehouseLocation)cbx.SelectedItem;
                V_GroupTypes = selectedStore.V_GroupTypes;
                //--▲-- 28/12/2020 DatTB

                IsMainStorage = selectedStore.IsMain;
                IsSubStorage = selectedStore.IsSubStorage;
                StoreTypeID = (long)selectedStore.StoreTypeID;

                if (SelectedOutInvoice != null && SelectedOutInvoice.ReqDrugInClinicDeptID > 0)
                {
                    spGetInBatchNumberAndPrice_ByRequestID(SelectedOutInvoice.ReqDrugInClinicDeptID.GetValueOrDefault(), StoreID, true, null);
                }
                else
                {
                    if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugMedDepts != null)
                    {
                        SelectedOutInvoice.OutwardDrugMedDepts.Clear();
                    }
                    ClearSelectedOutInvoice(); //29/12/2020 DatTB Fix không clear khi thay đổi kho xuất
                }
                SelectedSellVisitor = new RefGenMedProductDetails(); //20190417 TBL: BM 0006750. Khi doi kho thi du lieu tren autocomplete cung phai clear
            }
            //count++;
        }

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardClinicDept Request = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (Request != null)
                {
                    StoreID = Request.OutFromStoreID.GetValueOrDefault(1);
                    if (StoreID == 1 && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
                    {
                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2444_G1_PhieuLinhDangYCTuKD);
                    }
                    ClearData();

                    //KMx: 2 dòng này phải để sau dòng assign SelectedOutInvoice.V_OutputTo, nếu không SelectedOutInvoice.ReqNumCode sẽ bị mất giá trị.
                    //Vì khi assign V_OutputTo thì XuatCho_SelectionChanged() sẽ được gọi và ReqNumCode bị thay đổi (27/06/2014 15:03).
                    //SelectedOutInvoice.ReqDrugInClinicDeptID = Request.ReqDrugInClinicDeptID;
                    //SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;

                    if (StoreID <= 0)
                    {
                        StoreID = Request.OutFromStoreID.GetValueOrDefault();
                    }

                    SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;

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
                    SelectedOutInvoice.ReqDrugWasExportFromMedDept = (bool)Request.DaNhanHang;

                    ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, Request.RefGenDrugCatID_1);
                    SelectedOutInvoice.RefGenDrugCatID_1 = Request.RefGenDrugCatID_1;

                    spGetInBatchNumberAndPrice_ByRequestID(Request.ReqDrugInClinicDeptID, StoreID, true, null);

                    //20210917 QTD: Disable Combobox Select StoreOut when load request drug
                    if (SelectedOutInvoice != null && SelectedOutInvoice.ReqNumCode != null)
                    {
                        IsEnableCbxStoreOut = false;
                    }
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
                            SumTotalPrice();
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

        public void btnFindRequest(object sender, RoutedEventArgs e)
        {
            //hien thi nhung phieu da duyet ma chua xuat de xuat hang;
            //void onInitDlg(IRequestSearch proAlloc)
            //{
            IRequestSearch proAlloc = Globals.GetViewModel<IRequestSearch>();
            proAlloc.SearchCriteria.IsApproved = true;

            //KMx: Bên xuất hàng tìm phiếu yêu cầu theo ngày duyệt phiếu, không phải ngày tạo phiếu (18/12/2014 09:53).
            proAlloc.SearchCriteria.FindByApprovedDate = true;

            //proAlloc.SearchCriteria.DaNhanHang = false;
            proAlloc.SearchCriteria.DaNhanHang = false; //--◄--16/12/2020 DatTB sửa NULL thành 0 -> chỉ lấy chưa xuất hàng
            proAlloc.SearchCriteria.FromDate = Globals.ServerDate.Value.AddDays(-10); //--◄--16/12/2020 DatTB sửa 1 ngày thành 10 ngày
            proAlloc.SearchCriteria.ToDate = Globals.ServerDate.Value;
            proAlloc.V_MedProductType = V_MedProductType;
            //▼====== #003
            if (!chk_checked)
            {
                proAlloc.SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
            }
            else
            {
                proAlloc.IsRequestFromHIStore = true;
                proAlloc.SetList();
                proAlloc.SearchRequestDrugInwardHIStore(0, Globals.PageSize);
            }
            //▲====== #003
            //}
            //GlobalsNAV.ShowDialog<IRequestSearch>(onInitDlg);
            GlobalsNAV.ShowDialog_V3(proAlloc, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        //▼====== #003
        private bool chk_checked = false;
        public void Chk_Checked(object sender, RoutedEventArgs e)
        {
            chk_checked = true;
        }

        public void Chk_Unchecked(object sender, RoutedEventArgs e)
        {
            chk_checked = false;
        }

        //▲====== #003
        private void OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice OutwardInvoice, bool _bThuTien)
        {
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugMedDeptInvoice_SaveByType(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                //KMx: MedDeptCanGetCash: Cấu hình cho Khoa Dược có được phép thu tiền sau khi tạo phiếu xuất không?
                                bThuTien = _bThuTien;
                                if (bThuTien && Globals.ServerConfigSection.MedDeptElements.MedDeptCanGetCash)
                                {
                                    ShowFormCountMoney();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
                                    RefeshData();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                if (!IsOutClinicDept)
                                {
                                    //phat su kien de form duyet phieu load lai phieu  
                                    Globals.EventAggregator.Publish(new DrugDeptLoadAgainReqOutwardClinicDeptEvent { RequestID = OutwardInvoice.ReqDrugInClinicDeptID.GetValueOrDefault() });
                                }
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

        private bool CheckData()
        {
            if (SelectedOutInvoice == null)
            {
                return false;
            }

            if (SelectedOutInvoice.OutDate == null || SelectedOutInvoice.OutDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.A0863_G1_Msg_InfoNgXuatKhHopLe4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            //20190315 TBL: Hien tai chan viec xuat tu khoa duoc cho kho nha thuoc
            if (SelectedOutInvoice.OutputToID == 2)
            {
                MessageBox.Show(eHCMSResources.Z2617_G1_KhongTheXuatChoKhoNay);
                return false;
            }

            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StoreID = StoreID;

            string strError = "";
            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
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
                if (!IsBalanceView)
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

            }

            if (SelectedOutInvoice.OutwardDrugMedDepts == null || SelectedOutInvoice.OutwardDrugMedDepts.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0640_G1_Msg_InfoKhCoCTietPhXuat);
                return false;
            }
            else
            {
                if (!IsBalanceView)
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
                            && Globals.ServerConfigSection.MedDeptElements.BlockOutwardDrugFromMedDeptToClinicWhenRequestQtyDiffOutQty && !IsEstimationFromRequest)
                        {
                            MessageBox.Show("SL xuất khác SL yêu cầu. Vui lòng kiểm tra lại!");
                            return false;
                        }
                    }
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
            if (CheckData())
            {
                SelectedOutInvoice.V_MedProductType = V_MedProductType;
                //KMx: Dời TypID == HUYHANG vào trong Get của Property IsInternal luôn cho dễ quản lý (22/09/2014 14:37).
                //if (SelectedOutInvoice.IsInternal || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
                if (SelectedOutInvoice.ReqDrugWasExportFromMedDept && Globals.ServerConfigSection.MedDeptElements.SecondExportBlockFormTheRequestForm)
                {
                    Globals.ShowMessage(eHCMSResources.Z3003_G1_ChanXuatThuocLan2TuPhieuLinh, eHCMSResources.T0432_G1_Error);
                    return;
                }
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugMedDeptInvoice_SaveByType_Balance(SelectedOutInvoice, false);
                }
                else if (SelectedOutInvoice.IsInternal)
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugMedDeptInvoice_SaveByType(SelectedOutInvoice, false);
                }
                else
                {
                    SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
                    OutwardDrugMedDeptInvoice_SaveByType(SelectedOutInvoice, true);
                }
            }
        }

        public void btnMoney()
        {
            bThuTien = true;
            ShowFormCountMoney();
        }

        public void btnNew()
        {
            //if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            RefeshData();
            // }
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

        //KMx: Không hiểu để icount để làm gì, dẫn đến không đổi được giá (15/07/2014 10:01).
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
                SumTotalPrice();
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
            if (message != null && IsActive)
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

        private bool bThuTien = true;
        private void ShowFormCountMoney()
        {
            void onInitDlg(ISimplePayPharmacy proAlloc)
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
            }
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        #region IHandle<DrugDeptChooseBatchNumberEvent> Members

        public void Handle(DrugDeptChooseBatchNumberEvent message)
        {
            if (message != null && IsActive)
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
                SelectedOutwardDrugMedDept.VAT = message.BatchNumberSelected.VAT;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<DrugDeptChooseBatchNumberResetQtyEvent> Members

        public void Handle(DrugDeptChooseBatchNumberResetQtyEvent message)
        {
            if (message != null && IsActive)
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
                SelectedOutwardDrugMedDept.VAT = message.BatchNumberSelected.VAT;
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
            if (!IsBalanceView)
            {
                RefOutputTypes = Globals.RefOutputType.Where(x => x.IsSelected == true && x.TypID != 26).ToObservableCollection(); //20190624 TBL: Khong lay loai Xuat tra NCC
            }
            else
            {
                RefOutputTypes = Globals.RefOutputType.Where(x => x.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO).ToObservableCollection();
            }
            SetDefaultOutputType();
        }

        //KMx: Khi login đã lấy RefOutputType hết rồi, không cần về server nữa (28/06/2014 15:58).
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
                //--▼--29/12/2020 DatTB
                if (SelectedOutInvoice.TypID == 3)
                {
                    Is_Enabled = true;
                }
                //--▲--29/12/2020 DatTB

                if (SelectedOutInvoice.TypID != (long)AllLookupValues.RefOutputType.BANLE)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                }
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

        //KMx: Bị sai khi load phiếu xuất cũ lên xem. VD: Load phiếu 1, xuất cho: nhân viên. Load phiếu 2, xuất cho: kho khác. Phiếu số 2 không hiển thị "Kho nhận" và "Mã phiếu YC"
        //Vấn đề là do khác "xuất cho".(15/07/2014 18:14)
        //private int xcount = 0;
        //public void XuatCho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (xcount > 0 && SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaid && (sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
        //    {
        //        ClearSelectedOutInvoice();
        //    }
        //    xcount++;
        //}

        public void XuatCho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.outiID == 0 && (sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                ClearSelectedOutInvoice();
            }
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
                        proAlloc1.V_MedProductType = V_MedProductType;
                        proAlloc1.V_GroupTypes = V_GroupTypes; //--28/12/2020 DatTB Thêm điều kiện lọc loại kho GTGT
                        proAlloc1.bAdd = false;
                        //if (SelectedOutInvoice != null && SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
                        //{
                        //    proAlloc1.V_MedProductType = V_MedProductType;
                        //    proAlloc1.IsSubStorage = true;
                        //}
                        if (Globals.ServerConfigSection.MedDeptElements.IsEnableFilterStorage)
                        {
                            proAlloc1.IsSubStorage = IsSubStorage;
                            proAlloc1.IsMainStorage = IsMainStorage;
                            proAlloc1.StoreTypeID = StoreTypeID;
                        }
                    }
                    GlobalsNAV.ShowDialog<IDrugDeptStorage>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
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
            if (IsActive && message != null)
            {
                RefStorageWarehouseLocation sto = message.SelectedStorage as RefStorageWarehouseLocation;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.StoreID;
                    SelectedOutInvoice.FullName = sto.swhlName;
                    SelectedOutInvoice.Address = "";
                    SelectedOutInvoice.NumberPhone = "";
                    //▼====: #001
                    if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
                    {
                        OutputToStoreID = sto.StoreID;
                    }
                    else
                    {
                        OutputToStoreID = null;
                    }
                    //▲====: #001
                }
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchHospitalEvent> Members

        public void Handle(DrugDeptCloseSearchHospitalEvent message)
        {
            if (IsActive && message != null)
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
            if (IsActive && message != null)
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
            if (IsActive && message != null)
            {
                Staff sto = message.SelectedStaff as Staff;
                if (sto != null)
                {
                    SelectedOutInvoice.OutputToID = sto.StaffID;

                    //KMx: Tránh lỗi BS.BS.Tên khi in phiếu xuất (15/06/2015 15:15).
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
            if (!IsBalanceView)
            {
                VisibilityHuy = Visibility.Visible;
            }
            else
            {
                VisibilityHuy = Visibility.Collapsed;
            }
            _NotEditCost = true;
            IsGetProductHuy = false;

            //KMx: Chỉ clear khi nào là phiếu mới, nếu load phiếu cũ lên xem thì không clear, nếu không thì thông tin sẽ bị mất (16/07/2014 10:55).
            if (SelectedOutInvoice != null && SelectedOutInvoice.outiID <= 0)
            {
                ClearSelectedOutInvoice();
            }

            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.BANLE)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIATHONGTHUONG;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                    //20190114 TTM: Nếu chọn loại là xuất nội bộ thì phải là giá vốn và không được chỉnh sửa
                    //              Nên set enable loại giá thành false.
                    _NotEditCost = false;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.HUYHANG
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC
                    || SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DIEUCHUYEN)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                    VisibilityHuy = Visibility.Collapsed;
                    _NotEditCost = false;
                }
                else if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.CANBANGKHO)
                {
                    SelectedOutInvoice.V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON;
                    SelectedOutInvoice.StoreID = 0;
                }
            }

            if (SelectedOutInvoice.TypID == (long)AllLookupValues.RefOutputType.XUAT_DUNGCHUNG && Globals.ServerConfigSection.CommonItems.EnableCheckboxXCD)
            {
                IsEnableCheckboxXCD = true;
            }
            else
            {
                IsEnableCheckboxXCD = false;
                IsXuatChongDich = false;
            }
            NotifyOfPropertyChange(() => IsEditCost);
        }

        private bool? IsCode = false;
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode(V_MedProductType, txt);
                if (ViewCase == 0 || ViewCase == 1)
                {
                    SearchRefGenMedProductDetails(Code, true);
                }
                else
                {
                    GetDrugForBalanceCompleteFromCategory(Code, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(text))
                {
                    if (ViewCase == 0 || ViewCase == 1)
                    {
                        SearchRefGenMedProductDetails((sender as TextBox).Text, true);
                    }
                    else
                    {
                        GetDrugForBalanceCompleteFromCategory((sender as TextBox).Text, true);
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

        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.DisplayIndex == (int)DataGridCol.DonGia || e.Column.DisplayIndex == (int)DataGridCol.ThucXuat)
            {
                SumTotalPrice();
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
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
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }
        #endregion

        #region PharmacyPayEvent
        private long PaymentID = 0;

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            var paymentTypeTask = new AddTracsactionMedDeptForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug)
        {
            var paymentTypeTask = new AddTracsactionMedDeptForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            PaymentID = paymentTypeTask.PaymentID;
            yield break;
        }


        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (IsActive && message != null)
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

        public void btnUpdate()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }

            if (SelectedOutInvoice.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.A0951_G1_Msg_InfoKhTheCNhat4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                MessageBox.Show(eHCMSResources.A0947_G1_Msg_InfoKhTheCNhat3, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            void onInitDlg(IEditXuatNoiBo proAlloc)
            {
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.SelectedOutInvoice = SelectedOutInvoice.DeepCopy();
                proAlloc.SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
                proAlloc.OutwardDrugMedDeptsCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                proAlloc.OutwardDrugListCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                proAlloc.ListOutwardDrugFirstCopy = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                proAlloc.ListOutwardDrugFirst = SelectedOutInvoice.OutwardDrugMedDepts.DeepCopy();
                proAlloc.SumTotalPriceNotVAT = TotalPriceNotVAT;
                proAlloc.SumTotalPrice = SelectedOutInvoice.TotalInvoicePrice.DeepCopy();
                proAlloc.IDFirst = SelectedOutInvoice.outiID.DeepCopy();
                proAlloc.StoreID = StoreID;
            }
            GlobalsNAV.ShowDialog<IEditXuatNoiBo>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void Handle(DrugDeptCloseEditMedDeptInvoiceEvent message)
        {
            if (message != null && IsActive)
            {
                GetOutwardDrugMedDeptInvoice(message.SelectedOutMedDeptInvoice.outiID);
            }
        }

        public void Handle(DrugDeptCloseEditPayedEvent message)
        {
            if (message != null && IsActive)
            {
                GetOutwardDrugMedDeptInvoice(message.SelectedOutMedDeptInvoice.outiID);
            }
        }

        private void DeleteOutwardDrugMedDeptInvoice()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteOutwardDrugMedDeptInvoice(SelectedOutInvoice.outiID, Globals.LoggedUserAccount.Staff.StaffID, SelectedOutInvoice.V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            bool value = contract.EndDeleteOutwardDrugMedDeptInvoice(asyncResult);

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
            /*24/12/2020 DatTB*/
            /*▼====: #002*/
            //if (!SelectedOutInvoice.IsLockedUpdate && !SelectedOutInvoice.CanUpdate)
            //{
            //    MessageBox.Show(eHCMSResources.Z2309_G1_KhongTheHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            /*▲====: #002*/
            if (MessageBox.Show(eHCMSResources.Z0906_G1_CoMuonHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteOutwardDrugMedDeptInvoice();
            }
        }

        public void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (SelectedOutInvoice.VAT < 1 || SelectedOutInvoice.VAT > 2)
            {
                MessageBox.Show(string.Format("{0} ", eHCMSResources.K0263_G1_VATKhongHopLe2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedOutInvoice.VAT = 1;
            }
            SumTotalPrice();
        }

        //▼====== #003
        public void Handle(DrugDeptCloseSearchRequestForHIStoreEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardForHiStore Request = message.SelectedRequest as RequestDrugInwardForHiStore;
                if (Request != null)
                {
                    ClearData();

                    if (StoreID <= 0)
                    {
                        StoreID = Request.OutFromStoreID.GetValueOrDefault();
                    }

                    SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.XUATNOIBO;

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

                    SelectedOutInvoice.ReqDrugInClinicDeptID = Request.RequestDrugInwardHiStoreID;
                    SelectedOutInvoice.ReqNumCode = Request.ReqNumCode;
                    SelectedOutInvoice.ReqDrugWasExportFromMedDept = (bool)Request.DaNhanHang;

                    ChangeValue(SelectedOutInvoice.RefGenDrugCatID_1, Request.RefGenDrugCatID_1);
                    SelectedOutInvoice.RefGenDrugCatID_1 = Request.RefGenDrugCatID_1;

                    GetInBatchNumberAndPrice_ByRequestID(Request.RequestDrugInwardHiStoreID, StoreID, true, null);

                    //20210917 QTD: Disable Combobox Select StoreOut when load request drug
                    if (SelectedOutInvoice != null && SelectedOutInvoice.ReqNumCode != null)
                    {
                        IsEnableCbxStoreOut = false;
                    }
                }
            }
        }

        private void GetInBatchNumberAndPrice_ByRequestID(long RequestID, long StoreID, bool IsOutClinicDept, RequestDrugInwardForHiStore Req)
        {
            this.ShowBusyIndicator();

            GetIsCost();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetInBatchNumberAndPrice_ListForRequest(IsCost, RequestID, StoreID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInBatchNumberAndPrice_ListForRequest(asyncResult);
                            if (!IsOutClinicDept)
                            {
                                if (Req != null)
                                {
                                    SelectedOutInvoice.ReqDrugInClinicDeptID = Req.RequestDrugInwardHiStoreID;
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
                            SumTotalPrice();
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
        //▲====== #003

        //▼====== #004
        #region Tìm bằng tên hoạt chất
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
        private bool _visSearchByGenericName = false;
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

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                chkSearchByGenericName.IsChecked = true;
                vIsSearchByGenericName = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        #endregion
        //▲====== #004

        //▼====== #005
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
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

        public void AddItem_Click(object sender, object e)
        {
            ReCountQtyRequest();
            AddListOutwardDrugMedDept(SelectedSellVisitor);
        }
        //▲====== #005
        #region Xuất cân bằng
        private int _ViewCase = 0;
        public int ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                _ViewCase = value;
                if (_ViewCase >= 1)
                {
                    _IsBalanceView = true;
                    LoadRefOutputType();
                    VisibilityHuy = Visibility.Collapsed;
                    VisibilityHuyView = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => ViewCase);
                NotifyOfPropertyChange(() => IsBalanceView);
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
                    NotifyOfPropertyChange(() => mThuTienAndBalance);
                    NotifyOfPropertyChange(() => IsBalanceView);
                }
            }
        }
        private Visibility _VisibilityHuyView = Visibility.Visible;
        public Visibility VisibilityHuyView
        {
            get { return _VisibilityHuyView; }
            set
            {
                if (_VisibilityHuyView != value)
                {
                    _VisibilityHuyView = value;
                    NotifyOfPropertyChange(() => VisibilityHuyView);
                }
            }
        }

        private void OutwardDrugMedDeptInvoice_SaveByType_Balance(OutwardDrugMedDeptInvoice OutwardInvoice, bool _bThuTien)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugMedDeptInvoice_SaveByType_Balance(OutwardInvoice, ViewCase, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndDrugMedDeptInvoice_SaveByType_Balance(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                bThuTien = _bThuTien;
                                if (bThuTien && Globals.ServerConfigSection.MedDeptElements.MedDeptCanGetCash)
                                {
                                    ShowFormCountMoney();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
                                    RefeshData();
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                if (!IsOutClinicDept)
                                {
                                    //phat su kien de form duyet phieu load lai phieu  
                                    Globals.EventAggregator.Publish(new DrugDeptLoadAgainReqOutwardClinicDeptEvent { RequestID = OutwardInvoice.ReqDrugInClinicDeptID.GetValueOrDefault() });
                                }
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
        private void GetDrugForBalanceCompleteFromCategory(string Name, bool? IsCode)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            //KMx: Không tìm tất cả. Nếu làm như vậy thì sẽ bị đứng chương trình vì quá nhiều dữ liệu (05/07/2014 11:53).
            if (IsCode == false && Name.Length < 1)
            {
                return;
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
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForBalanceCompleteFromCategory(IsSearchByGenericName, IsCost, Name, StoreID
                            , V_MedProductType, RefGenDrugCatID_1, RequestID, IsCode, OutputToStoreID, null, null, true
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetDrugForBalanceCompleteFromCategory(asyncResult);

                                    RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                                    RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

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
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }
        private bool _mThuTienAndBalance;
        public bool mThuTienAndBalance
        {
            get
            {
                return _mThuTien && IsBalanceView;
            }
            set
            {
                if (_mThuTienAndBalance == value)
                    return;
                _mThuTienAndBalance = value;
                NotifyOfPropertyChange(() => mThuTienAndBalance);
            }
        }

        private bool _IsEnableCbxStoreOut = true;
        public bool IsEnableCbxStoreOut
        {
            get { return _IsEnableCbxStoreOut; }
            set
            {
                if (_IsEnableCbxStoreOut != value)
                {
                    _IsEnableCbxStoreOut = value;
                    NotifyOfPropertyChange(() => IsEnableCbxStoreOut);
                }
            }
        }

        //▼====: #009
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

        private long _StoreTypeID = (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT;
        public long StoreTypeID
        {
            get { return _StoreTypeID; }
            set
            {
                if (_StoreTypeID != value)
                {
                    _StoreTypeID = value;
                    NotifyOfPropertyChange(() => StoreTypeID);
                }
            }
        }
        //▲====: #009

        //▼====: #010
        private bool _IsEnableTextBoxNote = true;
        public bool IsEnableTextBoxNote
        {
            get
            {
                return _IsEnableTextBoxNote;
            }
            set
            {
                if (_IsEnableTextBoxNote != value)
                {
                    _IsEnableTextBoxNote = value;
                    NotifyOfPropertyChange(() => IsEnableTextBoxNote);
                }
            }
        }

        private bool _IsXuatChongDich = false;
        public bool IsXuatChongDich
        {
            get
            {
                return _IsXuatChongDich;
            }
            set
            {
                if (_IsXuatChongDich != value)
                {
                    _IsXuatChongDich = value;
                    if (_IsXuatChongDich == true)
                    {
                        IsEnableTextBoxNote = false;
                        SelectedOutInvoice.Notes = "[XCD]";
                    }
                    else
                    {
                        IsEnableTextBoxNote = true;
                        SelectedOutInvoice.Notes = "";
                    }
                    NotifyOfPropertyChange(() => IsXuatChongDich);
                }
            }
        }

        private bool _IsEnableCheckboxXCD = false;
        public bool IsEnableCheckboxXCD
        {
            get
            {
                return _IsEnableCheckboxXCD;
            }
            set
            {
                if (_IsEnableCheckboxXCD != value)
                {
                    _IsEnableCheckboxXCD = value;
                    NotifyOfPropertyChange(() => IsEnableCheckboxXCD);
                }
            }
        }
        //▲====: #010

        //▼====: #013
        private bool _IsEstimationFromRequest = false;
        public bool IsEstimationFromRequest
        {
            get
            {
                return _IsEstimationFromRequest;
            }
            set
            {
                if (_IsEstimationFromRequest != value)
                {
                    _IsEstimationFromRequest = value;
                    NotifyOfPropertyChange(() => IsEstimationFromRequest);
                }
            }
        }
        //▲====: #013
        #endregion
    }
}
