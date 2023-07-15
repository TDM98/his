using eHCMSLanguage;
using System.ComponentModel;
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
using Service.Core.Common;
using aEMR.StoreDept.Views;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using System.Windows.Data;
using aEMR.Controls;
using System.Text;
using Castle.Windsor;
/*
 * 20170410 #001 CMN: Add Popup follow the result of patient drug using
 * 20180426 #002 CMN: Added CheckValidMedicalInstructionDate Functions
 * 20181006 #003 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc.
 * 20181115 #004 TBL: BM 0005256: Lam tron so luong YC dua tren cau hinh      
 * 20190106 #005 TTM: BM 0005478: Thêm label thể hiện status của phiếu lĩnh
 * 20190106 #006 TTM: BM 0005478: AutoComplete không nhảy lung tung khi type thuốc
 * 20190106 #007 TTM: BM 0005478: Fix lỗi không thay đổi slcd khi vừa gõ liều dùng mà click ra khỏi Grid
 * 20190110 #008 TTM:             Bổ sung thêm Số lượng tồn và thay đổi Query trong AutoComplete lập phiếu lĩnh giống như xuất khoa dược để người dùng biết chính xác khoa dược còn những gì để lập phiếu lĩnh
 * 20190313 #009 TTM: BM 0006635: Lấy danh sách V_MedProductType từ cấu hình để kiểm tra xem Product đó có cần bác sĩ chỉ định hay không.
 * 20190411 #010 TTM: BM 0006636: Cho phép khoa nội trú tìm thuốc bằng tên hoạt chất.
 * 20220106 #011 TNHX: 887 Thêm điều kiện tìm thuốc theo danh mục covid
 */
namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(IClinicDeptInPtReqForm_V3)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ClinicDeptInPtReqForm_V3ViewModel : Conductor<object>, IClinicDeptInPtReqForm_V3
        , IHandle<DrugDeptCloseSearchRequestEvent>
        , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
        , IHandle<DrugResultEvent>
    {
        //▼====: #011
        private bool _IsCOVID;
        public bool IsCOVID
        {
            get { return _IsCOVID; }
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
        //==== #001 ====
        private bool _IsResultView = false;
        public bool IsResultView
        {
            get { return _IsResultView; }
            set
            {
                if (_IsResultView != value)
                {
                    _IsResultView = value;
                    NotifyOfPropertyChange(() => IsResultView);
                }
            }
        }
        //==== #001 ====        
        private bool _DoseVisibility;
        public bool DoseVisibility
        {
            get { return _DoseVisibility; }
            set
            {
                if (_DoseVisibility != value)
                {
                    _DoseVisibility = value;
                    NotifyOfPropertyChange(() => DoseVisibility);
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
                }
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

        private Patient _currentPatient;
        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                if (_currentPatient != value)
                {
                    _currentPatient = value;
                    NotifyOfPropertyChange(() => CurrentPatient);
                }
            }
        }
        [ImportingConstructor]
        public ClinicDeptInPtReqForm_V3ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Authorization();

            //form tim kiem
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = false;
            IsStatusCoupon = false;
            //searchPatientAndRegVm.mThemBN = mDangKyNoiTru_Patient_ThemBN;
            //searchPatientAndRegVm.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;

            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            //Coroutine.BeginExecute(DoGetStore_All());
            Coroutine.BeginExecute(DoGetRefGenericDrugCategory_1List());

            SearchCriteria = new RequestSearchCriteria
            {

                // TxD 18/11/2014: Set default search Date From = 3 days for Now
                FromDate = DateTime.Now.Date - new TimeSpan(3, 0, 0, 0),
                ToDate = DateTime.Now
            };

            ListRequestDrugDelete = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug = new RequestDrugInwardClinicDept();
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();


            RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
            //RefGenMedProductDetails.OnRefresh += RefGenMedProductDetails_OnRefresh;
            //RefGenMedProductDetails.PageSize = Globals.PageSize;

            ListStaff = new ObservableCollection<Staff>();
            GetListStaffFilter();

            MedicalInstructionDate = Globals.GetCurServerDateTime();

            aucHoldConsultDoctor = Globals.GetViewModel<IAucHoldConsultDoctor>();
            aucHoldConsultDoctor.StaffCatType = (long)V_StaffCatType.BacSi;
            iFlag = true;
        }
        private bool iFlag = false;
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (iFlag)
            {
                //var homevm = Globals.GetViewModel<IHome>();
                //ostvm = Globals.GetViewModel<ICheckStatusRequestInvoiceOutStandingTask>();
                //ostvm.V_MedProductType = V_MedProductType;
                //ostvm.LoadStore();
                //homevm.OutstandingTaskContent = ostvm;
                GetOutstandingTaskContent();
            }
            StoreCbxStaff = Globals.checkStoreWareHouse((long)AllLookupValues.MedProductType.THUOC, false, false);
            if (StoreCbxStaff == null || StoreCbxStaff.Count < 1)
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
            }
            Coroutine.BeginExecute(DoGetStore_MedDept());
            RefeshRequest();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (iFlag)
            {
                Globals.EventAggregator.Unsubscribe(this);
                var homevm = Globals.GetViewModel<IHome>();
                homevm.OutstandingTaskContent = null;
                homevm.IsExpandOST = false;
                iFlag = false;
            }
        }

        private long _StaffDetailID;
        public long StaffDetailID
        {
            get { return _StaffDetailID; }
            set
            {
                if (_StaffDetailID != value)
                {
                    _StaffDetailID = value;
                    NotifyOfPropertyChange(() => StaffDetailID);
                }
            }
        }

        void RefGenMedProductDetails_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        {
            //GetRefGenMedProductDetails_Auto(BrandName, false, RefGenMedProductDetails.PageIndex, RefGenMedProductDetails.PageSize);
            SearchRefGenMedProductDetails(BrandName, false);
        }

        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        private bool _mPhieuYeuCau_Tim = true;
        private bool _mPhieuYeuCau_Them = true;
        private bool _mPhieuYeuCau_Xoa = true;
        private bool _mPhieuYeuCau_XemIn = true;
        private bool _mPhieuYeuCau_In = true;

        public bool mPhieuYeuCau_Tim
        {
            get
            {
                return _mPhieuYeuCau_Tim;
            }
            set
            {
                if (_mPhieuYeuCau_Tim == value)
                    return;
                _mPhieuYeuCau_Tim = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Tim);
            }
        }

        public bool mPhieuYeuCau_Them
        {
            get
            {
                return _mPhieuYeuCau_Them;
            }
            set
            {
                if (_mPhieuYeuCau_Them == value)
                    return;
                _mPhieuYeuCau_Them = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Them);
            }
        }

        public bool mPhieuYeuCau_Xoa
        {
            get
            {
                return _mPhieuYeuCau_Xoa;
            }
            set
            {
                if (_mPhieuYeuCau_Xoa == value)
                    return;
                _mPhieuYeuCau_Xoa = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_Xoa);
            }
        }

        public bool mPhieuYeuCau_XemIn
        {
            get
            {
                return _mPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPhieuYeuCau_XemIn == value)
                    return;
                _mPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_XemIn);
            }
        }

        public bool mPhieuYeuCau_In
        {
            get
            {
                return _mPhieuYeuCau_In;
            }
            set
            {
                if (_mPhieuYeuCau_In == value)
                    return;
                _mPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPhieuYeuCau_In);
            }
        }

        #endregion

        #region 1. Property

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

        private DateTime? _MedicalInstructionDate;
        public DateTime? MedicalInstructionDate
        {
            get
            {
                return _MedicalInstructionDate;
            }
            set
            {
                _MedicalInstructionDate = value;
                NotifyOfPropertyChange(() => MedicalInstructionDate);
            }
        }

        private string _strNote;
        public string strNote
        {
            get
            {
                return _strNote;
            }
            set
            {
                _strNote = value;
                NotifyOfPropertyChange(() => strNote);
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

        private RefGenMedProductDetails _SelectRefGenMedProductDetail;
        public RefGenMedProductDetails SelectRefGenMedProductDetail
        {
            get
            {
                return _SelectRefGenMedProductDetail;
            }
            set
            {
                if (_SelectRefGenMedProductDetail != value)
                {
                    _SelectRefGenMedProductDetail = value;
                    NotifyOfPropertyChange(() => SelectRefGenMedProductDetail);
                }
            }
        }

        //private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetails;
        //public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetails
        //{
        //    get
        //    {
        //        return _RefGenMedProductDetails;
        //    }
        //    set
        //    {
        //        if (_RefGenMedProductDetails != value)
        //        {
        //            _RefGenMedProductDetails = value;
        //            NotifyOfPropertyChange(() => RefGenMedProductDetails);
        //        }
        //    }
        //}
        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetails;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetails
        {
            get
            {
                return _RefGenMedProductDetails;
            }
            set
            {
                if (_RefGenMedProductDetails != value)
                {
                    _RefGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetails);
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

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbxStaff;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbxStaff
        {
            get
            {
                return _StoreCbxStaff;
            }
            set
            {
                if (_StoreCbxStaff != value)
                {
                    _StoreCbxStaff = value;
                    NotifyOfPropertyChange(() => StoreCbxStaff);
                }
            }
        }

        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ListRequestDrugDelete;

        private RequestDrugInwardClinicDept _RequestDrug;
        public RequestDrugInwardClinicDept RequestDrug
        {
            get
            {
                return _RequestDrug;
            }
            set
            {
                if (_RequestDrug != value)
                {
                    _RequestDrug = value;
                    NotifyOfPropertyChange(() => RequestDrug);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _SelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient SelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _SelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_SelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _SelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => SelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        //private PagedCollectionView _ReqOutwardDrugClinicDeptPatientlst;
        //public PagedCollectionView ReqOutwardDrugClinicDeptPatientlst
        //{
        //    get { return _ReqOutwardDrugClinicDeptPatientlst; }
        //    set
        //    {
        //        if (_ReqOutwardDrugClinicDeptPatientlst != value)
        //        {
        //            _ReqOutwardDrugClinicDeptPatientlst = value;
        //            NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientlst);
        //        }
        //    }
        //}

        public CollectionView CollectionView_ReqDetails { get; set; }

        private CollectionViewSource _cvs_ReqDetails = null;
        public CollectionViewSource CVS_ReqDetails
        {
            get { return _cvs_ReqDetails; }
            set
            {
                _cvs_ReqDetails = value;
                ControlStatusCoupon();
            }
        }

        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
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
                    DoseVisibility = V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;
                    NotifyOfPropertyChange(() => DoseVisibility);
                    vIsSearchByGenericName = V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                    UsedForRequestingDrug = V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;
                    NotifyOfPropertyChange(() => UsedForRequestingDrug);
                    StoreCbxStaff = Globals.checkStoreWareHouse(V_MedProductType, false, false);
                    if (StoreCbxStaff == null || StoreCbxStaff.Count < 1)
                    {
                        MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho);
                    }
                    else
                    {
                        RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
                    }
                    Coroutine.BeginExecute(DoGetStore_MedDept());
                }
            }
        }

        private ObservableCollection<Lookup> _gMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
        public ObservableCollection<Lookup> gMedProductTypeCollection
        {
            get
            {
                return _gMedProductTypeCollection;
            }
            set
            {
                _gMedProductTypeCollection = value;
                NotifyOfPropertyChange(() => gMedProductTypeCollection);
            }
        }

        private ICheckStatusRequestInvoiceOutStandingTask _ostvm;
        public ICheckStatusRequestInvoiceOutStandingTask ostvm
        {
            get
            {
                return _ostvm;
            }
            set
            {
                if (_ostvm != value)
                {
                    _ostvm = value;
                    NotifyOfPropertyChange(() => ostvm);
                }
            }
        }

        //KMx: Không sử dụng Radio Button Code - Tên nữa (04/02/2016 11:52).
        //private bool _VisibilityName = true;
        //public bool VisibilityName
        //{
        //    get
        //    {

        //        return _VisibilityName;
        //    }
        //    set
        //    {
        //        _VisibilityName = value;
        //        _VisibilityCode = !_VisibilityName;
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
        #endregion

        #region 3. Function Member

        private void GetOutstandingTaskContent()
        {
            var homevm = Globals.GetViewModel<IHome>();
            ostvm = Globals.GetViewModel<ICheckStatusRequestInvoiceOutStandingTask>();
            ostvm.V_MedProductType = V_MedProductType;
            ostvm.LoadStore();
            homevm.OutstandingTaskContent = ostvm;
            homevm.IsExpandOST = true;
        }

        private void GetListStaffFilter()
        {
            if (ListStaff != null)
            {
                ListStaff.Clear();
                Staff item = new Staff
                {
                    StaffID = 0,
                    FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa)
                };
                ListStaff.Add(item);
                ListStaff.Add(GetStaffLogin());
            }

            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                var lst = (from e in RequestDrug.ReqOutwardDetails
                           select new { e.StaffID, e.StaffName }).Distinct();
                foreach (var staffItem in lst)
                {
                    if (staffItem.StaffID != GetStaffLogin().StaffID)
                    {
                        ListStaff.Add(new Staff { StaffID = staffItem.StaffID.GetValueOrDefault(0), FullName = staffItem.StaffName });
                    }
                }
            }
            StaffDetailID = ListStaff.FirstOrDefault().StaffID; //GetStaffLogin().StaffID;
        }

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
            if (RequestDrug != null && RefGenericDrugCategory_1s != null)
            {
                RequestDrug.RefGenDrugCatID_1 = RefGenericDrugCategory_1s.FirstOrDefault().RefGenDrugCatID_1;
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void ischanged(object item)
        {
            ReqOutwardDrugClinicDeptPatient p = item as ReqOutwardDrugClinicDeptPatient;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }

        private bool CheckValidationEditor()
        {
            bool result = true;
            StringBuilder st = new StringBuilder();
            if (RequestDrug != null)
            {
                if (RequestDrug.ReqOutwardDetails != null)
                {
                    int nIdx = 0;
                    foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                    {
                        nIdx++;
                        if (item.GenMedProductID != null && item.GenMedProductID != 0)
                        {
                            if (item.ReqQty < 0)
                            {
                                string strErr = string.Format("Dữ liệu dòng số ({0}): [{1}] Số lượng yêu cầu phải >= 0!", item.DisplayGridRowNumber.ToString(), item.RefGenericDrugDetail.BrandNameAndCode);
                                st.AppendLine(strErr);
                                result = false;
                            }
                            if (item.PrescribedQty <= 0)
                            {
                                string strErr = string.Format("Dữ liệu dòng số ({0}): [{1}] Số lượng Chỉ Định phải > 0!", item.DisplayGridRowNumber.ToString(), item.RefGenericDrugDetail.BrandNameAndCode);
                                st.AppendLine(strErr);
                                result = false;
                            }
                        }
                    }
                }
                if (!result)
                {
                    MessageBox.Show(st.ToString());
                }
            }
            return result;
        }

        private void FullOperatorRequestDrugInwardClinicDept()
        {
            // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
            //                  It helps the user to identify if a row has been verified against the doctor's request document
            if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    if (item.ItemVerified)
                    {
                        item.ItemVerfStat = 1;
                    }
                    else
                    {
                        item.ItemVerfStat = 0;
                    }
                }
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginFullOperatorRequestDrugInwardClinicDeptNew(RequestDrug, V_MedProductType,(long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                RequestDrugInwardClinicDept RequestOut;
                                contract.EndFullOperatorRequestDrugInwardClinicDeptNew(out RequestOut, asyncResult);
                                if (RequestOut != null)
                                {
                                    RequestDrug = RequestOut;
                                    RequestDrug.ReqOutwardDetails = RequestOut.ReqOutwardDetails.DeepCopy();

                                    // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
                                    //                  It helps the user to identify if a row has been verified against the doctor's request document
                                    if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
                                    {
                                        foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                                        {
                                            item.ItemVerified = false;
                                            if (item.ItemVerfStat == 1)
                                            {
                                                item.ItemVerified = true;
                                            }
                                        }
                                    }
                                }

                                ListRequestDrugDelete.Clear();
                                //FilterByStaff();
                                FillPagedCollectionAndGroup();

                                SetCheckAllReqOutwardDetail();

                                Globals.ShowMessage(eHCMSResources.Z1562_G1_DaLuu, eHCMSResources.G0442_G1_TBao);
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

        private void SaveRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfLuuThayDoiTrenPhYC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            SaveFullOp();
        }

        private bool ValidateDataOneLastTimeBeforeSaving()
        {
            if (RequestDrug.OutFromStoreObject == null || RequestDrug.OutFromStoreObject.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0318_G1_ChonKhoCC);
                return false;
            }
            if (RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count == 0)
            {
                MessageBox.Show(eHCMSResources.K0422_G1_NhapCTietPhYC);
                return false;
            }
            else
            {
                if (RequestDrug.ReqOutwardDetails.Count == 1)
                {
                    if (RequestDrug.ReqOutwardDetails[0].RefGenericDrugDetail == null)
                    {
                        MessageBox.Show(eHCMSResources.K0303_G1_ChonHgYC);
                        return false;
                    }
                }

                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail != null)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng Chỉ Định phải > 0";
                            MessageBox.Show(strErr);
                            return false;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].EntityState != EntityState.DETACHED && RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            string strErr = "Dữ liệu dòng số (" + RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString() + ") : [" + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + "] Số lượng yêu cầu phải >= 0";
                            MessageBox.Show(strErr);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private void SaveFullOp()
        {
            if (ListRequestDrugDelete.Count > 0)
            {
                if (RequestDrug.ReqOutwardDetails == null)
                {
                    RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (ReqOutwardDrugClinicDeptPatient item in ListRequestDrugDelete)
                {
                    RequestDrug.ReqOutwardDetails.Add(item);
                }
            }

            if (ValidateDataOneLastTimeBeforeSaving())
            {
                FullOperatorRequestDrugInwardClinicDept();
            }
        }

        private void RemoveRowBlank()
        {
            try
            {
                int idx = RequestDrug.ReqOutwardDetails.Count;
                if (idx > 0)
                {
                    idx--;
                    ReqOutwardDrugClinicDeptPatient obj = (ReqOutwardDrugClinicDeptPatient)RequestDrug.ReqOutwardDetails[idx];
                    if (obj.GenMedProductID == null || obj.GenMedProductID == 0)
                    {
                        RequestDrug.ReqOutwardDetails.RemoveAt(idx);
                    }
                }
                NotifyOfPropertyChange(() => RequestDrug);
            }
            catch
            { }
        }

        private void RemoveMark(object item)
        {
            ReqOutwardDrugClinicDeptPatient obj = item as ReqOutwardDrugClinicDeptPatient;
            if (obj != null)
            {
                RequestDrug.ReqOutwardDetails.Remove(obj);
                if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                {
                    obj.EntityState = EntityState.DETACHED;
                    ListRequestDrugDelete.Add(obj);
                }
            }
        }

        public void OpenPopUpSearchRequestInvoice(IList<RequestDrugInwardClinicDept> results, int Totalcount, bool bCreateNewListFromOld)
        {
            void onInitDlg(IStoreDeptRequestSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.RequestDruglist.Clear();
                proAlloc.RequestDruglist.TotalItemCount = Totalcount;
                proAlloc.RequestDruglist.PageIndex = 0;
                proAlloc.RequestDruglist.PageSize = 20;
                proAlloc.IsCreateNewListFromSelectExisting = bCreateNewListFromOld;

                if (results != null && results.Count > 0)
                {
                    foreach (RequestDrugInwardClinicDept p in results)
                    {
                        proAlloc.RequestDruglist.Add(p);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IStoreDeptRequestSearch>(onInitDlg);
        }

        private void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize, bool bCreateNewListFromOld = false)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            // TxD 23/12/2014: Filter search of requests by the requesting 
            if (RequestDrug != null && RequestDrug.InDeptStoreID > 0)
            {
                SearchCriteria.RequestStoreID = RequestDrug.InDeptStoreID;
            }
            else
            {
                if (StoreCbxStaff.Count() > 0)
                {
                    SearchCriteria.RequestStoreID = StoreCbxStaff[1].StoreID;
                }
            }

            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        this.HideBusyIndicator();
                                        OpenPopUpSearchRequestInvoice(results.ToList(), Total, bCreateNewListFromOld);
                                    }
                                    else
                                    {
                                        this.HideBusyIndicator();
                                        if (RequestDrug != null)
                                        {
                                            ChangeValue(RequestDrug.RefGenDrugCatID_1, results.FirstOrDefault().RefGenDrugCatID_1);
                                        }
                                        RequestDrug = results.FirstOrDefault();
                                        ListRequestDrugDelete.Clear();

                                        GetInPatientRequestingDrugListByReqID(RequestDrug.ReqDrugInClinicDeptID, bCreateNewListFromOld);
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        this.HideBusyIndicator();
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchRequestInvoice(null, 0, bCreateNewListFromOld);
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

        //private ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicDeptPatientlstCopy;

        private void GetInPatientRequestingDrugListByReqID(long RequestID, bool bCreateNewListFromOld = false)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReqOutwardDrugClinicDeptPatientByID(RequestID, bCreateNewListFromOld, (long)AllLookupValues.RegistrationType.NOI_TRU
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetReqOutwardDrugClinicDeptPatientByID(asyncResult);
                                if (results != null)
                                {
                                    RequestDrug.ReqOutwardDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (RequestDrug.ReqOutwardDetails != null)
                                    {
                                        RequestDrug.ReqOutwardDetails.Clear();
                                    }
                                }
                                if (RequestDrug == null)
                                {
                                    RequestDrug = new RequestDrugInwardClinicDept();
                                }

                                // TxD 28/04/2015: The following line looks very bogus, so comment it out for now, if something happend then we need to REVIEW
                                //RequestDrug.ReqOutwardDetails = RequestDrug.ReqOutwardDetails.DeepCopy();

                                GetListStaffFilter();
                                FillPagedCollectionAndGroup();
                                if (bCreateNewListFromOld)
                                {
                                    ResetingOldListToCreateNewList(bCreateNewListFromOld);
                                }
                                else
                                {
                                    // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
                                    //                  It helps the user to identify if a row has been verified against the doctor's request document
                                    if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
                                    {
                                        foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                                        {
                                            item.ItemVerified = false;
                                            if (item.ItemVerfStat == 1)
                                            {
                                                item.ItemVerified = true;
                                            }
                                        }
                                    }
                                }

                                SetCheckAllReqOutwardDetail();
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

        private void ResetingOldListToCreateNewList(bool bCreateNewListFromOld = false, bool IsLoadForInstruction = false)
        {
            CurrentPatient = null;
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            if (!IsLoadForInstruction)
            {
                RequestDrug.ReqDate = DateTime.Now;
                RequestDrug.FromDate = DateTime.Now;
                RequestDrug.ToDate = DateTime.Now;
            }
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;
            ListRequestDrugDelete.Clear();

            // TxD 14/01/2015: Commented out the following to allow Staff filtering to work properly
            //ReqOutwardDrugClinicDeptPatientlstCopy = null;

            if (!bCreateNewListFromOld && StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            //KMx: Không set lại, để user khỏi phải chọn lại (06/12/2014 14:43).
            //if (StoreCbx != null)
            //{
            //    RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            //}

            // TxD28/04/2015: Added the following verification checkbox to aid in the process of creating a new list based on an existing one
            //                  It helps the user to identify if a row has been verified against the doctor's request document
            if (RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.EntityState = EntityState.NEW;
                    item.ItemVerfStat = 0;
                }
            }

            //KMx: Không tự động set Category, vì khi set Category thì sẽ tự động gọi hàm KeyEnabledComboBox_SelectionChanged(), dẫn đến thuốc trong grid bị clear hết (06/12/2014 14:32).
            //SetDefultRefGenericDrugCategory();
        }

        private void RefeshRequest()
        {
            CurrentPatient = null;
            RequestDrug.ReqDrugInClinicDeptID = 0;
            RequestDrug.ReqNumCode = "";
            RequestDrug.Comment = "";
            RequestDrug.ReqDate = DateTime.Now;
            RequestDrug.FromDate = DateTime.Now;
            RequestDrug.ToDate = DateTime.Now;
            RequestDrug.SelectedStaff = GetStaffLogin();
            RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            RequestDrug.DaNhanHang = false;
            RequestDrug.IsApproved = false;
            StatusCoupon = "";
            IsStatusCoupon = false;
            ListRequestDrugDelete.Clear();

            if (StoreCbxStaff != null)
            {
                RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            SetDefultRefGenericDrugCategory();
            FillPagedCollectionAndGroup();
        }

        private void DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteRequestDrugInwardClinicDept(ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDeleteRequestDrugInwardClinicDept(asyncResult);
                                RefeshRequest();

                                SetCheckAllReqOutwardDetail();
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

        private void DeleteRequest()
        {
            if (RequestDrug.ReqDrugInClinicDeptID > 0)
            {
                DeleteRequestDrugInwardClinicDept(RequestDrug.ReqDrugInClinicDeptID);
            }
        }

        private bool CheckDeleted(object item)
        {
            ReqOutwardDrugClinicDeptPatient temp = item as ReqOutwardDrugClinicDeptPatient;
            if (temp != null && temp.EntityState == EntityState.DETACHED)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckValidGrid()
        {
            bool results = true;
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return false;
            }
            if (RequestDrug.OutFromStoreID == null || RequestDrug.OutFromStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1130_G1_ChonKhoCCap, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrug.InDeptStoreID == RequestDrug.OutFromStoreID)
            {
                Globals.ShowMessage(eHCMSResources.Z1131_G1_KhoYCKhoCCKgDcTrung, eHCMSResources.T0074_G1_I);
                return false;
            }

            if (RequestDrug.ReqOutwardDetails != null)
            {
                if (RequestDrug.ReqOutwardDetails.Count == 0)
                {
                    if (RequestDrug.ReqDrugInClinicDeptID > 0)
                    {
                        if (MessageBox.Show(eHCMSResources.A0922_G1_Msg_ConfXoaPhRong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            DeleteRequest();
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.K0443_G1_NhapSLggYC);
                        return false;
                    }
                }
                string error = "";
                for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                {
                    if (RequestDrug.ReqOutwardDetails[i].GenMedProductID > 0)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].PrescribedQty <= 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1775_G1_SLgChiDinhLonHon0, RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (RequestDrug.ReqOutwardDetails[i].ReqQty < 0)
                        {
                            results = false;
                            MessageBox.Show(string.Format(eHCMSResources.Z1124_G1_SLgYCLonHonBang0, RequestDrug.ReqOutwardDetails[i].DisplayGridRowNumber.ToString(), RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode));
                            break;
                        }
                        if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0)
                        {
                            if ((RequestDrug.ReqOutwardDetails[i].MedicalInstructionDate.GetValueOrDefault() - Globals.GetCurServerDateTime().Date).Days > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate)
                            {
                                error += "    " + RequestDrug.ReqOutwardDetails[i].RefGenericDrugDetail.BrandNameAndCode + Environment.NewLine;
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(error))
                {
                    string msg = Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate == 0
                        ? string.Format(eHCMSResources.Z1875_G1_NgYLenhKgLonHonNgHTai2, error)
                        : string.Format(eHCMSResources.Z1874_G1_NgYLenhKgLonHonNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate, error);
                    MessageBox.Show(msg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return results;
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).OrderByDescending(x => x.StoreID).ToObservableCollection();
            if (RequestDrug != null && StoreCbx != null)
            {
                RequestDrug.OutFromStoreObject = StoreCbx.FirstOrDefault();
            }
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_All()
        {
            var paymentTypeTask = new LoadStoreListTask(null, false, null, false, true);
            yield return paymentTypeTask;
            StoreCbxStaff = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetStorageByStaffID(long StaffID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetStoragesByStaffID(StaffID, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetStoragesByStaffID(asyncResult);
                                if (results != null)
                                {
                                    StoreCbxStaff = results.ToObservableCollection();
                                    if (RequestDrug != null)
                                    {
                                        RequestDrug.InDeptStoreObject = StoreCbxStaff.FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    if (StoreCbxStaff != null)
                                    {
                                        StoreCbxStaff.Clear();
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

        #endregion

        #region client member

        #region AutoGenMedProduct Member

        TextBox tbxQty;
        public void tbxQty_Loaded(object sender, RoutedEventArgs e)
        {
            tbxQty = sender as TextBox;
        }

        TextBox tbxMDoseStr;
        public void tbxMDoseStr_Loaded(object sender, RoutedEventArgs e)
        {
            tbxMDoseStr = sender as TextBox;
        }

        private string BrandName;

        //private void GetRefGenMedProductDetails_Auto(string BrandName, bool IsCode, int PageIndex, int PageSize)
        //{
        //    if (IsCode == false && BrandName.Length < 1)
        //    {
        //        return;
        //    }

        //    IsFocusTextCode = IsCode;

        //    long? RefGenDrugCatID_1 = null;
        //    if (RequestDrug != null)
        //    {
        //        RefGenDrugCatID_1 = RequestDrug.RefGenDrugCatID_1;
        //    }
        //    //IsLoading = true;
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginRefGenMedProductDetails_SearchAutoPaging(IsCode, BrandName, null, V_MedProductType, RefGenDrugCatID_1, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    int Total;
        //                    var results = contract.EndRefGenMedProductDetails_SearchAutoPaging(out Total, asyncResult);
        //                    //KMx: Không sử dụng Radio Button Code - Tên nữa (04/02/2016 11:52).
        //                    //if (IsCode.GetValueOrDefault())
        //                    if (IsCode)
        //                    {
        //                        if (results != null && results.Count > 0)
        //                        {
        //                            if (CurrentReqOutwardDrugClinicDeptPatient == null)
        //                            {
        //                                CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
        //                            }
        //                            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();

        //                            if (IsInputDosage)
        //                            {
        //                                if (tbxMDoseStr == null)
        //                                {
        //                                    return;
        //                                }
        //                                tbxMDoseStr.Focus();
        //                            }
        //                            else
        //                            {
        //                                if (tbxQty == null)
        //                                {
        //                                    return;
        //                                }
        //                                tbxQty.Focus();
        //                            }


        //                        }
        //                        else
        //                        {
        //                            if (CurrentReqOutwardDrugClinicDeptPatient != null)
        //                            {
        //                                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = null;
        //                            }

        //                            if (tbx != null)
        //                            {
        //                                txt = "";
        //                                tbx.Text = "";
        //                                tbx.Focus();
        //                            }
        //                            if (acbAutoDrug_Text != null)
        //                            {
        //                                acbAutoDrug_Text.Text = "";
        //                            }

        //                            MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        RefGenMedProductDetails.Clear();
        //                        RefGenMedProductDetails.TotalItemCount = Total;
        //                        RefGenMedProductDetails.SourceCollection = results;
        //                        //foreach (RefGenMedProductDetails p in results)
        //                        //{
        //                        //    RefGenMedProductDetails.Add(p);
        //                        //}
        //                        acbAutoDrug_Text.PopulateComplete();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //IsLoading = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}



        #endregion

        private bool _CheckUnVerifiedRows = false;
        public bool CheckUnVerifiedRows
        {
            get { return _CheckUnVerifiedRows; }
            set
            {
                _CheckUnVerifiedRows = value;
                NotifyOfPropertyChange(() => CheckUnVerifiedRows);
            }
        }

        DataGrid grdReqOutwardDetails = null;
        public void grdReqOutwardDetails_Loaded(object sender, RoutedEventArgs e)
        {
            grdReqOutwardDetails = sender as DataGrid;

            if (grdReqOutwardDetails == null)
            {
                return;
            }

            var colMDoseStr = grdReqOutwardDetails.GetColumnByName("colMDoseStr");
            var colADoseStr = grdReqOutwardDetails.GetColumnByName("colADoseStr");
            var colEDoseStr = grdReqOutwardDetails.GetColumnByName("colEDoseStr");
            var colNDoseStr = grdReqOutwardDetails.GetColumnByName("colNDoseStr");

            if (colMDoseStr == null || colADoseStr == null || colEDoseStr == null || colNDoseStr == null)
            {
                return;
            }

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
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

        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReqOutwardDrugClinicDeptPatient rowItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;

            // TxD 04/07/2018
            if (rowItem == null)
            {
                return;
            }

            rowItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";

            // TxD 20/04/2015: No need to do this anymore becuase when we save those deleted row will be added back into the main collection 
            //                  causing a flash of red painted row(s) on the screen of those rows that were deleted.
            //if (CheckDeleted(e.Row.DataContext))
            //{
            //    e.Row.Background = new SolidColorBrush(Colors.Red);
            //}

            if (CheckUnVerifiedRows)
            {
                if (!rowItem.ItemVerified)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    e.Row.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        public void ItemVerify_Checked(object source, object dataCtx)
        {
            if (dataCtx != null)
            {
                ischanged(dataCtx);
            }
        }

        public void ItemVerify_UnChecked(object source, object dataCtx)
        {
            if (dataCtx != null)
            {
                ischanged(dataCtx);
            }
        }

        //public void grdReqOutwardDetails_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        //{
        //    if ((sender as DataGrid).SelectedItem != null)
        //    {
        //        ischanged((sender as DataGrid).SelectedItem);
        //    }
        //    // TxD 03/04/2015 if PrescribedQty is changed then ReqQty will be changed AS WELL (As requested by Khoa A & NTM)
        //    if (e.Column.DisplayIndex == 5)
        //    {
        //        if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
        //        {
        //            ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
        //            selItem.ReqQty = selItem.PrescribedQty;
        //        }
        //    }
        //}

        //public void grdReqOutwardDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if ((sender as DataGrid).SelectedItem != null)
        //    {
        //        ischanged((sender as DataGrid).SelectedItem);
        //    }
        //    if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colMDoseStr")
        //    {
        //        Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedReqOutwardDrugClinicDeptPatient);
        //    }
        //    else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colADoseStr")
        //    {
        //        Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedReqOutwardDrugClinicDeptPatient);
        //    }
        //    else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colEDoseStr")
        //    {
        //        Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedReqOutwardDrugClinicDeptPatient);
        //    }
        //    else if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colNDoseStr")
        //    {
        //        Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedReqOutwardDrugClinicDeptPatient);
        //    }
        //    // TxD 03/04/2015 if PrescribedQty is changed then ReqQty will be changed AS WELL (As requested by Khoa A & NTM)
        //    else if(e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colPrescribedQty")
        //    {
        //        if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
        //        {
        //            ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
        //            selItem.ReqQty = selItem.PrescribedQty;
        //        }
        //    }
        //}

        private enum QtyColumnIndexes
        {
            M_Dose_Idx = 5,
            A_Dose_Idx = 6,
            E_Dose_Idx = 7,
            N_Dose_Idx = 8,
            Pres_Qty_Idx = 9,
            Req_Qty_Idx = 10
        };
        // TxD 13/07/2018: The previously edited Column       
        DataGridColumn prevWorkingColumn = null;

        public void grdReqOutwardDetails_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (prevWorkingColumn == null)
            {
                prevWorkingColumn = ((DataGrid)sender).CurrentColumn;
                return;
            }

            int prevIdx = prevWorkingColumn.DisplayIndex;
            //20190106 TTM: Vì khi click ra khỏi grid nên selected bị set về null nên cần phải gán giá trị
            //              trước đó của nó khi focus vào cell (cell trước khi click ra ngoài)
            if (SelectedReqOutwardDrugClinicDeptPatient == null)
            {
                SelectedReqOutwardDrugClinicDeptPatient = copySelectedReqOutwardDrugClinicDeptPatient;
            }

            if ((sender as DataGrid).SelectedItem != null)
            {
                ischanged((sender as DataGrid).SelectedItem);
            }
            if (prevIdx == (int)QtyColumnIndexes.M_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.A_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.E_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.N_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            // TxD 03/04/2015 if PrescribedQty is changed then ReqQty will be changed AS WELL (As requested by Khoa A & NTM)
            else if (prevIdx == (int)QtyColumnIndexes.Pres_Qty_Idx)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = selItem.PrescribedQty;
                }
            }
            else if (prevIdx == (int)QtyColumnIndexes.Req_Qty_Idx)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && (sender as DataGrid).SelectedIndex != -1)
                {
                    ReqOutwardDrugClinicDeptPatient selItem = RequestDrug.ReqOutwardDetails[(sender as DataGrid).SelectedIndex];
                    selItem.ReqQty = Globals.ChangeDoseStringToDecimal(selItem.ReqQtyStr);
                }
            }
            //==== #004 ====
            ConvertCeiling(SelectedReqOutwardDrugClinicDeptPatient);
            //==== #004 ====
            prevWorkingColumn = ((DataGrid)sender).CurrentColumn;

        }

        //==== #004 ====
        private void ConvertCeiling(ReqOutwardDrugClinicDeptPatient req)
        {
            if (req == null)
            {
                return;
            }
            if (Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                req.ReqQty = Math.Ceiling(req.ReqQty);
            }
            req.ReqQtyStr = req.ReqQty.ToString();
        }
        //==== #004 ====

        public void grdReqOutwardDetails_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (!CheckValidationEditor())
            {
                // TxD 26/03/2015: get rid of the following because if this Event is Cancel the the Row is still in Edit mode thus prevent the grid from calling Refresh
                //                  and at thie moment there is no need to cancel unless anything comes up later then review                                
                //e.Cancel = true;
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (((ClinicDeptInPtReqForm_V3View)this.GetView()).grdReqOutwardDetails != null)
            {
                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RemoveMark(((ClinicDeptInPtReqForm_V3View)this.GetView()).grdReqOutwardDetails.SelectedItem);

                    SetCheckAllReqOutwardDetail();
                }
            }
        }

        public DataGridRow GetDataGridRowByDataContext(DataGrid dataGrid, object dataContext)
        {
            //ChangedWPF-CMN
            //if (null != dataContext)
            //{
            //    dataGrid.ScrollIntoView(dataContext, null);

            //    System.Windows.Automation.Peers.DataGridAutomationPeer automationPeer = (System.Windows.Automation.Peers.DataGridAutomationPeer)System.Windows.Automation.Peers.DataGridAutomationPeer.CreatePeerForElement(dataGrid);

            //    // Get the DataGridRowsPresenterAutomationPeer so we can find the rows in the data grid...
            //    System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer dataGridRowsPresenterAutomationPeer = automationPeer.GetChildren().
            //        Where(a => (a is System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
            //        Select(a => (a as System.Windows.Automation.Peers.DataGridRowsPresenterAutomationPeer)).
            //        FirstOrDefault();

            //    if (null != dataGridRowsPresenterAutomationPeer)
            //    {
            //        foreach (var item in dataGridRowsPresenterAutomationPeer.GetChildren())
            //        {
            //            // loop to find the DataGridCellAutomationPeer from which we can interrogate the owner -- which is a DataGridRow
            //            foreach (var subitem in (item as System.Windows.Automation.Peers.DataGridItemAutomationPeer).GetChildren())
            //            {
            //                if ((subitem is System.Windows.Automation.Peers.DataGridCellAutomationPeer))
            //                {
            //                    // At last -- the only public method for finding a row....
            //                    DataGridRow row = DataGridRow.GetRowContainingElement(((subitem as System.Windows.Automation.Peers.DataGridCellAutomationPeer).Owner as FrameworkElement));

            //                    // check this row to see if it is bound to the requested dataContext.
            //                    if ((row.DataContext) == dataContext)
            //                    {
            //                        return row;
            //                    }

            //                    break; // Only need to check one cell in each row
            //                }
            //            }
            //        }
            //    }
            //}
            return null;
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            if (((ClinicDeptInPtReqForm_V3View)this.GetView()).grdReqOutwardDetails != null) // && ((ClinicDeptInPtReqForm_V2View)this.GetView()).grdReqOutwardDetails.IsValid)
            {
                if (CheckValidGrid())
                {
                    SaveRequest();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0539_G1_Msg_InfoDataKhDung);
            }
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                btnFindRequest();
            }
        }

        public void btnFindRequest()
        {
            SearchRequestDrugInwardClinicDept(0, 20);
        }

        //Huyen_Update: 05/08/2015

        public void GetRemainingQtyForInPtRequestDrug()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            List<ReqOutwardDrugClinicDeptPatient> RemainingList = null;
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRemainingQtyForInPtRequestDrug(RequestDrug.InDeptStoreID, V_MedProductType, RequestDrug.RefGenDrugCatID_1, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RemainingList = contract.EndGetRemainingQtyForInPtRequestDrug(asyncResult);
                                display(RemainingList);
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

        public void display(List<ReqOutwardDrugClinicDeptPatient> RemainingList)
        {
            // TxD
            //List<ReqOutwardDrugClinicDeptPatient> newList = RequestDrug.ReqOutwardDetails.GroupBy(x => x.GenMedProductID).Select
            //    (x => new { GenMedProductID = x.Key }
            //    );
            foreach (var a in RequestDrug.ReqOutwardDetails)
            {
                foreach (var b in RemainingList)
                {
                    if (a.GenMedProductID == b.GenMedProductID)
                    {
                        a.RemainingQty = b.RemainingQty;
                        break;
                    }
                }
            }

            void onInitDlg(IModify_ReqQty temp)
            {
                // Tạo list sao chép danh sách thuốc từ phiếu yêu cầu để thực hiện cộng dồn số lượng những thuốc có ID giống nhau
                //Không dùng observableCollection do không biết cách thực hiện vòng for để cộng dồn
                List<ReqOutwardDrugClinicDeptPatient> RequestDetails = new List<ReqOutwardDrugClinicDeptPatient>();
                RequestDetails = RequestDrug.ReqOutwardDetails.ToList();
                for (int i = 0; i < RequestDetails.Count; i++)
                {
                    for (int j = 0; j < RequestDetails.Count; j++)
                    {
                        if (RequestDetails[i].GenMedProductID == RequestDetails[j].GenMedProductID && i != j)
                        {
                            RequestDetails[i].PrescribedQty += RequestDetails[j].PrescribedQty;
                            RequestDetails[i].ReqQty += RequestDetails[j].ReqQty;
                            RequestDetails.Remove(RequestDetails[j]);
                        }
                    }
                }
                ObservableCollection<ReqOutwardDrugClinicDeptPatient> RequestDrugDetails = RequestDetails.ToObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                //Huyen 07/08/2015: gán giá trị cho các biến được khai báo trong Interface IModify_ReqQty
                temp.RequestDrug.ReqOutwardDetails = RequestDrugDetails;//Lấy nội dung chi tiết phiếu yêu cầu để hiển thị trong View
            }
            GlobalsNAV.ShowDialog<IModify_ReqQty>(onInitDlg);
        }

        public void btnAutoReqQty(object sender, RoutedEventArgs e)
        {
            //Huyen: Show Drug list of current request form within remaining quantity
            GetRemainingQtyForInPtRequestDrug();
        }
        //Huyen_End

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0146_G1_Msg_ConfTaoMoiPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshRequest();
                hblPatientOther();

                SetCheckAllReqOutwardDetail();

                aucHoldConsultDoctor.setDefault();
                MedicalInstructionDate = Globals.GetCurServerDateTime();
            }
        }

        public void btnCreateNewByOld(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null)
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0145_G1_Msg_ConfTaoMoiTuPhCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            // TxD 23/12/2014: Added filtering by requesting warehouse storeID
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return;
            }
            SearchCriteria = new RequestSearchCriteria();
            StatusCoupon = "";
            IsStatusCoupon = false;
            hblPatientOther();
            NotShowWhenUseCreateNewFromOld = true;
            SearchRequestDrugInwardClinicDept(0, 20, true);
        }

        private bool CheckValidForLoadRequest()
        {
            if (RequestDrug == null)
            {
                return false;
            }
            if (RequestDrug.InDeptStoreID == null || RequestDrug.InDeptStoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1129_G1_ChonKhoYC, eHCMSResources.T0074_G1_I);
                return false;
            }
            return true;
        }

        private void ReqOutwardDrugClinicFromInstruction()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginReqOutwardDrugClinicFromInstruction(RequestDrug.FromDate, RequestDrug.ToDate, RequestDrug.RefGenDrugCatID_1
                            , RequestDrug.InDeptStoreObject.DeptID.GetValueOrDefault(0), V_MedProductType, RequestDrug.IsInstructionFuture
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndReqOutwardDrugClinicFromInstruction(asyncResult);
                                if (results != null)
                                {
                                    RequestDrug.ReqOutwardDetails = results.ToObservableCollection();
                                }
                                else
                                {
                                    if (RequestDrug.ReqOutwardDetails != null)
                                    {
                                        RequestDrug.ReqOutwardDetails.Clear();
                                    }
                                }
                                if (RequestDrug == null)
                                {
                                    RequestDrug = new RequestDrugInwardClinicDept();
                                }

                                GetListStaffFilter();
                                FillPagedCollectionAndGroup();
                                ResetingOldListToCreateNewList(true, true);
                                SetCheckAllReqOutwardDetail();
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

        public void btnLoadInstruction()
        {
            if (!CheckValidForLoadRequest()) return;
            ReqOutwardDrugClinicFromInstruction();
        }

        public void btnDelete(object sender, RoutedEventArgs e)
        {
            //call delete 
            if (MessageBox.Show(eHCMSResources.A0120_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteRequest();
            }
        }

        #region printing member

        public void btnPreviewTH()
        {
            //20190228 TNHX: Update title of report base on V_MedProductType
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                switch (V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                        else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC";
                        }
                        break;
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                        break;
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                        break;
                    case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                        proAlloc.LyDo = "PHIẾU LĨNH VẬT TƯ Y TẾ TIÊU HAO";
                        break;
                    case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                        proAlloc.LyDo = "PHIẾU LĨNH TIÊM NGỪA";
                        break;
                    case (long)AllLookupValues.MedProductType.MAU:
                        proAlloc.LyDo = "PHIẾU LĨNH MÁU";
                        break;
                    case (long)AllLookupValues.MedProductType.THANHTRUNG:
                        proAlloc.LyDo = "PHIẾU LĨNH THANH TRÙNG";
                        break;
                    default:
                        proAlloc.LyDo = "PHIẾU LĨNH";
                        break;
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST;
            }
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPreviewCT()
        {
            void onInitDlg(IClinicDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = RequestDrug.ReqDrugInClinicDeptID;
                switch (V_MedProductType)
                {
                    case (long)AllLookupValues.MedProductType.THUOC:
                        if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.GAYNGHIEN)
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                        else if (RequestDrug.RefGenDrugCatID_1 == (long)AllLookupValues.RefGenDrugCatID_1.HUONGTHAN)
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            proAlloc.LyDo = "PHIẾU LĨNH THUỐC";
                        }
                        break;
                    case (long)AllLookupValues.MedProductType.Y_CU:
                        proAlloc.LyDo = "PHIẾU LĨNH Y CỤ";
                        break;
                    case (long)AllLookupValues.MedProductType.HOA_CHAT:
                        proAlloc.LyDo = "PHIẾU LĨNH HÓA CHẤT";
                        break;
                    case (long)AllLookupValues.MedProductType.VTYT_TIEUHAO:
                        proAlloc.LyDo = "PHIẾU LĨNH VẬT TƯ Y TẾ TIÊU HAO";
                        break;
                    case (long)AllLookupValues.MedProductType.TIEM_NGUA:
                        proAlloc.LyDo = "PHIẾU LĨNH TIÊM NGỪA";
                        break;
                    case (long)AllLookupValues.MedProductType.MAU:
                        proAlloc.LyDo = "PHIẾU LĨNH MÁU";
                        break;
                    case (long)AllLookupValues.MedProductType.THANHTRUNG:
                        proAlloc.LyDo = "PHIẾU LĨNH THANH TRÙNG";
                        break;
                    default:
                        proAlloc.LyDo = "PHIẾU LĨNH";
                        break;
                }
                proAlloc.eItem = ReportName.DRUGDEPT_REQUEST_DETAILS;
            }
            GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRequestPharmacyInPdfFormat(RequestDrug.ReqDrugInClinicDeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetRequestPharmacyInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
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

        #endregion
        #endregion

        #region IHandle<DrugDeptCloseSearchRequestEvent> Members

        public void Handle(DrugDeptCloseSearchRequestEvent message)
        {
            if (message != null && this.IsActive)
            {
                RequestDrugInwardClinicDept item = message.SelectedRequest as RequestDrugInwardClinicDept;
                if (RequestDrug != null && item != null)
                {
                    ChangeValue(RequestDrug.RefGenDrugCatID_1, item.RefGenDrugCatID_1);
                }
                RequestDrug = item;
                ListRequestDrugDelete.Clear();

                GetInPatientRequestingDrugListByReqID(RequestDrug.ReqDrugInClinicDeptID, message.IsCreateNewFromExisting);
            }
        }

        #endregion

        //KMx: Không sử dụng Radio Button Code - Tên nữa (04/02/2016 11:52).
        //bool? IsCode = false;
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

        public bool ListOutDrugReqFilter(object listObj)
        {
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient != null && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID > 0)
            {
                if (outItem.CurPatientRegistration != null && outItem.CurPatientRegistration.Patient != null && outItem.CurPatientRegistration.Patient.PatientID > 0)
                {
                    return (outItem.CurPatientRegistration.Patient.PatientID == CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.Patient.PatientID);
                }
            }
            else
            {
                if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration == null && outItem.CurPatientRegistration == null)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CurrentlyViewReqByStaffIDFilter = false;
        public bool ListRequestByStaffIDFilter(object listObj)
        {
            ReqOutwardDrugClinicDeptPatient outItem = listObj as ReqOutwardDrugClinicDeptPatient;

            if (StaffDetailID > 0)
            {
                if (outItem.StaffID == StaffDetailID)
                    return true;
            }
            else
            {
                return true;
            }

            return false;
        }

        public bool _usedForRequestingDrug = false;
        public bool UsedForRequestingDrug
        {
            get { return _usedForRequestingDrug; }
            set
            {
                _usedForRequestingDrug = value;
            }
        }

        //▼====: #002
        private bool _RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMed;
        public bool RequireDoctorAndDate
        {
            get { return _RequireDoctorAndDate; }
            set
            {
                if (_RequireDoctorAndDate != value)
                {
                    _RequireDoctorAndDate = value;
                    NotifyOfPropertyChange(() => RequireDoctorAndDate);
                }
            }
        }

        private bool CheckValidMedicalInstructionDate(DateTime? aMedicalInstructionDate = null)
        {
            if (aMedicalInstructionDate == null && MedicalInstructionDate != null)
            {
                MedicalInstructionDate = Globals.ApplyValidMedicalInstructionDate(MedicalInstructionDate.Value, CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration);
                aMedicalInstructionDate = MedicalInstructionDate;
            }
            if (!RequireDoctorAndDate || aMedicalInstructionDate == null || !aMedicalInstructionDate.HasValue
                || CurrentReqOutwardDrugClinicDeptPatient == null || CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration == null)
                return true;
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate.HasValue
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate != null
                && aMedicalInstructionDate > CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.DischargeDetailRecCreatedDate)
            {
                MessageBox.Show(eHCMSResources.Z2188_G1_NYLKhongLonHonNXV, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if ((!CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate.HasValue || CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate == null)
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.ExamDate != null
                && aMedicalInstructionDate < CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.ExamDate)
            {
                MessageBox.Show(eHCMSResources.Z2187_G1_NYLKhongNhoHonNDK, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate.HasValue
                && CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate != null
                && aMedicalInstructionDate < CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration.AdmissionDate)
            {
                MessageBox.Show(eHCMSResources.Z2183_G1_NgayYLKhongNhoHonNNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }
        //▲====: #002

        //▼====== #009
        private bool SplitProductType(string sInput)
        {
            string[] mInputArray = sInput.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (mInputArray.Length >= 1)
            {
                foreach (var x in mInputArray)
                {
                    if (V_MedProductType == Convert.ToInt32(x.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //▲====== #009

        public void btnAddItem()
        {
            NotShowWhenUseCreateNewFromOld = true;
            //20190723 TBL: BM 0012971. Khi so luong cua kho cung cap = 0 thi khong cho them vao phieu 
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Remaining == 0)
            {
                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.Z2779_G1_SoLuongKhoCCBang0, eHCMSResources.Z1418_G1_KgTheThemDuoc), eHCMSResources.G0442_G1_TBao);
                return;
            }
            //20190723
            //▼====: #002
            if (SplitProductType(Globals.ServerConfigSection.ClinicDeptElements.ProductTypeNotDocAndDateReq))
            {
                if (RequireDoctorAndDate && (aucHoldConsultDoctor == null || aucHoldConsultDoctor.StaffID <= 0))
                {
                    MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    NotShowWhenUseCreateNewFromOld = false;
                    return;
                }
            }
            if (!CheckValidMedicalInstructionDate())
            {
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }
            //▲====: #002

            System.Diagnostics.Debug.WriteLine(" ========> btnAddItem  1 .....");

            CurrentReqOutwardDrugClinicDeptPatient.StaffID = GetStaffLogin().StaffID;

            if (UsedForRequestingDrug && RequestDrug != null && RequestDrug.RefGenDrugCatID_1 <= 0 && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                Globals.ShowMessage(eHCMSResources.Z1146_G1_ChonPhanNhomHg, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }

            // TxD 22/12/2014: Modify the following condition to check when item is blank or Code is blank 
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null || CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }

            if (CurrentReqOutwardDrugClinicDeptPatient.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                NotShowWhenUseCreateNewFromOld = false;
                return;
            }

            if (CalByUnitUse)
            {
                //KMx: Tính ra số lượng dựa vào DispenseVolume (15/11/2014 09:01).
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
            }

            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();

            if (RequestDrug == null)
            {
                RequestDrug = new RequestDrugInwardClinicDept();
            }
            if (RequestDrug.ReqOutwardDetails == null)
            {
                RequestDrug.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
            var temp = RequestDrug.ReqOutwardDetails.Where(x => x.RefGenericDrugDetail != null && CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail != null && x.RefGenericDrugDetail.GenMedProductID == CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID && x.PtRegistrationID == CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID);

            //KMx: Sửa lần 1: Nếu hàng đã có trong grid rồi thì cộng PrescribedQty và ReqQty lại.
            // Sửa lần 2: Vì lần 2 có thêm liều dùng nên không thể gộp 2 dòng lại được và cũng không cho thêm dòng mới.
            //Sửa lần 3: Chị Diễm khoa A nói có trường hợp 1 loại hàng mà có 2 BS chỉ định nên cho tách ra dòng mới (16/05/2016 17:20).

            //if (temp != null && temp.Count() > 0)
            //{
            //    //temp.ToList()[0].PrescribedQty += CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
            //    //temp.ToList()[0].ReqQty += CurrentReqOutwardDrugClinicDeptPatient.ReqQty;

            //    //KMx: Nếu trong danh sách đã có rồi thì không cho thêm nữa (vì không thể gộp liều dùng lại) (21/04/2016 15:11).
            //    MessageBox.Show("Loại hàng này đã được thêm trước đó rồi. Hãy điều chỉnh ở danh sách bên dưới.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            //else
            {
                if (temp != null && temp.Count() > 0 && MessageBox.Show(eHCMSResources.A0774_G1_Msg_ConfThemHgDaCo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    NotShowWhenUseCreateNewFromOld = false;
                    return;
                }
                CurrentReqOutwardDrugClinicDeptPatient.EntityState = EntityState.NEW;
                // TxD 24/03/2015: Set PrescribedQty = ReqQty initially
                CurrentReqOutwardDrugClinicDeptPatient.PrescribedQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
                //==== #004 ====
                ConvertCeiling(CurrentReqOutwardDrugClinicDeptPatient);
                //==== #004 ====
                CurrentReqOutwardDrugClinicDeptPatient.DoctorStaff = new Staff();
                CurrentReqOutwardDrugClinicDeptPatient.DoctorStaff.StaffID = aucHoldConsultDoctor.StaffID;
                CurrentReqOutwardDrugClinicDeptPatient.DoctorStaff.FullName = aucHoldConsultDoctor.StaffName;
                CurrentReqOutwardDrugClinicDeptPatient.MedicalInstructionDate = MedicalInstructionDate;
                CurrentReqOutwardDrugClinicDeptPatient.Notes = strNote;

                var item = CurrentReqOutwardDrugClinicDeptPatient.DeepCopy();
                RequestDrug.ReqOutwardDetails.Add(item);

                // TxD 16/11/2014: For a brand new List NOT YET SAVE we have to call the following once to define Grouping for the PagedCollectionView.
                if (RequestDrug.ReqOutwardDetails.Count == 1)
                {
                    FillPagedCollectionAndGroup();
                }
            }

            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;

            //KMx: Không sử dụng Radio Button Code - Tên nữa (04/02/2016 11:52).
            //if (IsCode.GetValueOrDefault())
            if (IsFocusTextCode)
            {
                if (tbx != null)
                {
                    txt = "";
                    tbx.Text = "";
                    tbx.Focus();
                }
            }
            else
            {
                if (acbAutoDrug_Text != null)
                {
                    acbAutoDrug_Text.Text = "";
                    acbAutoDrug_Text.Focus();
                }
            }

            //KMx: Khi thêm thuốc chỉ cần refresh, không cần group lại, vì nều số lượng nhiều sẽ bị chậm (15/11/2014 17:48).
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.Count > 0)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //}
            ////FillPagedCollectionAndGroup();

            //// TxD 19/11/2014: Create a Filter for PagedCollectionView if it does not exist yet.
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter
            //               && (ReqOutwardDrugClinicDeptPatientlst.Filter == null || CurrentlyViewReqByStaffIDFilter))
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
            //    CurrentlyViewReqByStaffIDFilter = false;
            //}

            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Refresh();

                if (CollectionView_ReqDetails.CanFilter && (CollectionView_ReqDetails.Filter == null || CurrentlyViewReqByStaffIDFilter))
                {
                    CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                    CurrentlyViewReqByStaffIDFilter = false;
                }
            }

            SetCheckAllReqOutwardDetail();
            NotShowWhenUseCreateNewFromOld = false;
        }

        //private void FillPagedCollectionAndGroup()
        //{
        //    if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
        //    {
        //        ReqOutwardDrugClinicDeptPatientlst = new PagedCollectionView(RequestDrug.ReqOutwardDetails);
        //        FillGroupName();
        //    }
        //}

        private void FillPagedCollectionAndGroup()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null /*&& RequestDrug.ReqOutwardDetails.Count > 0*/)
            {
                CVS_ReqDetails = new CollectionViewSource { Source = RequestDrug.ReqOutwardDetails };
                CollectionView_ReqDetails = (CollectionView)CVS_ReqDetails.View;
                FillGroupName();
                NotifyOfPropertyChange(() => CollectionView_ReqDetails);
            }
        }

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
            ComboBox bSel = (ComboBox)sender;
            if (bSel == null)
                return;

            if (flag)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails.Count > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0993_G1_Msg_ConfDoiPhanNhomHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        flag = false;
                        bSel.SelectedItem = e.RemovedItems[0];
                        return;
                    }
                }
            }

            if (flag)
            {
                if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null)
                {
                    for (int i = 0; i < RequestDrug.ReqOutwardDetails.Count; i++)
                    {
                        if (RequestDrug.ReqOutwardDetails[i].EntityState == EntityState.PERSITED || RequestDrug.ReqOutwardDetails[i].EntityState == EntityState.MODIFIED)
                        {
                            RequestDrug.ReqOutwardDetails[i].EntityState = EntityState.DETACHED;
                            ListRequestDrugDelete.Add(RequestDrug.ReqOutwardDetails[i]);
                        }
                    }
                    RequestDrug.ReqOutwardDetails.Clear();
                }
                //20190722 TBL: BM 0012966. Khi đổi phân nhóm không xóa autocomplete
                BrandName = "";
                txt = "";
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                //20190722
            }

            flag = true;
            if (bSel.IsDropDownOpen)
            {
                bSel.IsDropDownOpen = false;
            }
        }

        private bool flagStoreSupplier = true;
        public void cbxStoreSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxStoreSupplier = (ComboBox)sender;
            if (cbxStoreSupplier == null)
            {
                return;
            }
            if (flagStoreSupplier && RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z2756_G1_Msg_ConfDoiKhoCC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RequestDrug.ReqOutwardDetails.Clear();
                    ClearAutoComplete();
                }
                else
                {
                    flagStoreSupplier = false;
                    cbxStoreSupplier.SelectedItem = e.RemovedItems[0];
                    return;
                }
            }
            else if (flagStoreSupplier)
            {
                ClearAutoComplete();
            }
            flagStoreSupplier = true;
        }

        public void gMedProductTypeCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefeshRequest();
            hblPatientOther();

            SetCheckAllReqOutwardDetail();

            aucHoldConsultDoctor.setDefault();
            MedicalInstructionDate = Globals.GetCurServerDateTime();
            GetOutstandingTaskContent();
        }

        private void ClearAutoComplete()
        {
            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;
        }

        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                CurrentPatient = message.Item.Patient;
                //▼====: #011
                if (message.Item.AdmissionInfo != null)
                {
                    IsCOVID = message.Item.AdmissionInfo.IsTreatmentCOVID;
                }
                else
                {
                    IsCOVID = false;
                }
                //▲====: #011
                if (CurrentReqOutwardDrugClinicDeptPatient == null)
                {
                    CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                }
                CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = message.Item;
                CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = message.Item.PtRegistrationID;

                // TxD 17/11/2014: Filtering here as well when a NEW Patient has been selected to see any existing items (if any) of that Patient
                //if (ReqOutwardDrugClinicDeptPatientlst != null)
                //{
                //    if (ReqOutwardDrugClinicDeptPatientlst.Filter == null || CurrentlyViewReqByStaffIDFilter)
                //    {
                //        ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
                //        CurrentlyViewReqByStaffIDFilter = false;
                //    }

                //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
                //}

                if (CollectionView_ReqDetails != null)
                {
                    if (CollectionView_ReqDetails.Filter == null || CurrentlyViewReqByStaffIDFilter)
                    {
                        CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                        CurrentlyViewReqByStaffIDFilter = false;
                    }

                    CollectionView_ReqDetails.Refresh();
                }
            }
        }

        #region Checked All Member

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

        public void chkAllReqOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                return;
            }

            if (CheckAllOutwardDetail)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in RequestDrug.ReqOutwardDetails)
                {
                    item.Checked = false;
                }
            }
        }

        private void SetCheckAllReqOutwardDetail()
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                CheckAllOutwardDetail = false;
                return;
            }

            CheckAllOutwardDetail = RequestDrug.ReqOutwardDetails.All(x => x.Checked);
        }

        public void chkReqOutwardDetail_Click(object sender, RoutedEventArgs e)
        {
            SetCheckAllReqOutwardDetail();
        }

        private enum DataGridCol
        {
            ColMultiDelete = 0,
            ColDelete = 1,
            MaThuoc = 2,
            TenThuoc = 3
        }

        private void HideShowColumnDelete()
        {
            if (((ClinicDeptInPtReqForm_V3View)GetView()).grdReqOutwardDetails != null)
            {
                if (RequestDrug.CanSave)
                {
                    ((ClinicDeptInPtReqForm_V3View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                    ((ClinicDeptInPtReqForm_V3View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    ((ClinicDeptInPtReqForm_V3View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                    ((ClinicDeptInPtReqForm_V3View)GetView()).grdReqOutwardDetails.Columns[(int)DataGridCol.ColMultiDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        public void btnDeleteHang()
        {
            if (RequestDrug != null && RequestDrug.ReqOutwardDetails != null && RequestDrug.ReqOutwardDetails.Count > 0)
            {
                var items = RequestDrug.ReqOutwardDetails.Where(x => x.Checked == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show("Bạn có chắc muốn xóa những hàng đã chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        foreach (ReqOutwardDrugClinicDeptPatient obj in items)
                        {
                            if (obj.EntityState == EntityState.PERSITED || obj.EntityState == EntityState.MODIFIED)
                            {
                                obj.EntityState = EntityState.DETACHED;
                                ListRequestDrugDelete.Add(obj);
                            }
                        }
                        RequestDrug.ReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked == false).ToObservableCollection();
                        FillPagedCollectionAndGroup();

                        SetCheckAllReqOutwardDetail();
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

        //private void FillGroupName()
        //{
        //    if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.Count > 0)
        //    {
        //        ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Clear();
        //        ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Clear();
        //        ReqOutwardDrugClinicDeptPatientlst.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));

        //        // TxD 17/11/2014: Commented out the following sort order because Filter has been applied to view and work with each Group at a time.
        //        //KMx: Sort theo thời gian thêm thuốc (15/11/2014 17:21).
        //        //ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTimeSelection", System.ComponentModel.ListSortDirection.Descending));

        //        ReqOutwardDrugClinicDeptPatientlst.Filter = null;

        //    }
        //}

        private void FillGroupName()
        {
            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.GroupDescriptions.Clear();
                CollectionView_ReqDetails.SortDescriptions.Clear();
                CollectionView_ReqDetails.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("CurPatientRegistration.Patient.PatientCodeAndName"));

                // TxD 17/11/2014: Commented out the following sort order because Filter has been applied to view and work with each Group at a time.
                //KMx: Sort theo thời gian thêm thuốc (15/11/2014 17:21).
                //ReqOutwardDrugClinicDeptPatientlst.SortDescriptions.Add(new System.ComponentModel.SortDescription("DateTimeSelection", System.ComponentModel.ListSortDirection.Descending));

                CollectionView_ReqDetails.Filter = null;
            }
        }

        public void hblPatientOther()
        {
            CurrentReqOutwardDrugClinicDeptPatient.CurPatientRegistration = null;
            CurrentReqOutwardDrugClinicDeptPatient.PtRegistrationID = 0;
            CurrentPatient = null;
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListOutDrugReqFilter);
            //    CurrentlyViewReqByStaffIDFilter = false;
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //}

            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Filter = new Predicate<object>(ListOutDrugReqFilter);
                CurrentlyViewReqByStaffIDFilter = false;
                CollectionView_ReqDetails.Refresh();
            }
        }

        public void btnPatientOther(object sender, RoutedEventArgs e)
        {
            hblPatientOther();
        }

        public void btnViewAll()
        {
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = null;
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //    FillPagedCollectionAndGroup();
            //}

            if (CollectionView_ReqDetails != null)
            {
                CollectionView_ReqDetails.Filter = null;
                CollectionView_ReqDetails.Refresh();
                FillPagedCollectionAndGroup();
            }
        }

        private bool bAllGroupsAlreadyClosed = false;

        public void btnCloseOpenGroups()
        {
            bool bClose = false;
            if (!bAllGroupsAlreadyClosed)
            {
                bClose = true;
            }

            CloseOrOpenGroups(bClose);
        }

        private void CloseOrOpenGroups(bool bClosed)
        {
            //if (ReqOutwardDrugClinicDeptPatientlst == null || ReqOutwardDrugClinicDeptPatientlst.Groups == null || ReqOutwardDrugClinicDeptPatientlst.Groups.Count <= 0)
            //{
            //    return;
            //}

            if (CollectionView_ReqDetails == null || CollectionView_ReqDetails.Groups == null || CollectionView_ReqDetails.Groups.Count <= 0)
            {
                return;
            }

            bAllGroupsAlreadyClosed = bClosed;

            //AxDataGridNy theGrid = ((ClinicDeptInPtReqForm_V2View)this.GetView()).grdReqOutwardDetails;
            ////ChangedWPF - CMN
            //foreach (Common.PagedCollectionView.CollectionViewGroup group in ReqOutwardDrugClinicDeptPatientlst.Groups)
            //{
            //    if (bClosed)
            //    {
            //        theGrid.CollapseRowGroup(group, true);
            //    }
            //    else
            //    {
            //        theGrid.ExpandRowGroup(group, true);
            //    }
            //}
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";

        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 1 .....");

                //20190722 TBL: BM 0012967. Chưa chọn Phân nhóm mà đã nhập vào autocomplete thì hiện thông báo
                if (RequestDrug.RefGenDrugCatID_1 < 0 && !string.IsNullOrEmpty(txt) && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    MessageBox.Show(eHCMSResources.Z1146_G1_ChonPhanNhomHg);
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                    return;
                }
                else if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);
                    //▼====== #008: Thay đổi Query => Query tìm AutoComplete tương tự xuất của Khoa dược.
                    //GetRefGenMedProductDetails_Auto(Code, true, 0, RefGenMedProductDetails.PageSize);
                    SearchRefGenMedProductDetails(Code, true);
                    //▲====== #008
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(" ====> AxTextBox_LostFocus 2 .....");
                    // TxD 23/12/2014 If Code is blank then clear selected item
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }

        //AutoCompleteBox au;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            //20190722 TBL: BM 0012967. Chưa chọn Phân nhóm mà đã nhập vào autocomplete thì hiện thông báo
            if (RequestDrug.RefGenDrugCatID_1 < 0 && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                MessageBox.Show(eHCMSResources.Z1146_G1_ChonPhanNhomHg);
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                return;
            }
            e.Cancel = true;
            BrandName = e.Parameter;
            //RefGenMedProductDetails.PageIndex = 0;
            //▼====== #008: Thay đổi Query => Query tìm AutoComplete tương tự xuất của Khoa dược.
            //GetRefGenMedProductDetails_Auto(e.Parameter, false, 0, RefGenMedProductDetails.PageSize);
            SearchRefGenMedProductDetails(BrandName, false);
            //▲====== #008
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //▼====== #006
            if (RefGenMedProductDetails == null || RefGenMedProductDetails.Count < 1)
            {
                return;
            }
            //▲====== #006
            RefGenMedProductDetails obj = acbAutoDrug_Text.SelectedItem as RefGenMedProductDetails;
            //if (CurrentReqOutwardDrugClinicDeptPatient != null)
            if (obj != null && CurrentReqOutwardDrugClinicDeptPatient != null)
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
            }
            AutoDrug_LostFocus(null, null);
            //▼====== #008: Clear trước khi DropDown để tránh trường hợp tìm kiếm trước có dữ liệu nhưng
            //              Giá trị tìm kiếm sau không có giá trị nhưng vẫn còn hiện thông tin của lần trước.
            RefGenMedProductDetails.Clear();
            //▲====== 
        }

        private AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }

        public void AutoDrug_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acbAutoDrug_Text == null || string.IsNullOrEmpty(acbAutoDrug_Text.Text))
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
                if (tbxQty == null)
                {
                    return;
                }
                tbxQty.Focus();
            }
        }

        public void FilterStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //FilterByStaff();
            //FillPagedCollectionAndGroup();
            //if (ReqOutwardDrugClinicDeptPatientlst != null && ReqOutwardDrugClinicDeptPatientlst.CanFilter)
            //{
            //    ReqOutwardDrugClinicDeptPatientlst.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
            //    ReqOutwardDrugClinicDeptPatientlst.Refresh();
            //    CurrentlyViewReqByStaffIDFilter = true;
            //}

            if (CollectionView_ReqDetails != null && CollectionView_ReqDetails.CanFilter)
            {
                CollectionView_ReqDetails.Filter = new Predicate<object>(ListRequestByStaffIDFilter);
                CollectionView_ReqDetails.Refresh();
                CurrentlyViewReqByStaffIDFilter = true;
            }
        }

        //private void FilterByStaff()
        //{
        //    if (ReqOutwardDrugClinicDeptPatientlstCopy != null)
        //    {
        //        if (StaffDetailID > 0 && RequestDrug != null)
        //        {
        //            RequestDrug.ReqOutwardDetails = ReqOutwardDrugClinicDeptPatientlstCopy.Where(x => x.StaffID == StaffDetailID).ToObservableCollection();
        //        }
        //        else
        //        {
        //            RequestDrug.ReqOutwardDetails = ReqOutwardDrugClinicDeptPatientlstCopy.DeepCopy();
        //        }
        //    }
        //}

        public void btnChangeDoctor(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0910_G1_Msg_InfoKhTheDoiBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0595_G1_Msg_InfoChonDongCanDoiBS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
            {
                if (item.DoctorStaff == null)
                {
                    item.DoctorStaff = new Staff();
                }
                item.DoctorStaff.StaffID = aucHoldConsultDoctor.StaffID;
                item.DoctorStaff.FullName = aucHoldConsultDoctor.StaffName;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1149_G1_DoiBSiChiDinhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeMedicalInstructionDate(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0912_G1_Msg_InfoDoiNgYLenhPhLanh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0597_G1_Msg_InfoChonDongCanDoiNgYL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
            {
                item.MedicalInstructionDate = MedicalInstructionDate;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1150_G1_DoiNgYLenhThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void btnChangeNote(object sender, RoutedEventArgs e)
        {
            if (RequestDrug == null || RequestDrug.ReqOutwardDetails == null || RequestDrug.ReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0911_G1_Msg_InfoKhTheDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            ObservableCollection<ReqOutwardDrugClinicDeptPatient> SelectReqOutwardDetails = RequestDrug.ReqOutwardDetails.Where(x => x.Checked).ToObservableCollection();

            if (SelectReqOutwardDetails == null || SelectReqOutwardDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0596_G1_Msg_InfoChonDongCanDoiGhiChu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            foreach (ReqOutwardDrugClinicDeptPatient item in SelectReqOutwardDetails)
            {
                item.Notes = strNote;

                ischanged(item);
            }

            MessageBox.Show(eHCMSResources.Z1151_G1_DoiGChuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        }

        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, CurrentReqOutwardDrugClinicDeptPatient);
        }

        //==== #001 ====
        //string[] clAdUse = new string[] { "colMDoseStr", "colADoseStr", "colEDoseStr", "colNDoseStr" };
        DataGridCell mCurrentCell;
        public void grdReqOutwardDetails_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //if (clAdUse.Contains(e.Column.GetValue(FrameworkElement.NameProperty)) && IsResultView)
            //{
            //    mCurrentCell = e.Column.GetCellContent(e.Row.DataContext).Parent as DataGridCell;
            //    e.Cancel = true;
            //    GlobalsNAV.ShowDialog<IDrugResult>();
            //}
            //▼====== #003
            if (((e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colMDoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colADoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colEDoseStr")))
                || (e.Column.Equals(grdReqOutwardDetails.GetColumnByName("colNDoseStr"))))
                 && IsResultView)
            {
                mCurrentCell = e.Column.GetCellContent(e.Row.DataContext).Parent as DataGridCell;
                e.Cancel = true;
                GlobalsNAV.ShowDialog<IDrugResult>();
            }
            //▲====== #003
            //▼====== #007: Trước khi edit cell nào thì lưu thông tin cell đó lại tránh trường hợp click ra khỏi cell
            //              thì selected bị set về null => không thay đổi số lượng CD
            copySelectedReqOutwardDrugClinicDeptPatient = SelectedReqOutwardDrugClinicDeptPatient;
            //▲====== #007
        }

        public void Handle(DrugResultEvent message)
        {
            if (message != null)
            {
                switch (message.Result)
                {
                    case 1:
                        mCurrentCell.Background = new SolidColorBrush(Colors.Blue);
                        break;
                    case 2:
                        mCurrentCell.Background = new SolidColorBrush(Colors.Green);
                        break;
                    default:
                        mCurrentCell.Background = new SolidColorBrush(Colors.White);
                        break;
                }
            }
        }

        //==== #001 ====
        private bool _EnableTestFunction = Globals.ServerConfigSection.CommonItems.EnableTestFunction;
        public bool EnableTestFunction
        {
            get
            {
                return _EnableTestFunction;
            }
            set
            {
                _EnableTestFunction = value;
                NotifyOfPropertyChange(() => EnableTestFunction);
            }
        }

        //▼====== #005
        private ReqOutwardDrugClinicDeptPatient _copySelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient copySelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _copySelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_copySelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _copySelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => copySelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }

        private string _StatusCoupon = "";
        public string StatusCoupon
        {
            get { return _StatusCoupon; }
            set
            {
                if (_StatusCoupon != value)
                {
                    _StatusCoupon = value;
                }
                NotifyOfPropertyChange(() => StatusCoupon);
            }
        }

        private bool _IsStatusCoupon = false;
        public bool IsStatusCoupon
        {
            get { return _IsStatusCoupon; }
            set
            {
                if (_IsStatusCoupon != value)
                {
                    _IsStatusCoupon = value;
                }
                NotifyOfPropertyChange(() => IsStatusCoupon);
            }
        }

        private bool _NotShowWhenUseCreateNewFromOld = false;
        public bool NotShowWhenUseCreateNewFromOld
        {
            get { return _NotShowWhenUseCreateNewFromOld; }
            set
            {
                if (_NotShowWhenUseCreateNewFromOld != value)
                {
                    _NotShowWhenUseCreateNewFromOld = value;
                }
                NotifyOfPropertyChange(() => NotShowWhenUseCreateNewFromOld);
            }
        }

        private void ControlStatusCoupon()
        {
            StatusCoupon = "";
            if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == null || RequestDrug.IsApproved == false))
            {
                StatusCoupon = eHCMSResources.Z2425_G1_ChuaDuyetPhieuLinh;
                IsStatusCoupon = true;
            }
            else if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == true && RequestDrug.DaNhanHang == false))
            {
                StatusCoupon = eHCMSResources.Z2423_G1_DaDuyetLinh;
                IsStatusCoupon = true;
            }
            else if (RequestDrug.ReqOutwardDetails.Count > 0 && (RequestDrug.IsApproved == true && RequestDrug.DaNhanHang == true))
            {
                StatusCoupon = eHCMSResources.Z2424_G1_DaDuyetVuiLongNhap;
                IsStatusCoupon = true;
            }
            if (NotShowWhenUseCreateNewFromOld)
            {
                StatusCoupon = "";
                IsStatusCoupon = false;
            }
            NotShowWhenUseCreateNewFromOld = false;
        }
        //▲====== #005

        //▼====== #008
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

        private void GroupRemaining(IList<RefGenMedProductDetails> results)
        {
            var ListRefGMP = from RefGMP in results
                             group RefGMP by new
                             {
                                 RefGMP.GenMedProductID,
                                 RefGMP.BrandName,
                                 RefGMP.SelectedUnit.UnitName,
                                 RefGMP.RequestQty,
                                 RefGMP.Code,
                                 RefGMP.ProductCodeRefNum,
                                 RefGMP.RefGenMedDrugDetails.Content,
                                 RefGMP.GenericName
                             } into RefGMPGroup
                             select new
                             {
                                 Remaining = RefGMPGroup.Sum(groupItem => groupItem.Remaining),
                                 RefGMPGroup.Key.GenMedProductID,
                                 RefGMPGroup.Key.UnitName,
                                 RefGMPGroup.Key.BrandName,
                                 RefGMPGroup.Key.GenericName,
                                 RefGMPGroup.Key.Content,
                                 RefGMPGroup.Key.Code,
                                 Qty = RefGMPGroup.Key.RequestQty,
                                 RefGMPGroup.Key.ProductCodeRefNum
                             };
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var Details in ListRefGMP)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = Details.GenMedProductID;
                item.BrandName = Details.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = Details.UnitName;
                item.Code = Details.Code;
                item.Remaining = Details.Remaining;
                item.RequestQty = Details.Qty;
                item.ProductCodeRefNum = Details.ProductCodeRefNum;
                RefGenMedProductDetailsListSum.Add(item);
            }
        }

        //▼====== 20190116 TTM: bỏ code cứng, thay bằng khi người dùng chọn kho sẽ query theo kho.
        AxComboBox CbxOutFromStoreObject;
        public void cbxStoreSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            CbxOutFromStoreObject = sender as AxComboBox;
        }
        //▲======
        private void SearchRefGenMedProductDetails(string Name, bool? IsCode)
        {
            long OutFromStoreObject = 1;
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }

            long? RefGenDrugCatID_1 = null;
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                RefGenDrugCatID_1 = RequestDrug.RefGenDrugCatID_1;
            }
            //▼====== 20190116 TTM: bỏ code cứng, thay bằng khi người dùng chọn kho sẽ query theo kho.
            if (StoreCbx != null)
            {
                OutFromStoreObject = (long)CbxOutFromStoreObject.SelectedValue;
            }
            //▲======
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, null
                        , Name, OutFromStoreObject, V_MedProductType, RefGenDrugCatID_1, null, IsCode
                        , null, null, null, true
                        //▼====: #01
                        , IsCOVID
                        //▲====: #011
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                //20190110 TTM: Vì query lấy lên tất cả số dòng nhập tồn nên cần phải làm việc là group các dòng đó lại lấy Remaining tổng.
                                GroupRemaining(results);

                                if (IsCode == true)
                                {
                                    if (results != null && results.Count > 0)
                                    {
                                        if (CurrentReqOutwardDrugClinicDeptPatient == null)
                                        {
                                            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                                        }
                                        CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();

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
                                            if (tbxQty == null)
                                            {
                                                return;
                                            }
                                            tbxQty.Focus();
                                        }
                                    }
                                    else
                                    {
                                        if (CurrentReqOutwardDrugClinicDeptPatient != null)
                                        {
                                            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = null;
                                        }

                                        if (tbx != null)
                                        {
                                            txt = "";
                                            tbx.Text = "";
                                            tbx.Focus();
                                        }
                                        if (acbAutoDrug_Text != null)
                                        {
                                            acbAutoDrug_Text.Text = "";
                                        }

                                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                else
                                {
                                    RefGenMedProductDetails.Clear();
                                    RefGenMedProductDetails = RefGenMedProductDetailsListSum;
                                    acbAutoDrug_Text.PopulateComplete();
                                }
                            }
                            else
                            {
                                //20190411 TTM: New lại để khi không tìm thấy gì thì không hiển thị lại cái cũ (không clear được)
                                RefGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
                            }
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
            });

            t.Start();
        }
        //▲====== #008

        //▼====== #010
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

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        //▲======= #010
    }
}
