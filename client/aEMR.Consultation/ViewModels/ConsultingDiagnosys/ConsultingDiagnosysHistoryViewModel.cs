using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using Caliburn.Micro;

/*
 * 20180915 #001 TBL: Khi tick vao Huy mo thi Ngay mo se an va khong duoc nhap
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultingDiagnosysHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ConsultingDiagnosysHistoryViewModel : ViewModelBase, IConsultingDiagnosysHistory
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        int gLoadedIndex = 3;

        [ImportingConstructor]
        public ConsultingDiagnosysHistoryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadCharityOrganizations();
            LoadSurgerySchedules();
            LoadSugeryLocations();
        }
        #region Method
        public void InitDataView()
        {
            if (gLoadedIndex <= 0)
            {
                if (gConsultingDiagnosys.DoctorStaff != null)
                    gConsultingDiagnosys.DoctorStaff = DoctorStaffs.Where(x => x.StaffID == gConsultingDiagnosys.DoctorStaff.StaffID).FirstOrDefault();
                if (gConsultingDiagnosys.SupportCharityOrganization != null && gConsultingDiagnosys.SupportCharityOrganization.CharityOrgID > 0)
                    gConsultingDiagnosys.SupportCharityOrganization = gCharityOrganizationCollection.Where(x => x.CharityOrgID == gConsultingDiagnosys.SupportCharityOrganization.CharityOrgID).FirstOrDefault();
                if (gConsultingDiagnosys.SurgeryScheduleDetail != null && gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetailID > 0)
                {
                    SelectedSurgerySchedule = SurgeryScheduleCollection.Where(x => x.SurgeryScheduleID == gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleID).FirstOrDefault();
                    gSelectedSugeryLocation = gSugeryLocationCollection.Where(x => x.DeptLocationID == gConsultingDiagnosys.SurgeryScheduleDetail.SSD_Room).FirstOrDefault();
                    gSelectedSegment = gSegmentCollection.Where(x => x.LookupID == gConsultingDiagnosys.SurgeryScheduleDetail.OpSeqNum).FirstOrDefault();
                    ExpSurgeryDate = gConsultingDiagnosys.SurgeryScheduleDetail.SSD_Date;
                    LoadSurgerySchedule_TeamMembers();
                }
                gSelectedSegment = gSegmentCollection.FirstOrDefault();
            }
        }
        private void ReloadInitDataView()
        {
            gLoadedIndex--;
            InitDataView();
        }
        private void LoadCharityOrganizations()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCharityOrganization(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                gCharityOrganizationCollection = new ObservableCollection<CharityOrganization>(contract.EndGetAllCharityOrganization(asyncResult));
                                ReloadInitDataView();
                            }
                            catch (Exception ex)
                            {
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadSurgerySchedules()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSurgerySchedules(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                SurgeryScheduleCollection = new ObservableCollection<SurgerySchedule>(contract.EndGetSurgerySchedules(asyncResult));
                                if (SurgeryScheduleCollection == null)
                                    SurgeryScheduleCollection = new ObservableCollection<SurgerySchedule>();
                                ReloadInitDataView();
                            }
                            catch (Exception ex)
                            {
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadSugeryLocations()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLocationsByDeptID(Globals.ServerConfigSection.Hospitals.SurgeryDeptID,null, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                gSugeryLocationCollection = new ObservableCollection<DeptLocation>(contract.EndGetAllLocationsByDeptID(asyncResult));
                                if (gSugeryLocationCollection == null)
                                    gSugeryLocationCollection = new ObservableCollection<DeptLocation>();
                                gSelectedSugeryLocation = gSugeryLocationCollection.FirstOrDefault();
                                ReloadInitDataView();
                            }
                            catch (Exception ex)
                            {
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadSurgerySchedule_TeamMembers()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSurgeryScheduleDetail_TeamMembers(gConsultingDiagnosys.SurgeryScheduleDetail.ConsultingDiagnosysID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember = contract.EndGetSurgeryScheduleDetail_TeamMembers(asyncResult);
                                if (gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember != null && gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember.Count > 0)
                                {
                                    if (SurgeryDoctorCollection == null) SurgeryDoctorCollection = new ObservableCollection<Staff>();
                                    SurgeryDoctorCollection.Clear();
                                    foreach (var item in gConsultingDiagnosys.SurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember)
                                    {
                                        SurgeryDoctorCollection.Add(DoctorStaffs.Where(x => x.StaffID == item.StaffID).FirstOrDefault());
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public SurgeryScheduleDetail GetSurgeryScheduleDetail()
        {
            if (SelectedSurgerySchedule == null || SelectedSurgerySchedule.SurgeryScheduleID == 0) return null;
            if (ExpSurgeryDate == null) throw new Exception(eHCMSResources.Z2179_G1_VuiLongNhapNgayMoDuKien);
            SurgeryScheduleDetail mSurgeryScheduleDetail = new SurgeryScheduleDetail
            {
                SurgeryScheduleID = SelectedSurgerySchedule.SurgeryScheduleID,
                ConsultingDiagnosysID = gConsultingDiagnosys.ConsultingDiagnosysID,
                SSD_Date = ExpSurgeryDate.Value,
                SSD_Room = gSelectedSugeryLocation == null ? 0 : gSelectedSugeryLocation.DeptLocationID,
                OpSeqNum = Convert.ToInt16(gSelectedSegment.LookupID)
            };
            if (mSurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember == null) mSurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember = new List<SurgeryScheduleDetail_TeamMember>();
            if (SurgeryDoctorCollection != null && SurgeryDoctorCollection.Count > 0)
            {
                foreach (Staff item in SurgeryDoctorCollection)
                {
                    mSurgeryScheduleDetail.SurgeryScheduleDetail_TeamMember.Add(new SurgeryScheduleDetail_TeamMember
                    {
                        StaffID = item.StaffID
                    });
                }
            }
            return mSurgeryScheduleDetail;
        }
        #endregion
        #region Properties
        private ConsultingDiagnosys _ConsultingDiagnosys;
        public ConsultingDiagnosys gConsultingDiagnosys
        {
            get
            {
                return _ConsultingDiagnosys;
            }
            set
            {
                _ConsultingDiagnosys = value;
                /*▼====: #001*/
                if (_ConsultingDiagnosys.IsCancelSurgery)
                {
                    IsEnabledSurgeryDate = false;
                    IsEnabledReasonCancelSurgery = true;
                }
                else
                {
                    IsEnabledSurgeryDate = true;
                    IsEnabledReasonCancelSurgery = false;
                }
                /*▲====: #001*/
                NotifyOfPropertyChange(() => gConsultingDiagnosys);
            }
        }
        private List<Lookup> _DischargeTypeArray;
        public List<Lookup> DischargeTypeArray
        {
            get
            {
                return _DischargeTypeArray;
            }
            set
            {
                _DischargeTypeArray = value;
                NotifyOfPropertyChange(() => DischargeTypeArray);
            }
        }
        private List<Staff> _DoctorStaffs;
        public List<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                _DoctorStaffs = value;
                NotifyOfPropertyChange(() => DoctorStaffs);
            }
        }
        private ObservableCollection<CharityOrganization> _gCharityOrganizationCollection;
        public ObservableCollection<CharityOrganization> gCharityOrganizationCollection
        {
            get
            {
                return _gCharityOrganizationCollection;
            }
            set
            {
                if (_gCharityOrganizationCollection == value) return;
                _gCharityOrganizationCollection = value;
                NotifyOfPropertyChange(() => gCharityOrganizationCollection);
            }
        }
        private Staff _SelectedSurgeryDoctor;
        public Staff SelectedSurgeryDoctor
        {
            get
            {
                return _SelectedSurgeryDoctor;
            }
            set
            {
                if (_SelectedSurgeryDoctor == value) return;
                _SelectedSurgeryDoctor = value;
                NotifyOfPropertyChange(() => SelectedSurgeryDoctor);
            }
        }
        private ObservableCollection<Staff> _SurgeryDoctorCollection;
        public ObservableCollection<Staff> SurgeryDoctorCollection
        {
            get
            {
                return _SurgeryDoctorCollection;
            }
            set
            {
                if (_SurgeryDoctorCollection == value) return;
                _SurgeryDoctorCollection = value;
                NotifyOfPropertyChange(() => SurgeryDoctorCollection);
            }
        }
        private Staff _CurrentSurgeryDoctor;
        public Staff CurrentSurgeryDoctor
        {
            get
            {
                return _CurrentSurgeryDoctor;
            }
            set
            {
                if (_CurrentSurgeryDoctor == value) return;
                _CurrentSurgeryDoctor = value;
                NotifyOfPropertyChange(() => CurrentSurgeryDoctor);
            }
        }
        private SurgerySchedule _SelectedSurgerySchedule;
        public SurgerySchedule SelectedSurgerySchedule
        {
            get { return _SelectedSurgerySchedule; }
            set
            {
                if (_SelectedSurgerySchedule == value) return;
                _SelectedSurgerySchedule = value;
                NotifyOfPropertyChange(() => SelectedSurgerySchedule);
            }
        }
        private ObservableCollection<SurgerySchedule> _SurgeryScheduleCollection;
        public ObservableCollection<SurgerySchedule> SurgeryScheduleCollection
        {
            get { return _SurgeryScheduleCollection; }
            set
            {
                if (_SurgeryScheduleCollection == value) return;
                _SurgeryScheduleCollection = value;
                NotifyOfPropertyChange(() => SurgeryScheduleCollection);
            }
        }
        private ObservableCollection<DeptLocation> _gSugeryLocationCollection;
        public ObservableCollection<DeptLocation> gSugeryLocationCollection
        {
            get { return _gSugeryLocationCollection; }
            set
            {
                if (_gSugeryLocationCollection == value) return;
                _gSugeryLocationCollection = value;
                NotifyOfPropertyChange(() => gSugeryLocationCollection);
            }
        }
        private DeptLocation _gSelectedSugeryLocation;
        public DeptLocation gSelectedSugeryLocation
        {
            get { return _gSelectedSugeryLocation; }
            set
            {
                if (_gSelectedSugeryLocation == value) return;
                _gSelectedSugeryLocation = value;
                NotifyOfPropertyChange(() => gSelectedSugeryLocation);
            }
        }
        private DateTime? _ExpSurgeryDate;
        public DateTime? ExpSurgeryDate
        {
            get { return _ExpSurgeryDate; }
            set
            {
                if (_ExpSurgeryDate == value) return;
                if (value != null && (SelectedSurgerySchedule == null || !(value.Value.Date >= SelectedSurgerySchedule.SSFromDate.Date && value.Value.Date <= SelectedSurgerySchedule.SSToDate.Date)))
                {
                    throw new Exception(eHCMSResources.A0793_G1_Msg_InfoNgKhHopLe);
                }
                _ExpSurgeryDate = value;
                NotifyOfPropertyChange(() => ExpSurgeryDate);
            }
        }
        private ObservableCollection<Lookup> _gSegmentCollection = new ObservableCollection<Lookup> {new Lookup { LookupID = 1, ObjectName = "Ca I" }, new Lookup { LookupID = 2, ObjectName = "Ca II" }};
        public ObservableCollection<Lookup> gSegmentCollection
        {
            get { return _gSegmentCollection; }
            set
            {
                _gSegmentCollection = value;
                NotifyOfPropertyChange(() => gSegmentCollection);
            }
        }
        private Lookup _gSelectedSegment;
        public Lookup gSelectedSegment
        {
            get { return _gSelectedSegment; }
            set
            {
                if (_gSelectedSegment == value) return;
                _gSelectedSegment = value;
                NotifyOfPropertyChange(() => gSelectedSegment);
            }
        }
        /*▼====: #001*/
        private bool _IsEnabledSurgeryDate = true;
        public bool IsEnabledSurgeryDate
        {
            get
            {
                return _IsEnabledSurgeryDate;
            }
            set
            {
                if (_IsEnabledSurgeryDate != value)
                {
                    _IsEnabledSurgeryDate = value;
                    NotifyOfPropertyChange(() => IsEnabledSurgeryDate);
                }
            }
        }
        /*▲====: #001*/
        private bool _IsEnabledReasonCancelSurgery = true;
        public bool IsEnabledReasonCancelSurgery
        {
            get
            {
                return _IsEnabledReasonCancelSurgery;
            }
            set
            {
                if (_IsEnabledReasonCancelSurgery != value)
                {
                    _IsEnabledReasonCancelSurgery = value;
                    NotifyOfPropertyChange(() => IsEnabledReasonCancelSurgery);
                }
            }
        }
        #endregion
        #region Events
        public void cboDoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboDoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gConsultingDiagnosys.DoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void cboCharityOrganization_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<CharityOrganization>(gCharityOrganizationCollection.Where(x => x.CharityOrgName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboCharityOrganization_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gConsultingDiagnosys.SupportCharityOrganization = ((AutoCompleteBox)sender).SelectedItem as CharityOrganization;
        }
        public void btnAddSugeryDoctor()
        {
            if (SurgeryDoctorCollection == null) SurgeryDoctorCollection = new ObservableCollection<Staff>();
            if (!SurgeryDoctorCollection.Contains(SelectedSurgeryDoctor))
            {
                SurgeryDoctorCollection.Add(SelectedSurgeryDoctor);
            }
            else
                MessageBox.Show(eHCMSResources.T1987_G1_DaTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            NotifyOfPropertyChange(() => SurgeryDoctorCollection);
        }
        public void cboSurgeryDoctor_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboSurgeryDoctor_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedSurgeryDoctor = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void DeleteCmd_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSurgeryDoctor != null)
            {
                SurgeryDoctorCollection.Remove(CurrentSurgeryDoctor);
            }
        }
        public void cboSurgerySchedule_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<SurgerySchedule>(SurgeryScheduleCollection.Where(x => x.SSName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void cboSurgerySchedule_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedSurgerySchedule = ((AutoCompleteBox)sender).SelectedItem as SurgerySchedule;
            ExpSurgeryDate = null;
        }
        public void btnAddSurgerySchedule()
        {
            Action<ISurgeryScheduleEdit> onInitDlg = (proAlloc) => {};
            Action<Common.Enums.MsgBoxOptions, IScreen> onCallbackDig = (m, s) =>
            {
                LoadSurgerySchedules();
            };
            GlobalsNAV.ShowDialog<ISurgeryScheduleEdit>(onInitDlg, onCallbackDig);
        }
        /*▼====: #001*/
        private DateTime? tmpSurgeryDate;
        public void chkIsCancelSurgery_Check(object sender, RoutedEventArgs e)
        {
            tmpSurgeryDate = gConsultingDiagnosys.SurgeryDate;
            IsEnabledSurgeryDate = false;
            gConsultingDiagnosys.SurgeryDate = null;
            IsEnabledReasonCancelSurgery = true;
        }

        public void chkIsCancelSurgery_UnCheck(object sender, RoutedEventArgs e)
        {
            gConsultingDiagnosys.SurgeryDate = tmpSurgeryDate;
            IsEnabledSurgeryDate = true;
            IsEnabledReasonCancelSurgery = false;
        }
        /*▲====: #001*/
        #endregion
    }
}
