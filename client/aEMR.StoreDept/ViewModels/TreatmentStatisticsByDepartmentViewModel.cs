using aEMR.Common.Collections;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Controls;

namespace aEMR.StoreDept.ViewModels
{
    [Export(typeof(ITreatmentStatisticsByDepartment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class TreatmentStatisticsByDepartmentViewModel : Conductor<object>, ITreatmentStatisticsByDepartment
    {
        #region Properties
        private ObservableCollection<int> _QuarterCollection = new ObservableCollection<int> { 1, 2, 3, 4 };
        public ObservableCollection<int> QuarterCollection
        {
            get
            {
                return _QuarterCollection;
            }
            set
            {
                if (_QuarterCollection == value)
                {
                    return;
                }
                _QuarterCollection = value;
                NotifyOfPropertyChange(() => QuarterCollection);
            }
        }
        private int _SelectedQuarter = 1;
        public int SelectedQuarter
        {
            get
            {
                return _SelectedQuarter;
            }
            set
            {
                if (_SelectedQuarter == value)
                {
                    return;
                }
                _SelectedQuarter = value;
                NotifyOfPropertyChange(() => SelectedQuarter);
            }
        }
        private ObservableCollection<int> _YearCollection = new ObservableCollection<int>();
        public ObservableCollection<int> YearCollection
        {
            get
            {
                return _YearCollection;
            }
            set
            {
                if (_YearCollection == value)
                {
                    return;
                }
                _YearCollection = value;
                NotifyOfPropertyChange(() => YearCollection);
            }
        }
        private int _SelectedYear = DateTime.Now.Year;
        public int SelectedYear
        {
            get
            {
                return _SelectedYear;
            }
            set
            {
                if (_SelectedYear == value)
                {
                    return;
                }
                _SelectedYear = value;
                NotifyOfPropertyChange(() => SelectedYear);
            }
        }
        private ObservableCollection<DiseasesReference> _SummaryDiseasesReference;
        public ObservableCollection<DiseasesReference> SummaryDiseasesReference
        {
            get
            {
                return _SummaryDiseasesReference;
            }
            set
            {
                if (_SummaryDiseasesReference == value)
                {
                    return;
                }
                _SummaryDiseasesReference = value;
                NotifyOfPropertyChange(() => SummaryDiseasesReference);
            }
        }
        private decimal _TotalQuantity = 0;
        public decimal TotalQuantity
        {
            get
            {
                return _TotalQuantity;
            }
            set
            {
                if (_TotalQuantity == value)
                {
                    return;
                }
                _TotalQuantity = value;
                NotifyOfPropertyChange(() => TotalQuantity);
            }
        }
        private decimal? _AverageMinute = 0;
        public decimal? AverageMinute
        {
            get
            {
                return _AverageMinute;
            }
            set
            {
                if (_AverageMinute == value)
                {
                    return;
                }
                _AverageMinute = value;
                NotifyOfPropertyChange(() => AverageMinute);
            }
        }
        private decimal? _AverageAmount = 0;
        public decimal? AverageAmount
        {
            get
            {
                return _AverageAmount;
            }
            set
            {
                if (_AverageAmount == value)
                {
                    return;
                }
                _AverageAmount = value;
                NotifyOfPropertyChange(() => AverageAmount);
            }
        }
        private ObservableCollection<PatientRegistration> _RegistrationCollection;
        public ObservableCollection<PatientRegistration> RegistrationCollection
        {
            get
            {
                return _RegistrationCollection;
            }
            set
            {
                if (_RegistrationCollection == value)
                {
                    return;
                }
                _RegistrationCollection = value;
                NotifyOfPropertyChange(() => RegistrationCollection);
            }
        }
        private bool _IsSearchDetailView = false;
        public bool IsSearchDetailView
        {
            get
            {
                return _IsSearchDetailView;
            }
            set
            {
                if (_IsSearchDetailView == value)
                {
                    return;
                }
                _IsSearchDetailView = value;
                NotifyOfPropertyChange(() => IsSearchDetailView);
            }
        }
        private enum ReportMethod
        {
            ByQuarter,
            ByDay
        }
        private ReportMethod CurrentReportMethod = ReportMethod.ByQuarter;
        public bool IsReportByQuarter
        {
            get
            {
                return CurrentReportMethod == ReportMethod.ByQuarter;
            }
        }
        private DateTime _StartDate = Globals.GetCurServerDateTime().AddMonths(-1).Date;
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                if (_StartDate == value)
                {
                    return;
                }
                _StartDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        private DateTime _EndDate = Globals.GetCurServerDateTime().Date;
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                {
                    return;
                }
                _EndDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        private enum RegistrationStatus : byte
        {
            Unknow = 0,
            Discharged = 1,
            Other = 2
        }
        private RegistrationStatus CurrentReportRegistrationStatus = RegistrationStatus.Discharged;
        private IDepartmentListing _DepartmentContent;
        public IDepartmentListing DepartmentContent
        {
            get
            {
                return _DepartmentContent;
            }
            set
            {
                if (_DepartmentContent == value)
                {
                    return;
                }
                _DepartmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }
        #endregion
        #region Events
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public TreatmentStatisticsByDepartmentViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            for (int i = DateTime.Now.Year; i >= 2015; i--)
            {
                YearCollection.Add(i);
            }
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = false;
            DepartmentContent.AddSelectedAllItem = false;
            //20200418 TBL: DesignerProperties không có IsInDesignTool và không biết cái này để làm gì nên tạm comment ra để build được
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    DepartmentContent.LoadData();
            //}
            DepartmentContent.LoadData();
        }
        public void SearchCmd()
        {
            IsSearchDetailView = false;
            if (DepartmentContent.SelectedItem == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new TransactionServiceClient())
                    {
                        GetDateCriteria();

                        var mContract = aFactory.ServiceInstance;
                        mContract.BeginGetTreatmentStatisticsByDepartment(DepartmentContent.SelectedItem.DeptID, StartDate, EndDate, (byte)CurrentReportRegistrationStatus, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                decimal OutTotalQuantity;
                                var CurrentCollection = mContract.EndGetTreatmentStatisticsByDepartment(out OutTotalQuantity, asyncResult);
                                TotalQuantity = OutTotalQuantity;
                                if (CurrentCollection == null)
                                {
                                    SummaryDiseasesReference = new ObservableCollection<DiseasesReference>();
                                }
                                else
                                {
                                    SummaryDiseasesReference = CurrentCollection.ToObservableCollection();
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void SearchContentCmd()
        {
            CallSearchContentCmd();
        }
        public void AbnormalCmd()
        {
            if (AverageMinute + AverageAmount == 0)
            {
                return;
            }
            CallSearchContentCmd(AverageMinute, AverageAmount);
        }
        public void ExportExcelCmd()
        {
            CallExportExcel(false);
        }
        public void ExportExcelDetailCmd()
        {
            CallExportExcel(true);
        }
        public void ExportExcelAbnormalCmd()
        {
            if (AverageMinute + AverageAmount == 0)
            {
                return;
            }
            CallExportExcel(true, AverageMinute.GetValueOrDefault(0), AverageAmount.GetValueOrDefault(0));
        }
        public void cboReportMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedIndex == 1)
            {
                CurrentReportMethod = ReportMethod.ByDay;
            }
            else
            {
                CurrentReportMethod = ReportMethod.ByQuarter;
            }
            NotifyOfPropertyChange(() => IsReportByQuarter);
        }
        public void cboRegistrationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    CurrentReportRegistrationStatus = RegistrationStatus.Unknow;
                    break;
                case 1:
                    CurrentReportRegistrationStatus = RegistrationStatus.Discharged;
                    break;
                case 2:
                    CurrentReportRegistrationStatus = RegistrationStatus.Other;
                    break;
            }
        }
        #endregion
        #region Methods
        private void CallSearchContentCmd(decimal? aAverageMinute = null, decimal? AverageAmount = null)
        {
            IsSearchDetailView = true;
            if (DepartmentContent.SelectedItem == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var aFactory = new TransactionServiceClient())
                    {
                        GetDateCriteria();

                        var mContract = aFactory.ServiceInstance;
                        mContract.BeginGetTreatmentStatisticsByDepartmentDetail(DepartmentContent.SelectedItem.DeptID, StartDate, EndDate, (byte)CurrentReportRegistrationStatus, aAverageMinute, AverageAmount, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var CurrentCollection = mContract.EndGetTreatmentStatisticsByDepartmentDetail(asyncResult);
                                if (CurrentCollection == null)
                                {
                                    RegistrationCollection = new ObservableCollection<PatientRegistration>();
                                }
                                else
                                {
                                    RegistrationCollection = CurrentCollection.ToObservableCollection();
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void GetDateCriteria()
        {
            if (IsReportByQuarter)
            {
                StartDate = new DateTime(SelectedYear, 1, 1);
                if (SelectedQuarter == 1)
                {
                    StartDate = new DateTime(SelectedYear, 1, 1);
                }
                else if (SelectedQuarter == 2)
                {
                    StartDate = new DateTime(SelectedYear, 4, 1);
                }
                else if (SelectedQuarter == 3)
                {
                    StartDate = new DateTime(SelectedYear, 7, 1);
                }
                else
                {
                    StartDate = new DateTime(SelectedYear, 10, 1);
                }
                EndDate = StartDate.AddMonths(3);
                EndDate = EndDate.AddSeconds(-1);
            }
        }
        private void CallExportExcel(bool IsSearchDetailView, decimal aAverageMinute = 0, decimal aAverageAmount = 0)
        {
            if (DepartmentContent.SelectedItem == null)
            {
                return;
            }
            SaveFileDialog CurrentFileDialog = new SaveFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "CSV file (*.csv)|*.csv",
                FilterIndex = 1
            };
            if (CurrentFileDialog.ShowDialog() != true)
            {
                return;
            }
            GetDateCriteria();
            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
            if (IsSearchDetailView)
            {
                RptParameters.reportName = ReportName.TreatsStatisticsByDept_Detail;
            }
            else
            {
                RptParameters.reportName = ReportName.TreatsStatisticsByDept;
            }
            RptParameters.FromDate = StartDate;
            RptParameters.ToDate = EndDate;
            RptParameters.DeptID = DepartmentContent.SelectedItem.DeptID;
            RptParameters.IsExportToCSV = true;
            RptParameters.AverageTime = aAverageMinute;
            RptParameters.AverageAmount = aAverageAmount;
            RptParameters.RegistrationStatus = (byte)CurrentReportRegistrationStatus;
            ExportToExcelGeneric.Action(RptParameters, CurrentFileDialog, this);
        }
        public void grdRegistrationDetail_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            PatientRegistration rowItem = e.Row.DataContext as PatientRegistration;
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        #endregion
    }
}