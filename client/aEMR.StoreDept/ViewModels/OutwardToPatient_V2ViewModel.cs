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
using aEMR.Common.Converters;
using Castle.Windsor;
/*
 * 20170803 #001 CMN:   Added variable to check InPt 5 year HI without paid enough
 * 20180319 #002 CMN:   Set CalByUnitUse into globals function
 * 20180329 #003 CMN:   Added CheckValidMedicalInstructionDate Functions
 * 20180406 #004 TTM:   Vì lý do sau khi lưu phiếu xuất trường Ngày nhập viện đã bị mất (null), nên khi thêm một món thuốc/ y cụ/ hóa chất sẽ khiến cho set điều kiện sai trong hàm ApplyValidMedicalInstructionDate
 *                      trong Globals (line 1852) bị sai dẫn đến lấy ExamDate thực hiện thay vì AdmissionDate. Nên tạo một biến tạm lưu thông tin AdmissionDate trước khi thực hiện lưu và gán lại giá trị cho
 *                      AdmissionDate sau khi load.
 * 20180908 #005 TBL:   Kiem tra bac si chi dinh co chung chi hanh nghe    
 * 20180908 #006 TBL:   Khi bo tick tinh BH thi gia BH van giu. Can phai TBR  
 * 20181006 #007 TTM:   BM0000155: Fix lỗi thay đổi liều dùng không thay đổi giá trị
 *                      Ví dụ: Thay đổi liều dùng sáng từ 0 -> 1 thì không cập nhật mà vẫn giữ giá trị 0.
 * 20181009 #008 TTM:   Thêm điều kiện ngăn chặn việc người dùng xuất số lượng nhiều hơn số lượng còn lại trong LÔ.
 *                      Ví dụ: Thuốc còn 30 viên chia đều 2 lô A và B, người dùng chỉ xuất 5 viên thuốc => phần mềm lấy 5 viên trong lô A ra. Sau đó người dùng đổi ý xuất
 *                      16 viên => nhiều hơn lô đang chọn 1 viên => không xuất đc do trong lô không còn đủ 16 viên mà chỉ còn 15 viên. Người dùng cần phải xóa dòng xuất
 *                      đó mà thêm lại (do tồn còn 30 nhưng đc chia ở 2 lô khác nhau, nếu người dùng chọn 16 viên ngay từ đầu thì sẽ có 2 dòng xuất trong grid, 1 dòng 15 viên
 *                      1 dòng 1 viên và xuất bt).
 * 20181113 #009 TBL:   BM 0005237: Ngay xuat khong duoc lon hon ngay xuat vien, khong duoc nho hon ngay dang ky neu la BN vang lai                 
 * 20181119 #010 TTM:   BM 0005257: Tạo out standing task tìm kiếm bệnh nhân nằm tại khoa và sự kiện chụp lại khi chọn bệnh nhân từ Out standing task.
 * 20190110 #011 TTM:               Bổ sung phiếu công khai thuốc - Xuất thuốc cho bệnh nhân
 * 20191121 #012 TTM:   BM 0019578: Giải quyết vấn đề liều dùng cho xuất bệnh nhân từ y lệnh ở màn hình xuất cho bệnh nhân nội trú. Giải thích ở dưới chỗ thực hiện task.
 * 20191224 #013 TTM:   Fix lỗi khi lưu, cập nhật, load dữ liệu của bệnh nhân ngoại trú sử dụng đồ nội trú sẽ báo null => Không có PtRegistrationID nên bị null.
 * 20200201 #014 TBL:   BM 0022872: Fix lỗi khi đổi lô SX có giá khác nhau thì giá BH không đổi làm cho tính toán sai
 * 20200417 #015 TTM:   BM 0032130: Fix lỗi thuốc có 2 lô mà 1 lô đã hết tồn thì không cho chọn lô còn lại để đổi.
 * 20200424 #016 TBL:   BM 0037148: Fix lỗi mất giá khi đổi lô cho phiếu xuất đã lưu
 * 20200724 #017 TTM:   BM 0039399: Lỗi không cập nhật giá lại khi tính lại bill.
 * 20201110 #018 TNHX:   BM : Thêm report phiếu sao thuốc hỗ trợ điều dưỡng phát thuốc nhanh
 * 20210223 #019 TNHX:  214 : Thêm cấu hình xuất thuốc 0.5 
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IOutwardToPatient_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutwardToPatient_V2ViewModel : Conductor<object>, IOutwardToPatient_V2
        , IHandle<DrugDeptCloseSearchOutClinicDeptInvoiceEvent>
        , IHandle<ClinicDeptChooseBatchNumberEvent>
        , IHandle<ClinicDeptChooseBatchNumberResetQtyEvent>
        , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
        , IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<ClinicDeptInPtSelReqFormForOutward>
        , IHandle<InPatientRegistrationSelectedForOutwardToPatient_V2>
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

        private bool _requireDoctorAndDate;
        public bool RequireDoctorAndDate
        {
            get { return _requireDoctorAndDate; }
            set
            {
                if (_requireDoctorAndDate != value)
                {
                    _requireDoctorAndDate = value;
                    NotifyOfPropertyChange(() => RequireDoctorAndDate);
                }
            }
        }

        private bool _DoseVisibility;
        public bool DoseVisibility
        {
            get { return _DoseVisibility; }
            set
            {
                if (_DoseVisibility != value)
                {
                    _DoseVisibility = value;
                    if (DoseVisibility) IsInputDosage = true;
                    NotifyOfPropertyChange(() => DoseVisibility);
                    NotifyOfPropertyChange(() => CheckedDoseVisibility);
                }
            }
        }

        private bool _IsInputDosage;
        public bool IsInputDosage
        {
            get { return _IsInputDosage; }
            set
            {
                if (_IsInputDosage != value)
                {
                    _IsInputDosage = value;
                    NotifyOfPropertyChange(() => IsInputDosage);
                    NotifyOfPropertyChange(() => CheckedDoseVisibility);
                    NotifyOfPropertyChange(() => DrugIsInputDosage);
                }
            }
        }

        public bool CheckedDoseVisibility
        {
            get
            {
                return DoseVisibility && IsInputDosage;
            }
        }
        public bool DrugIsInputDosage
        {
            get
            {
                return !IsInputDosage && this._V_MedProductType == AllLookupValues.MedProductType.THUOC;
            }
        }

        private bool _calByUnitUse;
        public bool CalByUnitUse
        {
            get { return _calByUnitUse; }
            set
            {
                if (_calByUnitUse != value)
                {
                    _calByUnitUse = value;
                    NotifyOfPropertyChange(() => CalByUnitUse);
                }
            }
        }
        // HPT: Thêm thuộc tính xác định trường hợp xuất thuốc cho bệnh nhân từ đăng ký vãng lai và tiền giải phẫu (chưa nhập viện)
        private bool _isCasualOrPreOpPt = false;
        public bool IsCasualOrPreOpPt
        {
            get
            {
                return _isCasualOrPreOpPt;
            }
            set
            {
                _isCasualOrPreOpPt = value;
                NotifyOfPropertyChange(() => IsCasualOrPreOpPt);
            }
        }
        private ISearchPatientAndRegistration _searchRegistrationContent;
        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
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

        [ImportingConstructor]
        public OutwardToPatient_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //form tim kiem
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = false;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();

            StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, false, false);
            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }

            SearchCriteria = new MedDeptInvoiceSearchCriteria();
            InitSelectedOutInvoice();

            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            long CurrentDeptID = Globals.DeptLocation.DeptID;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
            {
                aucHoldConsultDoctor.CurrentDeptID = Globals.DeptLocation.DeptID;
            }
            Is_Enabled = false; //--17/12/2020 DatTB
        }
        //▼====== #010:
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
            homeVm.OutstandingTaskContent = ostvm;
            homeVm.IsExpandOST = true;
            ostvm.WhichVM = SetOutStandingTask.XUATTHUOC;
        }

        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
        }
        //▲====== #010
        public void InitData()
        {
            if (IsCasualOrPreOpPt == true)
            {
                SearchRegistrationContent.SearchByVregForPtOfType = 1;
                SearchRegistrationContent.SearchAdmittedInPtRegOnly = false;
            }
            else
            {
                SearchRegistrationContent.SearchByVregForPtOfType = 0;
                SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            }
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                SearchRegistrationContent.mTimBN = false;
                SearchRegistrationContent.mThemBN = false;
                SearchRegistrationContent.mTimDangKy = true;
                SearchRegistrationContent.PatientFindByVisibility = false;
                SearchRegistrationContent.PatientFindBy = DataEntities.AllLookupValues.PatientFindBy.NGOAITRU;
                SearchRegistrationContent.IsSearchOutPtRegistrationOnly = true;
            }
            if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, false, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, false, true, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, true, false, false);
            }
            else if (V_MedProductType == AllLookupValues.MedProductType.HOA_CHAT)
            {
                StoreCbx = Globals.checkStoreWareHouse((long)V_MedProductType, false, false, false, false, true);
            }
            if (StoreCbx == null || StoreCbx.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
            SetDefaultForStore();
        }

        private void InitSelectedOutInvoice()
        {
            SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            //KMx: Không set mặc định là ngày hiện tại nữa, để cho user chọn, vì OutDate được dùng để chọn lô khi đi tìm thuốc. Tránh lỗi chọn lô có ngày nhập sau ngày xuất, sai báo cáo nhập xuất tồn (25/03/2015 15:13).
            SelectedOutInvoice.OutDate = Globals.GetCurServerDateTime();
            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StaffName = GetStaffLogin().FullName;
            //SetDefaultForStore();
        }

        private long StoreID = 0;
        private void RefreshData()
        {
            //if (chkAllCountHI != null)
            //{
            //    chkAllCountHI.IsChecked = false;
            //}

            //if (chkAllCountPatient != null)
            //{
            //    chkAllCountPatient.IsChecked = false;
            //}

            if (SelectedOutInvoice != null)
            {
                StoreID = SelectedOutInvoice.StoreID.GetValueOrDefault();
            }

            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            //KMx: Không set mặc định là ngày hiện tại nữa, để cho user chọn, vì OutDate được dùng để chọn lô khi đi tìm thuốc. Tránh lỗi chọn lô có ngày nhập sau ngày xuất, sai báo cáo nhập xuất tồn (25/03/2015 15:13).
            SelectedOutInvoice.OutDate = Globals.GetCurServerDateTime();
            SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
            SelectedOutInvoice.StaffName = GetStaffLogin().FullName;
            if (StoreID > 0)
            {
                SelectedOutInvoice.StoreID = StoreID;
            }
            else
            {
                SetDefaultForStore();
            }
            ClearData();
        }

        private void ClearData()
        {
            OutwardDrugClinicDeptsCopy = null;
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
            if (au != null)
            {
                au.Text = "";
            }
            RequestDrugInwardClinicDeptListSelected = null;
            RequestDrugInwardClinicDeptLst = null;
            if (CurrentInstructionCollection != null)
            {
                CurrentInstructionCollection.Clear();
            }
            //20200424 TBL: BM 0037148: Fix lỗi hiển thị sai số lượng tồn khi đổi lô cho phiếu xuất đã lưu
            OutwardDrugMedDeptListByGenMedProductIDFirst = new ObservableCollection<OutwardDrugClinicDept>();
        }


        private void SetDefaultForStore()
        {
            if (StoreCbx.Count > 0 && StoreCbx != null && SelectedOutInvoice != null)
            {
                SelectedOutInvoice.StoreID = StoreCbx.FirstOrDefault().StoreID;
                GetAllOutwardTemplate(StoreCbx.FirstOrDefault().DeptID.GetValueOrDefault());
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

        private bool _mXuatChoBenhNhan_Xem = true;
        private bool _mXuatChoBenhNhan_PhieuMoi = true;
        private bool _mXuatChoBenhNhan_XemIn = true;
        private bool _mXuatChoBenhNhan_In = true;

        public bool mXuatChoBenhNhan_Xem
        {
            get
            {
                return _mXuatChoBenhNhan_Xem;
            }
            set
            {
                if (_mXuatChoBenhNhan_Xem == value)
                    return;
                _mXuatChoBenhNhan_Xem = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_Xem);
            }
        }

        public bool mXuatChoBenhNhan_PhieuMoi
        {
            get
            {
                return _mXuatChoBenhNhan_PhieuMoi;
            }
            set
            {
                if (_mXuatChoBenhNhan_PhieuMoi == value)
                    return;
                _mXuatChoBenhNhan_PhieuMoi = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_PhieuMoi);
            }
        }

        public bool mXuatChoBenhNhan_XemIn
        {
            get
            {
                return _mXuatChoBenhNhan_XemIn;
            }
            set
            {
                if (_mXuatChoBenhNhan_XemIn == value)
                    return;
                _mXuatChoBenhNhan_XemIn = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_XemIn);
            }
        }

        public bool mXuatChoBenhNhan_In
        {
            get
            {
                return _mXuatChoBenhNhan_In;
            }
            set
            {
                if (_mXuatChoBenhNhan_In == value)
                    return;
                _mXuatChoBenhNhan_In = value;
                NotifyOfPropertyChange(() => mXuatChoBenhNhan_In);
            }
        }
        #endregion

        #region Properties Member

        private long V_TranRefType = (long)AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC;

        private IAucHoldConsultDoctor _aucHoldConsultDoctor;
        public IAucHoldConsultDoctor aucHoldConsultDoctor
        {
            get
            {
                return _aucHoldConsultDoctor;
            }
            set
            {
                if (_aucHoldConsultDoctor != value)
                {
                    _aucHoldConsultDoctor = value;
                    NotifyOfPropertyChange(() => aucHoldConsultDoctor);
                }
            }
        }

        private IMinHourDateControl _MedicalInstructionDateContent;
        public IMinHourDateControl MedicalInstructionDateContent
        {
            get { return _MedicalInstructionDateContent; }
            set
            {
                _MedicalInstructionDateContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionDateContent);
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

        private AllLookupValues.MedProductType _V_MedProductType = AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public AllLookupValues.MedProductType V_MedProductType
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
                    NotifyOfPropertyChange(() => DrugIsInputDosage);
                }
            }
        }


        private bool? IsCost = false;

        private ObservableCollection<RequestDrugInwardClinicDept> _RequestDrugInwardClinicDeptLst;
        public ObservableCollection<RequestDrugInwardClinicDept> RequestDrugInwardClinicDeptLst
        {
            get
            {
                return _RequestDrugInwardClinicDeptLst;
            }
            set
            {
                if (_RequestDrugInwardClinicDeptLst != value)
                {
                    _RequestDrugInwardClinicDeptLst = value;
                    NotifyOfPropertyChange(() => RequestDrugInwardClinicDeptLst);
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

        private ObservableCollection<OutwardDrugClinicDept> OutwardDrugClinicDeptsCopy;

        private OutwardDrugClinicDeptInvoice SelectedOutInvoiceCoppy;

        private OutwardDrugClinicDeptInvoice _SelectedOutInvoice;
        public OutwardDrugClinicDeptInvoice SelectedOutInvoice
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
                //NotifyOfPropertyChange(() => VisibilityName);
                //NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        #endregion

        private IEnumerator<IResult> DoGetStoreToSell()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }


        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                Button colBatchNumber = grdPrescription.Columns[4].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = grdPrescription.Columns[4].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        public void grdPrescription_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            SumTotalPrice();
        }

        public void grdOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //▼====== #007:
            if (e.Column.Equals(grdPrescription.GetColumnByName("colMDoseStr")))
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedOutwardDrugClinicDept);
            }
            else if (e.Column.Equals(grdPrescription.GetColumnByName("colADoseStr")))
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedOutwardDrugClinicDept);
            }
            else if (e.Column.Equals(grdPrescription.GetColumnByName("colEDoseStr")))
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedOutwardDrugClinicDept);
            }
            else if (e.Column.Equals(grdPrescription.GetColumnByName("colNDoseStr")))
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedOutwardDrugClinicDept);
            }
            //▲====== #007

            //▼====== #007: Comment lại vì cách này cũ đã sai, sử dụng GetValue(FrameworkElement.NameProperty) sẽ không lấy được giá trị của cột. 
            //if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colMDoseStr")
            //{
            //    Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedOutwardDrugClinicDept);
            //}
            //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colADoseStr")
            //{
            //    Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedOutwardDrugClinicDept);
            //}
            //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colEDoseStr")
            //{
            //    Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedOutwardDrugClinicDept);
            //}
            //else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colNDoseStr")
            //{
            //    Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedOutwardDrugClinicDept);
            //}
            //▲======= #007

            /*==== #001 ====*/
            if (SelectedOutwardDrugClinicDept.OutQuantity > 1 && SelectedOutwardDrugClinicDept.MaxQtyHIAllowItem > 1)
            {
                SelectedOutwardDrugClinicDept.OutQuantity = 1;
            }
            /*==== #001 ====*/
            if (e.Column.Equals(grdPrescription.GetColumnByName("colQty")))
            {
                EditedQtyRow = e.Row;
            }
        }
        private DataGridRow EditedQtyRow;
        public void grdPrescription_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedQtyRow != null && EditedQtyRow.DataContext != null
                && EditedQtyRow.DataContext is OutwardDrugClinicDept
                && Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru)
            {
                //▼====: #019
                if (Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan != ""
                    && Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan.Contains((EditedQtyRow.DataContext as OutwardDrugClinicDept).ChargeableItemCode))
                {
                    (EditedQtyRow.DataContext as OutwardDrugClinicDept).OutQuantity = (EditedQtyRow.DataContext as OutwardDrugClinicDept).OutQuantity;
                }
                else
                {
                    (EditedQtyRow.DataContext as OutwardDrugClinicDept).OutQuantity = Math.Ceiling((EditedQtyRow.DataContext as OutwardDrugClinicDept).OutQuantity);
                }
                //▲====: #019
            }
            EditedQtyRow = null;
        }
        private bool CheckValid()
        {
            bool result = true;
            if (SelectedOutInvoice != null)
            {
                if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
                {
                    return false;
                }
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
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

        private void GetOutwardDrugClinicDeptInvoice(long OutwardID)
        {
            ClearData();
            //isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutwardDrugClinicDeptInvoice(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndGetOutwardDrugClinicDeptInvoice(asyncResult);
                            if (result != null)
                            {
                                SelectedOutInvoice = result;
                                LoadOutwardClinicDeptDetails();
                                //▼===== #013 Ngăn lại không load danh sách phiếu lĩnh và danh sách y lệnh vì bệnh nhân ngoại trú sử dụng đồ nội trú không có 2 danh sách này.
                                if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                                {
                                    GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0), (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                                    LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
                                }
                                //▲===== #013
                                //▼=====#004
                                if (tmpAdmissionDate.Year > 2000)   // Kiem tra de bao dam tmpAdmissionDate da duoc gan o tren
                                {
                                    SelectedOutInvoice.PatientRegistration.AdmissionDate = tmpAdmissionDate;
                                }
                                //▲=====#004
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingGetID = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRequestDrugInwardClinicDept_ByRegistrationID(long PtRegistrationID, long V_MedProductType, long StoreID, long? outiID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRequestDrugInwardClinicDept_ByRegistrationID(PtRegistrationID, V_MedProductType, StoreID, outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var result = contract.EndGetRequestDrugInwardClinicDept_ByRegistrationID(asyncResult);
                            RequestDrugInwardClinicDeptLst = result.ToObservableCollection();
                            if (outiID.GetValueOrDefault(0) > 0)
                            {
                                if (RequestDrugInwardClinicDeptLst != null && RequestDrugInwardClinicDeptLst.Count > 0)
                                {
                                    RequestDrugInwardClinicDeptListSelected = RequestDrugInwardClinicDeptLst.Where(x => x.Checked == true).ToObservableCollection();

                                    //KMx: Những phiếu yêu cầu được chọn (20/07/2014 12:00).
                                    SelectedOutInvoice.RequestDrugInwardClinicDepts = RequestDrugInwardClinicDeptListSelected;
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

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            OutwardDrugClinicDeptInvoice_Search(0, 20);
        }

        private void OutwardDrugClinicDeptInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new MedDeptInvoiceSearchCriteria();
            }
            //SearchCriteria.StoreID =;
            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
            SearchCriteria.V_MedProductType = (long)V_MedProductType;

            if (SelectedOutInvoice != null && SelectedOutInvoice.StoreID.HasValue && SelectedOutInvoice.StoreID > 0)
            {
                SearchCriteria.StoreID = SelectedOutInvoice.StoreID.Value;
            }


            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutwardDrugClinicDeptInvoice_SearchByType(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    this.HideBusyIndicator();
                                    Action<IXuatNoiBoSearchClinicDept> onInitDlg = delegate (IXuatNoiBoSearchClinicDept proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardClinicDeptInvoiceList.Clear();
                                        proAlloc.OutwardClinicDeptInvoiceList.TotalItemCount = TotalCount;
                                        proAlloc.OutwardClinicDeptInvoiceList.PageIndex = 0;
                                        proAlloc.OutwardClinicDeptInvoiceList.PageSize = 20;
                                        foreach (OutwardDrugClinicDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardClinicDeptInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IXuatNoiBoSearchClinicDept>(onInitDlg);
                                }
                                else
                                {
                                    this.HideBusyIndicator();
                                    SelectedOutInvoice = results.FirstOrDefault();

                                    if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault(0) > 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0939_G1_Msg_InfoKhDcSua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }

                                    LoadOutwardClinicDeptDetails();
                                    // TxD 08/04/2015: Added the following to Load Request List when searching for old outward invoice only find 1 invoice ie. invoice number was entered.
                                    GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0), (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                                    LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
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

        private void LoadOutwardClinicDeptDetails()
        {
            ////lay thong tin benh nhan tu PtRegistrationID
            //if (SelectedOutInvoice != null && SelectedOutInvoice.PtRegistrationID > 0)
            //{
            //    GetPatientRegistrationByPtRegistrationID(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault());
            //}
            //else
            //{
            //load detail
            OutwardDrugClinicDeptDetails_Load(SelectedOutInvoice.outiID);
            //}
        }

        private void OutwardDrugClinicDeptDetails_Load(long outiID)
        {
            //isLoadingDetail = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugClinicDeptDetailByInvoice(outiID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugClinicDeptDetailByInvoice(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            SelectedOutInvoice.OutwardDrugClinicDepts = results.ToObservableCollection();
                            //SumTotalPrice();

                            SetCheckAllOutwardDetail();

                            CalcInvoicePrice();
                            DeepCopyOutwardDrugMedDept();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingDetail = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

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
                OutwardDrugClinicDeptInvoice_Search(0, 20);
            }
        }


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

        ////Tổng giá trị phiếu đã trừ tiền BH và tiền BN (nếu như chọn không tính cho BH hoặc không tính BN)
        //private decimal _TotalInvoiceEdited;
        //public decimal TotalInvoiceEdited
        //{
        //    get
        //    {
        //        return _TotalInvoiceEdited;
        //    }
        //    set
        //    {
        //        if (_TotalInvoiceEdited != value)
        //        {
        //            _TotalInvoiceEdited = value;
        //            NotifyOfPropertyChange(() => TotalInvoiceEdited);
        //        }
        //    }
        //}

        //private void SumTotalPrice()
        //{
        //    //Tổng giá trị phiếu thực sự.
        //    TotalInvoicePrice = 0;
        //    //Tổng giá trị phiếu đã trừ tiền BH và tiền BN (nếu như chọn không tính cho BH hoặc không tính BN).
        //    TotalInvoiceEdited = 0;
        //    TotalHIPayment = 0;
        //    TotalPatientPayment = 0;

        //    if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null)
        //    {
        //        return;
        //    }
        //    //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
        //    bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

        //    if (SelectedOutInvoice.PatientRegistration != null && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit > 0)
        //    {
        //        for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
        //        {
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity * (decimal)SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault();

        //            if (SelectedOutInvoice.OutwardDrugClinicDepts[i].IsCountPatient)
        //            {
        //                SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault() - SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment;
        //            }
        //            else
        //            {
        //                SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment = 0;
        //            }


        //            TotalInvoicePrice += SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault();
        //            TotalInvoiceEdited += SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment + SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment;
        //            TotalHIPayment += SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment;


        //            //dung khi luu du lieu
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].Qty = SelectedOutInvoice.OutwardDrugClinicDepts[i].RequestQty;
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].HIBenefit = SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault();
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].InvoicePrice = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutPrice;
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity * (decimal)(1 - SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault(0));
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
        //        {
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment = 0;

        //            if (SelectedOutInvoice.OutwardDrugClinicDepts[i].IsCountPatient)
        //            {
        //                SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault();
        //            }
        //            else
        //            {
        //                SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment = 0;
        //            }

        //            TotalInvoicePrice += SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault();
        //            TotalInvoiceEdited += SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment;

        //            //dung khi luu du lieu
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].Qty = SelectedOutInvoice.OutwardDrugClinicDepts[i].RequestQty;
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].InvoicePrice = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutPrice;
        //        }
        //    }

        //    if (onlyRoundResultForOutward)
        //    {
        //        TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, MidpointRounding.AwayFromZero);
        //        TotalInvoiceEdited = MathExt.Round(TotalInvoiceEdited, MidpointRounding.AwayFromZero);
        //        TotalHIPayment = MathExt.Round(TotalHIPayment, MidpointRounding.AwayFromZero);
        //    }
        //    //KMx: Vì lý do bên kho nội trú có trường hợp Không tính BH hoặc Không tính BN nên không thể lấy "tổng tiền thực sự của phiếu" - "tổng BH trả".
        //    //Phải lấy "tổng tiền của phiếu đã trừ BH và BN" - "tổng BH trả" (09/12/2014 14:05). 
        //    //TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
        //    TotalPatientPayment = TotalInvoiceEdited - TotalHIPayment;
        //}

        //private void SumTotalPrice()
        //{
        //    TotalInvoicePrice = 0;
        //    TotalHIPayment = 0;
        //    TotalPatientPayment = 0;
        //    if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.PatientRegistration == null)
        //    {
        //        return;
        //    }
        //    //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
        //    bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

        //    double HIBenefit = SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault(0);

        //    for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
        //    {
        //        if (SelectedOutInvoice.OutwardDrugClinicDepts[i].HIAllowedPrice.GetValueOrDefault() > 0)
        //        {
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].HIBenefit = HIBenefit;
        //        }

        //        SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity * (decimal)HIBenefit;

        //        if (!SelectedOutInvoice.OutwardDrugClinicDepts[i].IsCountPatient)
        //        {
        //            SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount = SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment;
        //        }

        //        SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault() - SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment;

        //        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugClinicDepts[i].OutAmount.GetValueOrDefault();

        //        TotalHIPayment += SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalHIPayment;

        //        //dung khi luu du lieu
        //        SelectedOutInvoice.OutwardDrugClinicDepts[i].Qty = SelectedOutInvoice.OutwardDrugClinicDepts[i].RequestQty;
        //        SelectedOutInvoice.OutwardDrugClinicDepts[i].InvoicePrice = SelectedOutInvoice.OutwardDrugClinicDepts[i].OutPrice;
        //        SelectedOutInvoice.OutwardDrugClinicDepts[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugClinicDepts[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity * (decimal)(1 - HIBenefit);
        //    }

        //    if (onlyRoundResultForOutward)
        //    {
        //        TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, MidpointRounding.AwayFromZero);
        //        TotalHIPayment = MathExt.Round(TotalHIPayment, MidpointRounding.AwayFromZero);
        //    }

        //    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
        //}


        //KMx: Tách chức năng tính tiền trên từng dòng và tính tổng phiếu ra làm 2 hàm, vì khi load lại phiếu cũ, không được tính lại từng dòng, lưu sao thì hiển thị như vậy, để có sai thì còn biết được (2015/02/14 15:57).
        //private void SumTotalPrice()
        //{
        //    TotalInvoicePrice = 0;
        //    TotalHIPayment = 0;
        //    TotalPatientPayment = 0;
        //    if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.PatientRegistration == null)
        //    {
        //        return;
        //    }
        //    //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
        //    bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

        //    double HIBenefit = SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault(0);

        //    foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
        //    {
        //        if (item.OutID > 0)
        //        {
        //            item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPriceNew : item.ChargeableItem.NormalPriceNew;
        //            item.HIAllowedPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIAllowedPriceNew : 0;
        //        }
        //        else
        //        {
        //            item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice;
        //            item.HIAllowedPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIAllowedPrice : 0;
        //        }

        //        if (item.HIAllowedPrice.GetValueOrDefault() > 0)
        //        {
        //            item.HIBenefit = HIBenefit;
        //        }

        //        item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)HIBenefit;

        //        if (!item.IsCountPatient)
        //        {
        //            item.OutAmount = item.TotalHIPayment;
        //        }

        //        item.TotalPatientPayment = item.OutAmount.GetValueOrDefault() - item.TotalHIPayment;

        //        TotalInvoicePrice += item.OutAmount.GetValueOrDefault();

        //        TotalHIPayment += item.TotalHIPayment;

        //        //dung khi luu du lieu
        //        item.Qty = item.RequestQty;
        //        item.InvoicePrice = item.OutPrice;
        //        item.TotalInvoicePrice = item.OutAmount.GetValueOrDefault(0);
        //        item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)(1 - HIBenefit);
        //    }

        //    if (onlyRoundResultForOutward)
        //    {
        //        TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, MidpointRounding.AwayFromZero);
        //        TotalHIPayment = MathExt.Round(TotalHIPayment, MidpointRounding.AwayFromZero);
        //    }

        //    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
        //}


        private void SumTotalPrice()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.PatientRegistration == null)
            {
                return;
            }
            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            double HIBenefit = SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault(0);

            foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                if (item.OutID > 0)
                {
                    item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPriceNew : item.ChargeableItem.NormalPriceNew;
                    //20200201 TBL: Do không biết khi nào thì dùng PriceNew nên kiểm tra nếu OutPrice = 0 thì lấy Price
                    if (item.OutPrice == 0)
                    {
                        item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice;
                    }
                    /*▼====: #006*/
                    //item.HIAllowedPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIAllowedPriceNew : 0;
                    /*▲====: #006*/
                }
                else
                {
                    item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice;
                    //20200201 TBL: Do không biết khi nào thì dùng Price nên kiểm tra nếu OutPrice = 0 thì lấy PriceNew
                    if (item.OutPrice == 0)
                    {
                        item.OutPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIPatientPriceNew : item.ChargeableItem.NormalPriceNew;
                    }
                    /*▼====: #006*/
                    //item.HIAllowedPrice = item.IsCountHI && HIBenefit > 0 ? item.ChargeableItem.HIAllowedPriceNew : 0;
                    /*▲====: #006*/
                }

                if (item.HIAllowedPrice.GetValueOrDefault() > 0)
                {
                    item.HIBenefit = HIBenefit;
                }
                /*▼====: #006*/
                //item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)HIBenefit;
                if (item.IsCountHI)
                {
                    item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)HIBenefit;
                }
                else
                {
                    item.TotalHIPayment = 0;
                }
                /*▲====: #006*/
                if (!item.IsCountPatient)
                {
                    item.OutAmount = item.TotalHIPayment;
                }

                item.TotalPatientPayment = item.OutAmount.GetValueOrDefault() - item.TotalHIPayment;

                //dung khi luu du lieu
                item.Qty = item.RequestQty;
                item.InvoicePrice = item.OutPrice;
                item.TotalInvoicePrice = item.OutAmount.GetValueOrDefault(0);
                item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault() * item.OutQuantity * (decimal)(1 - HIBenefit);
            }

            CalcInvoicePrice();
        }

        private void CalcInvoicePrice()
        {
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }
            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                TotalInvoicePrice += item.OutAmount.GetValueOrDefault();

                TotalHIPayment += item.TotalHIPayment;
            }

            if (onlyRoundResultForOutward)
            {
                TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, Common.Converters.MidpointRounding.AwayFromZero);
                TotalHIPayment = MathExt.Round(TotalHIPayment, Common.Converters.MidpointRounding.AwayFromZero);
            }

            TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
        }

        DataGrid grdPrescription = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;

            if (grdPrescription == null)
            {
                return;
            }

            var col = grdPrescription.GetColumnByName("colMedicalMaterial");
            var colMDoseStr = grdPrescription.GetColumnByName("colMDoseStr");
            var colADoseStr = grdPrescription.GetColumnByName("colADoseStr");
            var colEDoseStr = grdPrescription.GetColumnByName("colEDoseStr");
            var colNDoseStr = grdPrescription.GetColumnByName("colNDoseStr");

            if (col == null || colMDoseStr == null || colADoseStr == null || colEDoseStr == null || colNDoseStr == null)
            {
                return;
            }

            if (V_MedProductType == AllLookupValues.MedProductType.Y_CU || V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
            {
                col.Visibility = Visibility.Visible;
            }
            else
            {
                col.Visibility = Visibility.Collapsed;
            }

            if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
            {
                colMDoseStr.Visibility = Visibility.Visible;
                colADoseStr.Visibility = Visibility.Visible;
                colEDoseStr.Visibility = Visibility.Visible;
                colNDoseStr.Visibility = Visibility.Visible;
            }
            else
            {
                colMDoseStr.Visibility = Visibility.Collapsed;
                colADoseStr.Visibility = Visibility.Collapsed;
                colEDoseStr.Visibility = Visibility.Collapsed;
                colNDoseStr.Visibility = Visibility.Collapsed;
            }

        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        #region printing member

        public void btnPreview()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G0787_G1_Thuoc.ToUpper());

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G2907_G1_YCu.ToUpper());

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z3206_G1_DinhDuong.ToUpper());

                }
                else
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.T1616_G1_HC.ToUpper());

                }
                proAlloc.eItem = ReportName.CLINICDEPT_OUTWARD_PATIENT;
            };
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        //▼====== #011
        public void btnPreviewDetails()
        {
            Action<IClinicDeptReportDocumentPreview> onInitDlg = delegate (IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.outiID;
                if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G0787_G1_Thuoc.ToUpper());

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.G2907_G1_YCu.ToUpper());

                }
                else if (V_MedProductType == AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.Z3206_G1_DinhDuong.ToUpper());

                }
                else
                {
                    proAlloc.LyDo = string.Format(" {0} ", eHCMSResources.T1616_G1_HC.ToUpper());

                }
                proAlloc.eItem = ReportName.REPORT_PHIEU_CONG_KHAI_THUOC;
            };
            //▼====: #018
            GlobalsNAV.ShowDialog(onInitDlg);
            if (Globals.ServerConfigSection.CommonItems.WhichHospitalUseThisApp == 2) // Thanh vũ
            {
                Action<IClinicDeptReportDocumentPreview> onInitDlgPhgieuSaoThuoc = delegate (IClinicDeptReportDocumentPreview proAlloc)
                {
                    proAlloc.ID = SelectedOutInvoice.outiID;
                    proAlloc.eItem = ReportName.XRptPhieuSaoThuoc;
                    proAlloc.V_MedProductType = (long)SelectedOutInvoice.MedProductType;
                };
                GlobalsNAV.ShowDialog(onInitDlgPhgieuSaoThuoc);
            }
            //▲====: #018
        }
        //▲====== #011
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

        #endregion

        #region auto Drug For Prescription member

        private bool _IsFocusTextCode;
        public bool IsFocusTextCode
        {
            get { return _IsFocusTextCode; }
            set
            {
                if (_IsFocusTextCode != value)
                {
                    _IsFocusTextCode = value;
                }
                NotifyOfPropertyChange(() => IsFocusTextCode);
            }
        }

        private string BrandName;
        //private bool? IsHIPatient = false;

        //private void GetHIPatient(PatientRegistration Rgst)
        //{
        //    if (Rgst != null && Rgst.PtInsuranceBenefit > 0)
        //    {
        //        IsHIPatient = true;
        //    }
        //    else
        //    {
        //        IsHIPatient = false;
        //    }
        //}

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

        public void AutoDrug_LostFocus(object sender, RoutedEventArgs e)
        {
            if (au == null || string.IsNullOrEmpty(au.Text))
            {
                return;
            }
            if (IsInputDosage)
            {
                if (tbxMDoseStr == null)
                {
                    return;
                }
                tbxMDoseStr.Focus();
            }
            else
            {
                if (AxQty == null)
                {
                    return;
                }
                AxQty.Focus();
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            //KMx: Không sử dụng Radio Button Code - Tên nữa (07/04/2016 15:05).
            //if (!IsCode.GetValueOrDefault())
            //{
            //    BrandName = e.Parameter;
            //    //tim theo ten
            //    SearchRefGenMedProductDetails(e.Parameter, false);
            //}
            BrandName = e.Parameter;
            SearchRefGenMedProductDetails(e.Parameter, false);
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as RefGenMedProductDetails;
                AutoDrug_LostFocus(null, null);
            }
        }
        private void ListDisplayAutoComplete(bool IsCode)
        {
            var hhh = from hd in RefGenMedProductDetailsList
                      group hd by new { hd.GenMedProductID, hd.BrandName, hd.SelectedUnit.UnitName, hd.RequestQty, hd.Code, hd.DispenseVolume, hd.V_MedicalMaterial, hd.HIPaymentPercent, hd.MaxQtyHIAllowItem, hd.InsuranceCover } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          GenMedProductID = hdgroup.Key.GenMedProductID,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Code = hdgroup.Key.Code,
                          Qty = hdgroup.Key.RequestQty,
                          DispenseVolume = hdgroup.Key.DispenseVolume,
                          V_MedicalMaterial = hdgroup.Key.V_MedicalMaterial,
                          HIPaymentPercent = hdgroup.Key.HIPaymentPercent,
                          MaxQtyHIAllowItem = hdgroup.Key.MaxQtyHIAllowItem,
                          InsuranceCover = hdgroup.Key.InsuranceCover
                      };

            //KMx: Phải new rồi mới add. Nếu clear rồi add thì bị chậm (09/07/2014 17:35).
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
                item.DispenseVolume = i.DispenseVolume;
                item.V_MedicalMaterial = i.V_MedicalMaterial;
                item.HIPaymentPercent = i.HIPaymentPercent;
                item.MaxQtyHIAllowItem = i.MaxQtyHIAllowItem;
                item.InsuranceCover = i.InsuranceCover;
                RefGenMedProductDetailsListSum.Add(item);
            }
            if (IsCode)
            {
                if (RefGenMedProductDetailsListSum != null && RefGenMedProductDetailsListSum.Count > 0)
                {
                    // var item = RefGenMedProductDetailsListSum.Where(x => x.Code == txt);
                    // if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = RefGenMedProductDetailsListSum.ToList()[0];
                        //AxQty.Focus();

                        if (IsInputDosage)
                        {
                            if (tbxMDoseStr == null)
                            {
                                return;
                            }
                            tbxMDoseStr.Focus();
                        }
                        else
                        {
                            if (AxQty == null)
                            {
                                return;
                            }
                            AxQty.Focus();
                        }

                    }
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
                        tbx.Focus();
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

        private void SearchRefGenMedProductDetails(string Name, bool IsCode)
        {
            if (SelectedOutInvoice == null || (SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) <= 0 && SelectedOutInvoice.OutPtRegistrationID == 0))
            {
                MessageBox.Show(eHCMSResources.K0294_G1_ChonBN);
                return;
            }
            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }

            IsFocusTextCode = IsCode;

            //if (IsCode)
            //{
            //    isLoadingDetail = true;
            //}

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, SelectedOutInvoice.ReqDrugInClinicDeptID, IsCode, SelectedOutInvoice.PtRegistrationID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    //KMx: Không sử dụng biến IsHIPatient khi đi tìm thuốc nữa, vì bị lỗi người này gán cho người kia (14/02/2015 14:56).
                    //contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, SelectedOutInvoice.RequestDrugInwardClinicDepts, IsCode, SelectedOutInvoice.PtRegistrationID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(IsCost, Name, SelectedOutInvoice.StoreID.GetValueOrDefault(0), (long)V_MedProductType, null, SelectedOutInvoice.RequestDrugInwardClinicDepts, IsCode, SelectedOutInvoice.PtRegistrationID, null, 0 /*--28/01/2021 DatTB Thêm biến*/, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestClinicDept(asyncResult);
                            RefGenMedProductDetailsList = new ObservableCollection<RefGenMedProductDetails>();
                            RefGenMedProductDetailsTemp = new ObservableCollection<RefGenMedProductDetails>();

                            RefGenMedProductDetailsTemp = results.ToObservableCollection();

                            if (OutwardDrugClinicDeptsCopy != null && OutwardDrugClinicDeptsCopy.Count > 0)
                            {
                                foreach (OutwardDrugClinicDept d in OutwardDrugClinicDeptsCopy)
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
                                    //KMx: Nếu hàng trong grid không có trong AutoComplete thì thôi, bỏ vào AutoComplete cũng không hiển thị được (09/07/2014 11:56).
                                    //else
                                    //{
                                    //    RefGenMedProductDetails p = d.GenMedProductItem;
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
                                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                                {
                                    foreach (OutwardDrugClinicDept d in SelectedOutInvoice.OutwardDrugClinicDepts)
                                    {
                                        if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                                        {
                                            s.Remaining = s.Remaining - d.OutQuantity;
                                        }
                                    }
                                }
                                RefGenMedProductDetailsList.Add(s);
                            }
                            ListDisplayAutoComplete(IsCode);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //if (IsCode)
                            //{
                            //isLoadingDetail = false;
                            //if (AxQty != null)
                            //{
                            //    AxQty.Focus();
                            //}
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


        private void CheckBatchNumberExists(OutwardDrugClinicDept p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                //KMx: Khi thêm thuốc, nếu đã có trong grid thì vẫn add ra 1 dòng riêng, không gộp chung vào dòng đã có rồi (Khoa Dược yêu cầu) (21/12/2014 11:06).
                //foreach (OutwardDrugClinicDept p1 in SelectedOutInvoice.OutwardDrugClinicDepts)
                //{
                //    if (p.GenMedProductID == p1.GenMedProductID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                //    {
                //        p1.OutQuantity += p.OutQuantity;
                //        // p1.IsLoad = 0;
                //        p1.RequestQty += p.RequestQty;
                //        p1.Qty = p1.OutQuantity;
                //        kq = true;
                //        break;
                //    }
                //}
                if (!kq)
                {
                    if (p.InwardDrugClinicDept == null)
                    {
                        p.InwardDrugClinicDept = new InwardDrugClinicDept();
                        p.InwardDrugClinicDept.InID = p.InID.GetValueOrDefault();
                        p.InwardDrugClinicDept.GenMedProductID = p.GenMedProductID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrugClinicDept.NormalPrice = p.OutPrice;
                    p.InwardDrugClinicDept.HIPatientPrice = p.OutPrice;
                    p.InwardDrugClinicDept.HIAllowedPrice = p.HIAllowedPrice;
                    p.Qty = p.OutQuantity;
                    /*==== #001 ====*/
                    if (p.MaxQtyHIAllowItem > 1 && p.OutQuantity > 1)
                    {
                        var OutQty = p.OutQuantity;
                        p.Qty = 1;
                        p.OutQuantity = 1;
                        for (int i = 0; i < OutQty; i++)
                        {
                            SelectedOutInvoice.OutwardDrugClinicDepts.Add(p.DeepCopy());
                        }
                    }
                    else
                        /*==== #001 ====*/
                        SelectedOutInvoice.OutwardDrugClinicDepts.Add(p);
                }
                txt = "";
                SelectedSellVisitor = null;

                //KMx: Không sử dụng Radio Button Code - Tên nữa (04/02/2016 11:52).
                //if (IsCode.GetValueOrDefault())
                if (IsFocusTextCode)
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


        //private void ChooseBatchNumber(RefGenMedProductDetails value)
        //{
        //    if (SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null)
        //    {
        //        return;
        //    }
        //    var items = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID).OrderBy(p => p.STT);
        //    foreach (RefGenMedProductDetails item in items)
        //    {
        //        OutwardDrugClinicDept p = new OutwardDrugClinicDept();
        //        if (item.Remaining > 0)
        //        {
        //            p.GenMedProductItem = item;
        //            p.GenMedProductID = item.GenMedProductID;
        //            p.InBatchNumber = item.InBatchNumber;
        //            p.InID = item.InID;
        //            p.OutPrice = item.OutPrice;

        //            p.IsCountPatient = true;
        //            if (item.InsuranceCover.GetValueOrDefault() && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0)
        //            {
        //                p.IsCountHI = true;
        //                p.HIAllowedPrice = item.HIAllowedPrice;
        //            }
        //            else
        //            {
        //                p.HIAllowedPrice = 0;
        //            }

        //            p.InwardDate = item.InwardDate;
        //            p.InExpiryDate = item.InExpiryDate;
        //            p.SdlDescription = item.SdlDescription;
        //            p.OutClinicDeptReqID = item.OutClinicDeptReqID;

        //            p.V_MedicalMaterial = item.V_MedicalMaterial;
        //            if (p.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE)
        //            {
        //                p.IsReplaceMedMat = true;
        //            }
        //            else if (p.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO)
        //            {
        //                p.IsDisposeMedMat = true;
        //            }

        //            if (p.HIAllowedPrice > 0)
        //            {
        //                p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
        //            }

        //            if (item.Remaining - value.RequiredNumber < 0)
        //            {
        //                if (value.RequestQty > item.Remaining)
        //                {
        //                    p.RequestQty = item.Remaining;
        //                    value.RequestQty = value.RequestQty - item.Remaining;
        //                }
        //                else
        //                {
        //                    p.RequestQty = value.RequestQty;
        //                    value.RequestQty = 0;
        //                }
        //                value.RequiredNumber = value.RequiredNumber - item.Remaining;
        //                p.OutQuantity = item.Remaining;

        //                CheckBatchNumberExists(p);
        //                item.Remaining = 0;
        //            }
        //            else
        //            {
        //                p.RequestQty = value.RequestQty;
        //                value.RequestQty = 0;
        //                p.OutQuantity = value.RequiredNumber;

        //                CheckBatchNumberExists(p);
        //                item.Remaining = item.Remaining - value.RequiredNumber;
        //                break;
        //            }
        //        }
        //    }
        //    SumTotalPrice();
        //}

        private void ConvertInwardToOutward(ObservableCollection<RefGenMedProductDetails> inwardList, RefGenMedProductDetails value)
        {
            if (inwardList == null || inwardList.Count <= 0)
            {
                return;
            }

            foreach (RefGenMedProductDetails item in inwardList)
            {
                if (item.Remaining <= 0)
                {
                    continue;
                }
                OutwardDrugClinicDept p = new OutwardDrugClinicDept();

                p.GenMedProductItem = item;
                p.GenMedProductID = item.GenMedProductID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.OutPrice = item.OutPrice;

                p.IsCountPatient = true;
                if (item.InsuranceCover.GetValueOrDefault() && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0 && item.HIAllowedPrice.GetValueOrDefault(0) > 0)
                {
                    p.IsCountHI = true;
                    //p.HIAllowedPrice = item.HIAllowedPrice;
                }
                //▼===== #017: Nguyên nhân là không có giá bảo hiểm nên khi tính lại bill không biết món này có bảo hiểm hay không để gán thông tin bảo hiểm cho bệnh nhân.
                //else
                //{
                //    p.HIAllowedPrice = 0;
                //}
                p.HIAllowedPrice = item.HIAllowedPrice;
                //▲===== #017
                p.InwardDate = item.InwardDate;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.OutClinicDeptReqID = item.OutClinicDeptReqID;

                p.V_MedicalMaterial = item.V_MedicalMaterial;
                if (p.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE)
                {
                    p.IsReplaceMedMat = true;
                }
                else if (p.V_MedicalMaterial == (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO)
                {
                    p.IsDisposeMedMat = true;
                }

                p.HIPaymentPercent = item.HIPaymentPercent;

                if (p.HIAllowedPrice > 0)
                {
                    p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
                }

                p.MDoseStr = value.MDoseStr;
                p.ADoseStr = value.ADoseStr;
                p.EDoseStr = value.EDoseStr;
                p.NDoseStr = value.NDoseStr;

                p.MDose = value.MDose;
                p.ADose = value.ADose;
                p.EDose = value.EDose;
                p.NDose = value.NDose;

                p.DoctorStaff = new Staff();
                p.DoctorStaff.StaffID = aucHoldConsultDoctor.StaffID;
                p.DoctorStaff.FullName = aucHoldConsultDoctor.StaffName;
                p.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                p.Administration = value.Administration;
                p.VAT = item.VAT;
                if (item.Remaining - value.RequiredNumber < 0)
                {
                    decimal mRemaining = item.Remaining;
                    if ((Globals.ServerConfigSection.ClinicDeptElements.RoundDownInwardOutQty || Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru)
                        && (value.RequiredNumber % 1 == 0 || value.RequiredNumber % 1 > item.Remaining % 1))
                    {
                        mRemaining = Math.Floor(mRemaining);
                    }
                    if (value.RequestQty > item.Remaining)
                    {
                        p.RequestQty = mRemaining;
                        value.RequestQty = value.RequestQty - mRemaining;
                    }
                    else
                    {
                        p.RequestQty = value.RequestQty;
                        value.RequestQty = 0;
                    }
                    if (mRemaining == 0)
                    {
                        continue;
                    }
                    value.RequiredNumber = value.RequiredNumber - mRemaining;
                    p.OutQuantity = mRemaining;
                    CheckBatchNumberExists(p);
                    item.Remaining = 0;
                }
                else
                {
                    p.RequestQty = value.RequestQty;
                    value.RequestQty = 0;

                    p.OutQuantity = value.RequiredNumber;

                    CheckBatchNumberExists(p);
                    item.Remaining = item.Remaining - value.RequiredNumber;
                    value.RequiredNumber = 0;
                    break;
                }

            }

        }


        private void ChooseBatchNumber(RefGenMedProductDetails value)
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null)
            {
                return;
            }

            //KMx: Tách những inward thành 2 List, đầu tiền lấy những inward có ngày nhập trước ngày xuất, nếu không đủ thì lấy những inward sau ngày xuất, những inward sau ngày xuất chỉ được xem, chứ không lưu được (25/03/2015 17:41).
            ObservableCollection<RefGenMedProductDetails> InwardBeforeOutDate = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID && x.InwardDate.Date <= SelectedOutInvoice.OutDate.GetValueOrDefault().Date).OrderBy(p => p.STT).ToObservableCollection();
            ObservableCollection<RefGenMedProductDetails> InwardAfterOutDate = RefGenMedProductDetailsList.Where(x => x.GenMedProductID == value.GenMedProductID && x.InwardDate.Date > SelectedOutInvoice.OutDate.GetValueOrDefault().Date).OrderBy(p => p.STT).ToObservableCollection();

            ConvertInwardToOutward(InwardBeforeOutDate, value);

            if (value.RequiredNumber > 0)
            {
                ConvertInwardToOutward(InwardAfterOutDate, value);
            }

            SumTotalPrice();
        }



        private void AddListOutwardDrugMedDept(RefGenMedProductDetails value)
        {
            if (value != null)
            {
                if (value.RequiredNumber > 0)
                {
                    //int intOutput = 0;
                    //if (Int32.TryParse(value.RequiredNumber.ToString(), out intOutput))
                    //{
                    //    int a = Convert.ToInt32(value.RequiredNumber);
                    if (CheckValidDrugAuto(value))
                    {
                        //▼====: #002
                        //if (CalByUnitUse)
                        if (CalByUnitUse && !IsInputDosage)
                        //▲====: #002
                        {
                            //KMx: Tính ra số lượng dựa vào DispenseVolume, làm tròn lên 3 số, không làm tròn 2 số. Khoa Dược yêu cầu (15/11/2014 09:01).
                            value.RequiredNumber = value.RequiredNumber / (value.DispenseVolume == 0 ? 1 : (decimal)value.DispenseVolume);
                            value.RequiredNumber = Math.Round(value.RequiredNumber, 3);
                        }
                        if (Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru)
                        {
                            value.RequiredNumber = Math.Ceiling(value.RequiredNumber);
                            value.RequestQty = Math.Ceiling(value.RequestQty);
                        }
                        ChooseBatchNumber(value);
                    }
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Số lượng phải là số nguyên lớn hơn 0");
                    //}
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
                if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
                {
                    SelectedOutInvoice.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>();
                }
                //var results1 = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.GenMedProductID == SelectedSellVisitor.GenMedProductID);
                ObservableCollection<OutwardDrugClinicDept> results1 = new ObservableCollection<OutwardDrugClinicDept>();
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    if (item.GenMedProductID == SelectedSellVisitor.GenMedProductID)
                    {
                        results1.Add(item);
                    }
                }
                if (results1 != null && results1.Count() > 0)
                {
                    foreach (OutwardDrugClinicDept p in results1)
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

        private bool CheckBeforeAddItem()
        {
            if (aucHoldConsultDoctor == null || aucHoldConsultDoctor.StaffID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (MedicalInstructionDateContent.DateTime == null)
            {
                MessageBox.Show(eHCMSResources.A0594_G1_Msg_InfoChonNgYLenh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (!CheckValidMedicalInstructionDate())
            {
                return false;
            }

            return true;
        }
        //▼====: #003
        private bool CheckValidMedicalInstructionDate(DateTime? aMedicalInstructionDate = null)
        {
            if (aMedicalInstructionDate == null && MedicalInstructionDateContent.DateTime != null)
            {
                MedicalInstructionDateContent.DateTime = Globals.ApplyValidMedicalInstructionDate(MedicalInstructionDateContent.DateTime.GetValueOrDefault(), SelectedOutInvoice.PatientRegistration);
                aMedicalInstructionDate = MedicalInstructionDateContent.DateTime;
            }
            if (!RequireDoctorAndDate || aMedicalInstructionDate == null || !aMedicalInstructionDate.HasValue
                || SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null)
                return true;
            if (SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate.HasValue
                && SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate != null
                && aMedicalInstructionDate > SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate)
            {
                MessageBox.Show(eHCMSResources.Z2188_G1_NYLKhongLonHonNXV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if ((!SelectedOutInvoice.PatientRegistration.AdmissionDate.HasValue || SelectedOutInvoice.PatientRegistration.AdmissionDate == null)
                && SelectedOutInvoice.PatientRegistration.ExamDate != null
                && aMedicalInstructionDate < SelectedOutInvoice.PatientRegistration.ExamDate)
            {
                MessageBox.Show(eHCMSResources.Z2187_G1_NYLKhongNhoHonNDK, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if (SelectedOutInvoice.PatientRegistration.AdmissionDate.HasValue
                && SelectedOutInvoice.PatientRegistration.AdmissionDate != null
                && aMedicalInstructionDate < SelectedOutInvoice.PatientRegistration.AdmissionDate)
            {
                MessageBox.Show(eHCMSResources.Z2183_G1_NgayYLKhongNhoHonNNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }
        //▲====: #003
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        public void AddItem_Click(object sender, object e)
        {
            AddItem();
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
        public void AddItem()
        {
            if (SelectedOutInvoice.PatientRegistration != null && Globals.IsLockRegistration(SelectedOutInvoice.PatientRegistration.RegLockFlag, "xuất thuốc"))
            {
                return;
            }
            if (SelectedOutInvoice.OutDate == null)
            {
                MessageBox.Show(eHCMSResources.A0593_G1_Msg_InfoChonNgXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (RequireDoctorAndDate && !CheckBeforeAddItem())
            {
                return;
            }

            if (this.DoseVisibility && IsInputDosage && SelectedSellVisitor != null && SelectedSellVisitor.InsuranceCover.GetValueOrDefault())
            {
                SelectedSellVisitor.GetDoesFromDoseString();
            }
            if (this.DoseVisibility && IsInputDosage && SelectedSellVisitor != null && SelectedSellVisitor.InsuranceCover.GetValueOrDefault() && (SelectedSellVisitor.MDose + SelectedSellVisitor.ADose + SelectedSellVisitor.EDose + SelectedSellVisitor.NDose) <= 0.0)
            {
                MessageBox.Show(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            /*▼====: #005*/
            if (aucHoldConsultDoctor.CertificateNumber == null)
            {
                MessageBox.Show(eHCMSResources.Z2295_G1_BSKoCoCCHN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            /*▲====: #005*/
            else if (this.DoseVisibility && !IsInputDosage && SelectedSellVisitor != null && SelectedSellVisitor.InsuranceCover.GetValueOrDefault() && string.IsNullOrEmpty(SelectedSellVisitor.Administration))
            {
                MessageBox.Show(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //20200422 TBL: BM 0033146: Fix lỗi khi load từ y lệnh để xuất cho BN nhưng lại thêm đúng thuốc đó thì số lượng duyệt bị âm. Không hiểu hàm này để thực hiện mục đích gì.
            //ReCountQtyRequest();
            if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>();
            }
            AddListOutwardDrugMedDept(SelectedSellVisitor);

            SetCheckAllOutwardDetail();
        }

        #region Properties member
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListTemp;
        private ObservableCollection<RefGenMedProductDetails> BatchNumberListShow;
        private ObservableCollection<OutwardDrugClinicDept> OutwardDrugMedDeptListByGenMedProductID;
        private ObservableCollection<OutwardDrugClinicDept> _OutwardDrugMedDeptListByGenMedProductIDFirst;
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugMedDeptListByGenMedProductIDFirst
        {
            get { return _OutwardDrugMedDeptListByGenMedProductIDFirst; }
            set
            {
                _OutwardDrugMedDeptListByGenMedProductIDFirst = value;
                NotifyOfPropertyChange(() => OutwardDrugMedDeptListByGenMedProductIDFirst);
            }
        }

        private OutwardDrugClinicDept _SelectedOutwardDrugClinicDept;
        public OutwardDrugClinicDept SelectedOutwardDrugClinicDept
        {
            get { return _SelectedOutwardDrugClinicDept; }
            set
            {
                if (_SelectedOutwardDrugClinicDept != value)
                    _SelectedOutwardDrugClinicDept = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrugClinicDept);
            }
        }
        #endregion

        private void RefGenMedProductDetailsBatchNumber(long GenMedProductID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //KMx: Không sử dụng biến IsHIPatient khi đi tìm thuốc nữa, vì bị lỗi người này gán cho người kia (14/02/2015 14:56).
                    //contract.BeginspGetInBatchNumberAllClinicDept_ByGenMedProductID(GenMedProductID, (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginspGetInBatchNumberAllClinicDept_ByGenMedProductID(GenMedProductID, (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAllClinicDept_ByGenMedProductID(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            //▼===== #015:  Nếu thuốc có 2 lô mà 1 lô đã sử dụng hết tồn thì End chỉ trả về được 1 kết quả (Do Remaining đã hết). Gặp điều kiện chỉ lấy lớn hơn 1
                            //              dẫn đến không load được để chọn đổi lô. Chuyển điều kiện về > 0. Để chỉ có 1 lô cũng được đổi.
                            //if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 1)
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
                            }
                            //▲===== #015
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                OutwardDrugMedDeptListByGenMedProductID = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                if (OutwardDrugClinicDeptsCopy != null)
                {
                    OutwardDrugMedDeptListByGenMedProductIDFirst = OutwardDrugClinicDeptsCopy.Where(x => x.GenMedProductID == GenMedProductID).ToObservableCollection();
                }
                RefGenMedProductDetailsBatchNumber(GenMedProductID);
            }
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugMedDeptListByGenMedProductIDFirst != null && OutwardDrugMedDeptListByGenMedProductIDFirst.Count > 0)
            {
                foreach (OutwardDrugClinicDept d in OutwardDrugMedDeptListByGenMedProductIDFirst)
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
                        RefGenMedProductDetails p = d.GenMedProductItem;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.OutPrice = d.OutPrice;
                        //▼===== #016
                        p.NormalPrice = d.GenMedProductItem.NormalPrice > 0 ? d.GenMedProductItem.NormalPrice : d.GenMedProductItem.NormalPriceNew;
                        p.HIPatientPrice = d.GenMedProductItem.HIPatientPrice > 0 ? d.GenMedProductItem.HIPatientPrice : d.GenMedProductItem.HIPatientPriceNew;
                        p.Visa = d.GenMedProductItem.Visa;
                        p.VAT = d.VAT;
                        //▲===== #016
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
                    foreach (OutwardDrugClinicDept d in OutwardDrugMedDeptListByGenMedProductID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.GenMedProductID == s.GenMedProductID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrugClinicDept.InID)
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
                //it bua moi len lam
                Action<IChooseBatchNumberClinicDept> onInitDlg = delegate (IChooseBatchNumberClinicDept proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrugClinicDept.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugMedDeptListByGenMedProductID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugMedDeptListByGenMedProductID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberClinicDept>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao);
            }
        }

        #endregion

        //private void DeleteInvoiceDrugInObject()
        //{
        //    OutwardDrugClinicDept p = SelectedOutwardDrugClinicDept.DeepCopy();

        //    //KMx: Thêm ngày 24/01/2015 10:01.
        //    if (SelectedOutInvoice.OutwardDrugClinicDepts_Delete == null)
        //    {
        //        SelectedOutInvoice.OutwardDrugClinicDepts_Delete = new ObservableCollection<OutwardDrugClinicDept>();
        //    }

        //    if (SelectedOutwardDrugClinicDept.OutID > 0)
        //    {
        //        SelectedOutInvoice.OutwardDrugClinicDepts_Delete.Add(SelectedOutwardDrugClinicDept);
        //    }

        //    SelectedOutInvoice.OutwardDrugClinicDepts.Remove(SelectedOutwardDrugClinicDept);
        //    foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
        //    {
        //        if (item.GenMedProductID == p.GenMedProductID)
        //        {
        //            item.RequestQty = item.RequestQty + p.RequestQty;
        //            break;
        //        }
        //    }
        //    SelectedOutInvoice.OutwardDrugClinicDepts = SelectedOutInvoice.OutwardDrugClinicDepts.ToObservableCollection();
        //    SumTotalPrice();
        //}



        private void DeleteOutwardItem(OutwardDrugClinicDept outwardDelete)
        {
            if (outwardDelete == null)
            {
                return;
            }

            if (SelectedOutInvoice.OutwardDrugClinicDepts_Delete == null)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts_Delete = new ObservableCollection<OutwardDrugClinicDept>();
            }

            if (outwardDelete.OutID > 0)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts_Delete.Add(outwardDelete);
            }

            SelectedOutInvoice.OutwardDrugClinicDepts.Remove(outwardDelete);

            //20200422 TBL: BM 0033146: Fix lỗi khi load từ y lệnh để xuất cho BN nhưng lại thêm đúng thuốc đó thì số lượng duyệt bị âm, sau đó xóa dòng đó đi thì số lượng duyệt của dòng xuất từ y lệnh = 0.
            //OutwardDrugClinicDept item = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.GenMedProductID == outwardDelete.GenMedProductID).FirstOrDefault();
            //if (item != null)
            //{
            //    item.RequestQty = item.RequestQty + outwardDelete.RequestQty;
            //}
        }

        private void DeleteOutwardList(ObservableCollection<OutwardDrugClinicDept> deleteOutwardDrugList)
        {
            //KMx: Thêm ngày 24/12/2015 14:36.
            if (deleteOutwardDrugList == null || deleteOutwardDrugList.Count <= 0)
            {
                return;
            }

            foreach (OutwardDrugClinicDept outwardDelete in deleteOutwardDrugList)
            {
                DeleteOutwardItem(outwardDelete);
            }

            SumTotalPrice();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutwardDrugClinicDept != null && SelectedOutInvoice.CanSaveAndPaid)
            {
                DeleteOutwardItem(SelectedOutwardDrugClinicDept);
                SumTotalPrice();
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
                ClearData();
                GetAllOutwardTemplate(((RefStorageWarehouseLocation)cbx.SelectedItem).DeptID.GetValueOrDefault());
                if (SelectedOutInvoice != null)
                {
                    SelectedOutInvoice.outiID = 0;
                    if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
                    {
                        SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
                    }
                    if (SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) > 0)
                    {
                        GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0), (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), SelectedOutInvoice.outiID);
                        LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
                    }
                }
            }
        }
        //▼=====#004
        DateTime tmpAdmissionDate;
        //▲=====#004
        private void OutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            //isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            tmpAdmissionDate = new DateTime();
            //▼=====#004
            if (SelectedOutInvoice != null && SelectedOutInvoice.PatientRegistration != null && SelectedOutInvoice.PatientRegistration.AdmissionDate.HasValue)
            {
                tmpAdmissionDate = SelectedOutInvoice.PatientRegistration.AdmissionDate.Value;
            }
            //▲=====#004

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugClinicDeptInvoice_SaveByType(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                GetOutwardDrugClinicDeptInvoice(OutID);
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
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void CheckOutwardDrugClinicDeptModified(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugClinicDepts == null || OutwardInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }

            ObservableCollection<OutwardDrugClinicDept> CheckList = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.UNCHANGED).ToObservableCollection();
            foreach (OutwardDrugClinicDept item in CheckList)
            {
                long doctorStaffID = item.DoctorStaff != null ? item.DoctorStaff.StaffID : 0;
                long doctorStaffID_Orig = item.DoctorStaff_Orig != null ? item.DoctorStaff_Orig.StaffID : 0;

                if (item.IsCountHI != item.IsCountHI_Orig || item.IsCountPatient != item.IsCountPatient_Orig || item.OutQuantity != item.OutQuantity_Orig || item.InID != item.InID_Orig || item.OutNotes != item.OutNotes_Orig || item.V_MedicalMaterial != item.V_MedicalMaterial_Orig
                    || item.MDoseStr != item.MDoseStr_Orig || item.ADoseStr != item.ADoseStr_Orig || item.EDoseStr != item.EDoseStr_Orig || item.NDoseStr != item.NDoseStr_Orig
                    || doctorStaffID != doctorStaffID_Orig || item.MedicalInstructionDate != item.MedicalInstructionDate_Orig || item.Administration != item.Administration_Orig)
                {
                    item.RecordState = RecordState.MODIFIED;
                }
            }
        }

        private void OutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice OutwardInvoice)
        {
            if (OutwardInvoice == null || OutwardInvoice.OutwardDrugClinicDepts == null)
            {
                return;
            }

            CheckOutwardDrugClinicDeptModified(OutwardInvoice);

            this.ShowBusyIndicator();

            OutwardInvoice.OutwardDrugClinicDepts_Add = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.DETACHED).ToObservableCollection();

            OutwardInvoice.OutwardDrugClinicDepts_Update = OutwardInvoice.OutwardDrugClinicDepts.Where(x => x.RecordState == RecordState.MODIFIED).ToObservableCollection();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoice_UpdateByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugClinicDeptInvoice_UpdateByType(out OutID, out StrError, asyncResult);

                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                                GetOutwardDrugClinicDeptInvoice(OutID);
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
                            //isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
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
            string strError = "";
            if (grdPrescription != null)
            {
                if (SelectedOutInvoice != null)
                {
                    SelectedOutInvoice.StaffID = GetStaffLogin().StaffID;
                }

                if (SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) <= 0 && SelectedOutInvoice.OutPtRegistrationID == 0)
                {
                    Globals.ShowMessage(eHCMSResources.K0290_G1_ChonBN, eHCMSResources.G0442_G1_TBao);
                    return false;
                }

                if (SelectedOutInvoice.OutDate == null || SelectedOutInvoice.OutDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
                {
                    MessageBox.Show(eHCMSResources.A0863_G1_Msg_InfoNgXuatKhHopLe4, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }

                //KMx: Nếu tất cả các dòng xuất = 0, sau khi chương trình tự động xóa thì sẽ bị chặn lại ở dưới vì không có dòng nào để lưu. Cho nên không được đem dòng if này vào trong else (SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0) (24/12/2015 17:07).
                if (SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Count > 0)
                {
                    ObservableCollection<OutwardDrugClinicDept> outwardItem = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.OutQuantity <= 0).ToObservableCollection();
                    if (outwardItem != null && outwardItem.Count > 0)
                    {
                        if (MessageBox.Show(eHCMSResources.A0429_G1_Msg_InfoTuDongXoaDongSLgXuat0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            DeleteOutwardList(outwardItem);
                        }
                    }
                }

                if (SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count == 0)
                {
                    // TxD05/04/2015: Only existing outwardDrug invoice can be saved without any items to allow for modification to clean up the invoice
                    if (SelectedOutInvoice.outiID > 0)
                    {
                        if (MessageBox.Show(eHCMSResources.A0641_G1_Msg_InfoOKDeLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0642_G1_Msg_InfoKhCoDataPhXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                }
                else
                {
                    string strNumOfRow = "";
                    string strRowWithoutDoctorStaffID = "";
                    string strRowWithoutMedicalInstructionDate = "";
                    string strRowInvalidMedicalInstructDate = "";
                    //string strInvalidProduct = "";
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugClinicDepts.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugClinicDepts[i].GenMedProductItem != null && SelectedOutInvoice.OutwardDrugClinicDepts[i].OutQuantity <= 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1174_G1_SLgXuatLonHon0, eHCMSResources.G0442_G1_TBao);
                            return false;
                        }
                        //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                        //if (SelectedOutInvoice.OutwardDrugClinicDepts[i].OutPrice <= 0)
                        //{
                        //    Globals.ShowMessage("Giá bán phải > 0. Vui lòng xem lại bảng giá bán!", eHCMSResources.G0442_G1_TBao);
                        //    return false;
                        //}

                        //if (SelectedOutInvoice.OutDate.GetValueOrDefault().Date < SelectedOutInvoice.OutwardDrugClinicDepts[i].InwardDate.Date)
                        //{
                        //    strInvalidProduct += SelectedOutInvoice.OutwardDrugClinicDepts[i].GenMedProductItem.BrandName + ". Ngày nhập: " + SelectedOutInvoice.OutwardDrugClinicDepts[i].InwardDate.ToShortDateString() + Environment.NewLine;
                        //}

                        if (V_MedProductType == AllLookupValues.MedProductType.Y_CU && SelectedOutInvoice.OutwardDrugClinicDepts[i].V_MedicalMaterial <= 0)
                        {
                            strNumOfRow += (i + 1).ToString() + ", ";
                            //MessageBox.Show("Hãy chọn loại vật tư y tế trên từng dòng!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            //return false;
                        }

                        if (RequireDoctorAndDate && (SelectedOutInvoice.OutwardDrugClinicDepts[i].DoctorStaff == null || SelectedOutInvoice.OutwardDrugClinicDepts[i].DoctorStaff.StaffID <= 0))
                        {
                            strRowWithoutDoctorStaffID += (i + 1).ToString() + ", ";
                        }

                        if (RequireDoctorAndDate && SelectedOutInvoice.OutwardDrugClinicDepts[i].MedicalInstructionDate == null)
                        {
                            strRowWithoutMedicalInstructionDate += (i + 1).ToString() + ", ";
                        }
                        if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0
                            && (SelectedOutInvoice.OutwardDrugClinicDepts[i].MedicalInstructionDate.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)
                        {
                            strRowInvalidMedicalInstructDate += (i + 1).ToString() + ", ";
                        }
                        //neu ngay het han lon hon ngay hien tai
                        if (V_MedProductType == AllLookupValues.MedProductType.THUOC)
                        {
                            if (eHCMS.Services.Core.AxHelper.CompareDate(DateTime.Now, SelectedOutInvoice.OutwardDrugClinicDepts[i].InExpiryDate.GetValueOrDefault()) == 1)
                            {
                                strError += string.Format(eHCMSResources.Z1401_G1_DaHetHanDung, SelectedOutInvoice.OutwardDrugClinicDepts[i].GenMedProductItem.BrandName, (i + 1).ToString()) + Environment.NewLine;
                            }
                        }
                    }

                    //KMx: Tạm thời bỏ kiểm tra ngày xuất trước ngày nhập (29/03/2015 17:18).
                    //if (!string.IsNullOrEmpty(strInvalidProduct))
                    //{
                    //    MessageBox.Show("Không thể lưu vì những loại hàng sau đây có ngày nhập sau ngày xuất: " + Environment.NewLine + strInvalidProduct + Environment.NewLine + "Ngày xuất: " + SelectedOutInvoice.OutDate.GetValueOrDefault().ToShortDateString() + Environment.NewLine + "Cách giải quyết: Bạn hãy chọn lô hàng khác hoặc đổi ngày xuất."
                    //                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    return false;
                    //}

                    if (!string.IsNullOrEmpty(strNumOfRow))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1177_G1_ChuaChonLoaiVTYT, strNumOfRow), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }

                    if (!string.IsNullOrEmpty(strRowWithoutDoctorStaffID))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1179_G1_ChuaChonBSiCDinh, strRowWithoutDoctorStaffID), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }

                    if (!string.IsNullOrEmpty(strRowWithoutMedicalInstructionDate))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1182_G1_ChuaChonNgYLenh, strRowWithoutMedicalInstructionDate), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(strRowInvalidMedicalInstructDate))
                    {
                        string msg = Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate == 0
                            ? string.Format(eHCMSResources.Z1875_G1_NgYLenhKgLonHonNgHTai2, strRowInvalidMedicalInstructDate)
                            : string.Format(eHCMSResources.Z1874_G1_NgYLenhKgLonHonNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate, strRowInvalidMedicalInstructDate);
                        MessageBox.Show(msg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                        return false;
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
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.G0442_G1_TBao);
                return false;
            }
            return true;
        }

        public void btnUpdate()
        {
            if (MessageBox.Show(eHCMSResources.A0122_G1_Msg_ConfSuaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                btnSave();
            }
        }

        private RequestSearchCriteria reqSearchCriteria;

        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardClinicDept> results, int Totalcount, bool bCreateNewListFromOld)
        {
            Action<IStoreDeptRequestSearch> onInitDlg = delegate (IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria = reqSearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = (long)V_MedProductType;
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;

                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (SelectedOutInvoice != null)
                {
                    proAlloc.FilterByPtRegistrationID = SelectedOutInvoice.PtRegistrationID.GetValueOrDefault();
                }

                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardClinicDept p in results)
                    {
                        proAlloc.RequestDruglist.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
        }

        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            if (SelectedOutInvoice == null)
                return;

            if (SelectedOutInvoice.StoreID == null || (SelectedOutInvoice.StoreID.GetValueOrDefault(0) <= 0))
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat);
                return;
            }

            reqSearchCriteria = new RequestSearchCriteria();
            if (reqSearchCriteria == null)
            {
                return;
            }

            reqSearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
            reqSearchCriteria.ToDate = Globals.GetCurServerDateTime();


            reqSearchCriteria.RequestStoreID = SelectedOutInvoice.StoreID;
            reqSearchCriteria.PtRegistrationID = SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0);

            this.ShowBusyIndicator();

            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(reqSearchCriteria, (long)V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    //mo pop up tim
                                    OpenPopUpSearchRequestInvoice(results.ToList(), Total, false);
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchRequestInvoice(null, 0, false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }

        }

        public void btnAddMoreReq()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.PtRegistrationID == null)
            {
                return;
            }
            if (SelectedOutInvoice.PtRegistrationID.Value > 0)
            {
                SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
            }
        }

        //KMx: Trước đây, khi Click chọn VTYT trên từng dòng thì sẽ gọi hàm RdbMedicalMaterial_Click (*) để set V_MedicalMaterial, để set V_MedicalMaterial cho đúng thì phải bind IsReplaceMedMat và IsDisposeMedMat rồi sau đó mới gọi hàm (*).
        //Nhưng đôi lúc, chương trình gọi hàm (*) trước, rồi sau đó mới bind IsReplaceMedMat và IsDisposeMedMat sau, dẫn đến set V_MedicalMaterial sai.
        //Bay giờ, khi lưu thì set V_MedicalMaterial 1 lần luôn, không dùng hàm (*) nữa (20/05/2015 10:24).
        private void SetV_MedicalMaterial()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }

            foreach (OutwardDrugClinicDept outward in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                if (outward.IsReplaceMedMat)
                {
                    outward.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE;
                }
                else if (outward.IsDisposeMedMat)
                {
                    outward.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO;
                }
                else
                {
                    outward.V_MedicalMaterial = 0;
                }
            }
        }

        public void btnSave()
        {
            if (Globals.IsLockRegistration(SelectedOutInvoice.PatientRegistration.RegLockFlag, eHCMSResources.Z1188_G1_LuuPhXuatThuoc))
            {
                return;
            }
            if (V_MedProductType == AllLookupValues.MedProductType.Y_CU)
            {
                SetV_MedicalMaterial();
            }
            //▼=== #009
            if (SelectedOutInvoice.OutDate == null)
            {
                MessageBox.Show(eHCMSResources.A0593_G1_Msg_InfoChonNgXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.PatientRegistration != null && SelectedOutInvoice.PatientRegistration.ExamDate != null && SelectedOutInvoice.OutDate.Value.Date < SelectedOutInvoice.PatientRegistration.ExamDate.Date)
            {
                MessageBox.Show(eHCMSResources.Z2340_G1_NXKhongNhoHonNDK, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.PatientRegistration != null && SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate.HasValue && SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate != null && SelectedOutInvoice.OutDate.Value.Date > SelectedOutInvoice.PatientRegistration.DischargeDetailRecCreatedDate.Value.Date)
            {
                MessageBox.Show(eHCMSResources.Z2339_G1_NXKhongLonHonNXV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            //▲=== #009
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null
                && SelectedOutInvoice.OutwardDrugClinicDepts.Any(x => x.RecordState != RecordState.DELETED && !CheckValidMedicalInstructionDate(x.MedicalInstructionDate)))
            {
                return;
            }
            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.OutwardDrugClinicDepts.Any(x => x.RecordState != RecordState.DELETED && RequireDoctorAndDate && (x.DoctorStaff == null || x.DoctorStaff.StaffID <= 0)))
            {
                MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!CheckData())
            {
                return;
            }

            if (SelectedOutInvoice.RequestDrugInwardClinicDepts == null)
            {
                SelectedOutInvoice.RequestDrugInwardClinicDepts = new ObservableCollection<RequestDrugInwardClinicDept>();
            }

            if (RequestDrugInwardClinicDeptLst != null)
            {
                bool bFoundInSavedList = false;
                foreach (RequestDrugInwardClinicDept selItemInList in RequestDrugInwardClinicDeptLst)
                {
                    bFoundInSavedList = false;
                    if (selItemInList.Checked)
                    {
                        foreach (RequestDrugInwardClinicDept savedItem in SelectedOutInvoice.RequestDrugInwardClinicDepts)
                        {
                            if (savedItem.ReqDrugInClinicDeptID == selItemInList.ReqDrugInClinicDeptID)
                            {
                                bFoundInSavedList = true;
                                break;
                            }
                        }
                        if (!bFoundInSavedList)
                        {
                            SelectedOutInvoice.RequestDrugInwardClinicDepts.Add(selItemInList);
                        }
                    }
                }
            }

            SelectedOutInvoice.MedProductType = V_MedProductType;
            SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;


            if (SelectedOutInvoice.OutDate.GetValueOrDefault().TimeOfDay == new TimeSpan(0, 0, 0))
            {
                DateTime Now = Globals.GetCurServerDateTime();
                SelectedOutInvoice.OutDate = SelectedOutInvoice.OutDate.GetValueOrDefault().Date.AddHours(Now.Hour).AddMinutes(Now.Minute).AddSeconds(Now.Second);
            }

            if (this.DoseVisibility && SelectedOutInvoice != null
                && SelectedOutInvoice.OutwardDrugClinicDepts != null
                && SelectedOutInvoice.OutwardDrugClinicDepts.Any(x => x.GenMedProductItem.InsuranceCover.GetValueOrDefault() && (x.MDose + x.ADose + x.EDose + x.NDose) <= 0.0 && string.IsNullOrEmpty(x.Administration)))
            {
                MessageBox.Show(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //▼====== #008
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                int temp = 0;
                foreach (var tmpOutwardDrugClinicDept in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    if (tmpOutwardDrugClinicDept.OutQuantity > tmpOutwardDrugClinicDept.GenMedProductItem.RemainingFirst)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z2308_G1_KhongXuatVuotSLTrongLo
                            , tmpOutwardDrugClinicDept.GenMedProductItem.BrandName
                            , tmpOutwardDrugClinicDept.InBatchNumber
                            , tmpOutwardDrugClinicDept.GenMedProductItem.RemainingFirst
                            , tmpOutwardDrugClinicDept.OutQuantity));
                        return;
                    }
                    //▼====: #019
                    if (Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan != "" && (tmpOutwardDrugClinicDept.OutQuantity != (int)tmpOutwardDrugClinicDept.OutQuantity)
                        && !Globals.ServerConfigSection.ClinicDeptElements.ThuocDuocXuatThapPhan.Contains(tmpOutwardDrugClinicDept.ChargeableItemCode))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1400_G1_SLgXuatLaSoNguyen
                            , (temp + ": (" + tmpOutwardDrugClinicDept.ChargeableItemCode + ") - " + tmpOutwardDrugClinicDept.GenMedProductItem.BrandName)));
                        return;
                    }
                    //▲====: #019
                }
            }
            //▲====== #008
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                SelectedOutInvoice.V_RegistrationType = V_RegistrationType;
            }
            //KMx: Cập nhật phiếu theo cách mới (28/01/2015 14:13).
            if (SelectedOutInvoice.outiID > 0 && Globals.ServerConfigSection.ClinicDeptElements.UpdateOutwardToPatientNew)
            {
                OutwardDrugClinicDeptInvoice_UpdateByType(SelectedOutInvoice);
            }
            else
            {
                OutwardDrugClinicDeptInvoice_SaveByType(SelectedOutInvoice);
            }
        }

        public void btnNew()
        {
            RefreshData();
            SetCheckAllOutwardDetail();
            aucHoldConsultDoctor.setDefault();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
        }

        private void DeepCopyOutwardDrugMedDept()
        {
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                OutwardDrugClinicDeptsCopy = SelectedOutInvoice.OutwardDrugClinicDepts.DeepCopy();
            }
            else
            {
                OutwardDrugClinicDeptsCopy = null;
            }
            if (SelectedOutInvoice != null)
            {
                SelectedOutInvoiceCoppy = SelectedOutInvoice.DeepCopy();
            }

        }


        //KMx: Trước đây func này chỉ lấy thuốc/ y cụ/ hóa chất theo phiếu yêu cầu. Sửa lại, lấy theo template luôn (23/12/2015 16:45).
        private void GetInBatchNumberAndPrice_ListForRequestClinicDept(bool? IsCost
            , ObservableCollection<RequestDrugInwardClinicDept> ReqDrugInClinicDeptID, long OutwardTemplateID, long StoreID
            , long V_MedProductType, long PtRegistrationID, bool bReqFormSelectedFromPopup = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //KMx: Không sử dụng biến IsHIPatient khi đi tìm thuốc nữa, vì bị lỗi người này gán cho người kia (14/02/2015 14:56).
                    //contract.BeginspGetInBatchNumberAndPrice_ListForRequestClinicDept(IsCost, ReqDrugInClinicDeptID, StoreID, V_MedProductType, PtRegistrationID, IsHIPatient, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginspGetInBatchNumberAndPrice_ListForRequestClinicDept(IsCost, ReqDrugInClinicDeptID 
                        , OutwardTemplateID, StoreID, V_MedProductType, PtRegistrationID, null
                        , SelectedOutInvoice.OutDate.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndspGetInBatchNumberAndPrice_ListForRequestClinicDept(asyncResult);

                            // TxD 04/04/2015: If Request Form is selected from the newly created popup then ONLY ADD the outward items to the list
                            if (bReqFormSelectedFromPopup)
                            {
                                if (SelectedOutInvoice.OutwardDrugClinicDepts == null)
                                    SelectedOutInvoice.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>();
                                foreach (OutwardDrugClinicDept resItem in results)
                                {
                                    if (resItem.DoctorStaff == null && aucHoldConsultDoctor != null)
                                    {
                                        resItem.DoctorStaff = new Staff
                                        {
                                            StaffID = aucHoldConsultDoctor.StaffID,
                                            FullName = aucHoldConsultDoctor.StaffName
                                        };
                                    }
                                    if (MedicalInstructionDateContent != null)
                                    {
                                        resItem.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                                    }
                                    SelectedOutInvoice.OutwardDrugClinicDepts.Add(resItem);
                                }
                            }
                            else
                            {
                                SelectedOutInvoice.OutwardDrugClinicDepts = results.ToObservableCollection();
                            }

                            CountPatientOrNot(true);

                            if (SelectedOutInvoice != null && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0)
                            {
                                CountHIOrNot(true);
                            }

                            SetCheckAllOutwardDetail();

                            SumTotalPrice();

                            if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.PatientRegistration != null)
                            {
                                foreach (var item in SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.MedicalInstructionDate != null).ToList())
                                {
                                    item.MedicalInstructionDate = Globals.ApplyValidMedicalInstructionDate(item.MedicalInstructionDate.Value, SelectedOutInvoice.PatientRegistration);
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
            });

            t.Start();
        }

        //private void GetPatientRegistrationByPtRegistrationID(long PtRegistrationID)
        //{
        //    isLoadingFullOperator = true;
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PatientRegistrationServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetPatientRegistrationByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {

        //                    var results = contract.EndGetPatientRegistrationByPtRegistrationID(asyncResult);
        //                    if (results != null)
        //                    {
        //                        SelectedOutInvoice.PatientRegistration = results;
        //                        GetHIPatient(SelectedOutInvoice.PatientRegistration);
        //                        OutwardDrugClinicDeptDetails_Load(SelectedOutInvoice.outiID);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    isLoadingFullOperator = false;
        //                    // Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}


        //dung lai from tim kiem cua Demage
        #region IHandle<DrugDeptCloseSearchOutClinicDeptInvoiceEvent> Members

        public void Handle(DrugDeptCloseSearchOutClinicDeptInvoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugClinicDeptInvoice temp = message.SelectedOutClinicDeptInvoice as OutwardDrugClinicDeptInvoice;
                if (temp != null)
                {
                    ClearData();
                    SelectedOutInvoice = temp;

                    if (SelectedOutInvoice.InPatientBillingInvID.GetValueOrDefault(0) > 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0921_G1_PhXuatDaTaoBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }

                    LoadOutwardClinicDeptDetails();
                    //▼===== #013 Ngăn lại không load danh sách phiếu lĩnh và danh sách y lệnh vì bệnh nhân ngoại trú sử dụng đồ nội trú không có 2 danh sách này.
                    if (V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0), (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                        LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
                    }
                    //▲===== 
                }
            }
        }

        #endregion

        #region IHandle<DrugDeptPayEvent> Members
        private PatientTransactionPayment _PatientPayment = null;
        private IEnumerator<IResult> DoAddTracsactionForDrug()
        {
            AddTracsactionForDrugTask payTask = new AddTracsactionForDrugTask(_PatientPayment, SelectedOutInvoice.outiID, V_TranRefType);
            yield return payTask;
            if (payTask.Error == null)
            {
                if (payTask.PaymentID > 0)
                {
                    //goi ham in phieu thu hay phieu chi gi do
                    Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
                    {
                        proAlloc.ID = SelectedOutInvoice.outiID;
                        proAlloc.PaymentID = payTask.PaymentID;
                        proAlloc.StaffFullName = GetStaffLogin().FullName;
                        proAlloc.V_TranRefType = (long)AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC;
                        if (_PatientPayment.CreditOrDebit == 1)
                        {
                            proAlloc.LyDo = eHCMSResources.Z0335_G1_ThuTienBanThuocLe;
                            proAlloc.eItem = ReportName.PHARMACY_VISITORPHIEUTHU;
                        }
                        else
                        {
                            proAlloc.LyDo = "Đổi thuốc";
                            proAlloc.eItem = ReportName.PHARMACY_PHIEUCHI;
                        }
                    };
                    GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);

                    GetOutwardDrugClinicDeptInvoice(SelectedOutInvoice.outiID);
                }
            }
        }

        #endregion

        #region IHandle<ClinicDeptChooseBatchNumberEvent> Members

        public void Handle(ClinicDeptChooseBatchNumberEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugClinicDept.GenMedProductItem = message.BatchNumberSelected;
                SelectedOutwardDrugClinicDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugClinicDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugClinicDept.InID = message.BatchNumberSelected.InID;
                SelectedOutwardDrugClinicDept.VAT = message.BatchNumberSelected.VAT;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.OutPrice;
                }
                //▼===== #014
                SelectedOutwardDrugClinicDept.HIAllowedPrice = message.BatchNumberSelected.HIAllowedPrice;
                //▲===== #014
                SelectedOutwardDrugClinicDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<ClinicDeptChooseBatchNumberResetQtyEvent> Members

        public void Handle(ClinicDeptChooseBatchNumberResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrugClinicDept.GenMedProductItem = message.BatchNumberSelected;
                SelectedOutwardDrugClinicDept.InBatchNumber = message.BatchNumberSelected.InBatchNumber;
                SelectedOutwardDrugClinicDept.InExpiryDate = message.BatchNumberSelected.InExpiryDate;
                SelectedOutwardDrugClinicDept.InID = message.BatchNumberSelected.InID;
                SelectedOutwardDrugClinicDept.VAT = message.BatchNumberSelected.VAT;
                if (IsCost.GetValueOrDefault())
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.InCost;
                }
                else
                {
                    SelectedOutwardDrugClinicDept.OutPrice = message.BatchNumberSelected.OutPrice;
                }
                SelectedOutwardDrugClinicDept.SdlDescription = message.BatchNumberSelected.SdlDescription;
                SelectedOutwardDrugClinicDept.OutQuantity = message.BatchNumberSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        #region View By Code Member

        //private bool? IsCode = false;
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                string Code = Globals.FormatCode((long)V_MedProductType, txt);
                SearchRefGenMedProductDetails(Code, true);
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

        //private bool _VisibilityName = true;
        //public bool VisibilityName
        //{
        //    get
        //    {
        //        if (SelectedOutInvoice != null)
        //        {
        //            return _VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
        //        }
        //        return _VisibilityName;
        //    }
        //    set
        //    {
        //        if (SelectedOutInvoice != null)
        //        {
        //            _VisibilityName = value && SelectedOutInvoice.CanSaveAndPaid;
        //            _VisibilityCode = !_VisibilityName && SelectedOutInvoice.CanSaveAndPaid;
        //        }
        //        else
        //        {
        //            _VisibilityName = value;
        //            _VisibilityCode = !_VisibilityName;
        //        }
        //        NotifyOfPropertyChange(() => VisibilityName);
        //        NotifyOfPropertyChange(() => VisibilityCode);

        //    }
        //}

        //private bool _VisibilityCode = false;
        //public bool VisibilityCode
        //{
        //    get
        //    {
        //        return _VisibilityCode;
        //    }
        //    set
        //    {
        //        if (_VisibilityCode != value)
        //        {
        //            _VisibilityCode = value;
        //            NotifyOfPropertyChange(() => VisibilityCode);
        //        }
        //    }
        //}

        //public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    IsCode = true;
        //    VisibilityName = false;
        //}

        //public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    IsCode = false;
        //    VisibilityName = true;
        //}
        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        TextBox tbxMDoseStr;
        public void tbxMDoseStr_Loaded(object sender, RoutedEventArgs e)
        {
            tbxMDoseStr = sender as TextBox;
        }

        #endregion


        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (this.GetView() == null || message == null || message.Item == null)
            {
                return;
            }
            // Clear het cac du lieu con tren man hinh
            RefreshData();
            if (message.Item.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (message.Item.AdmissionDate == null)
                {
                    if (message.Item.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING_INPT || !Globals.IsCasuaOrPreOpPt(message.Item.V_RegForPatientOfType))
                    {
                        MessageBox.Show(eHCMSResources.Z1196_G1_TThaiDKKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                    if (!Globals.Check_CasualAndPreOpReg_StillValid(message.Item))
                    {
                        MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKDong_KhTheXuatThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                }
                else
                {
                    if (message.Item.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED)
                    {
                        MessageBox.Show(eHCMSResources.Z1196_G1_TThaiDKKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                    if (message.Item.IsDischarge)
                    {
                        MessageBox.Show(eHCMSResources.A0239_G1_Msg_InfoBNDaXV_KTheXuatThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        return;
                    }
                }
            }
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            else
            {
                RefreshData();
            }
            SelectedOutInvoice.PatientRegistration = message.Item;
            if (message.Item.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                SelectedOutInvoice.PtRegistrationID = message.Item.PtRegistrationID;
            }
            else
            {
                SelectedOutInvoice.OutPtRegistrationID = message.Item.PtRegistrationID;
            }
            ClearData();
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.Notes = "";
            SelectedOutInvoice.OutInvID = "";
            SelectedOutInvoice.BillingInvNum = "";
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
            }
            SetCheckAllOutwardDetail();
            if (message.Item.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PatientRegistration.PtRegistrationID, (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), SelectedOutInvoice.outiID);
                LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
            }
        }
        //▼====== #010:
        public void Handle(InPatientRegistrationSelectedForOutwardToPatient_V2 message)
        {
            if (this.GetView() == null || message == null || message.Source == null)
            {
                return;
            }
            RefreshData();
            if (message.Source.AdmissionDate == null)
            {
                if (message.Source.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING_INPT || !Globals.IsCasuaOrPreOpPt(message.Source.V_RegForPatientOfType))
                {
                    MessageBox.Show(eHCMSResources.Z1196_G1_TThaiDKKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (!Globals.Check_CasualAndPreOpReg_StillValid(message.Source))
                {
                    MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKDong_KhTheXuatThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            else
            {
                if (message.Source.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED)
                {
                    MessageBox.Show(eHCMSResources.Z1196_G1_TThaiDKKgHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                if (message.Source.IsDischarge)
                {
                    MessageBox.Show(eHCMSResources.A0239_G1_Msg_InfoBNDaXV_KTheXuatThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
            }
            else
            {
                RefreshData();
            }

            SelectedOutInvoice.PatientRegistration = message.Source;
            SelectedOutInvoice.PtRegistrationID = message.Source.PtRegistrationID;

            ClearData();
            SelectedOutInvoice.outiID = 0;
            SelectedOutInvoice.Notes = "";
            SelectedOutInvoice.OutInvID = "";
            SelectedOutInvoice.BillingInvNum = "";
            if (SelectedOutInvoice.OutwardDrugClinicDepts != null)
            {
                SelectedOutInvoice.OutwardDrugClinicDepts.Clear();
            }

            SetCheckAllOutwardDetail();

            GetRequestDrugInwardClinicDept_ByRegistrationID(SelectedOutInvoice.PatientRegistration.PtRegistrationID, (long)V_MedProductType, SelectedOutInvoice.StoreID.GetValueOrDefault(0), SelectedOutInvoice.outiID);
            LoadInPatientInstructionForRequest(SelectedOutInvoice.PtRegistrationID.Value);
        }
        //▲====== #010:
        public void btnFindRequest()
        {
            //IsChanged = true;
            Action<IStoreDeptRequestSearch> onInitDlg = delegate (IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria.IsApproved = true;
                proAlloc.SearchCriteria.DaNhanHang = true;
                if (SelectedOutInvoice != null)
                {
                    proAlloc.SearchCriteria.RequestStoreID = SelectedOutInvoice.StoreID;
                }
                proAlloc.V_MedProductType = (long)V_MedProductType;
                proAlloc.SearchRequestDrugInwardClinicDept(0, Globals.PageSize);
            };
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
        }


        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && IsActive)
            {
                RequestDrugInwardClinicDept item = message.SelectedRequest as RequestDrugInwardClinicDept;
                SelectedOutInvoice.ReqNumCode = item.ReqNumCode;
                SelectedOutInvoice.ReqDrugInClinicDeptID = item.ReqDrugInClinicDeptID;
                ClearData();
                //tu phieu yeu cau va ma dang ky lay ds hang can ban len
                if (SelectedOutInvoice.PtRegistrationID > 0)
                {
                    //GetInBatchNumberAndPrice_ListForRequestClinicDept(false, SelectedOutInvoice.ReqDrugInClinicDeptID.GetValueOrDefault(), SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault());
                }
            }
        }

        //public void btnGetOut()
        //{
        //    if (SelectedOutInvoice == null)
        //    {
        //        return;
        //    }
        //    if (SelectedOutInvoice.OutDate == null)
        //    {
        //        MessageBox.Show("Hãy chọn ngày xuất!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }
        //    if (RequestDrugInwardClinicDeptLst != null && RequestDrugInwardClinicDeptLst.Count > 0)
        //    {
        //        RequestDrugInwardClinicDeptListSelected = RequestDrugInwardClinicDeptLst.Where(x => x.Checked == true).ToObservableCollection();
        //        //KMx: Những phiếu yêu cầu được chọn (20/07/2014 12:00).
        //        SelectedOutInvoice.RequestDrugInwardClinicDepts = RequestDrugInwardClinicDeptListSelected;

        //        if (RequestDrugInwardClinicDeptListSelected != null && RequestDrugInwardClinicDeptListSelected.Count > 0)
        //        {
        //            GetInBatchNumberAndPrice_ListForRequestClinicDept(false, RequestDrugInwardClinicDeptListSelected, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault());
        //        }
        //    }
        //}

        public void Handle(ClinicDeptInPtSelReqFormForOutward message)
        {
            if (message == null || IsActive == false)
                return;
            if (SelectedOutInvoice == null)
                return;

            RequestDrugInwardClinicDept item = message.SelectedReqForm as RequestDrugInwardClinicDept;

            if (RequestDrugInwardClinicDeptLst == null)
            {
                RequestDrugInwardClinicDeptLst = new ObservableCollection<RequestDrugInwardClinicDept>();
            }

            bool bFoundItemInList = false;
            foreach (RequestDrugInwardClinicDept reqItemAlreadyInList in RequestDrugInwardClinicDeptLst)
            {
                if (item.ReqDrugInClinicDeptID == reqItemAlreadyInList.ReqDrugInClinicDeptID)
                {
                    bFoundItemInList = true;
                    break;
                }
            }
            if (bFoundItemInList)
            {
                string strMsg = string.Format(eHCMSResources.Z1776_G1_PhLinh0DaDcChon, item.ReqNumCode);
                MessageBox.Show(strMsg);
                return;
            }

            item.Checked = true;
            ObservableCollection<RequestDrugInwardClinicDept> theSelectedReqFormInAList = new ObservableCollection<RequestDrugInwardClinicDept>();
            theSelectedReqFormInAList.Add(item);

            //GetInBatchNumberAndPrice_ListForRequestClinicDept(false, theSelectedReqFormInAList, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(), true);            
            GetInBatchNumberAndPrice_ListForRequestClinicDept(false, theSelectedReqFormInAList, 0, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(), true);

            RequestDrugInwardClinicDeptLst.Add(item);

        }

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
            if (RequestDrugInwardClinicDeptLst != null && RequestDrugInwardClinicDeptLst.Count > 0)
            {
                for (int i = 0; i < RequestDrugInwardClinicDeptLst.Count; i++)
                {
                    RequestDrugInwardClinicDeptLst[i].Checked = true;
                }
            }
        }
        private void UnCheckedfc()
        {
            if (RequestDrugInwardClinicDeptLst != null && RequestDrugInwardClinicDeptLst.Count > 0)
            {
                for (int i = 0; i < RequestDrugInwardClinicDeptLst.Count; i++)
                {
                    RequestDrugInwardClinicDeptLst[i].Checked = false;
                }
            }
        }

        private ObservableCollection<RequestDrugInwardClinicDept> RequestDrugInwardClinicDeptListSelected;
        public void btnGetOut()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            if (Globals.IsLockRegistration(SelectedOutInvoice.PatientRegistration.RegLockFlag, eHCMSResources.Z1172_G1_XuatThuoc))
            {
                return;
            }
            if (SelectedOutInvoice.OutDate == null)
            {
                MessageBox.Show(eHCMSResources.A0593_G1_Msg_InfoChonNgXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (RequestDrugInwardClinicDeptLst != null && RequestDrugInwardClinicDeptLst.Count > 0)
            {
                RequestDrugInwardClinicDeptListSelected = RequestDrugInwardClinicDeptLst.Where(x => x.Checked == true).ToObservableCollection();

                //KMx: Những phiếu yêu cầu được chọn (20/07/2014 12:00).
                //SelectedOutInvoice.RequestDrugInwardClinicDepts = RequestDrugInwardClinicDeptListSelected;

                if (RequestDrugInwardClinicDeptListSelected != null && RequestDrugInwardClinicDeptListSelected.Count > 0)
                {
                    //GetInBatchNumberAndPrice_ListForRequestClinicDept(false, RequestDrugInwardClinicDeptListSelected, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault());
                    GetInBatchNumberAndPrice_ListForRequestClinicDept(false, RequestDrugInwardClinicDeptListSelected, 0, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault());
                }
            }
        }
        public void btnGetOutFromInstruction()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            if (SelectedOutInvoice.PatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.K0299_G1_ChonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.PatientRegistration != null && Globals.IsLockRegistration(SelectedOutInvoice.PatientRegistration.RegLockFlag, eHCMSResources.Z1172_G1_XuatThuoc))
            {
                return;
            }
            if (SelectedOutInvoice.OutDate == null)
            {
                MessageBox.Show(eHCMSResources.A0593_G1_Msg_InfoChonNgXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentInstructionCollection == null || !CurrentInstructionCollection.Any(x => x.IsChecked))
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new PharmacyClinicDeptServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(CurrentInstructionCollection.Where(x => x.IsChecked).Select(x => x.IntPtDiagDrInstructionID).ToArray(), SelectedOutInvoice == null ? -1 : SelectedOutInvoice.StoreID.GetValueOrDefault(-1), (long)V_MedProductType, SelectedOutInvoice.PatientRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                      {
                          try
                          {
                              var ItemCollection = CurrentContract.EndGetInBatchNumberAndPrice_ListForRequestClinicDeptFromInstruction(asyncResult);
                              if (ItemCollection == null)
                              {
                                  ItemCollection = new List<OutwardDrugClinicDept>();
                              }
                              else
                              {
                                  SelectedOutInvoice.FromIntPtDiagDrInstructionIDCollection = CurrentInstructionCollection.Where(x => x.IsChecked).ToArray();
                              }
                              SelectedOutInvoice.OutwardDrugClinicDepts = ItemCollection.ToObservableCollection();
                              CountPatientOrNot(true);
                              if (SelectedOutInvoice != null && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0)
                              {
                                  CountHIOrNot(true);
                              }
                              SetCheckAllOutwardDetail();
                              SumTotalPrice();
                              if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugClinicDepts != null && SelectedOutInvoice.PatientRegistration != null)
                              {
                                  foreach (var item in SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.MedicalInstructionDate != null).ToList())
                                  {
                                      item.MedicalInstructionDate = Globals.ApplyValidMedicalInstructionDate(item.MedicalInstructionDate.Value, SelectedOutInvoice.PatientRegistration);
                                  }
                                  //▼===== #012
                                  ObservableCollection<OutwardDrugClinicDept> ListOutwardDrugClinicDepts = SelectedOutInvoice.OutwardDrugClinicDepts;
                                  foreach (var item in ListOutwardDrugClinicDepts)
                                  {
                                      //Lấy ra cách pha chế của từng group dịch truyền sau đó gán ngược lại cho toàn bộ các dịch truyền trong gr.
                                      //Lý do:  Vì dịch truyền không có nhập liều dùng mà sẽ được bác sĩ hướng dẫn cách pha chế và tim truyền cho bệnh nhân. Với cách pha chế là tổng hợp của các
                                      //        thành phần bên trong dịch truyền và liều lượng sử dụng.
                                      tmpAdministration = "";
                                      if (SetValueForAdministrationOutwardDrug(item, out tmpAdministration))
                                      {
                                          foreach (var tmpOutwardDrugClinicDept in SelectedOutInvoice.OutwardDrugClinicDepts)
                                          {
                                              if ((tmpOutwardDrugClinicDept.MDose + tmpOutwardDrugClinicDept.EDose + tmpOutwardDrugClinicDept.ADose + tmpOutwardDrugClinicDept.NDose) <= 0
                                                    && string.IsNullOrEmpty(tmpOutwardDrugClinicDept.Administration) && tmpOutwardDrugClinicDept.IntravenousPlan_InPtID == item.IntravenousPlan_InPtID)
                                              {
                                                  tmpOutwardDrugClinicDept.Administration = tmpAdministration;
                                              }
                                          }
                                      }
                                  }
                                  //Sau khi đã set hết cho tất cả các thuốc trong dịch truyền rồi thì sẽ tới y cụ. 
                                  //Nếu như thuốc, y cụ không có liều dùng hoặc hướng dẫn sử dụng thì sẽ gán bằng câu mặc định: Sử dụng theo y lệnh bác sĩ.
                                  foreach (var tmpOutwardDrugClinicDept in SelectedOutInvoice.OutwardDrugClinicDepts)
                                  {
                                      if ((tmpOutwardDrugClinicDept.MDose + tmpOutwardDrugClinicDept.EDose + tmpOutwardDrugClinicDept.ADose + tmpOutwardDrugClinicDept.NDose <= 0)
                                            && string.IsNullOrEmpty(tmpOutwardDrugClinicDept.Administration))
                                      {
                                          tmpOutwardDrugClinicDept.Administration = eHCMSResources.Z2923_G1_SuDungTheoYLenhBacSi;
                                      }
                                  }
                                  //▲===== #012
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
            CurrentThread.Start();
        }
        //▼===== #012: Hàm dùng để lấy ra Cách pha chế (dịch truyền) của y lệnh.
        private string tmpAdministration = "";
        private bool SetValueForAdministrationOutwardDrug(OutwardDrugClinicDept tmpOutwardDrugClinicDept, out string tmpAdministration)
        {
            tmpAdministration = "";
            if (!string.IsNullOrEmpty(tmpOutwardDrugClinicDept.Notes))
            {
                tmpAdministration = tmpOutwardDrugClinicDept.Notes;
                return true;
            }
            return false;
        }
        //▲===== #012
        //public void RdbMedicalMaterial_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedOutwardDrugClinicDept == null)
        //    {
        //        return;
        //    }
        //    if (SelectedOutwardDrugClinicDept.IsReplaceMedMat)
        //    {
        //        SelectedOutwardDrugClinicDept.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_THAYTHE;
        //    }
        //    else if (SelectedOutwardDrugClinicDept.IsDisposeMedMat)
        //    {
        //        SelectedOutwardDrugClinicDept.V_MedicalMaterial = (long)AllLookupValues.V_MedicalMaterial.VTYT_TIEUHAO;
        //    }
        //    else
        //    {
        //        SelectedOutwardDrugClinicDept.V_MedicalMaterial = 0;
        //    }
        //}

        private bool _CheckAllOutwardDetail;
        public bool CheckAllOutwardDetail
        {
            get
            {
                return _CheckAllOutwardDetail;
            }
            set
            {
                if (_CheckAllOutwardDetail != value)
                {
                    _CheckAllOutwardDetail = value;
                    NotifyOfPropertyChange(() => CheckAllOutwardDetail);
                }
            }
        }

        private bool _CheckAllCountHI;
        public bool CheckAllCountHI
        {
            get
            {
                return _CheckAllCountHI;
            }
            set
            {
                if (_CheckAllCountHI != value)
                {
                    _CheckAllCountHI = value;
                    NotifyOfPropertyChange(() => CheckAllCountHI);
                    CountHIOrNot(_CheckAllCountHI);
                    SumTotalPrice();
                }
            }
        }

        private bool _CheckAllCountPatient;
        public bool CheckAllCountPatient
        {
            get
            {
                return _CheckAllCountPatient;
            }
            set
            {
                if (_CheckAllCountPatient != value)
                {
                    _CheckAllCountPatient = value;
                    NotifyOfPropertyChange(() => CheckAllCountPatient);
                    CountPatientOrNot(_CheckAllCountPatient);
                    SumTotalPrice();
                }
            }
        }

        public void chkAllOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                return;
            }

            if (CheckAllOutwardDetail)
            {
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    item.IsChecked = true;
                }
            }
            else
            {
                foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
                {
                    item.IsChecked = false;
                }
            }
        }

        private void SetCheckAllOutwardDetail()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                CheckAllOutwardDetail = false;
                return;
            }

            CheckAllOutwardDetail = SelectedOutInvoice.OutwardDrugClinicDepts.All(x => x.IsChecked);

        }

        public void chkOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            SetCheckAllOutwardDetail();
        }



        //CheckBox chkAllCountHI = new CheckBox();
        //CheckBox chkAllCountPatient = new CheckBox();
        //public void chkAllCountHI_Loaded(object sender)
        //{
        //    chkAllCountHI = sender as CheckBox;
        //}

        //public void chkAllCountPatient_Loaded(object sender)
        //{
        //    chkAllCountPatient = sender as CheckBox;
        //}

        public void chkCountPatient_Click(object sender, RoutedEventArgs e)
        {
            //CheckBox chkCountPatient = sender as CheckBox;
            //if (chkCountPatient == null || SelectedOutwardDrugClinicDept == null)
            //{
            //    return;
            //}
            //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).
            //KMx: Set property IsReadOnly của column = True sẽ giải quyết được lỗi này (21/09/2016 14:07).
            //if (SelectedOutwardDrugClinicDept.IsCountPatient != chkCountPatient.IsChecked)
            //{
            //    SelectedOutwardDrugClinicDept.IsCountPatient = chkCountPatient.IsChecked.GetValueOrDefault();
            //}

            SumTotalPrice();
        }


        public void chkCountHI_Click(object sender, RoutedEventArgs e)
        {
            //CheckBox chkCountHI = sender as CheckBox;
            //if (chkCountHI == null || SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null || SelectedOutwardDrugClinicDept == null || SelectedOutwardDrugClinicDept.GenMedProductItem == null)
            //{
            //    return;
            //}

            if (SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null || SelectedOutwardDrugClinicDept == null || SelectedOutwardDrugClinicDept.GenMedProductItem == null)
            {
                return;
            }

            string error = "";

            if (SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() <= 0)
            {
                error = eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT;
            }
            else if (!SelectedOutwardDrugClinicDept.GenMedProductItem.InsuranceCover.GetValueOrDefault())
            {
                error = eHCMSResources.Z1099_G1_LoaiDVKgThuocDMBH;
            }

            if (SelectedOutwardDrugClinicDept.IsCountHI && !string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                SelectedOutwardDrugClinicDept.IsCountHI = false;
                return;
            }

            if (SelectedOutwardDrugClinicDept.OutID > 0 && SelectedOutwardDrugClinicDept.GenMedProductItem.HIAllowedPriceNew == 0
                || SelectedOutwardDrugClinicDept.OutID <= 0 && SelectedOutwardDrugClinicDept.GenMedProductItem.HIAllowedPrice == 0)
            {
                MessageBox.Show(eHCMSResources.Z2193_G1_LoKhongCoGiaBaoHiem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                SelectedOutwardDrugClinicDept.IsCountHI = false;
                return;
            }

            //if (chkCountHI.IsChecked.GetValueOrDefault() && !string.IsNullOrEmpty(error))
            //{
            //    MessageBox.Show(error, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    chkCountHI.IsChecked = false;
            //    return;
            //}

            //KMx: Tránh trường hợp chưa bind mà đã gọi event, dẫn đến tính toán sai (09/12/2014 09:52).
            //KMx: Set property IsReadOnly của column = True sẽ giải quyết được lỗi này (21/09/2016 14:07).
            //if (SelectedOutwardDrugClinicDept.IsCountHI != chkCountHI.IsChecked)
            //{
            //    SelectedOutwardDrugClinicDept.IsCountHI = chkCountHI.IsChecked.GetValueOrDefault();
            //}

            SumTotalPrice();
        }

        public void chkAllCountPatient_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkAllCountPatient = sender as CheckBox;
            if (chkAllCountPatient == null)
            {
                return;
            }

            CountPatientOrNot(chkAllCountPatient.IsChecked.GetValueOrDefault());

            SumTotalPrice();
        }

        public void chkAllCountHI_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkAllCountHI = sender as CheckBox;
            if (chkAllCountHI == null || SelectedOutInvoice == null || SelectedOutInvoice.PatientRegistration == null)
            {
                return;
            }

            if (chkAllCountHI.IsChecked.GetValueOrDefault() && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() <= 0)
            {
                chkAllCountHI.IsChecked = false;
                MessageBox.Show(eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            CountHIOrNot(chkAllCountHI.IsChecked.GetValueOrDefault());

            SumTotalPrice();

        }

        private void CountPatientOrNot(bool value)
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null)
            {
                return;
            }

            foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                item.IsCountPatient = value;
            }

        }

        private void CountHIOrNot(bool value)
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null)
            {
                return;
            }

            foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
            {
                if (item.GenMedProductItem == null)
                {
                    continue;
                }

                if (value && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() > 0 && item.GenMedProductItem.InsuranceCover.GetValueOrDefault())
                {
                    item.IsCountHI = true;
                }
                else
                {
                    item.IsCountHI = false;
                }
            }
        }


        //private void CountHIOrNot(bool value)
        //{
        //    if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null)
        //    {
        //        return;
        //    }

        //    foreach (OutwardDrugClinicDept item in SelectedOutInvoice.OutwardDrugClinicDepts)
        //    {
        //        if (item.GenMedProductItem == null)
        //        {
        //            continue;
        //        }
        //        if (!item.GenMedProductItem.InsuranceCover.GetValueOrDefault())
        //        {
        //            item.IsCountHI = false;
        //            continue;
        //        }

        //        item.IsCountHI = value;
        //        NotifyOfPropertyChange(() => item.IsCountHI);
        //    }
        //}


        private ObservableCollection<OutwardDrugClinicDeptTemplate> _OutwardTemplateList;
        public ObservableCollection<OutwardDrugClinicDeptTemplate> OutwardTemplateList
        {
            get { return _OutwardTemplateList; }
            set
            {
                _OutwardTemplateList = value;
                NotifyOfPropertyChange(() => OutwardTemplateList);
            }
        }

        private OutwardDrugClinicDeptTemplate _SelectedOutwardTemplate;
        public OutwardDrugClinicDeptTemplate SelectedOutwardTemplate
        {
            get { return _SelectedOutwardTemplate; }
            set
            {
                _SelectedOutwardTemplate = value;
                NotifyOfPropertyChange(() => SelectedOutwardTemplate);
            }
        }

        public void GetAllOutwardTemplate(long DeptID)
        {
            if (DeptID <= 0)
            {
                OutwardTemplateList = new ObservableCollection<OutwardDrugClinicDeptTemplate>();
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllOutwardTemplate((long)V_MedProductType, DeptID, (long)AllLookupValues.V_OutwardTemplateType.OutwardTemplate, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<OutwardDrugClinicDeptTemplate> allItems = new ObservableCollection<OutwardDrugClinicDeptTemplate>();
                            try
                            {
                                allItems = contract.EndGetAllOutwardTemplate(asyncResult);

                                OutwardTemplateList = new ObservableCollection<OutwardDrugClinicDeptTemplate>(allItems);

                                OutwardDrugClinicDeptTemplate firstItem = new OutwardDrugClinicDeptTemplate();
                                firstItem.OutwardDrugClinicDeptTemplateID = -1;
                                firstItem.OutwardDrugClinicDeptTemplateName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
                                OutwardTemplateList.Insert(0, firstItem);

                                SelectedOutwardTemplate = OutwardTemplateList.FirstOrDefault();

                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                this.HideBusyIndicator();
                            }

                        }), null);
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
            });
            t.Start();
        }

        public void btnGetProductByOutwardTemplate()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            if (SelectedOutInvoice.OutDate == null)
            {
                MessageBox.Show(eHCMSResources.A0593_G1_Msg_InfoChonNgXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutInvoice.StoreID == null || SelectedOutInvoice.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0333_G1_ChonKhoXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) <= 0 && SelectedOutInvoice.OutPtRegistrationID == 0)
            {
                MessageBox.Show(eHCMSResources.Z1209_G1_ChonBNXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (SelectedOutwardTemplate == null || SelectedOutwardTemplate.OutwardDrugClinicDeptTemplateID < 0)
            {
                MessageBox.Show(eHCMSResources.A0590_G1_Msg_InfoChonMauXuat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            GetInBatchNumberAndPrice_ListForRequestClinicDept(false, null, SelectedOutwardTemplate.OutwardDrugClinicDeptTemplateID, SelectedOutInvoice.StoreID.GetValueOrDefault(), (long)V_MedProductType, SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(), true);
        }

        public void btnChangeDoctor(object sender, RoutedEventArgs e)
        {
            if (RequireDoctorAndDate && (aucHoldConsultDoctor == null || aucHoldConsultDoctor.StaffID <= 0))
            {
                MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0910_G1_Msg_InfoKhTheDoiBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.FromIntPtDiagDrInstructionIDCollection != null && SelectedOutInvoice.FromIntPtDiagDrInstructionIDCollection.Length > 0)
            {
                MessageBox.Show(eHCMSResources.Z2897_G1_KhongTheCapNhatYLCuaBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<OutwardDrugClinicDept> SelectOutwardDetails = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.IsChecked).ToObservableCollection();
            if (SelectOutwardDetails == null || SelectOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0595_G1_Msg_InfoChonDongCanDoiBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            foreach (OutwardDrugClinicDept item in SelectOutwardDetails)
            {
                if (item.DoctorStaff == null)
                {
                    item.DoctorStaff = new Staff();
                }
                item.DoctorStaff.StaffID = aucHoldConsultDoctor.StaffID;
                item.DoctorStaff.FullName = aucHoldConsultDoctor.StaffName;
            }
            MessageBox.Show(eHCMSResources.Z1149_G1_DoiBSiChiDinhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeMedicalInstructionDate(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.OutwardDrugClinicDepts.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0926_G1_Msg_InfoKhTheDoiNgYLenh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedOutInvoice.FromIntPtDiagDrInstructionIDCollection != null && SelectedOutInvoice.FromIntPtDiagDrInstructionIDCollection.Length > 0)
            {
                MessageBox.Show(eHCMSResources.Z2897_G1_KhongTheCapNhatYLCuaBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (!CheckValidMedicalInstructionDate())
            {
                return;
            }
            ObservableCollection<OutwardDrugClinicDept> SelectOutwardDetails = SelectedOutInvoice.OutwardDrugClinicDepts.Where(x => x.IsChecked).ToObservableCollection();
            if (SelectOutwardDetails == null || SelectOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0597_G1_Msg_InfoChonDongCanDoiNgYL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (OutwardDrugClinicDept item in SelectOutwardDetails)
            {
                item.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
            }

            MessageBox.Show(eHCMSResources.Z1150_G1_DoiNgYLenhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedSellVisitor, CalByUnitUse);
        }

        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedSellVisitor, CalByUnitUse);
        }

        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedSellVisitor, CalByUnitUse);
        }

        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedSellVisitor, CalByUnitUse);
        }

        //private bool _AllCountPatientChecked;
        //public bool AllCountPatientChecked
        //{
        //    get
        //    {
        //        return _AllCountPatientChecked;
        //    }
        //    set
        //    {
        //        if (_AllCountPatientChecked == value)
        //        {
        //            return;
        //        }
        //        _AllCountPatientChecked = value;

        //        CountPatientOrNot(_AllCountPatientChecked);

        //        SumTotalPrice();

        //        NotifyOfPropertyChange(() => AllCountPatientChecked);
        //    }
        //}

        //private bool _AllCountHIChecked;
        //public bool AllCountHIChecked
        //{
        //    get
        //    {
        //        return _AllCountHIChecked;
        //    }
        //    set
        //    {
        //        if (_AllCountHIChecked == value || SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugClinicDepts == null || SelectedOutInvoice.PatientRegistration == null)
        //        {
        //            return;
        //        }

        //        if (value == true && SelectedOutInvoice.PatientRegistration.PtInsuranceBenefit.GetValueOrDefault() <= 0)
        //        {
        //            MessageBox.Show(eHCMSResources.A0244_G1_Msg_InfoBNKhongCoBHYT, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //            return;
        //        }

        //        _AllCountHIChecked = value;

        //        CountHIOrNot(_AllCountHIChecked);

        //        SumTotalPrice();

        //        NotifyOfPropertyChange(() => AllCountHIChecked);
        //    }
        //}

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get => _V_RegistrationType; set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }
        private ObservableCollection<InPatientInstruction> _CurrentInstructionCollection;
        public ObservableCollection<InPatientInstruction> CurrentInstructionCollection
        {
            get
            {
                return _CurrentInstructionCollection;
            }
            set
            {
                if (_CurrentInstructionCollection == value)
                {
                    return;
                }
                _CurrentInstructionCollection = value;
                NotifyOfPropertyChange(() => CurrentInstructionCollection);
            }
        }
        private void LoadInPatientInstructionForRequest(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            if (CurrentInstructionCollection != null)
            {
                CurrentInstructionCollection.Clear();
            }
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionCollectionForCreateOutward(PtRegistrationID, false, (long)V_MedProductType, SelectedOutInvoice == null ? -1 : SelectedOutInvoice.StoreID.GetValueOrDefault(-1), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ItemCollection = contract.EndGetInPatientInstructionCollectionForCreateOutward(asyncResult);
                                if (ItemCollection != null)
                                {
                                    CurrentInstructionCollection = ItemCollection.ToObservableCollection();
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private bool _IsLoadTempByReqInvoice = Globals.ServerConfigSection.ClinicDeptElements.LoadOutwardTempBy != 1;
        public bool IsLoadTempByReqInvoice
        {
            get
            {
                return _IsLoadTempByReqInvoice;
            }
            set
            {
                if (_IsLoadTempByReqInvoice == value)
                {
                    return;
                }
                _IsLoadTempByReqInvoice = value;
                NotifyOfPropertyChange(() => IsLoadTempByReqInvoice);
            }
        }
        public void TCTempOutward_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoadTempByReqInvoice)
            {
                (sender as TabControl).SelectedItem = (sender as TabControl).Items.Cast<TabItem>().First(x => x.Name == "TIInstructionTemp");
            }
        }
    }
}
