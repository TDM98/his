using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Common.Collections;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.Windows;
using eHCMSLanguage;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IMedicalFileCheckOutHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedicalFileCheckOutHistoryViewModel : Conductor<object>, IMedicalFileCheckOutHistory
    {
        #region Properties
        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }
        public string BusyMessage { get { return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0995_G1_HThongDangXuLi); } }
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
        private ObservableCollection<Staff> _AllStaffContext;
        public ObservableCollection<Staff> AllStaffContext
        {
            get { return _AllStaffContext; }
            set
            {
                if (_AllStaffContext == value)
                    return;
                _AllStaffContext = value;
                NotifyOfPropertyChange(() => AllStaffContext);
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
        private ObservableCollection<Staff> _AllIssueStaffContext;
        public ObservableCollection<Staff> AllIssueStaffContext
        {
            get { return _AllIssueStaffContext; }
            set
            {
                _AllIssueStaffContext = value;
                NotifyOfPropertyChange(() => AllIssueStaffContext);
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
        private PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> _AllPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
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
        private int PageSize = 20;
        private PatientMedicalFileStorageCheckInCheckOut _SelectedFileStorage;
        public PatientMedicalFileStorageCheckInCheckOut SelectedFileStorage
        {
            get
            {
                return _SelectedFileStorage;
            }
            set
            {
                _SelectedFileStorage = value;
                NotifyOfPropertyChange(() => SelectedFileStorage);
            }
        }
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedicalFileCheckOutHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            GetAllStaffPerson();
        }
        public void btnSearch()
        {
            IsBusy = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetMedicalFileStorageCheckOutHistory(StartDate, EndDate, SelectedIssueStaff == null ? null : (long?)SelectedIssueStaff.StaffID, SelectedStaff == null ? null : (long?)SelectedStaff.StaffID, PageSize, 1,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            int TotalRow = 0;
                            var AllItems = contract.EndGetMedicalFileStorageCheckOutHistory(out TotalRow, asyncResult);
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
                    IsBusy = false;
                }
            });
            t.Start();
        }
        public void btnPrint()
        {
            if (SelectedFileStorage == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng nhập để in!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                return;
            }

            IsBusy = true;

            //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
            //mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
            //mPrintView.FromDate = StartDate;
            //mPrintView.ToDate = EndDate;
            //mPrintView.RegistrationID = SelectedFileStorage.PtRegistrationID;
            //mPrintView.IssueName = SelectedFileStorage.FullName;
            //mPrintView.ReceiveName = SelectedFileStorage.BorrowBy;
            //mPrintView.AdmissionDate = SelectedFileStorage.CheckoutDate.Value.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
            //var mInstance = mPrintView as Conductor<object>;
            //Globals.ShowDialog(mInstance, (o) => { });

            Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
            {
                mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                mPrintView.FromDate = StartDate;
                mPrintView.ToDate = EndDate;
                mPrintView.RegistrationID = SelectedFileStorage.PtRegistrationID;
                mPrintView.IssueName = SelectedFileStorage.FullName;
                mPrintView.ReceiveName = SelectedFileStorage.BorrowBy;
                mPrintView.AdmissionDate = SelectedFileStorage.CheckoutDate.Value.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);





            IsBusy = false;
        }
        public void cboBorrowBy_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            AllStaffContext = new ObservableCollection<Staff>(AllStaff.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllStaffContext;
            cboContext.PopulateComplete();
        }
        public void cboBorrowBy_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void cboIssueStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            AllIssueStaffContext = new ObservableCollection<Staff>(AllIssueStaff.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllIssueStaffContext;
            cboContext.PopulateComplete();
        }
        public void cboIssueStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedIssueStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        #endregion
        #region Method
        private void GetAllStaffPerson()
        {
            IsBusy = true;
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
                            IsBusy = false;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    IsBusy = false;
                }
            });
            t.Start();
        }
        #endregion
    }
}
