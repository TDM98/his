using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Linq;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.ComponentModel;
using Microsoft.Win32;
using DevExpress.ReportServer.Printing;
/*
* 20171004 #001 CMN: Added fromdate and todate.
* 20200315 #002 TNHX: Added IsNewForm for get new form of report 
*/
namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IRptBaoCaoTheoDot)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RptBaoCaoTheoDotViewModel : Conductor<object>, IRptBaoCaoTheoDot
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RptBaoCaoTheoDotViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = false;
            DepartmentContent.AddSelectedAllItem = true;
            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                DepartmentContent.LoadData();
            }
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(RptBaoCaoTheoDotViewModel_PropertyChanged);
            RptParameters = new ReportParameters();
            FillCondition();
            FillYear();
            RptParameters.FromDate = DateTime.Now;
            RptParameters.ToDate = DateTime.Now;
        }
        // -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
        protected override void OnActivate()
        {
            base.OnActivate();
            if (eItem == ReportName.SCIENTIFIC_RESEARCH_ACTIVITY || eItem == ReportName.HoatDongChiDaoTuyen)
            {
                FillQuarter(true);
            }
            else { FillQuarter(); }
            
        }
        // -------------- DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
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
        }
        private bool _mViewAndPrint = true;
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
        void RptBaoCaoTheoDotViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        #region Properties
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

        private bool _IsProcessing=false;
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
        /*▼====: #001*/
        public class Condition
        {
            public string Text { get; }
            public long Value { get; }
            public Condition(string theText, long theValue)
            {
                Text = theText;
                Value = theValue;
            }
        }
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
        /*▲====: #001*/
        private string _strHienThi = Globals.PageName;
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

        private bool _IsNewForm = true;
        public bool IsNewForm
        {
            get
            {
                return _IsNewForm;
            }
            set
            {
                _IsNewForm = value;
                NotifyOfPropertyChange(() => IsNewForm);
            }
        }

        private bool _ShowIsNewForm = false;
        public bool ShowIsNewForm
        {
            get
            {
                return _ShowIsNewForm;
            }
            set
            {
                _ShowIsNewForm = value;
                NotifyOfPropertyChange(() => ShowIsNewForm);
            }
        }
        #endregion

        #region FillData Member
        private void FillQuarter(bool IsMidYear = false)
        {
            if (ListQuartar == null)
            {
                ListQuartar = new ObservableCollection<int>();
            }
            else
            {
                ListQuartar.Clear();
            }
            if (!IsMidYear)
                ListQuartar = new ObservableCollection<int> { 3, 6, 9, 12 };
            else
                ListQuartar = new ObservableCollection<int> { 6, 12 };
            int Month = DateTime.Now.Month;
            if (Month <= 3)
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 3;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 6;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 9;
            }
            else // 4th Quarter = October 1 to December 31
            {
                // 1st Quarter = January 1 to March 31
                RptParameters.Quarter = 12;
            }
            if (IsMidYear)
            {
                if (RptParameters.Quarter == 9)
                    RptParameters.Quarter = 12;
                else if (RptParameters.Quarter == 3)
                    RptParameters.Quarter = 6;
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
        #endregion

        DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
        private void GetReport()
        {
            switch (_eItem)
            {
                case ReportName.TRANSACTION_CANLAMSAN:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp06CLS").PreviewModel;
                    break;
                case ReportName.TRANSACTION_DUOCBV:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp07DuocBV").PreviewModel;
                    break;
                case ReportName.TRANSACTION_HOATDONGKB:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_Temp02HoatDongKB").PreviewModel;
                    break;
                case ReportName.HoatDongChiDaoTuyen:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_SCIENTIFIC_RESEARCH_ACTIVITY").PreviewModel;
                    break;
                case ReportName.SCIENTIFIC_RESEARCH_ACTIVITY:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRpt_SCIENTIFIC_RESEARCH_ACTIVITY").PreviewModel;
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
                case ReportName.FINANCIAL_ACTIVITY_TEMP1:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptFinancialActivityTemp01").PreviewModel;
                    break;
                case ReportName.FINANCIAL_ACTIVITY_TEMP2:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptFinancialActivityTemp02").PreviewModel;
                    break;
                case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                    ReportModel = null;
                    ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.RptFinancialActivityTemp03").PreviewModel;
                    break;
                case ReportName.FollowICD:
                    ReportModel = null;
                    if (IsNewForm)
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptHISSummaryFollowByICD_Bieu14").PreviewModel;
                    }
                    else
                    {
                        ReportModel = new GenericReportModel("eHCMS.ReportLib.RptTransactions.XRptHISSummaryFollowByICD_New").PreviewModel;
                    }
                    break;
            }
            //ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            if (ReportModel != null)
            {
                //ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
                FillReport_ParameterValues();
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        //public void _reportModel_RequestDefaultParameterValues(object sender, System.EventArgs e)
        public void FillReport_ParameterValues()
        {
            switch (_eItem)
            {

                case ReportName.TRANSACTION_CANLAMSAN:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["TPKH"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PGD"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.TRANSACTION_DUOCBV:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["TPKH"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PGD"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.TRANSACTION_HOATDONGKB:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["TPKH"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PGD"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.HoatDongChiDaoTuyen:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["TPKH"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PGD"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.SCIENTIFIC_RESEARCH_ACTIVITY:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["TPKH"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PGD"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.TREATMENT_ACTIVITY:
                    rParams["IsSpecialist"].Value = false;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.SPECIALIST_TREATMENT_ACTIVITY:
                    rParams["IsSpecialist"].Value = true;
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.SURGERY_ACTIVITY:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.FINANCIAL_ACTIVITY_TEMP1:
                case ReportName.FINANCIAL_ACTIVITY_TEMP2:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["ChiefAccountant"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 40 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 40 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
                case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["Director"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 1 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 1 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    break;
                case ReportName.FollowICD:
                    rParams["FromDate"].Value = RptParameters.FromDate;
                    rParams["ToDate"].Value = RptParameters.ToDate;
                    rParams["LoggedUserAccount"].Value = Globals.LoggedUserAccount.Staff == null || Globals.LoggedUserAccount == null ? null : Globals.LoggedUserAccount.Staff.FullName;
                    rParams["ViceDirector"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 4 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 4 && x.IsActive).FirstOrDefault().FullNameString : null;
                    rParams["PlanningManager"].Value = Globals.allStaffPositions.Any(x => x.PositionRefID == 50 && x.IsActive) ? Globals.allStaffPositions.Where(x => x.PositionRefID == 50 && x.IsActive).FirstOrDefault().FullNameString : null;
                    break;
            }
        }

        private void CreateParameter()
        {
            if (CurrentCondition.Value == 0)
            {
                RptParameters.FromDate = new DateTime(RptParameters.Year, 1, 1);
                switch (Convert.ToInt32(RptParameters.Quarter))
                {
                    case 3:
                        RptParameters.ToDate = new DateTime(RptParameters.Year, 3, 31, 23, 59, 59);
                        break;
                    case 6:
                        RptParameters.ToDate = new DateTime(RptParameters.Year, 6, 30, 23, 59, 59);
                        break;
                    case 9:
                        RptParameters.ToDate = new DateTime(RptParameters.Year, 9, 30, 23, 59, 59);
                        break;
                    case 12:
                        RptParameters.ToDate = new DateTime(RptParameters.Year, 12, 31, 23, 59, 59);
                        break;
                }
            }
        }

        public void btnXemIn(object sender, EventArgs e)
        {
            /*▼====: #001*/
            CreateParameter();
            /*▲====: #001*/
            //KMx: Nếu là báo cáo bệnh nhân điều trị nội trú của Y Vụ thì không được xem/in vì chưa làm (03/06/2015 11:36)
            if (GetParameters())
            {
                GetReport();
            }
        }

        public void btnIn(object sender, EventArgs e)
        {
            MessageBox.Show(eHCMSResources.A0073_G1_CNangDangHThien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            return;
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
            RptParameters.Flag = 0;
            RptParameters.Show = RptParameters.ShowFirst + string.Format(" {0} ", eHCMSResources.Q0486_G1_Quy.ToUpper()) + RptParameters.Quarter.ToString()
                + string.Format(" {0} ", eHCMSResources.N0033_G1_Nam.ToUpper()) + RptParameters.Year.ToString();
            return result;
        }
        /*▼====: #001*/
        #region Methods
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
            Conditions.Add(new Condition(eHCMSResources.Z2129_G1_TheoDot, 0));
            Conditions.Add(new Condition(eHCMSResources.G0375_G1_TheoNg, 1));
            CurrentCondition = Conditions.FirstOrDefault();
        }
        private void ByDate()
        {
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
        }
        private void ByQuarter()
        {
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Visible;
            IsYear = Visibility.Visible;
        }
        #endregion

        #region Events
        public void cbxCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CurrentCondition.Value)
            {
                case 0:
                    ByQuarter();
                    break;
                case 1:
                    ByDate();
                    break;
            }
        }
        public void btnExportExcel()
        {
            if (eItem != ReportName.FINANCIAL_ACTIVITY_TEMP3 && eItem != ReportName.FollowICD) return;
            CreateParameter();
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }
            RptParameters.reportName = eItem;
            RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;

            if (GetParameters())
            {
                switch (eItem)
                {
                    case ReportName.FINANCIAL_ACTIVITY_TEMP3:
                        RptParameters.Show = "Financial_V3";
                        break;
                    case ReportName.FollowICD:
                        RptParameters.IsNewForm = IsNewForm;
                        break;
                }
                ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            }
        }
        #endregion
        /*▲====: #001*/
    }
}
