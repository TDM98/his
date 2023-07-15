using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using eHCMSLanguage;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IMedicalFileCheckInHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalFileCheckInHistoryViewModel : Conductor<object>, IMedicalFileCheckInHistory
    {
        #region Properties
        private DateTime? _StartDate;
        public DateTime? StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        private DateTime? _EndDate;
        public DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        private PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> _AllPatientMedicalFileStorageCheckOut;
        public PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> AllPatientMedicalFileStorageCheckOut
        {
            get
            {
                return _AllPatientMedicalFileStorageCheckOut;
            }
            set
            {
                _AllPatientMedicalFileStorageCheckOut = value;
                NotifyOfPropertyChange(() => AllPatientMedicalFileStorageCheckOut);
            }
        }
        private ObservableCollection<Staff> _AllStaff;
        public ObservableCollection<Staff> AllStaff
        {
            get { return _AllStaff; }
            set
            {
                if (_AllStaff == value)
                    return;
                _AllStaff = value;
                NotifyOfPropertyChange(() => AllStaff);
            }
        }
        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get { return _SelectedStaff; }
            set
            {
                _SelectedStaff = value;
                NotifyOfPropertyChange(() => SelectedStaff);
            }
        }
        private ObservableCollection<Staff> _AllIssueStaff;
        public ObservableCollection<Staff> AllIssueStaff
        {
            get { return _AllIssueStaff; }
            set
            {
                _AllIssueStaff = value;
                NotifyOfPropertyChange(() => AllIssueStaff);
            }
        }
        private Staff _SelectedIssueStaff;
        public Staff SelectedIssueStaff
        {
            get { return _SelectedIssueStaff; }
            set
            {
                _SelectedIssueStaff = value;
                NotifyOfPropertyChange(() => SelectedIssueStaff);
            }
        }
        private string _FileCodeNumber;
        public string FileCodeNumber
        {
            get { return _FileCodeNumber; }
            set
            {
                _FileCodeNumber = value;
                NotifyOfPropertyChange(() => FileCodeNumber);
            }
        }
        ComboBox cboStatus;
        private int PageSize = 20;
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedicalFileCheckInHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            AllPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
            AllPatientMedicalFileStorageCheckOut.OnRefresh += AllPatientMedicalFileStorageCheckOut_OnRefresh;
            AllPatientMedicalFileStorageCheckOut.PageSize = PageSize;
            AllPatientMedicalFileStorageCheckOut.PageIndex = 0;
            GetAllStaffPerson();
        }
        public void btnSearch()
        {
            LoadMedicalFiles();
        }
        public void btnPrint()
        {
            if (AllPatientMedicalFileStorageCheckOut.Count() == 0)
            {
                MessageBox.Show("Không có đăng ký để in!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationXMLFromMedicalFileList(AllPatientMedicalFileStorageCheckOut.ToList(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var RegistrationXML = contract.EndGetRegistrationXMLFromMedicalFileList(asyncResult);

                                //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
                                //mPrintView.eItem = ReportName.MEDICALFILECHECKOUTHISTORY;
                                //mPrintView.FromDate = StartDate;
                                //mPrintView.ToDate = EndDate;
                                //mPrintView.FileCodeNumber = RegistrationXML;
                                //var mInstance = mPrintView as Conductor<object>;
                                //Globals.ShowDialog(mInstance, (o) => { });

                                Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                {
                                    mPrintView.eItem = ReportName.MEDICALFILECHECKOUTHISTORY;
                                    mPrintView.FromDate = StartDate;
                                    mPrintView.ToDate = EndDate;
                                    mPrintView.FileCodeNumber = RegistrationXML;
                                };
                                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

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
        public void gvMedicalFileHistory_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DateTime mCurrentDate = Globals.GetCurServerDateTime();
            PatientMedicalFileStorageCheckInCheckOut objRows = e.Row.DataContext as PatientMedicalFileStorageCheckInCheckOut;
            if (objRows.CheckoutDate == null)
                return;
            if (((mCurrentDate - objRows.CheckoutDate.Value).TotalDays > Globals.ServerConfigSection.CommonItems.BorrowTimeLimit / 24) && objRows.CheckinDate == null)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
                return;
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
                return;
            }
        }
        public void dtpStartFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as DatePicker).SelectedDate == null)
                StartDate = null;
        }
        private void AllPatientMedicalFileStorageCheckOut_OnRefresh(object sender, RefreshEventArgs e)
        {
            LoadMedicalFiles();
        }
        public void cboStatus_Loaded(object sender, RoutedEventArgs e)
        {
            cboStatus = sender as ComboBox;
        }
        #endregion
        #region Method
        private void LoadMedicalFiles()
        {
            //15072018 TTM 
            //kiểm tra xem cboStatus có null không nếu null thì không làm gì
            if (cboStatus != null)
            {
                this.ShowBusyIndicator();
                int mStatus = Convert.ToInt32((cboStatus.SelectedItem as ComboBoxItem).Tag);
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ClinicManagementServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginSearchMedicalFilesHistory(StartDate, EndDate, FileCodeNumber == null ? "" : FileCodeNumber, SelectedIssueStaff == null ? null : (long?)SelectedIssueStaff.StaffID, SelectedStaff == null ? null : (long?)SelectedStaff.StaffID, mStatus, PageSize, 1,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                int TotalRow = 0;
                                var AllItems = contract.EndSearchMedicalFilesHistory(out TotalRow, asyncResult);
                                AllPatientMedicalFileStorageCheckOut.Clear();
                                AllPatientMedicalFileStorageCheckOut.TotalItemCount = TotalRow;
                                AllPatientMedicalFileStorageCheckOut.PageSize = PageSize;
                                AllPatientMedicalFileStorageCheckOut.PageIndex = 0;
                                foreach (var item in AllItems)
                                {
                                    AllPatientMedicalFileStorageCheckOut.Add(item);
                                }
                            }), null);
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
                });
                t.Start();
            }
        }
        private void GetAllStaffPerson()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffs(Globals.DispatchCallback((asyncResult) =>
                        {
                            AllStaff = new ObservableCollection<Staff>(contract.EndGetAllStaffs(asyncResult));
                            AllIssueStaff = new ObservableCollection<Staff>(AllStaff.Where(x => x.StaffCatgID == Globals.ServerConfigSection.ClinicDeptElements.FileEmployeeID));
                            SelectedIssueStaff = AllIssueStaff.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
                            this.HideBusyIndicator();
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
        #endregion
    }
}
