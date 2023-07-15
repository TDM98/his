using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using System.Windows.Controls;
using Microsoft.Win32;
/*
 * 20200404 #001 TTM: BM 0029080: Bổ sung nút xuất dữ liệu cổng QG theo ngày tháng.
 * 20200414 #002 TTM: BM 0032119: Đánh dấu mã giao dịch cho các ca báo cáo bằng thủ công thông qua VAS.
 */

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(ICreateHIReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CreateHIReportViewModel : ViewModelBase, ICreateHIReport
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CreateHIReportViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            AllHIReportType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HIReportType).ToObservableCollection();

            InitializeHIReport();
        }

        private HealthInsuranceReport _HIReport;
        public HealthInsuranceReport HIReport
        {
            get
            {
                return _HIReport;
            }
            set
            {
                _HIReport = value;
                NotifyOfPropertyChange(() => HIReport);
            }
        }

        private List<HealthInsuranceReport> _HIReportList;
        public List<HealthInsuranceReport> HIReportList
        {
            get
            {
                return _HIReportList;
            }
            set
            {
                if (_HIReportList != value)
                {
                    _HIReportList = value;
                    NotifyOfPropertyChange(() => HIReportList);
                }
            }
        }

        private ObservableCollection<Lookup> _AllHIReportType;
        public ObservableCollection<Lookup> AllHIReportType
        {
            get
            {
                return _AllHIReportType;
            }
            set
            {
                if (_AllHIReportType != value)
                {
                    _AllHIReportType = value;
                    NotifyOfPropertyChange(() => AllHIReportType);
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

        private Visibility _IsTime;
        public Visibility IsTime
        {
            get
            {
                return _IsTime;
            }
            set
            {
                if (_IsTime == value) return;
                _IsTime = value;
                NotifyOfPropertyChange(() => IsTime);
            }
        }

        private Visibility _gIsByRegistrationID;
        public Visibility gIsByRegistrationID
        {
            get
            {
                return _gIsByRegistrationID;
            }
            set
            {
                if (_gIsByRegistrationID == value) return;
                _gIsByRegistrationID = value;
                NotifyOfPropertyChange(() => gIsByRegistrationID);
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

        private ObservableCollection<int> _ListQuarter;
        public ObservableCollection<int> ListQuarter
        {
            get { return _ListQuarter; }
            set
            {
                if (_ListQuarter != value)
                {
                    _ListQuarter = value;
                    NotifyOfPropertyChange(() => ListQuarter);
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

        private void InitializeHIReport()
        {
            HIReport = new HealthInsuranceReport();
            HIReport.Staff = new Staff();
            HIReport.Staff = Globals.LoggedUserAccount.Staff;
            HIReport.V_HIReportType = AllHIReportType.FirstOrDefault();
            HIReport.FromDate = Globals.GetCurServerDateTime();
            HIReport.ToDate = Globals.GetCurServerDateTime();
            FillMonth();
            FillQuarter();
            FillYear();
            SwitchHIReportType();
        }

        public void SwitchHIReportType()
        {
            if (HIReport == null || HIReport.V_HIReportType == null)
            {
                return;
            }
            switch (HIReport.V_HIReportType.LookupID)
            {
                case (long)AllLookupValues.V_HIReportType.DATE:
                    ByDate();
                    break;
                case (long)AllLookupValues.V_HIReportType.MONTH:
                    ByMonth();
                    break;
                case (long)AllLookupValues.V_HIReportType.QUARTER:
                    ByQuarter();
                    break;
                case (long)AllLookupValues.V_HIReportType.YEAR:
                    ByYear();
                    break;
                case (long)AllLookupValues.V_HIReportType.TIME:
                    ByTime();
                    break;
                case (long)AllLookupValues.V_HIReportType.REGID:
                    ByRegID();
                    break;
                default:
                    ByDate();
                    break;
            }
        }

        public void cbxHIReportType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SwitchHIReportType();
        }

        private void ByDate()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
            IsTime = Visibility.Collapsed;
            IsExportDateForCongQG = Visibility.Collapsed;
            gIsByRegistrationID = Visibility.Collapsed;
        }

        private void ByMonth()
        {
            IsMonth = Visibility.Visible;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
            IsTime = Visibility.Collapsed;
            IsExportDateForCongQG = Visibility.Collapsed;
            gIsByRegistrationID = Visibility.Collapsed;
        }

        private void ByQuarter()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Visible;
            IsYear = Visibility.Visible;
            IsTime = Visibility.Collapsed;
            IsExportDateForCongQG = Visibility.Collapsed;
            gIsByRegistrationID = Visibility.Collapsed;
        }

        private void ByYear()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Visible;
            IsTime = Visibility.Collapsed;
            IsExportDateForCongQG = Visibility.Collapsed;
            gIsByRegistrationID = Visibility.Collapsed;
        }

        private void ByTime()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Visible;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
            IsTime = Visibility.Visible;
            IsExportDateForCongQG = Visibility.Visible;
            gIsByRegistrationID = Visibility.Collapsed;
        }

        private void ByRegID()
        {
            IsMonth = Visibility.Collapsed;
            IsDate = Visibility.Collapsed;
            IsQuarter = Visibility.Collapsed;
            IsYear = Visibility.Collapsed;
            IsTime = Visibility.Collapsed;
            IsExportDateForCongQG = Visibility.Collapsed;
            gIsByRegistrationID = Visibility.Visible;
        }

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
            HIReport.Month = Globals.GetCurServerDateTime().Month;
        }

        private void FillQuarter()
        {
            if (ListQuarter == null)
            {
                ListQuarter = new ObservableCollection<int>();
            }
            else
            {
                ListQuarter.Clear();
            }
            for (int i = 1; i < 5; i++)
            {
                ListQuarter.Add(i);
            }
            int Month = Globals.GetCurServerDateTime().Month;
            if (Month <= 3)
            {
                HIReport.Quarter = 1;
            }
            else if ((Month >= 4) && (Month <= 6))
            {
                HIReport.Quarter = 2;
            }
            else if ((Month >= 7) && (Month <= 9))
            {
                HIReport.Quarter = 3;
            }
            else
            {
                HIReport.Quarter = 4;
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
            int year = Globals.GetCurServerDateTime().Year;
            for (int i = year - 2; i < year + 1; i++)
            {
                ListYear.Add(i);
            }
            HIReport.Year = year;
        }

        public void grdHIReport_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void CreateReportCmd()
        {
            CreateHIReport();
        }

        private void CreateHIReport()
        {
            if (HIReport == null || HIReport.V_HIReportType == null || string.IsNullOrEmpty(HIReport.Title))
                return;
            if (HIReport.V_HIReportType.LookupID == (long)AllLookupValues.V_HIReportType.TIME && (HIReport.FromDate == null || HIReport.ToDate == null))
            {
                MessageBox.Show(eHCMSResources.K0426_G1_NhapDayDuNgTh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (HIReport.V_HIReportType.LookupID == (long)AllLookupValues.V_HIReportType.REGID && string.IsNullOrEmpty(HIReport.RegistrationIDList))
            {
                MessageBox.Show(eHCMSResources.Z2199_G1_VuiLongNhapDSMaDotDieuTri, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (HIReport.V_HIReportType.LookupID == (long)AllLookupValues.V_HIReportType.REGID && !string.IsNullOrEmpty(HIReport.RegistrationIDList))
            {
                try
                {
                    HealthInsuranceReport.ConvertIDListToXml(HIReport.RegistrationIDList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return;
                }
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCreateHIReport(HIReport,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var result = contract.EndCreateHIReport(asyncResult);

                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.A0998_G1_Msg_InfoTaoBCOK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0999_G1_Msg_InfoTaoBCFail);
                                    }

                                    GetHIReport();
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.Message);
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        public void SearchCmd()
        {
            GetHIReport();
        }

        private void GetHIReport()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIReport(Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    HIReportList = contract.EndGetHIReport(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.Message);
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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

        private void ExportExcel(object selectItem, ReportName reportName)
        {
            if (selectItem == null)
            {
                return;
            }
            HealthInsuranceReport HIReport_Current = (selectItem as HealthInsuranceReport);
            if (HIReport_Current == null || HIReport_Current.HIReportID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0281_G1_ChonBCDeXuatExcel);
                return;
            }
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel xls (*.xls)|*.xls",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.BAOCAO_BHYT;
            RptParameters.reportName = reportName;
            RptParameters.HIReportID = HIReport_Current.HIReportID;

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
        }

        public void hplExportExcelTemp19_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP19_V2);
        }

        public void hplExportExcelTemp20_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP20_V2);
        }

        public void hplExportExcelTemp21_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP21_V2);
        }

        public void hplExportExcelTemp79_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP79_V2);
        }

        public void hplExportExcelTemp79TraThuoc_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP79_TRATHUOC_V2);
        }

        public void hplExportExcelTemp80_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP80_V2);
        }

        public void hplExportExcelTemp9324_Bang1_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP9324_BANG_1);
        }

        public void hplExportExcelTemp9324_Bang2_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP9324_BANG_2);
        }

        public void hplExportExcelTemp9324_Bang3_Click(object selectItem)
        {
            ExportExcel(selectItem, ReportName.TEMP9324_BANG_3);
        }


        public void ExportXMLReport9324(object selectItem)
        {
            if (selectItem == null)
            {
                return;
            }
            HealthInsuranceReport HIReport_Current = (selectItem as HealthInsuranceReport);
            if (HIReport_Current == null || HIReport_Current.HIReportID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0282_G1_ChonBCDeXuatXML);
                return;
            }
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport9324_AllTab123_InOneRpt(HIReport_Current.HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetHIXmlReport9324_AllTab123_InOneRpt(asyncResult);
                                    SaveStreamToXML(results, objSFD);
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
            t.Start();
        }

        public void SaveStreamToXML(byte[] Result, SaveFileDialog saveFileDialog)
        {
            if (Result != null)
            {
                var myStream = saveFileDialog.OpenFile();
                myStream.Write(Result, 0, Result.Length);
                myStream.Close();
                myStream.Dispose();
                MessageBox.Show(eHCMSResources.A0804_G1_Msg_InfoLuuOK);
            }
        }

        public void hplExportXMLReport9324_Click(object selectItem)
        {
            ExportXMLReport9324(selectItem);
        }

        //▼===== #001
        #region ExportDateForCongQG
        private Visibility _IsExportDateForCongQG;
        public Visibility IsExportDateForCongQG
        {
            get
            { return _IsExportDateForCongQG; }
            set
            {
                if (_IsExportDateForCongQG != value)
                {
                    _IsExportDateForCongQG = value;
                    NotifyOfPropertyChange(() => IsExportDateForCongQG);
                }
            }
        }
        private DateTime _FromDate = Globals.GetCurServerDateTime();
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate = Globals.GetCurServerDateTime();
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        public void ExportDataForCongQG()
        {
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xml",
                Filter = "XML Files (*.xml)|*.xml"
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }
            if (HIReport.FromDate != null && HIReport.ToDate != null)
            {
                FromDate = HIReport.FromDate.Date;
                DateTime TmpToDate = HIReport.ToDate;
                TmpToDate = TmpToDate.AddDays(1);
                ToDate = TmpToDate.AddSeconds(-1);
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetXmlReportForCongDLQG(FromDate, ToDate,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetXmlReportForCongDLQG(asyncResult);
                                    SaveStreamToXML(results, objSFD);
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
            t.Start();
        }
        #endregion
        //▲===== #001

        //▼===== #002
        public void hplCheckVASoK_Click(object source, object sender)
        {
            if (source == null)
            {
                return;
            }
            CheckBox ckbCheckVASoK = source as CheckBox;
            bool? copier = ckbCheckVASoK.IsChecked;
            if (!(ckbCheckVASoK.DataContext is HealthInsuranceReport) || ckbCheckVASoK.DataContext == null)
            {
                return;
            }
            if ((MessageBox.Show(eHCMSResources.Z0553_G1_CoDongYLuuKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)) == MessageBoxResult.Yes)
            {
                MarkHIReportByVAS(ckbCheckVASoK.DataContext as HealthInsuranceReport);
            }
            else
            {
                ckbCheckVASoK.IsChecked = !(bool)copier;
            }
        }

        public void MarkHIReportByVAS(HealthInsuranceReport selectItem)
        {
            HealthInsuranceReport HIReport_Current = (selectItem as HealthInsuranceReport);
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMarkHIReportByVAS(HIReport_Current.HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndMarkHIReportByVAS(asyncResult);
                                    if (results)
                                    {
                                        MessageBox.Show(eHCMSResources.A0457_G1_Msg_InfoDaGhi);
                                        SearchCmd();
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
            t.Start();
        }
        //▲===== #002
    }
}
