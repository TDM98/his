using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using System.Linq;
using DevExpress.Xpf.Printing;
using System.ComponentModel;
using aEMR.ViewContracts;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using aEMR.ReportModel.ReportModels;
using aEMR.ServiceClient;
using Microsoft.Win32;
using aEMR.CommonTasks;
using aEMR.Common.ExportExcel;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using DevExpress.ReportServer.Printing;
using aEMR.Common.Collections;
/*
 * 20180906 #001 TBL:   Xuat excel mau 79a-80a theo dinh dang 3360
 * 20190709 #002 TNHX:  [BM0006694] Create report TEMP21_NEW + Add title
 * 20190806 #003 TNHX:  [BM0013097] Add filter KCBBD for Temp21_New
 * 20190823 #004 TNHX:  [BM0013191] Fix View/Print for 79aTH
 * 20200319 #005 TTM:   BM 0027022: [79A] Bổ sung tích chọn xuất Excel toàn bộ dữ liệu, đã xác nhận, chưa xác nhận. 
 * 20200325 #006 TTM:   BM 0023939: Fix lỗi xuất Excel không có dữ liệu khi tên có dấu chấm (bằng cách ngăn chặn). Vì khi có dấu chấm vào chương trình không lấy ra được đuôi của file.
 * 20200811 #007 TNHX:   BM : Thêm thống kê tình hình chỉ định thuốc TP cho khoa nội trú
 * 20201011 #008 TNHX: 670 Thêm báo cao BN có XN chờ kết quả
 * 20211224 #009 TNHX: 803 Thêm báo cao bn covid cho khoa dược/ kế toán tổng hợp
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(ITemp20NgoaiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Temp20NgoaiTruViewModel : Conductor<object>, ITemp20NgoaiTru
        , IHandle<PatientSelectedGoToKhamBenh_InPt<PatientRegistration>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public Temp20NgoaiTruViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            _eventArg = eventArg;
            authorization();

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = false;
            DepartmentContent.AddSelectedAllItem = true;

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                DepartmentContent.LoadData();
            }
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(Temp20NgoaiTruViewModel_PropertyChanged);

            /*TMA 18/10/2017 - LOAD COMBOBOX NHÓM */
            //IsV_MedProductType = Visibility.Visible;
            MedProductTypeContent = Globals.GetViewModel<IEnumListing>();
            MedProductTypeContent.EnumType = typeof(AllLookupValues.MedProductType2);
            MedProductTypeContent.AddSelectOneItem = true;
            MedProductTypeContent.LoadData();

            (MedProductTypeContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(V_MedProductTypeContent_PropertyChanged);
            /**/

            RptParameters = new ReportParameters();
            RptParameters.IsDetail = true;
            FillCondition();
            FillMonth();
            FillQuarter();
            FillYear();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            searchPatientAndRegVm.mTimBN = false;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            //RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductSimple>();
            //RefGenMedProductDetailss.OnRefresh += RefGenMedProductDetailss_OnRefresh;
            //RefGenMedProductDetailss.PageSize = Globals.PageSize;
            RefeshData();
            GetDrugForSellVisitorDisplays = new PagedSortableCollectionView<GetDrugForSellVisitor>();
            GetDrugForSellVisitorDisplays.OnRefresh += GetDrugForSellVisitorDisplays_OnRefresh;
            GetDrugForSellVisitorDisplays.PageSize = Globals.PageSize;

            //▼===== #005
            V_79AExportType = new ObservableCollection<Lookup>();
            foreach (var item in Globals.AllLookupValueList)
            {
                if (item.ObjectTypeID == (long)LookupValues.V_79AExportType)
                {
                    V_79AExportType.Add(item);
                }
            }
            //▲===== #005
        }

        void GetDrugForSellVisitorDisplays_OnRefresh(object sender, Collections.RefreshEventArgs e)
        {
            SearchGetDrugForSellVisitor(BrandName, IsCode, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
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
        private OutwardDrugClinicDeptInvoice _SelectedOutInvoice = null;
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
            }
        }
        public void Handle(PatientSelectedGoToKhamBenh_InPt<PatientRegistration> message)
        {
            if (message != null && message.Item != null)
            {

                if (SelectedOutInvoice == null)
                {
                    SelectedOutInvoice = new OutwardDrugClinicDeptInvoice();
                }

                SelectedOutInvoice.PatientRegistration = message.Item;
                SelectedOutInvoice.PtRegistrationID = message.Item.PtRegistrationID;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _eventArg.Subscribe(this);
            //--- 17/10/2017 DPT bao cao 15 ngay
            if (eItem == ReportName.BaoCao15Ngay)
            {
                ShowDepartment = Visibility.Visible;
                By15Date();
            }
            //-------
            //--- 3/11/2017 DPT So xet nghiem
            if (eItem == ReportName.SoXetNghiem)
            {
                mExportToExcel = false;
                ShowDepartment = Visibility.Collapsed;
                IsV_MedProductType = Visibility.Collapsed;
                IsViewBy = Visibility.Collapsed;
                ViDetail = Visibility.Collapsed;
                IsMonth = Visibility.Collapsed;
                Is15Date = Visibility.Collapsed;
                IsDate = Visibility.Visible;
                IsQuarter = Visibility.Collapsed;
                IsYear = Visibility.Collapsed;
            }
            //-------
            if (eItem == ReportName.TRANSACTION_CANLAMSAN || eItem == ReportName.TRANSACTION_DUOCBV || eItem == ReportName.TRANSACTION_HOATDONGKB)
            {
                mExportToExcel = false;
            }
            if (eItem == ReportName.FollowICD)
            {
                ByYear();
            }
            if (eItem == ReportName.EmployeesReport)
            {
                ByMonth();
                IsViewBy = Visibility.Collapsed;
            }
            if (eItem == ReportName.MEDICAL_EQUIPMENT_STATISTICS)
            {
                ByYear();
            }
            if (eItem == ReportName.TREATMENT_ACTIVITY || eItem == ReportName.SPECIALIST_TREATMENT_ACTIVITY || eItem == ReportName.SURGERY_ACTIVITY)
            {
                ByQuarter();
                IsViewBy = Visibility.Collapsed;
                mExportToExcel = false;
            }
            if (eItem == ReportName.InPtAdmDisStatistics)
            {
                ShowDepartment = Visibility.Visible;
                ByDate();
            }
            /*TMA : 18/10/2017 - BỔ SUNG THÊM TRƯỜNG HỢP LÀ BÁO CÁO SỔ KIỂM NHẬP THUỐC HOÁ CHẤT Y CỤ DỰA TRÊN CÁI CÓ SẴN DO MR.CÔNG ĐÃ TẠO*/
            if (eItem == ReportName.RptDrugMedDept)
            {
                mExportToExcel = false;
                ShowDepartment = Visibility.Visible;
                IsV_MedProductType = Visibility.Visible;
            }
            /*▼====: #001*/
            if (eItem == ReportName.TEMP79a_CHITIET || eItem == ReportName.TEMP80a_CHITIET)
            {
                if (eItem == ReportName.TEMP79a_CHITIET)
                {
                    //gán lại là tổng hợp vì store trên service chạy trên 79a_tonghop 
                    eItem = ReportName.TEMP79a_TONGHOP;
                    Only79A = true;
                }
                Check3360 = true;
            }
            /*▲====: #001*/
            else
            {
                IsV_MedProductType = Visibility.Collapsed;
            }
            if (eItem == ReportName.OutwardDrugsByStaffStatistic)
            {
                CurrentCondition = Conditions.Last();
                IsViewBy = Visibility.Collapsed;
                cbxCondition_SelectionChanged(null, null);
            }
            if (eItem == ReportName.OutwardDrugsByStaffStatisticDetails)
            {
                CurrentCondition = Conditions.Last();
                IsViewBy = Visibility.Collapsed;
                cbxCondition_SelectionChanged(null, null);
            }
            //▼====: #007
            if (eItem == ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP)
            {
                CurrentCondition = Conditions.Last();
                IsViewBy = Visibility.Collapsed;
                cbxCondition_SelectionChanged(null, null);
            }
            //▲====: #007
        }

        //▼====: #002
        private string _strHienThi = Globals.PageName;
        public string StrHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => StrHienThi);
            }
        }
        //▲====: #002

        private bool _IsEnableViewBy = true;
        public bool IsEnableViewBy
        {
            get
            {
                return _IsEnableViewBy;
            }
            set
            {
                if (_IsEnableViewBy == value)
                    return;
                _IsEnableViewBy = value;
                NotifyOfPropertyChange(() => IsEnableViewBy);
            }
        }

        private bool _IsYVu;
        public bool IsYVu
        {
            get
            {
                return _IsYVu;
            }
            set
            {
                if (_IsYVu == value)
                    return;
                _IsYVu = value;
                NotifyOfPropertyChange(() => IsYVu);
            }
        }

        //KMx: Do ViewModel này dùng chung cho nhiều báo cáo nên không thể dùng static enum eTransaction_Management.mTemp20NgoaiTru
        //      Mà phải dùng dynamic enum. Function nào gọi VM này thì truyền enum của func đó vào biến EnumOfFunction.
        private int _enumOfFunction = 0;
        public int EnumOfFunction
        {
            get
            {
                return _enumOfFunction;
            }
            set
            {
                if (_enumOfFunction == value)
                    return;
                _enumOfFunction = value;
                NotifyOfPropertyChange(() => EnumOfFunction);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            else
            {
                if (IsYVu)
                {
                    mExportToExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management,
                                 EnumOfFunction, (int)oYVu_ManagementEx.mExportToExcel, (int)ePermission.mView);
                    mViewAndPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management,
                                 EnumOfFunction, (int)oYVu_ManagementEx.mViewAndPrint, (int)ePermission.mView);
                }
                else
                {
                    //KMx: Nếu được cấu hình xem báo cáo thì cho thấy luôn nút Xuất Excel và Xem In, khỏi cần kiểm tra (24/08/2016 11:07).
                    //mExportToExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    //            EnumOfFunction, (int)oTransaction_ManagementEx.mExportToExcel, (int)ePermission.mView);
                    //mViewAndPrint = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                    //                 EnumOfFunction, (int)oTransaction_ManagementEx.mViewAndPrint, (int)ePermission.mView);
                }
            }
        }

        private bool _mExportToExcel = true;
        private bool _mViewAndPrint = true;

        public bool mExportToExcel
        {
            get
            {
                return _mExportToExcel;
            }
            set
            {
                if (_mExportToExcel == value)
                    return;
                _mExportToExcel = value;
                NotifyOfPropertyChange(() => mExportToExcel);
            }
        }

        public bool mViewAndPrint
        {
            get
            {
                return _mViewAndPrint;
            }
            set
            {
                if (_mViewAndPrint == value)
                    return;
                _mViewAndPrint = value;
                NotifyOfPropertyChange(() => mViewAndPrint);
            }
        }

        void Temp20NgoaiTruViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (DepartmentContent != null && DepartmentContent.SelectedItem != null)
                {
                    RptParameters.DeptID = DepartmentContent.SelectedItem.DeptID;
                    RptParameters.DeptName = DepartmentContent.SelectedItem.DeptName;
                }
                else
                {
                    RptParameters.DeptID = 0;
                    RptParameters.DeptName = "";
                }
            }
        }

        /*TMA 18/10/2017*/
        void V_MedProductTypeContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (MedProductTypeContent != null && MedProductTypeContent.SelectedItem != null)
                {
                    RptParameters.V_MedProductType = (long)((AllLookupValues.MedProductType2)MedProductTypeContent.SelectedItem.EnumItem);
                }
                else
                {
                    RptParameters.V_MedProductType = 0;
                }
            }
        }
        //

        #region Properties Member

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }

        public class Condition
        {
            private string _Text;
            private long _Value;
            public string Text { get { return _Text; } }
            public long Value { get { return _Value; } }
            public Condition(string theText, long theValue)
            {
                _Text = theText;
                _Value = theValue;
            }
        }

        private bool _IsProcessing = false;
        public bool IsProcessing
        {
            get { return _IsProcessing; }
            set
            {
                if (_IsProcessing != value)
                {
                    _IsProcessing = value;
                    NotifyOfPropertyChange(() => IsProcessing);
                }
            }
        }

        private DateTime? _FromDate = DateTime.Now;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private DateTime? _ToDate = DateTime.Now;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        private ReportParameters _RptParameters;
        public ReportParameters RptParameters
        {
            get { return _RptParameters; }
            set
            {
                if (_RptParameters != value)
                {
                    _RptParameters = value;
                    NotifyOfPropertyChange(() => RptParameters);
                }
            }
        }

        private bool _IsDetail = true;
        public bool IsDetail
        {
            get
            {
                return _IsDetail;
            }
            set
            {
                _IsDetail = value;
                NotifyOfPropertyChange(() => IsDetail);
            }
        }

        private bool _NotTreatedAsInPt;
        public bool NotTreatedAsInPt
        {
            get
            {
                return _NotTreatedAsInPt;
            }
            set
            {
                _NotTreatedAsInPt = value;
                NotifyOfPropertyChange(() => NotTreatedAsInPt);
            }
        }
        /*▼====: #001*/
        private bool _Check3360 = false;
        public bool Check3360
        {
            get
            {
                return _Check3360;
            }
            set
            {
                _Check3360 = value;
                NotifyOfPropertyChange(() => Check3360);
            }
        }
        /*▲====: #001*/
        private Visibility _ViDetail = Visibility.Collapsed;
        public Visibility ViDetail
        {
            get
            {
                return _ViDetail;
            }
            set
            {
                _ViDetail = value;
                NotifyOfPropertyChange(() => ViDetail);
            }
        }

        private Visibility _ShowDepartment = Visibility.Collapsed;
        public Visibility ShowDepartment
        {
            get
            {
                return _ShowDepartment;
            }
            set
            {
                _ShowDepartment = value;
                NotifyOfPropertyChange(() => ShowDepartment);
            }
        }

        private Visibility _ViTreatedOrNot = Visibility.Collapsed;
        public Visibility ViTreatedOrNot
        {
            get
            {
                return _ViTreatedOrNot;
            }
            set
            {
                _ViTreatedOrNot = value;
                NotifyOfPropertyChange(() => ViTreatedOrNot);
            }
        }

        private ObservableCollection<int> _ListMonth;
        public ObservableCollection<int> ListMonth
        {
            get { return _ListMonth; }
            set
            {
                if (_ListMonth != value)
                {
                    _ListMonth = value;
                    NotifyOfPropertyChange(() => ListMonth);
                }
            }
        }

        private ObservableCollection<int> _ListQuartar;
        public ObservableCollection<int> ListQuartar
        {
            get { return _ListQuartar; }
            set
            {
                if (_ListQuartar != value)
                {
                    _ListQuartar = value;
                    NotifyOfPropertyChange(() => ListQuartar);
                }
            }
        }


        private ObservableCollection<int> _ListYear;
        public ObservableCollection<int> ListYear
        {
            get { return _ListYear; }
            set
            {
                if (_ListYear != value)
                {
                    _ListYear = value;
                    NotifyOfPropertyChange(() => ListYear);
                }
            }
        }
        /*TMA 18/10/2017*/
        private IEnumListing _V_MedProductTypeContent;
        public IEnumListing MedProductTypeContent
        {
            get { return _V_MedProductTypeContent; }
            set
            {
                _V_MedProductTypeContent = value;
                NotifyOfPropertyChange(() => MedProductTypeContent);
            }
        }
        /*TMA*/

        private ObservableCollection<Condition> _Conditions;
        public ObservableCollection<Condition> Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                if (_Conditions != value)
                {
                    _Conditions = value;
                    NotifyOfPropertyChange(() => Conditions);
                }
            }
        }

        private Condition _CurrentCondition;
        public Condition CurrentCondition
        {
            get
            {
                return _CurrentCondition;
            }
            set
            {
                if (_CurrentCondition != value)
                {
                    _CurrentCondition = value;
                    NotifyOfPropertyChange(() => CurrentCondition);
                }
            }
        }

        private Visibility _IsMonth;
        public Visibility IsMonth
        {
            get
            { return _IsMonth; }
            set
            {
                if (_IsMonth != value)
                {
                    _IsMonth = value;
                    NotifyOfPropertyChange(() => IsMonth);
                }
            }
        }

        /*TMA 18/10/2017*/
        private Visibility _IsV_MedProductType = Visibility.Collapsed;
        public Visibility IsV_MedProductType
        {
            get
            { return _IsV_MedProductType; }
            set
            {
                if (_IsV_MedProductType != value)
                {
                    _IsV_MedProductType = value;
                    NotifyOfPropertyChange(() => IsV_MedProductType);
                }
            }
        }
        /**/

        private Visibility _IsDate = Visibility.Collapsed;
        public Visibility IsDate
        {
            get
            { return _IsDate; }
            set
            {
                if (_IsDate != value)
                {
                    _IsDate = value;
                    NotifyOfPropertyChange(() => IsDate);
                }
            }
        }

        //--- 17/10/2017 DPT bao cao 15 ngày
        private Visibility _Is15Date = Visibility.Collapsed;
        public Visibility Is15Date
        {
            get
            { return _Is15Date; }
            set
            {
                if (_Is15Date != value)
                {
                    _Is15Date = value;
                    NotifyOfPropertyChange(() => Is15Date);
                }
            }
        }
        //-----------------
        private Visibility _IsQuarter;
        public Visibility IsQuarter
        {
            get
            { return _IsQuarter; }
            set
            {
                if (_IsQuarter != value)
                {
                    _IsQuarter = value;
                    NotifyOfPropertyChange(() => IsQuarter);
                }
            }
        }

        private Visibility _IsYear;
        public Visibility IsYear
        {
            get
            { return _IsYear; }
            set
            {
                if (_IsYear != value)
                {
                    _IsYear = value;
                    NotifyOfPropertyChange(() => IsYear);
                }
            }
        }

        private Visibility _IsViewBy;
        public Visibility IsViewBy
        {
            get
            { return _IsViewBy; }
            set
            {
                if (_IsViewBy != value)
                {
                    _IsViewBy = value;
                    NotifyOfPropertyChange(() => IsViewBy);
                }
            }
        }
        //▼===== #005
        private ObservableCollection<Lookup> _V_79AExportType;
        public ObservableCollection<Lookup> V_79AExportType
        {
            get
            { return _V_79AExportType; }
            set
            {
                if (_V_79AExportType != value)
                {
                    _V_79AExportType = value;
                    NotifyOfPropertyChange(() => V_79AExportType);
                }
            }
        }
        //▲===== #005
        #endregion

        #region FillData Member

        private void FillMonth()
        {
            if (ListMonth == null)
            {
                ListMonth = new ObservableCollection<int>();
            }
            else
            {
                ListMonth.Clear();
            }
            for (int i = 1; i < 13; i++)
            {
                ListMonth.Add(i);
            }
            RptParameters.Month = DateTime.Now.Month;
        }

        private void FillQuarter()
        {
            if (ListQuartar == null)
            {
                ListQuartar = new ObservableCollection<int>();
            }
            else
            {
                ListQuartar.Clear();
            }
            for (int i = 1; i < 5; i++)
            {
                ListQuartar.Add(i);
            }
            int Month = DateTime.Now.Month;
            if (Month <= 3)
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 1;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 2;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 3;
            }
            else // 4th Quarter = October 1 to December 31
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 4;
            }
        }

        private void FillYear()
        {
            if (ListYear == null)
            {
                ListYear = new ObservableCollection<int>();
            }
            else
            {
                ListYear.Clear();
            }
            int year = DateTime.Now.Year;
            for (int i = year - 2; i < year + 1; i++)
            {
                ListYear.Add(i);
            }
            RptParameters.Year = year;

        }

        private void FillCondition()
        {
            if (Conditions == null)
            {
                Conditions = new ObservableCollection<Condition>();
            }
            else
            {
                Conditions.Clear();
            }

            Conditions.Add(new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0));
            Conditions.Add(new Condition(eHCMSResources.Z0939_G1_TheoTh, 1));
            Conditions.Add(new Condition(eHCMSResources.G0375_G1_TheoNg, 2));

            CurrentCondition = Conditions.FirstOrDefault();
            ByQuarter();
        }

        public void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CurrentCondition.Value)
            {
                case 0:
                    ByQuarter();
                    break;
                case 1:
                    ByMonth();
                    break;
                case 2:
                    ByDate();
                    break;
            }
        }
        //---- 17/10/2017 DPT bao cao 15 ngay
        private void By15Date()
        {
            mExportToExcel = false;
            IsV_MedProductType = Visibility.Collapsed;
            IsViewBy = Visibility.Collapsed;
            ViDetail = Visibility.Collapsed;
            IsMonth = Visibility.Collapsed;
            Is15Date = Visibility.Visible;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
            Conditions.Add(new Condition("Đến ngày", 3));
            CurrentCondition = Conditions.Last();
        }
        //------------------
        private void ByDate()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
        }

        private void ByMonth()
        {
            IsMonth = Visibility.Visible;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
        }

        private void ByQuarter()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Visible;
            IsYear = Visibility.Visible;
        }

        private void ByYear()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
            IsViewBy = Visibility.Collapsed;
        }

        #endregion

        private void GetReport()
        {
            switch (_eItem)
            {
                case ReportName.TEMP20_NGOAITRU:
                    ReportModel = null;
                    ReportModel = new TransactionTemp20NgoaiTru().PreviewModel;
                    break;
                case ReportName.TEMP20_NGOAITRU_TRATHUOC:
                    ReportModel = null;
                    ReportModel = new TransactionTemp20NgoaiTruTraThuoc().PreviewModel;
                    break;
                case ReportName.TEMP20_NOITRU:
                    ReportModel = null;
                    ReportModel = new TransactionTemp20NoiTru().PreviewModel;
                    break;
                case ReportName.TEMP20_VTYTTH:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp20_VTYTTH").PreviewModel;
                    break;
                case ReportName.TEMP21_NGOAITRU:
                    ReportModel = null;
                    ReportModel = new TransactionTemp21NgoaiTru().PreviewModel;
                    break;
                case ReportName.TEMP21_NOITRU:
                    ReportModel = null;
                    ReportModel = new TransactionTemp21NoiTru().PreviewModel;
                    break;
                case ReportName.TEMP25a_CHITIET:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp25aModel().PreviewModel;
                    break;
                case ReportName.TEMP25a_TONGHOP:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp25aTHModel().PreviewModel;
                    break;
                case ReportName.TEMP25aTRATHUOC_CHITIET:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp25aTraThuocModel().PreviewModel;
                    break;
                case ReportName.TEMP25aTRATHUOC_TONGHOP:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp25aTraThuocTHModel().PreviewModel;
                    break;
                case ReportName.TEMP26a_CHITIET:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp26aCTModel().PreviewModel;
                    break;
                case ReportName.TEMP26a_TONGHOP:
                    ReportModel = null;
                    ReportModel = new TransactionsTemp26aTHModel().PreviewModel;
                    break;
                case ReportName.TEMP19:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.TEMP20_NOITRU_NEW:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.TEMP21_NEW:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp21New").PreviewModel;
                    break;
                case ReportName.TEMP79a_CHITIET:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp79aCT").PreviewModel;
                    break;
                case ReportName.TEMP79a_TONGHOP:
                    ReportModel = null;
                    //▼====: #004
                    if (RptParameters.IsFullDetails)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp79aTH_BV").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp79aTH").PreviewModel;
                    }
                    //▲====: #004
                    break;
                case ReportName.TEMP79aTRATHUOC_CHITIET:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp79aTraThuoc").PreviewModel;
                    break;
                case ReportName.TEMP79aTRATHUOC_TONGHOP:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp79aTraThuocTH").PreviewModel;
                    break;
                case ReportName.TEMP80a_CHITIET:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp80aCT").PreviewModel;
                    break;
                case ReportName.TEMP80a_TONGHOP:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp80aTH").PreviewModel;
                    break;
                case ReportName.TEMP38a:
                    ReportModel = null;
                    ReportModel = new TransactionsTemplate38().PreviewModel;
                    break;
                case ReportName.THONGKEDOANHTHU:
                    ReportModel = null;
                    ReportModel = new TransactionThongKeDoanhThu().PreviewModel;
                    break;
                case ReportName.TRANSACTION_VIENPHICHITIET:
                    ReportModel = null;
                    ReportModel = new TransactionChiTietVienPhiReportModel().PreviewModel;
                    break;
                case ReportName.TRANSACTION_VIENPHICHITIET_PK:
                    ReportModel = null;
                    ReportModel = new TransactionChiTietVienPhiPKReportModel().PreviewModel;
                    break;
                case ReportName.HOSPITAL_FEES_REPORT:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.HIGH_TECH_FEES_REPORT:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.OUT_MEDICAL_MATERIAL_REPORT:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.HIGH_TECH_FEES_CHILD_REPORT:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                case ReportName.FollowICD:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptHISSummaryFollowByICD").PreviewModel;
                    break;
                case ReportName.EmployeesReport:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptHISStaffsInfo").PreviewModel;
                    break;
                case ReportName.MEDICAL_EQUIPMENT_STATISTICS:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptMedicalEquipmentStatistics").PreviewModel;
                    break;
                case ReportName.TREATMENT_ACTIVITY:
                case ReportName.SPECIALIST_TREATMENT_ACTIVITY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptTreatmentStatistics").PreviewModel;
                    break;
                case ReportName.SURGERY_ACTIVITY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptSurgeryStatistics").PreviewModel;
                    break;
                /*TMA : 06/10/2017 - BÁO CÁO CHUYỂN TUYẾN - PHỤ LỤC 2B - KO CẦN XEM IN*/
                case ReportName.TransferFormType2Rpt:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptTransferFormType2").PreviewModel;
                    break;
                /*TMA : 09/10/2017 - BÁO CÁO CHUYỂN TUYẾN - PHỤ LỤC 2A - KO CẦN XEM IN*/
                case ReportName.TransferFormType2_1Rpt:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptTransferFormType2_1").PreviewModel;
                    break;
                /*TMA : 12/10/2017 - BÁO CÁO CHUYỂN TUYẾN - PHỤ LỤC 5 - KO CẦN XEM IN*/
                case ReportName.TransferFormType5Rpt:
                case ReportName.TRANSFERFORMDATA:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    return;
                /*TMA : 18/10/2017 - SỔ KIỂM NHẬP THUỐC HOÁ CHẤT Y CỤ*/
                case ReportName.RptDrugMedDept:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptDrugMedDept").PreviewModel;
                    break;
                /*DuyNH : 20/01/2021 - Danh sách chi tiết BN chuyển đi các tuyến  - KO CẦN XEM IN*/
                case ReportName.BCChiTietBNChuyenDi:
                    MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien);
                    //ReportModel = null;
                    //ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BCChiTietBNChuyenDi").PreviewModel;
                    break;
                /*DuyNH : 07/04/2021 - Danh sách dịch vụ kỹ thuật có trên HIS  - KO CẦN XEM IN*/
                case ReportName.BC_DS_DichVuKyThuatTrenHIS:
                    MessageBox.Show(eHCMSResources.Z3113_G1_BC_DSDichVuKyThuat);
                    //ReportModel = null;
                    //ReportMode
                    break;
                /*DatTB : 02/06/2021 - Danh mục kỹ thuật mới*/
                case ReportName.BC_DM_KyThuatMoi:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptDanhMucKyThuatMoi").PreviewModel;
                    break;
                case ReportName.InPtAdmDisStatistics:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptPatientRegistration.RptInPtAdmDisStatistics").PreviewModel;
                    break;
                // 17/10/2017 DPT bao cao 15 ngay
                case ReportName.BaoCao15Ngay:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_BC15NGAY").PreviewModel;
                    break;
                // 03/11/2017 DPT so xet nghiem
                case ReportName.SoXetNghiem:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_SOXETNGHIEM").PreviewModel;
                    break;
            }
            if (ReportModel != null)
            {
                //ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
                FillReport_ParameterValues();
            }
            //ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
        }

        //public void _reportModel_RequestDefaultParameterValues(object sender, System.EventArgs e)
        public void FillReport_ParameterValues()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            string reportLogoUrl = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            switch (_eItem)
            {
                case ReportName.TEMP20_NGOAITRU:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.TEMP20_NGOAITRU_TRATHUOC:
                    goto case ReportName.TEMP20_NGOAITRU;
                case ReportName.TEMP20_NOITRU:
                    goto case ReportName.TEMP20_NGOAITRU;
                case ReportName.TEMP20_VTYTTH:
                    goto case ReportName.TEMP20_NGOAITRU;
                case ReportName.TEMP21_NGOAITRU:
                    goto case ReportName.TEMP20_NGOAITRU;
                case ReportName.TEMP21_NOITRU:
                    goto case ReportName.TEMP20_NGOAITRU;
                case ReportName.TEMP25a_CHITIET:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    //▼===== #005
                    if (_eItem == ReportName.TEMP79a_CHITIET)
                    {
                        rParams["V_79AExportType"].Value = RptParameters.V_79AExportType;
                    }
                    //▲====== #005
                    break;
                case ReportName.TEMP25a_TONGHOP:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP25aTRATHUOC_CHITIET:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP25aTRATHUOC_TONGHOP:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP26a_CHITIET:
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    rParams["DeptName"].Value = RptParameters.DeptID.GetValueOrDefault(0) > 0 ? RptParameters.DeptName : "";
                    rParams["NotTreatedAsInPt"].Value = NotTreatedAsInPt;
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP26a_TONGHOP:
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    rParams["DeptName"].Value = RptParameters.DeptID.GetValueOrDefault(0) > 0 ? RptParameters.DeptName : "";
                    rParams["NotTreatedAsInPt"].Value = NotTreatedAsInPt;
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP79a_CHITIET:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP79a_TONGHOP:
                    //▼====: #004
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    if (RptParameters.IsFullDetails)
                    {
                        rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                        rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                    }
                    else
                    {
                        rParams["parLogoUrl"].Value = reportLogoUrl;
                    }
                    //▼===== #005
                    rParams["V_79AExportType"].Value = RptParameters.V_79AExportType;
                    //▲===== #005
                    break;
                //▲====: #004
                case ReportName.TEMP79aTRATHUOC_CHITIET:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP79aTRATHUOC_TONGHOP:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP80a_CHITIET:
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    rParams["DeptName"].Value = RptParameters.DeptID.GetValueOrDefault(0) > 0 ? RptParameters.DeptName : "";
                    rParams["NotTreatedAsInPt"].Value = NotTreatedAsInPt;
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TEMP80a_TONGHOP:
                    goto case ReportName.TEMP80a_CHITIET;
                case ReportName.THONGKEDOANHTHU:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DateShow"].Value = RptParameters.Show;
                    rParams["parLogoUrl"].Value = reportLogoUrl;
                    break;
                case ReportName.TRANSACTION_VIENPHICHITIET:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.TRANSACTION_VIENPHICHITIET_PK:
                    goto case ReportName.TEMP25a_CHITIET;
                case ReportName.FollowICD:
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["ViceDirector"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.EmployeesReport:
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.MEDICAL_EQUIPMENT_STATISTICS:
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["AccountingManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 40 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 40 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.TREATMENT_ACTIVITY:
                    rParams["IsSpecialist"].Value = false;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.SPECIALIST_TREATMENT_ACTIVITY:
                    rParams["IsSpecialist"].Value = true;
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.SURGERY_ACTIVITY:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;

                /*TMA : 06/10/2017 - BÁO CÁO GIẤY CHUYỂN TUYẾN - PHỤ LỤC SỐ 2B*/
                case ReportName.TransferFormType2Rpt:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    break;

                /*TMA : 09/10/2017 - BÁO CÁO GIẤY CHUYỂN TUYẾN - PHỤ LỤC SỐ 2A*/
                case ReportName.TransferFormType2_1Rpt:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                /*TMA : 09/10/2017 - BÁO CÁO GIẤY CHUYỂN TUYẾN - PHỤ LỤC SỐ 2A*/

                /*TMA : 12/10/2017 - BÁO CÁO GIẤY CHUYỂN TUYẾN - PHỤ LỤC SỐ 5*/
                case ReportName.TransferFormType5Rpt:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    break;
                /*TMA : 12/10/2017 - BÁO CÁO GIẤY CHUYỂN TUYẾN - PHỤ LỤC SỐ 5*/
                case ReportName.InPtAdmDisStatistics:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    break;
                //RptDrugMedDept



                /*DuyNH: 20/01/2020 - Danh sách chi tiết BN chuyển đi các tuyến*/
                case ReportName.BCChiTietBNChuyenDi:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["TransferType"].Value = 1; // Chuyển tuyến đi. 
                    break;

                /*DatTB: 02/06/2020 - Danh sách chi tiết BN chuyển đi các tuyến*/
                case ReportName.BC_DM_KyThuatMoi:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    break;

                /*TMA : 18/10/2017 - SỔ KIỂM NHẬP THUỐC HOÁ CHẤT Y CỤ*/
                case ReportName.RptDrugMedDept:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["V_MedProductType"].Value = (int)RptParameters.V_MedProductType;//(int)RptParameters.V_MedProductType.GetValueOrDefault(0);
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    break;
                //----27/10/2017 DPT Bao Cao 15 ngay
                case ReportName.BaoCao15Ngay:
                    rParams["DeptID"].Value = (int)RptParameters.DeptID.GetValueOrDefault(0);
                    rParams["DeptName"].Value = RptParameters.DeptID.GetValueOrDefault(0) > 0 ? RptParameters.DeptName : "";
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["SearchID"].Value = SelectedOutInvoice == null ? 0 : Convert.ToInt32(SelectedOutInvoice.PatientRegistration.PtRegistrationID);
                    //rParams["PatientName"].Value = SelectedOutInvoice.PatientRegistration.AdmissionDate == null ? "" + SelectedOutInvoice.PatientRegistration.Patient.FullName + " (" + SelectedOutInvoice.PatientRegistration.Patient.PatientCode + ")" : "" + SelectedOutInvoice.PatientRegistration.Patient.FullName + " (" + SelectedOutInvoice.PatientRegistration.Patient.PatientCode + ") (" + SelectedOutInvoice.PatientRegistration.AdmissionDate.ToString() + ")";
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;


                    break;
                //--------  

                //----3/11/2017 DPT so xet nghiem
                case ReportName.SoXetNghiem:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    break;
                //--------  
                case ReportName.TRANSFERFORMDATA:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    break;
                //▼==== DUY - 07-4-2021
                case ReportName.BC_DS_DichVuKyThuatTrenHIS:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    break;
                //▲====


                //▼====: #003


                //▼====: #002
                case ReportName.TEMP21_NEW:
                    rParams["Quarter"].Value = RptParameters.Quarter;
                    rParams["Month"].Value = RptParameters.Month;
                    rParams["Year"].Value = RptParameters.Year;
                    rParams["Flag"].Value = RptParameters.Flag;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Show"].Value = RptParameters.Show;
                    rParams["TypeKCBBD"].Value = RptParameters.TypeKCBBD;
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parHospitalCode"].Value = Globals.ServerConfigSection.Hospitals.HospitalCode;
                    break;
                    //▲====: #002
                    //▲====: #003
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        public void btnXemIn(object sender, EventArgs e)
        {
            //KMx: Nếu là báo cáo bệnh nhân điều trị nội trú của Y Vụ thì không được xem/in vì chưa làm (03/06/2015 11:36).
            /*TMA : 06/10/2017 : BÁO CÁO CHUYỂN TUYẾN - PHỤ LỤC 2B - KO CÓ XEM IN NÊN THÊM ĐIỀU KIỆN "eItem == ReportName.TransferFormType2Rpt || eItem == ReportName.TransferFormType2_1Rpt || eItem == ReportName.TransferFormType5Rpt)" VÀO IF ()*/
            if (eItem == ReportName.REPORT_INPATIENT || eItem == ReportName.REPORT_GENERAL_TEMP02 || eItem == ReportName.TransferFormType5Rpt || eItem == ReportName.TRANSFERFORMDATA || eItem == ReportName.BC_DS_DichVuKyThuatTrenHIS)
            {
                MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (GetParameters())
            {
                GetReport();
            }
        }

        public void btnIn(object sender, EventArgs e)
        {
            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            return;

            //if (GetParameters())
            //{
            //    // Globals.EventAggregator.Publish(new TransactionPrintSilientEvent { IsDetail = this.IsDetail, RptParameters = this.RptParameters });
            //}
        }

        public void rdtChitiet_Checked(object sender, RoutedEventArgs e)
        {
            if (eItem == ReportName.TEMP79a_CHITIET || eItem == ReportName.TEMP79a_TONGHOP
                || eItem == ReportName.TEMP80a_CHITIET || eItem == ReportName.TEMP80a_TONGHOP
                || eItem == ReportName.BCSuDungToanBV)
            {
                RptParameters.IsDetail = true;
            }
            IsDetail = true;
            switch (_eItem)
            {
                case ReportName.TEMP25a_TONGHOP:
                    eItem = ReportName.TEMP25a_CHITIET;
                    break;
                case ReportName.TEMP25aTRATHUOC_TONGHOP:
                    eItem = ReportName.TEMP25aTRATHUOC_CHITIET;
                    break;
                case ReportName.TEMP26a_TONGHOP:
                    eItem = ReportName.TEMP26a_CHITIET;
                    break;
                //case ReportName.TEMP79a_TONGHOP:
                //    eItem = ReportName.TEMP79a_CHITIET;
                //    break;
                case ReportName.TEMP79aTRATHUOC_TONGHOP:
                    eItem = ReportName.TEMP79aTRATHUOC_CHITIET;
                    break;
                case ReportName.TEMP80a_TONGHOP:
                    eItem = ReportName.TEMP80a_CHITIET;
                    break;
            }
        }

        public void rdtTongHop_Checked(object sender, RoutedEventArgs e)
        {
            if (eItem == ReportName.TEMP79a_CHITIET || eItem == ReportName.TEMP79a_TONGHOP
                || eItem == ReportName.TEMP80a_CHITIET || eItem == ReportName.TEMP80a_TONGHOP
                || eItem == ReportName.BCSuDungToanBV)
            {
                RptParameters.IsDetail = false;
            }
            IsDetail = false;
            switch (_eItem)
            {
                case ReportName.TEMP25a_CHITIET:
                    eItem = ReportName.TEMP25a_TONGHOP;
                    break;
                case ReportName.TEMP25aTRATHUOC_CHITIET:
                    eItem = ReportName.TEMP25aTRATHUOC_TONGHOP;
                    break;
                case ReportName.TEMP26a_CHITIET:
                    eItem = ReportName.TEMP26a_TONGHOP;
                    break;
                //case ReportName.TEMP79a_CHITIET:
                //    eItem = ReportName.TEMP79a_TONGHOP;
                //    break;
                case ReportName.TEMP79aTRATHUOC_CHITIET:
                    eItem = ReportName.TEMP79aTRATHUOC_TONGHOP;
                    break;
                case ReportName.TEMP80a_CHITIET:
                    eItem = ReportName.TEMP80a_TONGHOP;
                    break;
            }
        }

        public void rdtTreatedAsInPt_Checked(object sender, RoutedEventArgs e)
        {
            NotTreatedAsInPt = false;
        }

        public void rdtNotTreatedAsInPt_Checked(object sender, RoutedEventArgs e)
        {
            NotTreatedAsInPt = true;
        }

        private bool GetParameters()
        {
            bool result = true;
            if (CurrentCondition == null)
            {
                CurrentCondition = new Condition(eHCMSResources.Z0938_G1_TheoQuy, 0);
            }
            if (CurrentCondition.Value == 0)
            {
                RptParameters.Flag = 0;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.Q0486_G1_Quy.ToUpper()) + RptParameters.Quarter.ToString()
                    + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();
            }
            else if (CurrentCondition.Value == 1)
            {
                RptParameters.Flag = 1;
                RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G0039_G1_Th.ToUpper()) + RptParameters.Month.ToString()
                    + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();
            }
            else if (CurrentCondition.Value == 2)
            {
                RptParameters.Flag = 2;
                if (RptParameters.FromDate == null || RptParameters.ToDate == null)
                {
                    MessageBox.Show(eHCMSResources.K0364_G1_ChonNgThCanXemBC);
                    result = false;
                }
                else
                {
                    RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                        + string.Format(" - {0}", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
            }
            //-----17/10/2017 DPT bao cao 15 ngay
            else
            {
                RptParameters.Flag = 3;
                if (RptParameters.FromDate == null || RptParameters.ToDate == null)
                {
                    MessageBox.Show(eHCMSResources.K0364_G1_ChonNgThCanXemBC);
                    result = false;
                }
                else
                {
                    TimeSpan Time = Convert.ToDateTime(RptParameters.ToDate) - Convert.ToDateTime(RptParameters.FromDate);
                    if (Time.Days < 0)
                    {
                        MessageBox.Show(" Thời gian xem báo cáo không hợp lệ");
                        result = false;
                    }
                    else
                    {
                        if (SelectedOutInvoice != null)
                        {
                            if (SelectedOutInvoice.PatientRegistration.AdmissionDate == null)
                            {
                                MessageBox.Show("Bệnh nhân " + SelectedOutInvoice.PatientRegistration.Patient.FullName + " chưa nhập viện, bạn không thể xem báo cáo");
                                result = false;
                            }
                            else
                            {
                                TimeSpan Time1 = Convert.ToDateTime(RptParameters.ToDate) - Convert.ToDateTime(SelectedOutInvoice.PatientRegistration.AdmissionDate);
                                if (Time1.Days < 0)
                                {
                                    MessageBox.Show("Bệnh nhân " + SelectedOutInvoice.PatientRegistration.Patient.FullName + " nhập viện sau khoản thời gian muốn báo cáo");
                                    result = false;
                                }
                                else
                                {
                                    RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                                        + string.Format(" - {0}", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                                }
                            }
                        }
                        else
                        {
                            RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.G1933_G1_TuNg.ToUpper()) + RptParameters.FromDate.GetValueOrDefault().ToString("dd/MM/yyyy")
                                + string.Format(" - {0}", eHCMSResources.K3192_G1_DenNg.ToUpper()) + RptParameters.ToDate.GetValueOrDefault().ToString("dd/MM/yyyy");
                        }
                    }
                }
            }
            RptParameters.FindPatient = PatientTypeIndex;
            return result;
        }

        #region ImportData Member

        public void btn_ImportData()
        {
            switch (_eItem)
            {
                case ReportName.TEMP25a_TONGHOP:
                    InsertData();
                    break;
                case ReportName.TEMP25a_CHITIET:
                    goto case ReportName.TEMP25a_TONGHOP;
                case ReportName.TEMP26a_TONGHOP:
                    InsertData();
                    break;
                case ReportName.TEMP26a_CHITIET:
                    goto case ReportName.TEMP26a_TONGHOP;
            }
        }
        private void InsertData()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0364_G1_ChonNgThCanXemBC);
            }
            else
            {
                InsertDataToImport(FromDate.GetValueOrDefault().ToString("MM/dd/yyyy"), ToDate.GetValueOrDefault().ToString("MM/dd/yyyy"));
            }
        }
        private void InsertDataToImport(string begindate, string enddate)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInsertDataToReport(begindate, enddate, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndInsertDataToReport(asyncResult);
                            MessageBox.Show(eHCMSResources.A0614_G1_Msg_InfoImportOK);
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

        #endregion

        public void btnExportExcel()
        {
            SaveFileDialog objSFD = new SaveFileDialog();
            if (Globals.ServerConfigSection.CommonItems.ApplyNewFuncExportExcel && _eItem != ReportName.OutwardDrugsByStaffStatistic
                    && _eItem != ReportName.OutwardDrugsByStaffStatisticDetails
                    && _eItem != ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP)
            {
                objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xlsx",
                    //Filter = "Excel xls (*.xls)|*.xls",
                    //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                    Filter = "Excel(2003) (.xls)|*.xls|Excel(2010) (.xlsx)|*.xlsx",
                    FilterIndex = 2
                };
            }
            else
            {
                objSFD = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = "Excel xls (*.xls)|*.xls",
                    //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                    FilterIndex = 1
                };
            }
            //if (_eItem == ReportName.TEMP79a_CHITIET || _eItem == ReportName.TEMP79a_TONGHOP)
            //{
            //    objSFD.Filter = "CSV file (*.csv)|*.csv";
            //}
            if (objSFD.ShowDialog() != true)
            {
                return;
            }
            //▼===== #006
            if (objSFD != null && objSFD.FileName.Length > 0)
            {
                string[] Name = objSFD.FileName.Split('.');
                if (Name.Length > 2)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z3001_G1_TenFileKhongDuocChuaKyTu, "Dấu chấm"));
                    return;
                }
            }
            //▲===== #006
            //IsProcessing = true;
            RptParameters.reportName = _eItem;
            RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
            if (GetParameters() && _eItem == ReportName.ExportExcel4210)
            {
                ExportToExcelForHIReport(objSFD);
            }
            else
            {
                if (GetParameters())
                {
                    switch (_eItem)
                    {
                        case ReportName.TEMP25aTRATHUOC_CHITIET:
                            RptParameters.Show = "Temp25aTraThuoc";
                            break;
                        case ReportName.TEMP26a_CHITIET:
                            RptParameters.Show = "Temp26a";
                            RptParameters.NotTreatedAsInPt = NotTreatedAsInPt;
                            break;
                        case ReportName.TEMP20_NGOAITRU:
                            RptParameters.Show = "Temp20";
                            break;
                        case ReportName.TEMP20_NGOAITRU_TRATHUOC:
                            RptParameters.Show = "Temp20TraThuoc";
                            break;
                        case ReportName.TEMP20_NOITRU:
                            RptParameters.Show = "Temp20NoiTru";
                            break;
                        case ReportName.TEMP20_VTYTTH:
                            RptParameters.Show = "Temp20VTYTTH";
                            break;
                        case ReportName.TEMP21_NGOAITRU:
                            RptParameters.Show = "Temp21";
                            break;
                        case ReportName.TEMP21_NOITRU:
                            RptParameters.Show = "Temp21NoiTru";
                            break;
                        case ReportName.TEMP19:
                            RptParameters.Show = "Temp19";
                            break;
                        case ReportName.TEMP20_NOITRU_NEW:
                            RptParameters.Show = "Temp20NoiTru";
                            break;
                        case ReportName.TEMP21_NEW:
                            RptParameters.Show = "Temp21";
                            break;
                        case ReportName.TEMP79a_CHITIET:
                        case ReportName.TEMP79a_TONGHOP:
                            RptParameters.Show = "Temp79a";
                            //RptParameters.IsExportToCSV = true;
                            break;
                        case ReportName.TEMP79aTRATHUOC_CHITIET:
                            RptParameters.Show = "Temp79aTraThuoc";
                            break;
                        case ReportName.TEMP80a_CHITIET:
                        case ReportName.TEMP80a_TONGHOP:
                            RptParameters.Show = "Temp80a";
                            RptParameters.NotTreatedAsInPt = NotTreatedAsInPt;
                            break;
                        case ReportName.TRANSACTION_VIENPHICHITIET_PK:
                            RptParameters.Show = "TempVienPhiChiTietPK";
                            break;
                        case ReportName.TRANSACTION_VIENPHICHITIET:
                            RptParameters.Show = "TempVienPhiChiTiet";
                            break;
                        case ReportName.THONGKEDOANHTHU:
                            RptParameters.Show = "TempThongKeDoanhThu";
                            break;
                        case ReportName.REPORT_INPATIENT:
                            RptParameters.Show = "BaoCaoBNDieuTriNoiTru";
                            break;
                        case ReportName.REPORT_GENERAL_TEMP02:
                            RptParameters.Show = "TongHopChiPhiDieuTriNoiTru";
                            break;
                        case ReportName.HOSPITAL_FEES_REPORT:
                            RptParameters.Show = "BaoCaoVienPhiTrai";
                            break;
                        case ReportName.HIGH_TECH_FEES_REPORT:
                            RptParameters.Show = "BaoCaoVienPhiKTC";
                            break;
                        case ReportName.OUT_MEDICAL_MATERIAL_REPORT:
                            RptParameters.Show = "BaoCaoXuatVTYT-KTC";
                            break;
                        case ReportName.HIGH_TECH_FEES_CHILD_REPORT:
                            RptParameters.Show = "BaoCaoVienPhiMoTreEm";
                            break;
                        /*TMA 06/10/2017 PHỤ LỤC 2B*/
                        case ReportName.TransferFormType2Rpt:
                            RptParameters.Show = "TransferFormType2Rpt";
                            break;
                        /*TMA 06/10/2017 PHỤ LỤC 2B*/
                        /*TMA 09/10/2017 PHỤ LỤC 2A*/
                        case ReportName.TransferFormType2_1Rpt:
                            RptParameters.Show = "TransferFormType2_1Rpt";
                            break;
                        /*TMA 09/10/2017 PHỤ LỤC 2A*/
                        /*TMA 12/10/2017 PHỤ LỤC 5*/
                        case ReportName.TransferFormType5Rpt:
                            RptParameters.Show = "TransferFormType5Rpt";
                            break;
                        /*TMA 12/10/2017 PHỤ LỤC 5*/
                        /*DUYNH 20/01/2021  Danh sách chi tiết BN chuyển đi các tuyến*/
                        case ReportName.BCChiTietBNChuyenDi:
                            RptParameters.Show = "BCChiTietBNChuyenDi";
                            break;
                        /**/
                        case ReportName.TRANSFERFORMDATA:
                            RptParameters.Show = eHCMSResources.Z2161_G1_DuLieuChuyenTuyen;
                            break;
                        case ReportName.OutwardDrugsByStaffStatistic:
                            RptParameters.Show = "OutwardDrugsByStaffStatistic";
                            break;
                        case ReportName.OutwardDrugsByStaffStatisticDetails:
                            RptParameters.Show = "OutwardDrugsByStaffStatisticDetails";
                            RptParameters.DrugID = 0;
                            if (SelectedSellVisitor != null)
                            {
                                RptParameters.DrugID = SelectedSellVisitor.DrugID;
                            }
                            break;
                        //▼====: #007
                        case ReportName.OutwardDrugClinicDeptsByStaffStatisticDetails_TP:
                            RptParameters.Show = "OutwardDrugClinicDeptsByStaffStatisticDetails_TP";
                            break;
                        //▲====: #007
                        //▼====: #009
                        case ReportName.ExportExcel_Temp19_THUOC_COVID_KD:
                            RptParameters.Show = "Temp 19 thuoc Covid";
                            break;
                        case ReportName.ExportExcel_Temp19_VTYT_COVID_KD:
                            RptParameters.Show = "Temp 19 vtyt Covid";
                            break;
                        case ReportName.ExportExcel_Temp21_COVID_KT:
                            RptParameters.Show = "Temp 21 Covid";
                            break;
                        //▲====: #009
                        case ReportName.HR_STATISTICS_BY_DEPT:
                        case ReportName.MED_EXAM_ACTIVITY:
                        case ReportName.TREATMENT_ACTIVITY:
                        case ReportName.SPECIALIST_TREATMENT_ACTIVITY:
                        case ReportName.SURGERY_ACTIVITY:
                        case ReportName.REPRODUCTIVE_HEALTH_ACTIVITY:
                        case ReportName.PCL_ACTIVITY:
                        case ReportName.PHARMACY_DEPT_STATISTICS:
                        case ReportName.MEDICAL_EQUIPMENT_STATISTICS:
                        case ReportName.SCIENTIFIC_RESEARCH_ACTIVITY:
                        case ReportName.FINANCIAL_ACTIVITY_TEMP1:
                        case ReportName.FINANCIAL_ACTIVITY_TEMP2:
                        case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                        case ReportName.ICD10_STATISTICS:
                            ExportFromExcel(objSFD);
                            return;
                        case ReportName.BCSuDungToanBV:
                            RptParameters.Show = "BÁO CÁO SỬ DỤNG TOÀN BỆNH VIỆN";
                            break;
                    }

                    //ExportToExcellAllGeneric(objSFD);
                    //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
                    ExportToExcelGeneric.Action(RptParameters, objSFD, this);
                }
            }
        }

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    //IsProcessing = false;
        //    yield break;
        //}

        //export excel all
        #region Export excel from database

        public void ExportToExcellAllGeneric(SaveFileDialog saveFileDialog)
        {
            IsProcessing = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAllGeneric(RptParameters, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToExcellAllGeneric(asyncResult);
                                SaveStreamToExcel(results, saveFileDialog);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                IsProcessing = false;
                            }
                            finally
                            {
                                //isLoadingSearch = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }


        public void ExportFromExcel(SaveFileDialog saveFileDialog)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportFromExcel(RptParameters, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndExportFromExcel(asyncResult);
                                    SaveStreamToExcel(results, saveFileDialog);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.T0432_G1_Error, ex.Message, MessageBoxButton.OK);
                                    //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    MessageBox.Show(eHCMSResources.T0432_G1_Error, ex.Message, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }


        public void ExportToExcellAll_Temp25a(SaveFileDialog saveFileDialog)
        {
            //isLoadingSearch = true;

            IsProcessing = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp25aNew(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp25aNew(asyncResult);
                            //Dinh moi sua cho nay
                            //ExportToExcelFileAllData.ExportAll(results, "Temp25a", filePath);

                            if (!string.IsNullOrEmpty(results))
                            {
                                GetAndSaveStreamToExcel(results, saveFileDialog);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            IsProcessing = false;
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SaveStreamToExcel(byte[] Result, SaveFileDialog saveFileDialog)
        {
            if (Result != null)
            {
                var myStream = saveFileDialog.OpenFile();
                myStream.Write(Result, 0, Result.Length);
                myStream.Close();
                myStream.Dispose();
                MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);
                IsProcessing = false;
            }

        }

        private void GetAndSaveStreamToExcel(string path, SaveFileDialog saveFileDialog)
        {
            IsProcessing = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetVideoAndImage(path, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetVideoAndImage(asyncResult);
                            if (items != null)
                            {
                                var myStream = saveFileDialog.OpenFile();
                                myStream.Write(items, 0, items.Length);
                                myStream.Close();
                                myStream.Dispose();
                                MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);

                            }

                        }
                        catch (Exception ex)
                        {

                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsProcessing = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void ExportToExcellAll_Temp26a()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp26a(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp26a(asyncResult);
                            ExportToExcelFileAllData.Export(results, "Temp26a");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_Temp20NgoaiTru()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp20NgoaiTru(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp20NgoaiTru(asyncResult);
                            ExportToExcelFileAllData.Export(results, "Temp20NgoaiTru");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_Temp20NoiTru()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp20NoiTru(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp20NoiTru(asyncResult);
                            ExportToExcelFileAllData.Export(results, "Temp20NoiTru");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_Temp21NgoaiTru()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp21NgoaiTru(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp21NgoaiTru(asyncResult);
                            ExportToExcelFileAllData.Export(results, "Temp21NgoaiTru");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_Temp21NoiTru()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_Temp21NoiTru(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_Temp21NoiTru(asyncResult);
                            ExportToExcelFileAllData.Export(results, "Temp21NoiTru");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_ChiTietVienPhi_PK()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_ChiTietVienPhi_PK(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_ChiTietVienPhi_PK(asyncResult);
                            ExportToExcelFileAllData.Export(results, "ChiTietVienPhi_PK");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_ChiTietVienPhi()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_ChiTietVienPhi(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_ChiTietVienPhi(asyncResult);
                            ExportToExcelFileAllData.Export(results, "ChiTietVienPhi");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void ExportToExcellAll_ThongKeDoanhThu()
        {
            //isLoadingSearch = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcellAll_ThongKeDoanhThu(RptParameters, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndExportToExcellAll_ThongKeDoanhThu(asyncResult);
                            ExportToExcelFileAllData.Export(results, "ThongKeDoanhThu");
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //isLoadingSearch = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #endregion

        public void LockConditionCombobox()
        {
            if (GetView() == null)
            {
                return;
            }
            IsEnableViewBy = false;
            //((Temp20NgoaiTruView)this.GetView()).cbxCondition.IsEnabled = false;
        }

        #region Thuốc

        //void RefGenMedProductDetailss_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
        //{
        //    SearchRefDrugGenericDetails_AutoPaging(null, BrandName, RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        //}
        private RefGenMedProductSimple _CurrentRefGenMedProductDetails;
        public RefGenMedProductSimple CurrentRefGenMedProductDetails
        {
            get
            {
                return _CurrentRefGenMedProductDetails;
            }
            set
            {
                if (_CurrentRefGenMedProductDetails != value)
                {
                    _CurrentRefGenMedProductDetails = value;
                }
                NotifyOfPropertyChange(() => CurrentRefGenMedProductDetails);
            }
        }
        private bool _mDrug = false;
        public bool mDrug
        {
            get
            {
                return _mDrug;
            }
            set
            {
                if (_mDrug == value)
                {
                    return;
                }
                _mDrug = value;
                NotifyOfPropertyChange(() => mDrug);
            }
        }
        private string BrandName;

        private PagedSortableCollectionView<RefGenMedProductSimple> _RefGenMedProductDetailss;
        public PagedSortableCollectionView<RefGenMedProductSimple> RefGenMedProductDetailss
        {
            get
            {
                return _RefGenMedProductDetailss;
            }
            set
            {
                if (_RefGenMedProductDetailss != value)
                {
                    _RefGenMedProductDetailss = value;
                }
                NotifyOfPropertyChange(() => RefGenMedProductDetailss);
            }
        }

        public long V_MedProductType { get; set; }
        //AutoCompleteBox au;
        //public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        //{
        //    au = sender as AutoCompleteBox;
        //    BrandName = e.Parameter;
        //    RefGenMedProductDetailss.PageIndex = 0;
        //    SearchRefDrugGenericDetails_AutoPaging(null, e.Parameter, 0, RefGenMedProductDetailss.PageSize);
        //}
        //public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    RefGenMedProductSimple obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductSimple;
        //    if (CurrentRefGenMedProductDetails != null)
        //    {
        //        if (CurrentRefGenMedProductDetails.BrandName != obj.BrandName)
        //        {
        //            CurrentRefGenMedProductDetails = obj;
        //        }
        //    }
        //    else
        //    {
        //        CurrentRefGenMedProductDetails = obj;
        //    }
        //}

        //private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        //{
        //    int totalCount = 0;
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginRefGenMedProductDetails_SimpleAutoPaging(IsCode, Name, V_MedProductType, null, PageSize, PageIndex, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var ListUnits = contract.EndRefGenMedProductDetails_SimpleAutoPaging(out totalCount, asyncResult);
        //                    if (IsCode.GetValueOrDefault())
        //                    {
        //                        if (ListUnits != null && ListUnits.Count > 0)
        //                        {
        //                            CurrentRefGenMedProductDetails = ListUnits.FirstOrDefault();
        //                        }
        //                        else
        //                        {
        //                            CurrentRefGenMedProductDetails = null;

        //                            if (au != null)
        //                            {
        //                                au.Text = "";
        //                            }
        //                            MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (ListUnits != null)
        //                        {
        //                            RefGenMedProductDetailss.Clear();
        //                            RefGenMedProductDetailss.TotalItemCount = totalCount;
        //                            RefGenMedProductDetailss.ItemCount = totalCount;
        //                            RefGenMedProductDetailss.SourceCollection = ListUnits;
        //                            NotifyOfPropertyChange(() => RefGenMedProductDetailss);
        //                        }
        //                        au.ItemsSource = RefGenMedProductDetailss;
        //                        au.PopulateComplete();
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}
        //public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    AxTextBox obj = sender as AxTextBox;

        //    if (obj == null || string.IsNullOrWhiteSpace(obj.Text))
        //    {
        //        return;
        //    }

        //    string Code = Globals.FormatCode(V_MedProductType, obj.Text);

        //    if (CurrentRefGenMedProductDetails != null)
        //    {
        //        if (CurrentRefGenMedProductDetails.Code.ToLower() != obj.Text.ToLower())
        //        {
        //            SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, RefGenMedProductDetailss.PageSize);
        //        }
        //    }
        //    else
        //    {
        //        SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, RefGenMedProductDetailss.PageSize);
        //    }
        //}

        private bool? IsCode = false;
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

        AutoCompleteBox au;
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
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
        private ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy;

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
                    NotifyOfPropertyChange(() => SelectedOutwardInfo);
                }
            }
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

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForSellVisitor(txt, true, GetDrugForSellVisitorDisplays.PageSize, GetDrugForSellVisitorDisplays.PageIndex);
                }
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
        private void RefeshData()
        {
            SelectedOutwardInfo = new OutwardDrugInvoice();
            SelectedOutwardInfo.OutDate = Globals.ServerDate.Value;
            SelectedOutwardInfo.TypID = (long)AllLookupValues.RefOutputType.BANLE;
            SelectedOutwardInfo.OutwardDrugs = new ObservableCollection<OutwardDrug>();

            GetDrugForSellVisitorListSum = null;
            GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();

            GetDrugForSellVisitorTemp = null;
            GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();


            ListOutwardDrugFirstCopy = null;
            ListOutwardDrugFirstCopy = new ObservableCollection<OutwardDrug>();

            BrandName = "";
        }
        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode, int PageSize, int PageIndex)
        {

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGennericDrugDetails_GetRemaining_Paging(Name, 2, IsCode, PageSize, PageIndex
                        , false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndRefGennericDrugDetails_GetRemaining_Paging(out Total, asyncResult);
                            GetDrugForSellVisitorTemp = results.ToObservableCollection().DeepCopy();
                            GetDrugForSellVisitorListSum = null;
                            GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
                            //GetDrugForSellVisitorListSum.Clear();

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
                                }
                                GetDrugForSellVisitorListSum.Add(s);
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
                                    GetDrugForSellVisitorDisplays.Clear();
                                    GetDrugForSellVisitorDisplays.TotalItemCount = Total;
                                    foreach (GetDrugForSellVisitor p in GetDrugForSellVisitorListSum)
                                    {
                                        GetDrugForSellVisitorDisplays.Add(p);
                                    }

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
                        }

                    }), null);

                }

            });

            t.Start();
        }
        #endregion

        //▼====: #003
        private bool _KCBBanDau = false;
        public bool ShowKCBBanDau
        {
            get
            {
                return _KCBBanDau;
            }
            set
            {
                if (_KCBBanDau == value)
                    return;
                _KCBBanDau = value;
                NotifyOfPropertyChange(() => ShowKCBBanDau);
            }
        }

        public void KCBBDBVCode_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypeKCBBD = 1;
        }

        public void KCBBDBVSameProvinceCode_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypeKCBBD = 2;
        }

        public void KCBBDBVDiffProvinceCode_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypeKCBBD = 3;
        }

        public void KCBBDBVAllCode_Checked(object sender, RoutedEventArgs e)
        {
            RptParameters.TypeKCBBD = 0;
        }
        //▲====: #003

        private bool _Only79A = false;
        public bool Only79A
        {
            get
            {
                return _Only79A;
            }
            set
            {
                _Only79A = value;
                NotifyOfPropertyChange(() => Only79A);
            }
        }

        private int _PatientTypeIndex = 2;
        public int PatientTypeIndex
        {
            get => _PatientTypeIndex;
            set
            {
                _PatientTypeIndex = value;
                NotifyOfPropertyChange(() => PatientTypeIndex);
            }
        }

        private Visibility _IsExportExcel4210 = Visibility.Collapsed;
        public Visibility IsExportExcel4210
        {
            get
            { return _IsExportExcel4210; }
            set
            {
                if (_IsExportExcel4210 != value)
                {
                    _IsExportExcel4210 = value;
                    NotifyOfPropertyChange(() => IsExportExcel4210);
                }
            }
        }

        public void ExportToExcelForHIReport(SaveFileDialog saveFileDialog)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginExportToExcelForHIReport(RptParameters, PatientTypeIndex
                        , Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndExportToExcelForHIReport(asyncResult);
                                SaveStreamToExcel(results, saveFileDialog);
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
    }
}
