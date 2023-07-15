using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System.Windows;
using System.Collections.ObjectModel;
using eHCMSLanguage;
using aEMR.CommonTasks;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Microsoft.Win32;
using System.Windows.Controls;

/*
 * 20180914 #001 TBL: Them STT cho grid
 * 20180914 #002 TBL: Khi chinh sua khong hien o tim kiem benh nhan
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultingDiagnosysReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultingDiagnosysReportViewModel : Conductor<object>, IConsultingDiagnosysReport
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ConsultingDiagnosysReportViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 0, Name = eHCMSResources.T0822_G1_TatCa });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 1, Name = eHCMSResources.Z2095_G1_ChuaHoanTatTTHS, Type = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 2, Name = eHCMSResources.Z2096_G1_DuocNhanHoSo, Type = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 3, Name = eHCMSResources.Z2098_G1_ChoMo, Type = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 4, Name = eHCMSResources.Z2097_G1_DaMo, Type = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 5, Name = eHCMSResources.Z2176_G1_BenhNhanDuraGraft, Type = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 6, Name = eHCMSResources.Z2099_G1_TreHen, Type = 2, ParentCode = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 7, Name = eHCMSResources.Z2100_G1_ChuaDenHen, Type = 2, ParentCode = 1 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 8, Name = eHCMSResources.Z2063_G1_HoanTatXetNghiem, Type = 2, ParentCode = 3 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 9, Name = eHCMSResources.Z2101_G1_ChuaHTatXetNghiem, Type = 2, ParentCode = 3 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 10, Name = eHCMSResources.Z2252_G1_DuKienPhauThuat, Type = 2, ParentCode = 3 });
            gAllReportFilters.Add(new ConsultingDiagnosysReportFilter { Code = 11, Name = eHCMSResources.Z2874_G1_HuyMo, Type = 1 });

            PatientFilterArray = new List<ConsultingDiagnosysReportFilter>(gAllReportFilters.Where(x => x.Type == 1));
            InitPatientFilterArray();
            SelectedPatientFilter = PatientFilterArray.FirstOrDefault();
        }
        #region Propeties
        private List<ConsultingDiagnosysReportFilter> gAllReportFilters = new List<ConsultingDiagnosysReportFilter>();
        private List<ConsultingDiagnosysReportFilter> _PatientFilterArray;
        public List<ConsultingDiagnosysReportFilter> PatientFilterArray
        {
            get
            {
                return _PatientFilterArray;
            }
            set
            {
                _PatientFilterArray = value;
                NotifyOfPropertyChange(() => PatientFilterArray);
            }
        }
        private List<ConsultingDiagnosysReportFilter> _ReportFilterArray;
        public List<ConsultingDiagnosysReportFilter> ReportFilterArray
        {
            get
            {
                return _ReportFilterArray;
            }
            set
            {
                _ReportFilterArray = value;
                NotifyOfPropertyChange(() => ReportFilterArray);
            }
        }
        private ConsultingDiagnosysReportFilter _SelectedPatientFilter;
        public ConsultingDiagnosysReportFilter SelectedPatientFilter
        {
            get
            {
                return _SelectedPatientFilter;
            }
            set
            {
                _SelectedPatientFilter = value;
                NotifyOfPropertyChange(() => SelectedPatientFilter);
                ReportFilterArray = new List<ConsultingDiagnosysReportFilter>(gAllReportFilters.Where(x => x.Code == 0 || (x.Type == 2 && SelectedPatientFilter != null && x.ParentCode == SelectedPatientFilter.Code)));
                SelectedReportFilterArray = ReportFilterArray.FirstOrDefault();
            }
        }
        private ConsultingDiagnosysReportFilter _SelectedReportFilterArray;
        public ConsultingDiagnosysReportFilter SelectedReportFilterArray
        {
            get
            {
                return _SelectedReportFilterArray;
            }
            set
            {
                _SelectedReportFilterArray = value;
                NotifyOfPropertyChange(() => SelectedReportFilterArray);
            }
        }
        private ConsultingDiagnosys _gSelectedConsultingDiagnosys;
        public ConsultingDiagnosys gSelectedConsultingDiagnosys
        {
            get { return _gSelectedConsultingDiagnosys; }
            set
            {
                _gSelectedConsultingDiagnosys = value;
                NotifyOfPropertyChange(() => gSelectedConsultingDiagnosys);
            }
        }
        private ObservableCollection<ConsultingDiagnosys> _ConsultingDiagnosysArray;
        public ObservableCollection<ConsultingDiagnosys> ConsultingDiagnosysArray
        {
            get
            {
                return _ConsultingDiagnosysArray;
            }
            set
            {
                _ConsultingDiagnosysArray = value;
                NotifyOfPropertyChange(() => ConsultingDiagnosysArray);
            }
        }
        private DateTime? _gFromDate = Globals.GetCurServerDateTime().AddDays(-15);
        public DateTime? gFromDate
        {
            get
            {
                return _gFromDate;
            }
            set
            {
                if (_gFromDate == value) return;
                _gFromDate = value;
                NotifyOfPropertyChange(() => gFromDate);
            }
        }
        private DateTime? _gToDate = Globals.GetCurServerDateTime();
        public DateTime? gToDate
        {
            get
            {
                return _gToDate;
            }
            set
            {
                if (_gToDate == value) return;
                _gToDate = value;
                NotifyOfPropertyChange(() => gToDate);
            }
        }
        private bool _IsWaitOnly;
        public bool IsWaitOnly
        {
            get { return _IsWaitOnly; }
            set
            {
                _IsWaitOnly = value;
                NotifyOfPropertyChange(() => IsWaitOnly);
            }
        }
        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        #endregion
        #region Events
        public void btnSearch()
        {
            ConsultingDiagnosysSearchCriteria mSearchCriteria = CreateSearchCriteria();
            if (mSearchCriteria == null) return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetReportConsultingDiagnosys(mSearchCriteria,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalItemCount;
                                ConsultingDiagnosysArray = contract.EndGetReportConsultingDiagnosys(out TotalItemCount, asyncResult);
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnExport()
        {
            ConsultingDiagnosysSearchCriteria mSearchCriteria = CreateSearchCriteria();
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = ".xls", Filter = "Excel xls (*.xls)|*.xls", FilterIndex = 1 };
            if (objSFD.ShowDialog() != true)
                return;
            this.ShowBusyIndicator();
            ReportParameters RptParameters = new ReportParameters { ConsultingDiagnosysSearchCriteria = mSearchCriteria };
            RptParameters.ReportType = ReportType.BAOCAO_TONGHOP;
            RptParameters.Show = "CONSULTINGDIAGNOSYSHISTORY";
            RptParameters.reportName = ReportName.CONSULTINGDIAGNOSYSHISTORY;
            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            this.HideBusyIndicator();
        }
        public void EditCmd_Click(RoutedEventArgs e)
        {
            if (gSelectedConsultingDiagnosys != null)
            { 
                Action<IConsultingDiagnosys> onInitDlg = (proAlloc) =>
                {
                    /*▼====: #002*/
                    proAlloc.VisibilitySearch = Visibility.Collapsed;
                    /*▲====: #002*/
                    Globals.EventAggregator.Publish(new ConsultingDiagnosys<ConsultingDiagnosys> { Item = gSelectedConsultingDiagnosys });
                };
                GlobalsNAV.ShowDialog<IConsultingDiagnosys>(onInitDlg);
            }
        }
        #endregion
        #region Methods
        private void InitPatientFilterArray()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_InCompleteFileList, (int)ePermission.mView))
            {
                PatientFilterArray.Remove(PatientFilterArray.Where(x => x.Code == 1).First());
            }
            if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_AppliedList, (int)ePermission.mView))
            {
                PatientFilterArray.Remove(PatientFilterArray.Where(x => x.Code == 2).First());
            }
            if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                       , (int)eConsultation.mConsultingDiagnosy
                                       , (int)oConsultationEx.mConsultingDiagnosys_OperatedList, (int)ePermission.mView))
            {
                PatientFilterArray.Remove(PatientFilterArray.Where(x => x.Code == 3).First());
            }
            if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_WaitForSurgeryList, (int)ePermission.mView))
            {
                PatientFilterArray.Remove(PatientFilterArray.Where(x => x.Code == 4).First());
            }
            if (!Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mConsultingDiagnosy
                                               , (int)oConsultationEx.mConsultingDiagnosys_DuraGraftList, (int)ePermission.mView))
            {
                PatientFilterArray.Remove(PatientFilterArray.Where(x => x.Code == 9).First());
            }
        }
        private ConsultingDiagnosysSearchCriteria CreateSearchCriteria()
        {
            if (SelectedPatientFilter == null) return null;
            if (IsWaitOnly)
            {
                SelectedPatientFilter = gAllReportFilters.Where(x => x.Code == 3).FirstOrDefault();
                SelectedReportFilterArray = gAllReportFilters.Where(x => x.Code == 10).FirstOrDefault();
            }
            ConsultingDiagnosysSearchCriteria mSearchCriteria = new ConsultingDiagnosysSearchCriteria();
            mSearchCriteria.IsConsultingHistoryView = true;
            if (SelectedPatientFilter.Code == 1)
            {
                mSearchCriteria.IsApproved = false;
                if (SelectedReportFilterArray.Code == 6)
                {
                    mSearchCriteria.IsLated = true;
                }
                if (SelectedReportFilterArray.Code == 7)
                {
                    mSearchCriteria.IsLated = false;
                }
                mSearchCriteria.ConsultDiagRepType = 1;
            }
            if (SelectedPatientFilter.Code == 2)
            {
                mSearchCriteria.IsApproved = true;
                mSearchCriteria.ConsultDiagRepType = 2;
            }
            if (SelectedPatientFilter.Code == 3)
            {
                mSearchCriteria.IsApproved = true;
                if (SelectedReportFilterArray.Code == 8)
                {
                    mSearchCriteria.IsAllExamCompleted = true;
                }
                if (SelectedReportFilterArray.Code == 9)
                {
                    mSearchCriteria.IsAllExamCompleted = false;
                }
                if (SelectedReportFilterArray.Code == 10)
                {
                    mSearchCriteria.IsWaitSurgery = true;
                }
                mSearchCriteria.ConsultDiagRepType = 3;
            }
            if (SelectedPatientFilter.Code == 4)
            {
                mSearchCriteria.IsSurgeryCompleted = true;
                mSearchCriteria.ConsultDiagRepType = 4;
            }
            if (SelectedPatientFilter.Code == 5)
            {
                mSearchCriteria.IsDuraGraft = true;
                mSearchCriteria.ConsultDiagRepType = 5;
            }
            if (SelectedPatientFilter.Code == 11)
            {
                mSearchCriteria.IsCancelSurgery = true;
                mSearchCriteria.ConsultDiagRepType = 6;
            }
            mSearchCriteria.FromDate = gFromDate;
            mSearchCriteria.ToDate = gToDate;
            return mSearchCriteria;
        }
        /*▼====: #001*/
        public void grdConsultDiag_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        /*▲====: #001*/
        #endregion
    }
    public class ConsultingDiagnosysReportFilter
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public int ParentCode { get; set; }
    }
}
